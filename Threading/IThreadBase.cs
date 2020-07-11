using System;

namespace dodSON.Core.Threading
{
    /// <summary>
    /// Defines functionality to create and manage well behaved, long running threads.
    /// </summary>
    public interface IThreadBase
        : IDisposable
    {
        /// <summary>
        /// Gets a value indicating the execution status of the thread.
        /// </summary>
        bool IsAlive { get; }
        /// <summary>
        /// Gets a value indicating if the thread is currently running. <b>True</b> if the thread is currently running; other <b>False</b>.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Gets a value indicating whether the thread was aborted. <b>True</b> if the thread was aborted; otherwise <b>False</b>.
        /// </summary>
        bool WasAborted { get; }
        /// <summary>
        /// Gets a value indicating the date and time the thread was started.
        /// </summary>
        DateTime StartedDate { get; }
        /// <summary>
        /// Gets a value indicating the date and time the thread was stopped.
        /// </summary>
        DateTime StoppedDate { get; }
        /// <summary>
        /// Gets a value indicating the time the thread has been alive. See <see cref="IsAlive"/>.
        /// </summary>
        TimeSpan AliveDuration { get; }
        /// <summary>
        /// Gets a value indicating the date and time the thread was last started.
        /// </summary>
        DateTime LastExecutionDate { get; }
        /// <summary>
        /// Gets a value indicating the time it took for the thread to process.
        /// </summary>
        TimeSpan LastExecutionDuration { get; }
        /// <summary>
        /// Gets a value indicating the next date and time the thread will be started. 
        /// See <see cref="LastExecutionDate"/>, <see cref="LastExecutionDuration"/>, <see cref="NextExecutionDate"/> and <see cref="NextExecutionInterval"/>.
        /// </summary>
        DateTime NextExecutionDate { get; }
        /// <summary>
        /// Gets a value indicating the amount of time before the thread will be started. 
        /// See <see cref="LastExecutionDate"/>, <see cref="LastExecutionDuration"/>, <see cref="NextExecutionDate"/> and <see cref="NextExecutionInterval"/>.
        /// </summary>
        TimeSpan NextExecutionInterval { get; }
        /// <summary>
        /// If the thread is not already running, this will cause the thread to be started immediately. If the thread is running, this method will do nothing.
        /// </summary>
        /// <param name="waitForThreadWorkerToComplete">If <b>True</b> this method will wait until the thread has completed its task before returning; otherwise if <b>False</b> this method will return immediately.</param>
        void ExecuteNow(bool waitForThreadWorkerToComplete);
        /// <summary>
        /// This will create and start a background thread. This will execute the thread, almost immediately.
        /// </summary>
        void Start();
        /// <summary>
        /// If the thread <see cref="IsAlive"/>, this method will stop the thread, aborting it if necessary.
        /// </summary>
        void Stop();
        /// <summary>
        /// If the thread <see cref="IsAlive"/>, this method will stop the thread, aborting it if necessary. Waiting the <paramref name="terminationWaitTime"/> time for the thread method to terminate on its own before aborting it.
        /// </summary>
        /// <param name="terminationWaitTime">The amount of time to wait for the thread method to terminate on its own before aborting it.</param>
        void Stop(TimeSpan terminationWaitTime);
    }
}