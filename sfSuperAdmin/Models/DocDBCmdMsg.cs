using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Dynamic;

namespace sfSuperAdmin.Models
{
    public class DocDBCmdMsg
    {
        public string job;
        public string entity;
        public string entityId;
        public string task;
        public int taskId;
        public string databaseName;
        public string collectionId;
        public string documentdbConnectionString;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;

        public DocDBCmdMsg(string action, int companyId, string requester, string requesterEmail, int taskId)
        {
            try
            {
                this.job = "provisioning company";
                this.entity = "company";
                this.entityId = companyId.ToString();
                this.task = action + " documentdb collection";
                this.taskId = taskId;
                this.databaseName = "db" + companyId;
                this.collectionId = companyId.ToString();
                this.requester = requester;
                this.requesterEmail = requesterEmail;
                this.requestDateTime = DateTime.UtcNow;
                GetDocumentDBConnectionString(companyId);
            }
            catch(Exception ex)
            {
                throw new Exception("DocumentDB Commend Message initial error: " + ex.Message);
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
            insensitiveObject.databaseName = this.databaseName;
            insensitiveObject.collectionId = this.collectionId;
            insensitiveObject.requester = this.requester;
            insensitiveObject.requesterEmail = this.requesterEmail;
            insensitiveObject.requestDateTime = requestDateTime;

            return JsonConvert.SerializeObject(insensitiveObject);
        }

        public void SendToServiceBus()
        {
            try
            {
                var client = QueueClient.CreateFromConnectionString(Global._sfServiceBusConnectionString, Global._sfInfraOpsQueue);
                var msg = JsonConvert.SerializeObject(this);
                client.Send(new BrokeredMessage(msg));

                StringBuilder logMessage = new StringBuilder();                
                logMessage.AppendLine("Service Bus Message:" + msg);
                Global._sfAppLogger.Info(logMessage);
            }
            catch(Exception ex)
            {
                throw new Exception("Send message to service bus failed: " + ex.Message);
            }
            
        }

        private async void GetDocumentDBConnectionString(int companyId)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._companyEndPoint;
            endPoint = endPoint + "/" + companyId;
            string jsonString = await apiHelper.callAPIService("GET", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            this.documentdbConnectionString = jsonResult.DocDBConnectionString;

            if (this.documentdbConnectionString == null)
                this.documentdbConnectionString = Global._sfDocDBConnectionString;
        }
    }
}