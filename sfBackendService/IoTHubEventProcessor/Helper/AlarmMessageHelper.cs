using Newtonsoft.Json.Linq;
using sfShareLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Helper
{
    public class AlarmMessageHelper
    {
        public int MessageId { get; set; }
        public int AlarmRuleId { get; set; }
        public string AlarmRuleName { get; set; }
        public string AlarmRuleDescription { get; set; }
        public string TriggeredTime { get; set; }
        public bool AlarmSent { get; set; }
        public string MessageDocumentId { get; set; }
        public JObject Payload { get; set; }

        public AlarmMessageHelper(AlarmRuleCatalog alarmRuleCatalog, 
            string triggeredTime, bool alarmSent, 
            string messageDocumentId, JObject payload)
        {
            MessageId = alarmRuleCatalog.MessageCatalogId;
            AlarmRuleId = alarmRuleCatalog.Id;
            AlarmRuleName = alarmRuleCatalog.Name;
            AlarmRuleDescription = alarmRuleCatalog.Description;
            TriggeredTime = triggeredTime;
            AlarmSent = alarmSent;
            MessageDocumentId = messageDocumentId;
            Payload = payload;
        }
    }

    
}
