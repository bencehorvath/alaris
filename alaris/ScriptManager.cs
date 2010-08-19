using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using Alaris.Irc;
using Alaris.Core;
using Alaris.Irc.Delegates.Channel;
using Alaris.Irc.Delegates.Disconnect;
using Alaris.Irc.Delegates.Messages;
using Alaris.Irc.Delegates.Server;

namespace Alaris
{
	/// <summary>
	/// A script manager for the IRC connections.
	/// Loads plugins, manages events etc.
	/// </summary>
	public class ScriptManager : IThreadContext
	{
		private readonly List<IAlarisBasic> _plugins = new List<IAlarisBasic>();
		private List<string> _channels = new List<string>();
		private readonly Guid _guid;
		/// <summary>
		/// The IRC connection instance.
		/// </summary>
		public Connection Connection { get { return _connection; }
		}
		
		private Connection _connection;
		
		/// <summary>
		/// Creates a new instance of ScriptManager
		/// </summary>
		/// <param name="con">
		/// The IRC connection. See <see cref="Alaris.Irc.Connection"/>
		/// </param>
		/// /// <param name="chans">
		/// Channels the bot is on.
		/// </param>
		public ScriptManager(ref Connection con, ref List<string> chans)
		{
			_connection = con;
			_channels = chans;
			_guid = Guid.NewGuid();
		}
		
		/// <summary>
		/// Gets the GUID.
		/// </summary>
		/// <returns>
		/// The GUID.
		/// </returns>
		public Guid GetGuid() { return _guid; }
		
		/// <summary>
		/// Run this instance.
		/// </summary>
		public void Run() { LoadPlugins(); }
		
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="ScriptManager"/> is reclaimed by garbage collection.
		/// </summary>
		~ScriptManager()
		{
			Log.Debug("ScriptManager", "~ScriptManager()");
		}
		
		/// <summary>
		/// Registers the given function to be run when a public message occurs.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnPublicHook(PublicMessageEventHandler handler)
		{
			Connection.Listener.OnPublic += handler;
		}
		
		/// <summary>
		/// Registers the given function to be run when somebody leaves a channel.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnPartHook(PartEventHandler handler)
		{
			Connection.Listener.OnPart += handler;
		}
		
		/// <summary>
		/// Registers the given function to be run when somebody quits the IRC server.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnQuitHook(QuitEventHandler handler)
		{
			Connection.Listener.OnQuit += handler;
		}
		
		/// <summary>
		/// Registers the given function to be run when the bot receives a private message from someone.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnPrivateHook(PrivateMessageEventHandler handler)
		{
			Connection.Listener.OnPrivate += handler;
		}
		
		/// <summary>
		/// Registers the given function to be run when the bot gets disconnected from the server.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnDisconnectedHook(DisconnectedEventHandler handler)
		{
			Connection.Listener.OnDisconnected += handler;
		}
		
		/// <summary>
		/// Registers the given function to be run when the bots gets registered on the server (usually after MOTD).
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnRegisteredHook(RegisteredEventHandler handler)
		{
			Connection.Listener.OnRegistered += handler;	
		}
		
		/// <summary>
		/// Registers the given function to be run when the bot receives an error message from the server.
		/// </summary>
		/// <param name="handler">
		/// The function to register.
		/// </param>
		public void RegisterOnErrorHook(ErrorMessageEventHandler handler)
		{
			Connection.Listener.OnError += handler;
		}
		
		/// <summary>
		/// Loads the plugins.
		/// </summary>
		public void LoadPlugins()
		{
			Log.Notice("ScriptManager", "Loading plugins...");
			var info = new DirectoryInfo("./plugins");
			FileInfo[] files = info.GetFiles();
			// List<AppDomain> domains = new List<AppDomain>();
			
			foreach(FileInfo f in files)
			{
				//Console.WriteLine(f.Name + " with ext: " + f.Extension);
				if(f.Extension != ".dll" || !f.Name.Contains("Plugin"))
					continue;
				
				//Log.Notice("ScriptManager", "Loading plugin: " + f.Name);
				
				try 
				{
					
					var asm = Assembly.LoadFrom("./plugins/" + f.Name);
					
					var plugin = asm.CreateInstance("Alaris." + f.Name.Replace(".dll", string.Empty) + ".AlarisPlugin");
					var usable = plugin as IAlarisBasic;
					
					if(_plugins.Contains(usable))
						continue;

				    if (usable != null)
				    {
				        usable.Initialize(ref _connection);
				        usable.Initialize(ref _connection, ref _channels);
				        _plugins.Add(usable);
				    }
				}
				catch(Exception x)
				{
					Log.Error("ScriptManager", "Failed to load: " + f.Name + " (" + x.Message + ")");
					return;
				}
				
				Log.Success("ScriptManager", "Loaded plugin: " + f.Name + " (hash: " + Utilities.MD5File("./plugins/" + f.Name) + ")");
				
			}
		}
		
		/// <summary>
		/// Loads the specified plugin.
		/// </summary>
		/// <param name="name">
		/// The name of the plugin to load.
		/// </param>
		public bool LoadPlugin(string name)
		{
			if(!name.EndsWith("Plugin"))
				name += "Plugin";
			
			if(!File.Exists("./plugins/" + name + ".dll"))
				return false;
			
			var fi = new FileInfo("./plugins/" + name + ".dll");
			
			try 
			{
				var asm = Assembly.LoadFrom(fi.FullName);
				
				var plugin = asm.CreateInstance("Alaris." + fi.Name.Replace(".dll", string.Empty) + ".AlarisPlugin");
				var usable = plugin as IAlarisBasic;
				
				if(_plugins.Contains(usable))
					return false;

			    if (usable != null)
			    {
			        usable.Initialize(ref _connection);
			        usable.Initialize(ref _connection, ref _channels);
			        usable.OnLoad();
			        _plugins.Add(usable);
			    }
			}
			catch(Exception)
			{
				Log.Error("ScriptManager", "Failed to load: " + fi.Name);
				return false;
			}
				
			Log.Success("ScriptManager", "Loaded plugin: " + fi.Name + " (hash: " + Utilities.MD5File("./plugins/" + fi.Name) + ")");
			
			return true;
		}
		
		/// <summary>
		/// Unloads the specified plugin.
		/// </summary>
		/// <param name="name">
		/// The name of the plugin to unload.
		/// </param>
		public bool UnloadPlugin(string name)
		{
			bool removed = false;
			foreach(var plugin in _plugins)
			{
				if(plugin.GetName().StartsWith(name))
				{
					plugin.OnUnload();
					_plugins.Remove(plugin);
					Log.Success("ScriptManager", "Successfully unloaded plugin: " + name);
					removed = true;
					break;
				}
			}
			
			if(removed == false)
				Log.Error("ScriptManager", "Couldn't unload plugin: " + name);
			
			return (removed);
		}
		
		/// <summary>
		/// Runs the public handlers found inside the loaded plugins.
		/// </summary>
		/// <param name="info">
		/// Userinfo passed to every handler.
		/// </param>
		/// <param name="chan">
		/// Channel passed to every handler.
		/// </param>
		/// <param name="msg">
		/// Message passed to every handler.
		/// </param>
		public void RunPublicHandlers(UserInfo info, string chan, string msg)
		{
			foreach(IAlarisBasic ob in _plugins)
			{
				try 
				{
					ob.OnPublicMessage(info, chan, msg);
				}
				catch(Exception x)
				{
					Log.Error("ScriptManager", "Couldn't call public message handler in a plugin.");
					Log.Debug("ScriptManager", x.ToString());
					return;
				}
			}
		}
		
		/// <summary>
		/// Runs the registered handlers in the loaded plugins.
		/// </summary>
		public void RunRegisteredHandlers()
		{
			foreach(IAlarisBasic ob in _plugins)
			{
				try 
				{
					ob.OnRegistered();
				}
				catch(Exception)
				{
					Log.Error("ScriptManager", "Couldn't call registered handler in a plugin.");
					return;
				}
			}
		}
		
		/// <summary>
		/// Gets the list of loaded plugins.
		/// </summary>
		/// <returns>
		/// A list of plugins.
		/// </returns>
		public IEnumerable<IAlarisBasic> GetPlugins() { return _plugins; }
		
	}
}