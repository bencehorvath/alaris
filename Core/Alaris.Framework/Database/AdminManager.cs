using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Alaris.Irc;

namespace Alaris.Framework.Database
{
    /// <summary>
    /// Class providing static methods for admin management.
    /// </summary>
    public static class AdminManager
    {
        /// <summary>
        /// Creates a new administrator in the database.
        /// </summary>
        /// <param name="user">IRC username</param>
        /// <param name="nick">IRC nick</param>
        /// <param name="hostname">IRC hostname</param>
        [DatabaseAccessor("Creates a new administrator", DatabaseAccessType.Insert, "admins")]
        public static void NewAdmin(string user, string nick, string hostname)
        {
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(nick) || string.IsNullOrEmpty(hostname) || !Irc.Rfc2812Util.IsValidNick(nick))
                return;

            user = HttpUtility.HtmlEncode(user);
            nick = HttpUtility.HtmlEncode(nick);
            hostname = HttpUtility.HtmlEncode(hostname);

            DatabaseManager.Query(string.Format("INSERT INTO admins(user,nick,hostname) VALUES('{0}', '{1}', '{2}')", user, nick, hostname));
        }

        /// <summary>
        /// Deletes the given admin.
        /// </summary>
        /// <param name="nick">Nick of the admin to delete.</param>
        [DatabaseAccessor("Deletes the given administrator.", DatabaseAccessType.Delete, "admins")]
        public static void DeleteAdmin(string nick)
        {
            if (string.IsNullOrEmpty(nick) || !Rfc2812Util.IsValidNick(nick))
                return;

            DatabaseManager.Query(string.Format("DELETE FROM admins WHERE nick = '{0}'", nick));
        }

        /// <summary>
        /// Determines whether the given user is admin or not.
        /// </summary>
        /// <param name="user">The user to check.</param>
        [DatabaseAccessor("Determines whether the given user is admin or not.", DatabaseAccessType.Select, "admins")]
        internal static bool IsAdmin(UserInfo user)
        {
            return
                (DatabaseManager.QueryFirstRow(
                    string.Format("SELECT * FROM admins WHERE user = '{0}' AND nick = '{1}' AND hostname = '{2}'",
                                  user.User, user.Nick, user.Hostname))) != null;
        }

        /// <summary>
        /// Gets the list of admins.
        /// </summary>
        /// <returns>List of admin nicks</returns>
        public static List<string> GetAdmins()
        {
            var table = DatabaseManager.Query("SELECT * FROM admins");

            var admins = (from DataRow row in table.Rows where row["nick"] != null select row["nick"].ToString()).ToList();

            return admins;
        }

        /// <summary>
        /// Gets the list of admins.
        /// </summary>
        public static List<string> Admins
        {
            get { return GetAdmins(); }
        }

        
    }
}
