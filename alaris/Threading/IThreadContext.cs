using System;

namespace Alaris.Threading
{
    /// <summary>
    ///   The interface used to define classes that can be run in the <see cref = "CThreadPool" />
    /// </summary>
    public interface IThreadContext
    {
        /// <summary>
        ///   Run this instance.
        /// </summary>
        void Run();

        /// <summary>
        ///   Gets the GUID of the instance(class).
        /// </summary>
        /// <returns>
        ///   The GUID.
        /// </returns>
        Guid GetGuid();
    }
}