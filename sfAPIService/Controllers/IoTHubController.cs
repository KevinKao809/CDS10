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
    [RoutePrefix("admin-api/IoTHub")]
    public class IoTHubController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            IoTHubModels iotHubModel = new Models.IoTHubModels();
            return Ok(iotHubModel.GetAllIoTHub());
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllByCompanyId(int companyId)
        {
            IoTHubModels iotHubModel = new Models.IoTHubModels();
            return Ok(iotHubModel.GetAllIoTHubByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetById(string id)
        {
            IoTHubModels iotHubModel = new IoTHubModels();
            try
            {
                IoTHubModels.Detail company = iotHubModel.getIoTHubById(id);
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
        public IHttpActionResult AddFormData([FromBody]IoTHubModels.Edit IoTHub)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(IoTHub);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || IoTHub == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTHubModels iotHubModel = new IoTHubModels();
                iotHubModel.addIoTHub(IoTHub);
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
        public IHttpActionResult EditFormData(string id, [FromBody] IoTHubModels.Edit IoTHub)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(IoTHub);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || IoTHub == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTHubModels iotHubModel = new IoTHubModels();
                iotHubModel.updateIoTHub(id, IoTHub);
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
        public IHttpActionResult Delete(string id)
        {
            try
            {
                IoTHubModels iotHubModel = new IoTHubModels();
                iotHubModel.deleteIoTHub(id);
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
