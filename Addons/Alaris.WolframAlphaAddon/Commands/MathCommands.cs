using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Alaris.Commands;
using Alaris.Framework;
using Alaris.Framework.Commands;
using Alaris.Framework.Extensions;
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
            var resultRegex =
                new Regex(
                    @"(Result[s]?)\:<\/h2><div.+\sclass\=.sub.><div\sclass=.output\spnt.\sid=.\S+.><img.+\salt=.(?<result>.+).\stitle",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var solutionRegex =
                new Regex(
                    @"(Solution[s]?)\:<\/h2><\S+.+\sclass\=.sub.><div\sclass=.output\spnt.\sid=.\S+.><img.+\salt=.(?<result>.+).\stitle",
                    RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

            var expression = parameters.ConcatenateWithSpaces();
            expression = expression.Replace("=", " = "); // for equations

            var wacode = HttpUtility.UrlEncode(expression);

            var retdt = Utilities.GetWebsiteString(string.Format("http://www.wolframalpha.com/input/?i={0}", wacode));

            if (retdt.IsNull())
            {
                Log.Error("WA response was empty");
                return;
            }

            Match match;

            if (solutionRegex.IsMatch(retdt))
                match = solutionRegex.Match(retdt);
            else if (resultRegex.IsMatch(retdt))
                match = resultRegex.Match(retdt);
            else
            {
                Log.Error("The regular expression didn't match the WA response");
                return;
            }

            var result = match.Groups["result"].ToString().Replace("\\/", "/");

            mp.Bot.SendMsg(mp.Channel, result);
        }
    }
}