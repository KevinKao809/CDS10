using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using sfShareLib;

using sfAPIService.Models;
using System.Web.Script.Serialization;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/ExternalApplication")]
    public class ExternalApplicationController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllExternalApplicationByCompanyId(int companyId)
        {
            ExternalApplicationModels externalApplicationModel = new Models.ExternalApplicationModels();
            return Ok(externalApplicationModel.GetAllExternalApplicationByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetExternalApplicationById(int id)
        {
            ExternalApplicationModels externalApplicationModel = new ExternalApplicationModels();
            try
            {
                ExternalApplicationModels.Detail company = externalApplicationModel.getExternalApplicationById(id);
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
        public IHttpActionResult AddFormData([FromBody]ExternalApplicationModels.Add externalApplication)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(externalApplication);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || externalApplication == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                ExternalApplicationModels externalApplicationModel = new ExternalApplicationModels();
                externalApplicationModel.addExternalApplication(externalApplication);
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
        public IHttpActionResult EditFormData(int id, [FromBody] ExternalApplicationModels.Update externalApplication)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(externalApplication);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || externalApplication == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                ExternalApplicationModels externalApplicationModel = new ExternalApplicationModels();
                externalApplicationModel.updateExternalApplication(id, externalApplication);
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
                ExternalApplicationModels externalApplicationModel = new ExternalApplicationModels();
                externalApplicationModel.deleteExternalApplication(id);
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
