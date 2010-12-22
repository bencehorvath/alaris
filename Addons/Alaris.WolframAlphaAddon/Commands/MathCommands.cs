using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Alaris.Commands;
using Alaris.Extensions;
using NLog;

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
            var rgx =
                new Regex(
                    @"Result\:<\/h2><div.+\sclass\=.sub.><div\sclass=.output\spnt.\sid=.\S+.><img.+\salt=.(?<result>.+).\stitle",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var expression = parameters.ConcatenateWithSpaces();

            var wacode = HttpUtility.UrlEncode(expression);

            var client = new WebClient();
            var retdt = client.DownloadString(string.Format("http://www.wolframalpha.com/input/?i={0}", wacode));

            if (retdt.IsNull())
            {
                Log.Error("WA response was empty");
                return;
            }

            if (!rgx.IsMatch(retdt))
            {
                Log.Error("The regular expression didn't match the WA response");
                return;
            }

            var match = rgx.Match(retdt);

            var result = match.Groups["result"].ToString();

            mp.Bot.SendMsg(mp.Channel, result);
        }
    }
}