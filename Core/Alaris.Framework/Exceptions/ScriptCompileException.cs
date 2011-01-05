using System;

namespace Alaris.Framework.Exceptions
{
    /// <summary>
    /// Exception thrown when there was an error compiling a script.
    /// </summary>
    public sealed class ScriptCompileException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ScriptCompileException" />
        /// </summary>
        /// <param name="reason"></param>
        public ScriptCompileException(string reason) : base(reason)
        {
        }
    }
}
