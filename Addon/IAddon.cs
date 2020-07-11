using System;

namespace dodSON.Core.Addon
{
    /// <summary>
    /// Defines mechanisms to control an <see cref="IAddon"/>.
    /// </summary>
    public interface IAddon
    {
        /// <summary>
        /// The date the <see cref="IAddon"/> was started.
        /// </summary>
        DateTime DateLastStarted { get; }
        /// <summary>
        /// The date the <see cref="IAddon"/> was stopped.
        /// </summary>
        DateTime DateLastStopped { get; }
        /// <summary>
        /// Returns whether the <see cref="IAddon"/> is running. Returns <b>true</b> if the <see cref="IAddon"/> is running; otherwise, <b>false</b>.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// Returns the total duration the <see cref="IAddon"/> has been running.
        /// </summary>
        /// <seealso cref="IAddon.LastRunDuration"/>
        TimeSpan OverallRunDuration { get; }
        /// <summary>
        /// Returns the duration the <see cref="IAddon"/> has been running since it was last started.
        /// </summary>
        /// <seealso cref="IAddon.OverallRunDuration"/>
        TimeSpan LastRunDuration { get; }
        /// <summary>
        /// Returns the number of times the <see cref="IAddon"/> has been started.
        /// </summary>
        int StartCount { get; }
        /// <summary>
        /// Returns the number of times the <see cref="IAddon"/> has been stopped.
        /// </summary>
        int StopCount { get; }
        /// <summary>
        /// Starts the <see cref="IAddon"/>.
        /// </summary>
        void Start();
        /// <summary>
        /// Stops the <see cref="IAddon"/>.
        /// </summary>
        void Stop();
    }
}
