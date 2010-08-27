using System;
using Alaris.API;
using Alaris.Core;
using Alaris.Irc;
using System.Collections.Generic;

namespace Alaris.ChannelRights
{
	public class AlarisPlugin : MarshalByRefObject, IAlarisBasic
	{
		private Connection _connection;
		
		public AlarisPlugin()
		{
			
		}
		
		~AlarisPlugin()
		{
			Log.Debug("ChannelRights", "~AlarisPlugin()");
		}
			
		public void Initialize(ref Connection con, List<string> a)
		{
            _connection = con;
		}
		
		public void OnPublicMessage(UserInfo user, string chan, string msg)
		{
			if(msg == "@ole")
			{
				_connection.Sender.PublicMessage(chan, "ugyvan");
				return;
			}
			// demotes
			
			if(msg == "@dv")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.Voice, user.Nick);
				return;
			}
			
			if(msg == "@do")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.ChannelOperator, user.Nick);
				return;
			}
			
			if(msg == "@dh")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.HalfChannelOperator, user.Nick);
				return;
			}
			
			if(msg == "@db")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.Ban, user.Nick);
				return;
			}
			
			
			if(msg.StartsWith("@dv "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.Voice, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@do "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.ChannelOperator, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@dh "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.HalfChannelOperator, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@db "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Remove, ChannelMode.Ban, msg.Remove(0,3));
				return;
			}
			
			//promotes 
			
			
			if(msg == "@v")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.Voice, user.Nick);
				return;
			}
			
			if(msg == "@o")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.ChannelOperator, user.Nick);
				return;
			}
			
			if(msg == "@h")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.HalfChannelOperator, user.Nick);
				return;
			}
			
			if(msg == "@b")
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.Ban, user.Nick);
				return;
			}
			
			
			if(msg.StartsWith("@v "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.Voice, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@o "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.ChannelOperator, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@h "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.HalfChannelOperator, msg.Remove(0,3));
				return;
			}
			
			if(msg.StartsWith("@b "))
			{
				_connection.Sender.ChangeChannelMode(chan, ModeAction.Add, ChannelMode.Ban, msg.Remove(0,3));
				return;
			}
			
			

		}
		
		public void OnRegistered()
		{
			
		}
		
		public void OnUnload() { _connection = null; }

	    public string Name
	    {
            get { return "ChannelRights"; }
	    }

	    public string Author
	    {
            get { return "Twl"; }
	    }

	    public void OnLoad() { }
		
	}
}

