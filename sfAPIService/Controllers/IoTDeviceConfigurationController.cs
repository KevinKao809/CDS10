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
    [RoutePrefix("admin-api/IoTDeviceConfiguration")]
    public class IoTDeviceConfigurationController : ApiController
    {
        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            IoTDeviceConfigurationModels iotDeviceConfigurationModel = new Models.IoTDeviceConfigurationModels();
            return Ok(iotDeviceConfigurationModel.GetAllIoTDeviceConfiguration());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetIoTDeviceConfigurationById(int id)
        {
            IoTDeviceConfigurationModels iotDeviceConfigurationModel = new IoTDeviceConfigurationModels();
            try
            {
                IoTDeviceConfigurationModels.Detail company = iotDeviceConfigurationModel.getIoTDeviceConfigurationById(id);
                return Ok(company);
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
        public IHttpActionResult AddFormData([FromBody]IoTDeviceConfigurationModels.Edit iotDeviceConfiguration)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(iotDeviceConfiguration);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || iotDeviceConfiguration == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceConfigurationModels iotDeviceConfigurationModel = new IoTDeviceConfigurationModels();
                iotDeviceConfigurationModel.addIoTDeviceConfiguration(iotDeviceConfiguration);
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
        public IHttpActionResult EditFormData(int id, [FromBody] IoTDeviceConfigurationModels.Edit iotDeviceConfiguration)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(iotDeviceConfiguration);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || iotDeviceConfiguration == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceConfigurationModels iotDeviceConfigurationModel = new IoTDeviceConfigurationModels();
                iotDeviceConfigurationModel.updateIoTDeviceConfiguration(id, iotDeviceConfiguration);
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
                IoTDeviceConfigurationModels iotDeviceConfigurationModel = new IoTDeviceConfigurationModels();
                iotDeviceConfigurationModel.deleteIoTDeviceConfiguration(id);
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
