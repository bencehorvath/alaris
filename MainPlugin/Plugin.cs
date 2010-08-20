using System;
using Alaris.Core;
using Alaris.Irc;
using System.Collections.Generic;
using Alaris.Extras;

namespace Alaris.MainPlugin
{
    public class AlarisPlugin : MarshalByRefObject, IAlarisBasic
	{
		private Connection _connection;
		private readonly Stack<int> _allocer = new Stack<int>();
		
		
		public AlarisPlugin()
		{

		}
		
		~AlarisPlugin()
		{
			Log.Debug("Main", "~AlarisPlugin()");
		}
		
		public void Initialize(ref Connection con)
		{
			_connection = con;
		}
		
		public void Initialize(ref Connection con, ref List<string> a) {}
		
		public void OnPublicMessage(UserInfo user, string chan, string msg)
		{
			if(msg == "plgreet")
			{
				_connection.Sender.PublicMessage(chan, "Udv, " + user.Nick);
				return;
			}
			
			if(msg.StartsWith("@say "))
			{
				var send = msg.Remove(0, 5);
				_connection.Sender.PublicMessage(chan, send);
				return;
			}
			
			if(msg == "@collect" && Utilities.IsAdmin(user))
			{
				_connection.Sender.PublicMessage(chan, "Running the garbage collector...");
				GC.Collect();
				GC.WaitForPendingFinalizers();
				_connection.Sender.PublicMessage(chan, "Garbage collector finished.");
				
				return;
			}
			
			if(msg.StartsWith("@alloc ") && Utilities.IsAdmin(user))
			{
				int amount = 0;
				try { amount = Convert.ToInt32(msg.Replace("@alloc ", string.Empty)); } catch (Exception) { _connection.Sender.PublicMessage(chan, "Amount is invalid.");  return; }
				amount /= sizeof(int);
				
				for(int i=0; i < amount; ++i)
				{
					_allocer.Push(i);
				}
				
				amount *= sizeof(int);
				
				_connection.Sender.PublicMessage(chan, "Allocated: " + amount/1024/1024 + " MB memory");
				
				return;
			}
			
			if(msg == "@dealloc all" && Utilities.IsAdmin(user))
			{
				int count = _allocer.Count;
				int size = (count*sizeof(int))/1024/1024;
				
				for(int i=0; i < _allocer.Count;++i)
				{
					_allocer.Pop();
				}
				
				_allocer.Clear();
				
				_connection.Sender.PublicMessage(chan, "Deallocated: " + size.ToString() + " MB memory (probably not yet collected)");
				var nowmemory = GC.GetTotalMemory(true)/1024/1024;
				_connection.Sender.PublicMessage(chan, "Currently " + nowmemory + " MB of memory is thought to be allocated.");
				return;
			}
			
			if(msg.StartsWith("@md5 "))
			{
				string tx = msg.Remove(0, 5);
				Log.Notice("MD5", "Text is: " + tx);
				//_connection.Sender.PublicMessage(chan, "Text: " + tx);
				_connection.Sender.PublicMessage(chan, Utilities.MD5String(tx));
				return;
			}
			
			if(msg.StartsWith("@title "))
			{
				try 
				{
					Utilities.HandleWebTitle(ref _connection, chan, msg);
					
				}
				catch (Exception)
				{
					Log.Error("WebHelper", "Invalid webpage address.");
					_connection.Sender.PublicMessage(chan, IrcConstants.Red + "Invalid address.");
					return;
				}
				return;
			}
		}
		
		public void OnRegistered()
		{
			
		}
		
		public void OnUnload() { _connection = null; }
		
		public void OnLoad() { }
		
		public string GetName()
		{
			return "MainPlugin";
		}
	}
}

