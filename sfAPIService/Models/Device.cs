using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using sfShareLib;
using System.Threading.Tasks;

namespace sfAPIService.Models
{
    public class DeviceViewModels
    {
    }
    public class DeviceModels
    {
        public class Detail
        {
            public string DeviceId { get; set; }
            public string IoTHubName { get; set; }
            public string IoTHubProtocol { get; set; }
            public string IoTHubAuthenticationType { get; set; }
            public string CertificateFileName { get; set; }
            public string CertificatePassword { get; set; }
            public string DeviceKey { get; set; }
            public string ContainerName { get; set; }
        }

        public async Task<Detail> GetIoTDeviceByDeviceId(string deviceId)
        {
            DeviceUtility deviceHelper = new DeviceUtility();
            SFDatabaseEntities dbEnty = new SFDatabaseEntities();
            var device = dbEnty.IoTDevice.Find(deviceId);
            if (device == null)
                throw new Exception("404");

            Detail returnDeviceInfo = new Detail()
            {
                DeviceId = device.IoTHubDeviceID,
                IoTHubProtocol = device.IoTHubProtocol,
                IoTHubAuthenticationType = device.AuthenticationType
            };

            //Confirm connectionstring
            if (await deviceHelper.CheckIoTHubConnectionString(device.IoTHub.P_IoTHubConnectionString, device.IoTHubDeviceID))
            {
                returnDeviceInfo.IoTHubName = device.IoTHub.P_IoTHubConnectionString.Split(';')[0].Split('=')[1];
                returnDeviceInfo.ContainerName = device.IoTHub.P_UploadContainer;
            }
            else if (await deviceHelper.CheckIoTHubConnectionString(device.IoTHub.S_IoTHubConnectionString, device.IoTHubDeviceID))
            {
                returnDeviceInfo.IoTHubName = device.IoTHub.S_IoTHubConnectionString.Split(';')[0].Split('=')[1];
                returnDeviceInfo.ContainerName = device.IoTHub.S_UploadContainer;
            }
            else
                throw new Exception("None vaild IoT Hub");

            if (returnDeviceInfo.IoTHubAuthenticationType == "Key")
            {
                returnDeviceInfo.DeviceKey = device.IoTHubDeviceKey;
            }
            else if (returnDeviceInfo.IoTHubAuthenticationType == "Certificate")
            {
                returnDeviceInfo.CertificateFileName = device.DeviceCertificate.FileName;
                returnDeviceInfo.CertificatePassword = device.DeviceCertificate.PFXPassword;
            }

            return returnDeviceInfo;
        }
    }

}