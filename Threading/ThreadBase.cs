using System;

namespace dodSON.Core.Threading
{
    /// <summary>
    /// Base class for creating and managing well behaved, long running threads.
    /// </summary>
    public abstract class ThreadBase
        : IThreadBase
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected ThreadBase()
        {
        }
        #endregion
        #region Private Fields
        private System.Threading.Thread _Thread = null;

        // these are guaranteed to be disposed: see the try...finally in the Start() method's thread
        private System.Threading.AutoResetEvent _WaitHandle_StopThread = null;
        private System.Threading.AutoResetEvent _WaitHandle_ThreadWorkerWorking = null;
        // this cannot be guaranteed to be disposed of inside this class: must call Stop(...) or Dispose() from outside
        private System.Threading.AutoResetEvent _WaitHandle_ThreadStateChange = null;
        // ****
        private readonly object _SyncLock = new object();

        // TODO: consider replacing this with a System.Threading.CancellationTokenSource
        //          however, since it is only used internally by the local thread, it should be alright
        //          one less thing to worry about disposing
        private readonly ThreadCancelToken _CancelToken = new ThreadCancelToken();

        private bool _ExecuteNow = false;
        private TimeSpan _LastExecutionDuration = TimeSpan.Zero;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets a value indicating the execution status of the thread.
        /// </summary>
        public bool IsAlive => (_Thread == null) ? false : _Thread.IsAlive;
        /// <summary>
        /// Gets a value indicating if the <see cref="OnExecute"/> method is currently running. <b>True</b> if the <see cref="OnExecute"/> method is currently running; other <b>False</b>.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// Gets a value indicating whether the thread was aborted. <b>True</b> if the thread was aborted; otherwise <b>False</b>.
        /// </summary>
        public bool WasAborted { get; private set; } = false;
        /// <summary>
        /// Gets a value indicating the date and time the thread was started.
        /// </summary>
        public DateTime StartedDate { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// Gets a value indicating the date and time the thread was stopped.
        /// </summary>
        public DateTime StoppedDate { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// Gets a value indicating the time the thread has been alive. See <see cref="IsAlive"/>.
        /// </summary>
        public TimeSpan AliveDuration
        {
            get
            {
                lock (_SyncLock)
                {
                    if (StoppedDate >= StartedDate)
                    {
                        return (StoppedDate - StartedDate);
                    }
                    return (DateTime.Now - StartedDate);
                }
            }
        }
        /// <summary>
        /// Gets a value indicating the date and time the <see cref="OnExecute"/> method was last started.
        /// </summary>
        public DateTime LastExecutionDate { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// Gets a value indicating the time it took for the <see cref="OnExecute"/> method to process.
        /// </summary>
        public TimeSpan LastExecutionDuration
        {
            get
            {
                lock (_SyncLock)
                {
                    if (IsRunning)
                    {
                        return (DateTime.Now - LastExecutionDate);
                    }
                    return _LastExecutionDuration;
                }
            }
        }
        /// <summary>
        /// Gets a value indicating the next date and time the <see cref="OnExecute"/> method will be started. 
        /// See <see cref="LastExecutionDate"/>, <see cref="LastExecutionDuration"/> and <see cref="ExecutionInterval"/>.
        /// </summary>
        public DateTime NextExecutionDate
        {
            get
            {
                if (_Thread != null)
                {
                    lock (_SyncLock)
                    {
                        return LastExecutionDate.Add(LastExecutionDuration).Add(ExecutionInterval);
                    }
                }
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Gets a value indicating the amount of time before the <see cref="OnExecute"/> method will be started. 
        /// See <see cref="LastExecutionDate"/>, <see cref="LastExecutionDuration"/> and <see cref="ExecutionInterval"/>.
        /// </summary>
        public TimeSpan NextExecutionInterval
        {
            get
            {
                lock (_SyncLock)
                {
                    if ((_Thread == null) || (IsRunning))
                    {
                        return TimeSpan.Zero;
                    }
                    var result = (LastExecutionDate.Add(_LastExecutionDuration).Add(ExecutionInterval)) - DateTime.Now;
                    if (result < TimeSpan.Zero)
                    {
                        result = TimeSpan.Zero;
                    }
                    return result;
                }
            }
        }
        // ****************
        /// <summary>
        /// This will create and start a background thread. This will execute, almost immediately, the <see cref="OnExecute"/> method.
        /// </summary>
        public void Start()
        {
            // if already started; exit
            if (_Thread != null)
            {
                return;
            }
            // create new thread
            lock (_SyncLock)
            {
                // create all wait handles
                _WaitHandle_StopThread = new System.Threading.AutoResetEvent(false);
                _WaitHandle_ThreadStateChange = new System.Threading.AutoResetEvent(false);
                _WaitHandle_ThreadWorkerWorking = new System.Threading.AutoResetEvent(false);
                _ExecuteNow = false;
                IsRunning = false;
                WasAborted = false;
                _CancelToken.CancelThread = false;
                _Thread = new System.Threading.Thread(new System.Threading.ThreadStart(() =>
                {
                    try
                    {
                        // indicate thread has started
                        _WaitHandle_ThreadStateChange.Set();
                        // loop forever...
                        var terminateThread = false;
                        do
                        {
                            // do thread worker work
                            try
                            {
                                lock (_SyncLock)
                                {
                                    _WaitHandle_ThreadWorkerWorking.Reset();
                                    IsRunning = true;
                                    LastExecutionDate = DateTime.Now;
                                }
                                OnExecute(_CancelToken);
                            }
                            catch (System.Threading.ThreadAbortException)
                            {
                                lock (_SyncLock)
                                {
                                    WasAborted = true;
                                    System.Threading.Thread.ResetAbort();
                                }
                                break;
                            }
                            finally
                            {
                                lock (_SyncLock)
                                {
                                    _LastExecutionDuration = (DateTime.Now - LastExecutionDate);
                                    IsRunning = false;
                                    _WaitHandle_ThreadWorkerWorking.Set();
                                }
                            }
                            // wait and check for thread termination || execute thread now
                            while (true)
                            {
                                if (_WaitHandle_StopThread.WaitOne(ExecutionInterval))
                                {
                                    // signaled.
                                    if (_ExecuteNow)
                                    {
                                        lock (_SyncLock)
                                        {
                                            _ExecuteNow = false;
                                            _WaitHandle_StopThread.Reset();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        // indicates request for thread stop
                                        terminateThread = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    // timeout.
                                    if (CanExecute)
                                    {
                                        break;
                                    }
                                }
                            }
                        } while (!terminateThread);
                        // indicate thread has stopped
                        _WaitHandle_ThreadStateChange.Set();
                    }
                    finally
                    {
                        lock (_SyncLock)
                        {
                            // close and dispose of all wait handles
                            if (_WaitHandle_StopThread != null)
                            {
                                _WaitHandle_StopThread.Close();
                                _WaitHandle_StopThread = null;
                            }
                            //
                            if (_WaitHandle_ThreadWorkerWorking != null)
                            {
                                _WaitHandle_ThreadWorkerWorking.Close();
                                _WaitHandle_ThreadWorkerWorking = null;
                            }
                        }
                    }
                }));
                _Thread.IsBackground = true;
                var parts = Guid.NewGuid().ToString().Split('-');
                _Thread.Name = string.Format("{0}-{1}-{2}-{3}-{4}{5}{6}{7}{8}", parts[0], parts[1], parts[2], parts[3], parts[4].Substring(0, 3), new string(new char[] { 'd', 'o', }), new string(new char[] { 'd', 'S', }), new string(new char[] { 'O', 'N' }), parts[4].Substring(4, 3));
                StartedDate = DateTime.Now;
                StoppedDate = DateTime.MinValue;
                LastExecutionDate = DateTime.MinValue;
                _LastExecutionDuration = TimeSpan.Zero;
            }
            // call OnStart
            OnStart();
            // start new thread
            _Thread.Start();
            _WaitHandle_ThreadStateChange.WaitOne();
        }
        /// <summary>
        /// If the <see cref="OnExecute"/> method is not already running, this will cause the <see cref="OnExecute"/> method to be started immediately. If the <see cref="OnExecute"/> method is running, this method will do nothing.
        /// </summary>
        /// <param name="waitForThreadWorkerToComplete">If <b>True</b> this method will wait until the <see cref="OnExecute"/> method has completed its task before returning; otherwise if <b>False</b> this method will return immediately.</param>
        public void ExecuteNow(bool waitForThreadWorkerToComplete)
        {
            if ((_Thread != null) && (!IsRunning))
            {
                lock (_SyncLock)
                {
                    _ExecuteNow = true;
                    _WaitHandle_ThreadWorkerWorking.Reset();
                    _WaitHandle_StopThread.Set();
                }
                if (waitForThreadWorkerToComplete)
                {
                    _WaitHandle_ThreadWorkerWorking.WaitOne();
                }
            }
        }
        /// <summary>
        /// If the thread <see cref="IsAlive"/>, this method will stop the thread, aborting it if necessary.
        /// </summary>
        public void Stop() => Stop(TimeSpan.FromMilliseconds(int.MaxValue));
        /// <summary>
        /// If the thread <see cref="IsAlive"/>, this method will stop the thread, aborting it if necessary. Waiting the <paramref name="terminationWaitTime"/> time for the <see cref="OnExecute"/> method to terminate on its own before aborting it.
        /// </summary>
        /// <param name="terminationWaitTime">The amount of time to wait for the <see cref="OnExecute"/> method to terminate on its own before aborting it.</param>
        public void Stop(TimeSpan terminationWaitTime)
        {
            if (_Thread != null)
            {
                try
                {
                    lock (_SyncLock)
                    {
                        _ExecuteNow = false;
                        _WaitHandle_ThreadStateChange.Reset();
                        _CancelToken.CancelThread = true;
                        _WaitHandle_StopThread.Set();
                    }
                    if (!_WaitHandle_ThreadStateChange.WaitOne(terminationWaitTime))
                    {
                        // timed out (thread still running)
                        _Thread.Abort();
                        _Thread.Join(terminationWaitTime);
                    }
                }
                finally
                {
                    lock (_SyncLock)
                    {
                        StoppedDate = DateTime.Now;
                        _Thread = null;
                        // close and dispose of all wait handles
                        // fail-safe
                        if (_WaitHandle_StopThread != null)
                        {
                            _WaitHandle_StopThread.Close();
                            _WaitHandle_StopThread = null;
                        }
                        // the only place where this WaitHandle is being disposed
                        _WaitHandle_ThreadStateChange.Close();
                        _WaitHandle_ThreadStateChange = null;
                        // fail-safe
                        if (_WaitHandle_ThreadWorkerWorking != null)
                        {
                            _WaitHandle_ThreadWorkerWorking.Close();
                            _WaitHandle_ThreadWorkerWorking = null;
                        }
                    }
                    // call OnStop()
                    OnStop();
                }
            }
        }
        #endregion
        #region Abstract Methods
        /// <summary>
        /// Gets a value indicating the amount of time to wait before checking the <see cref="CanExecute"/> method to determine if the <see cref="OnExecute(ThreadCancelToken)"/> should be executed.
        /// </summary>
        protected abstract TimeSpan ExecutionInterval
        {
            get;
        }
        /// <summary>
        /// Gets a value indicating whether the <see cref="OnExecute"/> methods should be executed.
        /// </summary>
        protected abstract bool CanExecute
        {
            get;
        }
        /// <summary>
        /// The method called to perform work every <see cref="ExecutionInterval"/> when the <see cref="CanExecute"/> method returns <b>true</b>.
        /// </summary>
        /// <param name="cancelToken">A <see cref="ThreadCancelToken"/> object which should be monitored by the <see cref="OnExecute"/> method. If the <see cref="ThreadCancelToken.CancelThread"/> becomes <b>True</b> the <see cref="OnExecute"/> method should, gracefully, terminate execution as quickly as possible.</param>
        protected abstract void OnExecute(ThreadCancelToken cancelToken);
        /// <summary>
        /// Called just before the thread is started.
        /// </summary>
        protected abstract void OnStart();
        /// <summary>
        /// Called just after the thread has stopped.
        /// </summary>
        protected abstract void OnStop();
        #endregion
        #region IDisposable Methods
        /// <summary>
        /// Provides a mechanism for releasing resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Ensures that the internal <see cref="System.Threading.AutoResetEvent"/>s are disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop(TimeSpan.FromSeconds(30));
            }
        }
        #endregion
    }
}
