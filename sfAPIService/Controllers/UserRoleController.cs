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
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/UserRole")]
    public class UserRoleController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllCompanies(int companyId)
        {
            UserRoleModels userRoleModel = new Models.UserRoleModels();
            return Ok(userRoleModel.GetAllUserRoleByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetCompanyById(int id)
        {
            UserRoleModels userRoleModel = new UserRoleModels();
            try
            {
                UserRoleModels.Detail company = userRoleModel.getUserRoleById(id);
                return Ok(company);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]UserRoleModels.Edit userRole)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(userRole);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || userRole == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                UserRoleModels userRoleModel = new UserRoleModels();
                userRoleModel.addUserRole(userRole);
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
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] UserRoleModels.Edit userRole)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(userRole);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || userRole == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                UserRoleModels userRoleModel = new UserRoleModels();
                userRoleModel.updateUserRole(id, userRole);
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
        public IHttpActionResult Delete(int id)
        {
            try
            {
                UserRoleModels userRoleModel = new UserRoleModels();
                userRoleModel.deleteUserRole(id);
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
