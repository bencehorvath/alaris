using System;

namespace Alaris.Exceptions
{
	/// <summary>
	/// Exception thrown when an invalid config file is detected.
	/// </summary>
	[Serializable]
	public abstract class ConfigFileInvalidException : Exception
	{
		/// <summary>
		/// Creates a new instance of <see cref="ConfigFileInvalidException"/>
		/// </summary>
		protected ConfigFileInvalidException() : base()
		{}
		
		/// <summary>
		/// Creates a new instance of <see cref="ConfigFileInvalidException"/>
		/// </summary>
		/// <param name="msg">
		/// The message of the exception.
		/// </param>
		protected ConfigFileInvalidException(string msg) : base(msg)
		{}
	}
}