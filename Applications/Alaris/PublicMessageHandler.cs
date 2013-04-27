using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using Alaris.Framework;
using Alaris.Framework.Maths.Types;
using Alaris.Irc;

namespace Alaris
{
    public partial class AlarisBot
    {
        /// <summary>
        ///   The method called when a public message occurs in one of the channels the bot is on.
        /// </summary>
        /// <param name = "user">
        ///   The data about the user who sent the message.
        /// </param>
        /// <param name = "chan">
        ///   The channel where the message occured.
        /// </param>
        /// <param name = "msg">
        ///   The message that was sent.
        /// </param>
        protected override void OnPublicMessage(UserInfo user, string chan, string msg)
        {
            var smsg = msg;

            Task.Factory.StartNew(() =>
                                      {
                                          var urlsin = Utility.GetUrls(smsg);

                                          if (urlsin.Count <= 0) return;

                                          try
                                          {
                                              Parallel.ForEach(urlsin, url => Utility.HandleWebTitle(chan, url));
                                              return;
                                          }
                                          catch (Exception ex)
                                          {
                                              Log.Error("Invalid webpage address: {0}", ex.Message);
                                              //_connection.Sender.PublicMessage(chan, IrcConstants.Red + "Invalid address.");
                                              return;
                                          }
                                      });


            // Lua code runner.

            //LuaEngine.LuaHelper.HandleLuaCommands(_manager.Lua.LuaVM, chan, msg);


            if (msg.Equals("@reload scripts", StringComparison.InvariantCultureIgnoreCase))
            {
                ScriptManager.Lua.LoadScripts(true);
                SendMsg(chan, "Lua scripts reloaded.");
            }

            if(msg.Equals("@UnloadPlugins", StringComparison.InvariantCultureIgnoreCase))
            {
                UnloadAll();
                SendMsg(chan, "Done.");
            }

            if(msg.Equals("@LoadPlugins", StringComparison.InvariantCultureIgnoreCase))
            {
                LoadAll();
                SendMsg(chan, "Done.");
            }

            if(msg.Equals("@ReloadPlugins", StringComparison.InvariantCultureIgnoreCase))
            {
                ReloadAll();
                SendMsg(chan, "Done.");
            }


            if(msg.StartsWith("@sort "))
            {
                var rest = msg.Remove(0, 6);
                var rgx = new Regex(@"(?<num>(?<prefix>\S)?(\d+(\.\d+)?))");

                if (!rgx.IsMatch(rest))
                    return;

                try
                {

                    var matches = rgx.Matches(rest);

                    var nums = new AutoSortedArray<double>();

                    foreach (Match match in matches)
                        nums.SimpleAdd(double.Parse(match.Groups["num"].ToString()));

                    nums.Sort();

                    SendMsg(chan, nums.ToString());
                }
                catch(Exception x)
                {
                    Log.Error(x.ToString());
                    Log.Debug(double.Parse("3.55", CultureInfo.InvariantCulture).ToString());
                    SendMsg(chan, "Hiba!");

                }
            }

            base.OnPublicMessage(user, chan, smsg);
        }
    }
}