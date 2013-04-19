using System;
using System.Text;
using Alaris.Framework.Commands;
using Alaris.Framework.Crypt;
using Alaris.Framework.Extensions;

namespace Alaris.Commands
{
    /// <summary>
    /// Advanced Alaris commands
    /// </summary>
    public static class AdvancedCommands
    {
        /// <summary>
        /// Handles AES commands.
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="parameters"></param>
        [ParameterizedAlarisCommand("aes", CommandPermission.Normal, 0, true)]
        public static void HandleAesCommands(AlarisMainParameter mp, params string[] parameters)
        {
            var action = parameters[0];
            var sb = new StringBuilder();
            for(var i = 1; i < parameters.Length; ++i)
                sb.AppendFormat("{0} ", parameters[i]);

            var text = sb.ToString();
            if (action.IsNull() || text.IsNull())
                return;


            if (action.Equals("decrypt", StringComparison.InvariantCultureIgnoreCase))
            {
                mp.Bot.SendMsg(mp.Channel, Rijndael.DecryptString(text));
            }
            else if(action.Equals("encrypt", StringComparison.InvariantCultureIgnoreCase))
                HandleAesEncryptCommand(mp, text);
        }

        /// <summary>
        /// Handles the Rijndael encrypt command.
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="text"></param>
        [AlarisSubCommand("aes encrypt")]
        public static void HandleAesEncryptCommand(AlarisMainParameter mp, string text)
        {
            mp.Bot.SendMsg(mp.Channel, Rijndael.EncryptString(text));   
        }

        /// <summary>
        /// Handles the add command.
        /// </summary>
        /// <param name="mp">The mp.</param>
        /// <param name="a">A.</param>
        /// <param name="b">The b.</param>
        [ParameterizedAlarisCommand("add", CommandPermission.Normal, 2)]
        public static void HandleAddCommand(AlarisMainParameter mp, string a, string b)
        {
            mp.Bot.SendMsg(mp.Channel, (a.Cast<int>() + b.Cast<int>()).ToString());
        }
    }
}
