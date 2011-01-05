using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Alaris.API;
using Alaris.Calculator;
using Alaris.Calculator.lexer;
using System.IO;
using Alaris.Calculator.node;
using Alaris.Calculator.parser;
using Alaris.Commands;
using Alaris.Framework;
using Alaris.Framework.Commands;
using Alaris.Irc;
using Alaris.Mathematics.Types;

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
            ThreadPool.QueueUserWorkItem(wc =>
                                             {
                                                 var urlsin = Utilities.GetUrls(smsg);

                                                 if (urlsin.Count <= 0) return;

                                                 try
                                                 {
                                                     Parallel.ForEach(urlsin, url => Utilities.HandleWebTitle(Connection, chan, url));
                                                     return;
                                                 }
                                                 catch (Exception ex)
                                                 {
                                                     Log.Error("Invalid webpage address: {0}", ex.Message);
                                                     //_connection.Sender.PublicMessage(chan, IrcConstants.Red + "Invalid address.");
                                                     return;
                                                 }
                                             });

            if(msg.StartsWith("@calc ", StringComparison.InvariantCultureIgnoreCase) || msg.StartsWith("@c ", StringComparison.InvariantCultureIgnoreCase))
            {
                if (msg.StartsWith("@calc ", StringComparison.InvariantCultureIgnoreCase)) msg = msg.Replace("@calc ", string.Empty);
                else if (msg.StartsWith("@c ", StringComparison.InvariantCultureIgnoreCase)) msg = msg.Replace("@c ", string.Empty);

                using (var reader = new StringReader(msg))
                {
                    var lexer = new Lexer(reader);

                    var parser = new Parser(lexer);

                    Start ast;

                    try {
                        ast = parser.Parse(); }
                    catch(Exception x)
                    {
                        Log.Error("Math", x.ToString());
                        return;
                    }

                    var printer = new AstPrinter();
                    ast.Apply(printer);
                    printer.Dispose();

                    var calc = new AstCalculator();
                    ast.Apply(calc);

                    SendMsg(chan, calc.CalculatedResult.ToString());
                    return;
                   
                }
            }

            // Lua code runner.

            //LuaEngine.LuaHelper.HandleLuaCommands(_manager.Lua.LuaVM, chan, msg);


            if (msg.Equals("@reload scripts", StringComparison.InvariantCultureIgnoreCase))
            {
                ScriptManager.Lua.LoadScripts(true);
                SendMsg(chan, "Lua scripts reloaded.");
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
                    {
                        nums.SimpleAdd(double.Parse(match.Groups["num"].ToString()));
                    }

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