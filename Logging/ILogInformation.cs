using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to get information about a log and retrieve the <see cref="SyncObject"/> for thread synchronization.
    /// </summary>
    public interface ILogInformation
    {
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Returns an object that can be used for synchronizing access to the underlying logging system across multiple threads.
        /// </summary>
        object SyncObject { get; }
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        bool Exists { get; }
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        bool IsOpen { get; }
        /// <summary>
        /// Gets the number of log entries contained in the log.
        /// </summary>
        int LogCount { get; }
    }
}
