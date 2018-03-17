using i18n;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Script.Serialization;

namespace sfAdmin.Models
{
    public class EmployeeSession
    {
        public int id;
        public int companyId;
        public string firstName;
        public string lastName;
        public string email;
        public string password;
        public string photoURL;
        public string permissions;
        public string navigationMenu;
        public string employeeNumber;
        public bool adminFlag;
        public string Lang;
        public string accessToken;
        public string issued;
        public string expires;
        public bool rememberMe;

        public string Serialize()
        {
            return new JavaScriptSerializer().Serialize(this);
        }

        public static EmployeeSession LoadByJsonString(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
                return null;

            return new JavaScriptSerializer().Deserialize<EmployeeSession>(jsonString);
        }
    }

    public class Employee
    {
        public EmployeeSession afterLoginInitial(string access_token, dynamic access_result)
        {
            EmployeeSession empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
            empSession.accessToken = access_token;
            if (access_result.CompanyId != null)
                empSession.companyId = access_result.CompanyId;
            empSession.photoURL = access_result.PhotoURL;
            empSession.id = access_result.Id;
            empSession.firstName = access_result.FirstName;
            empSession.lastName = access_result.LastName;
            empSession.email = access_result.Email;
            if (access_result.AdminFlag != null)
                empSession.adminFlag = bool.Parse((string)access_result.AdminFlag);
            else
                empSession.adminFlag = false;
            empSession.issued = access_result.issued;
            empSession.expires = access_result.expires;
            empSession.employeeNumber = access_result.EmployeeNumber;
            if (access_result.Lang != null)
                empSession.Lang = access_result.Lang;
            else
                empSession.Lang = "en";
            i18n.LanguageTag langTag = i18n.LanguageTag.GetCachedInstance(empSession.Lang);
            System.Web.HttpContext.Current.SetPrincipalAppLanguageForRequest(langTag);

            HttpContext.Current.Session["empSession"] = empSession.Serialize();

            StringBuilder logMessage = new StringBuilder();
            logMessage.AppendLine("audit: User Login Successful.");
            logMessage.AppendLine("email:" + empSession.email);
            Global._sfAuditLogger.Audit(logMessage);

            return empSession;
        }

        public void initialPermission(string permissionJson, string externalDashboardJson)
        {
            try
            {
                EmployeeSession empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
                bool isAdmin = empSession.adminFlag;
                if (HttpContext.Current.Session["loginBySA"] != null && bool.Parse(HttpContext.Current.Session["loginBySA"].ToString()))
                {
                    isAdmin = true;
                }
                dynamic permissions;
                string permissionString = "", navigationMenuString = "";
                List<int> permissionIds = new List<int>();

                if (!isAdmin)
                {
                    permissions = Json.Decode(permissionJson);
                    foreach (var permission in permissions)
                    {
                        permissionIds.Add((int)permission.PermissionId);
                        permissionString = permissionString + permission.PermissionId + ",";
                    }
                }
                else
                {
                    permissionString = "0,";
                }

                if (isAdmin || permissionIds.Find(item => item == 10) > 0)
                    navigationMenuString = navigationMenuString + "<li id=\"menuFactory\"><a href=/Factory/Index><i class=\"zmdi zmdi-windows\"></i> <span> [[[Factory]]] </span> </a></li>";

                if (isAdmin || permissionIds.Find(item => item == 20) > 0)
                    navigationMenuString = navigationMenuString + "<li id=\"menuEquipment\"><a href=\"/Equipment/Index\"><i class=\"ti-harddrives\"></i> <span> [[[Equipment]]] </span> </a></li>";

                if (isAdmin || permissionIds.Find(item => item == 30) > 0)
                {
                    navigationMenuString = navigationMenuString + "<li class=\"has-submenu\"  id=\"menuIoTDevice\"><span class=\"arrow-right\"></span><a href=\"#\"><i class=\"ti-signal\"></i><span> [[[IoT Device]]] </span> </a><ul class=\"submenu\"><li><a href=\"/IoTDevice/Index\">[[[IoT Devices]]]</a></li>";                    
                    if (isAdmin || permissionIds.Find(item => item == 34) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/IoTDevice/MessageAttach\">[[[Device Message]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 32) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/IoTDevice/ConfigurationManagement\">[[[Device Configuration]]]</a></li>";
                    navigationMenuString = navigationMenuString + "</ul></li>";
                }

                if (isAdmin || permissionIds.Find(item => item == 40) > 0)
                {
                    navigationMenuString = navigationMenuString + "<li class=\"has-submenu\"  id=\"menuMessage\"><span class=\"arrow-right\"></span><a href=\"#\"><i class=\"zmdi zmdi-calendar-note\"></i><span> [[[Message]]] </span> </a><ul class=\"submenu\"><li><a href=\"/Message/Index\">[[[Message Catalog]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 42) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Message/MessageElement\">[[[Message Element]]]</a></li>";
                    navigationMenuString = navigationMenuString + "</ul></li>";
                }

                if (isAdmin || permissionIds.Find(item => item >= 50 && item < 60) > 0)
                {
                    navigationMenuString = navigationMenuString + "<li class=\"has-submenu\" id=\"menuMonitor\"><span class=\"arrow-right\"></span><a href=\"#\"><i class=\"ti-desktop\"></i><span> [[[Operation]]] </span> </a><ul class=\"submenu\">";
                    if (isAdmin || permissionIds.Find(item => item == 50) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Monitor/RunningTask\">[[[Backend Task]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 51) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Monitor/IoTHubReceiver\">[[[IoT Hub Message Receiver]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 52) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Monitor/UsageLog\">[[[Usage Log]]]</a></li>";
                    navigationMenuString = navigationMenuString + "</ul></li>";
                }

                if (isAdmin || permissionIds.Find(item => item >= 60 && item < 70) > 0)
                {
                    navigationMenuString = navigationMenuString + "<li class=\"has-submenu\" id=\"menuDashboard\"><span class=\"arrow-right\"></span><a href=\"#\"><i class=\"zmdi zmdi-view-dashboard\"></i><span> [[[Dashboard]]] </span> </a><ul class=\"submenu\">";
                    if (isAdmin || permissionIds.Find(item => item == 60) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Dashboard/Index\" target=\"_blank\">[[[Company Board]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 61) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Dashboard/FactoryList\">[[[Factory Board]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 62) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Dashboard/EquipmentList\">[[[Equipment Board]]]</a></li>";


                    /* Append External Dashboard */
                    dynamic externalDashboards = Json.Decode(externalDashboardJson);
                    foreach (var extDashboard in externalDashboards)
                    {
                        string fullURL = extDashboard.URL;
                        if (!fullURL.StartsWith("http"))
                            fullURL = "http://" + fullURL;
                        navigationMenuString = navigationMenuString + "<li><a href=\"" + fullURL + "\" target=\"_blank\">" + extDashboard.Name + "</a></li>";
                    }
                    /* End of Append External Dashboard */

                    navigationMenuString = navigationMenuString + "</ul></li>";
                }

                if (isAdmin || permissionIds.Find(item => item >= 100 && item < 200) > 0)
                {
                    navigationMenuString = navigationMenuString + "<li class=\"has-submenu\" id=\"menuSetup\"><span class=\"arrow-right\"></span><a href=\"#\"><i class=\"ti-settings m-r-5\"></i><span> [[[Setup]]] </span> </a><ul class=\"submenu\">";

                    /* Remove IoT Hub Alias from Admin Console  */
                    //if (isAdmin || permissionIds.Find(item => item == 103) > 0)
                    //    navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/IoTHubAlias\">[[[IoT Hub Alias]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 110) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/EquipmentClass\">[[[Equipment Class]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 111) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/EquipmentMetadata\">[[[Equipment Meta Data]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 100) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/AlarmRuleCatalog\">[[[Alarm Rule Catalog]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 107) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/ExternalApplication\">[[[External Application]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 101) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/AlarmNotify\">[[[Alarm Action]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 102) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/DeviceCertificate\">[[[Device Certificate]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 109) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/DeviceConfiguration\">[[[Customize Device Configuration]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 108) > 0)
                    {
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/WidgetCatalogCompany\">[[[Widget Catalog - Company]]]</a></li>";
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/WidgetCatalogFactory\">[[[Widget Catalog - Factory]]]</a></li>";
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/WidgetCatalogEquipment\">[[[Widget Catalog - Equipment]]]</a></li>";
                    }
                    //if (isAdmin || permissionIds.Find(item => item == 106) > 0)
                    //{
                        /* Company and Factory are retried after offer drag and drop dashboard */
                        //navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/DashboardCompany\">[[[Dashboard - Company]]]</a></li>";
                        //navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/DashboardFactory\">[[[Dashboard - Factory]]]</a></li>";
                        /* Using SQL Trigger to insert EquipmentDashboard */
                        //navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/DashboardEquipment\">[[[Dashboard - Equipment]]]</a></li>";
                    //}
                    if (isAdmin || permissionIds.Find(item => item == 104) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/Employee\">[[[Employee]]]</a></li>";
                    if (isAdmin || permissionIds.Find(item => item == 105) > 0)
                        navigationMenuString = navigationMenuString + "<li><a href=\"/Setup/Role\">[[[Role]]]</a></li>";
                    navigationMenuString = navigationMenuString + "</ul></li>";
                }

                if (permissionString.Length > 0)
                    permissionString = permissionString.Substring(0, permissionString.Length - 1);
                empSession.permissions = permissionString;
                empSession.navigationMenu = navigationMenuString;
                HttpContext.Current.Session["empSession"] = empSession.Serialize();
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Exception on initialPermission. Message:" + ex.Message);
                Global._sfAppLogger.Error(logMessage);
            }            
        }
        //public void initialLang(string empLang)
        //{
        //    string lang = "en";            
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(empLang))
        //            lang = empLang;
        //        HttpContext.Current.Session["Lang"] = lang;                

        //        i18n.LanguageTag langTag = i18n.LanguageTag.GetCachedInstance(lang);
        //        System.Web.HttpContext.Current.SetPrincipalAppLanguageForRequest(langTag);
        //    }
        //    catch (Exception ex)
        //    {
        //        StringBuilder logMessage = new StringBuilder();
        //        logMessage.AppendLine("Exception on initialPermission. Message:" + ex.Message);
        //        Global._sfAppLogger.Error(logMessage);
        //    }
        //}

        public string getRedirectionPath()
        {
            EmployeeSession empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
            bool isAdmin = empSession.adminFlag;
            if (HttpContext.Current.Session["loginBySA"] != null && bool.Parse(HttpContext.Current.Session["loginBySA"].ToString()))
            {
                isAdmin = true;
            }
            if (isAdmin)
                return "Factory/Index";

            List<string> permissionList = empSession.permissions.Split(',').ToList();
            if (permissionList == null)
            {
                LoginMsgSession loginMsgSession = new LoginMsgSession();
                loginMsgSession.toastLevel = "warning";
                loginMsgSession.message = "You Don't Have Permission";
                HttpContext.Current.Session["loginMsgSession"] = loginMsgSession.Serialize();
                return "Home/Index";
            }

            string reController = "", reAction = "Index";
            permissionList.Sort();

            switch (int.Parse(permissionList[0]))
            {
                case 10:
                    reController = "Factory";
                    break;
                case 20:
                    reController = "Equipment";
                    break;
                case 30:
                    reController = "IoTDevice";
                    break;
                case 40:
                    reController = "Message";
                    break;
                case 50:
                case 51:
                case 52:
                    reController = "Monitor";
                    switch (int.Parse(permissionList[0]))
                    {
                        case 50:
                            reAction = "RunningTask";
                            break;
                        case 51:
                            reAction = "IoTHubReceiver";
                            break;
                        case 52:
                            reAction = "UsageLog";
                            break;
                    }
                    break;
                case 60:
                case 61:
                case 62:
                case 63:
                case 64:
                case 65:
                    reController = "Dashboard";
                    switch (int.Parse(permissionList[0]))
                    {                        
                        case 62:
                            reAction = "EquipmentList";
                            break;
                        default:
                            reAction = "FactoryList";
                            break;
                    }
                    break;
                case 100:
                case 101:
                case 102:
                case 103:
                case 104:
                case 105:
                case 106:
                case 107:
                case 108:
                case 109:
                case 110:
                    reController = "Setup";
                    switch (int.Parse(permissionList[0]))
                    {
                        case 100:
                            reAction = "AlarmRuleCatalog";
                            break;
                        case 101:
                            reAction = "AlarmNotify";
                            break;
                        case 102:
                            reAction = "DeviceCertificate";
                            break;
                        //case 103:
                        //    reAction = "IoTHubAlias";
                        //    break;
                        case 104:
                            reAction = "Employee";
                            break;
                        case 105:
                            reAction = "Role";
                            break;
                        //case 106:
                        //    reAction = "DashboardEquipment";
                        //    break;
                        case 107:
                            reAction = "ExternalApplication";
                            break;
                        case 108:
                            reAction = "WidgetCatalogCompany";
                            break;
                        case 109:
                            reAction = "DeviceConfiguration";
                            break;
                        case 110:
                            reAction = "EquipmentClass";
                            break;
                    }
                    break;
            }            
            return reController + "/" + reAction;            
        }
    }
}