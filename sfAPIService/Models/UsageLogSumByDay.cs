using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace sfAPIService.Models
{
    public class UsageLogSumByDayModels
    {
        public class Detail
        {
            public int? CompanyQty { get; set; }
            public int? FactoryQty { get; set; }
            public int? EquipmentQty { get; set; }
            public long? DeviceMessage { get; set; }
            public DateTime? UpdatedDateTime { get; set; }
        }

        public List<Detail> getAll(int days, string order)
        {
            DBHelper._UsageLogSumByDay dbhelp = new DBHelper._UsageLogSumByDay();

            return dbhelp.GetAll(days, order).Select(s => new Detail()
            {
                CompanyQty = s.CompanyQty,
                FactoryQty = s.FactoryQty,
                EquipmentQty = s.EquipmentQty,
                DeviceMessage = s.DeviceMessage,
                UpdatedDateTime = s.UpdatedDateTime
            }).ToList<Detail>();
        }
        public Detail getLast()
        {
            DBHelper._UsageLogSumByDay dbhelp = new DBHelper._UsageLogSumByDay();
            UsageLogSumByDay usageLog = dbhelp.GetLast();

            return new Detail()
            {
                CompanyQty = usageLog.CompanyQty,
                FactoryQty = usageLog.FactoryQty,
                EquipmentQty = usageLog.EquipmentQty,
                DeviceMessage = usageLog.DeviceMessage,
                UpdatedDateTime = usageLog.UpdatedDateTime
            };
        }
    }
}