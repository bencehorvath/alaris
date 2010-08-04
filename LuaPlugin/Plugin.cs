using System;
using Alaris.Core;
using Alaris.Irc;
using System.Collections.Generic;
using LuaInterface;

namespace Alaris.LuaPlugin
{
	public class AlarisPlugin : IAlarisBasic
	{
		private Connection _connection;
		private Lua _lua = new Lua();
		
		public AlarisPlugin()
		{
		}
		
		public void Initialize(ref Connection con) { _connection = con; }
		
		public void Initialize(ref Connection con, ref List<string> a) {}
		
		public void OnPublicMessage(UserInfo user, string chan, string msg)
		{
		}
		
		public void OnRegistered()
		{
		}
		
		public void OnLoad() {}
		public void OnUnload() {}
		
		
		public string GetName() { return "LuaPlugin"; }
	}
}
