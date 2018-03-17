using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Dynamic;

using Newtonsoft.Json.Linq;
using System.Text;
using sfShareLib;
using sfAPIService.Models;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/IoTDevice")]
    public class IoTDeviceController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllIoTDeciceByCompanyId(int companyId)
        {
            IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
            return Ok(iotDeviceModel.GetAllIoTDeviceByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}")]
        public IHttpActionResult GetAllIoTDeciceByFactoryId(int factoryId)
        {
            IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
            return Ok(iotDeviceModel.GetAllIoTDeviceByFactoryId(factoryId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetIoTDeciceById(string id)
        {
            IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
            try
            {
                return Ok(iotDeviceModel.getIoTDeviceById(id));
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
        public IHttpActionResult AddFormData([FromBody]IoTDeviceModels.Add iotDevice)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(iotDevice);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || iotDevice == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                iotDeviceModel.addIoTDevice(iotDevice);
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
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(string id, [FromBody] IoTDeviceModels.Update iotDevice)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(iotDevice);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || iotDevice == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                iotDeviceModel.updateIoTDevice(id, iotDevice);
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
        [HttpPut]
        [Route("{id}/ResetPassword")]
        public IHttpActionResult ResetPassword(string id, [FromBody] ResetPasswordModels model)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(model);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || model == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AccountModels accountModel = new AccountModels();
                accountModel.ResetIoTDevicePassword(id, model.NewPassword);
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
        public IHttpActionResult Delete(string id)
        {
            try
            {
                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                iotDeviceModel.deleteIoTDevice(id);
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
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{deviceId}/ChangePassword")]
        public IHttpActionResult ChangePassword(string deviceId, [FromBody] AccountModels.PasswordSet passwordSet)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(passwordSet);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || passwordSet == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                AccountModels accountModel = new AccountModels();
                accountModel.ChangeIoTDevicePassword(deviceId, passwordSet);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                switch (ex.Message)
                {
                    case "404":
                        return NotFound();
                    case "401":
                        return Unauthorized();
                }
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{deviceId}/Configuration")]
        public IHttpActionResult GetAllConfigurationByDeviceId(string deviceId)
        {
            IoTDeviceConfigurationValueModels iotDCVModel = new IoTDeviceConfigurationValueModels();

            //List<IoTDeviceConfigurationValueModels.Detail> configValues = new List<IoTDeviceConfigurationValueModels.Detail>();
            //configValues.AddRange(iotDCVModel.GetAllSystemConfiguration(deviceId));
            //configValues.AddRange(iotDCVModel.GetAllCustomizedConfiguration(deviceId));            

            //return Ok(configValues);
            IoTDeviceConfigurationValueModels model = new IoTDeviceConfigurationValueModels();

            return Ok(model.GetAll(deviceId));
        }

        /*
        [HttpPut]
        [Route("{deviceId}/Configuration/System")]
        public IHttpActionResult EditAllConfiguration(string deviceId, IoTDeviceSystemConfigurationValueModel.Edit configurationList)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(configurationList);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                IoTDeviceSystemConfigurationValueModel iotDCVModel = new IoTDeviceSystemConfigurationValueModel();
                iotDCVModel.UpdateAllConfiguration(deviceId, configurationList);
                DBHelper._IoTDevice dbHelpler = new DBHelper._IoTDevice();
                dbHelpler.UpdateDeviceConfigurationStatusAndProperty(deviceId, 0);
                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{deviceId}/Configuration/Customized")]
        public IHttpActionResult AddCustomizedConfiguration(string deviceId, [FromBody] IoTDeviceCustomizedConfigurationValueModel.Edit configuration)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(configuration);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                IoTDeviceCustomizedConfigurationValueModel iotDCCVModel = new IoTDeviceCustomizedConfigurationValueModel();
                iotDCCVModel.RefreshAllByIoTDeviceId(deviceId, configuration);

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }
        */
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{deviceId}/DesiredProperty")]
        public IHttpActionResult UpdateDesiredProperty(string deviceId, [FromBody] IoTDeviceModels.Update_Desired desired)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(desired);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                JObject desiredProperty = JObject.Parse(desired.DeviceTwinsDesired);
                IoTDeviceModels iotDeviceModel = new IoTDeviceModels();
                iotDeviceModel.updateIoTDeviceDesired(deviceId, desiredProperty);

                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{deviceId}/Message")]
        public IHttpActionResult GetAllMessageByDeviceId(string deviceId)
        {
            IoTDeviceMessageCatalogModels iotDMCModels = new IoTDeviceMessageCatalogModels();
            return Ok(iotDMCModels.GetAllMessageCatalogByIoTDeviceId(deviceId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{deviceId}/Message")]
        public IHttpActionResult AttachMessage(string deviceId, IoTDeviceMessageCatalogModels.Edit iotDMC)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(iotDMC);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid)
                return BadRequest();

            try
            {
                IoTDeviceMessageCatalogModels iotDMCModel = new IoTDeviceMessageCatalogModels();
                iotDMCModel.AttachMessage(deviceId, iotDMC);
                return Ok();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{deviceId}/MessageTemplate")]
        public IHttpActionResult GetAllMessageTemplateByDeviceId(string deviceId)
        {
            IoTDeviceModels iotDeviceModels = new IoTDeviceModels();
            IoTDeviceMessageCatalogModels iotDMCModels = new IoTDeviceMessageCatalogModels();
            MessageCatalogModels msgCatalogModels = new MessageCatalogModels();
            
            List<object> objectList = new List<object>();
            var msgCatalogs = iotDMCModels.GetAllMessageCatalogByIoTDeviceId(deviceId);
            foreach (var msgCatalog in msgCatalogs)
            {
                objectList.Add(msgCatalogModels.GetMessageCatalogTemplate(msgCatalog.MessageCatalogId, deviceId));
            }
            return Ok(objectList);
        }
    }
}
