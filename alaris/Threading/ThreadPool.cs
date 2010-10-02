using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Timers;
using Alaris.API;
using Timer = System.Timers.Timer;

namespace Alaris.Threading
{
    /// <summary>
    ///   A simple Thread pool implementation.
    /// </summary>
    public sealed class CThreadPool : IDisposable, IAlarisComponent
    {
        private readonly List<BackgroundWorker> _availableWorkers;
        private readonly List<BackgroundWorker> _busyWorkers;
        private readonly List<object> _activeJobs = new List<object>();
        private readonly List<object> _pendingJobs = new List<object>();
        private volatile bool _stop;
        private readonly Thread _watcher;
        private readonly Timer _integrityTimer;
        private readonly Guid _guid;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CThreadPool" /> class with the default (5) thread amount.
        /// </summary>
        public CThreadPool() : this(5)
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "CThreadPool" /> class.
        /// </summary>
        /// <param name = 'maxThreads'>
        ///   Max threads.
        /// </param>
        public CThreadPool(int maxThreads)
        {
            Log.Notice("ThreadPool", "Initializing with " + maxThreads + " threads.");


            _guid = Guid.NewGuid();

            _availableWorkers = new List<BackgroundWorker>(maxThreads + 1);
            _busyWorkers = new List<BackgroundWorker>(maxThreads + 1);

            for (int i = 0; i <= maxThreads; ++i)
            {
                _availableWorkers.Add(new BackgroundWorker());
            }

            Log.Success("ThreadPool", "Workers spawned.");
            _watcher = new Thread(WatcherProc) {Name = "Threadpool Job Watcher"};
            _watcher.Start();

            _integrityTimer = new Timer(20000);
            _integrityTimer.Elapsed += IntegrityCheck;
            _integrityTimer.Enabled = true;
            _integrityTimer.Start();
        }

        /// <summary>
        ///   Enqueue the specified job.
        /// </summary>
        /// <param name = 'job'>
        ///   Job.
        /// </param>
        public void Enqueue(IThreadContext job)
        {
            if (job == null) throw new ArgumentNullException("job");
            Log.Debug("ThreadPool", "Got new job: " + job.GetGuid());
            _pendingJobs.Add(job);
        }

        /// <summary>
        ///   Enqueue the specified task.
        /// </summary>
        /// <param name = 'task'>
        ///   Task.
        /// </param>
        public void Enqueue(ThreadRunnable task)
        {
            if (task == null) throw new ArgumentNullException("task");
            Log.Debug("ThreadPool", "Got new job: 0x" + Math.Abs(task.GetHashCode()).ToString("x"));
            _pendingJobs.Add(task);
        }

        /// <summary>
        ///   Free this pool.
        /// </summary>
        public void Free()
        {
            Log.Notice("ThreadPool", "Killing remaining thread(s).");
            _stop = true;
            _integrityTimer.Stop();

            for (var i = 0; i < _availableWorkers.Count; ++i)
            {
                var worker = _availableWorkers[i];
                _availableWorkers.Remove(worker);
                worker.Dispose();
            }

            for (var i = 0; i < _busyWorkers.Count; ++i)
            {
                var worker = _busyWorkers[i];

                if (worker.WorkerSupportsCancellation)
                    worker.CancelAsync();

                _busyWorkers.Remove(worker);
                worker.Dispose();
            }

            _watcher.Join(1000);
            _watcher.Abort();

            _activeJobs.Clear();
            _pendingJobs.Clear();

            Log.Success("ThreadPool", "Pool freed.");
        }

        /// <summary>
        ///   Dequeue the specified job.
        /// </summary>
        /// <param name = 'job'>
        ///   Job.
        /// </param>
        public void Dequeue(IThreadContext job)
        {
            _pendingJobs.Remove(job);
        }

        /// <summary>
        ///   Dequeue the specified task.
        /// </summary>
        /// <param name = 'task'>
        ///   Task.
        /// </param>
        public void Dequeue(ThreadRunnable task)
        {
            _pendingJobs.Remove(task);
        }

        /// <summary>
        ///   Stops the watcher thread.
        /// </summary>
        public void StopWatcher()
        {
            _stop = true;
        }

        private void WatcherProc()
        {
            Log.Notice("ThreadPool", "Job watcher is running.");
            while (!_stop)
            {
                if (_availableWorkers.Count == 0 || _pendingJobs.Count == 0) // no available thread, or no jobs at all.
                {
                    Thread.Sleep(100);
                    continue;
                }

                var worker = _availableWorkers[0];

                _busyWorkers.Add(worker);
                _availableWorkers.Remove(worker);

                // pick the first job.
                var job = _pendingJobs[0];
                DoWorkEventHandler dljob = null;
                if (job is IThreadContext)
                    dljob = (object sender, DoWorkEventArgs e) => { ((IThreadContext) job).Run(); };
                else if (job is ThreadRunnable)
                    dljob = (object sender, DoWorkEventArgs e) => { ((ThreadRunnable) job)(); };

                worker.DoWork += dljob;

                worker.RunWorkerCompleted += (s, e) =>
                                                 {
                                                     worker.DoWork -= dljob;
                                                     _busyWorkers.Remove(worker);
                                                     _availableWorkers.Add(worker);
                                                     Log.Debug("ThreadPool",
                                                               "Thread 0x" +
                                                               (Math.Abs(worker.GetHashCode())).ToString("x") +
                                                               " has entered the free pool.");
                                                     _activeJobs.Remove(job); // toss it to hell
                                                 };

                worker.RunWorkerAsync();
                _pendingJobs.Remove(job);
                _activeJobs.Add(job);

                if (job is IThreadContext)
                    Log.Debug("ThreadPool",
                              "Thread 0x" + (Math.Abs(worker.GetHashCode())).ToString("x") +
                              " is now executing task with guid: " + (job as IThreadContext).GetGuid());
                else if (job is ThreadRunnable)
                    Log.Debug("ThreadPool",
                              "Thread 0x" + (Math.Abs(worker.GetHashCode())).ToString("x") +
                              " is now executing task: 0x" + Math.Abs(job.GetHashCode()).ToString("x"));

                Thread.Sleep(50);
            }
        }

        private void IntegrityCheck(object s, ElapsedEventArgs e)
        {
            Log.Notice("ThreadPool", "Performing integrity check...");

            if (_pendingJobs.Count <= 1 && _activeJobs.Count <= 1 && _availableWorkers.Count >= 3)
            {
                Log.Debug("IntegrityCheck", "Killing 1 napping thread(s).");
                var worker = _availableWorkers[0];
                _availableWorkers.Remove(worker);
                worker.Dispose();
                return;
            }

            if ((_pendingJobs.Count + _activeJobs.Count) >= _availableWorkers.Count)
            {
                Log.Debug("IntegrityCheck", "Spawning a new worker thread...");
                var worker = new BackgroundWorker();
                _availableWorkers.Add(worker);
                return;
            }

            //Log.Debug("IntegrityCheck", "Spawned thread count: " + _availableWorkers.Count + _busyWorkers.Count);
        }


        /// <summary>
        ///   Disposes this object.
        /// </summary>
        public void Dispose()
        {
            _stop = true;
            _watcher.Join(600);
            _integrityTimer.Stop();
            _integrityTimer.Close();
        }

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}