using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;
using sfAPIService.Models;
using System.Threading.Tasks;

using System.Web.Script.Serialization;
using sfAPIService.Filter;
using static sfAPIService.Models.EquipmentModels;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/Equipment")]
    public class EquipmentController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAll()
        {
            EquipmentModels equipmentModel = new Models.EquipmentModels();
            return Ok(equipmentModel.GetAllEquipment());
        }       

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllByCompanyId(int companyId)
        {
            EquipmentModels equipmentModel = new Models.EquipmentModels();
            return Ok(equipmentModel.GetAllEquipmentByCompanyId(companyId));
        }

        [HttpGet]
        [Route("Company/{companyId}/MetaData")]
        public IHttpActionResult GetAllWithMetaDataByCompanyId(int companyId)
        {
            EquipmentModels equipmentModel = new Models.EquipmentModels();
            return Ok(equipmentModel.GetAllEquipmentWithMetaDataByCompanyId(companyId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}")]
        public IHttpActionResult GetAllByFactoryId(int factoryId)
        {
            EquipmentModels equipmentModel = new Models.EquipmentModels();
            return Ok(equipmentModel.GetAllEquipmentByFactoryId(factoryId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Factory/{factoryId}/MetaData")]
        public IHttpActionResult GetAllWithMetaDataByFactoryId(int factoryId)
        {
            EquipmentModels equipmentModel = new Models.EquipmentModels();
            return Ok(equipmentModel.GetAllEquipmentWithMetaDataByFactoryId(factoryId));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{Id}/MetaData")]
        public IHttpActionResult GetMetaDataById(int Id)
        {
            EquipmentModels equipmentModel = new EquipmentModels();
            try
            {
                return Ok(equipmentModel.GetMetaDataById(Id));
            }
            catch
            {                
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{Id}/MetaData")]
        public async Task<IHttpActionResult> UpdateMetaDataByIdAsync(int Id, [FromBody] MetaData_Edit inMetaData)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(inMetaData);
            string logAPI = "[Post] " + Request.RequestUri.ToString();
           
            if (!ModelState.IsValid || inMetaData == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }
            
            try
            {
                EquipmentModels equipmentModel = new Models.EquipmentModels();
                equipmentModel.UpdateMetaDataById(Id, inMetaData);
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
        [HttpGet]
        public IHttpActionResult GeById(int id)
        {
            EquipmentModels equipmentModel = new EquipmentModels();
            try
            {
                EquipmentModels.Detail company = equipmentModel.getEquipmentById(id);
                return Ok(company);
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("{Id}/WithMetaData")]
        public IHttpActionResult GetWithMetaDataById(int Id)
        {
            EquipmentModels equipmentModel = new EquipmentModels();
            try
            {
                return Ok(equipmentModel.getEquipmentWithMetaDataById(Id));
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
        public IHttpActionResult AddFormData([FromBody]EquipmentModels.Edit Equipment)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(Equipment);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || Equipment == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                int newEquipmentId = equipmentModel.addEquipment(Equipment);
                return Json(new { id = newEquipmentId});
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
        public IHttpActionResult EditFormData(int id, [FromBody] EquipmentModels.Edit Equipment)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(Equipment);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || Equipment == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                equipmentModel.updateEquipment(id, Equipment);
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
                EquipmentModels equipmentModel = new EquipmentModels();
                equipmentModel.deleteEquipment(id);
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
        [Route("{id}/Image")]
        public async Task<HttpResponseMessage> UploadLogoFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            EquipmentModels equipmentModel = new EquipmentModels();
            FileUtility fileHelper = new FileUtility();
            string root = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                EquipmentModels.Detail equipment = equipmentModel.getEquipmentById(id);
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            try
            {
                // Read the form data.
                string fileAbsoluteUri = "";
                await Request.Content.ReadAsMultipartAsync(provider);

                //FileData
                foreach (MultipartFileData fileData in provider.FileData)
                {
                    string formColumnName = fileData.Headers.ContentDisposition.Name;
                    string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Split('.')[1];
                    if (fileHelper.CheckImageExtensionName(formColumnName, fileExtenionName))
                    {
                        string uploadFilePath = "company-" + equipmentModel.getCompanyId(id) + "/equipment/" + id + "-default." + fileHelper.LowerAndFilterString(fileExtenionName);
                        fileAbsoluteUri = fileHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath, "images");
                    }
                }

                if (fileAbsoluteUri.Equals(""))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty or wrong extension name");

                //Edit company logo path
                equipmentModel.updateEquipmentLogoURL(id, fileAbsoluteUri);
                return Request.CreateResponse(HttpStatusCode.OK, new { imageURL = fileAbsoluteUri });
            }
            catch (System.Exception ex)
            {
                string logAPI = "[Put] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [Route("{equipmentId}/Message")]
        public IHttpActionResult GetMessageByEquipmentId(string equipmentId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                EquipmentModels equipmentModel = new EquipmentModels();
                int companyId = equipmentModel.getCompanyId(equipmentId);
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByEquipmentId(equipmentId, top, hours, order));
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }
        }
    }
}
