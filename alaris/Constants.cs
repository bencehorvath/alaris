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
	/// Class holding some constant values for IRC.
	/// </summary>
	public static class IrcConstants
	{
		/// <summary>
		/// Black irc color.
		/// </summary>
		public const string Black = "\u000301";
		/// <summary>
		/// Blie irc color.
		/// </summary>
		public const string Blue = "\u000312";
		/// <summary>
		/// Bold irc color.
		/// </summary>
		public const string Bold = "\u0002";
		/// <summary>
		/// Brown irc color.
		/// </summary>
		public const string Brown = "\u000305";
		/// <summary>
		/// Cyan irc color.
		/// </summary>
		public const string Cyan = "\u000311";
		/// <summary>
		/// Dark blue irc color.
		/// </summary>
		public const string DarkBlue = "\u000302";
		/// <summary>
		/// Dark gray irc color.
		/// </summary>
		public const string DarkGray = "\u000314";
		/// <summary>
		/// Dark green irc color.
		/// </summary>
		public const string DarkGreen = "\u000303";
		/// <summary>
		/// Green irc color.
		/// </summary>
		public const string Green = "\u000309";
		/// <summary>
		/// Light gray irc color.
		/// </summary>
		public const string LightGray = "\u000315";
		/// <summary>
		/// Magenta irc color.
		/// </summary>
		public const string Magenta = "\u000313";
		/// <summary>
		/// Normal irc color.
		/// </summary>
		public const string Normal = "\u000f";
		/// <summary>
		/// Olive irc color.
		/// </summary>
		public const string Olive = "\u000307";
		/// <summary>
		/// Purple irc color.
		/// </summary>
		public const string Purple = "\u000306";
		/// <summary>
		/// Red irc color.
		/// </summary>
		public const string Red = "\u000304";
		/// <summary>
		/// Reversed irc color.
		/// </summary>
		public const string Reverse = "\u0016";
		/// <summary>
		/// Teal irc color.
		/// </summary>
		public const string Teal = "\u000310";
		/// <summary>
		/// Underlined irc color.
		/// </summary>
		public const string Underline = "\u001f";
		/// <summary>
		/// White irc color.
		/// </summary>
		public const string White = "\u000300";
		/// <summary>
		/// Yellow irc color.
		/// </summary>
		public const string Yellow = "\u000308";
	}
			
}
