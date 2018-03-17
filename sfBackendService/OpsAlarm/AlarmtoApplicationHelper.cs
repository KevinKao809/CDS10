using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using sfShareLib;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using System.Collections.Specialized;

namespace OpsAlarm
{
    class AlarmMsgModel
    {
        public int messageCatalogId { get; set; }
        public int alarmRuleCatalogId { get; set; }
        public DateTime triggeredTime { get; set; }
        public JObject messagePayload { get; set; }
    }
    class AlarmtoApplicationHelper
    {
        int _MessageCatalogId;
        int _AlarmRuleCatalogId;
        DateTime _TriggeredTime;
        dynamic _Message;
        dynamic _FullAlarmMessage;

        public AlarmtoApplicationHelper(JObject msgObj)
        {
            _MessageCatalogId = int.Parse(msgObj["MessageCatalogId"].ToString());
            _AlarmRuleCatalogId = int.Parse(msgObj["AlarmRuleCatalogId"].ToString());
            _TriggeredTime = DateTime.Parse(msgObj["TriggeredTime"].ToString());
            _Message = JObject.Parse(msgObj["Message"].ToString());
            _FullAlarmMessage = JObject.Parse(msgObj.ToString());

            DBHelper._AlarmRuleCatalog dbHelp = new DBHelper._AlarmRuleCatalog();
            AlarmRuleCatalog alarmRuleCatalog = dbHelp.GetByid(_AlarmRuleCatalogId);
            if (alarmRuleCatalog != null)
            {
                _Message["AlarmRuleCatalogName"] = alarmRuleCatalog.Name;
                _Message["AlarmRuleCatalogDescription"] = alarmRuleCatalog.Description;
            }            
        }

        public void ThreadProc()
        {
            SFDatabaseEntities dbEnty = new SFDatabaseEntities();
            List<ExternalApplication> appList = (from alarm in dbEnty.AlarmNotification
                                                 join app in dbEnty.ExternalApplication on alarm.ExternalApplicationId equals app.Id
                                                 where alarm.AlarmRuleCatalogId == _AlarmRuleCatalogId
                                                 select app).ToList<ExternalApplication>();
            string exceptionString = "";
            foreach (var app in appList)
            {
                string applicationTargetType = app.TargetType.ToLower();
                WebUtility webUtitlity = new WebUtility();
                try
                {
                    string response = null;
                    JObject outputTemplate = new JObject();
                    switch (applicationTargetType)
                    {
                        case "external":
                            outputTemplate = ParsingOutputTemplate(app.MessageTemplate);
                            switch (app.Method.ToLower())
                            {
                                case "post-x-www":
                                    string postData = ConvertJObjectToQueryString(outputTemplate);
                                    switch (app.AuthType.ToLower())
                                    {
                                        case "none":
                                            
                                            response = webUtitlity.PostContent(app.ServiceURL, postData);
                                            break;
                                        case "basic auth":
                                            response = webUtitlity.PostContent(app.ServiceURL, postData, app.AuthID, app.AuthPW);
                                            break;
                                    }
                                    break;
                                case "post-multi":
                                    NameValueCollection formData = new NameValueCollection();
                                    foreach (var elem in outputTemplate)
                                    {
                                        formData.Add(elem.Key, outputTemplate[elem.Key].ToString());
                                    }
                                    switch (app.AuthType.ToLower())
                                    {
                                        case "none":
                                            response = webUtitlity.PostMultipartContent(app.ServiceURL, formData);
                                            break;
                                        case "basic auth":
                                            break;
                                    }
                                    break;
                                case "post-json":
                                    switch (app.AuthType.ToLower())
                                    {
                                        case "none":
                                            response = webUtitlity.PostJsonContent(app.ServiceURL, JsonConvert.SerializeObject(outputTemplate));
                                            break;
                                        case "basic auth":
                                            response = webUtitlity.PostJsonContent(app.ServiceURL, JsonConvert.SerializeObject(outputTemplate), app.AuthID, app.AuthPW);
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case "dashboard":
                            string defaultUrl = ConfigurationManager.AppSettings["RTMessageFeedInURL"];
                            response = webUtitlity.PostContent(defaultUrl, JsonConvert.SerializeObject(_FullAlarmMessage));
                            if (!response.Contains("OK"))
                                throw new Exception("RTMessageFeedIn Return: " + response);
                            break;
                        case "iot device":
                            string iotDeviceId = app.ServiceURL;
                            DBHelper._IoTDevice dbhelp = new DBHelper._IoTDevice();
                            IoTDevice iotDevice = dbhelp.GetByid(iotDeviceId);

                            ServiceClient serviceClient = null ;
                            try
                            {
                                serviceClient = ServiceClient.CreateFromConnectionString(iotDevice.IoTHub.P_IoTHubConnectionString);
                                outputTemplate = ParsingOutputTemplate(app.MessageTemplate);
                                var msg = new Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(outputTemplate)));
                                serviceClient.SendAsync(iotDeviceId, msg);
                            }
                            catch (Exception ex)
                            {
                                Program._sfAppLogger.Error("External App:" + app.ServiceURL + "; Exception:" + ex.Message);
                            }                            
                        break;
                    }
                    Program._sfAppLogger.Debug("External App:" + app.ServiceURL + "; Result:" + response);
                }
                catch(Exception ex)
                {
                    exceptionString += "Push externalApplication " + app.Name + "(id:" +app.Id + ") failed: " + ex.Message + "\n";
                    continue;
                }

                Console.WriteLine("Push externalApplication success(type: " + app.TargetType + ")");
            }

            if (!string.IsNullOrEmpty(exceptionString))
            {
                Console.WriteLine(exceptionString);
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Exception: " + exceptionString);
                logMessage.AppendLine("\tMessageCatalogId:" + _MessageCatalogId);
                logMessage.AppendLine("\tAlarmRuleCatalogId:" + _AlarmRuleCatalogId);
                logMessage.AppendLine("\tMessagePayload:" + JsonConvert.SerializeObject(_Message));

                Program._sfAppLogger.Error(logMessage);
            }
            else
            {
                Console.WriteLine("Push all external application success!");
            }
        }
        

        private JObject ParsingOutputTemplate(string messageTemplate)
        {
            JObject outputTemplate = JObject.Parse(messageTemplate);

            foreach (var elem in outputTemplate)
            {
                string valueStr = elem.Value.ToString();
                List<int> allAtIndexOfString = AllIndexesOf(valueStr, "@");

                if (allAtIndexOfString.Count > 0 && (allAtIndexOfString.Count % 2 == 0))
                {
                    Dictionary<string, string> strMappintReplacement = new Dictionary<string, string>();
                    for (int index = 0; index < allAtIndexOfString.Count; index += 2)
                    {
                        int length = allAtIndexOfString[index + 1] - allAtIndexOfString[index] + 1;
                        string waitReplaceStr = valueStr.Substring(allAtIndexOfString[index], length);
                        string messageKey = waitReplaceStr.Replace("@", "");
                        
                        string replaceStr = _Message[messageKey];
                        if (!strMappintReplacement.ContainsKey(waitReplaceStr))
                            strMappintReplacement.Add(waitReplaceStr, replaceStr);
                    }
                    foreach (var key in strMappintReplacement.Keys)
                    {
                        string replaceStr = strMappintReplacement[key];
                        valueStr = valueStr.Replace(key, replaceStr);
                    }
                    outputTemplate[elem.Key] = valueStr;
                }
            }
            return outputTemplate;
        }
        private List<int> AllIndexesOf(string str, string value)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            List<int> indexes = new List<int>();
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return indexes;
                indexes.Add(index);
            }
        }

        private string ConvertJObjectToQueryString(JObject jObj)
        {
            string queryString = "";
            foreach (var obj in jObj)
            {
                queryString += obj.Key + "=" + obj.Value + "&";
            }

            if(!string.IsNullOrEmpty(queryString))
                queryString = queryString.Substring(0, queryString.Length - 1);

            return queryString;
        }
    }
}
