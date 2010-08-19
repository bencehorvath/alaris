using System;
using System.Threading;
using Alaris.Core;
using Alaris.Irc;
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
                        Utilities.HandleWebTitle(ref _connection, chan, url);
                        Thread.Sleep(100);
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

            if (msg == "@quit" && Utilities.IsAdmin(user))
            {
                Disconnect("Quit command used by " + user.Nick);

                return;
            }

            if (msg == "@help")
            {
                _connection.Sender.PublicMessage(chan, "Available commands: info | quit | sys | join | title | mangos");
                return;
            }

            if (msg == "@info")
            {
                Utilities.SendInfo(ref _connection, chan);
                return;
            }

            if (msg == "@sys")
            {
                Utilities.SendSysStats(ref _connection, chan);

                return;
            }

            if (msg.StartsWith("@join ") && Utilities.IsAdmin(user))
            {
                string ch = msg.Replace("@join ", string.Empty);
                if (Rfc2812Util.IsValidChannelName(ch))
                    _connection.Sender.Join(ch);

                return;
            }


            if (msg == "@plugins")
            {
                foreach (var plugin in _manager.GetPlugins())
                {
                    _connection.Sender.PublicMessage(chan,
                                                     plugin.GetName().Replace("Plugin", string.Empty) + ": " +
                                                     IrcConstants.Green + "loaded.");
                }

                Log.Notice("Alaris", "Sent plugin list.");

                return;
            }

            if (msg.StartsWith("@plugin load ") && Utilities.IsAdmin(user))
            {
                string plname = msg.Replace("@plugin load ", string.Empty);

                if (_manager.LoadPlugin(plname))
                {
                    _connection.Sender.PublicMessage(chan,
                                                     IrcConstants.Bold + "[Load]: " + IrcConstants.Normal +
                                                     IrcConstants.Olive + plname + IrcConstants.Normal + " done.");
                    return;
                }

                _connection.Sender.PublicMessage(chan,
                                                 IrcConstants.Bold + "[Load]: " + IrcConstants.Normal +
                                                 IrcConstants.Olive + plname + IrcConstants.Normal + " failed.");

                return;
            }

            if (msg.StartsWith("@plugin unload ") && Utilities.IsAdmin(user))
            {
                string plname = msg.Replace("@plugin unload ", string.Empty);

                if (_manager.UnloadPlugin(plname))
                {
                    _connection.Sender.PublicMessage(chan,
                                                     IrcConstants.Bold + "[Unload]: " + IrcConstants.Normal +
                                                     IrcConstants.Olive + plname + IrcConstants.Normal + " done.");
                    return;
                }

                _connection.Sender.PublicMessage(chan,
                                                 IrcConstants.Bold + "[Unload]: " + IrcConstants.Normal +
                                                 IrcConstants.Olive + plname + IrcConstants.Normal + " failed.");

                return;
            }

            if (msg == "@request acs random" && AlarisServer)
            {
                AcsRandRequestChannel = chan;
                var packet = new AlarisPacket();
                packet.Write((int) Opcode.CmsgRequestACSRandom);
                packet.Write(chan);
                SendPacketToACS(packet);
                packet.Dispose();
            }

            if (msg.StartsWith("@sayid ") && MysqlEnabled)
            {
                int id;
                try
                {
                    id = Convert.ToInt32(msg.Remove(0, 7));
                }
                catch
                {
                    return;
                }

                var row = _sDatabaseManager.QueryFirstRow("SELECT msg FROM messages WHERE id = '" + id + "'");

                if (row == null)
                {
                    _connection.Sender.PublicMessage(chan, "Nincs ilyen sor.");
                    return;
                }

                _connection.Sender.PublicMessage(chan, row["msg"].ToString());
            }

            _manager.RunPublicHandlers(user, chan, msg);
        }
    }
}