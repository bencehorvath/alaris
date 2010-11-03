using System.Data.SQLite;

namespace Alaris.API.Database
{
    /// <summary>
    /// A manager interface.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Initializes the manager.
        /// </summary>
        void Initialize();

        /// <summary>
        /// The manager's resource connection.
        /// </summary>
        SQLiteConnection Connection { get; }
    }
}
