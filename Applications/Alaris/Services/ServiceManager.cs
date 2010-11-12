using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Alaris.Services.Remote;
using NLog;

namespace Alaris.Services
{
    /// <summary>
    /// Class used to manage every service in Alaris.
    /// </summary>
    public static class ServiceManager
    {
        private static ServiceHost _remoteHost;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Starts the services.
        /// </summary>
        public static void StartServices()
        {
            // start remote
            Log.Info("Starting remoting service...");
            var uri =
                new Uri(string.Format("http://localhost:{0}/{1}/Service", AlarisBot.Instance.Config.Config.Remote.Port,
                                      AlarisBot.Instance.Config.Config.Remote.Name));

            _remoteHost = new ServiceHost(typeof(Remoter), uri);

            var smdb = new ServiceMetadataBehavior {HttpGetEnabled = true};

            _remoteHost.Description.Behaviors.Add(smdb);

            _remoteHost.Open();

            Log.Info("Remoting service is up and running (at {0}).", uri.ToString());
        }

        /// <summary>
        /// Stops the services.
        /// </summary>
        public static void StopServices()
        {
            _remoteHost.Close();
            Log.Info("Remote service stopped.");
        }
    }
}
