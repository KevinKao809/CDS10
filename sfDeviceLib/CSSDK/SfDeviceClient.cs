using Microsoft.CDS.Devices.Client.Models;
using Microsoft.CDS.Devices.Client.Transport;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CDS.Devices.Client.Utility;
using Microsoft.Azure.Devices.Client.Exceptions;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.Devices.Shared;
using Microsoft.CDS.Devices.Client.SfDeviceTwin;

namespace Microsoft.CDS.Devices.Client
{
    public class SfDeviceClient
    {
        private static readonly string TAG = nameof(SfDeviceClient);
        const string MESSAGE_PROPERTY_MESSAGE_CATALOG_ID = "MessageCatalogId";
        public const string MESSAGE_PROPERTY_TYPE = "Type";
        public const string MESSAGE_PROPERTY_TYPE_COMMAND = "Command";
        public const string MESSAGE_PROPERTY_TYPE_MESSAGE = "Message";

        private RestfulAPIHelper _restfulAPIHelper;
        private HwProductKey _hwProductKey;
        private DeviceModels _deviceModels;
        private DeviceClient _deviceClient;
        private List<MessageCatalogSchema> _messageCatalogSchemaList;
        private SfTwinPropertiesHelper _sfTwinPropertiesHelper;
        private static readonly int MAXIMUM_RETRY_COUNT_IOT_HUB = 5;
        private int _retryCountIoTHub = 0;
        private string _certificatePath = null;

        SfDeviceClient(HwProductKey hwProductKey)
        {
            _hwProductKey = hwProductKey;
            _messageCatalogSchemaList = new List<MessageCatalogSchema>();
        }

        public static SfDeviceClient CreateSfDeviceClient(HwProductKey hwProductKey)
        {
            showAppVersion();

            if (hwProductKey == null || hwProductKey.email == null || hwProductKey.password == null)
            {
                throw new ArgumentNullException(nameof(hwProductKey));
            }

            SfDeviceClient sfDeviceClient = new SfDeviceClient(hwProductKey);

            return sfDeviceClient;
        }

        public static SfDeviceClient CreateSfDeviceClient(HwProductKey hwProductKey, string certificatePath)
        {
            SfDeviceClient sfDeviceClient = CreateSfDeviceClient(hwProductKey);

            sfDeviceClient._certificatePath = certificatePath;

            return sfDeviceClient;
        }

        public async Task Initial()
        {
            await initialzation(_certificatePath);

            return;
        }

        private async Task initialzation(string certificatePath)
        {
            _restfulAPIHelper = new RestfulAPIHelper(_hwProductKey.email, _hwProductKey.password);

            // Get the Device Model
            string resp = await _restfulAPIHelper.callDeviceAPIService("GET", _hwProductKey.email, null);
            _deviceModels = DeviceModels.CreateDeviceModels(resp);

            verifyAndAppendIoTHubAuthCertificatePath(_deviceModels, _certificatePath);

            // Get the Message Schema of Device
            resp = await _restfulAPIHelper.getDeviceMessageSchemaAPIService(_hwProductKey.email);
            _messageCatalogSchemaList = MessageCatalogSchema.CreateMessageCatalogSchemaList(resp);

            _deviceClient = getAzureIoTDeviceSDKClient(_deviceModels);

            // Get SfTwin Properties Helper
            _sfTwinPropertiesHelper = new SfTwinPropertiesHelper(_deviceClient);

            // Sync System Config of Twin
            await _sfTwinPropertiesHelper.SyncAndUpdateSfTwinPropertiesFromTwin();

        }

        /// <summary>
        /// Delegate for customer property update callbacks.  This will be called
        /// every time we receive a PATCH from the service.
        /// </summary>
        /// <param name="customerConfig">Properties that were contained in the update that was received from the service</param>
        public delegate Task CustomerPropertiesUpdateCallback(JObject customerConfig);

        public async Task SetCustomerPropertiesUpdateCallbackAsync(CustomerPropertiesUpdateCallback callback)
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            if (callback == null)
            {
                Logger.showDebug(TAG, "No any customer property update callback was set.");
            }
            else
            {
                await _sfTwinPropertiesHelper.SetCustomerPropertiesUpdateCallback(callback);
            }
        }

        public async Task UpdateReportedCustomerPropertiesAsync(JObject customerConfig)
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            await _sfTwinPropertiesHelper.UpdateReportedCustomerPropertiesAsync(customerConfig);
        }

        public async Task ClearAllReportedCustomerPropertiesAsync()
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            await _sfTwinPropertiesHelper.ClearAllReportedCustomerPropertiesAsync();
        }

        public async Task<Twin> GetTwinAsync()
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            return await _sfTwinPropertiesHelper.GetTwinAsync();
        }

        public async Task<JObject> GetDesiredCustomerPropertiesAsync()
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            return await _sfTwinPropertiesHelper.GetDesiredCustomerPropertiesAsync();
        }

        public async Task<JObject> GetReportedCustomerPropertiesAsync()
        {
            if (_sfTwinPropertiesHelper == null)
                throw new Exception("The Initial should be called first.");

            return await _sfTwinPropertiesHelper.GetReportedCustomerPropertiesAsync();
        }

        private void verifyAndAppendIoTHubAuthCertificatePath(DeviceModels deviceModels, string certificatePath)
        {
            if (deviceModels.IoTHubAuthenticationType.ToLower().Equals(DeviceModels.IOTHUB_AUTH_TYPE_CERTIFICATE))
            {
                if (string.IsNullOrEmpty(certificatePath))
                {
                    Logger.showError(TAG, "The certificate path should be specified for the x509 certified device.");
                    throw new ArgumentNullException(nameof(certificatePath));
                }
                else
                {
                    deviceModels.CertificateFileName = appendCertificateFullPath(certificatePath, deviceModels.CertificateFileName);
                }
            }
        }

        public async Task UploadFileToBlob(string filePath, string blobName)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            Logger.showDebug(TAG, "SendFileToBlob filePath: {0}".FormatInvariant(filePath));
            Logger.showDebug(TAG, "SendFileToBlob blobName: {0}".FormatInvariant(blobName));

            try
            {
                using (var sourceData = new FileStream(filePath, FileMode.OpenOrCreate))
                {
                    await _deviceClient.UploadToBlobAsync(blobName, sourceData);
                }

            }
            catch (Exception ex)
            {
                StringBuilder s = LogUtility.BuildExceptionMessage(ex);
                s.AppendLine("UploadFileToBlob Exception:" + ex.Message.ToString());
                //Logger.showError(TAG, s);
                throw new Exception(s.ToString());
            }
        }

        private async Task uploadLog(FileInfo fileInfo)
        {
            if (fileInfo == null)
                throw new ArgumentNullException(nameof(fileInfo));

            if (!fileInfo.Exists)
                throw new FileNotFoundException(fileInfo.FullName);

            try
            {
                string resp = await _restfulAPIHelper.uploadDeviceLogAPIService(this._hwProductKey.email, fileInfo);
            }
            catch (Exception)
            {
                throw;
            }

        }

        private string appendCertificateFullPath(string certificatePath, string certificateFileName)
        {
            return certificatePath + certificateFileName;
        }

        public async Task SendEventAsyncWithRetry(int messageCatalogId, string messageString)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            if (string.IsNullOrEmpty(messageString))
                throw new ArgumentNullException(nameof(messageString));

            try
            {
                ExpandoObject targetMessageObj = convertToMessageObj(messageCatalogId, messageString);
                string targetMessageJsonContent = JsonConvert.SerializeObject(targetMessageObj);

                var message = new Message(Encoding.UTF8.GetBytes(targetMessageJsonContent));
                message.Properties.Add(MESSAGE_PROPERTY_MESSAGE_CATALOG_ID, messageCatalogId.ToString());
                message.Properties.Add(MESSAGE_PROPERTY_TYPE, MESSAGE_PROPERTY_TYPE_MESSAGE);
                await _deviceClient.SendEventAsync(message);

                //Logger.showTrace(TAG, "{0} > Sending message: {1}".FormatInvariant(DateTime.UtcNow, messageString));
            }
            catch (IotHubException)
            {
                await reinitializeAndRetry(messageCatalogId, messageString);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    Exception innerException = ex.InnerException;
                    if (innerException.GetType() == typeof(DeviceMaximumQueueDepthExceededException))
                    {
                        Logger.showError(TAG, "DeviceMaximumQueueDepthExceededException: ".FormatInvariant(innerException.Message));

                        await reinitializeAndRetry(messageCatalogId, messageString);
                    }
                }

                StringBuilder s = LogUtility.BuildExceptionMessage(ex);
                s.AppendLine("SendEventAsyncWithRetry Exception:" + ex.Message.ToString());
                throw new Exception(s.ToString());

            }
        }

        private async Task reinitializeAndRetry(int messageCatalogId, string messageString)
        {
            if (_retryCountIoTHub >= MAXIMUM_RETRY_COUNT_IOT_HUB)
            {
                _retryCountIoTHub = 0;

                throw new Exception("SendEventAsyncWithRetry Exception: MAXIMUM_RETRY_COUNT_IOT_HUB");
            }

            // Re-initialzation
            await initialzation(this._certificatePath);

            _retryCountIoTHub++;
            retryWithExponentialBackoffAlgorithm(_retryCountIoTHub);

            await SendEventAsyncWithRetry(messageCatalogId, messageString);
        }

        private void retryWithExponentialBackoffAlgorithm(int retryCount)
        {
            int vaule = 2;
            long timeoutInSecond = (long)Math.Pow(vaule, retryCount);

            //Console.WriteLine("timeoutInSecond={0}, now={1}", timeoutInSecond, DateTime.Now);
            Task.Delay((int)timeoutInSecond * 1000).Wait();
        }

        public async Task<Message> ReceiveAsync()
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            Message receivedMessage = await _deviceClient.ReceiveAsync();
            return receivedMessage;
        }

        public async Task CompleteAsync(Message message)
        {
            if (_deviceClient == null)
                throw new Exception("Should be Initial first");

            await _deviceClient.CompleteAsync(message);
        }

        private DeviceClient getAzureIoTDeviceSDKClient(DeviceModels deviceModels)
        {
            if (deviceModels.IoTHubAuthenticationType.ToLower().Equals(DeviceModels.IOTHUB_AUTH_TYPE_KEY))
            {
                string connectionString = combineIoTHubDeviceConnectionString(deviceModels.IoTHubName, deviceModels.DeviceId, deviceModels.DeviceKey);

                Logger.showDebug(TAG, "IoTHubName={0}, DeviceId={1}, DeviceKey={2}".FormatInvariant(deviceModels.IoTHubName, deviceModels.DeviceId, deviceModels.DeviceKey));
                Logger.showDebug(TAG, "IoTHubProtocol={0}".FormatInvariant(deviceModels.IoTHubProtocol));

                DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(connectionString,
                    getTransportType(deviceModels.IoTHubProtocol.ToLower()));

                return deviceClient;
            }
            else if (deviceModels.IoTHubAuthenticationType.ToLower().Equals(DeviceModels.IOTHUB_AUTH_TYPE_CERTIFICATE))
            {
                // x509 certificate                
                var x509Certificate = new X509Certificate2(deviceModels.CertificateFileName, deviceModels.CertificatePassword);

                Logger.showDebug(TAG, "CertificateFileName={0}, CertificatePassword={1}".FormatInvariant(deviceModels.CertificateFileName, deviceModels.CertificatePassword));
                Logger.showDebug(TAG, "IoTHubProtocol={0}".FormatInvariant(deviceModels.IoTHubProtocol));

                var authMethod = new DeviceAuthenticationWithX509Certificate(deviceModels.DeviceId, x509Certificate);
                DeviceClient deviceClient = DeviceClient.Create(
                    deviceModels.IoTHubName,
                    authMethod,
                    getTransportType(deviceModels.IoTHubProtocol.ToLower()));

                return deviceClient;
            }
            else
            {
                Logger.showError(TAG, "Unsupported IoTHub Authentication Type {0}".FormatInvariant(deviceModels.IoTHubAuthenticationType));
                throw new InvalidOperationException("Unsupported IoTHub Authentication Type {0}".FormatInvariant(deviceModels.IoTHubAuthenticationType));
            }
        }

        /* Generate Dynamic Object according to Message Schema */
        private ExpandoObject convertToMessageObj(int msgId, string msgJsonContent)
        {
            MessageCatalogSchema msgSchema;
            var L2EQuery = from a in this._messageCatalogSchemaList
                           where a.MessageCatalogId == msgId
                           select a;
            if (L2EQuery.Count() == 0)
                throw new Exception("Not a valid Message Id");           //Message Id doens't apply on this IoT Device
            else
                msgSchema = L2EQuery.FirstOrDefault<MessageCatalogSchema>();

            ExpandoObject deviceMessage = new ExpandoObject();
            JObject sourceMsg = JObject.Parse(msgJsonContent);
            foreach (var element in msgSchema.ElementList)
            {
                if (element.MandatoryFlag)
                {
                    if (sourceMsg[element.Name] == null)
                        throw new Exception("Not a valid Message Schema, Missing: " + element.Name);
                    JSONHelper.AddExpandoObjectProperty(deviceMessage, element.Name, sourceMsg[element.Name]);
                }
                else
                {
                    if (sourceMsg[element.Name] != null)
                        JSONHelper.AddExpandoObjectProperty(deviceMessage, element.Name, sourceMsg[element.Name]);
                }
            }
            return deviceMessage;
        }

        private TransportType getTransportType(string protocol)
        {
            if (protocol.Equals("mqtt"))
            {
                return TransportType.Mqtt;
            }
            else if (protocol.Equals("amqp"))
            {
                return TransportType.Amqp;
            }
            else if (protocol.Equals("https"))
            {
                return TransportType.Http1;
            }
            else
            {
                Logger.showError(TAG, "Unsupported Transport Type {0}".FormatInvariant(protocol));
                throw new InvalidOperationException("Unsupported Transport Type {0}".FormatInvariant(protocol));
            }
        }

        private string combineIoTHubDeviceConnectionString(string hostName, string deviceId, string deviceKey)
        {
            return "HostName=" + hostName + ";DeviceId=" + deviceId + ";SharedAccessKey=" + deviceKey;
        }

        private static void showAppVersion()
        {
            AssemblyName name = Assembly.GetExecutingAssembly().GetName();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Microsoft Connected Device Studio C# Device SDK [Version: {0}]", name.Version.ToString());
            Console.ResetColor();
        }
    }
}
