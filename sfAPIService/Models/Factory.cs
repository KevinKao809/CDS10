using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using sfShareLib;

namespace sfAPIService.Models
{
    public class FactoryModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int CompanyId { get; set; }
            public string PhotoURL { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int TimeZone { get; set; }
            public string CultureInfoId { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Detail_readonly
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string PhotoURL { get; set; }
            public string Latitude { get; set; }
            public string Longitude { get; set; }
            public int TimeZone { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Edit
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string Description { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            [Required]
            public int TimeZone { get; set; }
            public string CultureInfoId { get; set; }
        }

        public List<Detail> GetAllFactoryByCompanyId(int companyId)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CompanyId = s.CompanyId,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoURL = s.PhotoURL,
                TimeZone = s.TimeZone,
                CultureInfoId = s.CultureInfo,
                CultureInfoName = s.RefCultureInfo.Name
            }).ToList<Detail>();
        }

        public List<Detail_readonly> GetAllFactoryByCompanyIdReadonly(int companyId)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail_readonly()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                Latitude = (s.Latitude == null) ? "" : s.Latitude.ToString(),
                Longitude = (s.Longitude == null) ? "" : s.Longitude.ToString(),
                PhotoURL = s.PhotoURL,
                TimeZone = s.TimeZone,
                CultureInfoName = s.RefCultureInfo.Name
            }).ToList<Detail_readonly>();
        }

        public Detail getFactoryById(int id)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();
            Factory factory = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = factory.Id,
                Name = factory.Name,
                Description = factory.Description,
                CompanyId = factory.CompanyId,
                Latitude = (factory.Latitude == null) ? "" : factory.Latitude.ToString(),
                Longitude = (factory.Longitude == null) ? "" : factory.Longitude.ToString(),
                PhotoURL = factory.PhotoURL,
                TimeZone = factory.TimeZone,
                CultureInfoId = factory.CultureInfo,
                CultureInfoName = factory.RefCultureInfo.Name
            };
        }

        public Detail_readonly getFactoryByIdReadonly(int id, int companyId)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();
            Factory factory = dbhelp.GetByid(id, companyId);

            return new Detail_readonly()
            {
                Id = factory.Id,
                Name = factory.Name,
                Description = factory.Description,
                Latitude = (factory.Latitude == null) ? "" : factory.Latitude.ToString(),
                Longitude = (factory.Longitude == null) ? "" : factory.Longitude.ToString(),
                PhotoURL = factory.PhotoURL,
                TimeZone = factory.TimeZone,
                CultureInfoName = factory.RefCultureInfo.Name
            };
        }

        public void addFactory(Edit factory)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();
            var newFactory = new Factory()
            {
                Name = factory.Name,
                Description = factory.Description,
                CompanyId = factory.CompanyId,
                Latitude = factory.Latitude,
                Longitude = factory.Longitude,
                TimeZone = factory.TimeZone,
                CultureInfo = factory.CultureInfoId
            };
            dbhelp.Add(newFactory);
        }

        public void updateFactory(int id, Edit factory)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();
            Factory existingFactory = dbhelp.GetByid(id);
            existingFactory.Name = factory.Name;
            existingFactory.Description = factory.Description;
            existingFactory.Latitude = factory.Latitude;
            existingFactory.Longitude = factory.Longitude;
            existingFactory.TimeZone = factory.TimeZone;
            existingFactory.CultureInfo = factory.CultureInfoId;

            dbhelp.Update(existingFactory);
        }

        public void deleteFactory(int id)
        {
            DBHelper._Factory dbhelp = new DBHelper._Factory();
            Factory existingFactory = dbhelp.GetByid(id);

            dbhelp.Delete(existingFactory);
        }
    }
}