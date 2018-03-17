using Microsoft.CDS.Devices.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Microsoft.CDS.Devices.Client.Models
{
    public class DeviceModels
    {
        public static string IOTHUB_AUTH_TYPE_CERTIFICATE = "certificate";
        public static string IOTHUB_AUTH_TYPE_KEY = "key";

        public string DeviceId { get; set; }
        public string IoTHubName { get; set; }
        public string IoTHubProtocol { get; set; }
        public string IoTHubAuthenticationType { get; set; }
        public string CertificateFileName { get; set; }
        public string CertificatePassword { get; set; }
        public string DeviceKey { get; set; }
        public string ContainerName { get; set; }

        public static DeviceModels CreateDeviceModels(string s)
        {
            try
            {
                return JsonConvert.DeserializeObject<DeviceModels>(s);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("CreateDeviceModels - failed to parse string to DeviceModel");
                Logger.showError("DeviceModels", logMessage);
                throw;
            }
        }
    }
}
