using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class DeviceCertificateModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string FileName { get; set; }
            public string Thumbprint { get; set; }
            public string PFXPassword { get; set; }
            public DateTime ExpiredAt { get; set; }
        }
        public class Edit
        {
            [Required]
            public string Name { get; set; }
            [Required]
            public int CompanyId { get; set; }
            public string FileName { get; set; }
            public string Thumbprint { get; set; }
            public string PFXPassword { get; set; }
            public DateTime ExpiredAt { get; set; }
        }

        public List<Detail> GetAllDeviceCertificateByCompanyId(int companyId)
        {
            DBHelper._DeviceCertificate dbhelp = new DBHelper._DeviceCertificate();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                FileName = s.FileName,
                Thumbprint = s.Thumbprint,
                PFXPassword = s.PFXPassword,
                ExpiredAt = (DateTime)s.ExpiredAt
            }).ToList<Detail>();

        }

        public Detail getDeviceCertificateById(int id)
        {
            DBHelper._DeviceCertificate dbhelp = new DBHelper._DeviceCertificate();
            DeviceCertificate deviceCertificate = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = deviceCertificate.Id,
                Name = deviceCertificate.Name,
                FileName = deviceCertificate.FileName,
                Thumbprint = deviceCertificate.Thumbprint,
                PFXPassword = deviceCertificate.PFXPassword,
                ExpiredAt = (DateTime)deviceCertificate.ExpiredAt
            };
        }

        public void addDeviceCertificate(Edit deviceCertificate)
        {
            DBHelper._DeviceCertificate dbhelp = new DBHelper._DeviceCertificate();
            var newDeviceCertificate = new DeviceCertificate()
            {
                Name = deviceCertificate.Name,
                CompanyID = deviceCertificate.CompanyId,
                FileName = deviceCertificate.FileName,
                Thumbprint = deviceCertificate.Thumbprint,
                PFXPassword = deviceCertificate.PFXPassword,
                ExpiredAt = (DateTime)deviceCertificate.ExpiredAt,
                DeletedFlag = false
            };
            dbhelp.Add(newDeviceCertificate);
        }

        public void updatedeviceCertificate(int id, Edit deviceCertificate)
        {
            DBHelper._DeviceCertificate dbhelp = new DBHelper._DeviceCertificate();
            DeviceCertificate existingDeviceCertificate = dbhelp.GetByid(id);
            existingDeviceCertificate.Name = deviceCertificate.Name;
            existingDeviceCertificate.CompanyID = deviceCertificate.CompanyId;
            existingDeviceCertificate.FileName = deviceCertificate.FileName;
            existingDeviceCertificate.Thumbprint = deviceCertificate.Thumbprint;
            existingDeviceCertificate.PFXPassword= deviceCertificate.PFXPassword;
            existingDeviceCertificate.ExpiredAt = deviceCertificate.ExpiredAt;

            dbhelp.Update(existingDeviceCertificate);
        }

        public void deleteDeviceCertificate(int id)
        {
            DBHelper._DeviceCertificate dbhelp = new DBHelper._DeviceCertificate();
            DeviceCertificate existingDeviceCertificate = dbhelp.GetByid(id);

            dbhelp.Delete(existingDeviceCertificate);
        }
    }
}