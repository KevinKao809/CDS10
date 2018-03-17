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
    [RoutePrefix("admin-api/IoTDeviceCustomizedConfiguration")]
    public class IoTDeviceCustomizedConfigurationController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetIoTDeviceConfigurationById(int id)
        {
            IoTDeviceCustomizedConfigurationModels model = new IoTDeviceCustomizedConfigurationModels();
            try
            {
                IoTDeviceCustomizedConfigurationModels.Detail config = model.getCustomizedConfigurationById(id);
                return Ok(config);
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
        public IHttpActionResult AddFormData([FromBody]IoTDeviceCustomizedConfigurationModels.Add config)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(config);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || config == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceCustomizedConfigurationModels model = new IoTDeviceCustomizedConfigurationModels();
                model.addIoTDeviceConfiguration(config);
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
        public IHttpActionResult EditFormData(int id, [FromBody] IoTDeviceCustomizedConfigurationModels.Update config)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(config);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || config == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceCustomizedConfigurationModels model = new IoTDeviceCustomizedConfigurationModels();
                model.updateCustomizedConfiguration(id, config);
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
                IoTDeviceCustomizedConfigurationModels model = new IoTDeviceCustomizedConfigurationModels();
                model.deleteIoTDeviceConfiguration(id);
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
