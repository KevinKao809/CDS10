using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sfAdmin.Controllers
{
    public class MonitorController : Controller
    {
        public static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RTMessageHub>();
        public ActionResult RTMessageFeedIn()
        {
            StringBuilder logMessage = new StringBuilder();
            MemoryStream memstream = new MemoryStream();
            string feedInData = null;
            Request.InputStream.CopyTo(memstream);
            memstream.Position = 0;
            using (StreamReader reader = new StreamReader(memstream))
            {
                feedInData = reader.ReadToEnd();
            }
            try
            {
                dynamic jsonMessages = JsonConvert.DeserializeObject(feedInData);                
                if (jsonMessages is JArray)
                {
                    foreach (var messageObj in jsonMessages)
                        processOneFeedInMessage(messageObj);
                }
                else
                {
                    dynamic messageObj = JsonConvert.DeserializeObject(feedInData);
                    processOneFeedInMessage(messageObj);
                }
                return this.Content("{\"Result\":\"OK\"}", "application/json");
            }
            catch (Exception ex)
            {
                logMessage.AppendLine("RTMessageFeedIn Exeption: " + ex.Message);
                logMessage.AppendLine("Received Message: " + feedInData);
                logMessage.AppendLine("Content Type:" + Request.ContentType);
                logMessage.AppendLine("Content Length:" + Request.ContentLength);
                logMessage.AppendLine("Source IP:" + Request.UserHostAddress);
                Global._sfAppLogger.Error(logMessage);
                return this.Content("{\"Result\":\"Fail\"}", "application/json");
            }
        }

        private void processOneFeedInMessage(dynamic messageObj)
        {
            StringBuilder logMessage = new StringBuilder();
            string cacheKey = null, messageString = null;
            int iCompanyId;

            messageString = JsonConvert.SerializeObject(messageObj);

            if (messageObj.AlarmRuleCatalogId != null)
            {
                iCompanyId = messageObj.Message.companyId;
                cacheKey = GetMessageCacheKeyFromFeedInData(messageObj, true);
                logMessage.AppendLine("SignalR Received Alarm (" + iCompanyId + "):" + messageString);
            }
            else
            {
                iCompanyId = messageObj.companyId;
                cacheKey = GetMessageCacheKeyFromFeedInData(messageObj, false);
                logMessage.AppendLine("SignalR Received Message (" + iCompanyId + "):" + messageString);               
            }
            hubContext.Clients.Group(iCompanyId.ToString()).onReceivedMessage(messageString);
            /*  Disable this feature here
            if (cacheKey != null)
                RedisCacheHelper.SetKeyValue(cacheKey, messageString, new TimeSpan(0, 1, 0));
            */
            Global._sfAppLogger.Info(logMessage);
        }        

        private string GetMessageCacheKeyFromFeedInData(dynamic message, bool isAlarm)
        {
            try
            {
                if (isAlarm)
                    return "c" + message.Message.companyId + "_" + message.Message.equipmentId + "_m" + message.Message.MessageCatalogId + "_alarm";
                else
                    return "c" + message.companyId + "_" + message.equipmentId + "_m" + message.MessageCatalogId + "_telemety";
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ActionResult> RunningTask()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                String runningTask = await apiHelper.callAPIService("GET", Global._operationTaskSearchEndPoint, null);
                //JObject jsonObj = JObject.Parse(runningTask);
                runningTask = runningTask.Replace("\r\n", "");
                ViewBag.RunningTaskList = runningTask.Replace("\\\\", "\\\\\\\\");
                System.Diagnostics.Debug.Print(runningTask);
                //ViewBag.RunningTaskList = await apiHelper.callAPIService("GET", Global._operationTaskEndPoint, null);
                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyId = compSession.id;
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
                ViewBag.MenuItem = "menuMonitor";
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

        public async Task<ActionResult> IoTHubReceiver()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.IoTHubList = await apiHelper.callAPIService("GET", Global._iotHubInCompanyEndPoint, null);

                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyId = compSession.id;
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
                ViewBag.MenuItem = "menuMonitor";
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

        public async Task<ActionResult> UsageLog()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                try
                {
                    ViewBag.UsageLogList = await apiHelper.callAPIService("GET", Global._usageLogEndPoint + "/Last", null);
                }
                catch
                {
                    ViewBag.UsageLogList = "";
                    //No data
                }

                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyId = compSession.id;
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
                ViewBag.MenuItem = "menuMonitor";
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
            if (Request.QueryString["mAction"] != null)
            {
                try
                {
                    EmployeeSession empSession = null;
                    if (Session["empSession"] != null)
                        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["mAction"].ToString().ToLower())
                    {
                        case "iothubreceiver":
                            {
                                string subAction = Request.QueryString["sAction"].ToString();
                                string IoTHubAlias = Request.Form["iotHubAlias"].ToString();

                                endPoint = Global._operationTaskEndPoint;

                                if (subAction.ToLower() == "launch iothub receiver")
                                {
                                    OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "create iothub alias", 0, empSession.firstName + " " + empSession.lastName, empSession.email);
                                    OpsTaskModel opsTask = new OpsTaskModel(subAction, empSession.companyId, "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
                                    postData = opsTask.GetPostData();
                                    jsonString = await apiHelper.callAPIService("POST", endPoint, postData);
                                    dynamic jsonResult = JObject.Parse(jsonString);
                                    if (jsonResult.id != null)
                                    {
                                        opsInfraMessage.taskId = jsonResult.id;
                                        opsInfraMessage.Send();
                                    }
                                }
                                else if (subAction.ToLower() == "restart iothub receiver")
                                {
                                    IoTHubEventProcessTopic iotHubTopic = new IoTHubEventProcessTopic("Restart", IoTHubAlias, 0, empSession.firstName + " " + empSession.lastName, empSession.email);
                                    OpsTaskModel opsTask = new OpsTaskModel(subAction, empSession.companyId, "IoTHubAlias", IoTHubAlias, iotHubTopic.GetJsonContent());
                                    postData = opsTask.GetPostData();
                                    jsonString = await apiHelper.callAPIService("POST", endPoint, postData);
                                    dynamic jsonResult = JObject.Parse(jsonString);
                                    if (jsonResult.id != null)
                                    {
                                        iotHubTopic.taskId = jsonResult.id;
                                        iotHubTopic.Send();
                                    }
                                }
                                break;
                            }
                        case "getrunningtask":
                            {
                                endPoint = Global._operationTaskSearchEndPoint;
                                endPoint = endPoint + "?";
                                if (Request.QueryString["taskstatus"] != null)
                                    endPoint = endPoint + "&taskstatus=" + Request.QueryString["taskstatus"];

                                if (Request.QueryString["hours"] != null)
                                    endPoint = endPoint + "&hours=" + Request.QueryString["hours"];
                                jsonString = await apiHelper.callAPIService("GET", endPoint, postData);

                                break;
                            }
                        case "getusagelog":
                            {
                                endPoint = Global._usageLogEndPoint;
                                endPoint = endPoint + "?";
                                if (Request.QueryString["days"] != null)
                                    endPoint = endPoint + "&days=" + Request.QueryString["days"];

                                if (Request.QueryString["order"] != null)
                                    endPoint = endPoint + "&order=" + Request.QueryString["order"];
                                jsonString = await apiHelper.callAPIService("GET", endPoint, postData);

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
    }
}