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
    public class SetupController : Controller
    {
        // GET: Setup

        public async Task<ActionResult> WidgetClassCompany()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string endPoint = Global._widgetClassEndPoint + "?level=company";
                ViewBag.Widget = await apiHelper.callAPIService("GET", endPoint, null);

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

        public async Task<ActionResult> WidgetClassFactory()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string endPoint = Global._widgetClassEndPoint + "?level=factory";
                ViewBag.Widget = await apiHelper.callAPIService("GET", endPoint, null);

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

        public async Task<ActionResult> WidgetClassEquipment()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                string endPoint = Global._widgetClassEndPoint + "?level=equipment";
                ViewBag.Widget = await apiHelper.callAPIService("GET", endPoint, null);

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

        public async Task<ActionResult> EquipmentClass()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.Equipment = await apiHelper.callAPIService("GET", Global._equipmentClassEndPoint, null);
                ViewBag.CompanyList = await apiHelper.callAPIService("GET", Global._companyEndPoint, null);

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

        public async Task<ActionResult> DeviceClass()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.DeviceClass= await apiHelper.callAPIService("GET", Global._deviceClassEndPoint, null);

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

        public async Task<ActionResult> DeviceConfigurationCatalog()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.DeviceConfig = await apiHelper.callAPIService("GET", Global._deviceConfigEndPoint, null);

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

        public async Task<ActionResult> MandatoryMessageElement()
        {            
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.MandatoryMsgList = await apiHelper.callAPIService("GET", Global._mandatoryMsgEndPoint, null);

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

        public async Task<ActionResult> PermissionCatalog()
        {
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.PermissionCatalogList = await apiHelper.callAPIService("GET", Global._permissionCatalogEndPoint, null);

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
            if (Request.QueryString["action"] != null)
            {
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        case "addwidgetclass":
                            endPoint = Global._widgetClassEndPoint;
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                            break;                        
                        case "updatewidgetclass":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._widgetClassEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            System.Diagnostics.Debug.Print("endPoint" + endPoint);
                            System.Diagnostics.Debug.Print("postData" + postData);
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        case "getequipmentwidgetclass":
                            jsonString = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=equipment", null);
                            break;
                        case "getcompanywidgetclass":
                            jsonString = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=company", null);
                            break;
                        case "getfactorywidgetclass":
                            jsonString = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=factory", null);
                            break;

                        case "addequipclass":
                            endPoint = Global._equipmentClassEndPoint;
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                            break;
                        case "deleteequipclass":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._equipmentClassEndPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        case "updateequipclass":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._equipmentClassEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            System.Diagnostics.Debug.Print("endPoint" + endPoint);
                            System.Diagnostics.Debug.Print("postData" + postData);
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        case "getequipmentclass":
                            jsonString = await apiHelper.callAPIService("GET", Global._equipmentClassEndPoint, null);
                            break;
                        case "getdeviceclass":
                            jsonString = await apiHelper.callAPIService("GET", Global._deviceClassEndPoint, null);
                            break;
                        case "deletedeviceclass":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._deviceClassEndPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        case "adddeviceclass":
                            endPoint = Global._deviceClassEndPoint;
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                            break;
                        case "updatedeviceclass":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._deviceClassEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            //System.Diagnostics.Debug.Print("endPoint" + endPoint);
                            //System.Diagnostics.Debug.Print("postData" + postData);
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        //Device Configuration
                        case "getdeviceconfig":
                            jsonString = await apiHelper.callAPIService("GET", Global._deviceConfigEndPoint, null);
                            break;
                        case "updatedeviceconfig":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._deviceConfigEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        case "adddeviceconfig":
                            endPoint = Global._deviceConfigEndPoint;
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                            break;
                        case "deletedeviceconfig":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._deviceConfigEndPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        //MandatoryMsg
                        case "getmandatorymsg":
                            jsonString = await apiHelper.callAPIService("GET", Global._mandatoryMsgEndPoint, null);
                            break;
                        case "addmandatorymsg":
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", Global._mandatoryMsgEndPoint, postData);
                            break;
                        case "updatemandatorymsg":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._mandatoryMsgEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        case "deletemandatorymsg":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._mandatoryMsgEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        //PermissionCatalog
                        case "getpermissioncatalog":
                            jsonString = await apiHelper.callAPIService("GET", Global._permissionCatalogEndPoint, null);
                            break;
                        case "addpermissioncatalog":
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("post", Global._permissionCatalogEndPoint, postData);
                            break;
                        case "updatepermissioncatalog":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._permissionCatalogEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                            break;
                        case "deletepermissioncatalog":
                            if (Request.QueryString["Id"] != null)
                                endPoint = Global._permissionCatalogEndPoint + "/" + Request.QueryString["Id"];
                            postData = Request.Form.ToString();
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
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