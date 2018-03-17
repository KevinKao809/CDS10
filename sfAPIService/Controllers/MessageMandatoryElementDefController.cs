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
    [RoutePrefix("admin-api/MessageMandatoryElementDef")]
    public class MessageMandatoryElementDefController : ApiController
    {
        /// <summary>
        /// Roles : superadmin, admin
        /// </summary>
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
        [HttpGet]
        public IHttpActionResult GetAllMessageMandatoryElementDef()
        {
            MessageMandatoryElementDefModels mMEDModel = new Models.MessageMandatoryElementDefModels();
            return Ok(mMEDModel.GetAllMessageMandatoryElementDef());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllMessageMandatoryElementDefBySuperAdmin()
        {
            MessageMandatoryElementDefModels mMEDModel = new Models.MessageMandatoryElementDefModels();
            return Ok(mMEDModel.GetAllMessageMandatoryElementDefBySuperAdmin());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMessageMandatoryElementDefById(int id)
        {
            try
            {
                MessageMandatoryElementDefModels mMEDModel = new MessageMandatoryElementDefModels();
                return Ok(mMEDModel.getMessageMandatoryElementDefById(id));
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
        public IHttpActionResult AddFormData([FromBody]MessageMandatoryElementDefModels.Add IoTHub)
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
                MessageMandatoryElementDefModels mMEDModel = new MessageMandatoryElementDefModels();
                mMEDModel.addMessageMandatoryElementDef(IoTHub);
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
        public IHttpActionResult EditFormData(int id, [FromBody] MessageMandatoryElementDefModels.Update IoTHub)
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
                MessageMandatoryElementDefModels mMEDModel = new MessageMandatoryElementDefModels();
                mMEDModel.updateMessageMandatoryElementDef(id, IoTHub);
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
                MessageMandatoryElementDefModels mMEDModel = new MessageMandatoryElementDefModels();
                mMEDModel.deleteMessageMandatoryElementDef(id);
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
