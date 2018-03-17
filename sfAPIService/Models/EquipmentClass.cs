using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class EquipmentClassModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool DeletedFlag { get; set; }
        }
        public class Add
        {
            [Required]
            public int CompanyId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class Update
        {
            [Required]
            public int CompanyId { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> GetAllEquipmentClass()
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                CompanyName = s.Company.Name,
                Name = s.Name,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public List<Detail> GetAllEquipmentClassBySuperAdmin()
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                CompanyName = s.Company.Name,
                Name = s.Name,
                Description = s.Description,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public Detail getEquipmentClassById(int id)
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();
            EquipmentClass equipmentClass = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = equipmentClass.Id,
                CompanyId = equipmentClass.CompanyId,
                CompanyName = equipmentClass.Company.Name,
                Name = equipmentClass.Name,
                Description = equipmentClass.Description,
                DeletedFlag = equipmentClass.DeletedFlag
            };
        }

        public void addEquipmentClass(Add equipmentClass)
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();
            var newEquipmentClass = new EquipmentClass()
            {
                CompanyId = equipmentClass.CompanyId,
                Name = equipmentClass.Name,
                Description = equipmentClass.Description,
            };
            dbhelp.Add(newEquipmentClass);
        }

        public void updateEquipmentClass(int id, Update equipmentClass)
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();
            EquipmentClass existingEquipmentClass = dbhelp.GetByid(id);
            existingEquipmentClass.CompanyId = equipmentClass.CompanyId;
            existingEquipmentClass.Name = equipmentClass.Name;
            existingEquipmentClass.Description = equipmentClass.Description;
            if(equipmentClass.DeletedFlag.HasValue)
                existingEquipmentClass.DeletedFlag = (bool) equipmentClass.DeletedFlag;

            dbhelp.Update(existingEquipmentClass);
        }

        public void deleteEquipmentClass(int id)
        {
            DBHelper._EquipmentClass dbhelp = new DBHelper._EquipmentClass();
            EquipmentClass existingEquipmentClass = dbhelp.GetByid(id);

            dbhelp.Delete(existingEquipmentClass);
        }
    }
}