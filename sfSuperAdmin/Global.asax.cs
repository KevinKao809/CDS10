using sfShareLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace sfSuperAdmin
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }

    public class Global
    {        
        private static string _sfAPIServiceBaseURI = ConfigurationManager.AppSettings["sfAPIServiceBaseURI"];
        public static string _sfSrvFabricBaseURI = ConfigurationManager.AppSettings["sfSrvFabricBaseURI"];
        public static string _sfAdminWebURI = ConfigurationManager.AppSettings["sfAdminWebURI"];
        public static string _sfAPIServiceTokenRole = ConfigurationManager.AppSettings["sfAPIServiceTokenRole"];
        public static string _sfAPIServiceTokenEndPoint = _sfAPIServiceBaseURI + "token";
        public static string _sfDocDBConnectionString = ConfigurationManager.AppSettings["sfDocDBConnectionString"];

        public static string _sbConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        public static string _sbProcessCommandTopic = ConfigurationManager.AppSettings["sfProcessCommandTopic"];
        public static string _sfServiceBusConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        public static string _sfInfraOpsQueue = ConfigurationManager.AppSettings["sfInfraOpsQueue"];
        public static string _sfAlarmOpsQueue = ConfigurationManager.AppSettings["sfAlarmOpsQueue"];
        
        static sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], logLevel);
        public static sfLog _sfAuditLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerAudit"], logLevel);

        /* Define Restful API EndPoint */
        public static string _companyEndPoint = _sfAPIServiceBaseURI + "admin-api/Company";
        public static string _employeeEndPoint = _sfAPIServiceBaseURI + "admin-api/Employee";
        public static string _externalDashboardEndPoint = _sfAPIServiceBaseURI + "admin-api/ExternalDashboard";
        public static string _userRoleEndPoint = _sfAPIServiceBaseURI + "admin-api/UserRole";
        public static string _cultureInfoEndPoint = _sfAPIServiceBaseURI + "admin-api/RefCultureInfo";
        public static string _superAdminEndPoint = _sfAPIServiceBaseURI + "admin-api/SuperAdmin";
        public static string _equipmentClassEndPoint = _sfAPIServiceBaseURI + "admin-api/EquipmentClass";
        public static string _widgetClassEndPoint = _sfAPIServiceBaseURI + "admin-api/WidgetClass";
        public static string _deviceClassEndPoint = _sfAPIServiceBaseURI + "admin-api/DeviceType";
        public static string _deviceConfigEndPoint = _sfAPIServiceBaseURI + "admin-api/IOTDeviceConfiguration";
        public static string _mandatoryMsgEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageMandatoryElementDef";
        public static string _permissionCatalogEndPoint = _sfAPIServiceBaseURI + "admin-api/PermissionCatalog";
        public static string _operationTaskEndPoint = _sfAPIServiceBaseURI + "admin-api/OperationTask";
        public static string _operationTaskSearchEndPoint = _sfAPIServiceBaseURI + "admin-api/OperationTask/search";
        public static string _iotHubEndPoint = _sfAPIServiceBaseURI + "admin-api/iothub";
        public static string _iotHubInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTHub/Company";
        public static string _deviceTypeEndPoint = _sfAPIServiceBaseURI + "admin-api/DeviceType";
        public static string _usageLogSumByDayEndPoint = _sfAPIServiceBaseURI + "admin-api/UsageLogSumByDay";
        
    }
}
