using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfShareLib;
using System;
using System.Net;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Diagnostics;

namespace OpsInfra
{
    class Program
    {
        static String _sbConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        static String _sbInfraOpsQueue = ConfigurationManager.AppSettings["sfInfraOpsQueue"];
        static String _superadminHeartbeatURL = ConfigurationManager.AppSettings["sfSuperAdminHeartbeatURL"];
        static QueueClient _sbQueueClient;
        static NamespaceManager _sbNameSpaceMgr;

        protected static PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        protected static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        
        static sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], logLevel);

        static Timer _HBTimer = null;
        static sfShareLib.WebUtility webUtility = new sfShareLib.WebUtility();
        static int _ProcessId = Process.GetCurrentProcess().Id;

        static bool _isRunning = false;

        static void Main(string[] args)
        {
            //DocumentDBMessageModel docDBMsg = new DocumentDBMessageModel()
            //{
            //    ConnectionString = "AccountEndpoint=https://sfdev.documents.azure.com:443/;AccountKey=PHsydcvXyVdELDtWTgLvlbrP5ohuaJbMKQNNCxZKR1UPwS45qVkYiTuXR6wTm9PhnqIDe5IwUoQ0fqmk28CJww==;",
            //    TaskId = "49",
            //    DatabaseName = "db9",
            //    CollectionId = "9"
            //};

            //DocumentDBHelper docDB = new DocumentDBHelper(docDBMsg, "SearchDB");
            //docDB.GetAllDatabase();
            //docDB.GetAllCollectionByDBId("db1");
            //docDB.GetCountOfMsgInCollection("db49", "49");
            //docDB.GetDatabaseAndCollectionInfo("db9", "9");

            /***** Devic Management testing code *****/            
            //SendTestDeviceManagementReportedMessage();
            //SendTestDeviceManagementDesiredMessage();

            /***** Start here *****/
            _sfAppLogger.Info("Start OpsInfra ....");
            SendProcessorHeartbeat();
            ListenOnServiceBusQueue();

            //avoid app finish
            while (true)
                Console.ReadLine();
        }

        /***** for testing *****/
        static void SendTestDeviceManagementDesiredMessage()
        {
            try
            {
                JObject DeviceSystemConfigurationObj = new JObject();
                DeviceSystemConfigurationObj["SFSYS-MinSmartFactorySDKVersion"] = "0.6.2";
                DeviceSystemConfigurationObj["SFSYS-DataCompression"] = false;

                JObject DeviceCustomizedConfigurationObj = new JObject();
                DeviceCustomizedConfigurationObj["interval"] = 444;

                JObject DeviceConfigurationObj = new JObject();
                DeviceConfigurationObj["SF_SystemConfig"] = DeviceSystemConfigurationObj;
                DeviceConfigurationObj["SF_CustomizedConfig"] = DeviceCustomizedConfigurationObj;
                DeviceConfigurationObj["SF_LastUpdatedTimestamp"] = 1498127846;

                dynamic msg = new ExpandoObject();
                msg.job = "device management";
                msg.entity = "iotdevice";
                msg.entityId = "DeviceTwinsTest1";
                msg.task = "update device desired property";
                msg.taskId = 1084;
                msg.primaryIothubConnectionString = "HostName=SmartFactoryTest.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=kL6UDlBi0yZs9Jd4LnbhxBMj4/m9YXE8Ft8V14Y72EQ=";
                //msg.secondaryIothubConnectionString = "";
                msg.iothubDeviceId = "DeviceTwinsTest1";
                msg.requester = "Andy Li";
                msg.requesterEmail = "a-andli@microsoft.com";
                msg.requestDateTime = "2017-05-9T15:54:26.4208436Z";
                msg.deviceConfiguration = DeviceConfigurationObj;

                var jsonStr = JsonConvert.SerializeObject(msg);
                _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbInfraOpsQueue);
                _sbQueueClient.Send(new BrokeredMessage(jsonStr));
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
        }

        static void SendTestDeviceManagementReportedMessage()
        {
            try
            {
                JObject DeviceConfigurationObj = new JObject();
                DeviceConfigurationObj["SF_LastUpdatedTimestamp"] = 1498123019;

                dynamic msg = new ExpandoObject();
                msg.job = "device management";
                msg.entity = "iotdevice";
                msg.entityId = "DeviceTwinsTest1";
                msg.task = "update device reported property to db";
                msg.iothubDeviceId = "DeviceTwinsTest1";
                msg.requester = "IoTHubReceiver_IoTHubAliasName";
                msg.requestDateTime = "2017-05-9T15:54:26.4208436Z";
                msg.iothubAlias = "AndyFreeTierIoTHub";
                msg.iothubIsPrimary = true;
                msg.deviceConfiguration = DeviceConfigurationObj;

                var jsonStr = JsonConvert.SerializeObject(msg);
                _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbInfraOpsQueue);
                _sbQueueClient.Send(new BrokeredMessage(jsonStr));
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
            /*
            
            */
        }

        static void ListenOnServiceBusQueue()
        {
            /* Create Queue client and listen on message  */
            _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbInfraOpsQueue);
            OnMessageOptions options = new OnMessageOptions();
            options.MaxConcurrentCalls = 1;
            options.AutoComplete = false;
            string messageBody = "";

            _isRunning = true;

            _sbQueueClient.OnMessage((message) =>
            {
                string task, command;
                int taskId = 0;
                try
                {
                    // Process message from queue.
                    messageBody = message.GetBody<string>();
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("OpsInfra onMessage: " + messageBody);

                    JObject jsonMessage = JObject.Parse(messageBody);

                    if (jsonMessage["taskId"] != null)
                        taskId = int.Parse(jsonMessage["taskId"].ToString());

                    if (jsonMessage["command"] != null)
                    {
                        command = jsonMessage["command"].ToString();
                        Console.WriteLine("command:" + command);
                        switch (command.ToLower())
                        {
                            case "shutdown":
                                _sfAppLogger.Info("Received Command: Shutdown");
                                message.Complete();
                                UpdateTaskBySuccess(taskId);
                                Environment.Exit(0);
                                break;
                        }
                        message.Complete();
                    }
                    else if (jsonMessage["task"] != null)
                    {
                        task = jsonMessage["task"].ToString();
                        Console.WriteLine("task:" + task);

                        _sfAppLogger.Info("Received task: " + task);
                        switch (task.ToLower())
                        {
                            case "create documentdb collection":
                            case "purge documentdb collection":
                                DocumentDBMessageModel docDBMsg = new DocumentDBMessageModel()
                                {
                                    ConnectionString = jsonMessage["documentdbConnectionString"].ToString(),
                                    DatabaseName = jsonMessage["databaseName"].ToString(),
                                    CollectionId = jsonMessage["collectionId"].ToString(),
                                    TaskId = jsonMessage["taskId"].ToString()
                                };

                                if (String.IsNullOrEmpty(docDBMsg.ConnectionString))
                                    docDBMsg.ConnectionString = ConfigurationManager.AppSettings["sfDocDBConnectionString"];

                                DocumentDBHelper docDB = new DocumentDBHelper(docDBMsg, task.ToLower());
                                Thread docDBthread = new Thread(new ThreadStart(docDB.ThreadProc));
                                docDBthread.IsBackground = false;
                                docDBthread.Start();
                                message.Complete();
                                break;
                            case "create iothub register":
                            case "update iothub register":
                            case "remove iothub register":
                                IoTHubDeviceMessageModel deviceMessage = new IoTHubDeviceMessageModel()
                                {
                                    taskId = jsonMessage["taskId"].ToString(),
                                    oldPrimaryIothubConnectionString = jsonMessage["oldPrimaryIothubConnectionString"].ToString(),
                                    //oldSecondaryIothubConnectionString = jsonMessage["oldSecondaryIothubConnectionString"].ToString(),
                                    primaryIothubConnectionString = jsonMessage["primaryIothubConnectionString"].ToString(),
                                    //secondaryIothubConnectionString = jsonMessage["secondaryIothubConnectionString"].ToString(),
                                    iothubDeviceId = jsonMessage["iothubDeviceId"].ToString(),
                                    iothubDeviceKey = jsonMessage["iothubDeviceKey"].ToString(),
                                    authenticationType = jsonMessage["authenticationType"].ToString(),
                                    certificateThumbprint = jsonMessage["certificateThumbprint"].ToString()
                                };

                                IoTHubDeviceHelper iotHubDeviceHelper = new IoTHubDeviceHelper(deviceMessage, task.ToLower());
                                Thread createDeviceThread = new Thread(new ThreadStart(iotHubDeviceHelper.ThreadProc));
                                createDeviceThread.IsBackground = false;
                                createDeviceThread.Start();
                                message.Complete();
                                break;
                            case "create iothub alias":
                            case "remove iothub alias":
                                IoTHubEventProcessorHelper iotHubEP = new IoTHubEventProcessorHelper(jsonMessage["entityId"].ToString(), int.Parse(jsonMessage["taskId"].ToString()), task.ToLower());
                                Thread manageIoTHubAliasThread = new Thread(new ThreadStart(iotHubEP.ThreadProc));
                                manageIoTHubAliasThread.IsBackground = false;
                                manageIoTHubAliasThread.Start();
                                message.Complete();
                                break;
                            case "update device desired property":
                                {
                                    DBHelper._IoTDevice dbhelper = new DBHelper._IoTDevice();
                                    IoTDevice iotDevice = dbhelper.GetByid(jsonMessage["iothubDeviceId"].ToString());
                                    int deviceConfigurationStatus = iotDevice.DeviceConfigurationStatus;
                                    JObject existingDesiredObj = JObject.Parse(iotDevice.DeviceTwinsDesired);
                                    JObject newDesiredObj = JObject.Parse(jsonMessage["deviceConfiguration"].ToString());
                                    int existingLastUpdatedTimestamp = Convert.ToInt32(existingDesiredObj["SF_LastUpdatedTimestamp"]);
                                    int newLastUpdatedTimestamp = Convert.ToInt32(newDesiredObj["SF_LastUpdatedTimestamp"]);

                                    if (newLastUpdatedTimestamp >= existingLastUpdatedTimestamp)
                                    {
                                        IoTHubDeviceManagementMessageModel deviceManagementMessage = new IoTHubDeviceManagementMessageModel()
                                        {
                                            taskId = jsonMessage["taskId"] == null ? "0" : jsonMessage["taskId"].ToString(),
                                            primaryIothubConnectionString = jsonMessage["primaryIothubConnectionString"] == null ? "" : jsonMessage["primaryIothubConnectionString"].ToString(),
                                            //secondaryIothubConnectionString = jsonMessage["secondaryIothubConnectionString"] == null ? "" : jsonMessage["secondaryIothubConnectionString"].ToString(),
                                            iothubDeviceId = jsonMessage["iothubDeviceId"].ToString(),
                                            deviceConfiguration = JObject.Parse(jsonMessage["deviceConfiguration"].ToString())
                                        };
                                        IoTHubDeviceManagementHelper iotHubDMHelper = new IoTHubDeviceManagementHelper(deviceManagementMessage, task.ToLower());
                                        Thread deviceManagementThread = new Thread(new ThreadStart(iotHubDMHelper.ThreadProc));
                                        deviceManagementThread.IsBackground = false;
                                        deviceManagementThread.Start();
                                        message.Complete();
                                    }
                                    else
                                    {
                                        throw new Exception("It's old version of device configuration");
                                    }
                                }
                                break;
                            case "update device reported property to db":
                                {
                                    DBHelper._IoTDevice dbhelper = new DBHelper._IoTDevice();
                                    IoTDevice iotDevice = dbhelper.GetByid(jsonMessage["iothubDeviceId"].ToString());
                                    int deviceConfigurationStatus = iotDevice.DeviceConfigurationStatus;
                                    JObject existingReportedObj = JObject.Parse(iotDevice.DeviceTwinsReported);
                                    JObject newReportedObj = JObject.Parse(jsonMessage["deviceConfiguration"].ToString());
                                    int existingLastUpdatedTimestamp = Convert.ToInt32(existingReportedObj["SF_LastUpdatedTimestamp"]);
                                    int newLastUpdatedTimestamp = Convert.ToInt32(newReportedObj["SF_LastUpdatedTimestamp"]);

                                    if (newLastUpdatedTimestamp >= existingLastUpdatedTimestamp)
                                    {
                                        switch (deviceConfigurationStatus)
                                        {
                                            case IoTDeviceConfigurationStatus.SUBMIT:
                                                message.Abandon();
                                                break;
                                            case IoTDeviceConfigurationStatus.WAITING_DEVICE_ACK:
                                            case IoTDeviceConfigurationStatus.RECEIVE_DEVICE_ACK:
                                                IoTHubDeviceManagementMessageModel deviceManagementMessage = new IoTHubDeviceManagementMessageModel()
                                                {
                                                    taskId = "0",
                                                    iothubDeviceId = jsonMessage["iothubDeviceId"].ToString(),
                                                    deviceConfiguration = newReportedObj,
                                                    iothubAlias = jsonMessage["iothubAlias"].ToString(),
                                                    iothubIsPrimary = Convert.ToBoolean(jsonMessage["iothubIsPrimary"].ToString()),
                                                    taskContent = JsonConvert.SerializeObject(jsonMessage)
                                                };
                                                IoTHubDeviceManagementHelper iotHubDMHelper = new IoTHubDeviceManagementHelper(deviceManagementMessage, task.ToLower());
                                                Thread deviceManagementThread = new Thread(new ThreadStart(iotHubDMHelper.ThreadProc));
                                                deviceManagementThread.IsBackground = false;
                                                deviceManagementThread.Start();
                                                message.Complete();
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("It's old version of device configuration");
                                    }
                                }
                            break;
                        }
                    }                    
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    Console.WriteLine(ex.Message);
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("OpsInfra Exception: " + ex.Message);
                    logMessage.AppendLine("OpsInfra Message: " + messageBody);
                    _sfAppLogger.Error(logMessage);
                    Console.WriteLine();
                    message.Complete();

                    UpdateTaskByFail(taskId, ex.Message);
                }
            }, options);
        }

        public static void UpdateTaskBySuccess(int taskId)
        {
            DBHelper._OperationTask opsTaskHelper = new DBHelper._OperationTask();
            OperationTask opsTask = opsTaskHelper.GetByid(taskId);
            opsTask.CompletedAt = DateTime.UtcNow;
            opsTask.TaskLog = DateTime.UtcNow + ": Done.";
            opsTask.TaskStatus = "Completed";

            opsTaskHelper.Update(opsTask);
        }

        public static void UpdateTaskByFail(int taskId, string failLog, int status = TaskStatus.FAILED)
        {
            try
            {
                DBHelper._OperationTask opsTaskHelper = new DBHelper._OperationTask();
                OperationTask opsTask = opsTaskHelper.GetByid(taskId);
                opsTask.RetryCounter = opsTask.RetryCounter + 1;
                opsTask.TaskLog = DateTime.UtcNow + ": " + failLog;
                switch (status)
                {
                    case TaskStatus.FAILED:
                        opsTask.TaskStatus = "Fail";
                        break;
                    case TaskStatus.WARNING:
                        opsTask.TaskStatus = "Warning";
                        break;
                    case TaskStatus.COMPLETED:
                        opsTask.TaskStatus = "Completed";
                        opsTask.TaskLog = DateTime.UtcNow + ": Done. " + failLog;
                        opsTask.CompletedAt = DateTime.UtcNow;
                        break;
                }

                opsTaskHelper.Update(opsTask);
            }
            catch (Exception)
            {

            }
        }

        public static string GetNonJsonExceptionMessage(string message)
        {
            try
            {
                var obj = JObject.Parse(message);
                return obj["Message"].ToString();
            }
            catch
            {
                return message;
            }
        }

        static void SendProcessorHeartbeat()
        {
            if (_sbNameSpaceMgr == null)
                _sbNameSpaceMgr = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(_sbConnectionString);

            _HBTimer = new Timer(new TimerCallback(PushHeartbeatSignal));
            _HBTimer.Change(10000, 10000);
        }

        static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "Ops Infra";
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;

            if (_isRunning)
                HeartbeatMessage.status = "Running";
            else
                HeartbeatMessage.status = "Stop";

            if (_sbNameSpaceMgr != null)
                HeartbeatMessage.queueLength = _sbNameSpaceMgr.GetQueue(_sbInfraOpsQueue).MessageCount;
            else
                HeartbeatMessage.queueLength = -1;  //Unknow

            HeartbeatMessage.cpu = Math.Round(_cpuCounter.NextValue(),2) + " %";
            HeartbeatMessage.ramAvail = _ramCounter.NextValue() + " MB";            

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
        }

        static void PushHeartbeatSignal(object state)
        {
            string jsonHB = GetHeartbeatStatus();
            try
            {
                webUtility.PostContent(_superadminHeartbeatURL, jsonHB);
                //Console.WriteLine("Heartbeat: " + jsonHB);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("OpsInfra Exception on send Heartbeat: " + ex.Message);
                _sfAppLogger.Error(logMessage);
            }
        }

        public static string FilterErrorMessage(string message)
        {
            message = message.Replace("{", "(");
            message = message.Replace("}", ")");
            message = message.Replace("\"", "");
            message = message.Replace('"', ' ');

            return message;
        }
    }

    static class TaskStatus
    {
        public const int COMPLETED = 0;
        public const int WARNING = 1;
        public const int FAILED = 2;
    }
    static class IoTDeviceConfigurationStatus
    {
        public const int SUBMIT = 0;
        public const int WAITING_DEVICE_ACK = 1;
        public const int RECEIVE_DEVICE_ACK = 2;
    }
}
