using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace sfAPIService.Controllers
{
    public class RefCultureInfoController : ApiController
    {
        /// <summary>
        /// AllowAnonymous
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            RedisKey cacheKey = "cultureCodesJson";
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (cacheValue == null)
            {
                using (var ctx = new SFDatabaseEntities())
                {
                    var cultureCodes = ctx.RefCultureInfo
                        .Select(s => new RefCultureInfoModels()
                        {
                            CultureCode = s.CultureCode,
                            Name = s.Name
                        }).ToList<RefCultureInfoModels>();
                    RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(cultureCodes));
                    return Ok(cultureCodes);
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<List<RefCultureInfoModels>>(cacheValue));
            }            
        }

        /// <summary>
        /// AllowAnonymous
        /// </summary>
        [HttpGet]
        [Route("~/Heartbeat")]
        public IHttpActionResult Heartbeat()
        {
            return Ok("success");
        }
    }
}
