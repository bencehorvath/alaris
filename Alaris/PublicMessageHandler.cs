using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Alaris.API;
using Alaris.API.Crypt;
using Alaris.API.Database;
using Alaris.Calculator;
using Alaris.Calculator.lexer;
using System.IO;
using Alaris.Calculator.node;
using Alaris.Calculator.parser;
using Alaris.Commands;
using Alaris.Irc;
using Alaris.Localization;
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
        private void OnPublicMessage(UserInfo user, string chan, string msg)
        {
            var urlsin = Utilities.GetUrls(msg);

            if (urlsin.Count > 0)
            {
                try
                {
                    foreach (var task in
                        urlsin.Select(url1 => new Task(() => Utilities.HandleWebTitle(ref _connection, chan, url1))))
                    {
                        task.Start();
                        
                        Thread.Sleep(400);
                    }

                    return;
                }
                catch (Exception ex)
                {
                    Log.Error("WebHelper", "Invalid webpage address: " + ex.Message);
                    //_connection.Sender.PublicMessage(chan, IrcConstants.Red + "Invalid address.");
                    return;
                }
            }

            CommandManager.HandleCommand(user, chan, msg);

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

                    _connection.Sender.PublicMessage(chan, calc.CalculatedResult.ToString());
                    return;
                   
                }
            }

            // Lua code runner.

            //LuaEngine.LuaHelper.HandleLuaCommands(_manager.Lua.LuaVM, chan, msg);


            if (msg.Equals("@reload scripts", StringComparison.InvariantCultureIgnoreCase))
            {
                _manager.Lua.LoadScripts(true);
                SendMsg(chan, "Lua scripts reloaded.");
            }

            /*
            if (msg.Equals("@admin", StringComparison.InvariantCultureIgnoreCase))
                SendMsg(chan, "Sub-commands: list | delete | add");


            if(msg.StartsWith("@admin add ", StringComparison.InvariantCultureIgnoreCase) && Utilities.IsAdmin(user))
            {
                msg = msg.Replace("@admin add ", string.Empty);

                var parts = msg.Split(' ');

                if (parts.Length != 3) { SendMsg(chan, "Syntax: ADMIN ADD <user> <nick> <hostname>");
                    return;  }

                DatabaseManager.Query(string.Format("INSERT INTO admins(user,nick,hostname) VALUES('{0}', '{1}', '{2}')", parts[0], parts[1], parts[2]));

                SendMsg(chan, string.Format("Admin {0} has been added.", parts[1]));
            }

            

            if(msg.Equals("@admin list", StringComparison.InvariantCultureIgnoreCase))
            {
                if(AdminManager.GetAdmins() == null)
                    SendMsg(chan, "No admins."); // shouldn't happen.
                else
                {
                    foreach(var adm in AdminManager.GetAdmins())
                        SendMsg(chan, adm);
                }
            }

            if(msg.StartsWith("@admin delete ", StringComparison.InvariantCultureIgnoreCase) && Utilities.IsAdmin(user))
            {
                msg = msg.Replace("@admin delete ", string.Empty);
                AdminManager.DeleteAdmin(msg);
                SendMsg(chan, string.Format("Admin {0} deleted.", msg));              
            }*/

   
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
                    Log.Error("Math", x.ToString());
                    Log.Debug("Math", double.Parse("3.55", CultureInfo.InvariantCulture).ToString());
                    SendMsg(chan, "Hiba!");

                }
            }

            //if(msg.StartsWith("@aes encrypt "))
            //{
            //    msg = msg.Replace("@aes encrypt ", string.Empty);

            //    SendMsg(chan, Rijndael.EncryptString(msg));
            //}

            //if (msg.StartsWith("@aes decrypt "))
            //{
            //    msg = msg.Replace("@aes decrypt ", string.Empty);

            //    SendMsg(chan, Rijndael.DecryptString(msg));
            //}

        }
    }
}