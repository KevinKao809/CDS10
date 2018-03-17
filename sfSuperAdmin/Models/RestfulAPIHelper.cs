using Newtonsoft.Json.Linq;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace sfSuperAdmin.Models
{
    public class RestfulAPIHelper
    {
        public RestfulAPIHelper()
        {
            if (HttpContext.Current.Session["email"] == null || HttpContext.Current.Session["password"] == null)
                throw new Exception("Invalid Session");

            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });
        }

        public async Task<string> callAPIService(string method, string endPointURI, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPointURI);
            request.Method = method;
            HttpWebResponse response = null;
            try
            {
                request.ContentType = "application/x-www-form-urlencoded";
                if (HttpContext.Current.Session["access_token"] != null)
                    request.Headers.Add("Authorization", "Bearer " + HttpContext.Current.Session["access_token"].ToString());
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
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", HttpContext.Current.Session["access_token"] != null ? HttpContext.Current.Session["access_token"].ToString() : "");
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
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(endPointURI);
            request.Method = method;
            HttpWebResponse response = null;
            try
            {
                request.ContentType = "application/x-www-form-urlencoded";
                if (HttpContext.Current.Session["access_token"] != null)
                    request.Headers.Add("Authorization", "Bearer " + HttpContext.Current.Session["access_token"].ToString());

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

            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "password" },
                { "email", HttpContext.Current.Session["email"].ToString() },
                { "password", HttpContext.Current.Session["password"].ToString() },
                { "role", Global._sfAPIServiceTokenRole }
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
                    HttpContext.Current.Session["access_token"] = access_token;
                    HttpContext.Current.Session["id"] = access_result.Id;
                    HttpContext.Current.Session["firstName"] = access_result.FirstName;
                    HttpContext.Current.Session["lastName"] = access_result.LastName;
                    HttpContext.Current.Session["email"] = access_result.Email;
                    HttpContext.Current.Session["issued"] = access_result.issued;
                    HttpContext.Current.Session["expires"] = access_result.expires;
                    StringBuilder logMessage = new StringBuilder();
                    logMessage.AppendLine("audit: User Login Successful.");
                    logMessage.AppendLine("email:" + HttpContext.Current.Session["email"].ToString());
                    Global._sfAuditLogger.Audit(logMessage);
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