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
using Atom.Core;
using Atom.Utils;
using Atom.AdditionalElements;
using Atom.Core.Collections;

namespace Alaris.FeedHandlerPlugin
{
	public static class FeedFactory
	{
		public static List<FeedRunner> FeedRunners = new List<FeedRunner>();
		
		static FeedFactory ()
		{
		}
		
		public static FeedRunner CreateFeedRunner(Uri feedurl, ref Connection con, IList<string> chans, string moduleName, int interval, string commandName)
		{
			var runner = new FeedRunner(feedurl, ref con, chans, moduleName, interval, commandName);
			FeedRunners.Add(runner);
			
			return runner;
			
		}
		
		public static void StartRunners()
		{
			foreach(var runner in FeedRunners)
			{
				runner.Start();
			}
		}
		
		public static void StopRunners()
		{
			foreach(var runner in FeedRunners)
			{
				runner.Stop();
			}
		}
	}
}

