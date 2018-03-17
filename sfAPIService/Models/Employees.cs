using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.ComponentModel.DataAnnotations;
using System.Web.Helpers;

namespace sfAPIService.Models
{
    public class EmployeeModels
    {
        public class Detail
        {
            public int Id { get; set; }
            public int CompanyId { get; set; }
            public string EmployeeNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhotoURL { get; set; }
            public bool AdminFlag { get; set; }
            public string Lang { get; set; }
            public bool DeletedFlag { get; set; }
        }

        public class Add
        {
            [Required]
            public int CompanyId { get; set; }
            public string EmployeeNumber { get; set; }
            [Required]
            public string Password { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public bool AdminFlag { get; set; }
            public string Lang { get; set; }
        }

        public class Update
        {
            [Required]
            public int CompanyId { get; set; }
            public string EmployeeNumber { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [Required]
            public bool AdminFlag { get; set; }
            public string Lang { get; set; }
            public bool? DeletedFlag { get; set; }
        }

        public List<Detail> GetAllEmployee()
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();

            return dbhelp.GetAll().Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                EmployeeNumber = s.EmployeeNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhotoURL = s.PhotoURL,
                AdminFlag = s.AdminFlag,
                Lang = s.Lang,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();
        }

        public List<Detail> GetAllEmployeeBySuperAdmin()
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();

            return dbhelp.GetAllBySuperAdmin().Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                EmployeeNumber = s.EmployeeNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhotoURL = s.PhotoURL,
                AdminFlag = s.AdminFlag,
                Lang = s.Lang,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();
        }

        public List<Detail> GetAllEmployeeByCompanyId(int companyId)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();

            return dbhelp.GetAllByCompanyId(companyId).Select(s => new Detail()
            {
                Id = s.Id,
                CompanyId = s.CompanyId,
                EmployeeNumber = s.EmployeeNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                PhotoURL = s.PhotoURL,
                AdminFlag = s.AdminFlag,
                Lang = s.Lang,
                DeletedFlag = s.DeletedFlag
            }).ToList<Detail>();
        }

        public Detail GetEmployeeById(int id)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            Employee employee = dbhelp.GetByid(id);

            return new Detail()
            {
                Id = employee.Id,
                CompanyId = employee.CompanyId,
                EmployeeNumber = employee.EmployeeNumber,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                PhotoURL = employee.PhotoURL,
                AdminFlag = employee.AdminFlag,
                Lang = employee.Lang,
                DeletedFlag = employee.DeletedFlag
            };
        }

        public object GetAllPermissionById(int id)
        {
            SFDatabaseEntities dbEntity = new SFDatabaseEntities();
            var L2Enty = from er in dbEntity.EmployeeInRole.AsNoTracking()
                         join urp in dbEntity.UserRolePermission on er.UserRoleID equals urp.UserRoleID
                         where er.EmployeeID == id && er.DeletedFlag == false && urp.DeletedFlag == false
                         orderby urp.PermissionCatalog.PermissionId ascending
                         select urp.PermissionCatalog;

            List<PermissionCatalog> permissionList = L2Enty.Distinct().ToList<PermissionCatalog>();

            return permissionList.Select(s => new  {
                PermissionId = s.PermissionId,
                PermissionName = s.Name
            });
        }

        public int addEmployee(Add employee)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            var newEmployee = new Employee()
            {
                CompanyId = employee.CompanyId,
                Password = Crypto.HashPassword(employee.Password),
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                EmployeeNumber = employee.EmployeeNumber,
                Email = employee.Email,
                AdminFlag = employee.AdminFlag,
                Lang = employee.Lang
            };
            dbhelp.Add(newEmployee);
            return newEmployee.Id;
        }

        public void updateEmployee(int id, Update employee)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            Employee existingEmployee = dbhelp.GetByid(id);
            existingEmployee.EmployeeNumber = employee.EmployeeNumber;
            existingEmployee.FirstName = employee.FirstName;
            existingEmployee.LastName = employee.LastName;
            existingEmployee.AdminFlag = employee.AdminFlag;
            existingEmployee.Lang = employee.Lang;
            if(employee.DeletedFlag.HasValue)
                existingEmployee.DeletedFlag = (bool) employee.DeletedFlag;

            dbhelp.Update(existingEmployee);
        }
        
        public void deleteEmployee(int id)
        {
            DBHelper._Employee dbhelp = new DBHelper._Employee();
            Employee existingEmployee = dbhelp.GetByid(id);

            dbhelp.Delete(existingEmployee);
        }
    }

    public class EmployeeRoleModels
    {
        public class Detail
        {
            public int UserRoleId { get; set; }
            public string UserRoleName { get; set; }
        }
        public class Edit
        {
            public int[] UserRoleId { get; set; }
        }
    }
    
}