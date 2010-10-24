using System;

namespace Alaris.API
{
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
