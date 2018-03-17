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
    [RoutePrefix("admin-api/MessageCatalog")]
    public class MessageCatalogController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllMessageCatalogByCompanyId(int companyId)
        {
            MessageCatalogModels messageCatalogModel = new Models.MessageCatalogModels();
            return Ok(messageCatalogModel.GetAllMessageCatalogByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}/NonChild")]
        public IHttpActionResult GetAllChildMessageCatalogByCompanyId(int companyId)
        {
            MessageCatalogModels messageCatalogModel = new Models.MessageCatalogModels();
            return Ok(messageCatalogModel.GetAllNonChildMessageCatalogByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{id}/Elements")]
        public IHttpActionResult GetAllMessageSchemaByDeviceId(int id)
        {
            MessageCatalogModels msgCatalogModels = new MessageCatalogModels();
            return Ok(msgCatalogModels.GetMessageCatalogElements(id));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetMessageCatalogById(int id)
        {
            MessageCatalogModels messageCatalogModel = new MessageCatalogModels();
            try
            {
                MessageCatalogModels.Detail MessageCatalog = messageCatalogModel.getMessageCatalogById(id);
                return Ok(MessageCatalog);
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
        public IHttpActionResult AddFormData([FromBody]MessageCatalogModels.Edit messageCatalog)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(messageCatalog);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || messageCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                MessageCatalogModels messageCatalogModel = new MessageCatalogModels();
                messageCatalogModel.addMessageCatalog(messageCatalog);
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
        public IHttpActionResult EditFormData(int id, [FromBody] MessageCatalogModels.Edit messageCatalog)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(messageCatalog);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || messageCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                MessageCatalogModels messageCatalogModel = new MessageCatalogModels();
                messageCatalogModel.updateMessageCatalog(id, messageCatalog);
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
                MessageCatalogModels messageCatalogModel = new MessageCatalogModels();
                messageCatalogModel.deleteMessageCatalog(id);
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
