using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Management;
using ICSharpCode.SharpZipLib;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Core;
using Timer=System.Timers.Timer;
using System.Security.Cryptography;

namespace Alaris.Extras
{
	/// <summary>
	/// Class providing useful methods for handling different things.
	/// </summary>
	public static class Utilities
	{
		/// <summary>
		/// The admin user's username.
		/// </summary>
		public static string AdminUser { get; set; }
		/// <summary>
		/// The admin user's nick.
		/// </summary>
		public static string AdminNick { get; set; }
		/// <summary>
		/// The admin user's hostname.
		/// </summary>
		public static string AdminHost { get; set; }
		
		/// <summary>
		/// Sends system stats using the specified connection.
		/// </summary>
		/// <param name="connection">
		/// The IRC connection.
		/// </param>
		/// <param name="chan">
		/// The channel to send to.
		/// </param>
		public static void SendSysStats(ref Connection connection, string chan)
		{
			Log.Notice("Alaris", "System info request.");
			string hostname = Environment.MachineName;
			string username = Environment.UserName;
			
			string os = Environment.OSVersion.ToString();
			long mem = Process.GetCurrentProcess().WorkingSet64/1024/1024;
			
			connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Bot version: " + IrcConstants.Normal + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
			connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Hosted by " + IrcConstants.Normal + username + " on machine " + hostname);
			connection.Sender.PublicMessage(chan, IrcConstants.Bold + "OS version: " + IrcConstants.Normal + os);
			connection.Sender.PublicMessage(chan, IrcConstants.Bold + "CPU: " + IrcConstants.Normal + GetCpuId() + " | " + Environment.ProcessorCount + " cores.");
			
			if(mem < 40)
				connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Memory eaten: " + IrcConstants.Normal + IrcConstants.Green + mem.ToString() + " MB");
			else if(mem > 40 && mem < 70)
				connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Memory eaten: " + IrcConstants.Normal + IrcConstants.Olive + mem.ToString() + " MB");
			else
				connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Memory eaten: " + IrcConstants.Normal + IrcConstants.Red + mem.ToString() + " MB");
		}
		
		/// <summary>
		/// Sends info using the specified connection.
		/// </summary>
		/// <param name="connection">
		/// The IRC connection.
		/// </param>
		/// <param name="chan">
		/// The channel to send to.
		/// </param>
		public static void SendInfo(ref Connection connection, string chan)
		{
			Log.Notice("Alaris", "Info request.");
			connection.Sender.PublicMessage(chan, IrcConstants.Cyan + "Alaris " + GetBotVersion());
			connection.Sender.PublicMessage(chan, IrcConstants.DarkGreen + "Developer: Twl");
		}
		
		/// <summary>
		/// Determines whether the specified user is admin or not.
		/// </summary>
		/// <param name="user">
		/// The user to check.
		/// </param>
		/// <returns>
		/// True if admin, otherwise false.
		/// </returns>
		public static bool IsAdmin(UserInfo user)
		{
			return (user.Hostname == AdminHost && user.Nick == AdminNick && user.User == AdminUser);
		}
		
		/// <summary>
		/// Calculates the MD5 sum of a file.
		/// </summary>
		/// <param name="fileName">
		/// The file to check.
		/// </param>
		/// <returns>
		/// The MD5 hash.
		/// </returns>
		public static string MD5File(string fileName)
		{
			FileStream file = new FileStream(fileName, FileMode.Open);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] retVal = md5.ComputeHash(file);
			file.Close();
			file.Dispose();
			
			var sb = new StringBuilder();
			
			for (int i = 0; i < retVal.Length; i++)
			{
				sb.Append(retVal[i].ToString("x2"));
				
			}
			
			return sb.ToString();
		}
		
		/// <summary>
		/// Calculates the MD5 hash of a string.
		/// </summary>
		/// <param name="Value">
		/// The string to calculate MD5 hash of.
		/// </param>
		/// <returns>
		/// The MD5 hash.
		/// </returns>
		public static string MD5String(string Value)
		{
			 var x = new MD5CryptoServiceProvider();
			
			 byte[] data = Encoding.ASCII.GetBytes(Value);
			 data = x.ComputeHash(data);
			 string ret = "";
			
			 for (int i=0; i < data.Length; i++)
			 	ret += data[i].ToString("x2").ToLower();
			
			 return ret;
		}
		
		/// <summary>
		/// Gets the unix time.
		/// </summary>
		/// <returns>
		/// The unix time.
		/// </returns>
		public static double GetUnixTime()
		{
			var elapsed = (DateTime.UtcNow - new DateTime(1970,1,1,0,0,0));
			
			return (elapsed.TotalSeconds);
		}
		
		/// <summary>
		/// Returns the bot's assembly version.
		/// </summary>
		/// <returns>
		/// The version.
		/// </returns>
		public static string GetBotVersion()
		{
			return (System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
		}
		
		/// <summary>
		/// Gets the URLs in the specified text.
		/// </summary>
		/// <param name="text">
		/// The text to search in.
		/// </param>
		/// <returns>
		/// The list of urls.
		/// </returns>
		public static List<string> GetUrls(string text)
		{
			var urls = new List<string>();
			
			try 
			{
				//var urlFind = new Regex(@"(?<page>http://(www\.)?\S+\.\S{2,6}(/\S*\s*)?)", RegexOptions.IgnoreCase);
				var urlFind = new Regex(@"(?<page>http://(www\.)?\S+\.\S{2,6}(/*\S*))", RegexOptions.IgnoreCase);
				
				if(urlFind.IsMatch(text))
				{
					MatchCollection matches = urlFind.Matches(text);
					
					foreach(Match match in matches)
					{
						urls.Add(match.Groups["page"].ToString());
					}
				}
			}
			catch(Exception x)
			{
				Log.Error("TitleFounder", x.Message);
			}
			
			return urls;
		}
		
		/// <summary>
		/// Handles the web title command.
		/// </summary>
		/// <param name="connection">
		/// The IRC connection used.
		/// </param>
		/// <param name="chan">
		/// The channel to send title to.
		/// </param>
		/// <param name="msg">
		/// The message containing the url.
		/// </param>
		public static void HandleWebTitle(ref Connection connection, string chan, string msg)
		{
			string tt = msg.Replace("@title ", string.Empty);
			if(!tt.StartsWith("http://"))
			{
				tt = "http://" + tt;
			}
			var url = new Uri(tt);
			string title = WebHelper.GetWebTitle(url);
			title = title.Replace(Environment.NewLine, " ").Replace("  ", " ").Replace("    ", " "); // the spaces are for youtube's shit
			
			// check if it's youtube.
			var youtubeRegex = new Regex(@"\s*YouTube\s*\-(?<song>.+)", RegexOptions.IgnoreCase);
			
			if(youtubeRegex.IsMatch(title))
			{
				var match = youtubeRegex.Match(title);
				string song = match.Groups["song"].ToString();
				
				connection.Sender.PublicMessage(chan, IrcConstants.Purple + "[YouTube]: " + IrcConstants.DarkGreen + song.Substring(1)); // about substr: remove the space before song name
				return;
			}
			
			connection.Sender.PublicMessage(chan, IrcConstants.Bold + "[Title]: " + IrcConstants.Normal + IrcConstants.DarkGreen + title);
		}
		
		/// <summary>
		/// Gets the cpu brand string.
		/// </summary>
		/// <returns>
		/// The CPU brand string.
		/// </returns>
		public static string GetCpuId()
        {
            var reader = new StreamReader("/proc/cpuinfo");
			
			string content = reader.ReadToEnd();
			reader.Close();
			reader.Dispose();
			
			var getBrandRegex = new Regex(@"model\sname\s:\s*(?<first>.+\sCPU)\s*(?<second>.+)", RegexOptions.IgnoreCase);
			
			if(!getBrandRegex.IsMatch(content))
			{
				// not intel
				var amdRegex = new Regex(@"model\sname\s:\s*(?<cpu>.+)");
				
				if(!amdRegex.IsMatch(content))
					return "Not found";
				
				var amatch = amdRegex.Match(content);
				string amd = amatch.Groups["cpu"].ToString();
				
				return amd;
			}
			
			var match = getBrandRegex.Match(content);
			string cpu = (match.Groups["first"].ToString() + " " + match.Groups["second"].ToString());
			
			//return cpu.Substring(1);
			return cpu;
			
        }
	}
	
}
