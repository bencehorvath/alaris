using System;
using System.Linq;
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
            var result = client.GetResult(expression);

            var results = from pod in result.Pods
                          where pod.Title.ToLower().Contains("solution")
                                || pod.Title.ToLower().Contains("result")
                                || pod.Title.ToLower().Contains("derivative")
                          select pod;

            foreach (var rs in results.Select(pod => pod.SubPods[0].PlainText))
            {
                mp.Bot.SendMsg(mp.Channel, rs);
            }

            
        }
    }
}