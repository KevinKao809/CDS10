using sfAPIService.Filter;
using sfAPIService.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/MetaDataDefination")]
    public class MetaDataDefinationController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}/Equipment")]
        public IHttpActionResult GetEquipmentMetaDataDefination(int companyId)
        {
            Models.MetaDataDefinationModels metaDataDefModel = new Models.MetaDataDefinationModels();
            return Ok(metaDataDefModel.GetEquipmentMetaDataDefination(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}/Factory")]
        public IHttpActionResult GetFactoryMetaDataDefination(int companyId)
        {
            Models.MetaDataDefinationModels metaDataDefModel = new Models.MetaDataDefinationModels();
            return Ok(metaDataDefModel.GetFactoryMetaDataDefination(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody] Models.MetaDataDefinationModels.Edit MetaDataDef)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(MetaDataDef);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || MetaDataDef == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                Models.MetaDataDefinationModels metaDataModel = new Models.MetaDataDefinationModels();
                int newMetaDataDefId = metaDataModel.addMetaDataDefination(MetaDataDef);
                return Json(new { id = newMetaDataDefId });
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
        public IHttpActionResult EditFormData(int id, [FromBody] Models.MetaDataDefinationModels.Edit MetaDataDef)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(MetaDataDef);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || MetaDataDef == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                Models.MetaDataDefinationModels metaDataDefModel = new Models.MetaDataDefinationModels();
                metaDataDefModel.updateMetaDataDefination(id, MetaDataDef);
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
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                Models.MetaDataDefinationModels metaDataDefModel = new Models.MetaDataDefinationModels();
                metaDataDefModel.deleteMetaDataDefination(id);
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
