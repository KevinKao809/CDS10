using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class IoTDeviceCustomizedConfigurationModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string DataType { get; set; }
            public string Description { get; set; }
            public string DefaultValue { get; set; }
            public DateTime? UpdatedAt { get; set; }
        }
        public class Add
        {
            [Required]
            public int CompanyId { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }

        public class Update
        {
            [Required]
            public string Name { get; set; }
            [Required]
            [MaxLength(10)]
            public string DataType { get; set; }
            public string Description { get; set; }
            [MaxLength(50)]
            public string DefaultValue { get; set; }
        }

        public List<Detail> GetAllCustomizedConfigurationByCompanyId(int companyId)
        {
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp = new DBHelper._IoTDeviceCustomizedConfiguration();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                DataType = s.DataType,
                Description = s.Description,
                DefaultValue = s.DefaultValue,
                UpdatedAt = s.UpdatedAt
            }).ToList<Detail>();

        }

        public Detail getCustomizedConfigurationById(int id)
        {
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp = new DBHelper._IoTDeviceCustomizedConfiguration();
            IoTDeviceCustomizedConfiguration iotDCC = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = iotDCC.Id,
                Name = iotDCC.Name,
                DataType = iotDCC.DataType,
                Description = iotDCC.Description,
                DefaultValue = iotDCC.DefaultValue,
                UpdatedAt = iotDCC.UpdatedAt
            };
        }

        public void addIoTDeviceConfiguration(Add iotDCC)
        {
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp = new DBHelper._IoTDeviceCustomizedConfiguration();
            var newIoTCustomizedConfiguration = new IoTDeviceCustomizedConfiguration()
            {
                CompanyId = iotDCC.CompanyId,
                Name = iotDCC.Name,
                DataType = iotDCC.DataType,
                Description = iotDCC.Description,
                DefaultValue = iotDCC.DefaultValue
            };
            dbhelp.Add(newIoTCustomizedConfiguration);
        }

        public void updateCustomizedConfiguration(int id, Update iotDCC)
        {
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp = new DBHelper._IoTDeviceCustomizedConfiguration();
            IoTDeviceCustomizedConfiguration existingIoTDCC = dbhelp.GetByid(id);
            existingIoTDCC.Name = iotDCC.Name;
            existingIoTDCC.DataType = iotDCC.DataType;
            existingIoTDCC.Description = iotDCC.Description;
            existingIoTDCC.DefaultValue = iotDCC.DefaultValue;

            dbhelp.Update(existingIoTDCC);
        }

        public void deleteIoTDeviceConfiguration(int id)
        {
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp = new DBHelper._IoTDeviceCustomizedConfiguration();
            IoTDeviceCustomizedConfiguration existingIoTDCC = dbhelp.GetByid(id);

            dbhelp.Delete(existingIoTDCC);
        }
    }
}