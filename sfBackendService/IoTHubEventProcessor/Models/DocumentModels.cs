using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Models
{
    public class DocumentType
    {
        public static string MessageDocument = "Message";
        public static string AlarmDocument = "Alarm";
    }

    public class MessageDocument
    {
        /* Each document in DocumentDB has an id property which uniquely identifies 
        * the document but that id field is of type string. 
        * When creating a document, you may choose not to specify a value for this field 
        * and DocumentDB assigns an id automatically but this value is a GUID. */
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Type { get; set; }
        public string DeviceId { get; set; }
        public int MessageCatalogId { get; set; }
        public JObject Message { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class AlarmDocument
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string Type { get; set; }
        public int MessageCatalogId { get; set; }
        public int AlarmRuleCatalogId { get; set; }
        public string AlarmRuleCatalogName { get; set; }
        public string AlarmRuleCatalogDescription { get; set; }
        public string TriggeredTime { get; set; }
        public bool AlarmSent { get; set; }
        public string MessageDocumentId { get; set; }
        public JObject Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
