using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace sfShareLib
{
    public enum sfLogLevel
    {
        Info,
        Debug,
        Warn,
        Audit,
        Error
    }
    public class sfLog
    {
        private sfLogLevel _logLevel;
        private CloudBlobContainer _container;

        public sfLog(string storageName, string storageKey, string storageContainer, sfLogLevel logLevel)
        {
            try
            {                
                _logLevel = logLevel;
                
                StorageCredentials creds = new StorageCredentials(storageName, storageKey);
                CloudStorageAccount strAcc = new CloudStorageAccount(creds, true);
                CloudBlobClient blobClient = strAcc.CreateCloudBlobClient();

                _container = blobClient.GetContainerReference(storageContainer);
                _container.CreateIfNotExistsAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void Info(StringBuilder log)
        {
            if (_logLevel <= sfLogLevel.Info)
                WriteToAppendBlob("Info", log.ToString());
        }

        public void Debug(StringBuilder log)
        {
            if (_logLevel <= sfLogLevel.Debug)
                WriteToAppendBlob("Debug", log.ToString());
        }

        public void Warn(StringBuilder log)
        {
            if (_logLevel <= sfLogLevel.Warn)
                WriteToAppendBlob("Warn", log.ToString());
        }

        public void Audit(StringBuilder log)
        {
            if (_logLevel <= sfLogLevel.Audit)
                WriteToAppendBlob("Audit", log.ToString());
        }

        public void Error(StringBuilder log)
        {
            if (_logLevel <= sfLogLevel.Error)
                WriteToAppendBlob("Error", log.ToString());
        }

        public void Info(string log)
        {
            if (_logLevel <= sfLogLevel.Info)
                WriteToAppendBlob("Info", log);
        }

        public void Debug(string log)
        {
            if (_logLevel <= sfLogLevel.Debug)
                WriteToAppendBlob("Debug", log);
        }

        public void Warn(string log)
        {
            if (_logLevel <= sfLogLevel.Warn)
                WriteToAppendBlob("Warn", log);
        }

        public void Audit(string log)
        {
            if (_logLevel <= sfLogLevel.Audit)
                WriteToAppendBlob("Audit", log);
        }

        public void Error(string log)
        {
            if (_logLevel <= sfLogLevel.Error)
                WriteToAppendBlob("Error", log);
        }

        private void WriteToAppendBlob(string logLevel, string log)
        {
            DateTime date = DateTime.Today;
            DateTime dateLogEntry = DateTime.UtcNow;
            try
            {
                CloudAppendBlob appBlob = _container.GetAppendBlobReference(string.Format("{0}{1}", date.ToString("yyyy-MM-dd"), ".log"));
                if (!appBlob.Exists())
                {
                    appBlob.CreateOrReplace();
                }
                appBlob.AppendText(string.Format("{0}\t{1}\t{2}\r\n", dateLogEntry.ToString("o"), logLevel, log));
            }
            catch (Exception)
            {
                ;
            }
        }
    }

    public class LogUtility
    {
        public static StringBuilder BuildExceptionMessage(Exception x, string urlPath)
        {
            StringBuilder message = BuildExceptionMessage(x);

            // Get the QueryString along with the Virtual Path
            message.AppendLine("Raw Url : " + urlPath);
            return message;
        }

        public static StringBuilder BuildExceptionMessage(Exception x)
        {
            Exception logException = x;
            if (x.InnerException != null)
            {
                logException = x.InnerException;
            }

            StringBuilder message = new StringBuilder();
            message.AppendLine();

            // Type of Exception
            message.AppendLine("Type of Exception : " + logException.GetType().Name);

            // Get the error message
            message.AppendLine("Message : " + logException.Message);

            // Source of the message
            message.AppendLine("Source : " + logException.Source);

            // Stack Trace of the error
            message.AppendLine("Stack Trace : " + logException.StackTrace);

            // Method where the error occurred
            message.AppendLine("TargetSite : " + logException.TargetSite);

            return message;
        }
    }
}
