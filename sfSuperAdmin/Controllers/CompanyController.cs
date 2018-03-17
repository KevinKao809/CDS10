using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfShareLib;
using sfSuperAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.ServiceBus.Messaging;

namespace sfSuperAdmin.Controllers
{
    public class CompanyController : Controller
    {
        public async Task<ActionResult> EditIoTHub()
        {
            if (Request.Form["inputCompanyId"] == null)
            {
                return RedirectToAction("Index", "Company");
            }
            try
            {
                string companyId = Request.Form["inputCompanyId"];
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string companyString = await apiHelper.callAPIService("GET", Global._companyEndPoint + "/" + companyId, null);
                dynamic companyObj = JObject.Parse(companyString);
                ViewBag.CompanyId = companyObj.Id;
                ViewBag.CompanyName = companyObj.Name;
                ViewBag.IoTHubList = await apiHelper.callAPIService("GET", Global._iotHubInCompanyEndPoint + "/" + companyId, null);

                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuCompany";
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "invalid session")
                {
                    Session["toastLevel"] = "warning";
                    Session["loginMessage"] = "Please Login";
                }
                else
                {
                    Session["toastLevel"] = "error";
                    Session["loginMessage"] = "Authentication Fail.";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + Session["email"]);
                    logMessage.AppendLine("password:" + Session["password"]);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        public async Task<ActionResult> EditEmployee()
        {
            if (Request.Form["inputCompanyId"] == null)
            {
                return RedirectToAction("Index", "Company");
            }
            try
            {
                string companyId = Request.Form["inputCompanyId"];
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string companyString = await apiHelper.callAPIService("GET", Global._companyEndPoint + "/" + companyId, null);
                dynamic companyObj = JObject.Parse(companyString);
                ViewBag.CompanyId = companyObj.Id;
                ViewBag.CompanyName = companyObj.Name;
                ViewBag.EmployeeList = await apiHelper.callAPIService("GET", Global._employeeEndPoint + "/Company/" + companyId, null);
                ViewBag.UserRoleList = await apiHelper.callAPIService("GET", Global._userRoleEndPoint + "/Company/" + companyId, null);

                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuCompany";
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "invalid session")
                {
                    Session["toastLevel"] = "warning";
                    Session["loginMessage"] = "Please Login";
                }
                else
                {
                    Session["toastLevel"] = "error";
                    Session["loginMessage"] = "Authentication Fail.";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + Session["email"]);
                    logMessage.AppendLine("password:" + Session["password"]);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<ActionResult> EditDashboard()
        {
            if (Request.Form["inputCompanyId"] == null)
            {
                return RedirectToAction("Index", "Company");
            }
            try
            {
                string companyId = Request.Form["inputCompanyId"];
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string companyString = await apiHelper.callAPIService("GET", Global._companyEndPoint + "/" + companyId, null);
                dynamic companyObj = JObject.Parse(companyString);

                ViewBag.dashboardList = await apiHelper.callAPIService("GET", Global._externalDashboardEndPoint + "/Company/" + companyId, null);
                ViewBag.CompanyId = companyId;
                ViewBag.CompanyName = companyObj.Name;
                //ViewBag.CompanyName = companyObj.Name;
                //ViewBag.EmployeeList = await apiHelper.callAPIService("GET", Global._employeeEndPoint + "/Company/" + companyId, null);
                // ViewBag.UserRoleList = await apiHelper.callAPIService("GET", Global._userRoleEndPoint + "/Company/" + companyId, null);

                //ViewBag.FirstName = Session["firstName"].ToString();
                // ViewBag.LastName = Session["lastName"].ToString();
                //ViewBag.Email = Session["email"].ToString();

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuCompany";
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "invalid session")
                {
                    Session["toastLevel"] = "warning";
                    Session["loginMessage"] = "Please Login";
                }
                else
                {
                    Session["toastLevel"] = "error";
                    Session["loginMessage"] = "Authentication Fail.";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + Session["email"]);
                    logMessage.AppendLine("password:" + Session["password"]);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // GET: Company
        public async Task<ActionResult> Index()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.CompanyList = await apiHelper.callAPIService("GET", Global._companyEndPoint, null);
                ViewBag.CultureInfoList = await apiHelper.callAPIService("GET", Global._cultureInfoEndPoint, null);
                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();
                ViewBag.DocDBConnectionString = Global._sfDocDBConnectionString;
                ViewBag.AdminWebURI = Global._sfAdminWebURI;

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuCompany";
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "invalid session")
                {
                    Session["toastLevel"] = "warning";
                    Session["loginMessage"] = "Please Login";
                }
                else
                {
                    Session["toastLevel"] = "error";
                    Session["loginMessage"] = "Authentication Fail.";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + Session["email"]);
                    logMessage.AppendLine("password:" + Session["password"]);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public async Task<ActionResult> ReqAction()
        {
            string jsonString = "", postData = "", endPoint = "";
            if (Request.QueryString["action"] != null)
            {
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    string requesterName = Session["firstName"].ToString() + " " + Session["lastName"].ToString();
                    string requesterEmail = Session["email"].ToString();
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "genextappkey":
                            string uniqueId = Guid.NewGuid().ToString();
                            jsonString = "{\"Key\":\"" + System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(uniqueId)) + "\"}";
                            break;
                        case "getsacredential":
                            var plainTextCredential = System.Text.Encoding.UTF8.GetBytes(Session["email"].ToString() + ":" + Session["password"].ToString());
                            jsonString = "{\"Credential\":\"" + System.Convert.ToBase64String(plainTextCredential) + "\"}";
                            break;
                        case "getcompany":
                            endPoint = Global._companyEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        case "deletecompany":
                            endPoint = Global._companyEndPoint;
                            if (Request.QueryString["Id"] != null)
                            {
                                /********** Ops Infra By Andy ***********/
                                int companyId = Int32.Parse(Request.QueryString["Id"]);
                                DocDBCmdMsg docDBCmdMsg = new DocDBCmdMsg("purge", companyId, requesterName, requesterEmail, 0);
                                int operationTaskId = await PostOperationTaskAsync(docDBCmdMsg.task, companyId, docDBCmdMsg.entity, docDBCmdMsg.entityId, docDBCmdMsg.GetJsonInsensitiveContent());

                                if (operationTaskId > 0)
                                {
                                    docDBCmdMsg.taskId = operationTaskId;
                                    docDBCmdMsg.SendToServiceBus();
                                }

                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            }

                            break;
                        case "addcompany":
                            {
                                endPoint = Global._companyEndPoint;
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);

                                //Get Object ID from API result
                                dynamic jsonResult = JObject.Parse(jsonString);
                                if (jsonResult.id != null)
                                {   //jsonResult = company
                                    if (Request.Files.Count > 0)
                                    {
                                        string entityID = jsonResult.id;
                                        endPoint = endPoint + "/" + entityID + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                    }

                                    /********** Ops Infra By Andy ***********/
                                    int companyId = (int)jsonResult.id;
                                    DocDBCmdMsg docDBCmdMsg = new DocDBCmdMsg("create", companyId, requesterName, requesterEmail, 0);
                                    int operationTaskId = await PostOperationTaskAsync(docDBCmdMsg.task, companyId, docDBCmdMsg.entity, docDBCmdMsg.entityId, docDBCmdMsg.GetJsonInsensitiveContent());

                                    if (operationTaskId > 0)
                                    {
                                        docDBCmdMsg.taskId = operationTaskId;
                                        docDBCmdMsg.SendToServiceBus();
                                    }
                                }
                                break;
                            }
                        case "updatecompany":
                            {
                                endPoint = Global._companyEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                if (Request.Files.Count > 0)
                                {
                                    //admin-api/Company/{id}/Image
                                    endPoint = endPoint + "/Image";
                                    byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                    Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                    jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                }
                                break;
                            }
                        case "getemployeebycmp":
                            {
                                endPoint = Global._employeeEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/Company/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "getuserrolebycmp":
                            {
                                endPoint = Global._userRoleEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/Company/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "deleteemployee":
                            {
                                endPoint = Global._employeeEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "addemployee":
                            {
                                endPoint = Global._employeeEndPoint;
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                dynamic jsonResult = JObject.Parse(jsonString);

                                if (Request.Files.Count > 0)
                                {
                                    //Get Object ID from API result
                                    if (jsonResult.id != null)
                                    {
                                        string entityID = jsonResult.id;
                                        endPoint = endPoint + "/" + entityID + "/Image";
                                        byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                        Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                        jsonString = await apiHelper.putUploadFile(endPoint, byteFile, Request.Files[0].FileName);
                                    }
                                }

                                if (jsonResult.id != null && Request.Form["UserRoleId"] != null)
                                {
                                    endPoint = endPoint + "/" + jsonResult.id + "/Role";
                                    jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                }

                                break;
                            }
                        case "updateemployee":
                            {
                                endPoint = Global._employeeEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);

                                var RoleEndPoint = endPoint + "/Role";
                                if (Request.Form["UserRoleId"] != null)
                                {
                                    jsonString = await apiHelper.callAPIService("put", RoleEndPoint, postData);
                                }
                                else
                                {
                                    jsonString = await apiHelper.callAPIService("put", RoleEndPoint, "UserRoleId=");
                                }

                                if (Request.Files.Count > 0)
                                {
                                    //admin-api/Company/{id}/Image
                                    var ImageEndPoint = endPoint + "/Image";
                                    byte[] byteFile = new byte[Request.Files[0].InputStream.Length];
                                    Request.Files[0].InputStream.Read(byteFile, 0, (int)Request.Files[0].InputStream.Length);
                                    jsonString = await apiHelper.putUploadFile(ImageEndPoint, byteFile, Request.Files[0].FileName);
                                }

                                break;
                            }
                        case "resetpassword":
                            endPoint = Global._employeeEndPoint + "/" + Request.QueryString["Id"] + "/resetpassword";
                            postData = Request.Form.ToString();
                            jsonString = apiHelper.changePassword("put", endPoint, postData);
                            break;
                        case "getuserrolebyemployeeid":
                            endPoint = Global._employeeEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Role";
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        case "getiothub":
                            {
                                endPoint = Global._iotHubInCompanyEndPoint;
                                if (Request.Form["CompanyId"] != null)
                                    endPoint = endPoint + "/" + Request.Form["CompanyId"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "deleteiothub":
                            {
                                endPoint = Global._iotHubEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                dynamic jsonResult = JObject.Parse(jsonString);
                                if (jsonResult.IoTHubAlias != null)
                                {
                                    string IoTHubAlias = jsonResult.IoTHubAlias;
                                    string CompanyId = Request.Form["CompanyId"];
                                    /* Send message to OpsInfra to remove IoTHub Receiver */
                                    OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "remove iothub alias", 0, Session["firstName"].ToString() + " " + Session["lastName"].ToString(), Session["email"].ToString());
                                    OpsTaskModel opsTask = new OpsTaskModel("Remove IoTHub Receiver", int.Parse(CompanyId), "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
                                    postData = opsTask.GetPostData();
                                    string taskEndPoint = Global._operationTaskEndPoint;
                                    jsonString = await apiHelper.callAPIService("POST", taskEndPoint, postData);
                                    jsonResult = JObject.Parse(jsonString);
                                    if (jsonResult.id != null)
                                    {
                                        opsInfraMessage.taskId = jsonResult.id;
                                        opsInfraMessage.Send();
                                    }

                                    /* Call Restful API to delete  */
                                    jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                }
                                break;
                            }
                        case "addiothub":
                            {
                                endPoint = Global._iotHubEndPoint;
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);

                                /* Send Message to OpsInfra to launch IoTHub Receiver */
                                string IoTHubAlias = Request.Form["IoTHubAlias"];
                                string CompanyId = Request.Form["CompanyId"];
                                OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "create iothub alias", 0, Session["firstName"].ToString() + " " + Session["lastName"].ToString(), Session["email"].ToString());
                                OpsTaskModel opsTask = new OpsTaskModel("Launch IoTHub Receiver", int.Parse(CompanyId), "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
                                postData = opsTask.GetPostData();
                                string taskEndPoint = Global._operationTaskEndPoint;
                                jsonString = await apiHelper.callAPIService("POST", taskEndPoint, postData);
                                dynamic jsonResult = JObject.Parse(jsonString);
                                if (jsonResult.id != null)
                                {
                                    opsInfraMessage.taskId = jsonResult.id;
                                    opsInfraMessage.Send();
                                }
                                break;
                            }
                        case "updateiothub":
                            {
                                endPoint = Global._iotHubEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        case "addexternaldashboard":
                            {
                                endPoint = Global._externalDashboardEndPoint;
                                //if (Request.QueryString["Id"] != null)
                                // endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updateexternaldashboard":
                            {
                                endPoint = Global._externalDashboardEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];

                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }

                        case "deleteexternaldashboard":
                            {
                                endPoint = Global._externalDashboardEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("delete", endPoint, postData);
                                break;
                            }
                        case "getallexterngetaldashboard":
                            {
                                endPoint = Global._externalDashboardEndPoint + "/Company/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("get", endPoint, postData);
                                break;
                            }


                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.ToLower() == "invalid session")
                        Response.StatusCode = 401;
                    else
                    {
                        StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                        logMessage.AppendLine("EndPoint:" + endPoint);
                        logMessage.AppendLine("Action:" + Request.QueryString["action"].ToString());
                        logMessage.AppendLine("PostData:" + Request.Form.ToString());
                        Global._sfAppLogger.Error(logMessage);
                        Response.StatusCode = 500;
                        jsonString = ex.Message;
                    }
                }
            }

            return Content(JsonConvert.SerializeObject(jsonString), "application/json");
        }

        private async Task<int> PostOperationTaskAsync(string name, int companyId, string entity, string entityId, string taskcontent)
        {
            OpsTaskModel opsTaskModel = new OpsTaskModel(name, companyId, entity, entityId, taskcontent);
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string opsTaskEndPoint = Global._operationTaskEndPoint;
            string opsTaskPostData = opsTaskModel.GetPostData();
            dynamic opsTaskObject = JObject.Parse(await apiHelper.callAPIService("post", opsTaskEndPoint, opsTaskPostData));

            return opsTaskObject.id;
        }

    }
}