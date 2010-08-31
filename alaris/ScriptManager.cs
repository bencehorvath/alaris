using System;
using System.Collections.Generic;
using System.IO;
using Alaris.API;
using Alaris.Irc;
using Alaris.Irc.Delegates.Channel;
using Alaris.Irc.Delegates.Disconnect;
using Alaris.Irc.Delegates.Messages;
using Alaris.Irc.Delegates.Server;


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
        private LuaEngine.LuaEngine _luaEngine;

        /// <summary>
        /// Gets the Lua engine.
        /// </summary>
        public LuaEngine.LuaEngine Lua { get { return _luaEngine; } }

        /// <summary>
        ///   The IRC connection instance.
        /// </summary>
        public Connection Connection
        {
            get { return _connection; }
        }

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
            _luaEngine = new LuaEngine.LuaEngine(ref _connection, Path.Combine(_scriptsPath, "lua"));
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

    }
}