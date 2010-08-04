using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Alaris.Core
{
	/// <summary>
	/// Logging class used all across Alaris.
	/// </summary>
	public static class Log
	{
		
		
		/// <summary>
		/// Emit a console notice.
		/// </summary>
		/// <param name="module">
		/// The module name where the thing described in the message happened.
		/// </param>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void Notice(string module, string message)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("{0}:{1} N ", DateTime.Now.Hour, DateTime.Now.Minute);
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}: ", module);
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write(message + "\n");
			
		}
		
		/// <summary>
		/// Emit a console debug message.
		/// </summary>
		/// <param name="module">
		/// The module name where the thing described in the message happened.
		/// </param>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void Debug(string module, string message)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("{0}:{1} ", DateTime.Now.Hour, DateTime.Now.Minute);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write("D ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}: ", module);
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.Write(message + "\n");
		}
		
		/// <summary>
		/// Emit a console error.
		/// </summary>
		/// <param name="module">
		/// The module name where the thing described in the message happened.
		/// </param>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void Error(string module, string message)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("{0}:{1} ", DateTime.Now.Hour, DateTime.Now.Minute);
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("E ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}: ", module);
			Console.ForegroundColor = ConsoleColor.Red;;
			Console.Write(message + "\n");
		}
		
		/// <summary>
		/// Emit a console success message.
		/// </summary>
		/// <param name="module">
		/// The module name where the thing described in the message happened.
		/// </param>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void Success(string module, string message)
		{
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.Write("{0}:{1} ", DateTime.Now.Hour, DateTime.Now.Minute);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write("S ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.Write("{0}: ", module);
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Write(message + "\n");
		}
		
		/// <summary>
		/// Emit a large warning message.
		/// </summary>
		/// <param name="message">
		/// The message to display.
		/// </param>
		public static void LargeWarning(string message)
		{
			string[] sp = message.Split('\n');
			List<string> lines = new List<string>(50);
			
			foreach(string s in sp)
				if(!string.IsNullOrEmpty(s))
					lines.Add(s);
			
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine();
			
			Console.WriteLine("**************************************************"); // 51
			
			foreach(string item in lines)
			{
				uint len = (uint)item.Length;
				uint diff = (48-len);
				Console.Write("* {0}", item);
				if(diff>0)
				{
					for(uint u=1;u<diff;++u)
					{
						Console.Write(" ");
					}
					
					Console.Write("*\n");
				}
				
			}
			
			Console.WriteLine("**************************************************");
			//Console.WriteLine();
			
		}

	}
}