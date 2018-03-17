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
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/Dashboard")]
    public class DashboardController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllByCompanyId(int companyId, string type = null)
        {
            DashboardModels dashboardModel = new Models.DashboardModels();
            return Ok(dashboardModel.getAllDashboardByCompnayId(companyId, type));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}")]
        public IHttpActionResult GetAllByFactoryId(int factoryId)
        {
            DashboardModels dashboardModel = new Models.DashboardModels();
            return Ok(dashboardModel.getAllDashboardByFactoryId(factoryId));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}/EquipmentClass")]
        public IHttpActionResult GetAll(int companyId)
        {
            DashboardModels dashboardModel = new Models.DashboardModels();
            return Ok(dashboardModel.getAllEquipmentClassDashboardByCompnayId(companyId));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult Add([FromBody]DashboardModels.Add dashboard)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(dashboard);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || dashboard == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DashboardModels dashboardModel = new DashboardModels();
                int newDashboardId = dashboardModel.addDashboard(dashboard);
                return Json(new { id = newDashboardId });
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
                DashboardModels dashboardModel = new DashboardModels();
                dashboardModel.deleteDashboard(id);
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
