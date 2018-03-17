using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using sfAPIService.Models;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [RoutePrefix("admin-api/UsageLog")]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    public class UsageLogController : ApiController
    {
        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllUsageLog([FromUri]int days = 1, [FromUri]string order = "asc")
        {
            UsagLogModels usageLogModel = new UsagLogModels();
            return Ok(usageLogModel.getAll(days, order));
        }        
    }
}
