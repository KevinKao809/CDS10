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
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/AlarmMessage")]
    public class AlarmMessageController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAlarmMessageByCompanyId(int companyId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
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
            //try
            //{
            //    DocDB_AlarmMessageModels alarmMessageModel = new DocDB_AlarmMessageModels(companyId);
            //    return Ok(alarmMessageModel.GetByCompanyId(companyId, top, hours, order));
            //}
            //catch (Exception ex)
            //{
            //    StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
            //    string logAPI = "[Get] " + Request.RequestUri.ToString();
            //    Startup._sfAppLogger.Error(logAPI + logMessage);

            //    return InternalServerError(ex);
            //}
        }
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Factory/{factoryId}")]
        public IHttpActionResult GetAlarmMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                FactoryModels factoryModel = new FactoryModels();
                int companyId = factoryModel.getFactoryById(factoryId).CompanyId;
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByFactoryId(factoryId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
            //try
            //{
            //    FactoryModels factoryModel = new FactoryModels();
            //    int companyId = factoryModel.getFactoryById(factoryId).CompanyId;
            //    DocDB_AlarmMessageModels alarmMessageModel = new DocDB_AlarmMessageModels(companyId);
            //    return Ok(alarmMessageModel.GetByFactoryId(factoryId, top, hours, order));
            //}
            //catch (Exception ex)
            //{
            //    StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
            //    string logAPI = "[Get] " + Request.RequestUri.ToString();
            //    Startup._sfAppLogger.Error(logAPI + logMessage);

            //    return InternalServerError(ex);
            //}
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("Equipment/{equipmentId}")]
        public IHttpActionResult GetAlarmMessageByEquipmentId(string equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                int companyId = equipmentModel.getCompanyId(equipmentId);
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetAlarmMessageByEquipmentId(equipmentId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
            /*
            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                int companyId = equipmentModel.getCompanyId(equipmentId);
                DocDB_AlarmMessageModels alarmMessageModel = new DocDB_AlarmMessageModels(companyId);
                return Ok(alarmMessageModel.GetByEquipmentId(equipmentId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }*/
        }
    }
}
