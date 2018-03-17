using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using System.Web.Http;

using sfAPIService.Models;
using System.Web.Script.Serialization;
using System.Text;
using sfShareLib;
using System.Web.Helpers;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [RoutePrefix("admin-api/SuperAdmin")]
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    public class SuperAdminController : ApiController
    {
        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAllSuperAdmins()
        {
            using (var ctx = new SFDatabaseEntities())
            {
                var superAdmins = ctx.SuperAdmin
                    .Select(s => new SuperAdminModels.Detail
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Email = s.Email,
                        CreatedAt = s.CreatedAt,
                        DeletedFlag = s.DeletedFlag
                    }).ToList<SuperAdminModels.Detail>();

                return Ok(superAdmins);
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetSuperAdminById(int id)
        {
            using (var ctx = new SFDatabaseEntities())
            {
                var superAdmin = ctx.SuperAdmin
                    .Where(s => s.Id == id)
                    .Select(s => new SuperAdminModels.Detail()
                    {
                        Id = s.Id,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Email = s.Email,
                        CreatedAt = s.CreatedAt,
                        DeletedFlag = s.DeletedFlag
                    }).FirstOrDefault<SuperAdminModels.Detail>();

                if (superAdmin == null)
                {
                    return NotFound();
                }
                return Ok(superAdmin);
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddSuperAdmin([FromBody]SuperAdminModels.Edit superAdmin)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(superAdmin);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            using (var ctx = new SFDatabaseEntities())
            {
                SuperAdmin newSuperAdmin = new SuperAdmin()
                {
                    FirstName = superAdmin.FirstName,
                    LastName = superAdmin.LastName,
                    Email = superAdmin.Email,
                    Password = Crypto.HashPassword(superAdmin.Password),
                    CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                    DeletedFlag = superAdmin.DeletedFlag
                };

                ctx.SuperAdmin.Add(newSuperAdmin);
                try
                {
                    ctx.SaveChanges();
                    return Ok();
                }
                catch(Exception ex)
                {
                    StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                    logMessage.AppendLine(logForm);
                    Startup._sfAppLogger.Error(logAPI + logMessage);
                    return InternalServerError();
                }
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditSuperAdminById(int id, [FromBody]SuperAdminModels.Edit superAdmin)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(superAdmin);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest("Invalid data");


            using (var ctx = new SFDatabaseEntities())
            {
                var existingSuperAdmin = ctx.SuperAdmin
                    .Where(s => s.Id == id)
                    .FirstOrDefault();
                if (existingSuperAdmin != null)
                {
                    existingSuperAdmin.FirstName = superAdmin.FirstName;
                    existingSuperAdmin.LastName = superAdmin.LastName;
                    existingSuperAdmin.Email = superAdmin.Email;
                    existingSuperAdmin.UpdatedAt = DateTime.Parse(DateTime.Now.ToString());
                    existingSuperAdmin.DeletedFlag = superAdmin.DeletedFlag;
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                        logMessage.AppendLine(logForm);
                        Startup._sfAppLogger.Error(logAPI + logMessage);
                        return InternalServerError();
                    }
                }
                else
                {
                    return NotFound();
                }

                return Ok("Success");
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult DeleteSuperAdminById(int id)
        {
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            using (var ctx = new SFDatabaseEntities())
            {
                var superAdmin = ctx.SuperAdmin
                    .Where(s => s.Id == id)
                    .FirstOrDefault();
                if (superAdmin != null)
                {
                    superAdmin.DeletedFlag = true;
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                        Startup._sfAppLogger.Error(logAPI + logMessage);
                        return InternalServerError();
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            return Ok("Success");
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpPut]
        [Route("{id}/changepassword")]
        public IHttpActionResult ChangePassword(int id, [FromBody]ChangePasswordModels newPasswords)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(newPasswords);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest("Invalid data");

            using (var ctx = new SFDatabaseEntities())
            {
                var existingSuperAdmin = ctx.SuperAdmin
                        .Where(s => s.Id == id)
                        .FirstOrDefault();

                if (existingSuperAdmin == null)
                    return NotFound();

                if (Crypto.VerifyHashedPassword(existingSuperAdmin.Password, newPasswords.OldPassword))
                {
                    existingSuperAdmin.Password = Crypto.HashPassword(newPasswords.NewPassword);
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                        logMessage.AppendLine(logForm);
                        Startup._sfAppLogger.Error(logAPI + logMessage);
                        return InternalServerError();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            return Ok("Success");
        }

    }
}
