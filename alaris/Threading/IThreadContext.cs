using System;
using Alaris.Threading;

namespace Alaris
{
	/// <summary>
	/// The interface used to define classes that can be run in the <see cref="CThreadPool"/>
	/// </summary>
	public interface IThreadContext
	{
		/// <summary>
		/// Run this instance.
		/// </summary>
		void Run();
		/// <summary>
		/// Gets the GUID of the instance(class).
		/// </summary>
		/// <returns>
		/// The GUID.
		/// </returns>
		Guid GetGuid();
	}
	
	/// <summary>
	/// Specifies a delegate which can be run by the ThreadPool directly.
	/// </summary>
	public delegate void IThreadRunnable();
}

