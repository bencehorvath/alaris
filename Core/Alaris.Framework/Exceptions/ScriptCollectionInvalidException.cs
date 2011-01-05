using System;

namespace Alaris.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown when a script collection is found invalid.
    /// </summary>
    public sealed class ScriptCollectionInvalidException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScriptCollectionInvalidException" />
        /// </summary>
        /// <param name="reason"></param>
        public ScriptCollectionInvalidException(string reason) : base(reason) {}
    }
}
