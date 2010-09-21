using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Alaris.API
{
    /// <summary>
    ///   A class which provides useful methods for working with the world-wide web.
    /// </summary>
    public static class WebHelper
    {
        /// <summary>
        ///   Gets the title of the specified webpage.
        /// </summary>
        /// <param name = "url">
        ///   The webpage's URL.
        /// </param>
        /// <returns>
        ///   The webpage's title.
        /// </returns>
        public static string GetWebTitle(Uri url)
        {
            var request = (HttpWebRequest) WebRequest.Create(url);
          
            request.Timeout = 3500;

            request.UserAgent = "Alaris Bot " + Utilities.BotVersion + " / .NET " + Environment.Version;
            request.Referer = "http://www.wowemuf.org";

            var response = request.GetResponse();

            var stream = response.GetResponseStream();

            var rdr = new StreamReader(stream);
            var data = rdr.ReadToEnd();

            rdr.Close();
            response.Close();

            var getTitleRegex = new Regex(@"<title>(?<ttl>.*\s*.+\s*.*)\s*</title>", RegexOptions.IgnoreCase);

            var match = getTitleRegex.Match(data);

            return (match.Success) ? (match.Groups["ttl"].ToString()) : "No title found.";
        }
    }
}