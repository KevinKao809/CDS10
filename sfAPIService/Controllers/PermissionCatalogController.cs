using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

using System.Text;
using sfShareLib;
using sfAPIService.Models;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/PermissionCatalog")]
    public class PermissionCatalogController : ApiController
    {
        /// <summary>
        /// Roles : superadmin, admin
        /// </summary>
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
        [HttpGet]
        public IHttpActionResult GetAllPermissionCatalogs()
        {
            PermissionCatalogModels permissionCatalogModel = new Models.PermissionCatalogModels();
            return Ok(permissionCatalogModel.GetAllPermissionCatalog());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllPermissionCatalogsBySuperAdmin()
        {
            PermissionCatalogModels permissionCatalogModel = new Models.PermissionCatalogModels();
            return Ok(permissionCatalogModel.GetAllPermissionCatalogBySuperAdmin());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetPermissionCatalogById(int id)
        {
            PermissionCatalogModels permissionCatalogModel = new PermissionCatalogModels();
            try
            {
                PermissionCatalogModels.Detail permissionCatalog = permissionCatalogModel.getPermissionCatalogById(id);
                return Ok(permissionCatalog);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]PermissionCatalogModels.Add permissionCatalog)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(permissionCatalog);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || permissionCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                PermissionCatalogModels permissionCatalogModel = new PermissionCatalogModels();
                permissionCatalogModel.addPermissionCatalog(permissionCatalog);
                return Ok();
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
        /// Roles : superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] PermissionCatalogModels.Update permissionCatalog)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(permissionCatalog);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || permissionCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                PermissionCatalogModels permissionCatalogModel = new PermissionCatalogModels();
                permissionCatalogModel.updatePermissionCatalog(id, permissionCatalog);
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
        /// Roles : superadmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                PermissionCatalogModels permissionCatalogModel = new PermissionCatalogModels();
                permissionCatalogModel.deletePermissionCatalog(id);
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
    }
}
