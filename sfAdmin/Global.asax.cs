using sfAdmin.Models;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace sfAdmin
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

        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection && Context.Request.Url.Host == Global._sfSecureDomain)
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));            
        }
    }

    public class Global
    {
        public static string _sfAdminVersion = ConfigurationManager.AppSettings["sfAdminVersion"];
        public static string _sfSecureDomain = ConfigurationManager.AppSettings["sfSecureDomain"];

        private static string _sfAPIServiceBaseURI = ConfigurationManager.AppSettings["sfAPIServiceBaseURI"];
        public static string _sfAPIServiceTokenRole = ConfigurationManager.AppSettings["sfAPIServiceTokenRole"];
        public static string _sfAPIServiceTokenEndPoint = _sfAPIServiceBaseURI + "token";

        public static string _sfProcessCommandTopic = ConfigurationManager.AppSettings["sfProcessCommandTopic"];

        public static string _sfServiceBusConnectionString = ConfigurationManager.AppSettings["sfServiceBusConnectionString"];
        public static string _sfInfraOpsQueue = ConfigurationManager.AppSettings["sfInfraOpsQueue"];

        static sfLogLevel logLevel = (sfLogLevel)Enum.Parse(typeof(sfLogLevel), ConfigurationManager.AppSettings["sfLogLevel"]);
        public static sfLog _sfAppLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerApp"], logLevel);
        public static sfLog _sfAuditLogger = new sfLog(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"], ConfigurationManager.AppSettings["sfLogStorageContainerAudit"], logLevel);

        public static string _sfGoogleMapAPIKey = ConfigurationManager.AppSettings["sfGoogleMapAPIKey"];
        public static string _sfBaiduMapAPIKey = ConfigurationManager.AppSettings["sfBaiduMapAPIKey"];

        /* Define Restful API EndPoint */
        public static string _companyEndPoint = _sfAPIServiceBaseURI + "admin-api/Company/{companyId}";
        public static string _companyAllowDomainEndPoint = _sfAPIServiceBaseURI + "admin-api/Company/{companyId}/AllowDomain";
        public static string _factoryEndPoint = _sfAPIServiceBaseURI + "admin-api/Factory";
        public static string _factoryInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/Factory/Company/{companyId}";
        public static string _equipmentEndPoint = _sfAPIServiceBaseURI + "admin-api/Equipment";
        public static string _equipmentInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/equipment/Company/{companyId}";
        public static string _equipmentInFactoryEndPoint = _sfAPIServiceBaseURI + "admin-api/equipment/Factory";
        public static string _iotDeviceEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTDevice";
        public static string _iotDeviceInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTDevice/Company/{companyId}";
        public static string _iotDeviceInFactoryEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTDevice/Factory";

        public static string _alarmRuleCatalogEndPoint = _sfAPIServiceBaseURI + "admin-api/AlarmRuleCatalog";
        public static string _alarmRuleCatalogInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/AlarmRuleCatalog/Company/{companyId}";
        public static string _iotHubEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTHub";
        public static string _iotHubInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTHub/Company/{companyId}";
        public static string _messageEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageCatalog";
        public static string _messageInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageCatalog/Company/{companyId}";
        public static string _nonChildMsgInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageCatalog/company/{companyId}/NonChild";
        public static string _messageElementEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageElement";
        public static string _messageFlatElementEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageCatalog";

        public static string _mandatoryMsgEndPoint = _sfAPIServiceBaseURI + "admin-api/MessageMandatoryElementDef";

        public static string _deviceCertificateEndPoint = _sfAPIServiceBaseURI + "admin-api/DeviceCertificate";
        public static string _deviceCertificateInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/DeviceCertificate/Company/{companyId}";

        public static string _widgetClassEndPoint = _sfAPIServiceBaseURI + "admin-api/WidgetClass";
        public static string _widgetCatalogEndPoint = _sfAPIServiceBaseURI + "admin-api/WidgetCatalog";
        public static string _widgetCatalogInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/WidgetCatalog/Company/{companyId}";

        public static string _externalApplicationInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/ExternalApplication/company/{companyId}";
        public static string _externalApplicationEndPoint = _sfAPIServiceBaseURI + "admin-api/ExternalApplication";

        public static string _dashboardInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/Dashboard/Company/{companyId}";
        public static string _equipmentClassDashboardInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/Dashboard/Company/{companyId}/EquipmentClass";
        //public static string _companyDashboard = _sfAPIServiceBaseURI + "admin-api/Dashboard/Company/{companyId}";
        public static string _factoryDashboard = _sfAPIServiceBaseURI + "admin-api/Dashboard/Factory";
        public static string _widgetInDashboardEndPoint = _sfAPIServiceBaseURI + "admin-api/DashboardWidget/Dashboard";
        public static string _dashboardWidgetEndPoint = _sfAPIServiceBaseURI + "admin-api/DashboardWidget";
        public static string _dashboardEndPoint = _sfAPIServiceBaseURI + "admin-api/Dashboard";

        public static string _employeeInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/Employee/company/{companyId}";
        public static string _employeeEndPoint = _sfAPIServiceBaseURI + "admin-api/Employee";
        public static string _userRoleEndPoint = _sfAPIServiceBaseURI + "admin-api/UserRole";
        public static string _userRoleInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/UserRole/company/{companyId}";
        public static string _userRolePermissionEndPoint = _sfAPIServiceBaseURI + "admin-api/UserRolePermission/UserRole";
        public static string _permissionCatlogEndPoint = _sfAPIServiceBaseURI + "admin-api/PermissionCatalog";
        public static string _equipmentClassEndPoint = _sfAPIServiceBaseURI + "admin-api/EquipmentClass";
        public static string _equipmentClassInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/EquipmentClass/company/{companyId}";
        public static string _equipmentMetadataEndPoint = _sfAPIServiceBaseURI + "admin-api/MetaDataDefination";
        public static string _equipmentMetadataInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/MetaDataDefination/Company/{companyId}/Equipment";
        public static string _deviceTypeEndPoint = _sfAPIServiceBaseURI + "admin-api/DeviceType";
        public static string _operationTaskEndPoint = _sfAPIServiceBaseURI + "admin-api/OperationTask";
        public static string _operationTaskSearchEndPoint = _sfAPIServiceBaseURI + "admin-api/OperationTask/Company/{companyId}/search";

        public static string _cultureInfoEndPoint = _sfAPIServiceBaseURI + "admin-api/RefCultureInfo";

        public static string _alarmMessageInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/AlarmMessage/company/{companyId}";
        public static string _alarmMessageInFactoryEndPoint = _sfAPIServiceBaseURI + "admin-api/AlarmMessage/Factory";
        public static string _alarmMessageInEquipmentEndPoint = _sfAPIServiceBaseURI + "admin-api/AlarmMessage/Equipment";
        public static string _usageLogEndPoint = _sfAPIServiceBaseURI + "admin-api/Company/{companyId}/UsageLog";

        public static string _deviceConfigInCompanyEndPoint = _sfAPIServiceBaseURI + "admin-api/Company/{companyId}/IoTDeviceCustomizedConfiguration";
        public static string _deviceConfigEndPoint = _sfAPIServiceBaseURI + "admin-api/IoTDeviceCustomizedConfiguration";

        public static string _externalDashboardEndPoint = _sfAPIServiceBaseURI + "admin-api/ExternalDashboard/Company/{companyId}";
    }
}
