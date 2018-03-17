using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CDS.Devices.Client;
using Microsoft.CDS.Devices.Client.Models;
using Microsoft.CDS.Devices.Client.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CDS.Devices.Client.SfDeviceTwin
{
    class SfTwinPropertiesHelper
    {
        private static readonly string TAG = nameof(SfTwinPropertiesHelper);
        
        public const string SF_SYSTEM_CONFIG_MINIMUM_SDK_VERSION_SUPPORT = "MinSmartFactorySDKVersion";
        public const string SF_SYSTEM_CONFIG_ENABLE_UPLOAD_LOG = "UploadLogData";
        public const string SF_SYSTEM_CONFIG_UPLOAD_MESSAGE_INTERVAL = "UploadMessageInterval";        

        private DeviceClient _deviceClient;
        private SfTwinProperties _sfTwinProperties;
        public SfDeviceClient.CustomerPropertiesUpdateCallback _customerPropertiesUpdateCallback { set; get; }

        public SfTwinPropertiesHelper(DeviceClient deviceClient)
        {
            _deviceClient = deviceClient;
            _customerPropertiesUpdateCallback = null;
        }

        public async Task SyncAndUpdateSfTwinPropertiesFromTwin()
        {
            try
            {
                Twin twin = await GetTwinAsync();
                _sfTwinProperties = SfTwinProperties.CreateSfTwinProperties(twin);
                await syncReportedSystemPropertiesAsync(twin.Properties.Desired);
            }
            catch(Exception ex)
            {
                Logger.showError(TAG, "SyncAndUpdateSfTwinPropertiesFromTwin Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }
            
        }

        public async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                if(checkIsAValidSFDesiredProperty(desiredProperties))
                {
                    await processAndDoActionInSystemConfig(desiredProperties);

                    processAndDoActionInCustomizedConfig(desiredProperties);
                }

            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "OnDesiredPropertyChanged Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }
        }

        private static TwinCollection CreateReportedCustomerProperties(JObject properties)
        {
            TwinCollection reportedCustomerProperties = new TwinCollection();
            reportedCustomerProperties[SfTwinProperties.SF_CUSTOMIZED_CONFIG] = properties;

            return reportedCustomerProperties;
        }

        public static TwinCollection CreateReportedCustomerPropertiesWithTimestamp(JObject properties)
        {
            TwinCollection reportedCustomerProperties = CreateReportedCustomerProperties(properties);
            reportedCustomerProperties[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP] = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            return reportedCustomerProperties;
        }

        private static TwinCollection CreateReportedSystemPropertiesWithTimestamp(JObject properties)
        {
            TwinCollection reportedSystemProperties = new TwinCollection();
            reportedSystemProperties[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP] = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            reportedSystemProperties[SfTwinProperties.SF_SYSTEM_CONFIG] = properties;

            return reportedSystemProperties;
        }

        private static TwinCollection CreateReportedProperties(JObject system, JObject customer, int timestamp)
        {
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP] = timestamp;
            reportedProperties[SfTwinProperties.SF_CUSTOMIZED_CONFIG] = customer;
            reportedProperties[SfTwinProperties.SF_SYSTEM_CONFIG] = system;

            return reportedProperties;
        }

        private void processAndDoActionInCustomizedConfig(TwinCollection desiredProperties)
        {
            if (desiredProperties.Contains(SfTwinProperties.SF_CUSTOMIZED_CONFIG) == false)
                return;

            _customerPropertiesUpdateCallback?.Invoke(desiredProperties[SfTwinProperties.SF_CUSTOMIZED_CONFIG]);
        }

        private async Task processAndDoActionInSystemConfig(TwinCollection desiredProperties)
        {
            if (desiredProperties.Contains(SfTwinProperties.SF_SYSTEM_CONFIG) == false)
                return;

            try
            {
                JObject sfSystemConfig = desiredProperties[SfTwinProperties.SF_SYSTEM_CONFIG];
                Logger.showDebug(TAG, "sdkMinimumVersion: " + sfSystemConfig[SF_SYSTEM_CONFIG_MINIMUM_SDK_VERSION_SUPPORT]);
                Logger.showDebug(TAG, "enableLogUpload: " + sfSystemConfig[SF_SYSTEM_CONFIG_ENABLE_UPLOAD_LOG]);
                Logger.showDebug(TAG, "messageInterval: " + sfSystemConfig[SF_SYSTEM_CONFIG_UPLOAD_MESSAGE_INTERVAL]);

                /* Stuff something here to perform the actions of System Config */



                /* Update */
                await UpdateReportedSystemPropertiesAsync(sfSystemConfig);

            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "processAndActionSystemConfig Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }
            
        }

        private async Task syncReportedSystemPropertiesAsync(TwinCollection desiredProperties)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            if (_sfTwinProperties == null)
                throw new Exception("Should be Load SfTwin Properties From Twin first");

            try
            {
                if (checkIsAValidSFDesiredProperty(desiredProperties))
                {
                    await processAndDoActionInSystemConfig(desiredProperties);
                }

            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "OnDesiredPropertyChanged Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }

        }

        public async Task UpdateReportedCustomerPropertiesAsync(JObject customerConfig)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            if(_sfTwinProperties == null)
                throw new Exception("Should be Load SfTwin Properties From Twin first");

            TwinCollection reported = CreateReportedCustomerPropertiesWithTimestamp(customerConfig);

            //JObject systemConfig = _sfTwinProperties.Reported.SystemConfig;
            //int timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //TwinCollection reportedProperties = SfTwinPropertiesHelper.CreateReportedProperties(systemConfig, customerConfig, timestamp);

            /* Update to device twin */
            await _deviceClient.UpdateReportedPropertiesAsync(reported);

            /* Send a D2C message to notice the update of reported property */
            JObject payload = new JObject();
            payload.Add(SfTwinProperties.SF_LASTUPDATED_TIMESTAMP, reported[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP]);
            payload.Add(SfTwinProperties.SF_UPDATED_TYPE, SfTwinProperties.SF_CUSTOMIZED_CONFIG);
            await SendD2CReportedCustomerPropertiesUpdate(payload.ToString());

            /* Update */
            //_sfTwinProperties.Reported.CustomerConfig = customerConfig;
            //_sfTwinProperties.Reported.LastUpdatedTimestamp = timestamp;
        }

        private async Task UpdateReportedSystemPropertiesAsync(JObject systemConfig)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            if (_sfTwinProperties == null)
                throw new Exception("Should be Load SfTwin Properties From Twin first");

            TwinCollection reported = CreateReportedSystemPropertiesWithTimestamp(systemConfig);

            //JObject customerConfig = _sfTwinProperties.Reported.CustomerConfig;
            //int timestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;

            //TwinCollection reportedProperties = CreateReportedProperties(systemConfig, customerConfig, timestamp);

            /* Update to device twin */
            await _deviceClient.UpdateReportedPropertiesAsync(reported);

            /* Send a D2C message to notice the update of reported property */
            JObject payload = new JObject();
            payload.Add(SfTwinProperties.SF_LASTUPDATED_TIMESTAMP, reported[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP]);
            payload.Add(SfTwinProperties.SF_UPDATED_TYPE, SfTwinProperties.SF_SYSTEM_CONFIG);
            await SendD2CReportedCustomerPropertiesUpdate(payload.ToString());

            /* Update */
            //_sfTwinProperties.Reported.SystemConfig = systemConfig;
            //_sfTwinProperties.Reported.LastUpdatedTimestamp = timestamp;
        }

        private async Task SendD2CReportedCustomerPropertiesUpdate(string reportedProperties)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            if (string.IsNullOrEmpty(reportedProperties))
                throw new ArgumentNullException(nameof(reportedProperties));

            try
            {
                var message = new Message(Encoding.UTF8.GetBytes(reportedProperties));
                message.Properties.Add(SfDeviceClient.MESSAGE_PROPERTY_TYPE, SfDeviceClient.MESSAGE_PROPERTY_TYPE_COMMAND);
                await _deviceClient.SendEventAsync(message);
            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "SendD2CReportedCustomerPropertiesUpdate Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }
        }

        public async Task ClearAllReportedPropertiesAsync()
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            Twin twin = await GetTwinAsync();
            TwinCollection oldTC = twin.Properties.Reported;
            JObject oldReportedProperties = JObject.Parse(oldTC.ToJson());

            TwinCollection tc = new TwinCollection();
            foreach (JProperty jp in oldReportedProperties.Properties())
            {
                if (String.Equals(jp.Name.Substring(0, 1), "$"))
                {
                    Logger.showDebug(TAG, jp.Name + " - " + jp.Value + "(internal used)");
                }
                else
                {
                    Logger.showDebug(TAG, jp.Name + " - " + jp.Value);
                    tc[jp.Name] = null;
                }
            }

            await _deviceClient.UpdateReportedPropertiesAsync(tc);
        }

        public async Task ClearAllReportedCustomerPropertiesAsync()
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            TwinCollection tc = CreateReportedCustomerProperties(null);
            await _deviceClient.UpdateReportedPropertiesAsync(tc);
        }

        public async Task<Twin> GetTwinAsync()
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            Twin twin = await _deviceClient.GetTwinAsync();
            return twin;
        }

        public async Task<JObject> GetDesiredSystemPropertiesAsync()
        {
            try
            {
                Twin twin = await GetTwinAsync();
                return SfTwinProperties.GetDesiredSystemProperties(twin);
            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "GetDesiredSystemPropertiesAsync Exception: ".FormatInvariant(ex.Message.ToString()));
            }

            return null;
        }

        public async Task<JObject> GetDesiredCustomerPropertiesAsync()
        {
            try
            {
                Twin twin = await GetTwinAsync();
                return SfTwinProperties.GetDesiredCustomerProperties(twin);
            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "GetDesiredCustomerPropertiesAsync Exception: ".FormatInvariant(ex.Message.ToString()));
            }

            return null;
        }

        public async Task<JObject> GetReportedCustomerPropertiesAsync()
        {
            try
            {
                Twin twin = await GetTwinAsync();
                return SfTwinProperties.GetReportedCustomerProperties(twin);
            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "GetReportedCustomerPropertiesAsync Exception: ".FormatInvariant(ex.Message.ToString()));
            }

            return null;
        }

        public async Task SetCustomerPropertiesUpdateCallback(SfDeviceClient.CustomerPropertiesUpdateCallback customerPropertiesUpdateCallback)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            _customerPropertiesUpdateCallback = customerPropertiesUpdateCallback;
            await _deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null);
        }

        private bool checkIsAValidSFDesiredProperty(TwinCollection desiredProperties)
        {
            try
            {
                if (desiredProperties.Contains(SfTwinProperties.SF_LASTUPDATED_TIMESTAMP))
                {
                    int lastUpdatedTS = desiredProperties[SfTwinProperties.SF_LASTUPDATED_TIMESTAMP];
                    Logger.showDebug(TAG, "lastUpdatedTS: {0}".FormatInvariant(lastUpdatedTS));
                    if (lastUpdatedTS != 0)
                        return true;
                }
            }
            catch (Exception ex)
            {
                Logger.showError(TAG, "checkIsAValidSFDesiredProperty Exception: {0}".FormatInvariant(ex.Message.ToString()));
            }
            
            return false;
        }
    }
}
