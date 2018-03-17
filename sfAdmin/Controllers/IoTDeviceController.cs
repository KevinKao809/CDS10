using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Microsoft.ServiceBus.Messaging;

namespace sfAdmin.Controllers
{
    public class IoTDeviceController : Controller
    {
        // GET: IoTDevice
        public async Task<ActionResult> Index()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.IoTDeviceList = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);
                ViewBag.IoTHubList = await apiHelper.callAPIService("GET", Global._iotHubInCompanyEndPoint, null);
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);
                ViewBag.DeviceTypeList = await apiHelper.callAPIService("GET", Global._deviceTypeEndPoint, null); 
                ViewBag.CertificateList = await apiHelper.callAPIService("GET", Global._deviceCertificateInCompanyEndPoint, null);
                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                /* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;
                ViewBag.MenuItem = "menuIoTDevice";
            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<ActionResult> ConfigurationManagement()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.IoTDeviceList = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);
                //ViewBag.IoTDeviceList = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);
                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                /* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;
                ViewBag.MenuItem = "menuIoTDevice";
            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<ActionResult> MessageAttach()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.iotdevice = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);
                ViewBag.nonChildMsg = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);
                ViewBag.FactoryList = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);


                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                /* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;
                ViewBag.MenuItem = "menuIoTDevice";                
            }
            catch (Exception ex)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                if (ex.Message.ToLower() == "invalid session")
                {
                    loginMsgSession.toastLevel = "warning";
                    loginMsgSession.message = "[[[Please Login]]]";
                }
                else
                {
                    loginMsgSession.toastLevel = "error";
                    loginMsgSession.message = "[[[Authentication Fail]]].";
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: Authentication Fail.");
                    logMessage.AppendLine("email:" + empSession.email);
                    logMessage.AppendLine("password:" + empSession.password);
                    Global._sfAuditLogger.Audit(logMessage);
                }
                Session["loginMsgSession"] = loginMsgSession.Serialize();
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
                    EmployeeSession empSession = null;
                    if (Session["empSession"] != null)
                        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    string requesterName = empSession.firstName + " " + empSession.lastName;
                    string requesterEmail = empSession.email;
                    int companyId = empSession.companyId;
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "getiotdevice":
                            {
                                endPoint = Global._iotDeviceInCompanyEndPoint;
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "getiotdevicebyfactory":
                            {
                                string factoryId = Request.QueryString["factoryId"];
                                if (factoryId != null)
                                {
                                    if (factoryId.ToLower() == "all")
                                        endPoint = Global._iotDeviceInCompanyEndPoint;
                                    else
                                        endPoint = Global._iotDeviceInFactoryEndPoint + "/" + factoryId;
                                }
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;

                            }
                        case "addiotdevice":
                            {
                                //add
                                endPoint = Global._iotDeviceEndPoint;
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("POST", endPoint, postData);
                                System.Diagnostics.Debug.Print(endPoint);
                                System.Diagnostics.Debug.Print(postData);

                                /********** Send OpsInfra command message ***********/
                                string iotHubAlias = Request.Form["IoTHubAlias"];
                                string iotHubDeviceId = Request.Form["IoTHubDeviceId"];
                                string authType = Request.Form["AuthenticationType"];
                                string ceritficateId = Request.Form["DeviceCertificateId"];
                                   
                                IoTDeviceCmdMsg deviceCmdMsg = new IoTDeviceCmdMsg("create iothub register", iotHubDeviceId, authType,  requesterName, requesterEmail, 0);
                                await deviceCmdMsg.Init(iotHubAlias, ceritficateId);

                                int operationTaskId = await PostOperationTaskAsync(deviceCmdMsg.task, companyId, deviceCmdMsg.entity, deviceCmdMsg.entityId, deviceCmdMsg.GetJsonInsensitiveContent());
                                if (operationTaskId > 0)
                                {
                                    deviceCmdMsg.taskId = operationTaskId;
                                    deviceCmdMsg.SendToServiceBus();
                                }
                                break;
                            }

                        case "updateiotdevice":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                dynamic oldIoTDeviceObj = JObject.Parse(jsonString);
                                jsonString = await apiHelper.callAPIService("PUT", endPoint, postData);

                                /********** Send OpsInfra command message ***********/
                                dynamic newIoTDeviceObj = new JObject();
                                string newIoTHubDeviceId = Request.Form["IoTHubDeviceId"];
                                string newAuthenticationType = Request.Form["AuthenticationType"];
                                string newIoTHubAlias = Request.Form["IoTHubAlias"];
                                string newDeviceCertificateId = Request.Form["DeviceCertificateId"];

                                string oldIoTHubDeviceId = oldIoTDeviceObj.IoTHubDeviceId;
                                string oldAuthenticationType = oldIoTDeviceObj.AuthenticationType;
                                string oldIoTHubAlias = oldIoTDeviceObj.IoTHubAlias;
                                string oldDeviceCertificateId = oldIoTDeviceObj.DeviceCertificateId;

                                IoTDeviceCmdMsg deviceCmdMsg = new IoTDeviceCmdMsg("update iothub register", newIoTHubDeviceId, newAuthenticationType, requesterName, requesterEmail, 0);
                                if(oldIoTHubAlias == newIoTHubAlias)
                                    await deviceCmdMsg.Init(newIoTHubAlias, newDeviceCertificateId);
                                else
                                    await deviceCmdMsg.Init(newIoTHubAlias, newDeviceCertificateId, oldIoTHubAlias);

                                int operationTaskId = await PostOperationTaskAsync(deviceCmdMsg.task, companyId, deviceCmdMsg.entity, deviceCmdMsg.entityId, deviceCmdMsg.GetJsonInsensitiveContent());

                                if (operationTaskId > 0)
                                {
                                    deviceCmdMsg.taskId = operationTaskId;
                                    deviceCmdMsg.SendToServiceBus();
                                }

                                break;
                            }
                        case "resetpassword":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"] +"/"+ "ResetPassword";
                                postData = Request.Form.ToString();
                                jsonString = apiHelper.changePassword("PUT", endPoint, postData);
                                break;
                            }
                        case "deleteiotdevice":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"];

                                /********** Send OpsInfra command message ***********/
                                dynamic existingIoTDeviceObj = JObject.Parse(await apiHelper.callAPIService("GET", endPoint, null));   
                                string iotHubAlias = existingIoTDeviceObj.IoTHubAlias;
                                string iotHubDeviceId = existingIoTDeviceObj.IoTHubDeviceId;
                                string authType = existingIoTDeviceObj.AuthenticationType;

                                IoTDeviceCmdMsg deviceCmdMsg = new IoTDeviceCmdMsg("remove iothub register", iotHubDeviceId, authType, requesterName, requesterEmail, 0);
                                await deviceCmdMsg.Init(iotHubAlias);

                                int operationTaskId = await PostOperationTaskAsync(deviceCmdMsg.task, companyId, deviceCmdMsg.entity, deviceCmdMsg.entityId, deviceCmdMsg.GetJsonInsensitiveContent());
                                if (operationTaskId > 0)
                                { 
                                    deviceCmdMsg.taskId = operationTaskId;
                                    deviceCmdMsg.SendToServiceBus();
                                }

                                //Delete data in DB
                                jsonString = await apiHelper.callAPIService("DELETE", endPoint, null);
                                break;
                            }
                        //attach message
                        case "getattachedmessagebydeviceid":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"] + "/Message";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "updateattachedmessage":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"] + "/Message";
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("PUT", endPoint, postData);
                                break;
                            }
                        case "getdeviceconfiguration":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"] + "/Configuration";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }

                        case "updatedeviceconfiguration":
                            {                                
                                string iotHubDeviceId = Request.QueryString["Id"];
                                if (iotHubDeviceId != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + iotHubDeviceId + "/DesiredProperty";
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("PUT", endPoint, postData);
                                System.Diagnostics.Debug.Print(endPoint);
                                System.Diagnostics.Debug.Print(postData);


                                /********** Send OpsInfra command message ***********/
                                string deviceConfiguration = Request.Form["devicetwinsdesired"];
                                IoTDeviceManagementCmdMsg cmdMsg = new IoTDeviceManagementCmdMsg("update device desired property", iotHubDeviceId, requesterName, requesterEmail, 0, deviceConfiguration);
                                await cmdMsg.Init();

                                int operationTaskId = await PostOperationTaskAsync(cmdMsg.task, companyId, cmdMsg.entity, cmdMsg.entityId, cmdMsg.GetJsonInsensitiveContent());
                                if (operationTaskId > 0)
                                {
                                    cmdMsg.taskId = operationTaskId;
                                    cmdMsg.SendToServiceBus();
                                }                                
                                break;
                            }              
                        case "downloadmessagetemplate":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._iotDeviceEndPoint + "/" + Request.QueryString["Id"] + "/MessageTemplate";
                                
                                HttpResponseMessage response = new HttpResponseMessage();
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);

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
                        System.Diagnostics.Debug.Print("EndPoint:" + endPoint);
                        System.Diagnostics.Debug.Print("Action:" + Request.QueryString["action"].ToString());
                        System.Diagnostics.Debug.Print("PostData:" + Request.Form.ToString());
                        System.Diagnostics.Debug.Print("Message:" + ex.Message);
                        Global._sfAppLogger.Error(logMessage);
                        Response.StatusCode = 500;
                        jsonString = ex.Message;
                    }
                }
            }
            return Content(JsonConvert.SerializeObject(jsonString), "application/json");
        }

        public async Task<int> PostOperationTaskAsync(string name, int companyId, string entity, string entityId, string taskcontent)
        {
            OpsTaskModel opsTaskModel = new OpsTaskModel(name, companyId, entity, entityId, taskcontent);
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string opsTaskEndPoint = Global._operationTaskEndPoint;
            string opsTaskPostData = opsTaskModel.GetPostData();
            dynamic opsTaskObject = JObject.Parse(await apiHelper.callAPIService("post", opsTaskEndPoint, opsTaskPostData));

            return opsTaskObject.id;
        }

        private async Task<string> GetIoTDeviceKey(string iotHubDeviceId)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._iotDeviceEndPoint;
            endPoint = endPoint + "/" + iotHubDeviceId;
            string jsonString = await apiHelper.callAPIService("GET", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            return jsonResult.IoTHubDeviceKey;
        }

        private async Task<string> GetCertificateThumbprint(string certificateId)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._deviceCertificateEndPoint;
            endPoint = endPoint + "/" + certificateId;
            string jsonString = await apiHelper.callAPIService("get", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            return jsonResult.Thumbprint;
        }

        private async Task<dynamic> GetIoTHubConnectionString(string iotHubAlias)
        {
            RestfulAPIHelper apiHelper = new RestfulAPIHelper();
            string endPoint = Global._iotHubEndPoint;
            endPoint = endPoint + "/" + iotHubAlias;
            string jsonString = await apiHelper.callAPIService("get", endPoint, null);
            dynamic jsonResult = JObject.Parse(jsonString);

            dynamic connectionStrings = new System.Dynamic.ExpandoObject();
            connectionStrings.primary = jsonResult.P_IoTHubConnectionString;
            connectionStrings.secondary = jsonResult.S_IoTHubConnectionString;

            return connectionStrings;
        }
        //public HttpResponseMessage GetText()
        //{
        //    try
        //    {
        //        string content = "Hello";

        //        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
        //        result.Content = new StringContent(content);
        //        //a text file is actually an octet-stream (pdf, etc)
        //        result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //        //we used attachment to force download
        //        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
        //        result.Content.Headers.ContentDisposition.FileName = "mytext.txt";
        //        return result;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new HttpResponseException(HttpStatusCode.InternalServerError);
        //    }

        //}
    }
}