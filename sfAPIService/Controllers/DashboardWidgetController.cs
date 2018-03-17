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
    [RoutePrefix("admin-api/DashboardWidget")]
    public class DashboardWidgetController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("Dashboard/{dashboardId}")]
        public IHttpActionResult GetAllByDashboardId(int dashboardId)
        {
            DashboardWidgetModels dashboardWidgetModel = new Models.DashboardWidgetModels();
            return Ok(dashboardWidgetModel.getAllDashboardWidgetByDashboardId(dashboardId));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GeById(int id)
        {
            DashboardWidgetModels dashboardWidgetModel = new DashboardWidgetModels();
            try
            {
                DashboardWidgetModels.Detail dashboardWidget = dashboardWidgetModel.getDashboardWidgetById(id);
                return Ok(dashboardWidget);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]DashboardWidgetModels.Add dashboardWidget)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(dashboardWidget);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dashboardWidget == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DashboardWidgetModels dashboardWidgetModel = new DashboardWidgetModels();
                int newDashboardWidgetId = dashboardWidgetModel.addDashboardWidget(dashboardWidget);
                return Json(new { id = newDashboardWidgetId });
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData([FromBody] DashboardWidgetModels.Update dashboardWidget)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(dashboardWidget);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dashboardWidget == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DashboardWidgetModels dashboardWidgetModel = new DashboardWidgetModels();
                dashboardWidgetModel.updateDashboardWidget(dashboardWidget);
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                DashboardWidgetModels dashboardWidgetModel = new DashboardWidgetModels();
                dashboardWidgetModel.deleteDashboardWidget(id);
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
