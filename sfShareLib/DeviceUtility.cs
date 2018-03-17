using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Microsoft.Azure.Devices;

namespace sfShareLib
{
    public class DeviceUtility
    {
        public async Task<string> CheckAndGetIoTHubConnectionString(string[] connectionString, string deviceId)
        {
            foreach (string contString in connectionString)
            {
                if (contString == null)
                    break;

                try
                {
                    RegistryManager registryManager = RegistryManager.CreateFromConnectionString(contString);
                    Device deviceOnIoTHub = await registryManager.GetDeviceAsync(deviceId);
                    if (deviceOnIoTHub != null)
                        return contString;
                }
                catch
                {
                    continue;
                }
            }
           
            return "";
        }

        public async Task<bool> CheckIoTHubConnectionString(string connectionString, string deviceId)
        {
            try
            {
                RegistryManager registryManager = RegistryManager.CreateFromConnectionString(connectionString);
                Device deviceOnIoTHub = await registryManager.GetDeviceAsync(deviceId);
                if (deviceOnIoTHub != null)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }

        public DateTime GetSpecifyTimeZoneDateTimeByTimeStamp(long timeStamp, int timeZone)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(timeStamp);
            dtDateTime = dtDateTime.AddHours(timeZone);
            return dtDateTime;
        }
    }
}
