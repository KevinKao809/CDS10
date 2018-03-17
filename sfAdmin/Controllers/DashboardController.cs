using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sfAdmin.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public async Task<ActionResult> Index()
        {
            ViewBag.Version = Global._sfAdminVersion;
            ViewBag.WidgetOutput = "<H2>Nothing Found.</H2>";
            ViewBag.GoogleMapAPIKey = Global._sfGoogleMapAPIKey;
            ViewBag.BaiduMapAPIKey = Global._sfBaiduMapAPIKey;
            CompanyModel companyModel = new CompanyModel();
            CompanySession compSession = await companyModel.GetCompanySessionData();

            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
                ViewBag.PermissionList = empSession.permissions;
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                List<ExpandoObject> FactoryEquipmentList = new List<ExpandoObject>();

                ViewBag.WidgetCatalogList = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=company", null);

                string FactoryString = await apiHelper.callAPIService("GET", Global._factoryInCompanyEndPoint, null);
                dynamic FactoryObjs = JsonConvert.DeserializeObject(FactoryString);

                /* Construct an New JSON String which contain all Factory and link to all equipment under each Factory;  */
                /* Using ExpandoObject to add couple new element into JSON object, which will be use on JavaScript */
                foreach (var factoryOjb in FactoryObjs)
                {
                    string equipmentInFactoryEndPoint = Global._equipmentInFactoryEndPoint + "/" + factoryOjb.Id;
                    string EquipmentString = await apiHelper.callAPIService("GET", equipmentInFactoryEndPoint, null);
                    dynamic EquipmentObjs = JsonConvert.DeserializeObject(EquipmentString);

                    ExpandoObject newFactoryObj = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(factoryOjb), new ExpandoObjectConverter());
                    AddExpandoObjectProperty(newFactoryObj, "alarm24H", 0);

                    List<ExpandoObject> Equipments = new List<ExpandoObject>();
                    foreach (var equipmentObj in EquipmentObjs)
                    {
                        ExpandoObject newEquipmentObj = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(equipmentObj), new ExpandoObjectConverter());
                        AddExpandoObjectProperty(newEquipmentObj, "msgTimestamp", "");
                        AddExpandoObjectProperty(newEquipmentObj, "alarmMsgTimestamp", "");
                        Equipments.Add(newEquipmentObj);
                    }

                    AddExpandoObjectProperty(newFactoryObj, "Equipments", Equipments);
                    FactoryEquipmentList.Add(newFactoryObj);
                }
                ViewBag.FactoryEquipmentList = JsonConvert.SerializeObject(FactoryEquipmentList);
                /* End JSON Construct */

                string AlarmMessageString = "[]";
                try
                {
                    string endPoint = Global._alarmMessageInCompanyEndPoint + "?hours=24&top=100&order=asc";
                    AlarmMessageString = await apiHelper.callAPIService("GET", endPoint, null);
                    dynamic alarmObjs = JsonConvert.DeserializeObject(AlarmMessageString);
                    ViewBag.AlarmMessageCount = alarmObjs.Count;
                    //AlarmMessageString = AlarmMessageString.Replace("\\\"", "");
                }
                catch (Exception ex)
                {
                    ViewBag.AlarmMessageCount = 0;
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("Error on retrieve AlarmMessage from DocDB");
                    logMessage.AppendLine("Exeption:" + ex.Message);
                    Global._sfAppLogger.Error(logMessage);
                }

                //ViewBag.AlarmMessageList = AlarmMessageString;

                /* Get Company Widget */
                string companyDashboardJson = await apiHelper.callAPIService("GET", Global._dashboardInCompanyEndPoint + "?type=company", null);
                try
                {
                    dynamic companyDashboardObj = JsonConvert.DeserializeObject(companyDashboardJson);
                    int DashboardId = (int)companyDashboardObj[0].Id;

                    if (DashboardId > 0)
                    {
                        string widgetJson = await apiHelper.callAPIService("GET", Global._widgetInDashboardEndPoint + "/" + DashboardId, null);
                        if (!string.IsNullOrEmpty(widgetJson))
                        {
                            DashboardModel dashboardModel = new DashboardModel();
                            dashboardModel.GenerateCompanyWidgetHTMLContent(widgetJson, FactoryString, compSession);
                            ViewBag.WidgetUpdateFunctions = dashboardModel.GetWidgetJavaScriptFunction();
                            ViewBag.WidgetOutput = dashboardModel.GetWidgetHTMLContent();
                            ViewBag.DashboardId = DashboardId;
                        }
                    }
                }
                catch (Exception)
                {
                    ;
                }

                /* Setup Company Name and Company Photo on Page */                
                ViewBag.CompanyId = compSession.id;
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = "";
                ViewBag.MenuItem = "";
            }
            catch (Exception ex)
            {
             //   EmployeeSession empSession = null;
               // if (Session["empSession"] != null)
                 //   empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
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


        public async Task<ActionResult> FactoryList()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
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
                ViewBag.MenuItem = "menuDashboard";
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

        public async Task<ActionResult> FactoryDashboard()
        {
            ViewBag.Version = Global._sfAdminVersion;            
            ViewBag.WidgetOutput = "<H2>Nothing Found.</H2>";
            ViewBag.GoogleMapAPIKey = Global._sfGoogleMapAPIKey;
            ViewBag.BaiduMapAPIKey = Global._sfBaiduMapAPIKey;

            CompanyModel companyModel = new CompanyModel();
            CompanySession compSession = await companyModel.GetCompanySessionData();

            EmployeeSession empSession = null;

            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());

            ViewBag.PermissionList = empSession.permissions;


            if (Request.QueryString["factoryId"] != null || Request.Form["factoryId"] != null)
            {
                string factoryId = Request.QueryString["factoryId"] != null ? Request.QueryString["factoryId"] : Request.Form["factoryId"];
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    string factoryString = await apiHelper.callAPIService("GET", Global._factoryEndPoint + "/" + factoryId, null);

                    ViewBag.Factory = factoryString;

                    string EquipmentString = await apiHelper.callAPIService("GET", Global._equipmentInFactoryEndPoint + "/" + factoryId, null);
                    dynamic EquipmentObjs = JsonConvert.DeserializeObject(EquipmentString);

                    String WidgetCatalog = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=factory", null);
                    ViewBag.WidgetCatalogList = WidgetCatalog.Replace("\r\n", "").Replace("\\\\", "\\\\\\\\");


                    /* Construct an New JSON String which contain all equipment under the Factory;  */
                    /* Using ExpandoObject to add couple new element into JSON object, which will be use on JavaScript */

                    List<ExpandoObject> Equipments = new List<ExpandoObject>();
                    foreach (var equipmentObj in EquipmentObjs)
                    {
                        ExpandoObject newEquipmentObj = JsonConvert.DeserializeObject<ExpandoObject>(JsonConvert.SerializeObject(equipmentObj), new ExpandoObjectConverter());
                        AddExpandoObjectProperty(newEquipmentObj, "msgTimestamp", "");
                        AddExpandoObjectProperty(newEquipmentObj, "alarmMsgTimestamp", "");
                        Equipments.Add(newEquipmentObj);
                    }

                    ViewBag.EquipmentList = JsonConvert.SerializeObject(Equipments);
                    /* End JSON Construct */

                    string AlarmMessageString = "[]";
                    try
                    {
                        string endPoint = Global._alarmMessageInFactoryEndPoint + "/" + factoryId + "?hours=24&top=100&order=asc";
                        AlarmMessageString = await apiHelper.callAPIService("GET", endPoint, null);
                        dynamic alarmObjs = JsonConvert.DeserializeObject(AlarmMessageString);
                        ViewBag.AlarmMessageCount = alarmObjs.Count;
                        //AlarmMessageString = AlarmMessageString.Replace("\\\"", "");
                    }
                    catch (Exception ex)
                    {
                        ViewBag.AlarmMessageCount = 0;
                        StringBuilder logMessage = new StringBuilder();
                        logMessage.AppendLine("Error on retrieve AlarmMessage from DocDB");
                        logMessage.AppendLine("Exeption:" + ex.Message);
                        Global._sfAppLogger.Error(logMessage);
                    }

                    //ViewBag.AlarmMessageList = AlarmMessageString;

                    /* Get Factory Widget */
                    string factoryDashboardJson = await apiHelper.callAPIService("GET", Global._factoryDashboard + "/" + factoryId, null);
                    try
                    {
                        dynamic factoryDashboardObj = JsonConvert.DeserializeObject(factoryDashboardJson);

                        int DashboardId = (int)factoryDashboardObj[0].Id;

                        if (DashboardId > 0)
                        {
                            string widgetJson = await apiHelper.callAPIService("GET", Global._widgetInDashboardEndPoint + "/" + DashboardId, null);
                            if (!string.IsNullOrEmpty(widgetJson))
                            {
                                DashboardModel dashboardModel = new DashboardModel();
                                dashboardModel.GenerateFactoryWidgetHTMLContent(widgetJson, factoryString,  compSession, EquipmentString);
                                ViewBag.WidgetOutput = dashboardModel.GetWidgetHTMLContent();
                                ViewBag.WidgetUpdateFunctions = dashboardModel.GetWidgetJavaScriptFunction();
                                ViewBag.DashboardId = DashboardId;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }


                    /* Setup Company Name and Company Photo on Page */                    
                    ViewBag.CompanyId = compSession.id;
                    ViewBag.CompanyName = compSession.shortName;
                    ViewBag.CompanyPhotoURL = compSession.photoURL;

                    /* Setup Menu Item Active */
                    ViewBag.MenuNavigation = "";
                    ViewBag.MenuItem = "";
                }
                catch (Exception ex)
                {
                   // EmployeeSession empSession = null;
                    //if (Session["empSession"] != null)
                      //  empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
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
            }
            return View();
        }

        public async Task<ActionResult> EquipmentList()
        {
            ViewBag.Version = Global._sfAdminVersion;
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                ViewBag.EquipmentList = await apiHelper.callAPIService("GET", Global._equipmentInCompanyEndPoint, null);

                /* Setup Company Name and Company Photo on Page */
                CompanyModel companyModel = new CompanyModel();
                CompanySession compSession = await companyModel.GetCompanySessionData();
                ViewBag.CompanyId = compSession.id;
                ViewBag.CompanyName = compSession.shortName;
                ViewBag.CompanyPhotoURL = compSession.photoURL;

                //* Setup Employee Data on Page */
                ViewBag.FirstName = empSession.firstName;
                ViewBag.LastName = empSession.lastName;
                ViewBag.Email = empSession.email;
                ViewBag.PhotoURL = empSession.photoURL;
                ViewBag.PermissionList = empSession.permissions;

                /* Setup Menu Item Active */
                ViewBag.MenuNavigation = empSession.navigationMenu;
                ViewBag.MenuItem = "menuDashboard";
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

        public async Task<ActionResult> EquipmentClassDashboard()
        {
            ViewBag.Version = Global._sfAdminVersion;
            int EquipmentClassId = 0, DashboardId = 0;
            string endPoint = "", EquipmentId = "";
            ViewBag.WidgetOutput = "<H2>Nothing Found.</H2>";
            ViewBag.WidgetUpdateFunctions = "";
            EmployeeSession empSession = null;
            if (Session["empSession"] != null)
                empSession = EmployeeSession.LoadByJsonString(Session["empSession"].ToString());
            ViewBag.PermissionList = empSession.permissions;

            if (Request.QueryString["equipmentId"] != null || Request.Form["equipmentId"] != null)
            {
                try
                {
                    RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                    if (Request.QueryString["equipmentId"] != null)
                        EquipmentId = Request.QueryString["equipmentId"].ToString();
                    else
                        EquipmentId = Request.Form["equipmentId"].ToString();
                    endPoint = Global._equipmentEndPoint + "/" + EquipmentId;
                    string equipmentString = await apiHelper.callAPIService("GET", endPoint, null);
                    endPoint = Global._dashboardInCompanyEndPoint + "?type=equipmentclass";
                    string equipmentClassDashboardJson = await apiHelper.callAPIService("GET", endPoint, null);
                    try
                    {
                        dynamic equipmentObj = JObject.Parse(equipmentString);
                        EquipmentClassId = equipmentObj.EquipmentClassId;
                        ViewBag.EquipmentId = (string)equipmentObj.EquipmentId;

                        dynamic dashboardObjs = JsonConvert.DeserializeObject(equipmentClassDashboardJson);
                        foreach (var dashboard in dashboardObjs)
                        {
                            if (dashboard.EquipmentClassId == EquipmentClassId)
                            {
                                DashboardId = dashboard.Id;
                                break;
                            }
                        }
                        if (DashboardId > 0)
                        {
                            ViewBag.WidgetCatalogList = await apiHelper.callAPIService("GET", Global._widgetCatalogInCompanyEndPoint + "?level=equipment", null);
                            ViewBag.DashboardId = DashboardId;
                            endPoint = Global._widgetInDashboardEndPoint + "/" + DashboardId;
                            string widgetJson = await apiHelper.callAPIService("GET", endPoint, null);
                            if (!string.IsNullOrEmpty(widgetJson))
                            {
                                DashboardModel dashboardModel = new DashboardModel();
                                dashboardModel.GenerateWidgetHTMLContent(equipmentString, widgetJson);
                                ViewBag.WidgetOutput = dashboardModel.GetWidgetHTMLContent();
                                ViewBag.WidgetUpdateFunctions = dashboardModel.GetWidgetJavaScriptFunction();
                                ViewBag.AlarmWidgetUpdateFunctions = dashboardModel.GetAlarmWidgetJavaScriptFunction();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ;
                    }
                    ViewBag.CompanyId = empSession.companyId;
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
            }

            return View();
        }

        public async Task<ActionResult> ReqAction()
        {
            string jsonString = "", endPoint = "", factoryId = "", equipmentId = "", dataRange = "24", top = "100";
            try
            {
                RestfulAPIHelper apiHelper = new RestfulAPIHelper();
                if (Request.QueryString["action"] != null)
                {
                    switch (Request.QueryString["action"].ToLower())
                    {
                        case "getcompanyalarm":
                            {
                                if (Request.QueryString["dataRange"] != null)
                                    dataRange = Request.QueryString["dataRange"];
                                endPoint = Global._alarmMessageInCompanyEndPoint + "?hours=" + dataRange + "&top=" + top + "&order=desc";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "getfactoryalarm":
                            {
                                factoryId = Request.QueryString["Id"];
                                if (Request.QueryString["dataRange"] != null)
                                    dataRange = Request.QueryString["dataRange"];
                                endPoint = Global._alarmMessageInFactoryEndPoint + "/" + factoryId + "?hours=" + dataRange + "&top=" + top + "&order=desc";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }
                        case "getequipmentalarm":
                            {
                                equipmentId = Request.QueryString["Id"];
                                if (Request.QueryString["dataRange"] != null)
                                    dataRange = Request.QueryString["dataRange"];
                                endPoint = Global._alarmMessageInEquipmentEndPoint + "/" + equipmentId + "?hours=" + dataRange + "&top=" + top + "&order=desc";
                                jsonString = await apiHelper.callAPIService("GET", endPoint, null);
                                break;
                            }                        
                    }
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
            return Content(JsonConvert.SerializeObject(jsonString), "application/json");
        }

        private void AddExpandoObjectProperty(ExpandoObject expando, string propertyName, object propertyValue)
        {
            // ExpandoObject supports IDictionary so we can extend it like this
            var expandoDict = expando as IDictionary<string, object>;
            if (expandoDict.ContainsKey(propertyName))
                expandoDict[propertyName] = propertyValue;
            else
                expandoDict.Add(propertyName, propertyValue);
        }
    }
}