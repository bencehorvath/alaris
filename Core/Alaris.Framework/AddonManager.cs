using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using Alaris.Framework.Extensions;
using NLog;


namespace Alaris.Framework
{
    /// <summary>
    /// Class used to manage (load, unload, reload) plugins dynamically.
    /// </summary>
    public class AddonManager
    {
        /// <summary>
        /// IRC channel list.
        /// </summary>
        public List<string> Channels { get; private set; }

        private readonly List<IAlarisAddon> _addons = new List<IAlarisAddon>();

        /// <summary>
        /// List of found assemblies.
        /// </summary>
        public readonly List<Assembly> Assemblies = new List<Assembly>();

        private readonly object _loadLock = new object();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes the Plugin manager.
        /// </summary>
        public static void Initialize() 
        {
            SetupAppDomainDebugHandlers();
        }

        /// <summary>
        /// Loads plugins from the specified directory.
        /// </summary>
        /// <param name="directory">The directory to check in</param>
        public void LoadPluginsFromDirectory(DirectoryInfo directory) { LoadPluginsFromDirectory(directory.FullName);}


        /// <summary>
        /// Loads plugins from the specified directory.
        /// </summary>
        /// <param name="directory">The directory to check in</param>
        public void LoadPluginsFromDirectory(string directory)
        {
            Contract.Requires(!string.IsNullOrEmpty(directory));


            var dir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, directory));
            Log.Info("Loading addons from: {0}", dir.FullName);

            foreach(var dll in dir.GetFiles("*.dll").AsParallel())
            {
                var asm = Assembly.LoadFrom(dll.FullName);
                
                if(asm == null || Assemblies.Contains(asm))
                    continue;

                foreach (var pl in asm.GetTypesWithInterface(typeof(IAlarisAddon)).Select(type => (IAlarisAddon)Activator.CreateInstance(type)))
                {
                    pl.Setup(AlarisBase.Instance);

                    lock (_loadLock)
                    {
                        _addons.Add(pl);
                        Assemblies.Add(asm);
                    }

                    Log.Info("Loaded plugin {0} {1} by {2} ({3})", pl.Name, asm.GetName().Version.ToString(), pl.Author, pl.Website);
                }
            }
        }

        /// <summary>
        /// Unloads all addons.
        /// </summary>
        public void UnloadPlugins()
        {
            lock (_loadLock)
            {
                foreach(var addon in _addons)
                {
                    addon.Destroy();
                }

                _addons.Clear();

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
