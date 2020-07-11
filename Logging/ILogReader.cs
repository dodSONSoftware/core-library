using System;
using System.Collections.Generic;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines functionality to read log entries from a log.
    /// </summary>
    public interface ILogReader
        : ILogInformation
    {
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        IEnumerable<ILogEntry> Read();
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate">A function which takes an <see cref="ILogEntry"/> and returns <b>true</b> to include the log entry, or <b>false></b> to skip the log entry.</param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate);
    }
}
