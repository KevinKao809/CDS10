using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using sfShareLib;

namespace sfAPIService.Controllers
{
    public class TestController : ApiController
    {
        [HttpGet]
        public IHttpActionResult test(int alarmRuleCatalogId)
        {
            DBHelper._AlarmRuleItem model = new DBHelper._AlarmRuleItem();
            return Ok(model.GetAllByAlarmRuleCatalogIdForRuleEngine(alarmRuleCatalogId));
        }
    }
}
