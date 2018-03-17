using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sfAPIService.Models
{
    public class MetaDataDefinationModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string EntityType { get; set; }
            public string ObjectName { get; set; }
        }

        public class Edit
        {
            public int CompanyId { get; set; }
            public string EntityType { get; set; }
            public string ObjectName { get; set; }
        }

        public List<Detail> GetEquipmentMetaDataDefination(int companyId)
        {
            DBHelper._MetaDataDefination dbhelp_metadataDef = new DBHelper._MetaDataDefination();
            return dbhelp_metadataDef.GetAllByCompanyIdAndEntityType(companyId, "equipment").Select(s => new Detail()
            {
                Id = s.Id,
                EntityType = s.EntityType,
                ObjectName = s.ObjectName
            }).ToList<Detail>();
        }

        public List<Detail> GetFactoryMetaDataDefination(int companyId)
        {
            DBHelper._MetaDataDefination dbhelp_metadataDef = new DBHelper._MetaDataDefination();
            return dbhelp_metadataDef.GetAllByCompanyIdAndEntityType(companyId, "factory").Select(s => new Detail()
            {
                Id = s.Id,
                EntityType = s.EntityType,
                ObjectName = s.ObjectName
            }).ToList<Detail>();
        }

        public int addMetaDataDefination(Edit metaDataDef)
        {
            DBHelper._MetaDataDefination dbhelp = new DBHelper._MetaDataDefination();
            var newMetaData = new MetaDataDefination()
            {
                CompanyId = metaDataDef.CompanyId,
                ObjectName = metaDataDef.ObjectName,
                EntityType = metaDataDef.EntityType
            };
            int newMetaDataDefId = dbhelp.Add(newMetaData);
            return newMetaDataDefId;
        }

        public void updateMetaDataDefination(int id, Edit metaDataDef)
        {
            DBHelper._MetaDataDefination dbhelp = new DBHelper._MetaDataDefination();
            MetaDataDefination existingMetaDataDef = dbhelp.GetByid(id);
            existingMetaDataDef.EntityType = metaDataDef.EntityType;
            existingMetaDataDef.ObjectName = metaDataDef.ObjectName;

            dbhelp.Update(existingMetaDataDef);
        }

        public void deleteMetaDataDefination(int id)
        {
            DBHelper._MetaDataDefination dbhelp = new DBHelper._MetaDataDefination();
            MetaDataDefination existingMetaDataDef = dbhelp.GetByid(id);

            dbhelp.Delete(existingMetaDataDef);
        }
    }
}