using System.Data.SQLite;

namespace Alaris.API.Database
{
    /// <summary>
    /// A manager interface.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Starts the manager.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops the manager.
        /// </summary>
        void Stop();
    }
}
