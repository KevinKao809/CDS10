using Microsoft.CDS.Devices.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.CDS.Devices.Client.Models
{
    class MessageCatalogSchema
    {
        public class ElementSchema
        {            
            public string Name { get; set; }
            public string DataType { get; set; }
            public bool MandatoryFlag { get; set; }
        }

        public int MessageCatalogId { get; set; }
        public string Name { get; set; }
        public List<ElementSchema> ElementList { get; set; }

        public static List<MessageCatalogSchema> CreateMessageCatalogSchemaList(string s)
        {
            try
            {
                List<MessageCatalogSchema> messageCatalogSchema = JsonConvert.DeserializeObject<List<MessageCatalogSchema>>(s);
                //if (messageCatalogSchema == null || messageCatalogSchema.Count == 0)
                if (messageCatalogSchema == null)
                    throw new Exception("No Available Message Catalog.");
                return messageCatalogSchema;
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = LogUtility.BuildExceptionMessage(ex);
                logMessage.AppendLine("CreateMessageCatalogSchemaList - failed to parse string to MessageCatalogSchema List");
                Logger.showError("MessageCatalogSchema", logMessage);
                throw;
            }
        }
    }
}
