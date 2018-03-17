using IoTHubEventProcessor.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor
{
    class RealTimeMessageSender
    {
        private static string rtMessageFeedInURL = ConfigurationManager.AppSettings["RTMessageFeedInURL"];

        public string PostRealTimeMessage(string postData)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(postData);

                var request = (HttpWebRequest)WebRequest.Create(rtMessageFeedInURL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

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
    }
}
