using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using System.IO;
using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Helpers;
using sfShareLib;
using System.Web.Script.Serialization;
using System.Text;
using StackExchange.Redis;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [RoutePrefix("admin-api/Employee")]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    public class EmployeeController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAllEmployee()
        {
            EmployeeModels employeeMode = new EmployeeModels();
            return Ok(employeeMode.GetAllEmployee());
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllEmployeeBySuperAdmin()
        {
            EmployeeModels employeeMode = new EmployeeModels();
            return Ok(employeeMode.GetAllEmployeeBySuperAdmin());
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetEmployeeById(int id)
        {
            RedisKey cacheKey = "employee_" + id;
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                EmployeeModels employeeMode = new EmployeeModels();
                try
                {
                    var employee = employeeMode.GetEmployeeById(id);
                    RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(employee));
                    return Ok(employee);
                }
                catch
                {
                    return NotFound();
                }                
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<Object>(cacheValue));
            }            
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{id}")]
        public IHttpActionResult GetEmployeeByCompanyId(int id)
        {
            EmployeeModels employeeMode = new EmployeeModels();
            return Ok(employeeMode.GetAllEmployeeByCompanyId(id));    
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddEmployee([FromBody]EmployeeModels.Add employee)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(employee);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || employee == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                EmployeeModels employeeModel = new EmployeeModels();
                int employeeId = employeeModel.addEmployee(employee);
                return Json(new { id = employeeId });
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditEmployeeById(int id, [FromBody]EmployeeModels.Update employee)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(employee);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || employee == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                EmployeeModels employeeModel = new EmployeeModels();
                employeeModel.updateEmployee(id, employee);
                RedisCacheHelper.DeleteEmployeeCache(id);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }            
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult DeleteEmployeeById(int id)
        {
            try
            {
                EmployeeModels employeeModel = new EmployeeModels();
                employeeModel.deleteEmployee(id);
                RedisCacheHelper.DeleteEmployeeCache(id);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                string logAPI = "[Delete] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Image")]
        public async Task<HttpResponseMessage> UploadPhotoFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            using (var ctx = new SFDatabaseEntities())
            {
                var existEmployee = ctx.Employee
                    .Where(s => s.Id == id)
                    .FirstOrDefault();

                if (existEmployee == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                string root = Path.GetTempPath();
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    // Read the form data.
                    string fileAbsoluteUri = "";
                    await Request.Content.ReadAsMultipartAsync(provider);
                    char[] trimChar = { '\"' };

                    //FileData
                    foreach (MultipartFileData fileData in provider.FileData)
                    {
                        string formColumnName = fileData.Headers.ContentDisposition.Name.ToLower().Trim(trimChar);
                        string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Split('.')[1].ToLower().Trim(trimChar);
                        if (formColumnName.Equals("image"))
                        {
                            if (fileExtenionName.Equals("png") || fileExtenionName.Equals("jpg"))
                            {
                                string uploadFilePath = "company-" + existEmployee.CompanyId + "/employee/" + id + "-default." + fileExtenionName;
                                SharedFunctions sharedFunctions = new SharedFunctions();
                                fileAbsoluteUri = sharedFunctions.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath, "images");
                            }
                            else
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong extension name");
                        }

                    }

                    if (fileAbsoluteUri.Equals(""))
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty");

                    //Edit employee photo path
                    existEmployee.PhotoURL = fileAbsoluteUri;
                    ctx.SaveChanges();
                    RedisCacheHelper.DeleteEmployeeCache(id);

                    var returnObj = new {
                        imageURL = fileAbsoluteUri
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, returnObj);
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{id}/changepassword")]
        public IHttpActionResult ChangePassword(int id, [FromBody]ChangePasswordModels model)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(model);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || model == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AccountModels accountModel = new AccountModels();
                accountModel.ChangeEmployeePassword(id, model);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "404":
                        return NotFound();
                    case "401":
                        return Unauthorized();
                }

                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{id}/ResetPassword")]
        public IHttpActionResult ResetPassword(int id, [FromBody]ResetPasswordModels model)
        {          
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(model);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || model == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AccountModels accountModel = new AccountModels();
                accountModel.ResetEmployeePassword(id, model.NewPassword);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]        
        [Route("{id}/Permissions")]
        public IHttpActionResult GetPermissionsByEmployeeId(int id)
        {
            RedisKey cacheKey = "employee_" + id + "_Permission";
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (cacheValue == null)
            { 
                EmployeeModels employeeModel = new EmployeeModels();
                var empPermissions = employeeModel.GetAllPermissionById(id);
                RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(empPermissions));
                return Ok(empPermissions);
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<List<Object>>(cacheValue));
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{id}/Role")]
        public IHttpActionResult GetRolesByEmployeeId(int id)
        {
            RedisKey cacheKey = "employee_" + id + "_Role";
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                using (var ctx = new SFDatabaseEntities())
                {
                    var roles = ctx.EmployeeInRole
                        .Where(s => s.EmployeeID == id && s.DeletedFlag == false)
                        .Select(s => new EmployeeRoleModels.Detail()
                        {
                            UserRoleId = s.UserRoleID,
                            UserRoleName = s.UserRole.Name
                        }).ToList<EmployeeRoleModels.Detail>();

                    RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(roles));                    
                    return Ok(roles);
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<List<Object>>(cacheValue));
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPost]
        [Route("{id}/Role")]
        public IHttpActionResult AddRolesByEmployeeId(int id, [FromBody] EmployeeRoleModels.Edit roles)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            using (var ctx = new SFDatabaseEntities())
            {
                foreach (var roleId in roles.UserRoleId)
                {
                    EmployeeInRole newEmployeeRole = new EmployeeInRole()
                    {
                        EmployeeID = id,
                        UserRoleID = roleId,
                        CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                        DeletedFlag = false
                    };
                    ctx.EmployeeInRole.Add(newEmployeeRole);
                }

                StringBuilder logMessage = new StringBuilder();
                try
                {
                    ctx.SaveChanges();
                    RedisCacheHelper.DeleteEmployeeCache(id);

                    logMessage.AppendLine("(AddRolesByEmployeeId) Delete EmployCache: " + id);
                    Startup._sfAppLogger.Debug(logMessage);
                    return Ok();
                }
                catch(Exception ex)
                {
                    logMessage.AppendLine("(AddRolesByEmployeeId) Excepton on Delete EmployCache: " + id + "; Exception:" + ex.Message);
                    Startup._sfAppLogger.Error(logMessage);
                    return InternalServerError();
                }
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Role")]
        public IHttpActionResult EditRolesByEmployeeId(int id, [FromBody] EmployeeRoleModels.Edit roles)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            using (var ctx = new SFDatabaseEntities())
            {
                var existingRoles = ctx.EmployeeInRole
                    .Where(s => s.EmployeeID == id)
                    .Select(s => s).ToList();

                //調整現有的Role
                List<int> existingRolesId = new List<int>();
                if (existingRoles != null)
                {
                    foreach (var er in existingRoles)
                    {
                        if (roles == null || (!roles.UserRoleId.Contains(er.UserRoleID) && !er.DeletedFlag))
                        {
                            er.DeletedFlag = true;
                            er.UpdatedAt = DateTime.Parse(DateTime.Now.ToString());
                        }
                        else if (roles.UserRoleId.Contains(er.UserRoleID) && er.DeletedFlag)
                        {
                            er.DeletedFlag = false;
                            er.UpdatedAt = DateTime.Parse(DateTime.Now.ToString());
                        }

                        existingRolesId.Add(er.UserRoleID);
                    }
                }
                //新增沒有的Role
                if (roles != null)
                {
                    foreach (var userRoleId in roles.UserRoleId)
                    {
                        if (existingRoles == null || (userRoleId > 0 && !existingRolesId.Contains(userRoleId)) )
                        {
                            var newEmployeeRole = new EmployeeInRole()
                            {
                                EmployeeID = id,
                                UserRoleID = userRoleId,
                                CreatedAt = DateTime.Parse(DateTime.Now.ToString())
                            };
                            ctx.EmployeeInRole.Add(newEmployeeRole);
                        }
                    }
                }

                StringBuilder logMessage = new StringBuilder();
                try
                {
                    ctx.SaveChanges();
                    RedisCacheHelper.DeleteEmployeeCache(id);

                    logMessage.AppendLine("(EditRolesByEmployeeId )Delete EmployCache: " + id);
                    Startup._sfAppLogger.Debug(logMessage);
                    return Ok();
                }
                catch (Exception ex)
                {
                    logMessage.AppendLine("(EditRolesByEmployeeId) Excepton on Delete EmployCache: " + id + "; Exception:" + ex.Message);
                    Startup._sfAppLogger.Error(logMessage);
                    return InternalServerError();
                }
            }
        }

    }
}
