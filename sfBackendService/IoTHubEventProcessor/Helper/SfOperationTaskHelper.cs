using IoTHubEventProcessor.Utilities;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Helper
{
    public class SfOperationTaskHelper
    {
        public static void RestartIoTHubRecevicer(int companyId, string ioTHubAlias, string requester, string requesterEmail)
        {
            string command = "restart iothub receiver";

            ProcessorCommandMessage processorCommandMessage = 
                new ProcessorCommandMessage(
                    command,
                    ioTHubAlias,
                    requester,
                    requesterEmail);

            OpsTaskModel opsTask = new OpsTaskModel(command,
                companyId, "IoTHubAlias", ioTHubAlias, processorCommandMessage.GetJsonContent());

            OperationTask task = new OperationTask();
            task.Name = opsTask.Name;
            task.TaskStatus = opsTask.TaskStatus;
            task.RetryCounter = opsTask.RetryCounter;
            task.CompanyId = opsTask.CompanyId;
            task.Entity = opsTask.Entity;
            task.EntityId = opsTask.EntityId;
            task.TaskContent = opsTask.TaskContent;

            int newOperationTaskId = addOperationTask(task);
            if(newOperationTaskId > 0)
            {
                processorCommandMessage.taskId = newOperationTaskId;
                processorCommandMessage.PublishProcessorCommandMessage();
            }
        }

        private static int addOperationTask(OperationTask operationTask)
        {
            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            DateTime invaildDatetime = new DateTime(1, 1, 1);

            var newOperationTask = new OperationTask()
            {
                Name = operationTask.Name,
                TaskStatus = operationTask.TaskStatus,
                CompanyId = operationTask.CompanyId,
                CompletedAt = (operationTask.CompletedAt.Equals(invaildDatetime)) ? null : operationTask.CompletedAt,
                RetryCounter = operationTask.RetryCounter,
                Entity = operationTask.Entity,
                EntityId = operationTask.EntityId,
                TaskContent = operationTask.TaskContent,
                TaskLog = operationTask.TaskLog
            };
            int operationTaskId = dbhelp.Add(newOperationTask);
            return operationTaskId;
        }
    }

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

}
