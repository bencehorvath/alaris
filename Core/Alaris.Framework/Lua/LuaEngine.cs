using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;

namespace Alaris.Framework.Lua
{
    /// <summary>
    /// Class used to load Lua script files.
    /// This script engine is an optional one and can be disabled by commenting a single line of code.
    /// </summary>
    public sealed class LuaEngine
    {
        private readonly LuaInterface.Lua _lua;
        private readonly Dictionary<string, LuaFunctionDescriptor> _luaFunctions;
        private readonly string _scriptPath;
        private readonly LuaFunctions _functions;
        private readonly FileSystemWatcher _watcher;

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        #region Properties

        /// <summary>
        /// Gets the Lua virtual machine.
        /// </summary>
        public LuaInterface.Lua LuaVM
        {
            get
            {
                return _lua;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new instance of <c>LuaEngine</c>
        /// </summary>
        /// <param name="scriptsPath">The directory where the Lua scripts are located.</param>
        public LuaEngine(string scriptsPath)
        {
            Log.Info("Initializing Lua engine");
            _lua = new LuaInterface.Lua();
            _luaFunctions = new Dictionary<string, LuaFunctionDescriptor>();
            _scriptPath = scriptsPath;
            _functions = new LuaFunctions(ref _lua);

            LuaHelper.RegisterLuaFunctions(_lua, ref _luaFunctions, _functions);

            LoadScripts();

            _watcher = new FileSystemWatcher(_scriptPath)
                           {
                               NotifyFilter =
                                   NotifyFilters.FileName | NotifyFilters.Attributes | NotifyFilters.LastAccess |
                                   NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size,

                               EnableRaisingEvents = true
                           };

            _watcher.Created += (s, e) => LoadScripts(true);
            _watcher.Changed += (s, e) => LoadScripts(true);
            _watcher.Deleted += (s, e) => LoadScripts(true);
            _watcher.Renamed += (s, e) => LoadScripts(true);
        }

        /// <summary>
        /// Loads the Lua scripts.
        /// </summary>
        /// <param name="reload">Is it a reload or not.</param>
        public void LoadScripts(bool reload = false)
        {
            if(reload)
            {
                foreach(var handler in _functions.RegisteredOnPM.AsParallel())
                {
                    AlarisBase.Instance.Connection.Listener.OnPublic -= handler;
                }
            }

            var di = new DirectoryInfo(_scriptPath);

            foreach(var file in di.GetFiles("*.lua").AsParallel())
            {
                Log.Info("Loading Lua script: {0}", file.Name);
                try {_lua.DoFile(file.FullName);}
                catch(Exception x)
                {
                    Log.ErrorException(string.Format("Exception thrown while loading Lua script {0}", file.Name), x);
                }
                
            }
        }

        /// <summary>
        /// Frees up resources.
        /// </summary>
        public void Free()
        {
            _lua.Dispose();
        }
    }
}
