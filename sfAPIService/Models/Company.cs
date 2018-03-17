using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class CompanyModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string LogoURL { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string CultureInfoName { get; set; }
            public string DocDBConnectionString { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
            public bool DeletedFlag { get; set; }
        }

        public class Detail_readonly
        {
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public string LogoURL { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoName { get; set; }
        }

        public class Add
        {
            [Required]
            public string Name { get; set; }
            [MaxLength(10)]
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string DocDBConnectionString { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
        }
        public class Update
        {
            [Required]
            public string Name { get; set; }
            [MaxLength(10)]
            public string ShortName { get; set; }
            public string Address { get; set; }
            public string CompanyWebSite { get; set; }
            public string ContactName { get; set; }
            public string ContactPhone { get; set; }
            public string ContactEmail { get; set; }
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string CultureInfoId { get; set; }
            public string DocDBConnectionString { get; set; }
            public string AllowDomain { get; set; }
            public string ExtAppAuthenticationKey { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> getAllCompanies()
        {
            DBHelper._Company dbhelp = new DBHelper._Company();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                ShortName = s.ShortName,
                Address = s.Address,
                CompanyWebSite = s.CompanyWebSite,
                ContactName = s.ContactName,
                ContactPhone = s.ContactPhone,
                ContactEmail = s.ContactEmail,
                Latitude = (float)s.Latitude,
                Longitude = (float)s.Longitude,
                LogoURL = s.LogoURL,
                CultureInfoId = s.CultureInfo,
                CultureInfoName = (s.RefCultureInfo == null ? "" : s.RefCultureInfo.Name),
                DocDBConnectionString = s.DocDBConnectionString,
                AllowDomain = s.AllowDomain,
                ExtAppAuthenticationKey = s.ExtAppAuthenticationKey,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();
        }

        public List<Detail> getAllCompaniesBySuperAdmin()
        {
            DBHelper._Company dbhelp = new DBHelper._Company();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                ShortName = s.ShortName,
                Address = s.Address,
                CompanyWebSite = s.CompanyWebSite,
                ContactName = s.ContactName,
                ContactPhone = s.ContactPhone,
                ContactEmail = s.ContactEmail,
                Latitude = (float)s.Latitude,
                Longitude = (float)s.Longitude,
                LogoURL = s.LogoURL,
                CultureInfoId = s.CultureInfo,
                CultureInfoName = (s.RefCultureInfo == null ? "" : s.RefCultureInfo.Name),
                DocDBConnectionString = s.DocDBConnectionString,
                AllowDomain = s.AllowDomain,
                ExtAppAuthenticationKey = s.ExtAppAuthenticationKey,
                DeletedFlag = s.DeletedFlag,
            }).ToList<Detail>();
        }

        public Detail getCompanyById(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company company = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = company.Id,
                Name = company.Name,
                ShortName = company.ShortName,
                Address = company.Address,
                CompanyWebSite = company.CompanyWebSite,
                ContactName = company.ContactName,
                ContactPhone = company.ContactPhone,
                ContactEmail = company.ContactEmail,
                Latitude = (float)company.Latitude,
                Longitude = (float)company.Longitude,
                LogoURL = company.LogoURL,
                CultureInfoId = company.CultureInfo,
                CultureInfoName = (company.RefCultureInfo == null ? "" : company.RefCultureInfo.Name),
                DocDBConnectionString = company.DocDBConnectionString,
                AllowDomain = company.AllowDomain,
                ExtAppAuthenticationKey = company.ExtAppAuthenticationKey,
                DeletedFlag = company.DeletedFlag
            };
        }

        public Detail_readonly getCompanyByIdReadonly(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company company = dbhelp.GetByid(id);

            return new Detail_readonly()
            {
                Name = company.Name,
                ShortName = company.ShortName,
                Address = company.Address,
                CompanyWebSite = company.CompanyWebSite,
                ContactName = company.ContactName,
                ContactPhone = company.ContactPhone,
                ContactEmail = company.ContactEmail,
                Latitude = (float)company.Latitude,
                Longitude = (float)company.Longitude,
                LogoURL = company.LogoURL,
                CultureInfoName = (company.RefCultureInfo == null ? "" : company.RefCultureInfo.Name)
            };
        }

        public int addCompany(Add company)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            var newCompany = new Company()
            {
                Name = company.Name,
                ShortName = company.ShortName,
                Address = company.Address,
                CompanyWebSite = company.CompanyWebSite,
                ContactName = company.ContactName,
                ContactPhone = company.ContactPhone,
                ContactEmail = company.ContactEmail,
                Latitude = (float)company.Latitude,
                Longitude = (float)company.Longitude,
                CultureInfo = company.CultureInfoId,
                CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                DeletedFlag = false,
                DocDBConnectionString = company.DocDBConnectionString,
                AllowDomain = company.AllowDomain,
                ExtAppAuthenticationKey = company.ExtAppAuthenticationKey
            };
            return dbhelp.Add(newCompany);
        }

        public void updateCompany(int id, Update company)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);
            existingCompany.Name = company.Name;
            existingCompany.ShortName = company.ShortName;
            existingCompany.Address = company.Address;
            existingCompany.CompanyWebSite = company.CompanyWebSite;
            existingCompany.ContactName = company.ContactName;
            existingCompany.ContactEmail = company.ContactEmail;
            existingCompany.ContactPhone = company.ContactPhone;
            existingCompany.Latitude = company.Latitude;
            existingCompany.Longitude = company.Longitude;
            existingCompany.CultureInfo = company.CultureInfoId;
            existingCompany.DocDBConnectionString = company.DocDBConnectionString;
            existingCompany.AllowDomain = company.AllowDomain;
            existingCompany.ExtAppAuthenticationKey = company.ExtAppAuthenticationKey;
            if (company.DeletedFlag.HasValue)
                existingCompany.DeletedFlag = (bool) company.DeletedFlag;

            dbhelp.Update(existingCompany);
        }

        public void deleteCompany(int id)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);

            dbhelp.Delete(existingCompany);
        }

        public void updateCompanyLogoURL(int id, string url)
        {
            DBHelper._Company dbhelp = new DBHelper._Company();
            Company existingCompany = dbhelp.GetByid(id);
            existingCompany.LogoURL = url;
            dbhelp.Update(existingCompany);
        }
    }
}