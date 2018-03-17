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
using StackExchange.Redis;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin")]
    [RoutePrefix("admin-api/DeviceType")]
    public class DeviceTypeController : ApiController
    {
        RedisKey cacheKey = "deviceType";

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
        [HttpGet]
        public IHttpActionResult GetAllDeviceTypes()
        {            
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);
            if (string.IsNullOrEmpty(cacheValue) || cacheValue.Length < 10)
            {
                DeviceTypeModels deviceTypeModel = new Models.DeviceTypeModels();
                List<DeviceTypeModels.Detail> deviceTypeList = deviceTypeModel.GetAllDeviceType();
                RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(deviceTypeList));
                return Ok(deviceTypeList);
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<Object>(cacheValue));
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllDeviceTypesBySuperAdmin()
        {
            DeviceTypeModels deviceTypeModel = new Models.DeviceTypeModels();
            return Ok(deviceTypeModel.GetAllDeviceTypeBySuperAdmin());
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetDeviceTypeById(int id)
        {
            try
            {
                DeviceTypeModels deviceTypeModel = new DeviceTypeModels();
                return Ok(deviceTypeModel.getDeviceTypeById(id));
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]DeviceTypeModels.Add IoTHub)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(IoTHub);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || IoTHub == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DeviceTypeModels deviceTypeModel = new DeviceTypeModels();
                deviceTypeModel.addDeviceType(IoTHub);
                RedisCacheHelper._RedisCache.KeyDelete(cacheKey, CommandFlags.FireAndForget);
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
        /// Roles : superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] DeviceTypeModels.Update IoTHub)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(IoTHub);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || IoTHub == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                DeviceTypeModels deviceTypeModel = new DeviceTypeModels();
                deviceTypeModel.updateDeviceType(id, IoTHub);
                RedisCacheHelper._RedisCache.KeyDelete(cacheKey, CommandFlags.FireAndForget);
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
        /// Roles : superadmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                DeviceTypeModels deviceTypeModel = new DeviceTypeModels();
                deviceTypeModel.deleteDeviceType(id);
                RedisCacheHelper._RedisCache.KeyDelete(cacheKey, CommandFlags.FireAndForget);
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