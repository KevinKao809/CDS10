using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sfShareLib;

namespace OpsAlarm
{
    class Program
    {
        static String _sbConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        static String _sbAlarmOpsQueue = ConfigurationManager.AppSettings["sfAlarmOpsQueue"];
        static String _superadminHeartbeatURL = ConfigurationManager.AppSettings["sfSuperAdminHeartbeatURL"];
        static QueueClient _sbQueueClient;
        static NamespaceManager _sbNameSpaceMgr;

        protected static PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        protected static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        //public static Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        static sfLogLevel _logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], _logLevel);

        static Timer _HBTimer = null;
        static int _ProcessId = Process.GetCurrentProcess().Id;
        static bool _isRunning = false;
        static sfShareLib.WebUtility webUtility = new sfShareLib.WebUtility();

        static void Main(string[] args)
        {
            //SendTestExternalMessage();
            _sfAppLogger.Info("Start OpsAlarm ....");

            SendProcessorHeartbeat();
            ListenOnServiceBusQueue();

            //avoid app finish
            while (true)
                Console.ReadLine();
        }

        static void SendTestDashboardMessage()
        {
            try
            {
                JObject msgPayloadObj = new JObject();
                msgPayloadObj["companyId"] = 1;
                msgPayloadObj["msgTimestamp"] = "2017-02-20T15:09:06";
                msgPayloadObj["equipmentId"] = "EquipmentIDA";
                msgPayloadObj["equipmentRunStatus"] = 1;
                msgPayloadObj["RPM"] = 5415;
                msgPayloadObj["Temperature"] = 59.5;
                msgPayloadObj["VehicleId"] = "SN20170214004";
                msgPayloadObj["Vehicle-Sensors_humidity"] = 78;
                msgPayloadObj["Vehicle-Sensors_pressure"] = 26;
                msgPayloadObj["Vehicle-Sensors_macAddr"] = "null";
                msgPayloadObj["Vehicle-Sensors_city"] = "null";

                dynamic msg = new ExpandoObject();
                msg.MessageCatalogId = 24;
                msg.AlarmRuleCatalogId = 37;
                msg.TriggeredTime = "2017-02-21T10:59:03";
                msg.Message = msgPayloadObj;

                var jsonStr = JsonConvert.SerializeObject(msg);
                _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbAlarmOpsQueue);
                _sbQueueClient.Send(new BrokeredMessage(jsonStr));
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
        }
        static void SendTestExternalMessage()
        {
            try
            {
                JObject msgPayloadObj = new JObject();
                msgPayloadObj["companyId"] = 1;
                msgPayloadObj["msgTimestamp"] = "2017-02-20T15:09:06";
                msgPayloadObj["equipmentId"] = "EquipmentIDA";
                msgPayloadObj["equipmentRunStatus"] = 1;
                msgPayloadObj["RPM"] = 5415;
                msgPayloadObj["Temperature"] = 59.5;
                msgPayloadObj["VehicleId"] = "SN20170214004";
                msgPayloadObj["Vehicle-Sensors_humidity"] = 78;
                msgPayloadObj["Vehicle-Sensors_pressure"] = 26;
                msgPayloadObj["Vehicle-Sensors_macAddr"] = "null";
                msgPayloadObj["Vehicle-Sensors_city"] = "null";

                dynamic msg = new ExpandoObject();
                msg.MessageCatalogId = 24;
                msg.AlarmRuleCatalogId = 34;
                msg.TriggeredTime = "2017-02-21T10:59:03";
                msg.Message = msgPayloadObj;

                var jsonStr = JsonConvert.SerializeObject(msg);
                _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbAlarmOpsQueue);
                _sbQueueClient.Send(new BrokeredMessage(jsonStr));
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
        }

        static void SendTestIoTDeviceMessage()
        {
            try
            {
                JObject msgPayloadObj = new JObject();
                msgPayloadObj["companyId"] = 1;
                msgPayloadObj["msgTimestamp"] = "2017-02-20T15:09:06";
                msgPayloadObj["equipmentId"] = "EquipmentIDA";
                msgPayloadObj["equipmentRunStatus"] = 1;
                msgPayloadObj["RPM"] = 5415;
                msgPayloadObj["Temperature"] = 59.5;
                msgPayloadObj["VehicleId"] = "SN20170214004";
                msgPayloadObj["Vehicle-Sensors_humidity"] = 78;
                msgPayloadObj["Vehicle-Sensors_pressure"] = 26;
                msgPayloadObj["Vehicle-Sensors_macAddr"] = "null";
                msgPayloadObj["Vehicle-Sensors_city"] = "null";

                dynamic msg = new ExpandoObject();
                msg.MessageCatalogId = 24;
                msg.AlarmRuleCatalogId = 38;
                msg.TriggeredTime = "2017-02-21T10:59:03";
                msg.Message = msgPayloadObj;

                var jsonStr = JsonConvert.SerializeObject(msg);
                _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbAlarmOpsQueue);
                _sbQueueClient.Send(new BrokeredMessage(jsonStr));
            }
            catch (Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
        }

        static void ListenOnServiceBusQueue()
        {   
            /* Create Queue client and listen on message  */
            _sbQueueClient = QueueClient.CreateFromConnectionString(_sbConnectionString, _sbAlarmOpsQueue);
            OnMessageOptions options = new OnMessageOptions();
            options.MaxConcurrentCalls = 1;
            options.AutoComplete = true;
            string messageBody = "";
            _isRunning = true;

            _sbQueueClient.OnMessage((message) =>
            {
                try
                {
                    // Process message from queue.
                    messageBody = message.GetBody<string>();
                    JObject jsonMessage = JObject.Parse(messageBody);

                    if (jsonMessage["command"] != null)
                    {
                        string command = jsonMessage["command"].ToString();
                        int taskId = int.Parse(jsonMessage["taskId"].ToString());
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
                    }
                    else
                    {
                        _sfAppLogger.Info("Received Message: " + messageBody);
                        AlarmtoApplicationHelper helper = new AlarmtoApplicationHelper(jsonMessage);

                        Thread thread = new Thread(new ThreadStart(helper.ThreadProc));
                        thread.IsBackground = false;
                        thread.Start();
                    }                        
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    Console.WriteLine(ex.Message);
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("Exception: " + ex.Message);
                    logMessage.AppendLine("Message: " + messageBody);
                    _sfAppLogger.Error(logMessage);
                    message.Complete();
                }
            }, options);
        }

        static void SendProcessorHeartbeat()
        {
            if (_sbNameSpaceMgr == null)
                _sbNameSpaceMgr = Microsoft.ServiceBus.NamespaceManager.CreateFromConnectionString(_sbConnectionString);

            _HBTimer = new Timer(new TimerCallback(PushHeartbeatSignal));
            _HBTimer.Change(10000, 10000);
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
                logMessage.AppendLine("OpsAlarm Exception on send Heartbeat: " + ex.Message);
                _sfAppLogger.Error(logMessage);
            }
        }

        static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "Ops Alarm";
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;

            if (_isRunning)
                HeartbeatMessage.status = "Running";
            else
                HeartbeatMessage.status = "Stop";

            if (_sbNameSpaceMgr != null)
                HeartbeatMessage.queueLength = _sbNameSpaceMgr.GetQueue(_sbAlarmOpsQueue).MessageCount;
            else
                HeartbeatMessage.queueLength = -1;  //Unknow

            HeartbeatMessage.cpu = Math.Round(_cpuCounter.NextValue(), 2) + " %";
            HeartbeatMessage.ramAvail = _ramCounter.NextValue() + " MB";

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
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
    }
}
