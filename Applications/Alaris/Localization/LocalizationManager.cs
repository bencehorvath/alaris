using Alaris.API.Database;
using Alaris.Irc;
using NLog;

namespace Alaris.Localization
{
    /// <summary>
    /// Class used to manage localizations of texts in Alaris.
    /// </summary>
    public static class LocalizationManager
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Retrieves the specified text's translation to the specified language from the database.
        /// </summary>
        /// <param name="text">Text to translate</param>
        /// <param name="locale">Locale (default: enGB)</param>
        /// <returns></returns>
        public static string GetLocalizedText(string text, string locale)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(locale) || locale.Length != 4)
                return string.Empty;

            if (locale == "enGB")
                return text;

            text = text.ToLowerInvariant();

            var row = DatabaseManager.QueryFirstRow(string.Format("SELECT text FROM localization WHERE LOWER(originalText) = '{0}' AND locale = '{1}'", text, locale));

            if (row == null)
            {
                Log.Info("No translations found");
                return text;
            }

            var localizedText = row["text"].ToString();

            return localizedText;
        }
    }
}
