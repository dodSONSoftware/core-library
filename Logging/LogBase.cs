using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dodSON.Core.Configuration;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines the base functionality for being a log.
    /// </summary>
    public abstract class LogBase
        : ILog
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected LogBase() { }
        #endregion
        #region ILog Methods
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public abstract string Id { get; }
        /// <summary>
        /// When overridden, should return an object that can be used for synchronizing access to the logging system across multiple threads.
        /// </summary>
        public object SyncObject { get; } = new object();
        /// <summary>
        /// When overridden, should get a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public abstract bool Exists { get; }
        /// <summary>
        /// When overridden, should return whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public abstract bool IsOpen { get; }
        /// <summary>
        /// When overridden, should get the number of log entries contained in the log.
        /// </summary>
        public abstract int LogCount { get; }
        /// <summary>
        /// When overridden, should get the current statistics for the log.
        /// </summary>
        public abstract ILogStatistics Statistics { get; }
        /// <summary>
        /// When overridden, should populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public abstract IConfigurationGroup Configuration { get; }
        /// <summary>
        /// When overridden, should remove all log entries from the log.
        /// </summary>
        /// <remarks>
        /// This should permanently delete all log entries from the log; this action should <i>not</i> be undoable.
        /// <para>This should clear the log, <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entries removed.</returns>
        public abstract int Clear();
        /// <summary>
        /// When overridden, should close the log.
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// When overridden, should create an empty log.
        /// </summary>
        public abstract void Create();
        /// <summary>
        /// When overridden, should delete the log.
        /// </summary>
        /// <remarks>This should permanently delete all log entries in the log; this action should <i>not</i> be undoable.</remarks>
        public abstract void Delete();
        /// <summary>
        /// When overridden, should open an existing log, or creates a new one if log not found.
        /// </summary>
        public abstract void Open();
        /// <summary>
        /// When overridden, should read each log entry from the log.
        /// </summary>
        /// <returns>Should return all log entries in the log.</returns>
        public abstract IEnumerable<ILogEntry> Read();
        /// <summary>
        /// When overridden, should read each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Should return every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public abstract IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate);
        /// <summary>
        /// When overridden, should write a group of log entries to the log.
        /// </summary>
        /// <param name="logEntries">The log entries to write to the log.</param>
        public abstract void Write(IEnumerable<ILogEntry> logEntries);
        /// <summary>
        /// Writes all of the logs in the <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public abstract void Write(Logs logs);
        /// <summary>
        /// When overridden, should write a log entry to the log.
        /// </summary>
        /// <param name="logEntry">The log entry to write to the log.</param>
        public abstract void Write(ILogEntry logEntry);
        /// <summary>
        /// When overridden, should create a log entry with the specified information and write it to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public abstract void Write(LogEntryType entryType, string sourceId, string message);
        /// <summary>
        /// When overridden, should create a log entry with the specified information and write it to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public abstract void Write(LogEntryType entryType, string sourceId, string message, int eventId);
        /// <summary>
        /// When overridden, should create a log entry with the specified information and write it to the log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        public abstract void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category);
        #endregion
    }
}
