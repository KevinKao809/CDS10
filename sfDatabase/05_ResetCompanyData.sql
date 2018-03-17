Delete from SmartFactory.AlarmRuleItem where AlarmRuleCatalogId in (select Id from SmartFactory.AlarmRuleCatalog where CompanyId=49)
Delete from SmartFactory.AlarmNotification where AlarmRuleCatalogId in (select Id from SmartFactory.AlarmRuleCatalog where CompanyId=49)
Delete from SmartFactory.AlarmRuleCatalog where CompanyId=49
Delete from SmartFactory.DashboardWidgets where DashboardID in (select Id from SmartFactory.Dashboard where CompanyId=49)
Delete from SmartFactory.Dashboard where CompanyId=49 AND DashboardType != 'company'
Delete from SmartFactory.EmployeeInRole where EmployeeID in (select ID from SmartFactory.Employee where CompanyID=49 and AdminFlag=0)
Delete from SmartFactory.Employee where CompanyID=49 and AdminFlag=0
Delete from SmartFactory.Equipment where FactoryId in (select id from SmartFactory.Factory where companyId=49)
Delete from SmartFactory.ExternalApplication where CompanyID=49
Delete from SmartFactory.IoTDeviceConfigurationValue where IoTHubDeviceID in (select IoTHubDeviceID from SmartFactory.IoTDevice where FactoryID in (select id from SmartFactory.Factory where companyId=49))
Delete from SmartFactory.IoTDeviceMessageCatalog where IoTHubDeviceID in (select IoTHubDeviceID from SmartFactory.IoTDevice where FactoryID in (select id from SmartFactory.Factory where companyId=49))
Delete from SmartFactory.IoTDevice where FactoryID in (select id from SmartFactory.Factory where companyId=49)
Delete from SmartFactory.IoTHub where CompanyId = 49
Delete from SmartFactory.DeviceCertificate where CompanyId=49
Delete from SmartFactory.MessageElement where MessageCatalogID in (select ID from SmartFactory.MessageCatalog where CompanyID=49)
Delete from SmartFactory.WidgetCatalog where MessageCatalogId in (select ID from SmartFactory.MessageCatalog where CompanyID=49)
Delete from SmartFactory.MessageCatalog where CompanyID=49
Delete from SmartFactory.WidgetCatalog where CompanyID=49
Delete from SmartFactory.OperationTask where CompanyId=49
Delete from SmartFactory.UsageLog where CompanyId=49
Delete from SmartFactory.UserRolePermission where UserRoleID in (select ID from SmartFactory.UserRole where CompanyId=49)
Delete from SmartFactory.EmployeeInRole where UserRoleID in (select ID from SmartFactory.UserRole where CompanyId=49)
Delete from SmartFactory.UserRole where CompanyId=49
Delete from SmartFactory.Factory where companyId=49


