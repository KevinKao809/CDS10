using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTDeviceMessageCatalogModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string IoTHubDeviceId { get; set; }
            public int MessageCatalogId { get; set; }
            public string MessageCatalogName { get; set; }
        }
        public class Edit
        {
            public int[] MessageCatalogIdList { get; set; }
        }

        public List<Detail> GetAllMessageCatalogByIoTDeviceId(string deviceId)
        {
            DBHelper._IoTDeviceMessageCatalog dbhelp = new DBHelper._IoTDeviceMessageCatalog();

            return dbhelp.GetAllByIoTDeviceId(deviceId).Select(s => new Detail()
            {
                Id = s.Id,
                IoTHubDeviceId = s.IoTHubDeviceID,
                MessageCatalogId = s.MessageCatalogID,
                MessageCatalogName = (s.MessageCatalog == null) ? "" : s.MessageCatalog.Name
            }).ToList<Detail>();

        }

        public void AttachMessage(string deviceId, Edit iotDMC)
        {
            DBHelper._IoTDeviceMessageCatalog dbhelp = new DBHelper._IoTDeviceMessageCatalog();
            List<IoTDeviceMessageCatalog> newIoTDMCList = new List<IoTDeviceMessageCatalog>();
            List<IoTDeviceMessageCatalog> existIoTDMCList = dbhelp.GetAllByIoTDeviceId(deviceId);

            dbhelp.Delete(existIoTDMCList);
            if (iotDMC != null)
            {
                foreach (int messageCatalogId in iotDMC.MessageCatalogIdList)
                {
                    newIoTDMCList.Add(new IoTDeviceMessageCatalog()
                    {
                        IoTHubDeviceID = deviceId,
                        MessageCatalogID = messageCatalogId
                    });
                }
            }
            
            dbhelp.Add(newIoTDMCList);
        }

        public void addIoTDeviceMessageCatalog(string deviceId, Edit IoTDMC)
        {
            DBHelper._IoTDeviceMessageCatalog dbhelp = new DBHelper._IoTDeviceMessageCatalog();
            List<IoTDeviceMessageCatalog> newIoTDMCList = new List<IoTDeviceMessageCatalog>();
            foreach (int messageCatalogId in IoTDMC.MessageCatalogIdList)
            {
                newIoTDMCList.Add(new IoTDeviceMessageCatalog()
                {
                    IoTHubDeviceID = deviceId,
                    MessageCatalogID = messageCatalogId
                });
            }

            dbhelp.Add(newIoTDMCList);
        }
        
    }
}