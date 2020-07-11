using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to create, delete, clear, read and write a log with <see cref="ILogEntry"/> log entries.
    /// </summary>
    public interface ILog
        : ILogInformation,
          ILogReaderWriter,
          Configuration.IConfigurable
    {
        /// <summary>
        /// Will create an empty log.
        /// </summary>
        void Create();
        /// <summary>
        /// Will delete the log.
        /// </summary>
        /// <remarks>This will permanently delete all log entries in the log; this action <i>cannot</i> be undone.</remarks>
        void Delete();
        /// <summary>
        /// Opens an existing log, or creates a new one if log not found.
        /// </summary>
        void Open();
        /// <summary>
        /// Closes the log.
        /// </summary>
        void Close();
        /// <summary>
        /// Will remove all log entries from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all log entries from the log; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entries removed.</returns>
        int Clear();
        /// <summary>
        /// Get the current statistics for the log.
        /// </summary>
        ILogStatistics Statistics { get; }
    }
}
