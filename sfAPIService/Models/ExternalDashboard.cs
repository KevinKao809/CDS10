using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class ExternalDashboardModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int? Order { get; set; }
            public string Name { get; set; }
            public string URL { get; set; }
        }
        public class Edit
        {
            [Required]
            public int CompanyId { get; set; }
            public int Order { get; set; }
            public string Name { get; set; }
            public string URL { get; set; }
        }
        public List<Detail> GetAllExternalDashboardByCompanyId(int companyId)
        {
            DBHelper._ExternalDashboard dbhelp = new DBHelper._ExternalDashboard();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Order = s.Order,
                URL = s.URL
            }).ToList<Detail>();
        }
        
        public Detail GetAllExternalDashboardById(int id)
        {
            DBHelper._ExternalDashboard dbhelp = new DBHelper._ExternalDashboard();
            ExternalDashboard externalDashboard = dbhelp.GetByid(id);

            if (externalDashboard != null)
            {
                return new Detail()
                {
                    Id = externalDashboard.Id,
                    Name = externalDashboard.Name,
                    Order = externalDashboard.Order,
                    URL = externalDashboard.URL
                };
            }
            else
            {
                return null;
            }
            
        }

        public void addExternalDashboard(Edit externalDashboard)
        {
            DBHelper._ExternalDashboard dbhelp = new DBHelper._ExternalDashboard();
            var newExternalDashboard = new ExternalDashboard()
            {
                CompanyId = externalDashboard.CompanyId,
                Name = externalDashboard.Name,
                Order = externalDashboard.Order,
                URL = externalDashboard.URL
            };
            dbhelp.Add(newExternalDashboard);
        }

        public void updateExternalDashboard(int id, Edit externalDashboard)
        {
            DBHelper._ExternalDashboard dbhelp = new DBHelper._ExternalDashboard();
            ExternalDashboard existingExternalDashboard = dbhelp.GetByid(id);
            existingExternalDashboard.Name = externalDashboard.Name;
            existingExternalDashboard.Order = externalDashboard.Order;
            existingExternalDashboard.URL = externalDashboard.URL;

            dbhelp.Update(existingExternalDashboard);
        }

        public void deleteExternalDashboard(int id)
        {
            DBHelper._ExternalDashboard dbhelp = new DBHelper._ExternalDashboard();
            ExternalDashboard existingExternalDashboard = dbhelp.GetByid(id);

            dbhelp.Delete(existingExternalDashboard);
        }
    }
}