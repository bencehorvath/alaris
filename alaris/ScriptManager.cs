using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using Alaris.Core;
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
    public class ScriptManager : IThreadContext
    {
        private List<string> _channels = new List<string>();
        private readonly Guid _guid;

        /// <summary>
        ///   The IRC connection instance.
        /// </summary>
        public Connection Connection
        {
            get { return _connection; }
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
        public ScriptManager(ref Connection con, ref List<string> chans)
        {
            _connection = con;
            _channels = chans;
            _guid = Guid.NewGuid();
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
            LoadPlugins();
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