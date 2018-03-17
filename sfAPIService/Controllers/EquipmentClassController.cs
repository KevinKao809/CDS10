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
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "superadmin, admin")]
    [RoutePrefix("admin-api/EquipmentClass")]
    public class EquipmentClassController : ApiController
    {
        /// <summary>
        /// Roles : AllowAnonymous
        /// </summary>
        [AllowAnonymous]
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllEquipmentClassByCompanyId(int companyId)
        {
            using (var ctx = new SFDatabaseEntities())
            {
                var equipClasses = ctx.EquipmentClass
                    .Where(s => s.CompanyId == companyId && s.DeletedFlag == false)
                    .Select(s => new EquipmentClassModels.Detail()
                    {
                        Id = s.Id,
                        CompanyId = s.CompanyId,
                        CompanyName = s.Company.Name,
                        Name = s.Name,
                        Description = s.Description                        
                    }).ToList<EquipmentClassModels.Detail>();
                return Ok(equipClasses);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAllEquipmentClass()
        {
            EquipmentClassModels equipmentClassModel = new Models.EquipmentClassModels();
            return Ok(equipmentClassModel.GetAllEquipmentClass());
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllEquipmentClassBySuperadmin()
        {
            EquipmentClassModels equipmentClassModel = new Models.EquipmentClassModels();
            return Ok(equipmentClassModel.GetAllEquipmentClassBySuperAdmin());
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetEquipmentClassById(int id)
        {
            try
            {
                EquipmentClassModels equipmentClassModel = new EquipmentClassModels();
                return Ok(equipmentClassModel.getEquipmentClassById(id));
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
        public IHttpActionResult AddFormData([FromBody]EquipmentClassModels.Add IoTHub)
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
                EquipmentClassModels equipmentClassModel = new EquipmentClassModels();
                equipmentClassModel.addEquipmentClass(IoTHub);
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
        public IHttpActionResult EditFormData(int id, [FromBody] EquipmentClassModels.Update IoTHub)
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
                EquipmentClassModels equipmentClassModel = new EquipmentClassModels();
                equipmentClassModel.updateEquipmentClass(id, IoTHub);
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
                EquipmentClassModels equipmentClassModel = new EquipmentClassModels();
                equipmentClassModel.deleteEquipmentClass(id);
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

