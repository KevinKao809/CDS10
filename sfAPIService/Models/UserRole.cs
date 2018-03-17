using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;

namespace sfAPIService.Models
{
    public class UserRoleModels
    {
        public class UserRolePermissionCatalog
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
        }
        public class Detail
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public List<UserRolePermissionCatalog> PermissionCatalogList { get; set; }
        }

        public class Edit
        {
            [Required]
            public int CompanyId { get; set; }
            [Required]
            [MaxLength(50)]
            public string Name { get; set; }
            public int[] PermissionCatalogId { get; set; }
        }

        public List<Detail> GetAllUserRoleByCompanyId(int companyId)
        {
            DBHelper._UserRole dbhelp = new DBHelper._UserRole();
            UserRolePermissionModels userRolePermissionModels = new UserRolePermissionModels();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                Name = s.Name,
                PermissionCatalogList = userRolePermissionModels.GetAllUserRolePermissionByUserRoleId(s.Id).Select(t => new UserRolePermissionCatalog()
                {
                    Id = t.PermissionCatalogId,
                    Name = t.PermissionCatalogName,
                    Description = t.PermissionDescription
                }).ToList<UserRolePermissionCatalog>()
            }).ToList<Detail>();

        }

        public Detail getUserRoleById(int id)
        {
            DBHelper._UserRole dbhelp_userRole = new DBHelper._UserRole();
            UserRolePermissionModels userRolePermissionModels = new UserRolePermissionModels();
            UserRole userRole = dbhelp_userRole.GetByid(id);

            return new Detail()
            {
                Id = userRole.Id,
                Name = userRole.Name,
                PermissionCatalogList = userRolePermissionModels.GetAllUserRolePermissionByUserRoleId(id).Select(s => new UserRolePermissionCatalog()
                {
                    Id = s.PermissionCatalogId,
                    Name = s.PermissionCatalogName,
                    Description = s.PermissionDescription
                }).ToList<UserRolePermissionCatalog>()
            };
        }

        public void addUserRole(Edit userRole)
        {
            DBHelper._UserRole dbhelp_useRole = new DBHelper._UserRole();
            var newUserRole = new UserRole()
            {
                Name = userRole.Name,
                CompanyId = userRole.CompanyId,
                DeletedFlag = false
            };
            int newUserRoleId = dbhelp_useRole.Add(newUserRole);

            DBHelper._UserRolePermission dbhelp_useRolePermission = new DBHelper._UserRolePermission();
            List<UserRolePermission> userRolePermissionList = new List<UserRolePermission>();
            foreach (int permissionCatalogId in userRole.PermissionCatalogId)
            {
                var newUserRolePermission = new UserRolePermission()
                {
                    UserRoleID = newUserRoleId,
                    PermissionCatalogID = permissionCatalogId
                };
                userRolePermissionList.Add(newUserRolePermission);
            }
            dbhelp_useRolePermission.AddManyRows(userRolePermissionList);
        }

        public void updateUserRole(int id, Edit userRole)
        {
            DBHelper._UserRole dbhelp_useRole = new DBHelper._UserRole();
            UserRole existingUserRole = dbhelp_useRole.GetByid(id);
            existingUserRole.Name = userRole.Name;
            existingUserRole.CompanyId = userRole.CompanyId;

            dbhelp_useRole.Update(existingUserRole);

            DBHelper._UserRolePermission dbhelp_useRolePermission = new DBHelper._UserRolePermission();
            List<UserRolePermission> existingPermissionList = dbhelp_useRolePermission.GetAllByUserRoleIdIncludeDelete(id);
            List<UserRolePermission> insertPermissionList = new List<UserRolePermission>();
            List<int> existingPermissionIdList = new List<int>();

            if (existingPermissionList.Count > 0)
            {
                foreach (var eurp in existingPermissionList)
                {
                    if (userRole.PermissionCatalogId == null || (!userRole.PermissionCatalogId.Contains(eurp.PermissionCatalogID) && !eurp.DeletedFlag))
                    {
                        eurp.DeletedFlag = true;
                    }
                    else if (userRole.PermissionCatalogId.Contains(eurp.PermissionCatalogID) && eurp.DeletedFlag)
                    {
                        eurp.DeletedFlag = false;
                    }
                    existingPermissionIdList.Add(eurp.PermissionCatalogID);
                }
                dbhelp_useRolePermission.UpdateManyRows(existingPermissionList);
            }
            

            if (userRole.PermissionCatalogId != null)
            {
                foreach (var permissionCatalogId in userRole.PermissionCatalogId)
                {
                    if (existingPermissionList.Count == 0 || (permissionCatalogId > 0 && !existingPermissionIdList.Contains(permissionCatalogId)))
                    {
                        var newUserRolePermissio = new UserRolePermission()
                        {
                            UserRoleID = id,
                            PermissionCatalogID = permissionCatalogId
                        };
                        insertPermissionList.Add(newUserRolePermissio);
                    }
                }
                dbhelp_useRolePermission.AddManyRows(insertPermissionList);
            }
        }

        public void deleteUserRole(int id)
        {
            DBHelper._UserRole dbhelp_userRole = new DBHelper._UserRole();
            UserRole existingUserRole = dbhelp_userRole.GetByid(id);
            dbhelp_userRole.Delete(existingUserRole);

            DBHelper._EmployeeInRole dbhelp_employeeInRole = new DBHelper._EmployeeInRole();

            List<EmployeeInRole> employeeInRoleList = dbhelp_employeeInRole.GetAllByUserRoleId(id);
            dbhelp_employeeInRole.Delete(employeeInRoleList);
        }
    }
}