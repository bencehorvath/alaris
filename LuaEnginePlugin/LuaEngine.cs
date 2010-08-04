using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Mono.Math;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Core;
using Alaris.Extras;
using Timer=System.Timers.Timer;

//using LuaInterface;
using Alaris.Script;

namespace Alaris.LuaEnginePlugin
{
	public class AlarisPlugin : IAlarisBasic
	{
		private Connection _connection;
		private List<string> _channels;
		private readonly Lua _lua = new Lua();
		
		public AlarisPlugin ()
		{
			
		}
	
		public void Initialize (ref Connection con)
		{
			
		}

		public void Initialize (ref Connection con, ref List<string> channels)
		{
			_connection = con;
			_channels = channels;
		}

		public void OnPublicMessage (UserInfo user, string channel, string msg)
		{
			if(msg.StartsWith("@lua "))
			{
				var cmd = msg.Remove(0, 5);
				_lua.DoString(cmd);
			}
		}

		public void OnRegistered ()
		{
			
		}

		public void OnLoad ()
		{
			var sBot = Singleton<AlarisBot>.Instance;
			
			sBot.Pool.Enqueue(() =>
			                  {
				
				_lua.RegisterFunction("sendmsg", sBot, sBot.GetType().GetMethod("SendMsg"));
				
			});
		}

		public void OnUnload ()
		{
			
		}

		public string GetName ()
		{
			return "LuaEnginePlugin";
		}
	}

}

