using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Alaris.Irc.Ctcp;
using Alaris.Irc.Dcc;
using Alaris.Irc.Delegates.Server;

#if SSL
using Org.Mentalis.Security.Ssl;
#endif

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

namespace Alaris.Irc
{
    /// <summary>
    ///   This class manages the connection to the IRC server and provides
    ///   access to all the objects needed to send and receive messages.
    /// </summary>
    [Serializable]
    public sealed class Connection : IDisposable
    {
        /// <summary>
        ///   Receive all the messages, unparsed, sent by the IRC server. This is not
        ///   normally needed but provided for those who are interested.
        /// </summary>
        public event RawMessageReceivedEventHandler OnRawMessageReceived;

        /// <summary>
        ///   Receive all the raw messages sent to the IRC from this connection
        /// </summary>
        public event RawMessageSentEventHandler OnRawMessageSent;


#if SSL
		private SecureTcpClient client;
#else
        [NonSerialized]
        private TcpClient _client;
#endif

        private readonly Regex _propertiesRegex;
        private readonly Listener _listener;
        private readonly Sender _sender;
        [NonSerialized]
        private CtcpListener _ctcpListener;
        [NonSerialized]
        private CtcpSender _ctcpSender;
        [NonSerialized]
        private CtcpResponder _ctcpResponder;
        private bool _ctcpEnabled;
        private bool _dccEnabled;
        [NonSerialized]
        private Thread _socketListenThread;
        [NonSerialized]
        private StreamReader _reader;
        private DateTime _timeLastSent;
        //Connected and registered with IRC server
        private bool _registered;
        //TCP/IP connection established with IRC server
        private bool _connected;
        private bool _handleNickFailure;
        private readonly ArrayList _parsers;
        [NonSerialized]
        private ServerProperties _properties;

        [NonSerialized]
        private StreamWriter _writer;
        internal ConnectionArgs ConnectionArgs;

        /// <summary>
        ///   Used for internal test purposes only.
        /// </summary>
        internal Connection(ConnectionArgs args)
        {
            ConnectionArgs = args;
            _sender = new Sender(this);
            _listener = new Listener();
            _timeLastSent = DateTime.Now;
            EnableCtcp = true;
            EnableDcc = true;
            TextEncoding = Encoding.Default;
        }

        /// <summary>
        ///   Prepare a connection to an IRC server but do not open it. This sets the text Encoding to Default.
        /// </summary>
        /// <param name = "args">The set of information need to connect to an IRC server</param>
        /// <param name = "enableCtcp">True if this Connection should support CTCP.</param>
        /// <param name = "enableDcc">True if this Connection should support DCC.</param>
        public Connection(ConnectionArgs args, bool enableCtcp, bool enableDcc)
        {
            _propertiesRegex = new Regex("([A-Z]+)=([^\\s]+)", RegexOptions.Compiled | RegexOptions.Singleline);
            _registered = false;
            _connected = false;
            _handleNickFailure = true;
            ConnectionArgs = args;
            _parsers = new ArrayList();
            _sender = new Sender(this);
            _listener = new Listener();

            RegisterDelegates();
            _timeLastSent = DateTime.Now;
            EnableCtcp = enableCtcp;
            EnableDcc = enableDcc;
            TextEncoding = Encoding.Default;
        }


        /// <summary>
        ///   Prepare a connection to an IRC server but do not open it.
        /// </summary>
        /// <param name = "args">The set of information need to connect to an IRC server</param>
        /// <param name = "enableCtcp">True if this Connection should support CTCP.</param>
        /// <param name = "enableDcc">True if this Connection should support DCC.</param>
        /// <param name = "textEncoding">The text encoding for the incoming stream.</param>
        public Connection(Encoding textEncoding, ConnectionArgs args, bool enableCtcp, bool enableDcc)
            : this(args, enableCtcp, enableDcc)
        {
            TextEncoding = textEncoding;
        }


        /// <summary>
        ///   Sets the text encoding used by the read and write streams.
        ///   Must be set before Connect() is called and should not be changed
        ///   while the connection is processing messages.
        /// </summary>
        /// <value>An Encoding constant.</value>
        public Encoding TextEncoding { get; set; }

        /// <summary>
        ///   A read-only property indicating whether the connection 
        ///   has been opened with the IRC server and the 
        ///   client has been successfully registered.
        /// </summary>
        /// <value>True if the client is connected and registered.</value>
        public bool Registered
        {
            get { return _registered; }
        }

        /// <summary>
        ///   A read-only property indicating whether a connection 
        ///   has been opened with the IRC server (but not whether 
        ///   registration has succeeded).
        /// </summary>
        /// <value>True if the client is connected.</value>
        public bool Connected
        {
            get { return _connected; }
        }

        /// <summary>
        ///   By default the connection itself will handle the case
        ///   where, while attempting to register the client's nick
        ///   is already in use. It does this by simply appending
        ///   2 random numbers to the end of the nick.
        /// </summary>
        /// <remarks>
        ///   The NickError event is shows that the nick collision has happened
        ///   and it is fixed by calling Sender's Register() method passing
        ///   in the replacement nickname.
        /// </remarks>
        /// <value>True if the connection should handle this case and
        ///   false if the client will handle it itself.</value>
        public bool HandleNickTaken
        {
            get { return _handleNickFailure; }
            set { _handleNickFailure = value; }
        }

        /// <summary>
        ///   A user friendly name of this Connection in the form 'nick@host'
        /// </summary>
        /// <value>Read only string</value>
        public string Name
        {
            get { return ConnectionArgs.Nick + "@" + ConnectionArgs.Hostname; }
        }

        /// <summary>
        ///   Whether Ctcp commands should be processed and if
        ///   Ctcp events will be raised.
        /// </summary>
        /// <value>True will enable the CTCP sender and listener and
        ///   false will cause their property calls to return null.</value>
        public bool EnableCtcp
        {
            get { return _ctcpEnabled; }
            set
            {
                if (value && !_ctcpEnabled)
                {
                    _ctcpListener = new CtcpListener(this);
                    _ctcpSender = new CtcpSender(this);
                }
                else if (!value)
                {
                    _ctcpListener = null;
                    _ctcpSender = null;
                }
                _ctcpEnabled = value;
            }
        }

        /// <summary>
        ///   Whether DCC requests should be processed or ignored 
        ///   by this Connection. Since the DccListener is a singleton and
        ///   shared by all Connections, listeners to DccListener events should
        ///   be manually removed when no longer needed.
        /// </summary>
        /// <value>True to process DCC requests.</value>
        public bool EnableDcc
        {
            get { return _dccEnabled; }
            set { _dccEnabled = value; }
        }

        /// <summary>
        ///   Sets an automatic responder to Ctcp queries.
        /// </summary>
        /// <value>Once this is set it can be removed by setting it to null.</value>
        public CtcpResponder CtcpResponder
        {
            get { return _ctcpResponder; }
            set
            {
                if (value == null && _ctcpResponder != null)
                {
                    _ctcpResponder.Disable();
                }
                _ctcpResponder = value;
            }
        }

        /// <summary>
        ///   The amount of time that has passed since the client
        ///   sent a command to the IRC server.
        /// </summary>
        /// <value>Read only TimeSpan</value>
        public TimeSpan IdleTime
        {
            get { return DateTime.Now - _timeLastSent; }
        }

        /// <summary>
        ///   The object used to send commands to the IRC server.
        /// </summary>
        /// <value>Read-only Sender.</value>
        public Sender Sender
        {
            get { return _sender; }
        }

        /// <summary>
        ///   The object that parses messages and notifies the appropriate delegate.
        /// </summary>
        /// <value>Read only Listener.</value>
        public Listener Listener
        {
            get { return _listener; }
        }

        /// <summary>
        ///   The object used to send CTCP commands to the IRC server.
        /// </summary>
        /// <value>Read only CtcpSender. Null if CtcpEnabled is false.</value>
        public CtcpSender CtcpSender
        {
            get { return _ctcpSender; }
        }

        /// <summary>
        ///   The object that parses CTCP messages and notifies the appropriate delegate.
        /// </summary>
        /// <value>Read only CtcpListener. Null if CtcpEnabled is false.</value>
        public CtcpListener CtcpListener
        {
            get { return _ctcpEnabled ? _ctcpListener : null; }
        }

        /// <summary>
        ///   The collection of data used to establish this connection.
        /// </summary>
        /// <value>Read only ConnectionArgs.</value>
        public ConnectionArgs ConnectionData
        {
            get { return ConnectionArgs; }
        }

        /// <summary>
        ///   A read-only collection of string key/value pairs
        ///   representing IRC server proprties.
        /// </summary>
        /// <value>This connection's ServerProperties obejct or null if it 
        ///   has not been created.</value>
        public ServerProperties ServerProperties
        {
            get { return _properties; }
        }

        private bool CustomParse(string line)
        {
            foreach (IParser parser in _parsers)
            {
                if (parser.CanParse(line))
                {
                    parser.Parse(line);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///   Respond to IRC keep-alives.
        /// </summary>
        /// <param name = "message">The message that should be echoed back</param>
        private void KeepAlive(string message)
        {
            _sender.Pong(message);
        }

        /// <summary>
        ///   Update the ConnectionArgs object when the user
        ///   changes his nick.
        /// </summary>
        /// <param name = "user">Who changed their nick</param>
        /// <param name = "newNick">The new nick name</param>
        private void MyNickChanged(UserInfo user, string newNick)
        {
            if (ConnectionArgs.Nick == user.Nick)
            {
                ConnectionArgs.Nick = newNick;
            }
        }

        private void OnRegistered()
        {
            _registered = true;
            _listener.OnRegistered -= OnRegistered;
        }

        /// <summary>
        /// </summary>
        /// <param name = "badNick"></param>
        /// <param name = "reason"></param>
        private void OnNickError(string badNick, string reason)
        {
            //If this is our initial connection attempt
            if (!_registered && _handleNickFailure)
            {
                var generator = new NameGenerator();
                string nick;
                do
                {
                    nick = generator.MakeName();
                } while (!Rfc2812Util.IsValidNick(nick) || nick.Length == 1);
                //Try to reconnect
                Sender.Register(nick);
            }
        }

        /// <summary>
        ///   Listen for the 005 info messages sent during registration so that the maximum lengths
        ///   of certain items (Nick, Away, Topic) can be determined dynamically.
        /// </summary>
        /// <param name = "code">Reply code enum</param>
        /// <param name = "info">An info line</param>
        private void OnReply(ReplyCode code, string info)
        {
            if (code == ReplyCode.RPL_BOUNCE) //Code 005
            {
                //Lazy instantiation
                if (_properties == null)
                {
                    _properties = new ServerProperties();
                }
                //Populate properties from name/value matches
                MatchCollection matches = _propertiesRegex.Matches(info);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        _properties.SetProperty(match.Groups[1].ToString(), match.Groups[2].ToString());
                    }
                }
                //Extract ones we are interested in
                ExtractProperties();
            }
        }

        private static void ExtractProperties()
        {
            //For the moment the only one we care about is NickLen
            //In fact we don't cae about any but keep here as an example
            /*
			if( properties.ContainsKey("NICKLEN") ) 
			{
				try 
				{
					maxNickLength = int.Parse( properties[ "NICKLEN" ] );
				}
				catch( Exception e )
				{
				}
			}
			*/
        }

        private void RegisterDelegates()
        {
            _listener.OnPing += KeepAlive;
            _listener.OnNick += MyNickChanged;
            _listener.OnNickError += OnNickError;
            _listener.OnReply += OnReply;
            _listener.OnRegistered += OnRegistered;
        }

#if SSL
		private void ConnectClient( SecureProtocol protocol )   
		{
			lock ( this ) 
			{
				if( connected ) 
				{
					throw new Exception("Connection with IRC server already opened.");
				}
				Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"[" + Thread.CurrentThread.Name +"] Connection::Connect()");
			
					SecurityOptions options = new SecurityOptions( protocol );
					options.Certificate = null;
					options.Entity = ConnectionEnd.Client;
					options.VerificationType = CredentialVerification.None;
					options.Flags = SecurityFlags.Default;
					options.AllowedAlgorithms = SslAlgorithms.SECURE_CIPHERS;
					client = new SecureTcpClient( options );		
					client.Connect( connectionArgs.Hostname, connectionArgs.Port );
			
				connected = true;
				writer = new StreamWriter( client.GetStream(), TextEncoding );
				writer.AutoFlush = true;
				reader = new StreamReader(  client.GetStream(), TextEncoding );
				socketListenThread = new Thread(new ThreadStart( ReceiveIRCMessages ) );
				socketListenThread.Name = Name;
				socketListenThread.Start();		
				sender.RegisterConnection( connectionArgs );
			}
		}
#endif

        /// <summary>
        ///   Read in message lines from the IRC server
        ///   and send them to a parser for processing.
        ///   Discard CTCP and DCC messages if these protocols
        ///   are not enabled.
        /// </summary>
        internal void ReceiveIRCMessages()
        {
            Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] Connection::ReceiveIRCMessages()");
            string line;
            try
            {
                while ((line = _reader.ReadLine()) != null)
                {
                    try
                    {
                        Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceVerbose,
                                          "[" + Thread.CurrentThread.Name + "] Connection::ReceiveIRCMessages() rec'd:" +
                                          line);
                        //Try any custom parsers first
                        if (CustomParse(line))
                        {
                            //One of the custom parsers handled this message so
                            //we go back to listening
                            continue;
                        }
                        if (DccListener.IsDccRequest(line))
                        {
                            if (_dccEnabled)
                            {
                                DccListener.DefaultInstance.Parse(this, line);
                            }
                        }
                        else if (CtcpListener.IsCtcpMessage(line))
                        {
                            if (_ctcpEnabled)
                            {
                                _ctcpListener.Parse(line);
                            }
                        }
                        else
                        {
                            _listener.Parse(line);
                        }
                        if (OnRawMessageReceived != null)
                        {
                            OnRawMessageReceived(line);
                        }
                    }
                    catch (ThreadAbortException e)
                    {
                        Thread.ResetAbort();
                        //This exception is raised when the Thread
                        //is stopped at user request, i.e. via Disconnect()
                        //This will stop the read loop and close the connection.
                        break;
                    }
                }
            }
            catch (IOException e)
            {
                //Trap a connection failure
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name +
                                  "] Connection::ReceiveIRCMessages() IO Error while listening for messages " + e);
                _listener.FireError(ReplyCode.ConnectionFailed, "Connection to server unexpectedly failed.");
            }
            //The connection to the IRC server has been closed either
            //by client request or the server itself closed the connection.
            _client.Close();
            _registered = false;
            _connected = false;
            _listener.Disconnected();
        }

        /// <summary>
        ///   Send a message to the IRC server and clear the command buffer.
        /// </summary>
        internal void SendCommand(StringBuilder command)
        {
            try
            {
                _writer.WriteLine(command.ToString());
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceVerbose,
                                  "[" + Thread.CurrentThread.Name + "] Connection::SendCommand() sent= " + command);
                _timeLastSent = DateTime.Now;
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] Connection::SendCommand() exception=" + e);
            }
            if (OnRawMessageSent != null)
            {
                OnRawMessageSent(command.ToString());
            }
            command.Remove(0, command.Length);
        }

        /// <summary>
        ///   Send a message to the IRC server which does
        ///   not affect the client's idle time. Used for automatic replies
        ///   such as PONG or Ctcp repsones.
        /// </summary>
        internal void SendAutomaticReply(StringBuilder command)
        {
            try
            {
                _writer.WriteLine(command.ToString());
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceVerbose,
                                  "[" + Thread.CurrentThread.Name + "] Connection::SendAutomaticReply() message=" +
                                  command);
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] Connection::SendAutomaticReply() exception=" + e);
            }
            command.Remove(0, command.Length);
        }


#if SSL
    ///<summary>
    /// Connect to the IRC server and start listening for messages
    /// on a new thread.
    /// </summary>
    /// <exception cref="SocketException">If a connection cannot be established with the IRC server</exception>
		public void Connect() 
		{
			Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"Connecting over clear socket");
			ConnectClient( SecureProtocol.None );
		}

		///<summary>
		/// Connect to the IRC server over an encrypted connection using TLS.
		/// </summary>
		/// <exception cref="SocketException">If a connection cannot be established with the IRC server</exception>
		public void SecureConnect() 
		{
			Debug.WriteLineIf( Rfc2812Util.IrcTrace.TraceInfo,"Connecting over encrypted socket");
			ConnectClient( SecureProtocol.Tls1 );
		}


#else
        /// <summary>
        ///   Connect to the IRC server and start listening for messages
        ///   on a new thread.
        /// </summary>
        /// <exception cref = "SocketException">If a connection cannot be established with the IRC server</exception>
        public void Connect()
        {
            lock (this)
            {
                if (_connected)
                {
                    throw new InvalidOperationException("Connection with IRC server already opened.");
                }
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceInfo,
                                  "[" + Thread.CurrentThread.Name + "] Connection::Connect()");
                _client = new TcpClient();

                _client.Connect(ConnectionArgs.Hostname, ConnectionArgs.Port);
                _connected = true;
                _writer = new StreamWriter(_client.GetStream(), TextEncoding) {AutoFlush = true};
                _reader = new StreamReader(_client.GetStream(), TextEncoding);
                _socketListenThread = new Thread(ReceiveIRCMessages);
                _socketListenThread.Name = Name;
                _socketListenThread.Start();
                _sender.RegisterConnection(ConnectionArgs);
            }
        }
#endif

        /// <summary>
        ///   Sends a 'Quit' message to the server, closes the connection,
        ///   and stops the listening thread.
        /// </summary>
        /// <remarks>
        ///   The state of the connection will remain the same even after a disconnect,
        ///   so the connection can be reopened. All the event handlers will remain registered.
        /// </remarks>
        /// <param name = "reason">A message displayed to IRC users upon disconnect.</param>
        public void Disconnect(string reason)
        {
            lock (this)
            {
                if (!_connected)
                {
                    throw new InvalidOperationException("Not connected to IRC server.");
                }
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceInfo,
                                  "[" + Thread.CurrentThread.Name + "] Connection::Disconnect()");
                _listener.Disconnecting();
                _sender.Quit(reason);
                _listener.Disconnected();
                //Thanks to Thomas for this next block
                if (_socketListenThread.Join(TimeSpan.FromSeconds(1)) == false)
                    _socketListenThread.Abort();
            }
        }

        /// <summary>
        ///   A friendly name for this connection.
        /// </summary>
        /// <returns>The Name property</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        ///   Adds a parser class to a list of custom parsers. 
        ///   Any number can be added. The custom parsers
        ///   will be tested using <c>CanParse()</c> before
        ///   the default parsers. The last parser to be added
        ///   will be the first to process a message.
        /// </summary>
        /// <param name = "parser">Any class that implements IParser.</param>
        public void AddParser(IParser parser)
        {
            _parsers.Insert(0, parser);
        }

        /// <summary>
        ///   Remove a custom parser class.
        /// </summary>
        /// <param name = "parser">Any class that implements IParser.</param>
        public void RemoveParser(IParser parser)
        {
            _parsers.Remove(parser);
        }


        public void Dispose()
        {
            _client.Close();
            _writer.Dispose();
            _reader.Dispose();
        }
    }
}