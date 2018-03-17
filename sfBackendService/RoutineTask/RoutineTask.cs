using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using RoutineTask.Interfaces;
using System.Net;
using System.IO;
using System.Text;
using sfShareLib;
using System.Diagnostics;
using System.Dynamic;
using Newtonsoft.Json;


namespace RoutineTask
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.None)]
    internal class RoutineTask : Actor, IRoutineTask, IRemindable
    {
        protected static PerformanceCounter _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        protected static PerformanceCounter _ramCounter = new PerformanceCounter("Memory", "Available MBytes");

        static int _ProcessId = Process.GetCurrentProcess().Id;
        static sfShareLib.WebUtility webUtility = new sfShareLib.WebUtility();
        /// <summary>
        /// Initializes a new instance of RoutineTask
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public RoutineTask(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task StartHost()
        {
            /* register all reminder hear */
            TimeSpan timeSpan = TimeSpan.FromSeconds(0);

            /* send heartBeat every 10 seconds */
            await this.RegisterReminderAsync("HeartBeat", null, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));

            /* calculate usage log at 01:00 everyday */
            timeSpan = GetSettingTimeSpan(1, 0);
            await this.RegisterReminderAsync("CalculateUsageLog", null, timeSpan, TimeSpan.FromDays(1));

            return;
        }

        public Task StartTask(string taskName)
        {
            Action action = new Action(taskName);
            switch (taskName)
            {
                case "CalculateUsageLog":
                    action = new Action_CalculateUsageLog(taskName);                    
                    break;
            }
            action.Exec();
            return Task.FromResult(true);
        }

        public Task ReceiveReminderAsync(string reminderName, byte[] context, TimeSpan dueTime, TimeSpan period)
        {
            switch (reminderName)
            {
                case "HeartBeat":
                    {
                        //PushDataToPowerBIAPI("https://api.powerbi.com/beta/72f988bf-86f1-41af-91ab-2d7cd011db47/datasets/ba983a0f-2f65-473d-958d-34e1e453c681/rows?key=gGHbn7R8Am6F0LMNar9DvOmqGiHKJBFf04NDKTnBQWrv6eIs%2FoCo11C9%2Bv39Md7NN1HEKQmnKJv5odhQYQFlvA%3D%3D");
                        ActorEventSource.Current.ActorMessage(this, "Send host actor heartbeat");
                        PushHeartbeatSignal();
                    }
                    break;
                case "CalculateUsageLog":
                    {
                        ActorEventSource.Current.ActorMessage(this, "Create New Actor : CalculateUsageLog");
                        var proxy = ActorProxy.Create<IRoutineTask>(ActorId.CreateRandom());
                        proxy.StartTask("CalculateUsageLog");
                    }
                    break;
            }

            return Task.FromResult(true);
        }

        TimeSpan GetSettingTimeSpan(int hours, int minuts = 0, int seconds = 0)
        {
            DateTime datetime_today = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd"));
            DateTime dateTime_willSet = datetime_today.AddHours(hours);
            dateTime_willSet = dateTime_willSet.AddMinutes(minuts);
            dateTime_willSet = dateTime_willSet.AddSeconds(seconds);
            TimeSpan timeSpan = dateTime_willSet - DateTime.Now;

            return timeSpan.TotalSeconds > 0 ? timeSpan : timeSpan + TimeSpan.FromDays(1);
        }

        static void PushHeartbeatSignal()
        {
            string jsonHB = GetHeartbeatStatus();
            try
            {
                webUtility.PostContent(Program._superadminHeartbeatURL, jsonHB);
                Console.WriteLine("Heartbeat: " + jsonHB);
            }
            catch (Exception ex)
            {
                StringBuilder logMessage = new StringBuilder();
                logMessage.AppendLine("Ops Routine Actor Exception on send Heartbeat: " + ex.Message);
            }
        }

        static string GetHeartbeatStatus()
        {
            dynamic HeartbeatMessage = new ExpandoObject();
            HeartbeatMessage.topic = "Process Heartbeat";
            HeartbeatMessage.name = "Ops Routine Host Actor";
            HeartbeatMessage.machine = Environment.MachineName;
            HeartbeatMessage.processId = _ProcessId;
            HeartbeatMessage.status = "Running";            

            HeartbeatMessage.cpu = Math.Round(_cpuCounter.NextValue(), 2) + " %";
            HeartbeatMessage.ramAvail = _ramCounter.NextValue() + " MB";

            HeartbeatMessage.timestampSource = DateTime.UtcNow;
            var jsonString = JsonConvert.SerializeObject(HeartbeatMessage);
            return jsonString;
        }
                
        private void PushDataToPowerBIAPI(string realTimePushURL)
        {
            try
            {
                String currentTime = DateTime.UtcNow.ToString("s") + "Z";
                Random r = new Random();
                int currentWind = r.Next(0, 15);
                int currentPower = currentWind * 100 + r.Next(0, 100);

                // Send POST request to the push URL
                // Uses the WebRequest sample code as documented here: https://msdn.microsoft.com/en-us/library/debx8sh9(v=vs.110).aspx
                WebRequest request = WebRequest.Create(realTimePushURL);
                request.Method = "POST";
                string postData = String.Format("[{{ \"time\": \"{0}\", \"power\":{1}, \"wind\":{2} }}]", currentTime, currentPower, currentWind);
                Console.WriteLine(String.Format("Making POST request with data: {0}", postData));

                // Prepare request for sending
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = byteArray.Length;

                // Get the request stream.
                Stream dataStream = request.GetRequestStream();

                // Write the data to the request stream.
                dataStream.Write(byteArray, 0, byteArray.Length);

                // Close the Stream object.
                dataStream.Close();

                // Get the response.
                WebResponse response = request.GetResponse();

                // Display the status.
                Console.WriteLine(String.Format("Service response: {0}", ((HttpWebResponse)response).StatusCode));

                // Get the stream containing content returned by the server.
                dataStream = response.GetResponseStream();

                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);

                // Read the content.
                string responseFromServer = reader.ReadToEnd();

                // Display the content.
                Console.WriteLine(responseFromServer);

                // Clean up the streams.
                reader.Close();
                dataStream.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }
        
        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        //protected async override Task OnActivateAsync()
        //{
        //    ActorEventSource.Current.ActorMessage(this, "Actor activated.");

        //    // The StateManager is this actor's private state store.
        //    // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
        //    // Any serializable object can be saved in the StateManager.
        //    // For more information, see https://aka.ms/servicefabricactorsstateserialization\ 
        //    await this.StateManager.AddOrUpdateStateAsync("ActorCount", 1, (key, oldValue) => oldValue + 1);
        //    int count = await this.StateManager.GetStateAsync<int>("ActorCount");
        //    ActorEventSource.Current.ActorMessage(this, "Actor Count : " + count.ToString());

        //    return;
        //}

        //protected override Task OnDeactivateAsync()
        //{
        //    ActorEventSource.Current.ActorMessage(this, "Actor activated.");

        //    // The StateManager is this actor's private state store.
        //    // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
        //    // Any serializable object can be saved in the StateManager.
        //    // For more information, see https://aka.ms/servicefabricactorsstateserialization

        //    return this.StateManager.TryAddStateAsync("count", 0);
        //}

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <returns></returns>
        //Task<int> IRoutineTask.GetCountAsync(CancellationToken cancellationToken)
        //{
        //    return this.StateManager.GetStateAsync<int>("count", cancellationToken);
        //}

        /// <summary>
        /// TODO: Replace with your own actor method.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        //Task IRoutineTask.SetCountAsync(int count, CancellationToken cancellationToken)
        //{
        //    // Requests are not guaranteed to be processed in order nor at most once.
        //    // The update function here verifies that the incoming count is greater than the current count to preserve order.
        //    return this.StateManager.AddOrUpdateStateAsync("count", count, (key, value) => count > value ? count : value, cancellationToken);
        //}
    }
}
