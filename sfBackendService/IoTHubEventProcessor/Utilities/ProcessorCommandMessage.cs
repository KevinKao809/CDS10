using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTHubEventProcessor.Utilities
{
    class ProcessorCommandMessage
    {
        public string command;
        public string program;
        public string parameters;
        public string requester;
        public string requesterEmail;
        public DateTime requestDateTime;
        public int taskId;

        private static string PROCESSOR_COMMAND_TOPIC_NAME = "processcommand";

        public ProcessorCommandMessage() { }

        public ProcessorCommandMessage(string command, string parameters, string requester, string requesterEmail)
        {
            switch (command.ToLower())
            {
                case "start iothub receiver":
                    this.command = "Start";
                    break;
                case "restart iothub receiver":
                    this.command = "Restart";
                    break;
                case "stop iothub receiver":
                    this.command = "Stop";
                    break;
                case "shutdown iothub receiver":
                    this.command = "Shutdown";
                    break;
                default:
                    throw new NotSupportedException("Not Supported Command Exception="+command.ToLower());
            }
            this.parameters = parameters;
            this.program = requester;
            this.requester = requester;
            this.requesterEmail = requesterEmail;
            this.requestDateTime = DateTime.UtcNow;
        }

        public string GetJsonContent()
        {
            return JsonConvert.SerializeObject(this);
        }

        public void PublishProcessorCommandMessage()
        {
            var namespaceManager = NamespaceManager.CreateFromConnectionString(Program._sbConnectionString);

            if (!namespaceManager.TopicExists(PROCESSOR_COMMAND_TOPIC_NAME))
            {
                namespaceManager.CreateTopic(PROCESSOR_COMMAND_TOPIC_NAME);
            }

            TopicClient Client = TopicClient.CreateFromConnectionString(Program._sbConnectionString, PROCESSOR_COMMAND_TOPIC_NAME);
            BrokeredMessage message = new BrokeredMessage(GetJsonContent());

            if (command == "Launch Program")
            {
                message.Properties["process"] = "launcher";
            }
            else
            {
                message.Properties["process"] = "iothubeventprocessor";
                message.Properties["iothubalias"] = parameters;
            }
            Client.Send(message);
        }
    }
}
