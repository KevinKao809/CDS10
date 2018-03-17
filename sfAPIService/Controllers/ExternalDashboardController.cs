using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;

using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Script.Serialization;

namespace sfAPIService.Controllers
{
    [Authorize]
    [RoutePrefix("admin-api/ExternalDashboard")]
    public class ExternalDashboardController : ApiController
    {        
        [HttpGet]
        [Route("{id}")]
        public IHttpActionResult GetById(int id)
        {
            ExternalDashboardModels model = new Models.ExternalDashboardModels();
            return Ok(model.GetAllExternalDashboardById(id));
        }

        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllByCompanyId(int companyId)
        {
            ExternalDashboardModels extDashboardModel = new Models.ExternalDashboardModels();
            return Ok(extDashboardModel.GetAllExternalDashboardByCompanyId(companyId));
        }

        [HttpPost]
        public IHttpActionResult Add([FromBody]ExternalDashboardModels.Edit externalDashboard)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(externalDashboard);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || externalDashboard == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                ExternalDashboardModels model = new ExternalDashboardModels();
                model.addExternalDashboard(externalDashboard);
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

        [HttpPut]
        [Route("{id}")]
        public IHttpActionResult EditFormData(int id, [FromBody]ExternalDashboardModels.Edit externalDashboard)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(externalDashboard);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || externalDashboard == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                ExternalDashboardModels model = new ExternalDashboardModels();
                model.updateExternalDashboard(id, externalDashboard);
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

        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                ExternalDashboardModels model = new ExternalDashboardModels();
                model.deleteExternalDashboard(id);
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
