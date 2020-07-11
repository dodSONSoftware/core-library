using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Encapsulates a single record in an event log.
    /// </summary>
    public interface ILogEntry
    {
        /// <summary>
        /// The type of event for this log entry.
        /// </summary>
        LogEntryType EntryType { get; }
        /// <summary>
        /// A user-defined id which represents the source of the log entry.
        /// </summary>
        string SourceId { get; }
        /// <summary>
        /// The message for this log entry.
        /// </summary>
        string Message { get; }
        /// <summary>
        /// The event identifier for this log entry.
        /// </summary>
        int EventId { get; }
        /// <summary>
        /// The category number for this log entry.
        /// </summary>
        ushort Category { get; }
        /// <summary>
        /// The date and time when this log entry was created.
        /// </summary>
        DateTimeOffset Timestamp { get; }
    }
}
