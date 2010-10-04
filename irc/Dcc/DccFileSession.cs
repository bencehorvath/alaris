using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Alaris.Irc.Delegates.Dcc.Files;

namespace Alaris.Irc.Dcc
{
    /// <summary>
    ///   Allows the user to send and receive files
    ///   from other IRC users.
    /// </summary>
    public sealed class DccFileSession
    {
        /// <summary>
        ///   The remote user did not accept the file within the timeout period.
        /// </summary>
        public event FileTransferTimeoutEventHandler OnFileTransferTimeout;

        /// <summary>
        ///   The file transfer connection is open and data will be sent or
        ///   received.
        /// </summary>
        public event FileTransferStartedEventHandler OnFileTransferStarted;

        /// <summary>
        ///   The file transfer was interrupted and did not complete.
        /// </summary>
        public event FileTransferInterruptedEventHandler OnFileTransferInterrupted;

        /// <summary>
        ///   The file transfer was successful.
        /// </summary>
        public event FileTransferCompletedEventHandler OnFileTransferCompleted;

        /// <summary>
        ///   How much of the file has been sent or received so far.
        /// </summary>
        public event FileTransferProgressEventHandler OnFileTransferProgress;

        //Does this session use send-ahead mode
        private bool _turboMode;
        //The last time any data was received or sent successfully
        //used to test for a timeout.
        private DateTime _lastActivity;
        //Signals whether the session is waiting for an Accept message 
        //in reponse to a Resume request.
        private bool _waitingOnAccept;
        private readonly DccUserInfo _dccUserInfo;
        private readonly byte[] _buffer;
        private readonly int _listenPort;
        private readonly string _sessionID;
        private string _listenIpAddress;
        private Socket _socket;
        private Socket _serverSocket;
        private Thread _thread;

        private readonly DccFileInfo _dccFileInfo;

        /// <summary>
        ///   Prepare a new instance with default values but do not connect
        ///   to another user.
        /// </summary>
        internal DccFileSession(DccUserInfo dccUserInfo, DccFileInfo dccFileInfo, int bufferSize, int listenPort,
                                string sessionID)
        {
            _dccUserInfo = dccUserInfo;
            _dccFileInfo = dccFileInfo;
            _buffer = new byte[bufferSize];
            _listenPort = listenPort;
            _sessionID = sessionID;
            _lastActivity = DateTime.Now;
            _waitingOnAccept = false;
        }

        internal DateTime LastActivity
        {
            get { return _lastActivity; }
        }

        /// <summary>
        ///   A unique identifier for this session.
        /// </summary>
        /// <value>Uses the TCP/IP port prefixed by an 'S' if this
        ///   session is serving the file or a 'C' if this session is receiving the
        ///   file.</value>
        public string ID
        {
            get { return _sessionID; }
        }

        /// <summary>
        ///   The DccUserInfo object associated with this DccFileSession.
        /// </summary>
        public DccUserInfo User
        {
            get { return _dccUserInfo; }
        }

        /// <summary>
        ///   The DccFileInfo object associated with this DccFileSession.
        /// </summary>
        public DccFileInfo File
        {
            get { return _dccFileInfo; }
        }

        /// <summary>
        ///   The information about the remote user.
        /// </summary>
        /// <value>A read only instance of DccUserInfo.</value>
        public DccUserInfo ClientInfo
        {
            get { return _dccUserInfo; }
        }

        private void SendAccept()
        {
            var builder = new StringBuilder("PRIVMSG ", 512);
            builder.Append(_dccUserInfo.Nick);
            builder.Append(" :\x0001DCC ACCEPT ");
            builder.Append(_dccFileInfo.DccFileName);
            builder.Append(" ");
            builder.Append(_listenPort);
            builder.Append(" ");
            builder.Append(_dccFileInfo.FileStartingPosition);
            builder.Append("\x0001\n");
            _dccUserInfo.Connection.Sender.Raw(builder.ToString());
        }

        private void DccSend(IPAddress sendAddress)
        {
            var builder = new StringBuilder("PRIVMSG ", 512);
            builder.Append(_dccUserInfo.Nick);
            builder.Append(" :\x0001DCC SEND ");
            builder.Append(_dccFileInfo.DccFileName);
            builder.Append(" ");
            builder.Append(DccUtil.IPAddressToLong(sendAddress));
            builder.Append(" ");
            builder.Append(_listenPort);
            builder.Append(" ");
            builder.Append(_dccFileInfo.CompleteFileSize);
            builder.Append(_turboMode ? " T" : "");
            builder.Append("\x0001\n");
            _dccUserInfo.Connection.Sender.Raw(builder.ToString());
        }

        private void SendResume()
        {
            var builder = new StringBuilder("PRIVMSG ", 512);
            builder.Append(_dccUserInfo.Nick);
            builder.Append(" :\x0001DCC RESUME ");
            builder.Append(_dccFileInfo.DccFileName);
            builder.Append(" ");
            builder.Append(_listenPort);
            builder.Append(" ");
            builder.Append(_dccFileInfo.FileStartingPosition);
            builder.Append("\x0001\n");
            _dccUserInfo.Connection.Sender.Raw(builder.ToString());
        }

        /// <summary>
        ///   Attempt to shut the session down correctly.
        /// </summary>
        private void Cleanup()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Cleanup()");
            DccFileSessionManager.DefaultInstance.RemoveSession(this);
            if (_serverSocket != null)
            {
                _serverSocket.Close();
            }
            if (_socket != null)
            {
                try
                {
                    _socket.Close();
                }
                catch (Exception e)
                {
                    //Ignore this exception
                }
            }
            _dccFileInfo.CloseFile();
        }

        private void ResetActivityTimer()
        {
            _lastActivity = DateTime.Now;
        }

        private void SignalTransferStart()
        {
            ResetActivityTimer();
            if (OnFileTransferStarted != null)
            {
                OnFileTransferStarted(this);
            }
        }

        private void Listen()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name + "] DccFileSession::Listen()");
            try
            {
                //Wait for remote client to connect
                var localEndPoint = new IPEndPoint(DccUtil.LocalHost(), _listenPort);
                _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _serverSocket.Bind(localEndPoint);
                _serverSocket.Listen(1);
                //Got one!
                _socket = _serverSocket.Accept();
                _serverSocket.Close();
                Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                                  "[" + Thread.CurrentThread.Name + "] DccFileSession::Listen() Remote user connected.");
                //Advance to the correct point in the file in case this is a resume 
                _dccFileInfo.GotoReadPosition();
                SignalTransferStart();
                if (_turboMode)
                {
                    Upload();
                }
                else
                {
                    UploadLegacy();
                }
            }
            catch (Exception se)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] DccFileSession::Listen() Connection broken");
                Interrupted();
            }
        }

        private void Upload()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Upload()" +
                              (_turboMode ? " Turbo" : " Legacy") + " mode");
            try
            {
                int bytesRead;
                var ack = new byte[4];
                while ((bytesRead = _dccFileInfo.TransferStream.Read(_buffer, 0, _buffer.Length)) != 0)
                {
                    _socket.Send(_buffer, 0, bytesRead, SocketFlags.None);
                    ResetActivityTimer();
                    AddBytesProcessed(bytesRead);
                }
                //Now we are done
                Finished();
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] DccFileSession::Upload() exception=" + e);
                Interrupted();
            }
        }

        private void UploadLegacy()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::UploadLegacy()");
            try
            {
                int bytesRead;
                var ack = new byte[4];
                while ((bytesRead = _dccFileInfo.TransferStream.Read(_buffer, 0, _buffer.Length)) != 0)
                {
                    _socket.Send(_buffer, 0, bytesRead, SocketFlags.None);
                    ResetActivityTimer();
                    AddBytesProcessed(bytesRead);
                    //Wait for acks from client
                    _socket.Receive(ack);
                }
                //Some IRC clients need a moment to catch up on their acks if our send buffer
                //is larger than their receive buffer. Test to make sure they ack all the bytes
                //before closing. This is only needed in legacy mode.
                while (!_dccFileInfo.AcksFinished(DccUtil.DccBytesToLong(ack)))
                {
                    _socket.Receive(ack);
                }
                //Now we are done
                Finished();
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] DccFileSession::UploadLegacy() exception=" + e);
                Interrupted();
            }
        }

        private void Download()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Download()" +
                              (_turboMode ? " Turbo" : " Legacy") + " mode");
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(_dccUserInfo.RemoteEndPoint);
                while (!_dccFileInfo.AllBytesTransfered())
                {
                    int bytesRead = _socket.Receive(_buffer);
                    //Remote server closed the connection before all bytes were sent
                    if (bytesRead == 0)
                    {
                        Interrupted();
                        return;
                    }
                    ResetActivityTimer();
                    AddBytesProcessed(bytesRead);
                    _dccFileInfo.TransferStream.Write(_buffer, 0, bytesRead);
                    //Send ack if in legacy mode
                    if (!_turboMode)
                    {
                        _socket.Send(DccUtil.DccBytesReceivedFormat(_dccFileInfo.CurrentFilePosition()));
                    }
                }
                _dccFileInfo.TransferStream.Flush();
                Finished();
            }
            catch (Exception e)
            {
                Debug.WriteLineIf(Rfc2812Util.IrcTrace.TraceWarning,
                                  "[" + Thread.CurrentThread.Name + "] DccFileSession::Download() exception=" + e);
                if (e.Message.IndexOf("refused") > 0)
                {
                    _dccUserInfo.Connection.Listener.FireError(ReplyCode.DccConnectionRefused,
                                                           "Connection refused by remote user.");
                }
                else
                {
                    _dccUserInfo.Connection.Listener.FireError(ReplyCode.ConnectionFailed,
                                                           "Unknown socket error:" + e.Message);
                }
                Interrupted();
            }
        }

        internal void AddBytesProcessed(int bytesRead)
        {
            _dccFileInfo.AddBytesTransfered(bytesRead);
            if (OnFileTransferProgress != null)
            {
                OnFileTransferProgress(this, bytesRead);
            }
        }

        /// <summary>
        ///   Called by DccListener when it receives a DCC Accept message.
        /// </summary>
        internal void OnDccAcceptReceived(long position)
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::OnDccAcceptReceived()");
            lock (this)
            {
                //Are we still waiting on the accept?
                if (!_waitingOnAccept)
                {
                    //Assume that a normal receive has gone ahead
                    return;
                }
                //No longer waiting
                _waitingOnAccept = false;
                if (!_dccFileInfo.AcceptPositionMatches(position))
                {
                    _dccUserInfo.Connection.Listener.FireError(ReplyCode.BadDccAcceptValue,
                                                           "Asked to start at " + _dccFileInfo.FileStartingPosition +
                                                           " but was sent " + position);
                    Interrupted();
                    return;
                }
                ResetActivityTimer();
                _dccFileInfo.SetResumeToFileSize();
                _dccFileInfo.GotoWritePosition();
                _thread = new Thread(Download);
                _thread.Name = ToString();
                _thread.Start();
            }
        }

        /// <summary>
        ///   A DCC Send request has already been sent and the remote user 
        ///   has responded with a Resume request.
        /// </summary>
        /// <param name = "resumePosition">The number of bytes the remote user already has..</param>
        /// <exception cref = "ArgumentException">If the session is no longer active or the file 
        ///   resume position was larger than the file.</exception>
        internal void OnDccResumeRequest(long resumePosition)
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::OnDccResumeRequest()");
            lock (this)
            {
                ResetActivityTimer();
                //Make sure we have not already started transfering data and that this file is
                //resumeable.
                if (_dccFileInfo.BytesTransfered == 0 && _dccFileInfo.CanResume())
                {
                    //Make sure the position is valid
                    if (_dccFileInfo.ResumePositionValid(resumePosition))
                    {
                        _dccFileInfo.SetResumePosition(resumePosition);
                        SendAccept();
                    }
                    else
                    {
                        _dccUserInfo.Connection.Listener.FireError(ReplyCode.BadResumePosition,
                                                               ToString() + " sent an invalid resume position.");
                        //Close the socket and stop listening
                        Cleanup();
                    }
                }
            }
        }

        /// <summary>
        ///   Called when there has been no activity is
        ///   a session for the the length of the timeout period.
        /// </summary>
        internal void TimedOut()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo, ToString() + " timed out.");
            if (_waitingOnAccept)
            {
                _waitingOnAccept = false;
                //Start a new thread to download the whole file
                _thread = new Thread(Download);
                _thread.Name = ToString();
                _thread.Start();
            }
            else
            {
                if (OnFileTransferTimeout != null)
                {
                    OnFileTransferTimeout(this);
                }
                Cleanup();
            }
        }

        /// <summary>
        ///   Non synchro version of Stop() for internal
        ///   use.
        /// </summary>
        internal void Interrupted()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Interrupted()");
            Cleanup();
            if (OnFileTransferInterrupted != null)
            {
                OnFileTransferInterrupted(this);
            }
        }

        /// <summary>
        ///   The file transfer is done. So close everything
        ///   cleanly.
        /// </summary>
        internal void Finished()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Finished()");
            Cleanup();
            if (OnFileTransferCompleted != null)
            {
                OnFileTransferCompleted(this);
            }
        }

        /// <summary>
        ///   Stop the file transfer.
        /// </summary>
        public void Stop()
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name + "] DccFileSession::Stop()");
            lock (this)
            {
                Cleanup();
                if (OnFileTransferInterrupted != null)
                {
                    OnFileTransferInterrupted(this);
                }
            }
        }

        /// <summary>
        ///   Summary information about this session.
        /// </summary>
        /// <returns>Simple information about this session in human readable format.</returns>
        public override string ToString()
        {
            return "DccFileSession:: ID=" + _sessionID + " User=" + _dccUserInfo + " File=" + _dccFileInfo.DccFileName;
        }

        /// <summary>
        ///   Ask a remote user to send a file. The remote user may or may not respond
        ///   and there is no fixed time within which he must respond. A response will
        ///   come in the form of a DCC Send request.
        /// </summary>
        /// <param name = "connection">The connection the remotes user is on.</param>
        /// <param name = "nick">Who to send the request to.</param>
        /// <param name = "fileName">The name of the file to have sent. This should
        ///   not contain any spaces.</param>
        /// <param name = "turbo">True to use send-ahead mode for transfers.</param>
        public static void Get(Connection connection, string nick, string fileName, bool turbo)
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name + "] DccFileSession::Get()");
            var builder = new StringBuilder("PRIVMSG ", 512);
            builder.Append(nick);
            builder.Append(" :\x0001DCC GET ");
            builder.Append(fileName);
            builder.Append(turbo ? " T" : "");
            builder.Append("\x0001\n");
            connection.Sender.Raw(builder.ToString());
        }

        /// <summary>
        ///   Attempt to send a file to a remote user. Start listening
        ///   on the given port and address. If the remote user does not accept
        ///   the offer within the timeout period the the session
        ///   will be closed.
        /// </summary>
        /// <remarks>
        ///   This method should be called from within a try/catch block 
        ///   in case there are socket errors. This methods will also automatically 
        ///   handle a Resume if the remote client requests it.
        /// </remarks>
        /// <param name = "dccUserInfo">The information about the remote user.</param>
        /// <param name = "listenIPAddress">The IP address of the local machine in dot 
        ///   quad format (e.g. 192.168.0.25). This is the address that will be sent to the 
        ///   remote user. The IP address of the NAT machine must be used if the
        ///   client is behind a NAT/Firewall system. </param>
        /// <param name = "listenPort">The port that the session will listen on.</param>
        /// <param name = "dccFileInfo">The file to be sent. If the file name has spaces in it
        ///   they will be replaced with underscores when the name is sent.</param>
        /// <param name = "bufferSize">The size of the send buffer. Generally should
        ///   be between 4k and 32k.</param>
        /// <param name = "turbo">True to use send-ahead mode for transfers.</param>
        /// <returns>A unique session instance for this file and remote user.</returns>
        /// <exception cref = "ArgumentException">If the listen port is already in use.</exception>
        public static DccFileSession Send(
            DccUserInfo dccUserInfo,
            string listenIPAddress,
            int listenPort,
            DccFileInfo dccFileInfo,
            int bufferSize,
            bool turbo)
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo, "[" + Thread.CurrentThread.Name + "] DccFileSession::Send()");
            DccFileSession session = null;
            //Test if we are already using this port
            if (DccFileSessionManager.DefaultInstance.ContainsSession("S" + listenPort))
            {
                throw new ArgumentException("Already listening on port " + listenPort);
            }
            try
            {
                session = new DccFileSession(dccUserInfo, dccFileInfo, bufferSize, listenPort, "S" + listenPort)
                              {
                                  _turboMode = turbo,
                                  _listenIpAddress = listenIPAddress
                              };
                //set turbo mode
                //Set server IP address
                //Add session to active sessions hashtable
                DccFileSessionManager.DefaultInstance.AddSession(session);
                //Create stream to file
                dccFileInfo.OpenForRead();
                //Start session Thread
                session._thread = new Thread(session.Listen) {Name = session.ToString()};
                session._thread.Start();
                //Send DCC Send request to remote user
                session.DccSend(IPAddress.Parse(listenIPAddress));
                return session;
            }
            catch (Exception e)
            {
                if (session != null)
                {
                    DccFileSessionManager.DefaultInstance.RemoveSession(session);
                }
                throw;
            }
        }

        /// <summary>
        ///   Another user has offered to send a file. This method should be called
        ///   to accept the offer and save the file to the give location. The parameters
        ///   needed to call this method are provided by the <c>OnDccFileTransferRequest()</c>
        ///   event.
        /// </summary>
        /// <remarks>
        ///   This method should be called from within a try/catch block 
        ///   in case it is unable to connect or there are other socket
        ///   errors.
        /// </remarks>
        /// <param name = "dccUserInfo">Information on the remote user.</param>
        /// <param name = "dccFileInfo">The local file that will hold the data being sent. If the file 
        ///   is the result of a previous incomplete download the the attempt will be made
        ///   to resume where the previous left off.</param>
        /// <param name = "turbo">Will the send ahead protocol be used.</param>
        /// <returns>A unique session instance for this file and remote user.</returns>
        /// <exception cref = "ArgumentException">If the listen port is already in use.</exception>
        public static DccFileSession Receive(DccUserInfo dccUserInfo, DccFileInfo dccFileInfo, bool turbo)
        {
            if (dccUserInfo == null) throw new ArgumentNullException("dccUserInfo");
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccFileSession::Receive()");
            //Test if we are already using this port
            if (DccFileSessionManager.DefaultInstance.ContainsSession("C" + dccUserInfo.remoteEndPoint.Port))
            {
                throw new ArgumentException("Already listening on port " + dccUserInfo.remoteEndPoint.Port);
            }
            DccFileSession session = null;
            try
            {
                session = new DccFileSession(dccUserInfo, dccFileInfo, (64*1024),
                                             dccUserInfo.remoteEndPoint.Port, "C" + dccUserInfo.remoteEndPoint.Port)
                              {_turboMode = turbo};
                //Has the initiator specified the turbo protocol? 
                //Open file for writing
                dccFileInfo.OpenForWrite();
                DccFileSessionManager.DefaultInstance.AddSession(session);
                //Determine if we can resume a download
                if (session._dccFileInfo.ShouldResume())
                {
                    session._waitingOnAccept = true;
                    session._dccFileInfo.SetResumeToFileSize();
                    session.SendResume();
                }
                else
                {
                    session._thread = new Thread(session.Download) {Name = session.ToString()};
                    session._thread.Start();
                }
                return session;
            }
            catch (Exception e)
            {
                if (session != null)
                {
                    DccFileSessionManager.DefaultInstance.RemoveSession(session);
                }
                throw;
            }
        }
    }
}