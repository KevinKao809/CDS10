using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json.Linq;
using IoTHubEventProcessor.Utilities;
using IoTHubEventProcessor.Models;
using Newtonsoft.Json;
using Microsoft.Azure.Documents;
using IoTHubEventProcessor.Helper;
using System.Web.Script.Serialization;

namespace IoTHubEventProcessor
{
    public class SfMessageEventProcessor : IEventProcessor
    {
        private MessageProcessorFactoryModel _msgProcessorFactoryModel;
        static RealTimeMessageSender _RTMessageSender = new RealTimeMessageSender();
        const string MESSAGE_PROPERTY_MESSAGECATALOGID = "MessageCatalogId";
        const string MESSAGE_PROPERTY_TYPE = "Type";
        const string MESSAGE_PROPERTY_TYPE_COMMAND = "Command";
        const string MESSAGE_PROPERTY_TYPE_MESSAGE = "Message";
        const string SF_UPDATED_TYPE = "SF_UpdatedType";
        const string SF_LASTUPDATED_TIMESTAMP = "SF_LastUpdatedTimestamp";

        public SfMessageEventProcessor(MessageProcessorFactoryModel msgProcessorFactoryModel)
        {
            this._msgProcessorFactoryModel = msgProcessorFactoryModel;
        }

        Stopwatch checkpointStopWatch;
        async Task IEventProcessor.CloseAsync(PartitionContext context, CloseReason reason)
        {
            ConsoleLog.WriteToConsole("IoTHubEventProcessor Shutting Down. Partition '{0}', Reason: '{1}'.", context.Lease.PartitionId, reason);
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

        Task IEventProcessor.OpenAsync(PartitionContext context)
        {
            ConsoleLog.WriteToConsole("IoTHubEventProcessor initialized.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset);
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        async Task IEventProcessor.ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (EventData eventData in messages)
            {
                try
                {
                    string data = Encoding.UTF8.GetString(eventData.GetBytes());
                    JObject payload = JObject.Parse(data);

                    string deviceId = getDeviceId(eventData);
                    if (deviceId == null)
                        continue;

                    if (isTwinChangedEvent(eventData))
                    {
                        processTwinChangedEvent(payload);
                    }
                    else
                    {
                        string messageType = getMessageProperty(eventData, MESSAGE_PROPERTY_TYPE);
                        if (messageType != null)
                        {
                            // New version of message processing ( + device management)
                            switch (messageType)
                            {
                                case MESSAGE_PROPERTY_TYPE_COMMAND:
                                    //ConsoleLog.WriteToConsole("MESSAGE_PROPERTY_TYPE_COMMAND");
                                    sendCommandToServiceBusQueue(_msgProcessorFactoryModel, deviceId, payload);
                                    break;
                                case MESSAGE_PROPERTY_TYPE_MESSAGE:
                                    //ConsoleLog.WriteToConsole("MESSAGE_PROPERTY_TYPE_MESSAGE");
                                    await processMessages(deviceId,
                                        getMessageCatalogIdInMessageProperty(eventData),
                                        payload);
                                    break;
                            }
                        }
                        else
                        {
                            // Compatible for old version
                            await processMessages(deviceId,
                                        getMessageCatalogIdInMessageProperty(eventData),
                                        payload);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ConsoleLog.WriteToConsole("(Skipped) Unsupported Message:{0}", ex.Message.ToString());
                    ConsoleLog.WriteBlobLogError("(Skipped) Unsupported Message:{0}", ex.Message.ToString());
                }
            }

            //Call checkpoint every 5 minutes, so that worker can resume processing from 5 minutes back if it restarts.
            if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
            {
                await context.CheckpointAsync();
                this.checkpointStopWatch.Restart();
            }
        }

        private void processTwinChangedEvent(JObject payload)
        {
            //ConsoleLog.WriteToConsole("(twinChangedEvent) payload:{0}", payload.ToString());
            var desired = payload["properties"]["desired"];
            var reported = payload["properties"]["reported"];

            if (desired != null)
            {
                //ConsoleLog.WriteToConsole("---- desired:{0}", desired.ToString());
            }

            if (reported != null)
            {
                //ConsoleLog.WriteToConsole("---- reported:{0}", reported.ToString());
                var systemConfig = reported["SF_SystemConfig"];
                var customizedConfig = reported["SF_CustomizedConfig"];

                if (systemConfig != null)
                    ConsoleLog.WriteToConsole("---- reported: System Config was updated");

                if (customizedConfig != null)
                    ConsoleLog.WriteToConsole("---- reported: Customized Config was updated");
            }

            // to do something here




        }

        private bool isTwinChangedEvent(EventData eventData)
        {
            bool bTwinChanged = false;
            try
            {
                if ("twinChangeEvents" == getMessageSystemProperty(eventData, "iothub-message-source"))
                    bTwinChanged = true;
            }
            catch
            {
                bTwinChanged = false;
            }

            return bTwinChanged;
        }

        private async Task processMessages(string deviceId, int messageCatalogId, JObject payload)
        {
            if (vaildateDeviceMessageCatalog(deviceId, messageCatalogId))
            {
                /* Feed the message to Web API */
                feedInMessage(messageCatalogId, (JObject)payload.DeepClone());                

                /* Put the message to DocumentDB (Hot Data) */
                string messageDocumentId = await putMessageDocument(deviceId, messageCatalogId, payload);

                /* Put the message to Blob Storage (Cold Data) */
                putMessageBlobStorage(messageDocumentId, deviceId, payload);

                /* Run the alarm rules by message id */
                await runAlarmRules(_msgProcessorFactoryModel, messageCatalogId, payload, messageDocumentId);
            }
            else
            {
                ConsoleLog.WriteBlobLogWarn("(Ignored) [{0}] Unsupported Message ID: {1}", deviceId, messageCatalogId);
                ConsoleLog.WriteToConsole("(Ignored) [{0}] Unsupported Message ID: {1}", deviceId, messageCatalogId);
            }
        }

        private int getMessageCatalogIdInMessageProperty(EventData eventData)
        {
            try
            {
                return Int32.Parse(getMessageProperty(eventData, MESSAGE_PROPERTY_MESSAGECATALOGID));
            }
            catch
            {
                ConsoleLog.WriteBlobLogWarn("MessageCatalogId Property was not found!");
                ConsoleLog.WriteToConsole("MessageCatalogId Property was not found!");
                throw;
            }

        }

        public async Task<bool> RunAlarmRulesTest(MessageProcessorFactoryModel msgProcessorFactoryModel, int messageCatalogId, JObject payload,
            bool putToDocuDB, bool sendToSB)
        {
            try
            {
                bool alarmSent = false;
                var ts = payload["msgTimestamp"];
                DateTime msgTimestamp = DateTime.Parse(ts.ToString());

                Dictionary<int, List<AlarmRuleCatalogEngine>> messageIdAlarmRules = msgProcessorFactoryModel.MessageIdAlarmRules;
                if (messageIdAlarmRules.ContainsKey(messageCatalogId) == true)
                {
                    List<AlarmRuleCatalogEngine> alarmRuleCatalogEngine = messageIdAlarmRules[messageCatalogId];

                    foreach (AlarmRuleCatalogEngine arcEngine in alarmRuleCatalogEngine)
                    {
                        if (arcEngine.RuleEngineItems.Count > 0)
                        {
                            // Get all results of equations
                            foreach (KeyValuePair<string, RuleEngineItem> ruleEngineItem in arcEngine.RuleEngineItems)
                            {
                                runSingleRuleItem(ruleEngineItem.Value, payload);
                            }

                            // Get the result of bitwise operation
                            bool alarmTriggered = compileBitWiseRules(arcEngine.RuleEngineItems.Count - 1, arcEngine.RuleEngineItems);
                            if (alarmTriggered)
                            {
                                string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                alarmSent = checkAlarmTimeWindow(arcEngine, msgTimestamp);

                                ConsoleLog.WriteMessageAlarmLogToConsole("alarmSent={0}, LastTriggerTime={1}", alarmSent, arcEngine.LastTriggerTime.ToString("yyyy-MM-ddTHH:mm:ss"));

                                AlarmMessageHelper alarmMessageHelper = new AlarmMessageHelper(arcEngine.AlarmRuleCatalog,
                                    now,
                                    alarmSent,
                                    "ABCDEFG-ABCDEFG-ABCDEFG-ABCDEFG-ABCDEFG-ABCDEFG",
                                    payload);

                                // Send the alarm to Service Bus if it matchs the time window
                                if (alarmSent && sendToSB)
                                    sendAlarmToServiceBusQueue(msgProcessorFactoryModel.SfQueueClient, alarmMessageHelper);

                                // Put the alarm to document DB
                                if (putToDocuDB)
                                    await putAlarmDocument(msgProcessorFactoryModel.SfDocumentDBHelper, alarmMessageHelper);
                            }

                        }
                    }
                }

                return alarmSent;
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteToConsole("runAlarmRules Exception: {0}", ex.ToString());
                throw new Exception("runAlarmRules Exception");
            }
        }

        private async Task runAlarmRules(MessageProcessorFactoryModel msgProcessorFactoryModel, int messageCatalogId, JObject payload, string messageDocumentId)
        {
            try
            {
                var ts = payload["msgTimestamp"];
                DateTime msgTimestamp = DateTime.Parse(ts.ToString());

                Dictionary<int, List<AlarmRuleCatalogEngine>> messageIdAlarmRules = msgProcessorFactoryModel.MessageIdAlarmRules;
                if (messageIdAlarmRules.ContainsKey(messageCatalogId) == true)
                {
                    List<AlarmRuleCatalogEngine> alarmRuleCatalogEngine = messageIdAlarmRules[messageCatalogId];

                    foreach (AlarmRuleCatalogEngine arcEngine in alarmRuleCatalogEngine)
                    {
                        if (arcEngine.RuleEngineItems.Count > 0)
                        {
                            // Get all results of equations
                            foreach (KeyValuePair<string, RuleEngineItem> ruleEngineItem in arcEngine.RuleEngineItems)
                            {
                                runSingleRuleItem(ruleEngineItem.Value, payload);
                            }

                            // Get the result of bitwise operation
                            bool alarmTriggered = compileBitWiseRules(arcEngine.RuleEngineItems.Count - 1, arcEngine.RuleEngineItems);
                            if (alarmTriggered)
                            {
                                string now = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                                bool alarmSent = checkAlarmTimeWindow(arcEngine, msgTimestamp);

                                ConsoleLog.WriteMessageAlarmLogToConsoleInfo("AlarmRuleCatalogId={0}, alarmSent={1}, LastTriggerTime={2}",
                                    arcEngine.AlarmRuleCatalogId, alarmSent, arcEngine.LastTriggerTime.ToString("yyyy-MM-ddTHH:mm:ss"));

                                AlarmMessageHelper alarmMessageHelper = new AlarmMessageHelper(arcEngine.AlarmRuleCatalog,
                                    now,
                                    alarmSent,
                                    messageDocumentId,
                                    payload);

                                // Send the alarm to Service Bus if it matchs the time window
                                if (alarmSent)
                                    sendAlarmToServiceBusQueue(msgProcessorFactoryModel.SfQueueClient, alarmMessageHelper);

                                // Put the alarm to document DB
                                await putAlarmDocument(msgProcessorFactoryModel.SfDocumentDBHelper, alarmMessageHelper);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteToConsole("runAlarmRules Exception: {0}", ex.ToString());
                ConsoleLog.WriteBlobLogError("runAlarmRules Exception: {0}", ex.ToString());
                return;
            }
        }

        public void RunSingleRuleItemTest(RuleEngineItem ruleEngineItem, JObject payload)
        {
            runSingleRuleItem(ruleEngineItem, payload);
        }

        private void runSingleRuleItem(RuleEngineItem ruleEngineItem, JObject payload)
        {
            string elementName = ruleEngineItem.ElementName;

            try
            {
                var value = payload[elementName];

                DynamicMessageElement dm = new DynamicMessageElement();
                dm.Name = elementName;
                switch (ruleEngineItem.DataType)
                {
                    case SupportDataTypeEnum.Bool:
                        dm.StringValue = value.ToString().ToLower();
                        break;
                    case SupportDataTypeEnum.String:
                        dm.StringValue = (string)value;
                        if (string.IsNullOrEmpty(dm.StringValue))
                            dm.StringValue = "null";
                        break;
                    case SupportDataTypeEnum.Numeric:
                        dm.DecimalValue = (decimal)value;
                        break;
                    default:
                        throw new NotSupportedException();
                }

                if (ruleEngineItem.DataType == SupportDataTypeEnum.Numeric || ruleEngineItem.DataType == SupportDataTypeEnum.Bool)
                    ruleEngineItem.Result = ruleEngineItem.Equality(dm);
                else
                {
                    // SupportDataTypeEnum.String
                    bool equal = string.Equals(dm.StringValue, ruleEngineItem.StringRightValue);
                    if (ruleEngineItem.StringEqualOperation.Equals("="))
                        ruleEngineItem.Result = equal;
                    else if (ruleEngineItem.StringEqualOperation.Equals("!="))
                        ruleEngineItem.Result = !equal;
                    else
                        throw new ArgumentNullException("String equal operation is not supported - " + ruleEngineItem.StringEqualOperation);
                }
            }
            catch (Exception)
            {
                ruleEngineItem.Result = false;
            }
        }

        private bool checkAlarmTimeWindow(AlarmRuleCatalogEngine arcEngine, DateTime msgTimestamp)
        {
            if (arcEngine.Triggered)
            {
                if (arcEngine.AlarmRuleCatalog.KeepHappenInSec <= 0)
                {
                    arcEngine.LastTriggerTime = msgTimestamp;
                    return true;// Always triggered
                }
                else
                {
                    DateTime nextAcceptableTime = arcEngine.LastTriggerTime.AddSeconds(arcEngine.AlarmRuleCatalog.KeepHappenInSec);
                    int result = DateTime.Compare(msgTimestamp, nextAcceptableTime);
                    if (result > 0)
                    {
                        // the message timestamp is later than the next accpetable time
                        arcEngine.LastTriggerTime = msgTimestamp;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                arcEngine.Triggered = true;
                arcEngine.LastTriggerTime = msgTimestamp;
                return true;
            }
        }

        private void sendAlarmToServiceBusQueue(QueueClient queueClient, AlarmMessageHelper alarmDocuHelper)
        {
            try
            {
                var alarmMessage = new
                {
                    MessageCatalogId = alarmDocuHelper.MessageId,
                    AlarmRuleCatalogId = alarmDocuHelper.AlarmRuleId,
                    AlarmRuleCatalogName = alarmDocuHelper.AlarmRuleName,
                    AlarmRuleCatalogDescription = alarmDocuHelper.AlarmRuleDescription,
                    MessageDocumentId = alarmDocuHelper.MessageDocumentId,
                    TriggeredTime = alarmDocuHelper.TriggeredTime,
                    Message = alarmDocuHelper.Payload
                };

                var messageString = JsonConvert.SerializeObject(alarmMessage);
                var msg = new BrokeredMessage(messageString);
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteMessageAlarmErrorLogToConsole("sendAlarmToServiceBusQueue Exception: messageId={0}, alarmRuleId={1}, triggeredTime={2} payload={3} ex={4}",
                    alarmDocuHelper.MessageId, alarmDocuHelper.AlarmRuleId, alarmDocuHelper.TriggeredTime, alarmDocuHelper.Payload, ex.ToString());
                ConsoleLog.WriteBlobLogError("sendAlarmToServiceBusQueue Exception: messageId={0}, alarmRuleId={1}, triggeredTime={2} payload={3} ex={4}",
                    alarmDocuHelper.MessageId, alarmDocuHelper.AlarmRuleId, alarmDocuHelper.TriggeredTime, alarmDocuHelper.Payload, ex.ToString());
            }
        }

        private void sendCommandToServiceBusQueue(MessageProcessorFactoryModel messageProcessorFactoryModel, string deviceId, JObject command)
        {
            QueueClient queueClient = messageProcessorFactoryModel.SfInfraQueueClient;
            string iothubAlias = messageProcessorFactoryModel.IoTHubAlias;
            bool iothubIsPrimary = messageProcessorFactoryModel.IsIoTHubPrimary;

            try
            {
                JObject msgObj = new JObject();
                msgObj.Add("job", "device management");
                msgObj.Add("entity", "iotdevice");
                msgObj.Add("entityId", deviceId);
                msgObj.Add("task", "update device reported property to db");
                msgObj.Add("iothubDeviceId", deviceId);
                msgObj.Add("requester", "IoTHubReceiver_" + iothubAlias);
                msgObj.Add("requestDateTime", DateTime.UtcNow);
                msgObj.Add("iothubAlias", iothubAlias);
                msgObj.Add("iothubIsPrimary", iothubIsPrimary);

                JObject payload = new JObject();
                payload.Add(SF_LASTUPDATED_TIMESTAMP, command[SF_LASTUPDATED_TIMESTAMP]);
                msgObj.Add("deviceConfiguration", payload);

                var messageString = JsonConvert.SerializeObject(msgObj);
                var msg = new BrokeredMessage(messageString);
                queueClient.Send(msg);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteMessageAlarmErrorLogToConsole("sendCommandToServiceBusQueue Exception: payload={0} ex={1}", command, ex.ToString());
                ConsoleLog.WriteBlobLogError("sendCommandToServiceBusQueue Exception:  payload={0} ex={1}", command, ex.ToString());
            }
        }

        private bool compileBitWiseRules(int offset, Dictionary<string, RuleEngineItem> ruleEngineItems)
        {
            RuleEngineItem rei = ruleEngineItems.ElementAt(offset).Value;

            if (offset == 0)
            {
                return rei.Result;
            }
            else
            {
                offset--;
                RuleEngineItem previousRei = ruleEngineItems.ElementAt(offset).Value;
                return AlarmRuleItemEngineUtility.ComplieBoolRule(rei.Result, previousRei.OrderOperation, compileBitWiseRules(offset, ruleEngineItems));
            }
        }

        private void feedInMessage(int messageCatalogId, JObject feedInPayload)
        {
            try
            {
                feedInPayload.Add("MessageCatalogId", messageCatalogId);

                string result = _RTMessageSender.PostRealTimeMessage(feedInPayload.ToString());

                //ConsoleLog.WriteBlobLogInfo("FeedIn Result:{0}", result);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteBlobLogError("feedInMessage Exception: messageCatalogId={0}, payload={1} , ex={2}", messageCatalogId, feedInPayload, ex.Message);
                ConsoleLog.WriteToConsole("feedInMessage Exception: messageCatalogId={0}, payload={1} , ex={2}", messageCatalogId, feedInPayload, ex.Message);
            }
        }

        private async Task<string> putMessageDocument(string deviceId, int messageCatalogId, JObject payload)
        {
            //showDeviceMessage(deviceId, messageCatalogId, payload.ToString());

            MessageDocument messageDocument = new MessageDocument
            {
                //Id = SfDocumentDBHelper.combineDocumentName(deviceId, messageCatalogId), // Marked the Id property will generate a random GUID automatically.
                Type = DocumentType.MessageDocument,
                DeviceId = deviceId,
                MessageCatalogId = messageCatalogId,
                Message = payload
            };

            SfDocumentDBHelper sfDocumentDBHelper = _msgProcessorFactoryModel.SfDocumentDBHelper;
            Document document = await sfDocumentDBHelper.DocumentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(sfDocumentDBHelper.DocumentDbDatabaseName, sfDocumentDBHelper.DocumentDbCollectionName),
                messageDocument);

            ConsoleLog.WriteMessageAlarmLogToConsoleInfo("Document.Id={0}", document.Id);
            return document.Id;
        }

        private void putMessageBlobStorage(string messageDocumentId, string deviceId, JObject payload)
        {
            DateTime now = DateTime.UtcNow;
            string[] dateString = now.ToString("MM/dd/yyyy").Split('/');
            string blobName = dateString[2] + "/" + dateString[0] + "/" + dateString[1] + "/" + deviceId + "/" + messageDocumentId + ".json";
            _msgProcessorFactoryModel.SfblobStorageHelper.Save(blobName, JsonConvert.SerializeObject(payload));
        }

        private async Task putAlarmDocument(SfDocumentDBHelper sfDocumentDBHelper, AlarmMessageHelper alarmDocumentHelper)
        {
            AlarmDocument alarmDocument = new AlarmDocument
            {
                Type = DocumentType.AlarmDocument,
                MessageCatalogId = alarmDocumentHelper.MessageId,
                AlarmRuleCatalogId = alarmDocumentHelper.AlarmRuleId,
                AlarmRuleCatalogName = alarmDocumentHelper.AlarmRuleName,
                AlarmRuleCatalogDescription = alarmDocumentHelper.AlarmRuleDescription,
                TriggeredTime = alarmDocumentHelper.TriggeredTime, // Machine Local Time
                AlarmSent = alarmDocumentHelper.AlarmSent,
                MessageDocumentId = alarmDocumentHelper.MessageDocumentId,
                Message = alarmDocumentHelper.Payload
            };

            await sfDocumentDBHelper.DocumentClient.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(sfDocumentDBHelper.DocumentDbDatabaseName, sfDocumentDBHelper.DocumentDbCollectionName),
                alarmDocument);
        }

        [Conditional("DEBUG")]
        private void showDeviceMessage(string deviceId, int messageCatalogId, string data)
        {
            ConsoleLog.WriteToConsole("\n******************************");
            ConsoleLog.WriteToConsole("deviceId: {0}", deviceId);
            ConsoleLog.WriteToConsole("MessageCatalogId: {0}", messageCatalogId);
            ConsoleLog.WriteToConsole("message: {0}", data);
        }

        private bool vaildateDeviceMessageCatalog(string deviceId, int msgCatalogId)
        {
            SimpleIoTDeviceMessageCatalog simpleDeviceMsgCatalog;
            var L2EQuery = from a in _msgProcessorFactoryModel.SimpleIoTDeviceMessageCatalogList
                           where a.DeviceId == deviceId
                           select a;

            if (L2EQuery.Count() == 0)
            {
                ConsoleLog.WriteToConsole("Device {0} doens't found on this IoT Hub", deviceId);
                return false;           //Device Id doens't found on this IoT Hub
            }
            else
                simpleDeviceMsgCatalog = L2EQuery.FirstOrDefault<SimpleIoTDeviceMessageCatalog>();

            var L2EIdQuery = from b in simpleDeviceMsgCatalog.MessageCatalogIds
                             where b == msgCatalogId
                             select b;
            if (L2EIdQuery.Count() == 0)
            {
                ConsoleLog.WriteToConsole("Message Catalog Id {0} doens't found on this IoT Device", msgCatalogId);
                return false;           //Message Catalog Id doens't found on this IoT Device
            }
            else
                return true;
        }

        private string getDeviceId(EventData eventData)
        {
            string key = "iothub-connection-device-id";
            object pulledObject = null;
            if (eventData.SystemProperties.TryGetValue(key, out pulledObject))
                return pulledObject.ToString();
            else
            {
                ConsoleLog.WriteBlobLogError("System Properties was NOT found: {0} - SequenceNumber={1}", key, eventData.SequenceNumber);
                ConsoleLog.WriteToConsole("System Properties was NOT found: {0} - SequenceNumber={1}", key, eventData.SequenceNumber);
                return null;
            }
        }

        private string getMessageSystemProperty(EventData eventData, string propertyName)
        {
            if (eventData.SystemProperties.Count > 0)
            {
                if (eventData.SystemProperties.ContainsKey(propertyName) && eventData.SystemProperties[propertyName] != null)
                    return eventData.SystemProperties[propertyName].ToString();
                else
                    return null;
            }
            else
            {
                return null;
            }
        }

        private string getMessageProperty(EventData eventData, string propertyName)
        {
            // Try get msgType from message Properties first
            if (eventData.Properties.Count > 0)
            {
                if (eventData.Properties.ContainsKey(propertyName) && eventData.Properties[propertyName] != null)
                    return eventData.Properties[propertyName].ToString();
                else
                    return null;
            }
            else
            {
                /* Get property value from message payload */
                //string data = Encoding.UTF8.GetString(eventData.GetBytes());
                //string matchPrefixString = "\"" + propertyName + "\":\"";
                //string suffix = "\",";
                //int startIdx = data.IndexOf(matchPrefixString);
                //if (startIdx < 0)
                //    return null;
                //int endIdx = data.IndexOf(suffix, startIdx + matchPrefixString.Length);
                //if (endIdx < 0)
                //    return null;
                //else
                //    return data.Substring(startIdx + matchPrefixString.Length, endIdx - (startIdx + matchPrefixString.Length));

                return null;
            }
        }
    }

    class SfEventMessageProcessorFactory : IEventProcessorFactory
    {
        private MessageProcessorFactoryModel msgProcessorFactoryModel;

        public SfEventMessageProcessorFactory(MessageProcessorFactoryModel msgProcessorFactoryModel)
        {
            this.msgProcessorFactoryModel = msgProcessorFactoryModel;

        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new SfMessageEventProcessor(msgProcessorFactoryModel);
        }
    }
}
