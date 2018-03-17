using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

using sfShareLib;
using sfAPIService.Models;
using StackExchange.Redis;
using sfAPIService.Filter;

namespace sfAPIService.Controllers
{
    [Authorize]    
    [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "admin, superadmin")]
    [RoutePrefix("admin-api/Company")]
    public class CompanyController : ApiController
    {
        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetAllCompanies()
        {
            CompanyModels companyModel = new Models.CompanyModels();
            return Ok(companyModel.getAllCompanies());
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("SuperAdmin")]
        public IHttpActionResult GetAllCompaniesBySuperAdmin()
        {
            CompanyModels companyModel = new Models.CompanyModels();
            return Ok(companyModel.getAllCompaniesBySuperAdmin());
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        public IHttpActionResult GetCompanyById(int id)
        {
            RedisKey cacheKey = "company_" + id;
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                CompanyModels companyModel = new CompanyModels();
                try
                {
                    CompanyModels.Detail company = companyModel.getCompanyById(id);
                    RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(company));
                    return Ok(company);
                }
                catch
                {
                    return NotFound();
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<Object>(cacheValue));
            }            
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPost]
        public IHttpActionResult AddFormData([FromBody]CompanyModels.Add company)
        {
            string logForm = "Form : " + Startup._jsSerializer.Serialize(company);
            string logAPI = "[Post] " + Request.RequestUri.ToString();
            
            if (!ModelState.IsValid || company == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                CompanyModels companyModel = new CompanyModels();
                int id = companyModel.addCompany(company);
                return Json(new { id = id });
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine(logForm);
                Startup._sfAppLogger.Error(logAPI + logMessage);

                return InternalServerError(ex);
            }

            /*
            var newCompany = new Company()
            {
                Name = company.Name,
                ShortName = company.ShortName,
                Address = company.Address,
                CompanyWebSite = company.CompanyWebSite,
                ContactName = company.ContactName,
                ContactPhone = company.ContactPhone,
                ContactEmail = company.ContactEmail,
                Latitude = (float)company.Latitude,
                Longitude = (float)company.Longitude,
                CultureInfo = company.CultureInfoId,
                CreatedAt = DateTime.Parse(DateTime.Now.ToString()),
                DeletedFlag = false
            };



            using (var ctx = new SFDatabaseEntities())
            {
                ctx.Company.Add(newCompany);
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
            return Json(new { id = newCompany.Id });
            */
        }

        /// <summary>
        /// AllowAnonymous
        /// </summary>
        [AllowAnonymous]
        [CustomAuthorizationFilter(ClaimType = "Roles", ClaimValue = "")]
        [HttpGet]
        [Route("{id}/AllowDomain")]        
        public IHttpActionResult GetAllowDomainByCompanyId(int id)
        {
            RedisKey cacheKey = "company_" + id;
            string cacheValue = RedisCacheHelper.GetValueByKey(cacheKey);

            if (cacheValue == null)
            {
                CompanyModels companyModel = new CompanyModels();
                try
                {
                    CompanyModels.Detail company = companyModel.getCompanyById(id);
                    RedisCacheHelper.SetKeyValue(cacheKey, new JavaScriptSerializer().Serialize(company));
                    return Ok(company.AllowDomain);
                }
                catch
                {
                    return NotFound();
                }
            }
            else
            {
                return Ok(new JavaScriptSerializer().Deserialize<CompanyModels.Detail>(cacheValue).AllowDomain);
            }
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        [Route("{id}/Image")]
        public async Task<HttpResponseMessage> UploadLogoFile(int id)
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
                return Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);

            CompanyModels companyModel = new CompanyModels();
            FileUtility fileHelper = new FileUtility();
            string root = Path.GetTempPath();
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                CompanyModels.Detail company = companyModel.getCompanyById(id);
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
                        string uploadFilePath = "company-" + id + "/" + id + "-default." + fileHelper.LowerAndFilterString(fileExtenionName);
                        fileAbsoluteUri = fileHelper.SaveFiletoStorage(fileData.LocalFileName, uploadFilePath, "images");
                    }
                }

                if (fileAbsoluteUri.Equals(""))
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "File is empty or wrong extension name");

                //Edit company logo path
                companyModel.updateCompanyLogoURL(id, fileAbsoluteUri);
                RedisCacheHelper.DeleteCompanyCache(id);
                return Request.CreateResponse(HttpStatusCode.OK, new { imageURL = fileAbsoluteUri });
            }
            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }            
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpPut]
        public IHttpActionResult EditFormData(int id, [FromBody] CompanyModels.Update company)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            string logForm = "Form : " + js.Serialize(company);
            string logAPI = "[Put] " + Request.RequestUri.ToString();

            if (!ModelState.IsValid || company == null)
            {
                Startup._sfAppLogger.Warn(logAPI + " || Input Parameter not expected || " + logForm);
                return BadRequest("Invalid data");
            }

            try
            {
                CompanyModels companyModel = new CompanyModels();
                companyModel.updateCompany(id, company);
                RedisCacheHelper.DeleteCompanyCache(id);
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
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpDelete]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                CompanyModels companyModel = new CompanyModels();
                companyModel.deleteCompany(id);
                RedisCacheHelper.DeleteCompanyCache(id);
                return Ok("Success");
            }
            catch(Exception ex)
            {
                string logAPI = "[Delete] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return InternalServerError();
            }
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/UsageLog")]
        public IHttpActionResult GetAllUsageLog(int id, [FromUri]int days = 1, [FromUri]string order = "asc")
        {
            UsagLogModels usageLogModel = new UsagLogModels();
            return Ok(usageLogModel.getAllByCompanyId(id, days, order));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/UsageLog/Last")]
        public IHttpActionResult GetLastUsageLog(int id)
        {
            try
            {
                UsagLogModels usageLogModel = new UsagLogModels();
                return Ok(usageLogModel.getLastByCompanyId(id));
            }
            catch (Exception ex)
            {
                string logAPI = "[Get] " + Request.RequestUri.ToString();
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                Startup._sfAppLogger.Error(logAPI + logMessage);
                return NotFound();
            }            
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/IoTDeviceCustomizedConfiguration")]
        public IHttpActionResult GetAllIoTDeviceCustomizedConfiguration(int id)
        {
            IoTDeviceCustomizedConfigurationModels iotDeviceCCModels = new IoTDeviceCustomizedConfigurationModels();
            return Ok(iotDeviceCCModels.GetAllCustomizedConfigurationByCompanyId(id));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [HttpGet]
        [Route("{id}/ExternalDashboard")]
        public IHttpActionResult GetAllExternalDashboard(int id)
        {
            ExternalDashboardModels model = new ExternalDashboardModels();
            return Ok(model.GetAllExternalDashboardByCompanyId(id));
        }

        /// <summary>
        /// Roles : admin, SuperAdmin
        /// </summary>
        [Route("{id}/Message")]
        public IHttpActionResult GetMessageByCompanyId(int id, [FromUri]int top = 10, [FromUri]int hours = 168, [FromUri]string order = "desc")
        {
            try
            {
                CompanyModels companyModel = new CompanyModels();
                CompanyModels.Detail company = companyModel.getCompanyById(id);
                DocumentDBHelper docDBHelpler = new DocumentDBHelper(id, company.DocDBConnectionString);
                return Ok(docDBHelpler.GetMessageByCompanyId(id, top, hours, order));
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
