using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Text;
using sfShareLib;

using System.Threading.Tasks;
using sfAPIService.Models;
using System.Web.Script.Serialization;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/Factory")]
    public class FactoryController : ApiController
    {
        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpGet]
        [Route("Company/{companyId}")]
        public IHttpActionResult GetAllFactoryByCompanyId(int companyId)
        {
            FactoryModels factoryModel = new FactoryModels();
            try
            {
                List<FactoryModels.Detail> factoryList = factoryModel.GetAllFactoryByCompanyId(companyId);
                return Ok(factoryList);
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
        [Route("{id}")]
        public IHttpActionResult GetFactoryById(int id)
        {
            FactoryModels factoryModel = new FactoryModels();
            try
            {
                FactoryModels.Detail factory = factoryModel.getFactoryById(id);
                return Ok(factory);
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
        public IHttpActionResult AddFactoryFormData([FromBody] FactoryModels.Edit factory)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(factory);
            string logAPI = "[Post] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || factory == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            var newFactory = new Factory()
            {
                Name = factory.Name,
                Description = factory.Description,
                CompanyId = factory.CompanyId,
                Latitude = (float)factory.Latitude,
                Longitude = (float)factory.Longitude,
                CultureInfo = factory.CultureInfoId,
                TimeZone = factory.TimeZone,
                CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                DeletedFlag = false
            };

            using (var ctx = new SFDatabaseEntities())
            {
                ctx.Factory.Add(newFactory);
                try
                {
                    ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                    logMessage.AppendLine(logForm);
                    Startup._sfAppLogger.Error(logAPI + logMessage);

                    return InternalServerError(ex);
                }
            }
            return Json(new { id = newFactory.Id });
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{factoryId}")]
        public IHttpActionResult EditFactoryFormData(int factoryId, [FromBody] FactoryModels.Edit factory)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid data");
            using (var ctx = new SFDatabaseEntities())
            {
                var existingFactory = ctx.Factory
                    .Where(s => s.Id == factoryId && s.DeletedFlag == false)
                    .FirstOrDefault();
                if (existingFactory != null)
                {
                    existingFactory.Name = factory.Name;
                    existingFactory.Description = factory.Description;
                    existingFactory.TimeZone = factory.TimeZone;
                    existingFactory.Latitude = factory.Latitude;
                    existingFactory.Longitude = factory.Longitude;
                    existingFactory.CultureInfo = factory.CultureInfoId;
                    existingFactory.UpdatedAt = DateTime.Parse(DateTime.Now.ToString());

                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch
                    {
                        return InternalServerError();
                    }
                }
                else
                {
                    return NotFound();
                }

            }
            return Ok("Success");
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpPut]
        [Route("{factoryId}/Image")]
        public async Task<HttpResponseMessage> UploadFactoryPhotoFile(int factoryId)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            using (var ctx = new SFDatabaseEntities())
            {
                var existingFactory = ctx.Factory
                    .Where(s => s.Id == factoryId && s.DeletedFlag == false)
                    .FirstOrDefault();

                if (existingFactory == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound);

                string root = Path.GetTempPath();
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    // Read the form data.
                    string fileAbsoluteUri = "";
                    await Request.Content.ReadAsMultipartAsync(provider);
                    char[] trimChar = { '\"' };

                    //FileData
                    foreach (MultipartFileData fileData in provider.FileData)
                    {
                        string formColumnName = fileData.Headers.ContentDisposition.Name.ToLower().Trim(trimChar);
                        string fileExtenionName = fileData.Headers.ContentDisposition.FileName.Split('.')[1].ToLower().Trim(trimChar);
                        if (formColumnName.Equals("image"))
                        {
                            if (fileExtenionName.Equals("png") || fileExtenionName.Equals("jpg"))
                            {
                                string uploadFilePath = "company-" + existingFactory.CompanyId + "/factory/" + factoryId + "-default." + fileExtenionName;
                                SharedFunctions sharedFunctions = new SharedFunctions();
                                fileAbsoluteUri = sharedFunctions.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath, "images");
                            }
                            else
                                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong extension name");
                        }

                    }

                    if (fileAbsoluteUri.Equals(""))
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty");

                    //Edit factory logo path
                    existingFactory.PhotoURL = fileAbsoluteUri;
                    ctx.SaveChanges();

                    var returnObj = new
                    {
                        imageURL = fileAbsoluteUri
                    };
                    return Request.CreateResponse(HttpStatusCode.OK, returnObj);
                }
                catch (System.Exception e)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
                }
            }
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [HttpDelete]
        [Route("{factoryId}")]
        public IHttpActionResult Delete(int factoryId)
        {
            using (var ctx = new SFDatabaseEntities())
            {
                var factory = ctx.Factory
                    .Where(s => s.Id == factoryId)
                    .FirstOrDefault();
                if (factory != null)
                {
                    factory.DeletedFlag = true;
                    try
                    {
                        ctx.SaveChanges();
                    }
                    catch
                    {
                        return InternalServerError();
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok("Success");
        }

        /// <summary>
        /// Roles : admin, superadmin
        /// </summary>
        [Route("{factoryId}/Message")]
        public IHttpActionResult GetMessageByFactoryId(int factoryId, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                FactoryModels factoryModel = new FactoryModels();
                int companyId = factoryModel.getFactoryById(factoryId).CompanyId;
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(companyId);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(companyId, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByFactoryId(factoryId, top, hours, order));
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
