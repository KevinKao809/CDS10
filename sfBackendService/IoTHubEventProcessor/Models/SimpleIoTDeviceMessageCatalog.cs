using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor
{
    public class SimpleIoTDeviceMessageCatalog
    {
        public string DeviceId { get; set; }
        public List<int> MessageCatalogIds { get; set; }
    }
}
