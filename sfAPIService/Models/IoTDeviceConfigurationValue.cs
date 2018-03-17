using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sfShareLib;
using Newtonsoft.Json.Linq;

namespace sfAPIService.Models
{
    public class IoTDeviceConfigurationValueModels
    {
        public class Detail
        {
            public bool IsSystem { get; set; }
            public bool Readonly { get; set; }
            public string ConfigurationName { get; set; }
            public string ConfigurationDescription { get; set; }
            public string ConfigurationDataTye { get; set; }
            public string DeviceValue { get; set; }
            public string SettingValue { get; set; }
            public bool EnabledFlag { get; set; }
        }

        public List<Detail> GetAll(string deviceID)
        {
            DBHelper._IoTDevice dbhelp_iotDevice = new DBHelper._IoTDevice();
            DBHelper._IoTDeviceConfiguration dbhelp_sysConfig = new DBHelper._IoTDeviceConfiguration();
            DBHelper._IoTDeviceCustomizedConfiguration dbhelp_customizedConfig = new DBHelper._IoTDeviceCustomizedConfiguration();
            List<Detail> returnConfigList = new List<Detail>();

            IoTDevice iotDevice = dbhelp_iotDevice.GetByid(deviceID);
            int companyId = iotDevice.Factory.CompanyId;
            /***** retrieve existing desired config *****/
            JObject desiredProperty = JObject.Parse(iotDevice.DeviceTwinsDesired);
            Dictionary<string, string> dic_existingSysDesiredConfig = new Dictionary<string, string>();
            if (desiredProperty["SF_SystemConfig"] != null)
            {
                JObject systemConfig = JObject.Parse(desiredProperty["SF_SystemConfig"].ToString());

                foreach (var obj in systemConfig)
                {
                    string value = obj.Value.Value<string>();
                    if (value == "True" || value == "False")
                        value = value.ToLower();

                    dic_existingSysDesiredConfig.Add(obj.Key, value);
                }
            }

            Dictionary<string, string> dic_existingCustomizedDesiredConfig = new Dictionary<string, string>();
            if (desiredProperty["SF_CustomizedConfig"] != null)
            {
                JObject customizedConfig = JObject.Parse(desiredProperty["SF_CustomizedConfig"].ToString());
                foreach (var obj in customizedConfig)
                {
                    string value = obj.Value.Value<string>();
                    if (value == "True" || value == "False")
                        value = value.ToLower();

                    dic_existingCustomizedDesiredConfig.Add(obj.Key, value);
                }
            }

            /***** retrieve existing reported config *****/
            JObject reportedProperty = JObject.Parse(iotDevice.DeviceTwinsReported);
            Dictionary<string, string> dic_existingSysReportedConfig = new Dictionary<string, string>();
            if (reportedProperty["SF_SystemConfig"] != null)
            {
                JObject systemConfig = JObject.Parse(reportedProperty["SF_SystemConfig"].ToString());
                foreach (var obj in systemConfig)
                {
                    dic_existingSysReportedConfig.Add(obj.Key, obj.Value.Value<string>());
                }
            }

            Dictionary<string, string> dic_existingCustomizedReportedConfig = new Dictionary<string, string>();
            if (reportedProperty["SF_CustomizedConfig"] != null)
            {
                JObject customizedConfig = JObject.Parse(reportedProperty["SF_CustomizedConfig"].ToString());
                foreach (var obj in customizedConfig)
                {
                    dic_existingCustomizedReportedConfig.Add(obj.Key, obj.Value.Value<string>());
                }
            }

            /***** return System Configuration *****/
            foreach (var config in dbhelp_sysConfig.GetAll())
            {
                if (dic_existingSysDesiredConfig.ContainsKey(config.Name))
                {
                    returnConfigList.Add(new Detail()
                    {
                        IsSystem = true,
                        Readonly = false,
                        ConfigurationName = config.Name,
                        ConfigurationDataTye = config.DataType,
                        ConfigurationDescription = config.Description,
                        DeviceValue = dic_existingSysReportedConfig.ContainsKey(config.Name) ? dic_existingSysReportedConfig[config.Name] : "",
                        SettingValue = dic_existingSysDesiredConfig[config.Name],
                        EnabledFlag = true
                    });
                }
                else
                {
                    returnConfigList.Add(new Detail()
                    {
                        IsSystem = true,
                        Readonly = false,
                        ConfigurationName = config.Name,
                        ConfigurationDataTye = config.DataType,
                        ConfigurationDescription = config.Description,
                        DeviceValue = "",
                        SettingValue = config.DefaultValue,
                        EnabledFlag = false
                    });
                }
                
            }

            /***** return Customized Configuration *****/
            List<string> list_deviceCustomizedConfigName = new List<string>();
            foreach (var config in dbhelp_customizedConfig.GetAllByCompanyId(companyId))
            {
                if (dic_existingCustomizedDesiredConfig.ContainsKey(config.Name))
                {
                    returnConfigList.Add(new Detail()
                    {
                        IsSystem = false,
                        Readonly = false,
                        ConfigurationName = config.Name,
                        ConfigurationDataTye = config.DataType,
                        ConfigurationDescription = config.Description,
                        DeviceValue = dic_existingCustomizedReportedConfig.ContainsKey(config.Name) ? dic_existingCustomizedReportedConfig[config.Name] : "",
                        SettingValue = dic_existingCustomizedDesiredConfig[config.Name],
                        EnabledFlag = true
                    });
                }
                else
                {
                    returnConfigList.Add(new Detail()
                    {
                        IsSystem = false,
                        Readonly = false,
                        ConfigurationName = config.Name,
                        ConfigurationDataTye = config.DataType,
                        ConfigurationDescription = config.Description,
                        DeviceValue = "",
                        SettingValue = config.DefaultValue,
                        EnabledFlag = false
                    });
                }
                list_deviceCustomizedConfigName.Add(config.Name);
            }

            /***** return Customized Configuration (readonly) *****/
            foreach (var config in dic_existingCustomizedReportedConfig)
            {
                if (!list_deviceCustomizedConfigName.Contains(config.Key))
                {
                    returnConfigList.Add(new Detail()
                    {
                        IsSystem = false,
                        Readonly = true,
                        ConfigurationName = config.Key,
                        ConfigurationDataTye = "",
                        ConfigurationDescription = "",
                        DeviceValue = config.Value,
                        SettingValue = "",
                        EnabledFlag = false
                    });
                }
            }

            return returnConfigList;
        }
    }
}