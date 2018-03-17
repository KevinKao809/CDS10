using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class DeviceTypeModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Add
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class Update
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> GetAllDeviceType()
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public List<Detail> GetAllDeviceTypeBySuperAdmin()
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public Detail getDeviceTypeById(int id)
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();
            DeviceType deviceType = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = deviceType.Id,
                Name = deviceType.Name,
                Description = deviceType.Description,
                DeletedFlag = deviceType.DeletedFlag
            };
        }

        public void addDeviceType(Add deviceType)
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();
            var newDeviceType = new DeviceType()
            {
                Name = deviceType.Name,
                Description = deviceType.Description,
            };
            dbhelp.Add(newDeviceType);
        }

        public void updateDeviceType(int id, Update deviceType)
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();
            DeviceType existingDeviceType = dbhelp.GetByid(id);
            existingDeviceType.Name = deviceType.Name;
            existingDeviceType.Description = deviceType.Description;
            if(deviceType.DeletedFlag.HasValue)
                existingDeviceType.DeletedFlag = (bool) deviceType.DeletedFlag;

            dbhelp.Update(existingDeviceType);
        }

        public void deleteDeviceType(int id)
        {
            DBHelper._DeviceType dbhelp = new DBHelper._DeviceType();
            DeviceType existingDeviceType = dbhelp.GetByid(id);

            dbhelp.Delete(existingDeviceType);
        }
    }
}