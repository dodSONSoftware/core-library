using System;
using System.Collections.Generic;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to write log entries to a log.
    /// </summary>
    public interface ILogWriter
        : ILogInformation
    {
        /// <summary>
        /// Writes a log entry to the log.
        /// </summary>
        /// <param name="logEntry">The log entry to write to this log.</param>
        void Write(ILogEntry logEntry);
        /// <summary>
        /// Writes a group of log entries to the log.
        /// </summary>
        /// <param name="logEntries">The log entries to write to this log.</param>
        void Write(IEnumerable<ILogEntry> logEntries);
        /// <summary>
        /// Writes all of the logs in <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        void Write(Logs logs);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        void Write(LogEntryType entryType, string sourceId, string message, int eventId);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        void Write(LogEntryType entryType, string sourceId, string message);
    }
}
