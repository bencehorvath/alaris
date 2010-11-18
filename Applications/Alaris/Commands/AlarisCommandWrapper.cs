using System;

namespace Alaris.Commands
{
    ///<summary>
    /// Marks a method as an Alaris command.
    ///</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AlarisCommandAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; protected set; }
        /// <summary>
        /// Gets the command permission.
        /// </summary>
        public CommandPermission Permission { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Marks a method as an Alaris command.
        /// </summary>
        /// <param name="command">The command corresponding to this method.</param>
        /// <param name="permission">Command's access permission</param>
        public AlarisCommandAttribute(string command, CommandPermission permission = CommandPermission.Normal)
        {
            Command = command;
            Permission = permission;
        }

        #endregion
    }

    ///<summary>
    /// Marks a method as an Alaris sub-command.
    ///</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AlarisSubCommandAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; protected set; }
        /// <summary>
        /// Gets the command permission.
        /// </summary>
        public CommandPermission Permission { get; protected set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Marks a method as an Alaris sub-command.
        /// </summary>
        /// <param name="command">The command corresponding to this method.</param>
        /// <param name="permission">Command's access permission</param>
        public AlarisSubCommandAttribute(string command, CommandPermission permission = CommandPermission.Normal)
        {
            Command = command;
            Permission = permission;
        }

        #endregion
    }

    /// <summary>
    /// Alaris IRC command.
    /// </summary>
    public sealed class AlarisCommandWrapper : IEquatable<AlarisCommandWrapper>
    {
        #region Properties

        /// <summary>
        /// The command text.
        /// </summary>
        public string Command { get; set; }
        /// <summary>
        /// Permission of the command.
        /// </summary>
        public CommandPermission Permission { get; set; }

        /// <summary>
        /// Gets or sets whether the parameter count is unspecified or not.
        /// </summary>
        public bool IsParameterCountUnspecified { get; set; }

        #endregion

        #region IEquatable<T>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(AlarisCommandWrapper other)
        {
            return (Command == other.Command && Permission == other.Permission);
        }

        #endregion
    }
}
