using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ICSharpCode.SharpZipLib;
using Mono.Math;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Core;

namespace Alaris.Extras
{
	/// <summary>
	/// A class which provides useful methods for working with the world-wide web.
	/// </summary>
	public static class WebHelper
	{
		/// <summary>
		/// Gets the title of the specified webpage.
		/// </summary>
		/// <param name="url">
		/// The webpage's URL.
		/// </param>
		/// <returns>
		/// The webpage's title.
		/// </returns>
		public static string GetWebTitle(Uri url)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(url);
			
			request.Timeout = 3500;
			
			request.UserAgent = "Alaris Bot " + Utilities.GetBotVersion() + " / Mono " + Environment.Version;
			request.Referer = "http://www.wowemuf.org";
			
			var response = request.GetResponse();
			
			var stream = response.GetResponseStream();
			
			var rdr = new StreamReader(stream);
			var data = rdr.ReadToEnd();
			
			rdr.Close();
			stream.Close();
			response.Close();
			
			var getTitleRegex = new Regex(@"<title>(?<ttl>.*\s*.+\s*.*)\s*</title>", RegexOptions.IgnoreCase);
			
			var match = getTitleRegex.Match(data);
			
			return (match.Success) ? (match.Groups["ttl"].ToString()) : "No title found.";
		}
	}
}