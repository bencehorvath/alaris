using System;

namespace Alaris.API
{
    /// <summary>
    /// Interface which forces a component to have a GUID.
    /// </summary>
    public interface IAlarisComponent
    {
        /// <summary>
        ///   Gets the GUID of the instance(class).
        /// </summary>
        /// <returns>
        ///   The GUID.
        /// </returns>
        Guid GetGuid();
    }
}
