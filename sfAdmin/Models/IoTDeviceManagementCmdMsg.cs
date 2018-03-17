using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace sfAdmin.Models
{
    public class IoTDeviceManagementCmdMsg
    {
        public string job;
        public string entity;
        public string entityId;
        public string task;
        public int taskId;
        public string primaryIothubConnectionString;
        public string secondaryIothubConnectionString;
        public string iothubDeviceId;
        public string deviceConfiguration;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;

        public IoTDeviceManagementCmdMsg(string task, string deviceId, string requester, string requesterEmail, int taskId, string devicConfig)
        {
            this.job = "device management";
            this.entity = "iotdevice";
            this.entityId = deviceId;
            this.task = task;
            this.taskId = taskId;
            this.iothubDeviceId = deviceId;
            this.deviceConfiguration = devicConfig;
            this.requester = requester;
            this.requesterEmail = requesterEmail;
            this.requestDateTime = DateTime.UtcNow;
        }

        public async Task Init()
        {
            await Init_IoTHubConnectionString(this.iothubDeviceId);
        }

        private async Task Init_IoTHubConnectionString(string iotDeviceId)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string jsonString = null;
            dynamic jsonResult = null;

            string endPoint_iotdevice = Global._iotDeviceEndPoint + "/" + iotDeviceId;
            jsonString = await apiHelper.callAPIService("get", endPoint_iotdevice, null);
            jsonResult = JObject.Parse(jsonString);
            string iotHubAlias = jsonResult.IoTHubAlias;

            string endPoint_iotHub = Global._iotHubEndPoint + "/" + iotHubAlias;
            jsonString = await apiHelper.callAPIService("get", endPoint_iotHub, null);
            jsonResult = JObject.Parse(jsonString);
            this.primaryIothubConnectionString = jsonResult.P_IoTHubConnectionString;
            this.secondaryIothubConnectionString = jsonResult.S_IoTHubConnectionString;
        }

        public string GetJsonInsensitiveContent()
        {
            dynamic insensitiveObject = new ExpandoObject();

            insensitiveObject.job = this.job;
            insensitiveObject.entity = this.entity;
            insensitiveObject.entityId = this.entityId;
            insensitiveObject.task = this.task;
            insensitiveObject.taskId = this.taskId;
            insensitiveObject.iothubDeviceId = this.iothubDeviceId;
            insensitiveObject.requester = this.requester;
            insensitiveObject.requesterEmail = this.requesterEmail;
            insensitiveObject.requestDateTime = requestDateTime;

            return JsonConvert.SerializeObject(insensitiveObject);
        }

        public void SendToServiceBus()
        {
            BrokeredMessage message = new BrokeredMessage(JsonConvert.SerializeObject(this));

            try
            {
                var client = QueueClient.CreateFromConnectionString(Global._sfServiceBusConnectionString, Global._sfInfraOpsQueue);
                client.Send(message);

                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Service Message:" + message.ToString());
                Global._sfAppLogger.Info(logMessage);

            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Error on Send Message to Service Bus:" + ex.Message);
                logMessage.AppendLine("Service Message:" + message.ToString());
                Global._sfAppLogger.Error(logMessage);

                throw;
            }
        }
    }
}