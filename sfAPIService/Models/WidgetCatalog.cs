using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;

namespace sfAPIService.Models
{
    public class WidgetCatalogModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public int? MessageCatalogId { get; set; }
            public string Level { get; set; }
            public string MessageCatalogName { get; set; }
            public string Name { get; set; }
            public int WidgetClassKey { get; set; }
            public JObject Context { get; set; }
        }
        public class Add
        {
            [Required]
            public int CompanyId { get; set; }
            public int? MessageCatalogId { get; set; }
            [Required]
            public string Name { get; set; }
            [MaxLength(50)]
            [Required]
            public string Level { get; set; }
            [Required]
            public int WidgetClassKey { get; set; }
            [Required]
            public string Context { get; set; }
        }

        public class Update
        {
            public int? MessageCatalogId { get; set; }
            [MaxLength(50)]
            [Required]
            public string Level { get; set; }
            [MaxLength(50)]
            [Required]
            public string Name { get; set; }
            [Required]
            public int WidgetClassKey { get; set; }
            [Required]
            public string Context { get; set; }
        }
        public List<Detail> getAllWidgetCatalogByCompanyId(int companyId, string level = null)
        {
            DBHelper._WidgetCatalog dbhelp = new DBHelper._WidgetCatalog();
            if (string.IsNullOrEmpty(level))
            {
                return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    CompanyId = s.CompanyID,
                    MessageCatalogId = s.MessageCatalogID,
                    MessageCatalogName = (s.MessageCatalog == null ? "" : s.MessageCatalog.Name),
                    Level = s.Level,
                    WidgetClassKey = s.WidgetClassKey,
                    Context = JObject.Parse(s.Context)
                }).ToList<Detail>();
            }
            else
            {
                return dbhelp.GetAllByCompanyId(companyId, level).Select(s => new Detail()
                {
                    Id = s.Id,
                    Name = s.Name,
                    CompanyId = s.CompanyID,
                    MessageCatalogId = s.MessageCatalogID,
                    MessageCatalogName = (s.MessageCatalog == null ? "" : s.MessageCatalog.Name),
                    Level = s.Level,
                    WidgetClassKey = s.WidgetClassKey,
                    Context = JObject.Parse(s.Context)
                }).ToList<Detail>();
            }
            
        }        

        public Detail getWidgetCatalogById(int id)
        {
            DBHelper._WidgetCatalog dbhelp = new DBHelper._WidgetCatalog();
            WidgetCatalog widgetCatalog = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = widgetCatalog.Id,
                Name = widgetCatalog.Name,
                CompanyId = widgetCatalog.CompanyID,
                MessageCatalogId = widgetCatalog.MessageCatalogID,
                MessageCatalogName = (widgetCatalog.MessageCatalog == null ? "" : widgetCatalog.MessageCatalog.Name),
                Level = widgetCatalog.Level,
                WidgetClassKey = widgetCatalog.WidgetClassKey,
                Context = JObject.Parse(widgetCatalog.Context)
            };
        }

        public int addWidgetCatalog(Add widgetCatalog)
        {
            DBHelper._WidgetCatalog dbhelp = new DBHelper._WidgetCatalog();
            var newWidgetCatalog = new WidgetCatalog()
            {
                Name = widgetCatalog.Name,
                CompanyID = widgetCatalog.CompanyId,
                MessageCatalogID = widgetCatalog.MessageCatalogId,
                Level = widgetCatalog.Level,
                WidgetClassKey = widgetCatalog.WidgetClassKey,
                Context = widgetCatalog.Context
            };

            return dbhelp.Add(newWidgetCatalog);
        }

        public void updateWidgetCatalog(int id, Update widgetCatalog)
        {
            DBHelper._WidgetCatalog dbhelp = new DBHelper._WidgetCatalog();
            WidgetCatalog existingWidgetCatalog = dbhelp.GetByid(id);
            existingWidgetCatalog.Name = widgetCatalog.Name;
            existingWidgetCatalog.MessageCatalogID = widgetCatalog.MessageCatalogId;
            existingWidgetCatalog.Level = widgetCatalog.Level;
            existingWidgetCatalog.WidgetClassKey = widgetCatalog.WidgetClassKey;
            existingWidgetCatalog.Context = widgetCatalog.Context;

            dbhelp.Update(existingWidgetCatalog);
        }

        public void deleteWidgetCatalog(int id)
        {
            DBHelper._WidgetCatalog dbhelp = new DBHelper._WidgetCatalog();
            WidgetCatalog existingWidgetCatalog = dbhelp.GetByid(id);

            dbhelp.Delete(existingWidgetCatalog);
        }
        
    }
}