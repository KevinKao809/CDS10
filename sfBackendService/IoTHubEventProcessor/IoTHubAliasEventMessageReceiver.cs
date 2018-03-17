using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Yare.Lib.Rules;
using System.IO;
using ProtoBuf;
using IoTHubEventProcessor.Utilities;
using IoTHubEventProcessor.Helper;
using IoTHubEventProcessor.Models;

namespace IoTHubEventProcessor
{
    public class IoTHubAliasEventMessageReceiver
    {
        private int _companyId;
        private string _IoTHubAliasName;
        private DateTime _primaryFailDT, _secondaryFailDT;
        private bool _isPrimary = true, _isRunning = false;
        private EventProcessorHost _eventProcessorHost;

        private string _documentDB_EndpointUri;
        private string _documentDB_PrimaryKey;
        private MessageProcessorFactoryModel _msgProcessorFactoryModel = new MessageProcessorFactoryModel();

        private class CompatibleEventHub
        {
            public string EndpointName { get; set; }
            public string IoTHubConnectionString { get; set; }
            public string ConsumerGroup { get; set; }
            public string StorageConnectionString { get; set; }

            public CompatibleEventHub(string endpointName,
                string iothubConnectionString,
                string consumerGroup,
                string storageConnectionString)
            {
                this.EndpointName = endpointName;
                this.IoTHubConnectionString = iothubConnectionString;
                this.ConsumerGroup = consumerGroup;
                this.StorageConnectionString = storageConnectionString;
            }

            public CompatibleEventHub() { }
        }

        private CompatibleEventHub _Primary_CompatibleEventHub;
        private CompatibleEventHub _Secondary_CompatibleEventHub;
        private CompatibleEventHub _CompatibleEventHub = new CompatibleEventHub();

        public IoTHubAliasEventMessageReceiver(string iotHubAliasName)
        {
            _IoTHubAliasName = iotHubAliasName;
        }

        public async Task<bool> Start()
        {
            if (_isRunning)
                return true;

            loadConfigurationFromDB(_IoTHubAliasName, _msgProcessorFactoryModel);
            fetchAvailableIoTHub();

            _msgProcessorFactoryModel.SfDocumentDBHelper = await fetchDocumentDB();
            _msgProcessorFactoryModel.SfblobStorageHelper = new BlobStorageHelper(_CompatibleEventHub.StorageConnectionString, "telemetry");
            _msgProcessorFactoryModel.SfQueueClient = QueueClient.CreateFromConnectionString(Program._sbConnectionString, Program._sbAlarmOpsQueue);
            _msgProcessorFactoryModel.SfInfraQueueClient = QueueClient.CreateFromConnectionString(Program._sbConnectionString, Program._sbInfraOpsQueue);
            _msgProcessorFactoryModel.IsIoTHubPrimary = _isPrimary;
            _msgProcessorFactoryModel.IoTHubAlias = _IoTHubAliasName;

            string eventProcessorHostName = _companyId + "-" + _IoTHubAliasName.ToLower();
            if (eventProcessorHostName.Contains(" "))
                eventProcessorHostName = eventProcessorHostName.Replace(' ', Program.REPLACE_SPACE_TO_DASH);
            string leaseName = eventProcessorHostName;
            _eventProcessorHost = new EventProcessorHost(
                eventProcessorHostName, // Task ID
                _CompatibleEventHub.EndpointName, // Endpoint: messages/events
                _CompatibleEventHub.ConsumerGroup,// Consumer Group
                _CompatibleEventHub.IoTHubConnectionString, // IoT Hub Connection String
                _CompatibleEventHub.StorageConnectionString,
                leaseName);

            ConsoleLog.WriteToConsole("Registering IoTHubAliasEventMessageReceiver on {0} ...", _IoTHubAliasName);

            var options = new EventProcessorOptions
            {
                InitialOffsetProvider = (partitionId) => DateTime.UtcNow
            };
            options.ExceptionReceived += (sender, e) =>
            {
                ConsoleLog.WriteToConsole("EventProcessorOptions Exception:{0}", e.Exception);
                ConsoleLog.WriteBlobLogError("EventProcessorOptions Exception:{0}", e.Exception);
            };

            try
            {
                //await _eventProcessorHost.RegisterEventProcessorAsync<sfEventMessage>(options);
                await _eventProcessorHost.RegisterEventProcessorFactoryAsync(new SfEventMessageProcessorFactory(_msgProcessorFactoryModel), options);
                _isRunning = true;
            }
            catch (Exception ex)
            {
                _isRunning = false;
                //keepFailStatus();

                //string hubname = _isPrimary ? "Primary" : "Secondary";
                //ConsoleLog.WriteToConsole("{0} IoTHub Fail, try switch to another IoTHub - Exception: {1}", hubname, ex.Message);
                //ConsoleLog.WriteBlobLogError("{0} IoTHub Fail, try switch to another IoTHub - Exception: {1}", hubname, ex.Message);
                ConsoleLog.WriteBlobLogError("IoTHub Fail, Closed. - Exception: {0}", ex.Message);

                /* Send out a message: restart to Topic */
                //SfOperationTaskHelper.RestartIoTHubRecevicer(
                //    _companyId,
                //    _IoTHubAliasName,
                //    Program.GetEventProcessorHostName(_IoTHubAliasName),
                //    "null");                
                Environment.Exit(0);
            }

            return true;
        }

        public async Task<bool> Stop()
        {
            if (!_isRunning)
                return true;
            await _eventProcessorHost.UnregisterEventProcessorAsync();
            _isRunning = false;

            ConsoleLog.WriteToConsole("Unregistering IoTHubAliasEventMessageReceiver on {0} ...", _IoTHubAliasName);
            return true;
        }

        private void loadConfigurationFromDB(string iotHubAliasName, MessageProcessorFactoryModel msgProcessorFactoryModel)
        {
            DBHelper._IoTHub iotHubHelper = new DBHelper._IoTHub();

            IoTHub iotHub = iotHubHelper.GetByid(_IoTHubAliasName);
            if (iotHub == null)
            {
                ConsoleLog.WriteToConsole("IoTHubAlias Not Found. Alias:{0}", iotHubAliasName);
                ConsoleLog.WriteBlobLogError("IoTHubAlias Not Found. Alias:{0}", iotHubAliasName);
                throw new Exception("IoTHubAlias Not Found");
            }

            _companyId = iotHub.CompanyID;

            msgProcessorFactoryModel.SimpleIoTDeviceMessageCatalogList = findAllMessageSchema(iotHub);
            msgProcessorFactoryModel.MessageIdAlarmRules = findAllMessageAlarmRules(iotHub.IoTHubAlias);
            findAllCompatibleEventHubs(iotHub);
            findDocDBConnectionString(iotHub);
        }

        private Dictionary<int, List<AlarmRuleCatalogEngine>> findAllMessageAlarmRules(string iotHubAlias)
        {
            Dictionary<int, List<AlarmRuleCatalogEngine>> messageIdAlarmRules = new Dictionary<int, List<AlarmRuleCatalogEngine>>();

            SFDatabaseEntities dbEntity = new SFDatabaseEntities();
            var L2Enty = from c in dbEntity.IoTDevice
                         join msgCatalog in dbEntity.IoTDeviceMessageCatalog on c.IoTHubDeviceID equals msgCatalog.IoTHubDeviceID
                         where c.IoTHubAlias == iotHubAlias && msgCatalog.MessageCatalog.DeletedFlag == false
                         select msgCatalog.MessageCatalog;
            List<MessageCatalog> mcList = L2Enty.Distinct().ToList<MessageCatalog>();

            foreach (MessageCatalog mc in mcList)
            {
                List<AlarmRuleCatalogEngine> arcEngineList = new List<AlarmRuleCatalogEngine>();
                foreach (AlarmRuleCatalog arc in mc.AlarmRuleCatalog)
                {
                    if (arc.DeletedFlag == false && arc.ActiveFlag == true)
                    {
                        ConsoleLog.WriteMessageAlarmLogToConsole("AlarmRuleCatalogId={0}", arc.Id);

                        AlarmRuleCatalogEngine are = new AlarmRuleCatalogEngine();
                        are.AlarmRuleCatalogId = arc.Id;
                        are.AlarmRuleCatalog = arc;
                        are.RuleEngineItems = createRuleEngineItem(arc.Id);
                        are.LastTriggerTime = new DateTime(2017, 1, 1);
                        are.Triggered = false;

                        arcEngineList.Add(are);
                    }
                }

                messageIdAlarmRules.Add(mc.Id, arcEngineList);
            }

            return messageIdAlarmRules;
        }

        private Dictionary<string, RuleEngineItem> createRuleEngineItem(int alarmRuleCatalogId)
        {
            DBHelper._AlarmRuleItem dbhelperAlarmRuleItem = new DBHelper._AlarmRuleItem();
            var detailForRuleEngineModelList = dbhelperAlarmRuleItem.GetAllByAlarmRuleCatalogIdForRuleEngine(alarmRuleCatalogId);

            Dictionary<string, RuleEngineItem> ruleEngineItems = new Dictionary<string, RuleEngineItem>();

            int index = 0;
            foreach (var detailForRuleEngineModel in detailForRuleEngineModelList)
            {
                RuleEngineItem rei = new RuleEngineItem();
                rei.ElementName = detailForRuleEngineModel.MessageElementFullName;
                rei.DataType = AlarmRuleItemEngineUtility.GetSupportDataType(detailForRuleEngineModel.MessageElementDataType);
                rei.OrderOperation = detailForRuleEngineModel.BitWiseOperation;
                rei.Result = false;

                ConsoleLog.WriteMessageAlarmLogToConsole("--ElementName={0}, BitWiseOperation={1}", rei.ElementName, rei.OrderOperation);

                if (rei.DataType == SupportDataTypeEnum.String &&
                    (string.IsNullOrEmpty(detailForRuleEngineModel.Value) || detailForRuleEngineModel.Value.ToLower().Equals("null")))
                {
                    detailForRuleEngineModel.Value = "null";
                }

                if (rei.DataType == SupportDataTypeEnum.Numeric || rei.DataType == SupportDataTypeEnum.Bool)
                {
                    rei.Equality = createCompiledRuleFunc(rei.DataType, detailForRuleEngineModel.EqualOperation, detailForRuleEngineModel.Value);
                }
                else
                {
                    // SupportDataTypeEnum.String
                    rei.Equality = null;
                    rei.StringRightValue = detailForRuleEngineModel.Value;

                    if (detailForRuleEngineModel.EqualOperation.Equals("=") || detailForRuleEngineModel.EqualOperation.Equals("!="))
                        rei.StringEqualOperation = detailForRuleEngineModel.EqualOperation;
                    else
                        throw new ArgumentNullException("String equal operation is not supported - " + detailForRuleEngineModel.EqualOperation);

                    ConsoleLog.WriteMessageAlarmLogToConsole("----ruleText=({0} {1} {2})", rei.ElementName, detailForRuleEngineModel.EqualOperation, rei.StringRightValue);
                }

                // Add the index to avoid the duplicate key
                ruleEngineItems.Add(rei.ElementName + "-" + index, rei);
                index++;
            }

            return ruleEngineItems;
        }

        public Func<DynamicMessageElement, bool> CreateCompiledRuleFuncTest(SupportDataTypeEnum isDecimalType, string op, string right)
        {
            return createCompiledRuleFunc(isDecimalType, op, right);
        }

        private Func<DynamicMessageElement, bool> createCompiledRuleFunc(SupportDataTypeEnum isDecimalType, string op, string right)
        {
            EqualityRule rule;

            switch (isDecimalType)
            {
                case SupportDataTypeEnum.Bool:
                    rule = new EqualityRule("StringValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right.ToLower());
                    break;
                case SupportDataTypeEnum.Numeric:
                    rule = new EqualityRule("DecimalValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right);
                    break;
                case SupportDataTypeEnum.String:
                    rule = new EqualityRule("StringValue", AlarmRuleItemEngineUtility.GetEqualityOperation(op), right);
                    break;
                default:
                    throw new NotSupportedException();
            }

            RuleBase rb = null;
            using (var mem = new MemoryStream())
            {
                Serializer.Serialize(mem, rule);
                mem.Position = 0;
                rb = Serializer.Deserialize<RuleBase>(mem);
            }

            string ruleText;
            Func<DynamicMessageElement, bool> compiledRule = rb.CompileRule<DynamicMessageElement>(out ruleText);

            ConsoleLog.WriteMessageAlarmLogToConsole("----ruleText={0}", ruleText);

            return compiledRule;
        }

        private void findDocDBConnectionString(IoTHub iotHub)
        {
            string docDBConnectionString;
            if (iotHub.Company.DocDBConnectionString == null)
            {
                // Use the DocumentDB connection string of default settings
                docDBConnectionString = Program._sfDocDBConnectionString;
                ConsoleLog.WriteDocDBLogToConsole("Use the default DocumentDB of SmartFactory...");
            }
            else
            {
                // Use the DocumentDB connection string of customer
                docDBConnectionString = iotHub.Company.DocDBConnectionString;
                ConsoleLog.WriteDocDBLogToConsole("Use the customer's DocumentDB...");
                ConsoleLog.WriteBlobLogDebug("Use the customer's DocumentDB...");
            }

            string accountEndpoint = docDBConnectionString.Split(';')[0];
            _documentDB_EndpointUri = accountEndpoint.Replace("AccountEndpoint=", "");

            string accountKey = docDBConnectionString.Split(';')[1];
            _documentDB_PrimaryKey = accountKey.Replace("AccountKey=", "");
        }

        private void findAllCompatibleEventHubs(IoTHub iotHub)
        {
            _Primary_CompatibleEventHub = new CompatibleEventHub(
                iotHub.P_IoTHubEndPoint, // messages/events
                iotHub.P_IoTHubConnectionString, // IoT Hub Connection String
                iotHub.P_EventConsumerGroup, // Consumer Group
                iotHub.P_EventHubStorageConnectionString); // Storage Connection String

            _Secondary_CompatibleEventHub = new CompatibleEventHub(
                iotHub.S_IoTHubEndPoint, // messages/events
                iotHub.S_IoTHubConnectionString, // IoT Hub Connection String
                iotHub.S_EventConsumerGroup, // Consumer Group
                iotHub.S_EventHubStorageConnectionString); // Storage Connection String
        }

        private List<SimpleIoTDeviceMessageCatalog> findAllMessageSchema(IoTHub iotHub)
        {
            List<SimpleIoTDeviceMessageCatalog> simpleIoTDeviceMessageCatalogList = new List<SimpleIoTDeviceMessageCatalog>();

            foreach (IoTDevice iotDevice in iotHub.IoTDevice)
            {
                SimpleIoTDeviceMessageCatalog simpleIoTDeviceMessageCatalog = new SimpleIoTDeviceMessageCatalog();

                simpleIoTDeviceMessageCatalog.DeviceId = iotDevice.IoTHubDeviceID;
                simpleIoTDeviceMessageCatalog.MessageCatalogIds = new List<int>();

                foreach (IoTDeviceMessageCatalog ioTDeviceMessageCatalog in iotDevice.IoTDeviceMessageCatalog)
                {
                    simpleIoTDeviceMessageCatalog.MessageCatalogIds.Add(ioTDeviceMessageCatalog.MessageCatalogID);
                }

                simpleIoTDeviceMessageCatalogList.Add(simpleIoTDeviceMessageCatalog);
            }

            showSimpleIoTDeviceMessageCatalogList(simpleIoTDeviceMessageCatalogList);

            return simpleIoTDeviceMessageCatalogList;
        }

        [Conditional("DEBUG")]
        private void showSimpleIoTDeviceMessageCatalogList(List<SimpleIoTDeviceMessageCatalog> simpleIoTDeviceMessageCatalogList)
        {
            int count = 0;
            foreach (SimpleIoTDeviceMessageCatalog simpleDeviceMsgCatalog in simpleIoTDeviceMessageCatalogList)
            {
                ConsoleLog.WriteToConsole("--- Device [{0}] {1} ---", count, simpleDeviceMsgCatalog.DeviceId);
                count++;

                int msgCount = 0;
                foreach (int msgCatalogId in simpleDeviceMsgCatalog.MessageCatalogIds)
                {
                    ConsoleLog.WriteToConsole("[{0}] msgCatalogId {1}", msgCount, msgCatalogId);
                    msgCount++;
                }
            }
        }

        public string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.companyId = _companyId;
            HeartbeatMessage.topic = "IoTHubEventProcessor Heartbeat";
            HeartbeatMessage.iotHubAlias = _IoTHubAliasName;
            if (_isPrimary)
                HeartbeatMessage.iotHubNode = "Primary";
            else
                HeartbeatMessage.iotHubNode = "Secondary";

            if (_isRunning)
                HeartbeatMessage.status = "Running";
            else
                HeartbeatMessage.status = "Stop";

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
        }

        private async Task<SfDocumentDBHelper> fetchDocumentDB()
        {
            try
            {
                SfDocumentDBHelper sfDocumentDBHelper = await checkAndCreateSfDocumentDB();
                return sfDocumentDBHelper;
            }
            catch (DocumentClientException de)
            {
                Exception baseException = de.GetBaseException();
                ConsoleLog.WriteToConsole("{0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                ConsoleLog.WriteBlobLogError("fetchDocumentDB: {0} error occurred: {1}, Message: {2}", de.StatusCode, de.Message, baseException.Message);
                throw;
            }
            catch (Exception e)
            {
                Exception baseException = e.GetBaseException();
                ConsoleLog.WriteToConsole("Error: {0}, Message: {1}", e.Message, baseException.Message);
                ConsoleLog.WriteBlobLogError("fetchDocumentDB: Error: {0}, Message: {1}", e.Message, baseException.Message);
                throw;
            }
        }

        private void fetchAvailableIoTHub()
        {
            if (((DateTime.UtcNow - _primaryFailDT).TotalMinutes < 2) && ((DateTime.UtcNow - _secondaryFailDT).TotalMinutes < 2))
            {
                throw new Exception("Both IoTHub Fail");
            }

            if (((DateTime.UtcNow - _primaryFailDT).TotalMinutes > 2))
            {
                _CompatibleEventHub.EndpointName = _Primary_CompatibleEventHub.EndpointName;
                _CompatibleEventHub.IoTHubConnectionString = _Primary_CompatibleEventHub.IoTHubConnectionString;
                _CompatibleEventHub.ConsumerGroup = _Primary_CompatibleEventHub.ConsumerGroup;
                _CompatibleEventHub.StorageConnectionString = _Primary_CompatibleEventHub.StorageConnectionString;
                _isPrimary = true;
                ConsoleLog.WriteToConsole("On Primary IoTHub");
            }
            else
            {
                _CompatibleEventHub.EndpointName = _Secondary_CompatibleEventHub.EndpointName;
                _CompatibleEventHub.IoTHubConnectionString = _Secondary_CompatibleEventHub.IoTHubConnectionString;
                _CompatibleEventHub.ConsumerGroup = _Secondary_CompatibleEventHub.ConsumerGroup;
                _CompatibleEventHub.StorageConnectionString = _Secondary_CompatibleEventHub.StorageConnectionString;
                _isPrimary = false;
                ConsoleLog.WriteToConsole("On Secondary IoTHub");
            }
        }

        private void keepFailStatus()
        {
            if (_isPrimary)
                _primaryFailDT = DateTime.UtcNow;
            else
                _secondaryFailDT = DateTime.UtcNow;
        }

        private async System.Threading.Tasks.Task<SfDocumentDBHelper> checkAndCreateSfDocumentDB()
        {
            DocumentClient dc = new DocumentClient(new Uri(this._documentDB_EndpointUri), this._documentDB_PrimaryKey);

            SfDocumentDBHelper sfDocumentDBHelper = new SfDocumentDBHelper(dc, this._companyId);

            await checkDatabaseIfNotExists(sfDocumentDBHelper);

            await checkCollectionIfNotExists(sfDocumentDBHelper);

            return sfDocumentDBHelper;
        }

        private async System.Threading.Tasks.Task checkDatabaseIfNotExists(SfDocumentDBHelper sfDocumentDB)
        {
            // Check to verify a database with the id=FamilyDB does not exist
            try
            {
                await sfDocumentDB.DocumentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(sfDocumentDB.DocumentDbDatabaseName));
                ConsoleLog.WriteDocDBLogToConsole("Database Found {0}", sfDocumentDB.DocumentDbDatabaseName);
            }
            catch (DocumentClientException de)
            {
                // If the database does not exist, create a new database
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    ConsoleLog.WriteDocDBLogToConsole("Database NotFound {0}", sfDocumentDB.DocumentDbDatabaseName);
                    ConsoleLog.WriteBlobLogError("Database NotFound {0}", sfDocumentDB.DocumentDbDatabaseName);
                }

                throw;
            }
        }

        private async System.Threading.Tasks.Task checkCollectionIfNotExists(SfDocumentDBHelper sfDocumentDBHelper)
        {
            try
            {
                await sfDocumentDBHelper.DocumentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(sfDocumentDBHelper.DocumentDbDatabaseName, sfDocumentDBHelper.DocumentDbCollectionName));
                ConsoleLog.WriteDocDBLogToConsole("Collection Found {0}", sfDocumentDBHelper.DocumentDbCollectionName);
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    ConsoleLog.WriteDocDBLogToConsole("Collection NotFound {0}", sfDocumentDBHelper.DocumentDbCollectionName);
                    ConsoleLog.WriteBlobLogError("Collection NotFound {0}", sfDocumentDBHelper.DocumentDbCollectionName);
                }

                throw;
            }
        }

    }
}
