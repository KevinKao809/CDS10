using System;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using RoutineTask.Interfaces;
using Microsoft.ServiceFabric.Actors;
using System.Configuration;

namespace RoutineTask
{
    internal static class Program
    {        
        static public string _superadminHeartbeatURL = ConfigurationManager.AppSettings["sfSuperAdminHeartbeatURL"];
        static public string _sfLogLevel = ConfigurationManager.AppSettings["sfLogLevel"];
        static public string _sfLogStorageName = ConfigurationManager.AppSettings["sfLogStorageName"];
        static public string _sfLogStorageKey = ConfigurationManager.AppSettings["sfLogStorageKey"];
        static public string _sfLogStorageContainerApp = ConfigurationManager.AppSettings["sfLogStorageContainerApp"];

        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // This line registers an Actor Service to host your actor class with the Service Fabric runtime.
                // The contents of your ServiceManifest.xml and ApplicationManifest.xml files
                // are automatically populated when you build this project.
                // For more information, see https://aka.ms/servicefabricactorsplatform

                ActorRuntime.RegisterActorAsync<RoutineTask>(
                   (context, actorType) => new AutoActiveRoutineTaskActorService(context, actorType)).GetAwaiter().GetResult();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ActorEventSource.Current.ActorHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }


    internal class AutoActiveRoutineTaskActorService : ActorService
    {
        public AutoActiveRoutineTaskActorService(StatefulServiceContext context, ActorTypeInformation typeInfo)
        : base(context, typeInfo)
        { }

        protected async override Task RunAsync(CancellationToken cancellationToken)
        {
            await base.RunAsync(cancellationToken);

            var proxy = ActorProxy.Create<IRoutineTask>(ActorId.CreateRandom());
            await proxy.StartHost();
        }
    }
}
