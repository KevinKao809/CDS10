using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Security;
using System.Net.Http;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using Microsoft.CDS.Devices.Client.Utility;
using Microsoft.CDS.Devices.Client.Models;
using System.Web;
using Microsoft.CDS.Devices.Client.MultipartFormData;

namespace Microsoft.CDS.Devices.Client.Transport
{
    class RestfulAPIHelper
    {
        private static readonly string TAG = nameof(RestfulAPIHelper);
        public static readonly string DEVICE_LOG_API_FORMKEY_STARTTS = "startTS";
        public static readonly string DEVICE_LOG_API_FORMKEY_FILENAME = "filename";

        private readonly string _sfAPIServiceBaseURI;
        private readonly string _sfAPIServiceTokenEndPoint;
        private readonly string _sfAPIServiceDeviceAPIURI;
        private readonly string _email;
        private readonly string _password;
        private DeviceToken _currentDeviceToken;

        public RestfulAPIHelper(string email, string password)
        {
            _email = email;
            _password = password;
            try
            {
                string developerMode = ConfigurationManager.AppSettings["APIService.DeveloperMode"];
                if (string.IsNullOrEmpty(developerMode) == false && developerMode.Equals("true"))
                    _sfAPIServiceBaseURI = "https://sfapiservice.trafficmanager.net/"; // Development
                else
                {
                    _sfAPIServiceBaseURI = ConfigurationManager.AppSettings["API.Uri"];
                    if (string.IsNullOrEmpty(this._sfAPIServiceBaseURI))
                        _sfAPIServiceBaseURI = "https://msfapiservice.trafficmanager.net//"; // Production
                }
            }
            catch (Exception)
            {
                _sfAPIServiceBaseURI = "https://msfapiservice.trafficmanager.net//"; // Production
            }

            Logger.showDebug("RestfulAPIHelper", "sfAPIServiceBaseURI={0}".FormatInvariant(_sfAPIServiceBaseURI));
            _sfAPIServiceTokenEndPoint = _sfAPIServiceBaseURI + "token";
            _sfAPIServiceDeviceAPIURI = _sfAPIServiceBaseURI + "device-api/device";
            _currentDeviceToken = null;

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
        }

        public async Task<string> getDeviceMessageSchemaAPIService(string deviceId)
        {
            string endPointURI = _sfAPIServiceDeviceAPIURI + "/" + deviceId + "/MessageSchema";
            var request = (HttpWebRequest)WebRequest.Create(endPointURI);

            request.Method = "GET";
            HttpWebResponse response = null;
            try
            {
                request.ContentType = "application/x-www-form-urlencoded";
                string token = getCurrentToken();
                if (token != null)
                    request.Headers.Add("Authorization", "bearer " + token);

                response = (HttpWebResponse)request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                var httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (getAPIToken())
                        return await getDeviceMessageSchemaAPIService(deviceId);
                }
                else
                {
                    StringBuilder s = LogUtility.BuildExceptionMessage(ex);
                    s.AppendLine("getDeviceMessageSchemaAPIService StatusCode:" + response.StatusCode.ToString() + "- WebException: " + ex.Message.ToString());
                    throw new Exception(s.ToString());
                }

            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("EndPoint:" + endPointURI);
                logMessage.AppendLine("deviceId:" + deviceId);
                Logger.showError(TAG, logMessage);
                throw new Exception(logMessage.ToString());
            }

            return null;
        }

        /* Using HttpWebRequest to upload log */
        public async Task<string> uploadDeviceLogAPIService(string deviceId, FileInfo fileToUpload)
        {
            string endPointURI = _sfAPIServiceDeviceAPIURI + "/" + deviceId + "/log";
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPointURI);
            request.Method = "POST";
            request.KeepAlive = true;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            HttpWebResponse response = null;
            try
            {
                string token = getCurrentToken();
                if (token != null)
                    request.Headers.Add("Authorization", "bearer " + token);

                Stream requestStream = request.GetRequestStream();
                if (fileToUpload != null)
                {
                    string mimeType = MimeMapping.GetMimeMapping(fileToUpload.Name);
                    fileToUpload.WriteMultipartFormData(requestStream, boundary, mimeType, fileToUpload.Name);
                }
                byte[] endBytes = System.Text.Encoding.UTF8.GetBytes("--" + boundary + "--");
                requestStream.Write(endBytes, 0, endBytes.Length);

                requestStream.Close();

                response = request.GetResponse() as HttpWebResponse;
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                };
            }
            catch (WebException ex)
            {
                var httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (getAPIToken())
                        return await uploadDeviceLogAPIService(deviceId, fileToUpload);
                }
                else
                {
                    StringBuilder s = LogUtility.BuildExceptionMessage(ex);
                    s.AppendLine("getDeviceMessageSchemaAPIService StatusCode:" + response.StatusCode.ToString() + "- WebException: " + ex.Message.ToString());
                    throw new Exception(s.ToString());
                }
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("EndPoint:" + endPointURI);
                logMessage.AppendLine("FileName:" + fileToUpload.FullName);
                Logger.showError(TAG, logMessage);
                throw new Exception(logMessage.ToString());
            }

            return null;
        }

        public async Task<string> callDeviceAPIService(string method, string deviceId, string postData)
        {
            string endPointURI = _sfAPIServiceDeviceAPIURI + "/" + deviceId;
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPointURI);

            request.Method = method;
            HttpWebResponse response = null;
            try
            {
                request.ContentType = "application/x-www-form-urlencoded";

                string token = getCurrentToken();
                if (token != null)
                    request.Headers.Add("Authorization", "bearer " + token);

                switch (method.ToLower())
                {
                    case "get":
                    case "delete":
                        response = request.GetResponse() as HttpWebResponse;
                        break;
                    case "post":
                    case "put":
                        using (Stream requestStream = request.GetRequestStream())
                        using (StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII))
                        {
                            writer.Write(postData);
                        }
                        response = (HttpWebResponse)request.GetResponse();
                        break;
                    default:
                        throw new Exception("Method:" + method + " Not Support");
                }
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                var httpResponse = (HttpWebResponse)ex.Response;

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    if (getAPIToken())
                        return await callDeviceAPIService(method, deviceId, postData);
                }
                else
                {
                    StringBuilder s = LogUtility.BuildExceptionMessage(ex);
                    s.AppendLine("callDeviceAPIService StatusCode:" + response.StatusCode.ToString() + "- WebException: " + ex.Message.ToString());
                    throw new Exception(s.ToString());
                }
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("EndPoint:" + _sfAPIServiceDeviceAPIURI);
                logMessage.AppendLine("Method:" + method);
                logMessage.AppendLine("PostData:" + postData);
                Logger.showError(TAG, logMessage);
                throw new Exception(logMessage.ToString());
            }

            return null;
        }

        private bool getAPIToken()
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(_sfAPIServiceTokenEndPoint);

            request.Method = "POST";
            HttpWebResponse response = null;
            string postData = "grant_type=password&email=" + _email + "&password=" + _password + "&role=device";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    _currentDeviceToken = JsonConvert.DeserializeObject<DeviceToken>(sr.ReadToEnd());
                }

                if (!string.IsNullOrEmpty(_currentDeviceToken.access_token))
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: User Login Successful.");
                    //logMessage.AppendLine("accessToken:" + currentDeviceToken.access_token);
                    //logMessage.AppendLine("tokenType:" + currentDeviceToken.token_type);
                    Logger.showDebug(TAG, logMessage);
                    return true;
                }
                return false;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Authentication Fail");
                else
                {
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("Failed to get API Token.");
                    logMessage.AppendLine("Url: " + _sfAPIServiceBaseURI);
                    logMessage.AppendLine("Device ID: " + _email);
                    logMessage.AppendLine("Device Password: " + _password);
                    Logger.showDebug(TAG, logMessage);
                    throw new Exception(logMessage.ToString());
                }
            }
        }

        string getCurrentToken()
        {
            DeviceToken deviceToken = _currentDeviceToken;
            if (deviceToken != null && deviceToken.access_token != null)
            {
                if (deviceToken.token_type.ToLower().Equals("bearer"))
                    return deviceToken.access_token;
                else
                {
                    // x509 to do...
                    Logger.showError(TAG, "Unsupported Device API Token Type {0}".FormatInvariant(deviceToken.access_token));
                    throw new InvalidOperationException("Unsupported Device API Token Type {0}".FormatInvariant(deviceToken.access_token));
                }
            }
            else
                return null;
        }
    }
}
