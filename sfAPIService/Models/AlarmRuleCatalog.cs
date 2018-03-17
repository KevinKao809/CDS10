using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;
using Newtonsoft.Json.Linq;

namespace sfAPIService.Models
{
    public class AlarmRuleCatalogModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CompanyId { get; set; }
            public int MessageCatalogId { get; set; }
            public string MessageCatalogName { get; set; }
            public int KeepHappenInSec { get; set; }
            public bool ActiveFlag { get; set; }
        }

        public class Add
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int CompanyId { get; set; }
            [Required]
            public int MessageCatalogId { get; set; }
            [Required]
            public bool ActiveFlag { get; set; }
            [Required]
            public int KeepHappenInSec { get; set; }

        }

        public class Update
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int MessageCatalogId { get; set; }
            [Required]
            public bool ActiveFlag { get; set; }
            [Required]
            public int KeepHappenInSec { get; set; }
        }


        public List<Detail> GetAllAlarmRuleCatalogByCompanyId(int companyId)
        {
            DBHelper._AlarmRuleCatalog dbhelp = new DBHelper._AlarmRuleCatalog();
            List<AlarmRuleCatalog> alarmRuleCatalogs = dbhelp.GetAllByCompanyId(companyId);
            List<Detail> returnResult = new List<Detail>();

            foreach (var alarmRuleCatalog in alarmRuleCatalogs)
            {
                try
                {
                    returnResult.Add(new Detail()
                    {
                        Id = alarmRuleCatalog.Id,
                        Name = alarmRuleCatalog.Name,
                        Description = alarmRuleCatalog.Description,
                        CompanyId = alarmRuleCatalog.CompanyId,
                        MessageCatalogId = alarmRuleCatalog.MessageCatalogId,
                        MessageCatalogName = (alarmRuleCatalog.MessageCatalog == null) ? "" : alarmRuleCatalog.MessageCatalog.Name,
                        ActiveFlag = alarmRuleCatalog.ActiveFlag,
                        KeepHappenInSec = alarmRuleCatalog.KeepHappenInSec
                    });
                }
                catch {}
            }

            return returnResult;
        }

        public Detail getAlarmRuleCatalogById(int id)
        {
            DBHelper._AlarmRuleCatalog dbhelp = new DBHelper._AlarmRuleCatalog();
            AlarmRuleCatalog alarmRuleCatalog = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = alarmRuleCatalog.Id,
                Name = alarmRuleCatalog.Name,
                Description = alarmRuleCatalog.Description,
                CompanyId = alarmRuleCatalog.CompanyId,
                MessageCatalogId = alarmRuleCatalog.MessageCatalogId,
                MessageCatalogName = (alarmRuleCatalog.MessageCatalog == null) ? "" : alarmRuleCatalog.MessageCatalog.Name,
                ActiveFlag = alarmRuleCatalog.ActiveFlag,
                KeepHappenInSec = alarmRuleCatalog.KeepHappenInSec
            };
        }

        public void addAlarmRuleCatalog(Add alarmRuleCatalog)
        {
            DBHelper._AlarmRuleCatalog dbhelp = new DBHelper._AlarmRuleCatalog();
            var newAlarmRuleCatalog = new AlarmRuleCatalog()
            {
                Name = alarmRuleCatalog.Name,
                Description = alarmRuleCatalog.Description,
                CompanyId = alarmRuleCatalog.CompanyId,
                MessageCatalogId = alarmRuleCatalog.MessageCatalogId,
                ActiveFlag = alarmRuleCatalog.ActiveFlag,
                KeepHappenInSec = alarmRuleCatalog.KeepHappenInSec
            };
            dbhelp.Add(newAlarmRuleCatalog);
        }

        public void updateAlarmRuleCatalog(int id, Update alarmRuleCatalog)
        {
            DBHelper._AlarmRuleCatalog dbhelp = new DBHelper._AlarmRuleCatalog();
            AlarmRuleCatalog existingAlarmRuleCatalog = dbhelp.GetByid(id);
            existingAlarmRuleCatalog.Name = alarmRuleCatalog.Name;
            existingAlarmRuleCatalog.Description = alarmRuleCatalog.Description;
            existingAlarmRuleCatalog.MessageCatalogId = alarmRuleCatalog.MessageCatalogId;
            existingAlarmRuleCatalog.ActiveFlag = alarmRuleCatalog.ActiveFlag;
            existingAlarmRuleCatalog.KeepHappenInSec = alarmRuleCatalog.KeepHappenInSec;

            dbhelp.Update(existingAlarmRuleCatalog);
        }

        public void deleteAlarmRuleCatalog(int id)
        {
            DBHelper._AlarmRuleCatalog dbhelp = new DBHelper._AlarmRuleCatalog();
            AlarmRuleCatalog existingAlarmRuleCatalog = dbhelp.GetByid(id);

            dbhelp.Delete(existingAlarmRuleCatalog);
        }
    }
}