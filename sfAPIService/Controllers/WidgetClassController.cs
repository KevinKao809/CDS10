using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;
using System.Net;
using System.Net.Http;

using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Script.Serialization;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    public class WidgetClassController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("admin-api/WidgetClass")]
        public IHttpActionResult GetAll([FromUri]string level = null)
        {
            WidgetClassModels widgetClassModel = new Models.WidgetClassModels();
            if (string.IsNullOrEmpty(level))
                return Ok(widgetClassModel.getAllwidgetClasses());
            else
                return Ok(widgetClassModel.getAllWidgetClassesByLevel(level));
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("admin-api/WidgetClass/{id}")]
        public IHttpActionResult GeById(int id)
        {
            WidgetClassModels widgetClassModel = new WidgetClassModels();
            try
            {
                WidgetClassModels.Detail widgetClass = widgetClassModel.getWidgetClassById(id);
                return Ok(widgetClass);
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
        [Route("admin-api/WidgetClass")]
        public IHttpActionResult AddFormData([FromBody]WidgetClassModels.Add widgetClass)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(widgetClass);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || widgetClass == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                WidgetClassModels widgetClassModel = new WidgetClassModels();
                int newWidgetClassId = widgetClassModel.addWidgetClass(widgetClass);
                return Json(new { id = newWidgetClassId });
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
        [Route("admin-api/WidgetClass/{id}")]
        public IHttpActionResult EditFormData(int id, [FromBody] WidgetClassModels.Update widgetClass)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(widgetClass);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || widgetClass == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                WidgetClassModels widgetClassModel = new WidgetClassModels();
                widgetClassModel.updateWidgetClass(id, widgetClass);
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
        [Route("admin-api/WidgetClass/{id}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                WidgetClassModels widgetClassModel = new WidgetClassModels();
                widgetClassModel.deleteWidgetClass(id);
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

        /*
        [HttpPut]
        [Route("admin-api/WidgetClass/{id}/Image")]
        public async Task<HttpResponseMessage> UploadWidgetClassImageFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            WidgetClassModels widgetClassModel = new WidgetClassModels();
            WidgetClassModels.Detail existWidgetClass;
            FileUtility fileHelper = new FileUtility();
            string root = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                existWidgetClass = widgetClassModel.getWidgetClassById(id);
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
                        string uploadFilePath = "WidgetClass/" + existWidgetClass.Name + "." + fileHelper.LowerAndFilterString(fileExtenionName);
                        fileAbsoluteUri = fileHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath, "images");
                    }
                }

                if (fileAbsoluteUri.Equals(""))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty or wrong extension name");

                //Edit company logo path
                widgetClassModel.updateWidgetClassLogoURL(id, fileAbsoluteUri);
                return Request.CreateResponse(HttpStatusCode.OK, new { imageURL = fileAbsoluteUri });
            }
            catch (System.Exception ex)
            {
                string logAPI = "[Put] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }            
        }*/
    }
}