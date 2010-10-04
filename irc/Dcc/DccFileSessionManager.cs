using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;

namespace Alaris.Irc.Dcc
{
    /// <summary>
    ///   This class checks each file session to see if it has not 
    ///   had any activity within the timeout period so that
    ///   inactive sessions can be closed.
    /// </summary>
    public sealed class DccFileSessionManager
    {
        //How long to wait
        private TimeSpan _timeout;
        //A clone of the session hashtable to iterate over
        private Hashtable _sessionClone;
        //A place to store the active sessions
        private readonly Hashtable _sessions;
        //Check for timeouts every 10 seconds
        private const int TimeoutCheckPeriod = 10000;
        //Default to tming out after 30 seconds of no activity.
        private const int DefaultTimeout = 30000;
        private static DccFileSessionManager _defaultInstance;
        private static readonly object LockObject = new object();
        private readonly Timer _timerThread;
        private bool _timerStopped;

        private DccFileSessionManager()
        {
            _timeout = new TimeSpan(DefaultTimeout*TimeSpan.TicksPerMillisecond);
            //Create Timer but don't start it yet
            _timerThread = new Timer(CheckSessions, null, Timeout.Infinite, TimeoutCheckPeriod);
            _timerStopped = true;
            _sessions = Hashtable.Synchronized(new Hashtable());
        }

        private Boolean TimedOut(DccFileSession session)
        {
            if ((DateTime.Now - session.LastActivity) >= _timeout)
            {
                return true;
            }
            return false;
        }

        internal void AddSession(DccFileSession session)
        {
            _sessions.Add(session.ID, session);
            if (_timerStopped)
            {
                _timerStopped = false;
                _timerThread.Change(TimeoutCheckPeriod, TimeoutCheckPeriod);
            }
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccSessionManager::AddSession() ID=" + session.ID);
        }

        internal void RemoveSession(DccFileSession session)
        {
            _sessions.Remove(session.ID);
            if (_sessions.Count == 0)
            {
                _timerStopped = true;
                _timerThread.Change(Timeout.Infinite, TimeoutCheckPeriod);
            }
            Debug.WriteLineIf(DccUtil.DccTrace.TraceInfo,
                              "[" + Thread.CurrentThread.Name + "] DccSessionManager::RemoveSession() ID=" + session.ID);
        }

        internal void CheckSessions(object state)
        {
            Debug.WriteLineIf(DccUtil.DccTrace.TraceVerbose,
                              "[" + Thread.CurrentThread.Name + "] DccSessionManager::CheckSessions()");
            _sessionClone = (Hashtable) _sessions.Clone();
            foreach (var session in _sessionClone.Values)
            {
                var fileSession = (DccFileSession) session;
                lock (fileSession)
                {
                    if (TimedOut(fileSession))
                    {
                        fileSession.TimedOut();
                    }
                }
            }
        }

        internal bool ContainsSession(string sessionID)
        {
            return _sessions.Contains(sessionID);
        }

        internal DccFileSession LookupSession(string sessionID)
        {
            //Make sure this session is till active
            if (!ContainsSession(sessionID))
            {
                throw new ArgumentException(sessionID + " is not active.");
            }
            //Lookup corresponding session
            return (DccFileSession) _sessions[sessionID];
        }

        /// <summary>
        ///   Returns the singleton instance.
        /// </summary>
        public static DccFileSessionManager DefaultInstance
        {
            get
            {
                lock (LockObject)
                {
                    if (_defaultInstance == null)
                    {
                        _defaultInstance = new DccFileSessionManager();
                        Debug.WriteLineIf(DccUtil.DccTrace.TraceVerbose,
                                          "[" + Thread.CurrentThread.Name + "] DccFileSessionManager::init");
                    }
                }
                return _defaultInstance;
            }
        }

        /// <summary>
        ///   Timeout period in milliseconds
        /// </summary>
        public long TimeoutPeriod
        {
            get
            {
                lock (_defaultInstance)
                {
                    return _timeout.Ticks*TimeSpan.TicksPerMillisecond;
                }
            }
            set
            {
                lock (_defaultInstance)
                {
                    _timeout = new TimeSpan(value*TimeSpan.TicksPerMillisecond);
                }
            }
        }
    }
}