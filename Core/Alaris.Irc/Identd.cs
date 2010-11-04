using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using NLog;


namespace Alaris.Irc
{
	/// <summary>
	/// An Ident daemon is still used by some IRC networks for 
	/// authentication. It is a simple service which when queried
	/// by a remote system returns a username. The server is controlled via static
	/// methods all of which are Thread safe.
	/// </summary>
	public static class Identd
	{
		private static TcpListener _listener;
		private static bool _running; 
		private static readonly object LockObject;
		private static string _username;
		private const string Reply = " : USERID : UNIX : ";
		private const int IdentdPort = 113;

	    private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		static Identd() 
		{
			_running = false;
			LockObject = new object();
		}

		//Declare constructor private so it cannot be instatiated.

	    /// <summary>
		/// The Identd server will start listening for queries
		/// in its own thread. It can be stopped by calling
		/// <see cref="Identd.Stop"/>.
		/// </summary>
		/// <param name="userName">Should be the same username as the one used
		/// in the ConnectionArgs object when establishing a connection.</param>
		/// <exception cref="Exception">If the server has already been started.</exception>
		public static void Start( string userName ) 
		{
			lock( LockObject ) 
			{
				if( _running ) 
				{
					throw new Exception("Identd already started.");
				}

                
				_running = true;
				_username = userName;
				var socketThread = new Thread(Run) {Name = "Identd"};
			    socketThread.Start();

                Log.Info("Identd Service daemon running.");
                
			}
		}
		/// <summary>
		/// Check if the Identd server is running
		/// </summary>
		/// <returns>True if it is running</returns>
		public static bool IsRunning() 
		{
			lock( LockObject ) 
			{
				return _running;
			}
		}
		/// <summary>
		/// Stop the Identd server and close the thread.
		/// </summary>
		public static void Stop() 
		{
			lock( LockObject ) 
			{
				if( _running ) 
				{
					_listener.Stop();
					Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Stop()");
					_listener = null;
					_running = false;
				}
			}
		}

		private static void Run() 
		{
			Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Run()");
			try 
			{
				_listener = new TcpListener( IdentdPort );
				_listener.Start();

				loop:
				{
					try 
					{
						TcpClient client = _listener.AcceptTcpClient();
						//Read query
						var reader =  new StreamReader(client.GetStream() );
						string line = reader.ReadLine();
						Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceVerbose,"[" + Thread.CurrentThread.Name +"] Identd::Run() received=" + line);

						//Send back reply
						var writer = new StreamWriter( client.GetStream() );
						writer.WriteLine( line.Trim() + Reply + _username );
						writer.Flush();

						//Close connection with client
						client.Close();
					}
					catch( IOException ioe ) 
					{
						Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceWarning,"[" + Thread.CurrentThread.Name +"] Identd::Run() exception=" + ioe);
					}
					goto loop;
				}
			}
			catch( Exception e ) 
			{
				Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Identd::Run() Identd stopped");
			}
			finally 
			{
				_running = false;
			}
		}

	}
}
