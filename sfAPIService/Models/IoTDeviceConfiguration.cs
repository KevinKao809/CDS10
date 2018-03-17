using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTDeviceConfigurationModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DataType { get; set; }
            public string Description { get; set; }
            public string DefaultValue { get; set; }
        }
        public class Edit
        {
            public string Name { get; set; }
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }

        public List<Detail> GetAllIoTDeviceConfiguration()
        {
            DBHelper._IoTDeviceConfiguration dbhelp = new DBHelper._IoTDeviceConfiguration();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                DataType = s.DataType,
                Description = s.Description,
                DefaultValue = s.DefaultValue
            }).ToList<Detail>();

        }

        public Detail getIoTDeviceConfigurationById(int id)
        {
            DBHelper._IoTDeviceConfiguration dbhelp = new DBHelper._IoTDeviceConfiguration();
            IoTDeviceConfiguration iotDC = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = iotDC.Id,
                Name = iotDC.Name,
                DataType = iotDC.DataType,
                Description = iotDC.Description,
                DefaultValue = iotDC.DefaultValue
            };
        }

        public void addIoTDeviceConfiguration(Edit iotDC)
        {
            DBHelper._IoTDeviceConfiguration dbhelp = new DBHelper._IoTDeviceConfiguration();
            var newIoTDeviceConfiguration = new IoTDeviceConfiguration()
            {
                Name = iotDC.Name,
                DataType = iotDC.DataType,
                Description = iotDC.Description,
                DefaultValue = iotDC.DefaultValue
            };
            dbhelp.Add(newIoTDeviceConfiguration);
        }

        public void updateIoTDeviceConfiguration(int id, Edit iotDC)
        {
            DBHelper._IoTDeviceConfiguration dbhelp = new DBHelper._IoTDeviceConfiguration();
            IoTDeviceConfiguration existingIoTDC = dbhelp.GetByid(id);
            existingIoTDC.Name = iotDC.Name;
            existingIoTDC.DataType = iotDC.DataType;
            existingIoTDC.Description = iotDC.Description;
            existingIoTDC.DefaultValue = iotDC.DefaultValue;

            dbhelp.Update(existingIoTDC);
        }

        public void deleteIoTDeviceConfiguration(int id)
        {
            DBHelper._IoTDeviceConfiguration dbhelp = new DBHelper._IoTDeviceConfiguration();
            IoTDeviceConfiguration existingIoTDC = dbhelp.GetByid(id);

            dbhelp.Delete(existingIoTDC);
        }
    }
}