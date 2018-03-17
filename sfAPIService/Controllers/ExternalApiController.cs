using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using sfAPIService.Filter;
using System.Security.Claims;
using System.Threading;
using System.Text;
using Swashbuckle.Swagger.Annotations;
using Microsoft.Azure.Devices;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "external")]
    [RoutePrefix("cdstudio")]
    public class ExternalApiController : ApiController
    {
        /// <summary>
        /// Roles : external
        /// </summary>        
        [HttpGet]
        [Route("Company")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(CompanyModels.Detail_readonly))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetCompanyById()
        {
            int companyId = GetCompanyIdFromToken();
            RedisKey cacheKey = "external_company_" + companyId;
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                CompanyModels companyModel = new CompanyModels();
                try
                {
                    CompanyModels.Detail_readonly company = companyModel.getCompanyByIdReadonly(Convert.ToInt32(companyId.ToString()));
                    RedisCacheHelper.SetKeyValue(cacheKey, JsonConvert.SerializeObject(company));
                    return Ok(company);
                }
                catch (Exception ex)
                {
                    StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                    string logAPI = "[Get] " + Request.RequestUri.ToString();
                    Startup._sfAppLogger.Error(logAPI + logMessage);

                    return InternalServerError();
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<Object>(cacheValue));
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Factory")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<FactoryModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllFactoryByCompanyId()
        {
            int companyId = GetCompanyIdFromToken();
            FactoryModels factoryModel = new FactoryModels();
            try
            {
                List<FactoryModels.Detail_readonly> factory = factoryModel.GetAllFactoryByCompanyIdReadonly(companyId);
                return Ok(factory);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(FactoryModels.Detail_readonly))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetFactoryById(int factoryId)
        {
            int companyId = GetCompanyIdFromToken();
            if (!General.IsFactoryUnderCompany(factoryId, companyId))
                return Unauthorized();

            FactoryModels factoryModel = new FactoryModels();
            try
            {
                FactoryModels.Detail_readonly factory = factoryModel.getFactoryByIdReadonly(factoryId, companyId);
                return Ok(factory);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}/IoTDevice")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTDeviceModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllIoTDeviceByFactoryId(int factoryId)
        {
            int companyId = GetCompanyIdFromToken();
            if (!General.IsFactoryUnderCompany(factoryId, companyId))
                return Unauthorized();

            try
            {
                IoTDeviceModels iotDeviceModel = new Models.IoTDeviceModels();
                return Ok(iotDeviceModel.GetAllIoTDeviceByFactoryIdReadonly(factoryId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}/Equipment")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentByFactoryId(int factoryId)
        {
            int companyId = GetCompanyIdFromToken();
            if (!General.IsFactoryUnderCompany(factoryId, companyId))
                return Unauthorized();

            try
            {
                EquipmentModels equipmentModel = new Models.EquipmentModels();
                return Ok(equipmentModel.GetAllEquipmentByFactoryIdReadonly(factoryId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }

        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}/EquipmentWithMetaData")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentWithMetaDataByFactoryId(int factoryId)
        {
            int companyId = GetCompanyIdFromToken();
            if (!General.IsFactoryUnderCompany(factoryId, companyId))
                return Unauthorized();

            try
            {
                EquipmentModels equipmentModel = new Models.EquipmentModels();
                return Ok(equipmentModel.GetAllEquipmentWithMetaDataByFactoryIdReadonly(factoryId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }

        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Factory/{factoryId}/AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByEquipmentId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsFactoryUnderCompany(factoryId, companyId))
                    return Unauthorized();

                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByFactoryId(factoryId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Factory/{factoryId}/Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByEquipmentId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsFactoryUnderCompany(factoryId, companyId))
                    return Unauthorized();

                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByFactoryId(factoryId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("IoTDevice")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<IoTDeviceModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllIoTDeviceByCompanyId()
        {
            int companyId = GetCompanyIdFromToken();
            try
            {
                IoTDeviceModels iotDeviceModel = new Models.IoTDeviceModels();
                return Ok(iotDeviceModel.GetAllIoTDeviceByCompanyIdReadonly(companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("IoTDevice/{iotDeviceId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(IoTDeviceModels.Detail_readonly))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetIoTDeviceById(string iotDeviceId)
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsIoTDeviceUnderCompany(iotDeviceId, companyId))
                    return Unauthorized();

                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                return Ok(iotDeviceModel.getIoTDeviceByIdReadonly(iotDeviceId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Equipment")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentByCompanyId()
        {
            int companyId = GetCompanyIdFromToken();            
            try
            {
                EquipmentModels equipmentModel = new Models.EquipmentModels();
                return Ok(equipmentModel.GetAllEquipmentByCompanyIdReadonly(companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("EquipmentWithMetaData")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(List<EquipmentModels.Detail_readonly>))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAllEquipmentWithMetaDataByCompanyId()
        {
            int companyId = GetCompanyIdFromToken();
            try
            {
                EquipmentModels equipmentModel = new Models.EquipmentModels();
                return Ok(equipmentModel.GetAllEquipmentWithMetaDataByCompanyId(companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Equipment/{equipmentId}")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EquipmentModels.Detail_readonly))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetEquipmentById(string equipmentId)
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                EquipmentModels equipmentModel = new EquipmentModels();
                return Ok(equipmentModel.getEquipmentByIdReadonly(equipmentId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpGet]
        [Route("Equipment/{equipmentId}/WithMetaData")]
        [SwaggerResponse(HttpStatusCode.OK, Type = typeof(EquipmentModels.Detail_readonly))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetEquipmentWithMetaDataById(string equipmentId)
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                EquipmentModels equipmentModel = new EquipmentModels();
                return Ok(equipmentModel.getEquipmentWithMetaDataById(equipmentId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Equipment/{equipmentId}/AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByEquipmentId(string equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByEquipmentId(equipmentId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Equipment/{equipmentId}/Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByEquipmentId(string equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsEquipmentUnderCompany(equipmentId, companyId))
                    return Unauthorized();

                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByEquipmentId(equipmentId, top, hours, order, companyId));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("AlarmMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetAlarmMessageByCompanyId([FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {            
            try
            {
                int companyId = GetCompanyIdFromToken();
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByCompanyId(companyId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }
        /// <summary>
        /// Roles : external
        /// </summary>
        /// <param name="top">default 10</param>
        /// <param name="hours">default 168</param>
        /// <param name="order">value = asc or desc, default desc</param>
        [HttpGet]
        [Route("Message")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public IHttpActionResult GetMessageByCompanyId([FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByCompanyId(companyId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : external
        /// </summary>
        [HttpPut]
        [Route("IoTDevice/{iotDeviceId}/C2DMessage")]
        [SwaggerResponse(HttpStatusCode.OK)]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        public async System.Threading.Tasks.Task<IHttpActionResult> SendC2DMessageAsync(string iotDeviceId)
        {
            try
            {
                int companyId = GetCompanyIdFromToken();
                if (!General.IsIoTDeviceUnderCompany(iotDeviceId, companyId))
                    return Unauthorized();

                string payload = await Request.Content.ReadAsStringAsync();

                //Retrieve IoTHub Connection String
                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                IoTHubModels iotHubModel = new IoTHubModels();
                string IoTHubAlias = iotDeviceModel.getIoTDeviceById(iotDeviceId).IoTHubAlias;                
                string IoTHubConnectionString = iotHubModel.getIoTHubById(IoTHubAlias).P_IoTHubConnectionString;

                //Send out Cloud to Device Message
                ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(IoTHubConnectionString);
                var commandMessage = new Message(Encoding.ASCII.GetBytes(payload));
                await serviceClient.SendAsync(iotDeviceId, commandMessage);
                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Put] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return InternalServerError(ex);
            }
        }

        private int GetCompanyIdFromToken()
        {
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var companyId = identity.Claims.Where(c => c.Type == "CompanyId")
                   .Select(c => c.Value).SingleOrDefault();
            return Convert.ToInt32(companyId);
        }        
    }
}
