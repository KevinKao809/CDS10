using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace sfAPIService.Models
{
    public class UsagLogModels
    {
        public class Detail
        {
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public int? FactoryQty { get; set; }
            public int? EquipmentQty { get; set; }
            public long DeviceMessage { get; set; }
            public long AlarmMessage { get; set; }
            public int DocSizeInGB { get; set; }
            public int DocDBPercentage { get; set; }
            public DateTime UpdatedAt { get; set; }
        }

        public List<Detail> getAll(int days, string order)
        {
            DBHelper._UsageLog dbhelp = new DBHelper._UsageLog();

            return dbhelp.GetAll(days, order).Select(s => new Detail()
            {
                CompanyId = s.CompanyId,
                CompanyName = s.Company == null ? "" : s.Company.Name,
                FactoryQty = s.FactoryQty,
                EquipmentQty = s.EquipmentQty,
                DeviceMessage = s.DeviceMessage,
                AlarmMessage = s.AlarmMessage,
                DocSizeInGB = s.DocSizeInGB,
                DocDBPercentage = s.DocDBPercentage,
                UpdatedAt = s.UpdatedAt
            }).ToList<Detail>();
        }
        public List<Detail> getAllByCompanyId(int companyId, int days, string order)
        {
            DBHelper._UsageLog dbhelp = new DBHelper._UsageLog();

            return dbhelp.GetAllByCompanyId(companyId, days, order).Select(s => new Detail()
            {
                CompanyId = s.CompanyId,
                CompanyName = s.Company == null ? "" : s.Company.Name,
                FactoryQty = s.FactoryQty,
                EquipmentQty = s.EquipmentQty,
                DeviceMessage = s.DeviceMessage,
                AlarmMessage = s.AlarmMessage,
                DocSizeInGB = s.DocSizeInGB,
                DocDBPercentage = s.DocDBPercentage,
                UpdatedAt = s.UpdatedAt
            }).ToList<Detail>();
        }

        public Detail getLastByCompanyId(int companyId)
        {
            DBHelper._UsageLog dbhelp = new DBHelper._UsageLog();
            UsageLog usageLog = dbhelp.GetLastByCommpanyId(companyId);

            return new Detail()
            {
                CompanyId = usageLog.CompanyId,
                CompanyName = usageLog.Company == null ? "" : usageLog.Company.Name,
                FactoryQty = usageLog.FactoryQty,
                EquipmentQty = usageLog.EquipmentQty,
                DeviceMessage = usageLog.DeviceMessage,
                AlarmMessage = usageLog.AlarmMessage,
                DocSizeInGB = usageLog.DocSizeInGB,
                DocDBPercentage = usageLog.DocDBPercentage,
                UpdatedAt = usageLog.UpdatedAt
            };
        }
    }
}