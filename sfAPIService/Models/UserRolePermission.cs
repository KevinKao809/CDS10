using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class UserRolePermissionModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int UserRoleId { get; set; }
            public string UserRoleName { get; set; }
            public int PermissionCatalogId { get; set; }
            public string PermissionCatalogName { get; set; }
            public string PermissionDescription { get; set; }
        }
        public class Edit
        {
            [Required]
            public int UserRoleId { get; set; }
            [Required]
            public int PermissionCatalogId { get; set; }
        }

        public List<Detail> GetAllUserRolePermissionByUserRoleId(int userRoleId)
        {
            DBHelper._UserRolePermission dbhelp = new DBHelper._UserRolePermission();

            return dbhelp.GetAllByUserRoleId(userRoleId).Select(s => new Detail()
            {
                Id = s.Id,
                UserRoleId = s.UserRole.Id,
                UserRoleName = s.UserRole.Name,
                PermissionCatalogId = s.PermissionCatalog.Id,
                PermissionCatalogName = s.PermissionCatalog.Name,
                PermissionDescription = s.PermissionCatalog.Description
            }).ToList<Detail>();

        }

        public Detail getUserRolePermissionById(int id)
        {
            DBHelper._UserRolePermission dbhelp = new DBHelper._UserRolePermission();
            UserRolePermission userRolePermission = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = userRolePermission.Id,
                UserRoleId = userRolePermission.UserRole.Id,
                UserRoleName = userRolePermission.UserRole.Name,
                PermissionCatalogId = userRolePermission.PermissionCatalog.Id,
                PermissionCatalogName = userRolePermission.PermissionCatalog.Name,
                PermissionDescription = userRolePermission.PermissionCatalog.Description
            };
        }

        public void addUserRolePermission(Edit userRolePermission)
        {
            DBHelper._UserRolePermission dbhelp = new DBHelper._UserRolePermission();
            var newUserRolePermission = new UserRolePermission()
            {
                UserRoleID = userRolePermission.UserRoleId,
                PermissionCatalogID = userRolePermission.PermissionCatalogId
            };
            dbhelp.Add(newUserRolePermission);
        }

        public void updateUserRolePermission(int id, Edit userRolePermission)
        {
            DBHelper._UserRolePermission dbhelp = new DBHelper._UserRolePermission();
            UserRolePermission existingUserRolePermission = dbhelp.GetByid(id);
            existingUserRolePermission.PermissionCatalogID = userRolePermission.PermissionCatalogId;
            existingUserRolePermission.UserRoleID = userRolePermission.UserRoleId;

            dbhelp.Update(existingUserRolePermission);
        }

        public void deleteUserRolePermission(int id)
        {
            DBHelper._UserRolePermission dbhelp = new DBHelper._UserRolePermission();
            UserRolePermission existingUserRolePermission = dbhelp.GetByid(id);

            dbhelp.Delete(existingUserRolePermission);
        }
    }
}