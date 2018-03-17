using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Text;
using System.Dynamic;

namespace sfAdmin.Models
{
    public class IoTDeviceCmdMsg
    {
        public string job;
        public string entity;
        public string entityId;
        public string task;
        public int taskId;
        public string oldPrimaryIothubConnectionString;
        public string oldSecondaryIothubConnectionString;
        public string primaryIothubConnectionString;
        public string secondaryIothubConnectionString;
        public string iothubDeviceId;
        public string authenticationType;
        public string iothubDeviceKey;
        public string certificateThumbprint;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;

        public IoTDeviceCmdMsg(string task, string deviceId, string authType, string requester, string requesterEmail, int taskId)
        {
            this.job = "provisioning iotdevice";
            this.entity = "iotdevice";
            this.entityId = deviceId;
            this.task = task;
            this.taskId = taskId;
            this.iothubDeviceId = deviceId;
            this.authenticationType = authType;
            this.requester = requester;
            this.requesterEmail = requesterEmail;
            this.requestDateTime = DateTime.UtcNow;
        }

        public async Task Init(string iotHubAlias, string certificateId = null, string oldIoTHubAlias = null)
        {
            await Init_IoTHubConnectionString(iotHubAlias);

            if (!string.IsNullOrEmpty(oldIoTHubAlias))
                await Init_oldIoTHubConnectionString(oldIoTHubAlias);

            if (this.authenticationType.ToLower() == "key")
                await Init_IoTDeviceKey();
            else if (!string.IsNullOrEmpty(certificateId))
            {
                await Init_CertificateThumbprint(certificateId);
            }
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
            insensitiveObject.authenticationType = this.authenticationType;
            insensitiveObject.certificateThumbprint = this.certificateThumbprint;
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

        private string GenerateIoTDeviceKey()
        {
            return Base64Encode(Guid.NewGuid().ToString().Substring(0, 30));
        }

        private string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private async Task Init_IoTDeviceKey()
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._iotDeviceEndPoint;
            endPoint = endPoint + "/" + this.iothubDeviceId;
            string jsonString = await apiHelper.callAPIService("GET", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            this.iothubDeviceKey = jsonResult.IoTHubDeviceKey;
            if (string.IsNullOrEmpty(this.iothubDeviceKey))
                this.iothubDeviceKey = GenerateIoTDeviceKey();
        }

        private async Task Init_CertificateThumbprint(string certificateId)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._deviceCertificateEndPoint;
            endPoint = endPoint + "/" + certificateId;
            string jsonString = await apiHelper.callAPIService("get", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            this.certificateThumbprint = jsonResult.Thumbprint;
        }

        private async Task Init_IoTHubConnectionString(string iotHubAlias)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._iotHubEndPoint;
            endPoint = endPoint + "/" + iotHubAlias;
            string jsonString = await apiHelper.callAPIService("get", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            this.primaryIothubConnectionString = jsonResult.P_IoTHubConnectionString;
            this.secondaryIothubConnectionString = jsonResult.S_IoTHubConnectionString;
        }

        private async Task Init_oldIoTHubConnectionString(string iotHubAlias)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._iotHubEndPoint;
            endPoint = endPoint + "/" + iotHubAlias;
            string jsonString = await apiHelper.callAPIService("get", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            this.oldPrimaryIothubConnectionString = jsonResult.P_IoTHubConnectionString;
            this.oldSecondaryIothubConnectionString = jsonResult.S_IoTHubConnectionString;
        }
    }
}