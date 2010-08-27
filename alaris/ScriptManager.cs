using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Alaris.API;
using Alaris.Config;
using Alaris.Core;
using Alaris.Exceptions;
using Alaris.Irc;
using Alaris.Irc.Delegates.Channel;
using Alaris.Irc.Delegates.Disconnect;
using Alaris.Irc.Delegates.Messages;
using Alaris.Irc.Delegates.Server;
using System.CodeDom.Compiler;

namespace Alaris
{
    /// <summary>
    ///   A script manager for the IRC connections.
    ///   Loads plugins, manages events etc.
    /// </summary>
    public sealed class ScriptManager : IThreadContext
    {
        private readonly List<string> _channels = new List<string>();
        private readonly Guid _guid;
        private readonly string _scriptsPath;
        private readonly List<IAlarisBasic> _plugins = new List<IAlarisBasic>();
        private readonly Dictionary<string,string> _pluginInfo = new Dictionary<string, string>();
      

        /// <summary>
        ///   The IRC connection instance.
        /// </summary>
        public Connection Connection
        {
            get { return _connection; }
        }

        /// <summary>
        /// Information about loaded plugins.
        /// </summary>
        public Dictionary<string, string> PluginInfos { get { return _pluginInfo; } }

        /// <summary>
        /// List of loaded plugins.
        /// </summary>
        public IEnumerable<IAlarisBasic> Plugins { get { return _plugins; } }

        /// <summary>
        /// Gets or sets the directory where the scripts reside.
        /// </summary>
        public string ScriptDirectory
        {
            get; set;
        }

        private Connection _connection;


        /// <summary>
        ///   Creates a new instance of ScriptManager
        /// </summary>
        /// <param name = "con">
        ///   The IRC connection. See <see cref = "Alaris.Irc.Connection" />
        /// </param>
        /// ///
        /// <param name = "chans">
        ///   Channels the bot is on.
        /// </param>
        /// <param name="scriptPath">Path to scripts.</param>
        public ScriptManager(ref Connection con, List<string> chans, string scriptPath)
        {
            _connection = con;
            _channels = chans;
            _guid = Guid.NewGuid();
            _scriptsPath = scriptPath;
        }

        /// <summary>
        ///   Gets the GUID.
        /// </summary>
        /// <returns>
        ///   The GUID.
        /// </returns>
        public Guid GetGuid()
        {
            return _guid;
        }

        /// <summary>
        ///   Run this instance.
        /// </summary>
        public void Run()
        {
            LoadScripts();


        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "ScriptManager" /> is reclaimed by garbage collection.
        /// </summary>
        ~ScriptManager()
        {
            Log.Debug("ScriptManager", "~ScriptManager()");
        }

        /// <summary>
        ///   Registers the given function to be run when a public message occurs.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPublicHook(PublicMessageEventHandler handler)
        {
            Connection.Listener.OnPublic += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when somebody leaves a channel.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPartHook(PartEventHandler handler)
        {
            Connection.Listener.OnPart += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when somebody quits the IRC server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnQuitHook(QuitEventHandler handler)
        {
            Connection.Listener.OnQuit += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot receives a private message from someone.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPrivateHook(PrivateMessageEventHandler handler)
        {
            Connection.Listener.OnPrivate += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot gets disconnected from the server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnDisconnectedHook(DisconnectedEventHandler handler)
        {
            Connection.Listener.OnDisconnected += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bots gets registered on the server (usually after MOTD).
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnRegisteredHook(RegisteredEventHandler handler)
        {
            Connection.Listener.OnRegistered += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot receives an error message from the server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnErrorHook(ErrorMessageEventHandler handler)
        {
            Connection.Listener.OnError += handler;
        }

        /// <summary>
        /// Loads all the scripts.
        /// </summary>
        public void LoadScripts()
        {
            var di = new DirectoryInfo(_scriptsPath);

            foreach(var file in di.GetFiles("*.cs"))
            {
                LoadScript(file.FullName);
            }

            foreach(var dir in di.GetDirectories())
            {
                LoadScriptCollection(dir.FullName);
            }

        }

        /// <summary>
        /// Reloads all scripts.
        /// </summary>
        public void ReloadScripts()
        {
            foreach(var plugin in _plugins)
                plugin.OnUnload();

            _plugins.Clear();
            _pluginInfo.Clear();

            LoadScripts();
        }

        /// <summary>
        /// Loads the specified script.
        /// </summary>
        /// <param name="path">Path to the script.</param>
        /// <exception cref="ScriptTypeException"></exception>
        /// <exception cref="ScriptCompileException"></exception>
        public void LoadScript(string path)
        {
            var scpath = Path.Combine(_scriptsPath, path);

            var file = new FileInfo(scpath);
            var plname = Path.GetFileNameWithoutExtension(file.FullName);

            Log.Notice("ScriptManager", string.Format("Loading script: {0}", plname));

            var provider = CodeDomProvider.CreateProvider("CSharp");

            var parameters = new CompilerParameters
                                 {
                                     GenerateInMemory = true,
                                     IncludeDebugInformation = false,
                                     TreatWarningsAsErrors = true
                                 };

            SetupCompilerReferences(ref parameters);

            var result = provider.CompileAssemblyFromFile(parameters, file.FullName);

            CheckErrors(scpath, result);



            var type = result.CompiledAssembly.GetType(string.Format("Alaris.{0}.AlarisPlugin", plname));
            var plugin = (result.CompiledAssembly.CreateInstance(type.FullName, true)) as IAlarisBasic;

            if(plugin == null)
                throw new ScriptTypeException(string.Format("The scripts AlarisPlugin type does not implement the IAlarisBasic interface. Script name: {0}", file.Name));

            plugin.Initialize(ref _connection, _channels);

            if(!_plugins.Contains(plugin))
                _plugins.Add(plugin);

            if(!_pluginInfo.ContainsKey(plugin.Name))
                _pluginInfo.Add(plugin.Name, string.Format("Author: {0}", plugin.Author));
           

            provider.Dispose();

            Log.Success("ScriptManager", string.Format("Loaded {0}", plname));
        }

        private static void SetupCompilerReferences(ref CompilerParameters parameters)
        {
            parameters.ReferencedAssemblies.Add("Alaris.exe");
            parameters.ReferencedAssemblies.Add("Alaris.Irc.dll");
            parameters.ReferencedAssemblies.Add("Alaris.API.dll");
            parameters.ReferencedAssemblies.Add("System.Web.dll");
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("Atom.NET.dll");
        }

        /// <summary>
        /// Loads all scripts in the specified directory together. (Useful for dependencies)
        /// </summary>
        /// <param name="directory">Directory to search in</param>
        /// <exception cref="ScriptCompileException"></exception>
        public void LoadScriptCollection(string directory)
        {

            var scpath = Path.Combine(_scriptsPath, directory);

            if(!File.Exists(Path.Combine(scpath, "plugin.xml")))
                throw new ScriptCollectionInvalidException("The specified script collection does not include a plugin information file with it (plugin.xml)");


            var dir = new DirectoryInfo(scpath);

            Log.Notice("ScriptManager", string.Format("Loading script collection: {0}", dir.Name));

            var scripts = dir.GetFiles("*.cs").Select(file => file.FullName).ToList();

            var plinfo = new XmlSettings(Path.Combine(scpath, "plugin.xml"), "alaris");

            var nmspace = plinfo.GetSetting("plugin/info/namespace", "Alaris.Plugins");
            var author = plinfo.GetSetting("plugin/info/author", "Twl");
            var name = plinfo.GetSetting("plugin/info/name", "AlarisPlugin");

            var provider = CodeDomProvider.CreateProvider("CSharp");

            var parameters = new CompilerParameters
            {
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                TreatWarningsAsErrors = true
            };

            SetupCompilerReferences(ref parameters);

            var result = provider.CompileAssemblyFromFile(parameters, scripts.ToArray());

            CheckErrors(scpath, result);

            var type = result.CompiledAssembly.GetType(string.Format("Alaris.{0}.AlarisPlugin", nmspace));
            var plugin = (result.CompiledAssembly.CreateInstance(type.FullName, true)) as IAlarisBasic;
            
            if (plugin == null)
                throw new ScriptTypeException(string.Format("The scripts AlarisPlugin type does not implement the IAlarisBasic interface or the specified namespace was invalid. Script collection: {0}", scpath));

            plugin.Initialize(ref _connection, _channels);

            if (!_plugins.Contains(plugin))
                _plugins.Add(plugin);

            if (!_pluginInfo.ContainsKey(name))
                _pluginInfo.Add(name, string.Format("Author: {0}", author));

            provider.Dispose();
            
            Log.Success("ScriptManager", string.Format("Loaded collection: {0}", scpath));
        }

        private static void CheckErrors(string scpath, CompilerResults result)
        {
            if (result.Errors.Count != 0)
            {
                var sb = new StringBuilder();
                var errors = 0;
                foreach (var error in result.Errors.Cast<CompilerError>().Where(error => !error.IsWarning))
                {
                    sb.AppendFormat("Error {0}, ({1}: {2}): {3}{4}", error.ErrorNumber, error.FileName, error.Line,
                                    error.ErrorText, Environment.NewLine);

                    ++errors;
                }

                if(errors > 0)
                    throw new ScriptCompileException(string.Format("Couldn't compile collection: {0}, errors: {1}", scpath,
                                                                   sb));

                sb.Clear();
            }
        }


        /// <summary>
        ///   Runs the public handlers found inside the loaded plugins.
        /// </summary>
        /// <param name = "info">
        ///   Userinfo passed to every handler.
        /// </param>
        /// <param name = "chan">
        ///   Channel passed to every handler.
        /// </param>
        /// <param name = "msg">
        ///   Message passed to every handler.
        /// </param>
        public void RunPublicHandlers(UserInfo info, string chan, string msg)
        {
            foreach (var ob in _plugins)
            {
                try
                {
                    ob.OnPublicMessage(info, chan, msg);
                }
                catch (Exception x)
                {
                    Log.Error("ScriptManager", "Couldn't call public message handler in a plugin.");
                    Log.Debug("ScriptManager", x.ToString());
                    return;
                }
            }
        }

        /// <summary>
        ///   Runs the registered handlers in the loaded plugins.
        /// </summary>
        public void RunRegisteredHandlers()
        {
            foreach (var ob in _plugins)
            {
                try
                {
                    ob.OnRegistered();
                }
                catch (Exception)
                {
                    Log.Error("ScriptManager", "Couldn't call registered handler in a plugin.");
                    return;
                }
            }
        }

        /// <summary>
        ///   Gets the list of loaded plugins.
        /// </summary>
        /// <returns>
        ///   A list of plugins.
        /// </returns>
        public IEnumerable<IAlarisBasic> GetPlugins()
        {
            return _plugins;
        }
    }
}