using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace sfShareLib
{
    public class WebUtility
    {
        public string PostContent(string postURL, string postData, string basicAuthId = null, string basicAuthPW = null)
        {
            try
            {
                //var data = Encoding.UTF8.GetBytes(postData);
                var request = (HttpWebRequest)WebRequest.Create(postURL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = data.Length;

                if(!string.IsNullOrEmpty(basicAuthId) && !string.IsNullOrEmpty(basicAuthPW))
                {
                    /* Add Authentication Header Here */
                    string authString = basicAuthId + ":" + basicAuthPW;
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(authString));
                }

                using (Stream requestStream = request.GetRequestStream())
                using (StreamWriter writer = new StreamWriter(requestStream, Encoding.ASCII))
                {
                    writer.Write(postData);
                }
                var response = (HttpWebResponse)request.GetResponse();

                //using (var stream = request.GetRequestStream())
                //{
                //    stream.Write(data, 0, data.Length);
                //}

                //var response = (HttpWebResponse)request.GetResponse();
                //return new StreamReader(response.GetResponseStream()).ReadToEnd();
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string PostJsonContent(string postURL, string jsonString, string basicAuthId = null, string basicAuthPW = null)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(jsonString);

                var request = (HttpWebRequest)WebRequest.Create(postURL);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                if (!string.IsNullOrEmpty(basicAuthId) && !string.IsNullOrEmpty(basicAuthPW))
                {
                    /* Add Authentication Header Here */
                    string authString = basicAuthId + ":" + basicAuthPW;
                    request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(authString));
                }

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                return new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
        public string PostMultipartContent(string URL, NameValueCollection formData, string FilePath = null, string basicAuthId = null, string basicAuthPW = null)
        {
            //string URLAuth = "https://technet.rapaport.com/HTTP/Authenticate.aspx";
            //WebClient webClient = new WebClient();
            //NameValueCollection authData = new NameValueCollection();
            //authData["Username"] = "myUser";
            //authData["Password"] = "myPassword";

            //byte[] responseBytes = webClient.UploadValues(URLAuth, "POST", authData);
            //string resultAuthTicket = Encoding.UTF8.GetString(responseBytes);
            //webClient.Dispose();

            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            System.Net.WebRequest webRequest = System.Net.WebRequest.Create(URL);

            webRequest.Method = "POST";
            webRequest.ContentType = "multipart/form-data; boundary=" + boundary;

            if (!string.IsNullOrEmpty(basicAuthId) && !string.IsNullOrEmpty(basicAuthPW))
            {
                /* Add Authentication Header Here */
                string authString = basicAuthId + ":" + basicAuthPW;
                webRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(authString));
            }

            Stream postDataStream = GetPostStream(FilePath, formData, boundary);

            webRequest.ContentLength = postDataStream.Length;
            Stream reqStream = webRequest.GetRequestStream();

            postDataStream.Position = 0;

            byte[] buffer = new byte[1024];
            int bytesRead = 0;

            while ((bytesRead = postDataStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                reqStream.Write(buffer, 0, bytesRead);
            }

            postDataStream.Close();
            reqStream.Close();

            var response = (HttpWebResponse)webRequest.GetResponse();
            return new StreamReader(response.GetResponseStream()).ReadToEnd();
        }
        
        private static Stream GetPostStream(string filePath, NameValueCollection formData, string boundary)
        {
            Stream postDataStream = new System.IO.MemoryStream();

            //adding form data
            string formDataHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
            "Content-Disposition: form-data; name=\"{0}\";" + Environment.NewLine + Environment.NewLine + "{1}";

            foreach (string key in formData.Keys)
            {
                byte[] formItemBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(formDataHeaderTemplate,
                key, formData[key]));
                postDataStream.Write(formItemBytes, 0, formItemBytes.Length);
            }

            //adding file data
            if (filePath != null)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                string fileHeaderTemplate = Environment.NewLine + "--" + boundary + Environment.NewLine +
                "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"" +
                Environment.NewLine + "Content-Type: application/vnd.ms-excel" + Environment.NewLine + Environment.NewLine;

                byte[] fileHeaderBytes = System.Text.Encoding.UTF8.GetBytes(string.Format(fileHeaderTemplate,
                "UploadCSVFile", fileInfo.FullName));

                postDataStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);

                FileStream fileStream = fileInfo.OpenRead();

                byte[] buffer = new byte[1024];

                int bytesRead = 0;

                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    postDataStream.Write(buffer, 0, bytesRead);
                }

                fileStream.Close();

            }

            byte[] endBoundaryBytes = System.Text.Encoding.UTF8.GetBytes(Environment.NewLine + "--" + boundary + "--");
            postDataStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);

            return postDataStream;
        }

    }
}
