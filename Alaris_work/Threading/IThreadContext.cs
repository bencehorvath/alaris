using System;
using Alaris.API;

namespace Alaris.Threading
{
    /// <summary>
    ///   The interface used to define classes that can be run in the <see cref = "CThreadPool" />
    /// </summary>
    public interface IThreadContext : IAlarisComponent
    {
        /// <summary>
        ///   Run this instance.
        /// </summary>
        void Run();

     
    }
}