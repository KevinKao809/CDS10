using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;

using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class OperationTaskModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string TaskStatus { get; set; }
            public DateTime? CompletedAt { get; set; }
            public string RetryCounter { get; set; }
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string Entity { get; set; }
            public string EntityId { get; set; }
            public JObject TaskContent { get; set; }
            public string TaskLog { get; set; }
        }

        public class Add
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            [MaxLength(20)]
            public string TaskStatus { get; set; }
            public DateTime? CompletedAt { get; set; }
            public int RetryCounter { get; set; }
            [Required]
            public int CompanyId { get; set; }
            [MaxLength(50)]
            public string Entity { get; set; }
            public string EntityId { get; set; }
            public string TaskContent { get; set; }
            public string TaskLog { get; set; }
        }

        public class Update
        {
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            [MaxLength(20)]
            public string TaskStatus { get; set; }
            public DateTime? CompletedAt { get; set; }
            public int RetryCounter { get; set; }
            [MaxLength(50)]
            public string Entity { get; set; }
            public string EntityId { get; set; }
            public string TaskContent { get; set; }
            public string TaskLog { get; set; }
        }

        public class SearchCondition
        {
            public string taskStatus { get; set; }
            public int hours { get; set; }
        }

        public List<Detail> GetAllOperationTaskByCompanyId(int companyId)
        {
            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            DBHelper._Company dbhelp_company = new DBHelper._Company();
            List<OperationTask> operationTaskList = new List<OperationTask>();
            List<Detail> returnOperationtTaskDetail = new List<Detail>();
            
            DateTime invaildDatetime = new DateTime(1, 1, 1);

            operationTaskList = dbhelp.GetAllByCompanyId(companyId);

            foreach (OperationTask operationTask in operationTaskList)
            {
                Company company = dbhelp_company.GetByid(operationTask.CompanyId);

                returnOperationtTaskDetail.Add(new Detail() {
                    Id = operationTask.Id,
                    Name = operationTask.Name,
                    TaskStatus = operationTask.TaskStatus,
                    CompanyId = operationTask.CompanyId,
                    CompanyName = company == null ? "" : company.Name,
                    CompletedAt = operationTask.CompletedAt,
                    RetryCounter = (operationTask.RetryCounter == null) ? "" : operationTask.RetryCounter.ToString(),
                    Entity = operationTask.Entity,
                    EntityId = operationTask.EntityId,
                    TaskContent = (operationTask.TaskContent == null) ? null : JObject.Parse(operationTask.TaskContent),
                    TaskLog = operationTask.TaskLog
                });
            }
            return returnOperationtTaskDetail;
        }

        public List<Detail> searchInPastSevenDaysOperations(SearchCondition condition, int companyId = 0)
        {
            int hours;
            string taskStatus;
            if (condition == null)
            {
                hours = -168;
                taskStatus = null;
            }
            else
            {
                hours = (condition.hours == 0 || condition.hours > 168) ? -168 : -(condition.hours);
                taskStatus = condition.taskStatus;
            }
            
            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            DBHelper._Company dbhelp_company = new DBHelper._Company();
            List<OperationTask> operationTaskList = new List<OperationTask>();
            List<Detail> returnOperationtTaskDetail = new List<Detail>();

            if (companyId > 0)
            {
                operationTaskList = dbhelp.Search(taskStatus, hours, companyId);
                Company company = dbhelp_company.GetByid(companyId);

                foreach (OperationTask operationTask in operationTaskList)
                {
                    try
                    {
                        returnOperationtTaskDetail.Add(new Detail()
                        {
                            Id = operationTask.Id,
                            Name = operationTask.Name,
                            TaskStatus = operationTask.TaskStatus,
                            CompanyId = operationTask.CompanyId,
                            CompanyName = company == null ? "" : company.Name,
                            CompletedAt = operationTask.CompletedAt,
                            RetryCounter = (operationTask.RetryCounter == null) ? "" : operationTask.RetryCounter.ToString(),
                            Entity = operationTask.Entity,
                            EntityId = operationTask.EntityId,
                            TaskContent = (operationTask.TaskContent == null) ? null : JObject.Parse(operationTask.TaskContent),
                            TaskLog = operationTask.TaskLog
                        });
                    }
                    catch { }
                }
            }
            else
            {
                operationTaskList = dbhelp.Search(taskStatus, hours);

                List<int> companyIdList = operationTaskList.Select(s => s.CompanyId).Distinct().ToList<int>();
                SFDatabaseEntities dbEntity = new SFDatabaseEntities();
                var L2Enty = from c in dbEntity.Company
                             where companyIdList.Contains(c.Id)
                             select new { Id = c.Id, Name = c.Name };
                Dictionary<int, string> companyTable = new Dictionary<int, string>();
                foreach (var company in L2Enty)
                {
                    companyTable.Add(company.Id, company.Name);
                }

                foreach (OperationTask operationTask in operationTaskList)
                {
                    try
                    {
                        returnOperationtTaskDetail.Add(new Detail()
                        {
                            Id = operationTask.Id,
                            Name = operationTask.Name,
                            TaskStatus = operationTask.TaskStatus,
                            CompanyId = operationTask.CompanyId,
                            CompanyName = companyTable.ContainsKey(operationTask.CompanyId) ? companyTable[operationTask.CompanyId] : "",
                            CompletedAt = operationTask.CompletedAt,
                            RetryCounter = (operationTask.RetryCounter == null) ? "" : operationTask.RetryCounter.ToString(),
                            Entity = operationTask.Entity,
                            EntityId = operationTask.EntityId,
                            TaskContent = (operationTask.TaskContent == null) ? null : JObject.Parse(operationTask.TaskContent),
                            TaskLog = operationTask.TaskLog
                        });
                    }
                    catch { }             
                }
            }

            return returnOperationtTaskDetail;
        }

        public Detail getOperationTaskById(int id)
        {
            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            DBHelper._Company dbhelp_company = new DBHelper._Company();
            OperationTask operationTask = dbhelp.GetByid(id);
            Company company = dbhelp_company.GetByid(operationTask.CompanyId);

            return new Detail()
            {
                Id = operationTask.Id,
                Name = operationTask.Name,
                TaskStatus = operationTask.TaskStatus,
                CompanyId = operationTask.CompanyId,
                CompanyName = company == null ? "" : company.Name,
                CompletedAt = operationTask.CompletedAt,
                RetryCounter = (operationTask.RetryCounter == null) ? "" : operationTask.RetryCounter.ToString(),
                Entity = operationTask.Entity,
                EntityId = operationTask.EntityId,
                TaskContent = (operationTask.TaskContent == null) ? null : JObject.Parse(operationTask.TaskContent),
                TaskLog = operationTask.TaskLog
            };
        }

        public int addOperationTask(Add operationTask)
        {
            try
            {
                if (operationTask.TaskContent != null)
                {
                    JObject tmp = JObject.Parse(operationTask.TaskContent);
                }
            }
            catch
            {
                throw new Exception("TaskContent must be in Json fromat");
            }

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

        public void updateOperationTask(int id, Update operationTask)
        {
            try
            {
                if (operationTask.TaskContent != null)
                {
                    JObject tmp = JObject.Parse(operationTask.TaskContent);
                }
            }
            catch
            {
                throw new Exception("TaskContent must be in Json fromat");
            }

            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            OperationTask existingOperationTask = dbhelp.GetByid(id);
            DateTime invaildDatetime = new DateTime(1, 1, 1);

            existingOperationTask.Name = operationTask.Name;
            existingOperationTask.TaskStatus = operationTask.TaskStatus;
            existingOperationTask.CompletedAt = (operationTask.CompletedAt.Equals(invaildDatetime)) ? null : operationTask.CompletedAt;
            existingOperationTask.RetryCounter = operationTask.RetryCounter;
            existingOperationTask.Entity = operationTask.Entity;
            existingOperationTask.EntityId = operationTask.EntityId;
            existingOperationTask.TaskContent = operationTask.TaskContent;
            existingOperationTask.TaskLog = operationTask.TaskLog;

            dbhelp.Update(existingOperationTask);
        }

        public void deleteOperationTask(int id)
        {
            DBHelper._OperationTask dbhelp = new DBHelper._OperationTask();
            OperationTask existingOperationTask = dbhelp.GetByid(id);

            dbhelp.Delete(existingOperationTask);
        }
    }
}