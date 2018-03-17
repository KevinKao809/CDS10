using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OpsInfra
{
    class IoTHubEventProcessorHelper
    {
        public static string _sfSrvFabricBaseURI = ConfigurationManager.AppSettings["sfSrvFabricBaseURI"];
        public static string _sfSrvFabricIoTHubTypeName = ConfigurationManager.AppSettings["sfSrvFabricIoTHubTypeName"];
        public static string _sfSrvFabricIoTHubTypeVersion = ConfigurationManager.AppSettings["sfSrvFabricIoTHubTypeVersion"];
        public static string _sfSrvFabricCertificate = ConfigurationManager.AppSettings["sfSrvFabricCertificateURI"];
        public static string _sfSrvFabricCertificatePW = ConfigurationManager.AppSettings["sfSrvFabricCertificatePW"];
        public static byte[] _certbytes;

        public string _IoTHubAlias;
        public int _TaskId;
        public string _Action;


        public IoTHubEventProcessorHelper(string IoTHubAlias, int taskId, string action)
        {
            _IoTHubAlias = IoTHubAlias;
            _TaskId = taskId;
            if (action.StartsWith("create"))
                _Action = "create";
            else if (action.StartsWith("remove"))
                _Action = "remove";
            else
                _Action = "";
        }

        public void ThreadProc()
        {
            try
            {
                switch (_Action)
                {
                    case "create":
                        CreateIoTHubEventProcessorApplication(_IoTHubAlias);
                        Program._sfAppLogger.Info("Create IoTHubEventProcessor Application completed. IoTHub Alias:" + _IoTHubAlias);
                        break;
                    case "remove":
                        RemoveIoTHubEventProcessorApplication(_IoTHubAlias);
                        Program._sfAppLogger.Info("Remove IoTHubEventProcessor Application completed. IoTHub Alias:" + _IoTHubAlias);
                        break;
                }
                Program.UpdateTaskBySuccess(_TaskId);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Create/Remove IoTHubEventProcessor Application fail. IoTHub Alias:" + _IoTHubAlias);
                logMessage.AppendLine("\tException:" + ex.Message);
                Program._sfAppLogger.Error(logMessage);
                Program.UpdateTaskByFail(_TaskId, Program.FilterErrorMessage(ex.Message));
                Console.WriteLine(logMessage);
            }
        }

        private void CreateIoTHubEventProcessorApplication(string IoTHubAlias)
        {
            string endPoint = "Applications/$/Create?api-version=1.0";
            string postData = "{\"Name\":\"fabric:/IoTHubEP_" + IoTHubAlias + "\",\"TypeName\":\"" + _sfSrvFabricIoTHubTypeName + "\",\"TypeVersion\":\"" + _sfSrvFabricIoTHubTypeVersion + "\",\"ParameterList\":{\"input_IoTHubAlias\":\"" + IoTHubAlias + "\",\"IoTHubEventProcessor_InstanceCount\":\"1\" }}";
            CallServiceFabricAPI(endPoint, postData);
        }

        private void RemoveIoTHubEventProcessorApplication(string IoTHubAlias)
        {
            string appName = "IoTHubEP_" + IoTHubAlias;
            string endPoint = "Applications/" + appName  + "/$/Delete?api-version=1.0";
            CallServiceFabricAPI(endPoint, null);
        }

        private void CallServiceFabricAPI(string endPoint, string rawData)
        {
            HttpWebRequest req = null;
            string URI = _sfSrvFabricBaseURI + endPoint;

            try
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback((sender, certificate, chain, policyErrors) => { return true; });

                req = (HttpWebRequest)WebRequest.Create(URI);
                req.Method = "POST";
                req.Headers.Add("api-version", "1.0");

                if (URI.ToLower().StartsWith("https://"))
                {
                    if (_certbytes == null)
                        _certbytes = loadServiceFabricCertificate();
                    X509Certificate2 cert = new X509Certificate2(_certbytes, _sfSrvFabricCertificatePW);
                    req.ClientCertificates.Add(cert);
                }

                if (!string.IsNullOrEmpty(rawData))
                {
                    using (Stream stm = req.GetRequestStream())
                    {
                        using (StreamWriter stmw = new StreamWriter(stm))
                        {
                            stmw.Write(rawData);
                            stmw.Close();
                        }
                    }
                    WebResponse response = req.GetResponse();
                    Stream responseStream = response.GetResponseStream();
                    response = req.GetResponse();
                    StreamReader sr = new StreamReader(response.GetResponseStream());
                    string result = sr.ReadToEnd();
                    sr.Close();
                    if (req != null) req.GetRequestStream().Close();
                }
                else
                {
                    req.ContentLength = 0;
                    WebResponse response = req.GetResponse();
                }                
            }
            catch (Exception ex)
            {
                throw;
            }            
        }

        private byte[] loadServiceFabricCertificate()
        {
            try
            {
                StorageCredentials creds = new StorageCredentials(ConfigurationManager.AppSettings["sfLogStorageName"], ConfigurationManager.AppSettings["sfLogStorageKey"]);
                CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);
                CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

                string certStore = ConfigurationManager.AppSettings["sfSrvFabricCertificate"];
                int div = certStore.IndexOf("/");
                string containerName = certStore.Substring(0, div);
                string certFile = certStore.Substring(div + 1, certStore.Length-(div+1));

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(certFile);
                blockBlob.FetchAttributes();
                byte[] certbytes = new byte[blockBlob.Properties.Length];
                blockBlob.DownloadToByteArray(certbytes, 0);
                return certbytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
