using System;
using System.IO;
using Alaris.Irc.Delegates.Channel;
using Alaris.Irc.Delegates.Disconnect;
using Alaris.Irc.Delegates.Messages;
using Alaris.Irc.Delegates.Server;
using NLog;

namespace Alaris.Framework
{
    /// <summary>
    ///   A script manager for the IRC connections.
    ///   Loads plugins, manages events etc.
    /// </summary>
    [Serializable]
    public sealed class ScriptManager
    {
        private readonly Guid _guid;
        private readonly string _scriptsPath;
        [NonSerialized]
        private Lua.LuaEngine _luaEngine;

        [NonSerialized]
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets the Lua engine.
        /// </summary>
        public Lua.LuaEngine Lua { get { return _luaEngine; } }

        /// <summary>
        /// Gets or sets the directory where the scripts reside.
        /// </summary>
        public string ScriptDirectory
        {
            get; set;
        }

        /// <summary>
        ///   Creates a new instance of ScriptManager
        /// </summary>
        /// <param name="scriptPath">Path to scripts.</param>
        public ScriptManager(string scriptPath)
        {
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
            if(AlarisBase.Instance.LuaEnabled)
                _luaEngine = new Lua.LuaEngine(Path.Combine(_scriptsPath, "lua"));
            Log.Info("Lua support is disabled.");
        }

        /// <summary>
        ///   Releases unmanaged resources and performs other cleanup operations before the
        ///   <see cref = "ScriptManager" /> is reclaimed by garbage collection.
        /// </summary>
        ~ScriptManager()
        {
            Log.Trace("~ScriptManager()");
        }

        /// <summary>
        ///   Registers the given function to be run when a public message occurs.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPublicHook(PublicMessageEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnPublic += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when somebody leaves a channel.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPartHook(PartEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnPart += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when somebody quits the IRC server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnQuitHook(QuitEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnQuit += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot receives a private message from someone.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnPrivateHook(PrivateMessageEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnPrivate += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot gets disconnected from the server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnDisconnectedHook(DisconnectedEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnDisconnected += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bots gets registered on the server (usually after MOTD).
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnRegisteredHook(RegisteredEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnRegistered += handler;
        }

        /// <summary>
        ///   Registers the given function to be run when the bot receives an error message from the server.
        /// </summary>
        /// <param name = "handler">
        ///   The function to register.
        /// </param>
        public void RegisterOnErrorHook(ErrorMessageEventHandler handler)
        {
            AlarisBase.Instance.Connection.Listener.OnError += handler;
        }

    }
}