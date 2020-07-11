using System;
using System.Collections.Generic;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines access to and control of a configurable, single-entry, memory-cached, archive-enabled logging mechanism.
    /// </summary>
    /// <seealso cref="LogMarshal"/>
    /// <seealso cref="MarshalByRefObject"/>
    public interface ILogController
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Indicates the operating state of the <see cref="ILogController"/>.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="ILogController"/> started.
        /// </summary>
        DateTimeOffset StartTime { get; }
        /// <summary>
        /// The duration that the <see cref="ILogController"/> has run. 
        /// </summary>
        TimeSpan RunTime { get; }
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId { get; }
        /// <summary>
        /// Settings to control the actions of the <see cref="ILogController"/>.
        /// </summary>
        LogControllerSettings Settings { get; }
        /// <summary>
        /// Statistical information about the logging system.
        /// </summary>
        Logging.ILogStatistics Statistics { get; }
        /// <summary>
        /// A logger derived from <see cref="MarshalByRefObject"/>, that is configurable, memory-cached and archive-enabled.
        /// Being derived from <see cref="MarshalByRefObject"/>, allows the <see cref="LogMarshal"/> to be passed from one <see cref="AppDomain"/> to another, yet still reference the singular instance.
        /// </summary>
        LogMarshal LogMarshal { get; }
        /// <summary>
        /// Initializes and engages the logging sub-systems.
        /// </summary>
        void Start();
        /// <summary>
        /// Terminates the logging sub-systems.
        /// </summary>
        void Stop();
        /// <summary>
        /// Will remove all log entries.
        /// </summary>
        /// <param name="includeArchivedLogs"><b>True</b> will clear all archived logs; otherwise, <b>false</b> will leave the archived logs alone.</param>
        void Clear(bool includeArchivedLogs);
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        IEnumerable<Logging.ILogEntry> Read();
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        IEnumerable<Logging.ILogEntry> Read(Func<Logging.ILogEntry, bool> logEntryFilterPredicate);
        /// <summary>
        /// Returns each log entry in the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="Logging.ILogEntry"/> and returns <b>true</b> to include the log entry, or false to skip the log entry.</param>
        /// <param name="archiveFileFilterPredicate">A function which takes the <see cref="FileStorage.IFileStoreItem"/> about to be processed and returns <b>true</b> to include the log file, or false to skip the log file.</param>
        /// <returns>Every log entry from the log and the archive which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        IEnumerable<Logging.ILogEntry> Read(Func<FileStorage.IFileStoreItem, bool> archiveFileFilterPredicate, Func<Logging.ILogEntry, bool> logEntryFilterPredicate);
        /// <summary>
        /// Writes a group of log entries to the log.
        /// </summary>
        /// <param name="entries">The log entries to write to this log.</param>
        void Write(IEnumerable<Logging.ILogEntry> entries);
        /// <summary>
        /// Writes a group of log entries to the log.
        /// </summary>
        /// <param name="entries">The log entries to write to this log.</param>
        void Write(Logging.Logs entries);
        /// <summary>
        /// Writes a log entry to the log.
        /// </summary>
        /// <param name="logEntry">The log entry to write to this log.</param>
        void Write(Logging.ILogEntry logEntry);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        void Write(Logging.LogEntryType entryType, string sourceId, string message);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId, ushort category);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        /// <param name="timeStamp">The date for this log entry.</param>
        void Write(Logging.LogEntryType entryType, string sourceId, string message, int eventId, ushort category, DateTimeOffset timeStamp);
    }
}
