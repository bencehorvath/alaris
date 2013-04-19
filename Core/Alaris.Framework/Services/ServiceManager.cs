using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Alaris.Framework.Services.Remote;
using NLog;

namespace Alaris.Framework.Services
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
                new Uri(string.Format("http://127.0.0.1:{0}/{1}/Service", AlarisBase.Instance.Config.Config.Remote.Port,
                                      AlarisBase.Instance.Config.Config.Remote.Name));

            _remoteHost = new ServiceHost(typeof(Remoter), uri);

            var smdb = new ServiceMetadataBehavior {HttpGetEnabled = true};

            _remoteHost.Description.Behaviors.Add(smdb);

            try
            {
                _remoteHost.Open();
            }
            catch (Exception exception)
            {
                Log.Error("Exception thrown while opening the service at {0}. This is likely due to URL reservation restriction. " +
                          "You will have to use netsh to reserve the appropriate service URL {0} (modifiable through the configuration file" +
                          "for your user. The service will not function. (Exception name: {1})", uri.ToString(), exception.GetType().FullName);

            }

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
