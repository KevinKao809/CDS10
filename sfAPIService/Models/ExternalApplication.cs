using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class ExternalApplicationModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CompanyId { get; set; }
            public JObject MessageTemplate { get; set; }
            public string Method { get; set; }
            public string ServiceURL { get; set; }
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
            public string TargetType { get; set; }
        }

        public class Add
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string MessageTemplate { get; set; }
            [Required]
            public string Method { get; set; }
            [Required]
            public string ServiceURL { get; set; }
            [Required]
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
            [Required]
            public string TargetType { get; set; }
        }

        public class Update
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            public string MessageTemplate { get; set; }
            [Required]
            public string Method { get; set; }
            [Required]
            public string ServiceURL { get; set; }
            [Required]
            public string AuthType { get; set; }
            public string AuthID { get; set; }
            public string AuthPW { get; set; }
            public string TokenURL { get; set; }
            public string HeaderValues { get; set; }
            [Required]
            public string TargetType { get; set; }
        }


        public List<Detail> GetAllExternalApplicationByCompanyId(int companyId)
        {
            DBHelper._ExternalApplication dbhelp = new DBHelper._ExternalApplication();
            List<ExternalApplication> externalApps = dbhelp.GetAllByCompanyId(companyId);
            List<Detail> returnList = new List<Detail>();

            foreach (var externalApp in externalApps)
            {
                try
                {
                    returnList.Add(new Detail()
                    {
                        Id = externalApp.Id,
                        Name = externalApp.Name,
                        Description = externalApp.Description,
                        CompanyId = externalApp.CompanyId,
                        MessageTemplate = (externalApp.MessageTemplate == null) ? null : JObject.Parse(externalApp.MessageTemplate),
                        Method = externalApp.Method,
                        ServiceURL = externalApp.ServiceURL,
                        AuthType = externalApp.AuthType,
                        AuthID = externalApp.AuthID,
                        AuthPW = externalApp.AuthPW,
                        TokenURL = externalApp.TokenURL,
                        HeaderValues = externalApp.HeaderValues,
                        TargetType = externalApp.TargetType
                    });
                }
                catch { }
            }

            return returnList;
        }

        public Detail getExternalApplicationById(int id)
        {
            DBHelper._ExternalApplication dbhelp = new DBHelper._ExternalApplication();
            ExternalApplication externalApplication = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = externalApplication.Id,
                Name = externalApplication.Name,
                Description = externalApplication.Description,
                CompanyId = externalApplication.CompanyId,
                MessageTemplate = (externalApplication.MessageTemplate == null) ? null : JObject.Parse(externalApplication.MessageTemplate),
                Method = externalApplication.Method,
                ServiceURL = externalApplication.ServiceURL,
                AuthType = externalApplication.AuthType,
                AuthID = externalApplication.AuthID,
                AuthPW = externalApplication.AuthPW,
                TokenURL = externalApplication.TokenURL,
                HeaderValues = externalApplication.HeaderValues,
                TargetType = externalApplication.TargetType
            };
        }

        public void addExternalApplication(Add externalApplication)
        {            
            try
            {
                if (externalApplication.MessageTemplate != null)
                {
                    JObject tmp = JObject.Parse(externalApplication.MessageTemplate);
                }
            }
            catch
            {
                throw new Exception("MessageTemplate must be in Json fromat");
            }

            DBHelper._ExternalApplication dbhelp = new DBHelper._ExternalApplication();
            var newExternalApplication = new ExternalApplication()
            {
                Name = externalApplication.Name,
                Description = externalApplication.Description,
                CompanyId = externalApplication.CompanyId,
                MessageTemplate = externalApplication.MessageTemplate,
                Method = externalApplication.Method,
                ServiceURL = externalApplication.ServiceURL,
                AuthType = externalApplication.AuthType,
                AuthID = externalApplication.AuthID,
                AuthPW = externalApplication.AuthPW,
                TokenURL = externalApplication.TokenURL,
                HeaderValues = externalApplication.HeaderValues,
                TargetType = externalApplication.TargetType
            };
            dbhelp.Add(newExternalApplication);
        }

        public void updateExternalApplication(int id, Update externalApplication)
        {
            try
            {
                if (externalApplication.MessageTemplate != null)
                {
                    JObject tmp = JObject.Parse(externalApplication.MessageTemplate);
                }
            }
            catch
            {
                throw new Exception("MessageTemplate must be in Json fromat");
            }

            DBHelper._ExternalApplication dbhelp = new DBHelper._ExternalApplication();
            ExternalApplication existingExternalApplication = dbhelp.GetByid(id);
            existingExternalApplication.Name = externalApplication.Name;
            existingExternalApplication.Description = externalApplication.Description;
            existingExternalApplication.MessageTemplate = externalApplication.MessageTemplate;
            existingExternalApplication.Method = externalApplication.Method;
            existingExternalApplication.ServiceURL = externalApplication.ServiceURL;
            existingExternalApplication.AuthType = externalApplication.AuthType;
            existingExternalApplication.AuthID = externalApplication.AuthID;
            existingExternalApplication.AuthPW = externalApplication.AuthPW;
            existingExternalApplication.TokenURL = externalApplication.TokenURL;
            existingExternalApplication.HeaderValues = externalApplication.HeaderValues;
            existingExternalApplication.TargetType = externalApplication.TargetType;

            dbhelp.Update(existingExternalApplication);
        }

        public void deleteExternalApplication(int id)
        {
            DBHelper._ExternalApplication dbhelp = new DBHelper._ExternalApplication();
            ExternalApplication existingExternalApplication = dbhelp.GetByid(id);

            dbhelp.Delete(existingExternalApplication);
        }
    }
}