using Microsoft.AspNet.SignalR;
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

namespace sfSuperAdmin.Controllers
{
    public class MonitorController : Controller
    {
        public static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<RTMessageHub>();
        public ActionResult RTMessageFeedIn()
        {
            string feedInData = Server.UrlDecode(Request.Form.ToString());
            dynamic jsonMessages = JsonConvert.DeserializeObject(feedInData);
            if (jsonMessages != null)
            {
                hubContext.Clients.All.onReceivedMessage(feedInData);

                //StringBuilder logMessage = new StringBuilder();
                //logMessage.AppendLine("SignalR Received:" + feedInData);
                //Global._logger.Info(logMessage);
            }
            return this.Content("{\"Result\":\"OK\"}", "application/json");
        }

        public async Task<ActionResult> RunningTask()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                String runningTask = await apiHelper.callAPIService("GET", Global._operationTaskSearchEndPoint, null);
                //JObject jsonObj = JObject.Parse(runningTask);
                string temp = runningTask.Replace("\r\n", "");
                ViewBag.RunningTaskList = temp.Replace("\\\\", "\\\\\\\\");
                System.Diagnostics.Debug.Print(runningTask);
                

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuMonitor";

                /* Setup Employee Data on Page */
                ViewBag.FirstName = Session["firstName"];
                ViewBag.LastName = Session["lastName"];
                ViewBag.Email = Session["email"];
                ViewBag.PhotoURL = Session["photoURL"];
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

        public ActionResult RunningOpsProcess()
        {            
            try
            {                
                if (Session["email"] == null)
                {
                    throw new Exception("invalid session");
                }

                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();
                ViewBag.ServiceFabricURI = Global._sfSrvFabricBaseURI;

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuMonitor";                
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

        public async Task<ActionResult> IoTHubReceiver()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.IoTHubList = await apiHelper.callAPIService("GET", Global._iotHubEndPoint, null);
                if (Session["email"] == null)
                {
                    throw new Exception("invalid session");
                }

                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuMonitor";                
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
        public async Task<ActionResult> UsageLog()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string endPoint = Global._usageLogSumByDayEndPoint+"/Last";
                ViewBag.UsageLog = await apiHelper.callAPIService("GET", endPoint, null);

                /* Setup Company Name and Company Photo on Page */
                ViewBag.CompanyName = Session["companyName"];
                ViewBag.CompanyPhotoURL = Session["companyPhotoURL"];

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuSetup";

                /* Setup Employee Data on Page */
                ViewBag.FirstName = Session["firstName"];
                ViewBag.LastName = Session["lastName"];
                ViewBag.Email = Session["email"];
                ViewBag.PhotoURL = Session["photoURL"];
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
            int companyId;            
            if (Request.QueryString["mAction"] != null)
            {
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["mAction"].ToString().ToLower())
                    {
                        case "iothubreceiver":
                            {
                                companyId = int.Parse(Request.QueryString["companyId"].ToString());
                                string subAction = Request.QueryString["sAction"].ToString();
                                string IoTHubAlias = Request.Form["iotHubAlias"].ToString();

                                endPoint = Global._operationTaskEndPoint;

                                if (subAction.ToLower() == "launch iothub receiver")
                                {
                                    OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "create iothub alias", 0, Session["firstName"].ToString() + " " + Session["lastName"].ToString(), Session["email"].ToString());
                                    OpsTaskModel opsTask = new OpsTaskModel(subAction, companyId, "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
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
                                    IoTHubEventProcessTopic iotHubTopic = new IoTHubEventProcessTopic("Restart", IoTHubAlias, 0, Session["firstName"].ToString() + " " + Session["lastName"].ToString(), Session["email"].ToString());
                                    OpsTaskModel opsTask = new OpsTaskModel(subAction, companyId, "IoTHubAlias", IoTHubAlias, iotHubTopic.GetJsonContent());
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
                                string q = "?";
                                if (Request.QueryString["taskstatus"] != null)
                                {
                                    endPoint = endPoint + q + "taskstatus=" + Request.QueryString["taskstatus"];
                                    q = "&";
                                }
                                if (Request.QueryString["hours"] != null)
                                    endPoint = endPoint + q + "hours=" + Request.QueryString["hours"];
                                jsonString = await apiHelper.callAPIService("GET", endPoint, postData);

                                break;
                            }
                        case "getusagelogsumbyday":
                            {
                                endPoint = Global._usageLogSumByDayEndPoint;
                                string q = "?";
                                if (Request.QueryString["day"] != null)
                                {
                                    endPoint = endPoint + q + "days=" + Request.QueryString["day"];
                                    q = "&";
                                }
                              
                                 endPoint = endPoint + q + "order=desc";
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