using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Alaris.API;
using Alaris.Framework.Database;
using Alaris.Irc;
using NLog;

namespace Alaris.Framework
{
    /// <summary>
    /// Class providing useful methods for handling different things.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///   The admin user's username.
        /// </summary>
        public static string AdminUser { get; set; }

        /// <summary>
        ///   The admin user's nick.
        /// </summary>
        public static string AdminNick { get; set; }

        /// <summary>
        ///   The admin user's hostname.
        /// </summary>
        public static string AdminHost { get; set; }

        private static readonly object SendLock = new object();

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Adminsitrator list.
        /// </summary>
        public static readonly List<Admin> Admins = new List<Admin>();

        static Utilities()
        {
            Admins.Add(new Admin
                           {
                               User = AdminUser,
                               Nick = AdminNick,
                               Host = AdminHost
                           });
        }

        /// <summary>
        ///   Sends system stats using the specified connection.
        /// </summary>
        /// <param name = "connection">
        ///   The IRC connection.
        /// </param>
        /// <param name = "chan">
        ///   The channel to send to.
        /// </param>
        public static void SendSysStats(ref Connection connection, string chan)
        {
            Log.Info("System info request.");
            var hostname = Environment.MachineName;
            var username = Environment.UserName;

            var os = Environment.OSVersion.ToString();
            var mem = GC.GetTotalMemory(true)/1024/1024;
            var gen = GC.GetGeneration(connection);
            

            connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Bot version: " + IrcConstants.Normal + BotVersion);
            connection.Sender.PublicMessage(chan, IrcConstants.Bold + "Thread count: " + IrcConstants.Normal + Process.GetCurrentProcess().Threads.Count);
            connection.Sender.PublicMessage(chan,
                                            IrcConstants.Bold + "CPU: " + IrcConstants.Normal + GetCpuId() + " | " +
                                            Environment.ProcessorCount + " cores.");

            if (mem < 60)
                connection.Sender.PublicMessage(chan,
                                                string.Format("{0}Memory allocated: {1}{2}{3} MB (gen: {4})", 
                                                IrcConstants.Bold, IrcConstants.Normal, IrcConstants.Green, mem, gen));
            else if (mem > 60 && mem < 80)
                connection.Sender.PublicMessage(chan,
                                                string.Format("{0}Memory allocated: {1}{2}{3} MB (gen: {4})", 
                                                IrcConstants.Bold, IrcConstants.Normal, IrcConstants.Olive, mem, gen));
            else
                connection.Sender.PublicMessage(chan,
                                                string.Format("{0}Memory allocated: {1}{2}{3} MB (gen: {4})", 
                                                IrcConstants.Bold, IrcConstants.Normal, IrcConstants.Red, mem, gen));
        }

        /// <summary>
        ///   Sends info using the specified connection.
        /// </summary>
        /// <param name = "connection">
        ///   The IRC connection.
        /// </param>
        /// <param name = "chan">
        ///   The channel to send to.
        /// </param>
        public static void SendInfo(ref Connection connection, string chan)
        {
            connection.Sender.PublicMessage(chan, IrcConstants.Cyan + "Alaris " + BotVersion);
            connection.Sender.PublicMessage(chan, IrcConstants.DarkGreen + "Developer: Twl");
        }

        /// <summary>
        ///   Determines whether the specified user is admin or not.
        /// </summary>
        /// <param name = "user">
        ///   The user to check.
        /// </param>
        /// <returns>
        ///   True if admin, otherwise false.
        /// </returns>
        public static bool IsAdmin(UserInfo user)
        {
            if (user == null) throw new ArgumentNullException("user");
            return ((user.Hostname == AdminHost && user.Nick == AdminNick && user.User == AdminUser) || AdminManager.IsAdmin(user));
        }

        /// <summary>
        ///   Calculates the MD5 sum of a file.
        /// </summary>
        /// <param name = "fileName">
        ///   The file to check.
        /// </param>
        /// <returns>
        ///   The MD5 hash.
        /// </returns>
        public static string MD5File(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            byte[] retVal;

            using(var file = new FileStream(fileName, FileMode.Open))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                retVal = md5.ComputeHash(file);
                md5.Dispose();
            }

            var sb = new StringBuilder();

            if (retVal != null)
                for (var i = 0; i < retVal.Length; i++)
                    sb.Append(retVal[i].ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        ///   Calculates the MD5 hash of a string.
        /// </summary>
        /// <param name = "value">
        ///   The string to calculate MD5 hash of.
        /// </param>
        /// <returns>
        ///   The MD5 hash.
        /// </returns>
        public static string MD5String(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            var x = new MD5CryptoServiceProvider();

            var data = Encoding.ASCII.GetBytes(value);
            data = x.ComputeHash(data);
            x.Dispose();
            var ret = "";

            for (var i = 0; i < data.Length; i++)
                ret += data[i].ToString("x2").ToLower();

            return ret;
        }

        /// <summary>
        ///   The current unix time.
        /// </summary>
        public static double UnixTime
        {
            get
            {
                var elapsed = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));

                return (elapsed.TotalSeconds);
            }
        }

        /// <summary>
        ///   The bot's assembly version.
        /// </summary>
        public static string BotVersion
        {
            get { return (Assembly.GetExecutingAssembly().GetName().Version.ToString()); }
        }

        /// <summary>
        ///   Gets the URLs in the specified text.
        /// </summary>
        /// <param name = "text">
        ///   The text to search in.
        /// </param>
        /// <returns>
        ///   The list of urls.
        /// </returns>
        public static List<string> GetUrls(string text)
        {
            var urls = new List<string>();

            try
            {
                //var urlFind = new Regex(@"(?<page>http://(www\.)?\S+\.\S{2,6}(/\S*\s*)?)", RegexOptions.IgnoreCase);
                //var urlFind = new Regex(@"(?<page>http://(www\.)?\S+\.\S{2,6}(/*\S*))", RegexOptions.IgnoreCase);

                var urlFind = new Regex(@"(?<url>(http://)?(www\.)?\S+\.\S{2,6}([/]*\S+))",
                                        RegexOptions.Compiled | RegexOptions.IgnoreCase);

                if (urlFind.IsMatch(text))
                {
                    var matches = urlFind.Matches(text);

                    //urls.AddRange(from Match match in matches select match.Groups["page"].ToString());

                    foreach (var url in from Match match in matches select match.Groups["url"].ToString())
                    {
                        var lurl = url;
                        if (!lurl.StartsWith("http://") && !url.StartsWith("https://"))
                            lurl = string.Format("http://{0}", url);

                        Log.Debug("Utilities", string.Format("Checking: {0}", url));

                        urls.Add(lurl);
                    }
                }
            }
            catch (Exception x)
            {
                Log.Error(x.Message);
            }

            return urls;
        }

        /// <summary>
        ///   Handles the web title command.
        /// </summary>
        /// <param name = "connection">
        ///   The IRC connection used.
        /// </param>
        /// <param name = "chan">
        ///   The channel to send title to.
        /// </param>
        /// <param name = "msg">
        ///   The message containing the url.
        /// </param>
        public static void HandleWebTitle(Connection connection, string chan, string msg)
        {
           
            if (connection == null) throw new ArgumentNullException("connection");
            if (chan == null) throw new ArgumentNullException("chan");
            if (msg == null) throw new ArgumentNullException("msg");

            try
            {

                var tt = msg.Replace("@title ", string.Empty);

                var url = new Uri(tt);
                var webTitle = WebHelper.GetWebTitle(url);

                if (string.IsNullOrEmpty(webTitle))
                    return;

                var title = Regex.Replace(webTitle, @"\s+", " ");


                // check if it's youtube.
                var youtubeRegex = new Regex(@"\s*YouTube\s*\-(?<song>.+)", RegexOptions.IgnoreCase);

                if (youtubeRegex.IsMatch(title))
                {
                    var match = youtubeRegex.Match(title);
                    var song = match.Groups["song"].ToString();

                    lock (SendLock)
                    {
                        connection.Sender.PublicMessage(chan,
                                                        IrcConstants.Purple + "[YouTube]: " + IrcConstants.DarkGreen +
                                                        song.Substring(1));
                        // about substr: remove the space before song name
                    }
                    return;
                }

                lock (SendLock)
                {
                    Log.Debug("WebHelper", string.Format("Title: {0}", title));
                    connection.Sender.PublicMessage(chan,
                                                    IrcConstants.Bold + "[Title]: " + IrcConstants.Normal +
                                                    IrcConstants.DarkGreen + title);
                }
            }
            catch(Exception x)
            {
                Log.Debug("Utilities", x.Message);
                return;
            }

        }

        /// <summary>
        ///   Gets the cpu brand string.
        /// </summary>
        /// <returns>
        ///   The CPU brand string.
        /// </returns>
        private static string GetCpuId()
        {
            var mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

            return
                (from ManagementObject mo in mos.Get() select (Regex.Replace(Convert.ToString(mo["Name"]), @"\s+", " ")))
                    .FirstOrDefault();
        }


        /// <summary>
        /// Executes the provided action safely.
        /// </summary>
        /// <param name="action">The action.</param>
        public static void ExecuteSafely(Action action)
        {
            try
            {
                action();
            }
            catch(Exception x)
            {
                Log.Error("An exception has been thrown inside the safe execution context. ({0})", x.Message);
                return;
            }
        }

    }
}
