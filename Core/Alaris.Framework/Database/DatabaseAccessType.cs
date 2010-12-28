using System;

namespace Alaris.Framework.Database
{
    /// <summary>
    /// Used to specifiy how a method accesses the database.
    /// </summary>
    [Flags]
    public enum DatabaseAccessType
    {
        /// <summary>
        /// It executes insert operations.
        /// </summary>
        Insert,
        /// <summary>
        /// It executes delete operations.
        /// </summary>
        Delete,
        /// <summary>
        /// It executes update operations.
        /// </summary>
        Update,
        /// <summary>
        /// It executes select operations (weakest)
        /// </summary>
        Select
    }
}
