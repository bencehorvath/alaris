using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Alaris.Commands;
using Alaris.Framework;
using Alaris.Framework.Commands;
using Alaris.Framework.Extensions;
using NLog;
using WolframAPI;

namespace Alaris.WolframAlphaAddon.Commands
{
    public static class MathCommands
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Handles Wolfram Alpha computation requests.
        /// </summary>
        /// <param name="mp"></param>
        /// <param name="parameters"></param>
        [ParameterizedAlarisCommand("wa", CommandPermission.Normal, 0, true)]
        public static void HandleWACommand(AlarisMainParameter mp, params string[] parameters)
        {
            var expression = parameters.ConcatenateWithSpaces();
            expression = expression.Replace("=", " = "); // for equations

            var client = new WAClient("557QYQ-UUUWTKX95V");
            var result = client.Solve(expression);
            
            mp.Bot.SendMsg(mp.Channel, result);
        }
    }
}