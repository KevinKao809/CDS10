using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class PermissionCatalogModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int PermissionId { get; set; }
            public bool? DeleteFlag { get; set; }
        }
        public class Add
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int PermissionId { get; set; }
        }

        public class Update
        {
            [Required]
            public string Name { get; set; }
            public string Description { get; set; }
            [Required]
            public int PermissionId { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> GetAllPermissionCatalog()
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                PermissionId = s.PermissionId,
                DeleteFlag = s.DeletedFlag
            }).ToList<Detail>();

        }

        public List<Detail> GetAllPermissionCatalogBySuperAdmin()
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                PermissionId = s.PermissionId,
                DeleteFlag = s.DeletedFlag
            }).ToList<Detail>();
        }

        public Detail getPermissionCatalogById(int id)
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();
            PermissionCatalog permissionCatalog = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = permissionCatalog.Id,
                Name = permissionCatalog.Name,
                Description = permissionCatalog.Description,
                PermissionId = permissionCatalog.PermissionId,
                DeleteFlag = permissionCatalog.DeletedFlag
            };
        }

        public void addPermissionCatalog(Add permissionCatalog)
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();
            var newPermissionCatalog = new PermissionCatalog()
            {
                Name = permissionCatalog.Name,
                Description = permissionCatalog.Description,
                PermissionId = permissionCatalog.PermissionId
            };
            dbhelp.Add(newPermissionCatalog);
        }

        public void updatePermissionCatalog(int id, Update permissionCatalog)
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();
            PermissionCatalog existingPermissionCatalog = dbhelp.GetByid(id);
            existingPermissionCatalog.Name = permissionCatalog.Name;
            existingPermissionCatalog.Description = permissionCatalog.Description;
            existingPermissionCatalog.PermissionId = permissionCatalog.PermissionId;
            if(permissionCatalog.DeletedFlag.HasValue)
                existingPermissionCatalog.DeletedFlag = (bool) permissionCatalog.DeletedFlag;

            dbhelp.Update(existingPermissionCatalog);
        }

        public void deletePermissionCatalog(int id)
        {
            DBHelper._PermissionCatalog dbhelp = new DBHelper._PermissionCatalog();
            PermissionCatalog existingPermissionCatalog = dbhelp.GetByid(id);

            dbhelp.Delete(existingPermissionCatalog);
        }
    }
}