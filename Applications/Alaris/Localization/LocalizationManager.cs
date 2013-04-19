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
        /// <param name="locale">Locale (default: enUS)</param>
        /// <returns></returns>
        public static string GetLocalizedText(string text, string locale = "enUS")
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(locale) || locale.Length != 4)
                return string.Empty;

            if (locale == "enUS")
                return text;

            return text;
        }
    }
}
