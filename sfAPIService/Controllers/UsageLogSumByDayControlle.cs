using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Text;

using sfShareLib;
using sfAPIService.Models;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/UsageLogSumByDay")]
    public class UsageLogSumByDayController : ApiController
    {
        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllData([FromUri]int days = 1, [FromUri]string order = "asc")
        {
            UsageLogSumByDayModels usageLogModel = new UsageLogSumByDayModels();
            return Ok(usageLogModel.getAll(days, order));
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("Last")]
        public IHttpActionResult GetLastData()
        {
            try
            {
                UsageLogSumByDayModels usageLogModel = new UsageLogSumByDayModels();
                return Ok(usageLogModel.getLast());
            }
            catch (Exception ex)
            {
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return NotFound();
            }
        }
    }
}
