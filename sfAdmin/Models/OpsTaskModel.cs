using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace sfAdmin.Models
{
    public class OpsTaskModel
    {
        public string Name;
        public string TaskStatus;
        public int CompanyId;
        public int RetryCounter;
        public DateTime CompletedAt;
        public string Entity;
        public string EntityId;
        public string TaskContent;

        public OpsTaskModel() { }

        public OpsTaskModel(string name, int companyId, string entity, string entityId, string taskcontent)
        {
            Name = name;
            TaskStatus = "Submit";
            CompanyId = companyId;
            RetryCounter = 0;
            Entity = entity;
            EntityId = entityId;
            TaskContent = taskcontent;
        }

        public string GetPostData()
        {
            string postData = "";
            postData = postData + "&Name=" + this.Name;
            postData = postData + "&TaskStatus=" + this.TaskStatus;
            postData = postData + "&RetryCounter=" + this.RetryCounter;
            postData = postData + "&CompanyId=" + this.CompanyId;
            postData = postData + "&Entity=" + this.Entity;
            postData = postData + "&EntityId=" + this.EntityId;
            postData = postData + "&TaskContent=" + this.TaskContent;
            return postData;
        }
    }
    public class OpsInfraMessage
    {
        public string job;
        public string entity;
        public string entityId;
        public string task;
        public int taskId;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;

        public OpsInfraMessage(string job, string entity, string entityId, string task, int taskId, string requester, string requesterEmail)
        {
            this.job = job;
            this.entity = entity;
            this.entityId = entityId;
            this.task = task;
            this.taskId = taskId;
            this.requester = requester;
            this.requesterEmail = requesterEmail;
            this.requestDateTime = DateTime.UtcNow;
        }

        public void Send()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(Global._sfServiceBusConnectionString);

            if (!namespaceManager.QueueExists(Global._sfInfraOpsQueue))
            {
                namespaceManager.CreateQueue(Global._sfInfraOpsQueue);
            }
            QueueClient Client = QueueClient.CreateFromConnectionString(Global._sfServiceBusConnectionString, Global._sfInfraOpsQueue);

            BrokeredMessage message = new BrokeredMessage(GetJsonContent());
            Client.Send(message);
        }

        public string GetJsonContent()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class IoTHubEventProcessTopic
    {
        public string command;
        public string parameters;
        public int taskId;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;

        public IoTHubEventProcessTopic(string command, string parameters, int taskId, string requester, string requesterEmail)
        {
            this.command = command;
            this.parameters = parameters;
            this.taskId = taskId;
            this.requester = requester;
            this.requesterEmail = requesterEmail;
            this.requestDateTime = DateTime.UtcNow;
        }

        public void Send()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(Global._sfServiceBusConnectionString);

            if (!namespaceManager.TopicExists(Global._sfProcessCommandTopic))
            {
                namespaceManager.CreateTopic(Global._sfProcessCommandTopic);
            }
            TopicClient Client = TopicClient.CreateFromConnectionString(Global._sfServiceBusConnectionString, Global._sfProcessCommandTopic);
            BrokeredMessage message = new BrokeredMessage(GetJsonContent());

            message.Properties["process"] = "iothubeventprocessor";
            message.Properties["iothubalias"] = parameters;

            Client.Send(message);
        }

        public string GetJsonContent()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class ProcessorCommandMessage
    {
        public string command;
        public string program;
        public string parameters;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;
        public int taskId;

        public ProcessorCommandMessage() { }
        public string GetJsonContent()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}