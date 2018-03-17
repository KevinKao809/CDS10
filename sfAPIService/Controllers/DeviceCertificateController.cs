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
    [RoutePrefix("admin-api/DeviceCertificate")]
    public class DeviceCertificateController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllCompanies(int companyId)
        {
            DeviceCertificateModels deviceCertificateModel = new Models.DeviceCertificateModels();
            return Ok(deviceCertificateModel.GetAllDeviceCertificateByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetCompanyById(int id)
        {
            DeviceCertificateModels deviceCertificateModel = new DeviceCertificateModels();
            try
            {
                DeviceCertificateModels.Detail company = deviceCertificateModel.getDeviceCertificateById(id);
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
        public IHttpActionResult AddFormData([FromBody]DeviceCertificateModels.Edit deviceCertificate)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(deviceCertificate);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || deviceCertificate == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DeviceCertificateModels deviceCertificateModel = new DeviceCertificateModels();
                deviceCertificateModel.addDeviceCertificate(deviceCertificate);
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
        public IHttpActionResult EditFormData(int id, [FromBody] DeviceCertificateModels.Edit deviceCertificate)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(deviceCertificate);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || deviceCertificate == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DeviceCertificateModels deviceCertificateModel = new DeviceCertificateModels();
                deviceCertificateModel.updatedeviceCertificate(id, deviceCertificate);
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
                DeviceCertificateModels deviceCertificateModel = new DeviceCertificateModels();
                deviceCertificateModel.deleteDeviceCertificate(id);
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
