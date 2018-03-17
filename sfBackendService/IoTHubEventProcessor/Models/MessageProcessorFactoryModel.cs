using IoTHubEventProcessor.Helper;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Models
{
    public class MessageProcessorFactoryModel
    {
        public SfDocumentDBHelper SfDocumentDBHelper { get; set; }
        public BlobStorageHelper SfblobStorageHelper { get; set; }
        public QueueClient SfQueueClient { get; set; }
        public QueueClient SfInfraQueueClient { get; set; }
        public List<SimpleIoTDeviceMessageCatalog> SimpleIoTDeviceMessageCatalogList { get; set; }
        public Dictionary<int, List<AlarmRuleCatalogEngine>> MessageIdAlarmRules { get; set; }

        public string IoTHubAlias { get; set; }

        public bool IsIoTHubPrimary { get; set; }
    }
}
