using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sfAdmin.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public async Task<ActionResult> Index()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._messageInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuMessage";
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
        public async Task<ActionResult> MessageElement()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._messageInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuMessage";
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
            string jsonString = "";
            if (Request.QueryString["action"] != null)
            {
                string endPoint = Global._messageEndPoint;
                EmployeeSession empSession = null;
                if (Session["empSession"] != null)
                    empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "getmessagecatalog":
                            endPoint = Global._messageInCompanyEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        case "deletemessagecatalog":
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        case "addmessagecatalog":
                            {
                                string postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatemessagecatalog":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                string postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Message Element
                        case "getmessageelementbyid":
                            {
                                endPoint = Global._messageElementEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/MessageCatalog/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;

                            }
                        case "getchildmessagebyid":
                            {
                                endPoint = Global._messageEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;

                            }
                        case "addmessageelement":
                            {
                                string postData = Request.Form.ToString();
                                endPoint = Global._messageElementEndPoint;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;

                            }
                        case "deletemessageelement":
                            {
                                endPoint = Global._messageElementEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                string postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("delete", endPoint, postData);
                                break;

                            }
                        case "updatemessageelement":
                            {
                                endPoint = Global._messageElementEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                string postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
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
