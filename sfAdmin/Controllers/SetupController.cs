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
    public class SetupController : Controller
    {
        // GET: Setup

        private string getMessageOutputDefaultTemplate(string mandatoryMessageElements)
        {
            string result = "{";     
            dynamic messageElements = JsonConvert.DeserializeObject(mandatoryMessageElements);
            foreach (var msgElement in messageElements)
            {
                result = result + "\"" + msgElement.ElementName + "\":\"@" + msgElement.ElementName + "@\",";
            }
            result = result.Substring(0,result.Length-1) + "}";
            return result;
        }

        public async Task<ActionResult> AlarmRuleCatalog()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);
                ViewBag.AlarmRuleCatalogList = await apiHelper.callAPIService("GET", Global._alarmRuleCatalogInCompanyEndPoint, null);                

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> AlarmNotify()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);
                ViewBag.AlarmRuleCatalogList = await apiHelper.callAPIService("GET", Global._alarmRuleCatalogInCompanyEndPoint, null);
                ViewBag.ExternalApplicationList = await apiHelper.callAPIService("GET", Global._externalApplicationInCompanyEndPoint, null);
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
                ViewBag.MenuItem = "menuSetup";
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

        //public async Task<ActionResult> DashboardCompany()
        //{
        //    ViewBag.Version = Global._sfAdminVersion;
        //    EmployeeSession empSession = null;
        //    if (Session["empSession"] != null)
        //        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
        //    try
        //    {
        //        /* Get JSON entiries that Pages need */
        //        RestfulAPIHelper apiHelper = new RestfulAPIHelper();
        //        ViewBag.CompanyDashboardList = await apiHelper.callAPIService("GET", Global._dashboardInCompanyEndPoint + "?type=company", null);
        //        ViewBag.WidgetCatalogList = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=company", null);
        //        ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=company", null);

        //        /* Setup Company Name and Company Photo on Page */
        //        CompanyModel companyModel = new CompanyModel();
        //        CompanySession compSession = await companyModel.GetCompanySessionData();
        //        ViewBag.CompanyId = compSession.id;
        //        ViewBag.CompanyName = compSession.shortName;
        //        ViewBag.CompanyPhotoURL = compSession.photoURL;

        //        /* Setup Employee Data on Page */
        //        ViewBag.FirstName = empSession.firstName;
        //        ViewBag.LastName = empSession.lastName;
        //        ViewBag.Email = empSession.email;
        //        ViewBag.PhotoURL = empSession.photoURL;
        //        ViewBag.PermissionList = empSession.permissions;

        //        /* Setup Menu Item Active */
        //        ViewBag.MenuNavigation = empSession.navigationMenu;
        //        ViewBag.MenuItem = "menuSetup";
        //    }
        //    catch (Exception ex)
        //    {
        //        LoginMsgSession loginMsgSession = new LoginMsgSession();
        //        if (ex.Message.ToLower() == "invalid session")
        //        {
        //            loginMsgSession.toastLevel = "warning";
        //            loginMsgSession.message = "[[[Please Login]]]";
        //        }
        //        else
        //        {
        //            loginMsgSession.toastLevel = "error";
        //            loginMsgSession.message = "[[[Authentication Fail]]].";
        //            StringBuilder logMessage = new StringBuilder();
        //            logMessage.AppendLine("audit: Authentication Fail.");
        //            logMessage.AppendLine("email:" + empSession.email);
        //            logMessage.AppendLine("password:" + empSession.password);
        //            Global._sfAuditLogger.Audit(logMessage);
        //        }
        //        Session["loginMsgSession"] = loginMsgSession.Serialize();
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        //public async Task<ActionResult> DashboardFactory()
        //{
        //    ViewBag.Version = Global._sfAdminVersion;
        //    EmployeeSession empSession = null;
        //    if (Session["empSession"] != null)
        //        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
        //    try
        //    {
        //        /* Get JSON entiries that Pages need */
        //        RestfulAPIHelper apiHelper = new RestfulAPIHelper();
        //        ViewBag.FactoryDashboardList = await apiHelper.callAPIService("GET", Global._dashboardInCompanyEndPoint + "?type=factory", null);
        //        ViewBag.WidgetCatalogList = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=factory", null);
        //        ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=factory", null);

        //        /* Setup Company Name and Company Photo on Page */
        //        CompanyModel companyModel = new CompanyModel();
        //        CompanySession compSession = await companyModel.GetCompanySessionData();
        //        ViewBag.CompanyId = compSession.id;
        //        ViewBag.CompanyName = compSession.shortName;
        //        ViewBag.CompanyPhotoURL = compSession.photoURL;

        //        /* Setup Employee Data on Page */
        //        ViewBag.FirstName = empSession.firstName;
        //        ViewBag.LastName = empSession.lastName;
        //        ViewBag.Email = empSession.email;
        //        ViewBag.PhotoURL = empSession.photoURL;
        //        ViewBag.PermissionList = empSession.permissions;

        //        /* Setup Menu Item Active */
        //        ViewBag.MenuNavigation = empSession.navigationMenu;
        //        ViewBag.MenuItem = "menuSetup";
        //    }
        //    catch (Exception ex)
        //    {
        //        LoginMsgSession loginMsgSession = new LoginMsgSession();
        //        if (ex.Message.ToLower() == "invalid session")
        //        {
        //            loginMsgSession.toastLevel = "warning";
        //            loginMsgSession.message = "[[[Please Login]]]";
        //        }
        //        else
        //        {
        //            loginMsgSession.toastLevel = "error";
        //            loginMsgSession.message = "[[[Authentication Fail]]].";
        //            StringBuilder logMessage = new StringBuilder();
        //            logMessage.AppendLine("audit: Authentication Fail.");
        //            logMessage.AppendLine("email:" + empSession.email);
        //            logMessage.AppendLine("password:" + empSession.password);
        //            Global._sfAuditLogger.Audit(logMessage);
        //        }
        //        Session["loginMsgSession"] = loginMsgSession.Serialize();
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        //public async Task<ActionResult> DashboardEquipment()
        //{
        //    ViewBag.Version = Global._sfAdminVersion;
        //    EmployeeSession empSession = null;
        //    if (Session["empSession"] != null)
        //        empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
        //    try
        //    {
        //        /* Get JSON entiries that Pages need */
        //        RestfulAPIHelper apiHelper = new RestfulAPIHelper();
        //        ViewBag.EquipmentDashboardList = await apiHelper.callAPIService("GET", Global._equipmentClassDashboardInCompanyEndPoint, null);                
        //        //ViewBag.WidgetCatalogList = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=equipment", null);
        //        //ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=equipment", null);

        //        /* Setup Company Name and Company Photo on Page */
        //        CompanyModel companyModel = new CompanyModel();
        //        CompanySession compSession = await companyModel.GetCompanySessionData();
        //        ViewBag.CompanyId = compSession.id;
        //        ViewBag.CompanyName = compSession.shortName;
        //        ViewBag.CompanyPhotoURL = compSession.photoURL;

        //        /* Setup Employee Data on Page */
        //        ViewBag.FirstName = empSession.firstName;
        //        ViewBag.LastName = empSession.lastName;
        //        ViewBag.Email = empSession.email;
        //        ViewBag.PhotoURL = empSession.photoURL;
        //        ViewBag.PermissionList = empSession.permissions;

        //        /* Setup Menu Item Active */
        //        ViewBag.MenuNavigation = empSession.navigationMenu;
        //        ViewBag.MenuItem = "menuSetup";
        //    }
        //    catch (Exception ex)
        //    {
        //        LoginMsgSession loginMsgSession = new LoginMsgSession();
        //        if (ex.Message.ToLower() == "invalid session")
        //        {
        //            loginMsgSession.toastLevel = "warning";
        //            loginMsgSession.message = "[[[Please Login]]]";
        //        }
        //        else
        //        {
        //            loginMsgSession.toastLevel = "error";
        //            loginMsgSession.message = "[[[Authentication Fail]]].";
        //            StringBuilder logMessage = new StringBuilder();
        //            logMessage.AppendLine("audit: Authentication Fail.");
        //            logMessage.AppendLine("email:" + empSession.email);
        //            logMessage.AppendLine("password:" + empSession.password);
        //            Global._sfAuditLogger.Audit(logMessage);
        //        }
        //        Session["loginMsgSession"] = loginMsgSession.Serialize();
        //        return RedirectToAction("Index", "Home");
        //    }

        //    return View();
        //}

        public async Task<ActionResult> EquipmentClass()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.Equipment = await apiHelper.callAPIService("GET", Global._equipmentClassInCompanyEndPoint, null);                

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> EquipmentMetadata()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.EquipmentMetadata = await apiHelper.callAPIService("GET", Global._equipmentMetadataInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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


        public async Task<ActionResult>  ExternalApplication()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.ExternalApplicationList = await apiHelper.callAPIService("GET", Global._externalApplicationInCompanyEndPoint, null);
                ViewBag.IoTDeviceList = await apiHelper.callAPIService("GET", Global._iotDeviceInCompanyEndPoint, null);

                string jsonString = await apiHelper.callAPIService("GET", Global._mandatoryMsgEndPoint, null);
                //ViewBag.MessageOutputDefaultValue = getMessageOutputDefaultTemplate(jsonString);
                ViewBag.MessageOutputDefaultValue = "{\"AlarmRuleCatalogName\":\"@AlarmRuleCatalogName@\",\"AlarmRuleCatalogDescription\":\"@AlarmRuleCatalogDescription@\",\"companyId\":\"@companyId@\",\"msgTimestamp\":\"@msgTimestamp@\",\"equipmentId\":\"@equipmentId@\",\"equipmentRunStatus\":\"@equipmentRunStatus@\"}";


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
                ViewBag.MenuItem = "menuSetup";
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


        public async Task<ActionResult> Employee()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.EmployeeList = await apiHelper.callAPIService("GET", Global._employeeInCompanyEndPoint, null);
                ViewBag.UserRoleList = await apiHelper.callAPIService("GET", Global._userRoleInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> Role()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.RoleList = await apiHelper.callAPIService("GET", Global._userRoleInCompanyEndPoint, null);
                ViewBag.PermissionCatlog = await apiHelper.callAPIService("GET", Global._permissionCatlogEndPoint, null);
                
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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> IoTHubAlias()
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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> WidgetCatalogCompany()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                String WidgetCatalog = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=company", null);
                ViewBag.WidgetCatalogList = WidgetCatalog.Replace("\r\n", "").Replace("\\\\", "\\\\\\\\");

                ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=company", null);
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> WidgetCatalogFactory()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                String WidgetCatalog = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=factory", null);
                ViewBag.WidgetCatalogList = WidgetCatalog.Replace("\r\n", "").Replace("\\\\", "\\\\\\\\");

                ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=factory", null);
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> WidgetCatalogEquipment()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                String WidgetCatalog = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=equipment", null);
                ViewBag.WidgetCatalogList = WidgetCatalog.Replace("\r\n", "").Replace("\\\\", "\\\\\\\\");

                ViewBag.WidgetClassList = await apiHelper.callAPIService("GET", Global._widgetClassEndPoint + "?level=equipment", null);
                ViewBag.MessageCatalogList = await apiHelper.callAPIService("GET", Global._nonChildMsgInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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

        public async Task<ActionResult> DeviceCertificate()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.DeviceCertificateList = await apiHelper.callAPIService("GET", Global._deviceCertificateInCompanyEndPoint, null);

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
                ViewBag.MenuItem = "menuSetup";
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
        public async Task<ActionResult> DeviceConfiguration()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                /* Get JSON entiries that Pages need */
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.DeviceConfigInCompanyList = await apiHelper.callAPIService("GET", Global._deviceConfigInCompanyEndPoint, null);
               

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
                ViewBag.MenuItem = "menuSetup";
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
                    switch (Request.QueryString["action"].ToString().ToLower())
                    {
                        //Device Configuration
                        case "getdeviceconfigincompany":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._deviceConfigInCompanyEndPoint, null);
                                break;

                            }
                        case "deletedeviceconfig":
                            {
                                endPoint = Global._deviceConfigEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "adddeviceconfig":
                            {
                                endPoint = Global._deviceConfigEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatedeviceconfig":
                            {
                                endPoint = Global._deviceConfigEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Alarm Rule Catalog
                        case "getalarmrulecatalog":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._alarmRuleCatalogInCompanyEndPoint, null);
                                break;

                            }
                        case "deletealarmrulecatalog":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "addalarmrulecatalog":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatealarmrulecatalog":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        case "getrulebyid":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Rules";

                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "changerules":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Rules";
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        case "getmessageelement":
                            {
                                endPoint = Global._messageElementEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/MessageCatalog/" + Request.QueryString["Id"];

                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "getmessageflatelement":
                            {
                                endPoint = Global._messageFlatElementEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Elements";

                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        //AlarmNotify
                        case "getrulenotificationbyid":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Notification";

                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "updaterulenotification":
                            {
                                endPoint = Global._alarmRuleCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Notification";
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Widget Catalog
                        case "getwidgetcatalog":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=equipment", null);
                                break;
                            }
                        case "getcompanywidgetcatalog":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=company", null);
                                break;
                            }
                        case "getfactorywidgetcatalog":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=factory", null);
                                break;
                            }
                        case "getequipmentwidgetcatalog":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=equipment", null);
                                break;
                            }
                        case "deletewidgetcatalog":
                            {
                                endPoint = Global._widgetCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "addwidgetcatalog":
                            {
                                endPoint = Global._widgetCatalogEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatewidgetcatalog":
                            {
                                endPoint = Global._widgetCatalogEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Unvalidated.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }                        
                        // Equipment Dashboard, Company Dashboard
                        case "createequipmentclassdashboard":
                            {
                                endPoint = Global._dashboardEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "getequipmentclassdashboard":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._equipmentClassDashboardInCompanyEndPoint, null);
                                break;
                            }
                        case "getcompanydashboard":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._dashboardInCompanyEndPoint + "?type=company", null);
                                break;
                            }
                        case "getfactorydashboard":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._dashboardInCompanyEndPoint + "?type=factory", null);
                                break;
                            }
                        case "getdashboardwidgetbyid":
                            {
                                endPoint = Global._widgetInDashboardEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "addwidgetintodashboard":
                            {
                                endPoint = Global._dashboardWidgetEndPoint;
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "deletwidgetfromdashboard":
                            {
                                endPoint = Global._dashboardWidgetEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "updatewidgetindashboard":
                            {
                                endPoint = Global._dashboardWidgetEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //External Application
                        case "getexternalapplication":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._externalApplicationInCompanyEndPoint, null);
                                break;
                            }
                        case "deleteexternalapplication":
                            {
                                endPoint = Global._externalApplicationEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "addexternalapplication":
                            {
                                endPoint = Global._externalApplicationEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updateexternalapplication":
                            {
                                endPoint = Global._externalApplicationEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                            //User Role
                        case "getuserrole":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._userRoleInCompanyEndPoint, null);
                                break;
                            }
                        case "deleteuserrole":
                            {
                                endPoint = Global._userRoleEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "adduserrole":
                            {
                                endPoint = Global._userRoleEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updateuserrole":
                            {
                                endPoint = Global._userRoleEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //IoT Hub Alias
                        //case "getiothub":
                        //    {
                        //        endPoint = Global._iotHubInCompanyEndPoint;
                        //        if (Request.QueryString["Id"] != null)
                        //            endPoint = endPoint + "/" + Request.QueryString["Id"];
                        //        jsonString = await apiHelper.callAPIService("get", endPoint, null);
                        //        break;
                        //    }
                        //case "deleteiothub":
                        //    {
                        //        endPoint = Global._iotHubEndPoint;
                        //        if (Request.QueryString["Id"] != null)
                        //            endPoint = endPoint + "/" + Request.QueryString["Id"];
                        //        jsonString = await apiHelper.callAPIService("get", endPoint, null);
                        //        dynamic jsonResult = JObject.Parse(jsonString);
                        //        if (jsonResult.IoTHubAlias != null)
                        //        {
                        //            string IoTHubAlias = jsonResult.IoTHubAlias;
                        //            /* Send message to OpsInfra to remove IoTHub Receiver */
                        //            OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "remove iothub alias", 0, empSession.firstName + " " + empSession.lastName, empSession.email);
                        //            OpsTaskModel opsTask = new OpsTaskModel("Remove IoTHub Receiver", empSession.companyId, "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
                        //            postData = opsTask.GetPostData();
                        //            string taskEndPoint = Global._operationTaskEndPoint;
                        //            jsonString = await apiHelper.callAPIService("POST", taskEndPoint, postData);
                        //            jsonResult = JObject.Parse(jsonString);
                        //            if (jsonResult.id != null)
                        //            {
                        //                opsInfraMessage.taskId = jsonResult.id;
                        //                opsInfraMessage.Send();
                        //            }

                        //            /* Call Restful API to delete  */
                        //            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                        //        }
                        //        break;
                        //    }
                        //case "addiothub":
                        //    {
                        //        endPoint = Global._iotHubEndPoint;
                        //        postData = Request.Form.ToString();
                        //        postData = postData + "&CompanyId=" + empSession.companyId;
                        //        jsonString = await apiHelper.callAPIService("post", endPoint, postData);

                        //        /* Send Message to OpsInfra to launch IoTHub Receiver */
                        //        string IoTHubAlias = Request.Form["IoTHubAlias"];
                        //        OpsInfraMessage opsInfraMessage = new OpsInfraMessage("provisioning iothub alias", "IoTHubAlias", IoTHubAlias, "create iothub alias", 0, empSession.firstName + " " + empSession.lastName, empSession.email);
                        //        OpsTaskModel opsTask = new OpsTaskModel("Launch IoTHub Receiver", empSession.companyId, "IoTHubAlias", IoTHubAlias, opsInfraMessage.GetJsonContent());
                        //        postData = opsTask.GetPostData();
                        //        string taskEndPoint = Global._operationTaskEndPoint;
                        //        jsonString = await apiHelper.callAPIService("POST", taskEndPoint, postData);
                        //        dynamic jsonResult = JObject.Parse(jsonString);
                        //        if (jsonResult.id != null)
                        //        {
                        //            opsInfraMessage.taskId = jsonResult.id;
                        //            opsInfraMessage.Send();
                        //        }
                        //        break;
                        //    }
                        //case "updateiothub":
                        //    {
                        //        endPoint = Global._iotHubEndPoint;
                        //        if (Request.QueryString["Id"] != null)
                        //            endPoint = endPoint + "/" + Request.QueryString["Id"];
                        //        postData = Request.Form.ToString();
                        //        postData = postData + "&CompanyId=" + empSession.companyId;
                        //        jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                        //        break;
                        //    }
                        //Device Certificate
                        case "getdevicecertificate":
                            {
                                endPoint = Global._deviceCertificateInCompanyEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("get", endPoint, null);
                                break;
                            }
                        case "deletedevicecertificate":
                            {
                                endPoint = Global._deviceCertificateEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "adddevicecertificate":
                            {
                                endPoint = Global._deviceCertificateEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "updatedevicecertificate":
                            {
                                endPoint = Global._deviceCertificateEndPoint;
                                if (Request.QueryString["Id"] != null)
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];

                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Equipment Meta data
                        case "getequipmetadata":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._equipmentMetadataInCompanyEndPoint, null);
                                break;
                            }
                        case "addequipdetadata":
                            {
                                endPoint = Global._equipmentMetadataEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&EntityType=equipment&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "deleteequipmetadata":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._equipmentMetadataEndPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "updateequipmetadata":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._equipmentMetadataEndPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                postData = postData + "&EntityType=equipment&CompanyId=" + empSession.companyId;
                                System.Diagnostics.Debug.Print("endPoint" + endPoint);
                                System.Diagnostics.Debug.Print("postData" + postData);
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Equipment Class
                        case "getequipclass":
                            {
                                jsonString = await apiHelper.callAPIService("GET", Global._equipmentClassInCompanyEndPoint, null);
                                break;
                            }
                        case "addequipclass":
                            {
                                endPoint = Global._equipmentClassEndPoint;
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                jsonString = await apiHelper.callAPIService("post", endPoint, postData);
                                break;
                            }
                        case "deleteequipclass":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._equipmentClassEndPoint + "/" + Request.QueryString["Id"];
                                jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                                break;
                            }
                        case "updateequipclass":
                            {
                                if (Request.QueryString["Id"] != null)
                                    endPoint = Global._equipmentClassEndPoint + "/" + Request.QueryString["Id"];
                                postData = Request.Form.ToString();
                                postData = postData + "&CompanyId=" + empSession.companyId;
                                System.Diagnostics.Debug.Print("endPoint" + endPoint);
                                System.Diagnostics.Debug.Print("postData" + postData);
                                jsonString = await apiHelper.callAPIService("put", endPoint, postData);
                                break;
                            }
                        //Employee
                        case "resetpassword":
                            endPoint = Global._employeeEndPoint + "/" + Request.QueryString["Id"] + "/ResetPassword";
                            postData = Request.Form.ToString();
                            jsonString = apiHelper.changePassword("put", endPoint, postData);
                            break;
                        case "getemployee":
                            endPoint = Global._employeeInCompanyEndPoint;
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);                            
                            break;
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
                                {
                                    postData = Request.Form.ToString();
                                    endPoint = endPoint + "/" + Request.QueryString["Id"];
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
                        case "deleteemployee":
                            endPoint = Global._employeeEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"];
                            jsonString = await apiHelper.callAPIService("delete", endPoint, null);
                            break;
                        //case "getemployeebycmp":
                        //    endPoint = Global._employeeEndPoint;
                        //    if (Request.QueryString["Id"] != null)
                        //        endPoint = endPoint + "/Company/" + Request.QueryString["Id"];
                        //    jsonString = await apiHelper.callAPIService("get", endPoint, null);
                        //    break;
                        case "getuserrolebyemployeeid":
                            endPoint = Global._employeeEndPoint;
                            if (Request.QueryString["Id"] != null)
                                endPoint = endPoint + "/" + Request.QueryString["Id"] + "/Role";
                            jsonString = await apiHelper.callAPIService("get", endPoint, null);
                            break;
                        //case "getuserrolebycompany":                            
                        //    jsonString = await apiHelper.callAPIService("get", Global._userRoleInCompanyEndPoint, null);
                        //    break;                        
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