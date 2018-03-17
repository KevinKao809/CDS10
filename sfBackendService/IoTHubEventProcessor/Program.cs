using IoTHubEventProcessor.Utilities;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json.Linq;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IoTHubEventProcessor
{
    class Program
    {
        public static string _IoTHubAlias;
        static SubscriptionClient _sbSubscriptionClient;
        public static string _sbConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        public static string _sbAlarmOpsQueue = ConfigurationManager.AppSettings["sfAlarmOpsQueue"];
        public static string _sbInfraOpsQueue = ConfigurationManager.AppSettings["sfInfraOpsQueue"];
        static string _sbProcessCommandTopic = ConfigurationManager.AppSettings["sfProcessCommandTopic"];
        static string _adminHeartbeatURL = ConfigurationManager.AppSettings["sfAdminHeartbeatURL"];
        static string _superadminHeartbeatURL = ConfigurationManager.AppSettings["sfSuperAdminHeartbeatURL"];
        public static string _sfDocDBConnectionString = ConfigurationManager.AppSettings["sfDocDBConnectionString"];
        static IoTHubAliasEventMessageReceiver _IoTHubMessageReceiver;
        static Timer _HBTimer = null;
        static WebUtility webUtility = new WebUtility();
        public static char REPLACE_SPACE_TO_DASH = '-';

        static int Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    ConsoleLog.WriteToConsole("Usage: IoTHubEventProcessor <IoTHubAlias>");
            //    Environment.Exit(0);
            //}

            //GetIoTHubAliasFromArgs(args);
            GetIoTHubAlias();

            if (string.IsNullOrEmpty(_IoTHubAlias))
            {
                ConsoleLog.WriteBlobLogError("Can't find IoTHubAlias from EnvironmentVariable");
                Environment.Exit(0);
            }

            ConsoleLog.WriteToConsole("IoTHubAlias:" + _IoTHubAlias);
            ConsoleLog.WriteBlobLogInfo("IoTHubAlias:" + _IoTHubAlias);

            try
            {
                SendProcessorHeartbeat();

                _IoTHubMessageReceiver = new IoTHubAliasEventMessageReceiver(_IoTHubAlias);
                _IoTHubMessageReceiver.Start().Wait();

                ListenOnServiceBusTopic();
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteToConsole(ex.Message + "\t" + ex.InnerException);
                ConsoleLog.WriteBlobLogError("Exception: {0}\t{1}", ex.Message, ex.InnerException);

                if (_IoTHubMessageReceiver != null)
                    _IoTHubMessageReceiver.Stop().Wait();

                Environment.Exit(-1);
            }

            AvoidAppFinish();

            Console.ReadLine();
            _IoTHubMessageReceiver.Stop().Wait();

            return 0;
        }

        static void GetIoTHubAliasFromArgs(string[] args)
        {
            if (args.Length == 0)
            {
                ConsoleLog.WriteToConsole("Usage: IoTHubEventProcessor <IoTHubAlias>");
                Environment.Exit(0);
            }

            _IoTHubAlias = args[0];
        }

        static void GetIoTHubAlias()
        {            
            _IoTHubAlias = Environment.GetEnvironmentVariable("IoTHubAlias");
        }

        [Conditional("RELEASE")]
        static void AvoidAppFinish()
        {
            //avoid app finish
            while (true)
                Console.ReadLine();
        }

        static void ListenOnServiceBusTopic()
        {
            /* Create Topic Subscription Client, and bind with Message Property on companyid = xx */
            var namespaceManager = NamespaceManager.CreateFromConnectionString(_sbConnectionString);
            //string subscriptionName = "iothubeventprocessor_iothubalias_" + _IoTHubAlias;
            string subscriptionName = GetEventProcessorHostName(_IoTHubAlias);
            SqlFilter messageFilter = new SqlFilter("process = 'iothubeventprocessor' AND iothubalias = '" + _IoTHubAlias + "'");

            /* If the subscription not exist, create it. */
            if (!namespaceManager.SubscriptionExists(_sbProcessCommandTopic, subscriptionName))
                namespaceManager.CreateSubscription(_sbProcessCommandTopic, subscriptionName, messageFilter);

            /* Create subscription client and listen on message  */
            _sbSubscriptionClient = SubscriptionClient.CreateFromConnectionString(_sbConnectionString, _sbProcessCommandTopic, subscriptionName);
            OnMessageOptions options = new OnMessageOptions();
            options.AutoComplete = true;

            _sbSubscriptionClient.OnMessage(async (message) =>
            {
                string command = "", messageBody = "";
                int taskId = 0;

                try
                {
                    // Process message from subscription.
                    messageBody = message.GetBody<string>();
                    ConsoleLog.WriteBlobLogDebug("onMessage: {0}", messageBody);

                    JObject jsonMessage = JObject.Parse(messageBody);

                    if (jsonMessage["command"] != null)
                        command = jsonMessage["command"].ToString();
                    if (jsonMessage["taskId"] != null)
                        taskId = int.Parse(jsonMessage["taskId"].ToString());

                    ConsoleLog.WriteToConsole("Command:" + command);
                    ConsoleLog.WriteBlobLogInfo("Received Command:" + command);

                    switch (command.ToLower())
                    {
                        case "start":
                            await _IoTHubMessageReceiver.Start();
                            break;
                        case "stop":
                            await _IoTHubMessageReceiver.Stop();
                            break;
                        case "restart":
                            await _IoTHubMessageReceiver.Stop();
                            await _IoTHubMessageReceiver.Start();
                            break;
                        case "shutdown":
                            message.Complete();
                            await _IoTHubMessageReceiver.Stop();
                            UpdateTaskBySuccess(taskId);
                            Environment.Exit(0);
                            break;
                    }

                    UpdateTaskBySuccess(taskId);
                }
                catch (Exception ex)
                {
                    // Indicates a problem, unlock message in subscription.
                    ConsoleLog.WriteToConsole(ex.Message);
                    ConsoleLog.WriteBlobLogError("Exception: {0}", ex.Message);

                    message.Complete();

                    UpdateTaskByFail(taskId, ex.Message);
                }
            }, options);
        }

        static void UpdateTaskBySuccess(int taskId)
        {
            DBHelper._OperationTask opsTaskHelper = new DBHelper._OperationTask();
            OperationTask opsTask = opsTaskHelper.GetByid(taskId);
            opsTask.CompletedAt = DateTime.UtcNow;
            opsTask.TaskLog = DateTime.UtcNow + ": Done.";
            opsTask.TaskStatus = "Completed";

            opsTaskHelper.Update(opsTask);
        }

        static void UpdateTaskByFail(int taskId, string failLog)
        {
            try
            {
                DBHelper._OperationTask opsTaskHelper = new DBHelper._OperationTask();
                OperationTask opsTask = opsTaskHelper.GetByid(taskId);
                opsTask.RetryCounter = opsTask.RetryCounter + 1;
                opsTask.TaskStatus = "Fail";
                opsTask.TaskLog = DateTime.UtcNow + ": " + failLog;

                opsTaskHelper.Update(opsTask);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteBlobLogError("Exception on UpdateTaskByFail: {0}", ex.Message);
            }
        }

        static void SendProcessorHeartbeat()
        {
            _HBTimer = new Timer(new TimerCallback(PushHeartbeatSignal));
            _HBTimer.Change(10000, 10000);
        }

        static void PushHeartbeatSignal(object state)
        {
            string jsonHB = _IoTHubMessageReceiver.GetHeartbeatStatus();
            try
            {
                webUtility.PostContent(_adminHeartbeatURL, jsonHB);
                webUtility.PostContent(_superadminHeartbeatURL, jsonHB);
            }
            catch (Exception ex)
            {
                ConsoleLog.WriteBlobLogError("Exception on send Heartbeat: {0}", ex.Message);
            }
        }

        public static string GetEventProcessorHostName(string iothubAlias)
        {
            string name = "ephost_iothub_" + _IoTHubAlias;
            if (name.Contains(" "))
                name = name.Replace(' ', REPLACE_SPACE_TO_DASH);

            return name;
        }
    }
}
