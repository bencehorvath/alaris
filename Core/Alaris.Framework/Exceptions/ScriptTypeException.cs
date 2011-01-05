using System;

namespace Alaris.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown when the types in the compiled script are not appropirate.
    /// </summary>
    public sealed class ScriptTypeException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScriptTypeException" />
        /// </summary>
        /// <param name="reason"></param>
        public ScriptTypeException(string reason) : base(reason)
        {
        }
    }
}
