using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoutineTask.Interfaces;
using sfShareLib;

namespace RoutineTask
{
    class Action_CalculateUsageLog : Action
    {
        public Action_CalculateUsageLog(string taskName)
            : base(taskName)
        {            
        }

        public override void Exec()
        {
            RecordStartLog();
            DBHelper._Company dbhelp_Company = new DBHelper._Company();
            DBHelper._Factory dbhelp_Facotry = new DBHelper._Factory();
            DBHelper._Equipment dbhelp_Equipment = new DBHelper._Equipment();
            DBHelper._UsageLog dbhelp_UsageLog = new DBHelper._UsageLog();
            List<UsageLog> usageLogList = new List<UsageLog>();

            foreach (var company in dbhelp_Company.GetAll())
            {
                int companyId = company.Id;
                int factoryQty = 0;
                int equipmentQty = 0;
                long deviceMsgQty = 0;
                long alarmMsgQty = 0;
                int DocDbSizeInGB = 0;
                int DocDBPercentage = 0;

                try
                {
                    foreach (var factory in dbhelp_Facotry.GetAllByCompanyId(companyId))
                    {
                        factoryQty++;
                        equipmentQty += dbhelp_Equipment.GetAllByFactoryId(factory.Id).Count;
                    }

                    DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                    docDBHelpler.Init();
                    deviceMsgQty = docDBHelpler.GetCompanyDeviceMsgQty();
                    alarmMsgQty = docDBHelpler.GetCompanyAlarmMessageQty();

                    DocDbSizeInGB = Convert.ToInt32(docDBHelpler._SizeQuotaInKB / (1024 * 1024));
                    DocDBPercentage = Convert.ToInt32(docDBHelpler._SizeUsageInKB * 100 / docDBHelpler._SizeQuotaInKB);
                }
                catch (Exception ex)
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("Routine TaskActor : " + _TaskName);
                    logMessage.AppendLine("\tCompayId " + companyId + " is failed");
                    logMessage.AppendLine("\tException: " + ex.Message);
                    if(ex.InnerException != null)
                        logMessage.AppendLine("\tInnerException: " + ex.InnerException.Message);
                    _sfAppLogger.Error(logMessage);                    
                }

                usageLogList.Add(new UsageLog()
                {
                    CompanyId = companyId,
                    FactoryQty = factoryQty,
                    EquipmentQty = equipmentQty,
                    DeviceMessage = deviceMsgQty,
                    AlarmMessage = alarmMsgQty,
                    DocSizeInGB = DocDbSizeInGB,
                    DocDBPercentage = DocDBPercentage,
                    UpdatedAt = DateTime.Now
                });
            }
            try
            {
                dbhelp_UsageLog.Add(usageLogList);
            }
            catch(Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Routine TaskActor : " + _TaskName);
                logMessage.AppendLine("\tException: " + ex.Message);
                logMessage.AppendLine("\tInnerException: " + ex.InnerException.Message);
                _sfAppLogger.Error(logMessage);
            }
            
            RecordEndLog();
        }
    }
}
