using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using ICSharpCode.SharpZipLib;
using System.Reflection;
using System.Threading;
using System.Timers;
using Alaris.Irc;
using Alaris.Core;
using Timer=System.Timers.Timer;

namespace Alaris.Extras
{
	/// <summary>
	/// A class which provides methods to compile and run arbitrary Haskell code.
	/// </summary>
	public class HaskellEvaluator
	{
		private readonly AlarisBot _bot;
		
		/// <summary>
		/// Initializes a new instance of <see cref="HaskellEvaluator"/>
		/// </summary>
		/// <param name="bot">
		/// The bot instance.
		/// </param>
		public HaskellEvaluator(ref AlarisBot bot)
		{
			_bot = bot;
		}
		
		
	}
}
