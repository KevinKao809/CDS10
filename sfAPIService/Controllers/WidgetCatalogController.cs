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
    [RoutePrefix("admin-api/WidgetCatalog")]
    public class WidgetCatalogController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllByCompanyId(int companyId, [FromUri]string level = null)
        {
            WidgetCatalogModels widgetCatalogModel = new Models.WidgetCatalogModels();
            return Ok(widgetCatalogModel.getAllWidgetCatalogByCompanyId(companyId, level));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GeById(int id)
        {
            WidgetCatalogModels widgetCatalogModel = new WidgetCatalogModels();
            try
            {
                WidgetCatalogModels.Detail widgetCatalog = widgetCatalogModel.getWidgetCatalogById(id);
                return Ok(widgetCatalog);
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
        public IHttpActionResult AddFormData([FromBody]WidgetCatalogModels.Add widgetCatalog)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(widgetCatalog);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || widgetCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                WidgetCatalogModels widgetCatalogModel = new WidgetCatalogModels();
                int newwidgetCatalogId = widgetCatalogModel.addWidgetCatalog(widgetCatalog);
                return Json(new { id = newwidgetCatalogId });
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
        public IHttpActionResult EditFormData(int id, [FromBody] WidgetCatalogModels.Update widgetCatalog)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(widgetCatalog);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || widgetCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                WidgetCatalogModels widgetCatalogModel = new WidgetCatalogModels();
                widgetCatalogModel.updateWidgetCatalog(id, widgetCatalog);
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
                WidgetCatalogModels widgetCatalogModel = new WidgetCatalogModels();
                widgetCatalogModel.deleteWidgetCatalog(id);
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
