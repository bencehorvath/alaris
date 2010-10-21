using System;

namespace Alaris.Exceptions
{
    /// <summary>
    ///   Exception thrown when an invalid config file is detected.
    /// </summary>
    [Serializable]
    public sealed class ConfigFileInvalidException : Exception
    {
        /// <summary>
        ///   Creates a new instance of <see cref = "ConfigFileInvalidException" />
        /// </summary>
        public ConfigFileInvalidException()
        {
        }

        /// <summary>
        ///   Creates a new instance of <see cref = "ConfigFileInvalidException" />
        /// </summary>
        /// <param name = "msg">
        ///   The message of the exception.
        /// </param>
        public ConfigFileInvalidException(string msg) : base(msg)
        {
        }
    }
}