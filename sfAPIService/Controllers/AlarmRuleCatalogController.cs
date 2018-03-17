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
    [RoutePrefix("admin-api/AlarmRuleCatalog")]
    public class AlarmRuleCatalogController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("company/{companyId}")]
        public IHttpActionResult GetAllAlarmRuleCatalogByCompanyId(int companyId)
        {
            AlarmRuleCatalogModels alarmRuleCatalogModel = new Models.AlarmRuleCatalogModels();
            return Ok(alarmRuleCatalogModel.GetAllAlarmRuleCatalogByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAlarmRuleCatalogById(int id)
        {
            AlarmRuleCatalogModels alarmRuleCatalogModel = new AlarmRuleCatalogModels();
            try
            {
                AlarmRuleCatalogModels.Detail company = alarmRuleCatalogModel.getAlarmRuleCatalogById(id);
                return Ok(company);
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
        public IHttpActionResult AddFormData([FromBody]AlarmRuleCatalogModels.Add alarmRuleCatalog)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(alarmRuleCatalog);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || alarmRuleCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AlarmRuleCatalogModels alarmRuleCatalogModel = new AlarmRuleCatalogModels();
                alarmRuleCatalogModel.addAlarmRuleCatalog(alarmRuleCatalog);
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] AlarmRuleCatalogModels.Update alarmRuleCatalog)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(alarmRuleCatalog);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || alarmRuleCatalog == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AlarmRuleCatalogModels alarmRuleCatalogModel = new AlarmRuleCatalogModels();
                alarmRuleCatalogModel.updateAlarmRuleCatalog(id, alarmRuleCatalog);
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
                AlarmRuleCatalogModels alarmRuleCatalogModel = new AlarmRuleCatalogModels();
                alarmRuleCatalogModel.deleteAlarmRuleCatalog(id);
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

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/Notification")]
        public IHttpActionResult GetAllNotification(int id)
        {
            AlarmNotificationModels alarmNotificationModel = new AlarmNotificationModels();
            return Ok(alarmNotificationModel.GetAllExternalApplicationByAlarmRuleCatalogId(id));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Notification")]
        public IHttpActionResult AttachNotification(int id, [FromBody] AlarmNotificationModels.Edit alarmNotification)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(alarmNotification);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AlarmNotificationModels alarmRuleCatalogModel = new AlarmNotificationModels();
                alarmRuleCatalogModel.AttachExternalApplication(id, alarmNotification);
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
        [HttpGet]
        [Route("{id}/Rules")]
        public IHttpActionResult GetAllRules(int id)
        {
            AlarmRuleItemModels alarmRuleItemModel = new AlarmRuleItemModels();
            return Ok(alarmRuleItemModel.GetAllAlarmRuleItemByAlarmRuleCatalogId(id));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Rules")]
        public IHttpActionResult UpdateAllRules(int id, [FromBody] AlarmRuleItemModels.Edit input)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(input);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || input == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AlarmRuleItemModels alarmRuleItemModel = new AlarmRuleItemModels();
                alarmRuleItemModel.UpdateAllAlarmRules(id, input);
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
    }
}
