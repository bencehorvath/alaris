using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Alaris.API;
using Alaris.API.Database;
using Alaris.Calculator;
using Alaris.Calculator.lexer;
using System.IO;
using Alaris.Calculator.node;
using Alaris.Calculator.parser;
using Alaris.Irc;
using Alaris.Localization;
using Alaris.Network;

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
                    foreach (var url in urlsin)
                    {
                        var url1 = url;
                        var task = new Task(() => Utilities.HandleWebTitle(ref _connection, chan, url1));

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

            if (msg.Equals("@quit", StringComparison.InvariantCultureIgnoreCase) && Utilities.IsAdmin(user))
            {
                Disconnect("Quit command used by " + user.Nick);

                return;
            }

            if (msg.Equals("@help", StringComparison.InvariantCultureIgnoreCase))
            {
                _connection.Sender.PublicMessage(chan, string.Format("{0}: info | quit | sys | join | title | mangos", LocalizationManager.GetLocalizedText("Available commands", Locale)));
                return;
            }

            if (msg.Equals("@info", StringComparison.InvariantCultureIgnoreCase))
            {
                Utilities.SendInfo(ref _connection, chan);
                return;
            }

            if (msg.Equals("@sys", StringComparison.InvariantCultureIgnoreCase))
            {
                Utilities.SendSysStats(ref _connection, chan);

                return;
            }

            if (msg.StartsWith("@join ", StringComparison.InvariantCultureIgnoreCase) && Utilities.IsAdmin(user))
            {
                var ch = msg.Replace("@join ", string.Empty);
                if (Rfc2812Util.IsValidChannelName(ch))
                    _connection.Sender.Join(ch);

                return;
            }

            if (msg.Equals("@reload scripts", StringComparison.InvariantCultureIgnoreCase))
            {
                _manager.Lua.LoadScripts(true);
                SendMsg(chan, "Lua scripts reloaded.");
            }

            if (msg.Equals("@request acs random", StringComparison.InvariantCultureIgnoreCase) && AlarisServer)
            {
                AcsRandRequestChannel = chan;
                var packet = new AlarisPacket();
                packet.Write((int) Opcode.CmsgRequestACSRandom);
                packet.Write(chan);
                SendPacketToACS(packet);
                packet.Dispose();
            }

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
            }

            if(msg.StartsWith("@prime ", StringComparison.InvariantCultureIgnoreCase))
            {
                msg = msg.Remove(0, 7);
                var num = 0d;
                try {num = Convert.ToDouble(msg);}catch{}

                Log.Debug("Math", string.Format("Checking prime for: {0}", num));

                SendMsg(chan, Utilities.Math.IsPrime(num) ? "It's a prime." : "It's not a prime.");
            }

        }
    }
}