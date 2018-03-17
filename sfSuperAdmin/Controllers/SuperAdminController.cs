using Newtonsoft.Json;
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
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        public async Task<ActionResult> Index()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.SuperAdminList = await apiHelper.callAPIService("GET", Global._superAdminEndPoint, null);
                ViewBag.FirstName = Session["firstName"].ToString();
                ViewBag.LastName = Session["lastName"].ToString();
                ViewBag.Email = Session["email"].ToString();

                /* Setup Menu Item Active */
                ViewBag.MenuItem = "menuSuperAdmin";
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
            string jsonString = "";
            if (Request.QueryString["action"] != null)
            {
                string endPoint = Global._superAdminEndPoint;
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "getsuperadmin":
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        case "deletesuperadmin":
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        case "addsuperadmin":
                            {
                                string postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatesuperadmin":
                            {
                                //admin-api/SuperAdmin/{id}/
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                string postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);                                
                                break;
                            }
                        case "changepassword":
                            {
                                endPoint = Global._superAdminEndPoint + "/" + Request.QueryString["Id"] + "/changepassword";
                                string postData = Request.Form.ToString();
                                jsonString = apiHelper.changePassword("put", endPoint, postData);
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