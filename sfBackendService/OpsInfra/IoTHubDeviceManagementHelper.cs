using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Threading;
using System.Dynamic;
using sfShareLib;

namespace OpsInfra
{
    class IoTHubDeviceManagementMessageModel
    {
        public string taskId { get; set; }
        public string primaryIothubConnectionString { get; set; }
        //public string secondaryIothubConnectionString { get; set; }
        public string iothubDeviceId { get; set; }
        public string iothubAlias { get; set; }
        public bool iothubIsPrimary { get; set; }
        public JObject deviceConfiguration { get; set; }
        public string taskContent { get; set; }
    }
    class IoTHubDeviceManagementHelper
    {        
        public int _TaskId;
        public string _PrimaryIothubConnectionString;
        //public string _SecondaryIothubConnectionString;
        public string _IoTHubDeviceId;
        public JObject _DeviceConfiguration;    //include SF_SystemConfig, SF_SystemConfig, SF_LastUpdatedTimestamp
        public string _IoTHubAlias;
        public bool _IoTHubIsPrimary;
        public string _TaskContent;
        public string _Action;
        private int _Status; //Number of faild IoTHub

        public IoTHubDeviceManagementHelper(IoTHubDeviceManagementMessageModel message, string task)
        {
            _TaskId = Convert.ToInt32(message.taskId);
            _PrimaryIothubConnectionString = message.primaryIothubConnectionString;
            //_SecondaryIothubConnectionString = message.secondaryIothubConnectionString;
            _DeviceConfiguration = message.deviceConfiguration;
            _IoTHubDeviceId = message.iothubDeviceId;
            _IoTHubAlias = message.iothubAlias;
            _IoTHubIsPrimary = message.iothubIsPrimary;
            _TaskContent = message.taskContent;

            if (task.Equals("update device desired property"))
                _Action = "update deired";
            else if (task.Equals("update device reported property to db"))
                _Action = "update db device reported";
        }

        public async void ThreadProc()
        {      
            try
            {                
                string exceptionString = "";
                DBHelper._IoTDevice dbhelper = new DBHelper._IoTDevice();
                DBHelper._IoTHub dbhelper_iotHub = new DBHelper._IoTHub();
                IoTDevice iotDevice = dbhelper.GetByid(_IoTHubDeviceId);
                int deviceConfigurationStatus = iotDevice.DeviceConfigurationStatus;
                switch (_Action)
                {
                    case "update deired":
                        exceptionString += await UpdateDeviceConfigurationToTwins(_PrimaryIothubConnectionString, "primary");
                        //exceptionString += await UpdateDeviceConfigurationToTwins(_SecondaryIothubConnectionString, "secondary");
                        if (_Status != TaskStatus.FAILED)
                            dbhelper.UpdateDeviceConfigurationStatusAndProperty(_IoTHubDeviceId, IoTDeviceConfigurationStatus.WAITING_DEVICE_ACK);
                        break;
                    case "update db device reported":
                        {
                            Thread.Sleep(30000);
                            string iotHubConnectionString = "";

                            //Get IoT Hub Device Twins Reported Value
                            IoTHub iotHub = dbhelper_iotHub.GetByid(_IoTHubAlias);
                            if (_IoTHubIsPrimary)
                                iotHubConnectionString = iotHub.P_IoTHubConnectionString;
                            else
                                iotHubConnectionString = iotHub.S_IoTHubConnectionString;

                            string reportedObjJsonString = await GetDeviceTwinsReportedValue(iotHubConnectionString, _IoTHubDeviceId);

                            Console.WriteLine(reportedObjJsonString);

                            //update db
                            dbhelper.UpdateDeviceConfigurationStatusAndProperty(_IoTHubDeviceId, IoTDeviceConfigurationStatus.RECEIVE_DEVICE_ACK, null, reportedObjJsonString);

                            return;
                            //recode to operationTask
                        }
                        break;
                }               
                
                if (exceptionString == "")
                {
                    Program.UpdateTaskBySuccess(_TaskId);
                    Console.WriteLine("[DeviceManagement] Apply device configuration to IoTHub desired property success: IoTHubDeviceId-" + _IoTHubDeviceId);
                }
                else
                    throw new Exception(exceptionString);
            }
            catch(Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("[DeviceManagement] Apply device configuration to IoTHub desired property faild: IoTHubDeviceId-" + _IoTHubDeviceId);
                logMessage.AppendLine("\tMessage:" + JsonConvert.SerializeObject(this));
                logMessage.AppendLine("\tException:" + ex.Message);
                Program._sfAppLogger.Error(logMessage);
                Program.UpdateTaskByFail(_TaskId, Program.FilterErrorMessage(ex.Message), _Status);
                Console.WriteLine(logMessage);
                _Status = 0;
            }
            
        }

        private async Task<string> GetDeviceTwinsReportedValue(string connectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);

            var query = registryManager.CreateQuery("SELECT * FROM devices WHERE deviceId = '" + _IoTHubDeviceId + "'");
            var results = await query.GetNextAsTwinAsync();
            JObject reportedObj = JObject.Parse(results.FirstOrDefault().Properties.Reported.ToJson());
            dynamic sfReportedObj = new
            {
                SF_SystemConfig = reportedObj["SF_SystemConfig"],
                SF_CustomizedConfig = reportedObj["SF_CustomizedConfig"],
                SF_LastUpdatedTimestamp = reportedObj["SF_LastUpdatedTimestamp"]
            };

            return JsonConvert.SerializeObject(sfReportedObj);
        }
        private async Task<string> UpdateDeviceConfigurationToTwins(string connectionString, string iotHubType)
        {
            RegistryManager registryManager;
            
            try
            {
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                var twin = await registryManager.GetTwinAsync(_IoTHubDeviceId);

                //Clean old desired property
                dynamic nullProperty = new ExpandoObject();
                nullProperty.SF_SystemConfig = null;
                nullProperty.SF_CustomizedConfig = null;
                nullProperty.SF_LastUpdatedTimestamp = 0;

                var patch = new
                {
                    properties = new
                    {
                        desired = nullProperty
                    }
                };
                twin = await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);
                
                //Update IoTHub desired property
                int nowUnixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                dynamic _DeviceTwinsDesiredPropertyObj = new
                {
                    SF_SystemConfig = _DeviceConfiguration["SF_SystemConfig"],
                    SF_CustomizedConfig = _DeviceConfiguration["SF_CustomizedConfig"],
                    SF_LastUpdatedTimestamp = nowUnixTimestamp
                };                
                patch = new
                {
                    properties = new
                    {
                        desired = _DeviceTwinsDesiredPropertyObj
                    }
                };
                twin = await registryManager.UpdateTwinAsync(twin.DeviceId, JsonConvert.SerializeObject(patch), twin.ETag);                

                //while (true)
                //{
                //    var query = registryManager.CreateQuery("SELECT * FROM devices WHERE deviceId = '" + _IoTHubDeviceId + "'");
                //    var results = await query.GetNextAsTwinAsync();
                //    foreach (var result in results)
                //    {
                //        Console.WriteLine("Config report for: {0}", result.DeviceId);
                //        Console.WriteLine("Desired SF_SystemConfig: {0}", JsonConvert.SerializeObject(result.Properties.Desired["SF_SystemConfig"], Formatting.Indented));
                //        Console.WriteLine("Desired SF_CustomizedConfig: {0}", JsonConvert.SerializeObject(result.Properties.Desired["SF_CustomizedConfig"], Formatting.None));
                //        Console.WriteLine("Desired SF_LastUpdatedTimestamp: {0}", JsonConvert.SerializeObject(result.Properties.Desired["SF_LastUpdatedTimestamp"], Formatting.None));
                //        Console.WriteLine("Desired telemetryConfig: {0}", JsonConvert.SerializeObject(result.Properties.Desired["$version"], Formatting.None));
                //        Console.WriteLine("Desired telemetryConfig: {0}", JsonConvert.SerializeObject(result.Properties.Desired, Formatting.None));

                //        Console.WriteLine("Reported SF_SystemConfig: {0}", JsonConvert.SerializeObject(result.Properties.Reported["SF_SystemConfig"], Formatting.Indented));
                //        Console.WriteLine("Reported SF_CustomizedConfig: {0}", JsonConvert.SerializeObject(result.Properties.Reported["SF_CustomizedConfig"], Formatting.None));
                //        Console.WriteLine("Reported SF_LastUpdatedTimestamp: {0}", JsonConvert.SerializeObject(result.Properties.Reported["SF_LastUpdatedTimestamp"], Formatting.None));
                //        Console.WriteLine("Reported telemetryConfig: {0}", JsonConvert.SerializeObject(result.Properties.Reported["$version"], Formatting.None));
                //        Console.WriteLine("Reported telemetryConfig: {0}", JsonConvert.SerializeObject(result.Properties.Reported, Formatting.None));
                //        Console.WriteLine("=========================================================================");
                //    }
                //    Thread.Sleep(15000);
                //}
                //return null;
                return "";
            }
            catch (Exception ex)
            {
                _Status++;
                return "\t Cann't connect " + iotHubType + " IoTHub:" + Program.GetNonJsonExceptionMessage(ex.Message) + "\n";
            }
        }

    }
}
