namespace Alaris.Framework.Commands
{
    /// <summary>
    /// Specifies a command's access permission.
    /// </summary>
    public enum CommandPermission
    {
        /// <summary>
        /// Normals user permission. Anyone can use that command.
        /// </summary>
        Normal,
        /// <summary>
        /// Admin permission. Only admins are allowed to use that command.
        /// </summary>
        Admin
    }
}
