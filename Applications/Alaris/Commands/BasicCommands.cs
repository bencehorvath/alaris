using Alaris.API;
using Alaris.Irc;
using Alaris.Localization;

namespace Alaris.Commands
{
    /// <summary>
    /// Basic Alaris commands.
    /// </summary>
    public static class BasicCommands
    {
        ///<summary>
        /// Handles the quit command.
        ///</summary>
        ///<param name="mp"></param>
        [AlarisCommand("quit", CommandPermission.Admin)]
        public static void HandleQuitCommand(AlarisMainParameter mp)
        {
            mp.Bot.Disconnect("Quit command used by " + mp.User.Nick);
            return;
        }

        /// <summary>
        /// Handles the help command.
        /// </summary>
        /// <param name="mp"></param>
        [AlarisCommand("help")]
        public static void HandleHelpCommand(AlarisMainParameter mp)
        {
            mp.IrcConnection.Sender.PublicMessage(mp.Channel, string.Format("{0}: info | quit | sys | join | title | calc | sort | admin", "Available commands"));
            return;
        }

        /// <summary>
        /// Handles the information command.
        /// </summary>
        /// <param name="mp"></param>
        [AlarisCommand("info")]
        public static void HandleInfoCommand(AlarisMainParameter mp)
        {
            var ircConnection = mp.IrcConnection;
            Utilities.SendInfo(ref ircConnection, mp.Channel);
            return;
        }

        /// <summary>
        /// Handles the system information command.
        /// </summary>
        /// <param name="mp"></param>
        [AlarisCommand("sys")]
        public static void HandleSysCommand(AlarisMainParameter mp)
        {
            var connection = mp.IrcConnection;
            Utilities.SendSysStats(ref connection, mp.Channel);

            return;
        }

        /// <summary>
        /// Handles the join command.
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="chan"></param>
        [ParameterizedAlarisCommand("join")]
        public static void HandleJoinCommand(AlarisMainParameter mp, string chan)
        {
            if (Rfc2812Util.IsValidChannelName(chan))
                mp.IrcConnection.Sender.Join(chan);

            return;
        }


    }
}
