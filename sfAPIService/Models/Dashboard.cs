using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class DashboardModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public string DashboardType { get; set; }
            public int? FactoryId { get; set; }
            public string FactoryName { get; set; }
            public int? EquipmentClassId { get; set; }
            public string EquipmentClassName { get; set; }
            public int? EquipmentId { get; set; }
            public string EquipmentName { get; set; }
            public string Status { get; set; }
        }
        public class Detail_EquipmentClass
        {
            public int DashboardId { get; set; }
            public int? EquipmentClassId { get; set; }
            public string EquipmentClassName { get; set; }
            public string Description { get; set; }
            public bool IsReady { get; set; }
        }
        public class Add
        {
            [Required]
            public int CompanyId { get; set; }
            public int? FactoryId { get; set; }
            public int? EquipmentClassId { get; set; }
            public int? EquipmentId { get; set; }
            public string DashboardType { get; set; }
        }
        
        public List<Detail> getAllDashboardByCompnayId(int companyId, string type = null)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

            if (string.IsNullOrEmpty(type))
            {
                return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
                {
                    Id = s.Id,
                    CompanyId = s.CompanyID,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    EquipmentId = s.EquipmentID,
                    EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
                    EquipmentClassId = s.EquipmentClassID,
                    EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
                    DashboardType = s.DashboardType
                }).ToList<Detail>();
            }
            else
            {
                return dbhelp.GetAllByCompanyId(companyId, type).Select(s => new Detail()
                {
                    Id = s.Id,
                    CompanyId = s.CompanyID,
                    FactoryId = s.FactoryID,
                    FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                    EquipmentId = s.EquipmentID,
                    EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
                    EquipmentClassId = s.EquipmentClassID,
                    EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
                    DashboardType = s.DashboardType
                }).ToList<Detail>();
            }            
        }

        public List<Detail> getAllDashboardByFactoryId(int factoryId)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

            return dbhelp.GetAllByFactoryId(factoryId).Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyID,
                FactoryId = s.FactoryID,
                FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                EquipmentId = s.EquipmentID,
                EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
                EquipmentClassId = s.EquipmentClassID,
                EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
                DashboardType = s.DashboardType
            }).ToList<Detail>();
        }

        public List<Detail_EquipmentClass> getAllEquipmentClassDashboardByCompnayId(int companyId)
        {
            List<Detail_EquipmentClass> resultList = new List<Detail_EquipmentClass>();
            SFDatabaseEntities dbEntity = new SFDatabaseEntities();
            var allFactoryIds = from c in dbEntity.Factory.AsNoTracking()
                               where c.CompanyId == companyId && c.DeletedFlag == false
                               select c.Id;
            
            var allEquipmentClassesGroup = from c in dbEntity.Equipment.AsNoTracking()
                                      where allFactoryIds.Contains(c.FactoryId) && c.DeletedFlag == false
                                      join ec in dbEntity.EquipmentClass on c.EquipmentClassId equals ec.Id
                                      select ec;

            var allEquipmentClasses = allEquipmentClassesGroup.GroupBy(equipmentClass => equipmentClass.Id)
                                        .Select(s => s.FirstOrDefault())
                                        .Select(s => new Detail_EquipmentClass() {
                                            EquipmentClassId = s.Id,
                                            EquipmentClassName = s.Name,
                                            Description = s.Description,
                                            IsReady = false
                                        });

            var allExistEquipmentClassDashboards = from c in dbEntity.Dashboard.AsNoTracking()
                                                       where c.CompanyID == companyId && c.DashboardType == "EquipmentClass"
                                                       select c;
            var existingEquipmentClassId = from c in allExistEquipmentClassDashboards
                                           select c.EquipmentClassID;

            foreach (var equipmentClass in allEquipmentClasses)
            {
                if (existingEquipmentClassId.Contains(equipmentClass.EquipmentClassId))
                {
                    equipmentClass.IsReady = true;
                    var tmp = from c in allExistEquipmentClassDashboards
                              where c.EquipmentClassID == equipmentClass.EquipmentClassId
                              select c.Id;
                    equipmentClass.DashboardId = tmp.FirstOrDefault();
                }
                resultList.Add(equipmentClass);
            }

            return resultList;
        }

        public List<Detail> getAllElementClassDashboardByCompnayId(int companyId)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyID,
                FactoryId = s.FactoryID,
                FactoryName = (s.Factory == null ? "" : s.Factory.Name),
                EquipmentId = s.EquipmentID,
                EquipmentName = (s.Equipment == null ? "" : s.Equipment.Name),
                EquipmentClassId = s.EquipmentClassID,
                EquipmentClassName = (s.EquipmentClass == null ? "" : s.EquipmentClass.Name),
                DashboardType = s.DashboardType
            }).ToList<Detail>();
        }

        public Detail getDashboardById(int id)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();
            Dashboard dashboard = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = dashboard.Id,
                CompanyId = dashboard.CompanyID,
                FactoryId = dashboard.FactoryID,
                FactoryName = (dashboard.Factory == null ? "" : dashboard.Factory.Name),
                EquipmentId = dashboard.EquipmentID,
                EquipmentName = (dashboard.Equipment == null ? "" : dashboard.Equipment.Name),
                EquipmentClassId = dashboard.EquipmentClassID,
                EquipmentClassName = (dashboard.EquipmentClass == null ? "" : dashboard.EquipmentClass.Name),
                DashboardType = dashboard.DashboardType
            };
        }

        public int addDashboard(Add dashboard)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();
            var newDashboard = new Dashboard()
            {
                CompanyID = dashboard.CompanyId,
                DashboardType = dashboard.DashboardType,
                FactoryID = dashboard.FactoryId,
                EquipmentClassID = dashboard.EquipmentClassId,
                EquipmentID = dashboard.EquipmentId
            };
            return dbhelp.Add(newDashboard);
        }
        public void deleteDashboard(int id)
        {
            DBHelper._Dashboard dbhelp = new DBHelper._Dashboard();
            Dashboard existingDashboard = dbhelp.GetByid(id);

            dbhelp.Delete(existingDashboard);
        }
    }
}