using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CDS.Devices.Client.Utility
{
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
