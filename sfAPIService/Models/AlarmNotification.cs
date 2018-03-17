using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class AlarmNotificationModels
    {
        public class Detail
        {
            public int? ExternalApplicationId { get; set; }
            public string ExternalApplicationName { get; set; }
        }
        public class Edit
        {
            public int[] ExternalApplicationIdList { get; set; }
        }

        public List<Detail> GetAllExternalApplicationByAlarmRuleCatalogId(int alarmNotificationId)
        {
            DBHelper._AlarmNotification dbhelp = new DBHelper._AlarmNotification();

            return dbhelp.GetAllByAlarmRuleCatalogId(alarmNotificationId).Select(s => new Detail()
            {
                ExternalApplicationId = s.ExternalApplicationId,
                ExternalApplicationName = (s.ExternalApplication == null) ? "" : s.ExternalApplication.Name
            }).ToList<Detail>();

        }

        public void AttachExternalApplication(int alarmNotificationId, Edit AlarmNotification)
        {
            DBHelper._AlarmNotification dbhelp = new DBHelper._AlarmNotification();
            List<AlarmNotification> newExternalApplicationList = new List<AlarmNotification>();
            List<AlarmNotification> existExternalApplicationList = dbhelp.GetAllByAlarmRuleCatalogId(alarmNotificationId);

            dbhelp.Delete(existExternalApplicationList);
            if (AlarmNotification != null)
            {
                foreach (int ExternalApplicationId in AlarmNotification.ExternalApplicationIdList)
                {
                    if (ExternalApplicationId > 0)
                    {
                        newExternalApplicationList.Add(new AlarmNotification()
                        {
                            AlarmRuleCatalogId = alarmNotificationId,
                            ExternalApplicationId = ExternalApplicationId
                        });
                    }                    
                }
            }

            dbhelp.Add(newExternalApplicationList);
        }
    }
}