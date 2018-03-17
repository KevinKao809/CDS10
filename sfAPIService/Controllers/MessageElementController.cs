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
    [RoutePrefix("admin-api/MessageElement")]
    public class MessageElementController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("MessageCatalog/{messageCatalogId}")]
        public IHttpActionResult GetAllCompanies(int messageCatalogId)
        {
            MessageElementModels messageElementModel = new Models.MessageElementModels();
            return Ok(messageElementModel.GetAllMessageElementByMessageCatalogId(messageCatalogId));
        }
        /*
        [HttpGet]
        public IHttpActionResult GetCompanyById(int id)
        {
            MessageElementModels messageElementModel = new MessageElementModels();
            try
            {
                MessageElementModels.Detail company = messageElementModel.ge(id);
                return Ok(company);
            }
            catch
            {
                return NotFound();
            }
        }
        */
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]MessageElementModels.Add messageElement)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(messageElement);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || messageElement == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                MessageElementModels messageElementModel = new MessageElementModels();
                messageElementModel.addMessageElement(messageElement);
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
        public IHttpActionResult EditFormData(int id, [FromBody] MessageElementModels.Update messageElement)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(messageElement);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || messageElement == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                MessageElementModels messageElementModel = new MessageElementModels();
                messageElementModel.updateMessageElement(id, messageElement);
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
                MessageElementModels messageElementModel = new MessageElementModels();
                messageElementModel.deleteMessageElement(id);
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
