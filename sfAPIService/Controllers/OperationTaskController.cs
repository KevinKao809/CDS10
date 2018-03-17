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
    [RoutePrefix("admin-api/OperationTask")]
    public class OperationTaskController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Search")]
        public IHttpActionResult SearchInPastSevenDaysOperations([FromUri] OperationTaskModels.SearchCondition searchCondition)
        {
            try
            {
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                return Ok(operationTaskModel.searchInPastSevenDaysOperations(searchCondition));
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}/Search")]
        public IHttpActionResult GetAllOperationTasksByCompanyId(int companyId, [FromUri] OperationTaskModels.SearchCondition searchCondition)
        {
            try
            {
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                return Ok(operationTaskModel.searchInPastSevenDaysOperations(searchCondition, companyId));
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllOperationTasksByCompanyId(int companyId)
        {
            OperationTaskModels operationTaskModel = new Models.OperationTaskModels();
            return Ok(operationTaskModel.GetAllOperationTaskByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetOperationTaskById(int id)
        {
            try
            {
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                OperationTaskModels.Detail detail = operationTaskModel.getOperationTaskById(id);
                return Ok(operationTaskModel.getOperationTaskById(id));
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
        public IHttpActionResult AddFormData([FromBody]OperationTaskModels.Add operationTask)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(operationTask);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || operationTask == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                int newOperationTaskId = operationTaskModel.addOperationTask(operationTask);
                return Json(new { id = newOperationTaskId });
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
        public IHttpActionResult EditFormData(int id, [FromBody] OperationTaskModels.Update operationTask)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(operationTask);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || operationTask == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                operationTaskModel.updateOperationTask(id, operationTask);
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
                OperationTaskModels operationTaskModel = new OperationTaskModels();
                operationTaskModel.deleteOperationTask(id);
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
