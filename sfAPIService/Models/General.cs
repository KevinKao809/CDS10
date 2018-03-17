using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;

namespace sfAPIService.Models
{
    public class General
    {
        public static bool IsFactoryUnderCompany(int factoryId, int companyId)
        {
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            var factory = dbhelp_factory.GetByid(factoryId, companyId);
            if (factory == null)
                return false;
            else
                return true;
        }

        public static bool IsEquipmentUnderCompany(string equipmentId, int companyId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            if (companyId == dbhelp.GetCompanyId(equipmentId))
                return true;
            else
                return false;
        }

        public static bool IsIoTDeviceUnderCompany(string iotDeviceId, int companyId)
        {
            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
            if (companyId == dbhelp.GetCompanyId(iotDeviceId))
                return true;
            else
                return false;
        }
    }
}