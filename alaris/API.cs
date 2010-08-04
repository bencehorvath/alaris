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
using Alaris.Extras;

namespace Alaris.Core
{
	public partial class AlarisBot
	{
		/// <summary>
		/// Sends the given message to the specified channel.
		/// </summary>
		/// <param name="channel">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		public void SendMsg(string channel, string message)
		{
			_connection.Sender.PublicMessage(channel, message);
		}
	}
}