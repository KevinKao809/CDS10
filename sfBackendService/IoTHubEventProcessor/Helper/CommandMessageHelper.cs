using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using sfShareLib;

namespace IoTHubEventProcessor.Helper
{
    class CommandMessageHelper
    {
        public string CommandName { get; set; }
        public JObject Payload { get; set; }

        public CommandMessageHelper(string commandName, JObject payload)
        {
            CommandName = commandName;
            Payload = payload;
        }
    }
}
