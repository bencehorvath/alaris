using System;

namespace Alaris.API.Database
{
    /// <summary>
    /// Attribute used to mark methods which access the database.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DatabaseAccessorAttribute : Attribute
    {
        private readonly string _doc;
        private readonly string[] _tables;
        private readonly DatabaseAccessType _accessType;

        /// <summary>
        /// Marks a method as a database accessor.
        /// </summary>
        /// <param name="doc">The documentation of the method.</param>
        /// <param name="accessType">Type of access.</param>
        /// <param name="tables">Tables which it accesses.</param>
        public DatabaseAccessorAttribute(string doc, DatabaseAccessType accessType, params string[] tables)
        {
            _doc = doc;
            _tables = tables;
            _accessType = accessType;
        }
    }
}