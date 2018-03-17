using Newtonsoft.Json.Linq;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sfAdmin.Models
{
    public class RestfulAPIHelper
    {
        EmployeeSession empSession = null;
        int companyId = 0;
        public RestfulAPIHelper()
        {            
            if (HttpContext.Current.Session["empSession"] == null)
                throw new Exception("Invalid Session");

            empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
        }

        public RestfulAPIHelper(bool withSession, int companyId)
        {
            if (withSession)
            {
                if (HttpContext.Current.Session["empSession"] == null)
                    throw new Exception("Invalid Session");

                empSession = EmployeeSession.LoadByJsonString(HttpContext.Current.Session["empSession"].ToString());
            }
            this.companyId = companyId;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
        }

        private string SetEndPointURI(string endPointURI)
        {
            string settleEndPointURI = endPointURI;
            if (empSession!= null && empSession.companyId > 0)
                settleEndPointURI = settleEndPointURI.Replace("{companyId}", empSession.companyId.ToString());
            else
                settleEndPointURI = settleEndPointURI.Replace("{companyId}", companyId.ToString());

            return settleEndPointURI;
        }

        public async Task<string> callAPIService(string method, string endPointURI, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(SetEndPointURI(endPointURI));
            request.Method = method;
            HttpWebResponse response = null;

            try
            {
                request.ContentType = "application/x-www-form-urlencoded";
                if (empSession != null)
                    request.Headers.Add("Authorization", "Bearer " + empSession.accessToken);

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

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && empSession != null)
                {
                    if (await getAPIToken())
                        return await callAPIService(method, endPointURI, postData);
                }
                else
                    throw new Exception(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("EndPoint:" + endPointURI);
                logMessage.AppendLine("Method:" + method);
                logMessage.AppendLine("PostData:" + postData);
                Global._sfAppLogger.Error(logMessage);
                throw;
            }

            return null;
        }

        public async Task<string> putUploadFile(string endPointURI, byte[] image, string imageFileName)
        {
            HttpClientHandler handler = new HttpClientHandler();
            using (var client = new HttpClient(handler, false))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", empSession.accessToken);
                var content = new MultipartFormDataContent("Upload----" + DateTime.Now.ToString(CultureInfo.InvariantCulture));
                content.Add(new StreamContent(new MemoryStream(image)), "image", imageFileName);

                HttpResponseMessage response = null;

                try
                {
                    response = await client.PutAsync(endPointURI, content);
                    if (response.IsSuccessStatusCode)
                        return "Completed.";

                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        if (await getAPIToken())
                            return await putUploadFile(endPointURI, image, imageFileName);
                    }
                    return response.ReasonPhrase;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public string changePassword(string method, string endPointURI, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(SetEndPointURI(endPointURI));
            request.Method = method;
            HttpWebResponse response = null;
            try
            {
                request.ContentType = "application/x-www-form-urlencoded";
                if (empSession != null)
                    request.Headers.Add("Authorization", "Bearer " + empSession.accessToken);

                switch (method.ToLower())
                {
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
                    throw new Exception("Old Password Not Match.");
                else
                    throw new Exception(response.StatusCode.ToString());
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("EndPoint:" + endPointURI);
                logMessage.AppendLine("Method:" + method);
                logMessage.AppendLine("PostData:" + postData);
                Global._sfAppLogger.Error(logMessage);
                throw;
            }
        }

        private async Task<bool> getAPIToken()
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = null;
            string tokenRole = Global._sfAPIServiceTokenRole;

            if (HttpContext.Current.Session["loginBySA"] != null && bool.Parse(HttpContext.Current.Session["loginBySA"].ToString()))
            {
                tokenRole = "superadmin";
            }

            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "email", empSession.email },
                { "password", empSession.password },
                { "role", tokenRole }
            });

            string uri = Global._sfAPIServiceTokenEndPoint;
            response = await client.PostAsync(uri, content);
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                dynamic access_result = JObject.Parse(result);
                string access_token = access_result.access_token;
                if (!string.IsNullOrEmpty(access_token))
                {
                    Employee employee = new Employee();
                    this.empSession = employee.afterLoginInitial(access_token, access_result);

                    string permissionJson = await callAPIService("GET", Global._employeeEndPoint + "/" + access_result.Id + "/Permissions", null);
                    string externalDashboardJson = await callAPIService("GET", Global._externalDashboardEndPoint, null);
                    employee.initialPermission(permissionJson, externalDashboardJson);
                    //employee.initialLang((string)access_result.Lang);
                    return true;
                }
                return false;
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new Exception("Authentication Fail");
                else
                    throw new Exception();
            }
        }
    }
}