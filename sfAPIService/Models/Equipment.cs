using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class EquipmentModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public int EquipmentClassId { get; set; }
            public string EquipmentClassName { get; set; }
            public int FactoryId { get; set; }
            public string FactoryName { get; set; }
            public string IoTHubDeviceId { get; set; }
            public string IoTDeviceTypeName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
            public string PhotoUrl { get; set; }
        }
        public class Detail_readonly
        {
            public string EquipmentId { get; set; }
            public string Name { get; set; }
            public string EquipmentClassName { get; set; }
            public string FactoryName { get; set; }
            public string IoTHubDeviceId { get; set; }
            public string IoTDeviceTypeName { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public string PhotoUrl { get; set; }
        }

        public class Detail_WithMetaData : Detail
        {
            public List<MetaDataValue_View> MetaDatas { get; set; }
        }

        public class Edit
        {
            [Required]
            public string EquipmentId { get; set; }
            [Required]
            public string Name { get; set; }
            [Required]
            public int EquipmentClassId { get; set; }
            [Required]
            public int FactoryId { get; set; }
            [Required]
            public string IoTHubDeviceId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int MaxIdleInSec { get; set; }
        }

        public List<Detail> GetAllEquipment()
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            return dbhelp_equipment.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail_WithMetaData> GetAllEquipmentWithMetaData()
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment(); 
            
            return dbhelp_equipment.GetAll().Select(s => new Detail_WithMetaData()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL,
                MetaDatas = GetMetaDataById(s.Id)
            }).ToList<Detail_WithMetaData>();
        }

        public List<Detail> GetAllEquipmentByCompanyId(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<Equipment> equipmentList = new List<Equipment>();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();

            foreach (int factoryId in factoryIdList)
            {
                equipmentList.AddRange(dbhelp_equipment.GetAllByFactoryId(factoryId));
            }
            return equipmentList.Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail_WithMetaData> GetAllEquipmentWithMetaDataByCompanyId(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<Equipment> equipmentList = new List<Equipment>();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();

            foreach (int factoryId in factoryIdList)
            {
                equipmentList.AddRange(dbhelp_equipment.GetAllByFactoryId(factoryId));
            }
            return equipmentList.Select(s => new Detail_WithMetaData()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL,
                MetaDatas = GetMetaDataById(s.Id)
            }).ToList<Detail_WithMetaData>();
        }

        public List<Detail_readonly> GetAllEquipmentByCompanyIdReadonly(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<Equipment> equipmentList = new List<Equipment>();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();

            foreach (int factoryId in factoryIdList)
            {
                equipmentList.AddRange(dbhelp_equipment.GetAllByFactoryId(factoryId));
            }
            return equipmentList.Select(s => new Detail_readonly()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL
            }).ToList<Detail_readonly>();
        }

        public List<Detail_WithMetaData> GetAllEquipmentWithMetaDataByCompanyIdReadonly(int companyId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();
            DBHelper._Factory dbhelp_factory = new DBHelper._Factory();
            List<Equipment> equipmentList = new List<Equipment>();
            List<int> factoryIdList = dbhelp_factory.GetAllByCompanyId(companyId).Select(s => s.Id).ToList<int>();

            foreach (int factoryId in factoryIdList)
            {
                equipmentList.AddRange(dbhelp_equipment.GetAllByFactoryId(factoryId));
            }
            return equipmentList.Select(s => new Detail_WithMetaData()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL,
                MetaDatas = GetMetaDataById(s.Id)
            }).ToList<Detail_WithMetaData>();
        }

        public List<Detail> GetAllEquipmentByFactoryId(int factoryId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();

            return dbhelp.GetAllByFactoryId(factoryId).Select(s => new Detail()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL
            }).ToList<Detail>();
        }

        public List<Detail_WithMetaData> GetAllEquipmentWithMetaDataByFactoryId(int factoryId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();

            return dbhelp.GetAllByFactoryId(factoryId).Select(s => new Detail_WithMetaData()
            {
                Id = s.Id,
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassId = s.EquipmentClassId,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryId = s.FactoryId,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                MaxIdleInSec = s.MaxIdleInSec,
                PhotoUrl = s.PhotoURL,
                MetaDatas = GetMetaDataById(s.Id)
            }).ToList<Detail_WithMetaData>();
        }

        public List<Detail_readonly> GetAllEquipmentByFactoryIdReadonly(int factoryId)
        {     
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();

            return dbhelp_equipment.GetAllByFactoryId(factoryId).Select(s => new Detail_readonly()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL
            }).ToList<Detail_readonly>();
        }

        public List<Detail_WithMetaData> GetAllEquipmentWithMetaDataByFactoryIdReadonly(int factoryId)
        {
            DBHelper._Equipment dbhelp_equipment = new DBHelper._Equipment();

            return dbhelp_equipment.GetAllByFactoryId(factoryId).Select(s => new Detail_WithMetaData()
            {
                EquipmentId = s.EquipmentId,
                Name = s.Name,
                EquipmentClassName = s.EquipmentClass.Name,
                FactoryName = s.Factory.Name,
                IoTHubDeviceId = s.IoTHubDeviceID,
                IoTDeviceTypeName = s.IoTDevice.DeviceType.Name,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoUrl = s.PhotoURL,
                MetaDatas = GetMetaDataById(s.Id)
            }).ToList<Detail_WithMetaData>();
        }

        public Detail getEquipmentById(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = equipment.Id,
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                EquipmentClassId = equipment.EquipmentClassId,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryId = equipment.FactoryId,
                FactoryName = equipment.Factory.Name,
                IoTHubDeviceId = equipment.IoTHubDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                MaxIdleInSec = equipment.MaxIdleInSec,
                PhotoUrl = equipment.PhotoURL
            };
        }

        public Detail_WithMetaData getEquipmentWithMetaDataById(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(id);

            return new Detail_WithMetaData()
            {
                Id = equipment.Id,
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                EquipmentClassId = equipment.EquipmentClassId,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryId = equipment.FactoryId,
                FactoryName = equipment.Factory.Name,
                IoTHubDeviceId = equipment.IoTHubDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                MaxIdleInSec = equipment.MaxIdleInSec,
                PhotoUrl = equipment.PhotoURL,
                MetaDatas = GetMetaDataById(equipment.Id)
            };
        }

        public Detail_WithMetaData getEquipmentWithMetaDataById(string id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(id);

            return new Detail_WithMetaData()
            {
                Id = equipment.Id,
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                EquipmentClassId = equipment.EquipmentClassId,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryId = equipment.FactoryId,
                FactoryName = equipment.Factory.Name,
                IoTHubDeviceId = equipment.IoTHubDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                MaxIdleInSec = equipment.MaxIdleInSec,
                PhotoUrl = equipment.PhotoURL,
                MetaDatas = GetMetaDataById(equipment.Id)
            };
        }

        public Detail_readonly getEquipmentByIdReadonly(string equipmentId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment equipment = dbhelp.GetByid(equipmentId);

            return new Detail_readonly()
            {
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                EquipmentClassName = equipment.EquipmentClass.Name,
                FactoryName = equipment.Factory.Name,
                IoTHubDeviceId = equipment.IoTHubDeviceID,
                IoTDeviceTypeName = equipment.IoTDevice.DeviceType.Name,
                Latitude = (equipment.Latitude == null) ? "" : equipment.Latitude.ToString(),
                Longitude = (equipment.Longitude == null) ? "" : equipment.Longitude.ToString(),
                PhotoUrl = equipment.PhotoURL
            };
        }

        public int addEquipment(Edit equipment)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            var newEquipment = new Equipment()
            {
                EquipmentId = equipment.EquipmentId,
                Name = equipment.Name,
                EquipmentClassId = equipment.EquipmentClassId,
                FactoryId = equipment.FactoryId,
                IoTHubDeviceID = equipment.IoTHubDeviceId,
                Latitude = equipment.Latitude,
                Longitude = equipment.Longitude,
                MaxIdleInSec = equipment.MaxIdleInSec
            };
            int newEquipmentId = dbhelp.Add(newEquipment);
            return newEquipmentId;
        }

        public void updateEquipment(int id, Edit equipment)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existingEquipment = dbhelp.GetByid(id);
            existingEquipment.EquipmentId = equipment.EquipmentId;
            existingEquipment.Name = equipment.Name;
            existingEquipment.EquipmentClassId = equipment.EquipmentClassId;
            existingEquipment.FactoryId = equipment.FactoryId;
            existingEquipment.IoTHubDeviceID = equipment.IoTHubDeviceId;
            existingEquipment.Latitude = equipment.Latitude;
            existingEquipment.Longitude = equipment.Longitude;
            existingEquipment.MaxIdleInSec = equipment.MaxIdleInSec;

            dbhelp.Update(existingEquipment);
        }

        public void deleteEquipment(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existingEquipment = dbhelp.GetByid(id);

            dbhelp.Delete(existingEquipment);
        }

        public int getCompanyId(int id)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existEquipment = dbhelp.GetByid(id);
            int companyId = existEquipment.Factory.CompanyId;
            return companyId;
        }

        public int getCompanyId(string equipmentId)
        {
            SFDatabaseEntities dbEntity = new SFDatabaseEntities();
            int companyId = (from c in dbEntity.Equipment
                             where c.EquipmentId == equipmentId
                             select c.Factory.CompanyId).Single<int>();
            return companyId;
        }

        public void updateEquipmentLogoURL(int id, string url)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            Equipment existEquipment = dbhelp.GetByid(id);
            existEquipment.PhotoURL = url;
            dbhelp.Update(existEquipment);
        }

        public class MetaDataValue_View
        {
            public int Id { get; set; }
            public string ObjectName { get; set; }
            public string ObjectValue { get; set; }
            public int MetaDataDefinationId { get; set; }
        }

        public List<MetaDataValue_View> GetMetaDataById(int equipmentId)
        {
            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            int companyId = getCompanyId(equipmentId);
            return dbhelp.GetMetaDataById(equipmentId, companyId).Select(s => new MetaDataValue_View()
            {
                Id = s.Id,
                ObjectName = s.MetaDataDefination.ObjectName,
                ObjectValue = s.ObjectValue,
                MetaDataDefinationId = s.MetaDataDefinationId
            }).ToList<MetaDataValue_View>();
        }

        public class MetaData_Edit
        {
            public MetaDataValue_View[] metaDatas { get; set; }
        }

        public void UpdateMetaDataById(int equipmentId, MetaData_Edit metaDataEdit)
        {
            List<MetaDataValue> metaDataValueList = new List<MetaDataValue>();
            foreach (var inMetaData in metaDataEdit.metaDatas)
            {
                MetaDataValue metaData = new MetaDataValue();
                metaData.Id = inMetaData.Id;
                metaData.MetaDataDefinationId = inMetaData.MetaDataDefinationId;
                metaData.ObjectValue = inMetaData.ObjectValue;
                metaData.ReferenceId = equipmentId;

                metaDataValueList.Add(metaData);
            }

            DBHelper._Equipment dbhelp = new DBHelper._Equipment();
            dbhelp.UpdateMetaDataById(equipmentId, metaDataValueList);
        }
    }
}