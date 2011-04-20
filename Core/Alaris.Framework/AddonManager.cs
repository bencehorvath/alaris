using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Alaris.API;
using Alaris.Framework.Extensions;
using Alaris.Irc;
using NLog;

namespace Alaris.Framework
{
    /// <summary>
    /// Class used to manage (load, unload, reload) plugins dynamically.
    /// </summary>
    public static class AddonManager
    {
        /// <summary>
        /// The IRC connection.
        /// </summary>
        public static Connection Connection { get; private set; }
        /// <summary>
        /// IRC channel list.
        /// </summary>
        public static List<string> Channels { get; private set; }

        private static readonly List<IAlarisAddon> Addons = new List<IAlarisAddon>();

        /// <summary>
        /// List of found assemblies.
        /// </summary>
        public static readonly List<Assembly> Assemblies = new List<Assembly>();

        private static readonly object LoadLock = new object();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the Plugin manager.
        /// </summary>
        /// <param name="con">IRC connection</param>
        /// <param name="channels">IRC channel list</param>
        public static void Initialize(ref Connection con, List<string> channels) 
        {
            Connection = con;
            Channels = channels;
            SetupAppDomainDebugHandlers();
        }

        /// <summary>
        /// Loads plugins from the specified directory.
        /// </summary>
        /// <param name="directory">The directory to check in</param>
        public static void LoadPluginsFromDirectory(DirectoryInfo directory) { LoadPluginsFromDirectory(directory.FullName);}


        /// <summary>
        /// Loads plugins from the specified directory.
        /// </summary>
        /// <param name="directory">The directory to check in</param>
        public static void LoadPluginsFromDirectory(string directory)
        {
            var dir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, directory));
            Log.Info("Loading addons from: {0}", dir.FullName);

            foreach(var dll in dir.GetFiles("*.dll").AsParallel())
            {
                var asm = Assembly.LoadFrom(dll.FullName);
                
                if(asm == null || Assemblies.Contains(asm))
                    continue;

                IAlarisAddon pl = null;

                foreach (var type in 
                    from t in asm.GetTypes().AsParallel() 
                    where t.GetInterfaces().Contains(typeof(IAlarisAddon)) 
                    select t)
                {
                    pl = Activator.CreateInstance(type).Cast<IAlarisAddon>();

                    if (pl == null)
                        continue;

                    var connection = Connection;

                    pl.Setup(ref connection, Channels);

                    lock (LoadLock)
                    {
                        Addons.Add(pl);
                        Assemblies.Add(asm);
                    }

                    Log.Info("Loaded plugin {0} {1} by {2} ({3})", pl.Name, asm.GetName().Version.ToString(), pl.Author, pl.Website);
                }
            }
        }

        /// <summary>
        /// Unloads all addons.
        /// </summary>
        public static void UnloadPlugins()
        {
            lock (LoadLock)
            {
                for (var i = 0; i < Addons.Count; ++i )
                {
                    var pl = Addons[i];
                    pl.Destroy();
                    Addons.Remove(pl);
                }

                Assemblies.Clear();  
            }
        }

        private static void SetupAppDomainDebugHandlers()
        {
            AppDomain.CurrentDomain.DomainUnload +=
                (sender, args) => Log.Trace("PluginManager: AppDomain::DomainUnload, hash: {0}",
                                            AppDomain.CurrentDomain.GetHashCode());

            AppDomain.CurrentDomain.AssemblyLoad +=
                (sender, ea) =>
                Log.Trace("PluginManager: AppDomain::AssemblyLoad, sender is: {0}, loaded assembly: {1}."
                                        , sender.GetHashCode()
                                        , ea.LoadedAssembly.FullName);

           

            AppDomain.CurrentDomain.AssemblyResolve +=
                (sender, eargs) =>
                    {
                        Log.Trace("PluginManager: AppDomain::AssemblyResolve, sender: {0}, name: {1}, asm: {2}", 
                            sender.GetHashCode(), eargs.Name, eargs.RequestingAssembly.FullName );

                        return null;
                    };

        }
    }
}
