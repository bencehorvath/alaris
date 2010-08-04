using System;

namespace Alaris.Extras
{
	/// <summary>
	/// Exception thrown when an invalid config file is detected.
	/// </summary>
	public class ConfigFileInvalidException : Exception
	{
		/// <summary>
		/// Creates a new instance of <see cref="ConfigFileInvalidException"/>
		/// </summary>
		public ConfigFileInvalidException() : base()
		{}
		
		/// <summary>
		/// Creates a new instance of <see cref="ConfigFileInvalidException"/>
		/// </summary>
		/// <param name="msg">
		/// The message of the exception.
		/// </param>
		public ConfigFileInvalidException(string msg) : base(msg)
		{}
	}
}