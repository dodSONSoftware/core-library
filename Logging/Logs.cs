using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Represents a collection of <see cref="ILogEntry"/>s.
    /// </summary>
    [Serializable]
    public class Logs
        : ICollection<ILogEntry>
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new empty instance of the <see cref="Logs"/> class.
        /// </summary>
        public Logs() { }
        /// <summary>
        /// Instantiates a new instance of the <see cref="Logs"/> class, populating it with the logs from the <paramref name="entries"/> type.
        /// </summary>
        /// <param name="entries">A <see cref="Logging.Logs"/> type containing the initial logs.</param>
        public Logs(Logs entries)
        : this()
        {
            //if (entries == null) { throw new ArgumentNullException(nameof(entries)); }   // checked in Add()
            Add(entries);
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="Logs"/> class, populating it with the logs from the <paramref name="entries"/> collection.
        /// </summary>
        /// <param name="entries">A collection of <see cref="ILogEntry"/>.</param>
        public Logs(IEnumerable<ILogEntry> entries)
        : this()
        {
            //if (entries == null) { throw new ArgumentNullException(nameof(entries)); }   // checked in Add()
            Add(entries);
        }
        #endregion
        #region Private Fields
        private List<ILogEntry> _Entries = new List<ILogEntry>();
        #endregion
        #region IInstallLog Methods
        /// <summary>
        /// Gets a value indicating whether the <see cref="Logging.Logs"/> is read-only.
        /// </summary>
        public bool IsReadOnly => false;
        /// <summary>
        /// Gets the number of elements contained in the <see cref="Logging.Logs"/>.
        /// </summary>
        public int Count => _Entries.Count;
        /// <summary>
        /// The earliest <see cref="DateTimeOffset"/> found the log entries.
        /// </summary>
        public DateTimeOffset FirstDate => ((_Entries.Count > 0) ? (_Entries.First().Timestamp) : DateTimeOffset.MinValue);
        /// <summary>
        /// The latest <see cref="DateTimeOffset"/> found the log entries.
        /// </summary>
        public DateTimeOffset LastDate => ((_Entries.Count > 0) ? (_Entries.Last().Timestamp) : DateTimeOffset.MinValue);
        /// <summary>
        /// The total amount of time all the <see cref="ILogEntry"/>s span in the <see cref="Logging.Logs"/>
        /// </summary>
        public TimeSpan DateSpan => ((_Entries.Count > 0) ? (LastDate - FirstDate) : TimeSpan.Zero);
        /// <summary>
        /// Determines whether the <see cref="Logging.Logs"/> contains a specific value.
        /// </summary>
        /// <param name="item">The <see cref="ILogEntry"/> to locate in the <see cref="Logging.Logs"/>.</param>
        /// <returns>Whether the <see cref="Logging.Logs"/> contains a specific value.</returns>
        public bool Contains(ILogEntry item) => _Entries.Contains(item);
        /// <summary>
        /// Adds an <see cref="ILogEntry"/> to the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="item">The <see cref="ILogEntry"/> to add.</param>
        public void Add(ILogEntry item)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            _Entries.Add(item);
        }
        /// <summary>
        /// Adds all the <see cref="ILogEntry"/>s from <paramref name="entries"/> to the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="entries">The <see cref="Logging.Logs"/> to add the <see cref="ILogEntry"/>s from.</param>
        public void Add(Logs entries) => Add((IEnumerable<ILogEntry>)entries);
        /// <summary>
        /// Adds all the <see cref="ILogEntry"/>s from a collection of <see cref="ILogEntry"/>s to the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="entries">The collection of <see cref="ILogEntry"/>s to add.</param>
        public void Add(IEnumerable<ILogEntry> entries)
        {
            if (entries == null) { throw new ArgumentNullException(nameof(entries)); }
            foreach (var item in entries) { Add(item); }
        }
        /// <summary>
        /// Add an <see cref="ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        public void Add(LogEntryType entryType, string sourceId, string message) => Add(new LogEntry(entryType, sourceId, message, 0, 0, DateTimeOffset.Now));
        /// <summary>
        /// Add an <see cref="ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        public void Add(LogEntryType entryType, string sourceId, string message, int eventId) => Add(new LogEntry(entryType, sourceId, message, eventId, 0, DateTimeOffset.Now));
        /// <summary>
        /// Add an <see cref="ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        /// <param name="category">The category for this log entry.</param>
        public void Add(LogEntryType entryType, string sourceId, string message, int eventId, ushort category) => Add(new LogEntry(entryType, sourceId, message, eventId, category, DateTimeOffset.Now));
        /// <summary>
        /// Add an <see cref="ILogEntry"/> to the <see cref="Logging.Logs"/> with the given information.
        /// </summary>
        /// <param name="entryType">The type of log entry.</param>
        /// <param name="sourceId">The source of the log entry.</param>
        /// <param name="message">The log entry's message.</param>
        /// <param name="eventId">The event id of the log entry.</param>
        /// <param name="category">The category for this log entry.</param>
        /// <param name="timeStamp">The <see cref="DateTimeOffset"/> the log entry occurred.</param>
        public void Add(LogEntryType entryType, string sourceId, string message, int eventId, ushort category, DateTimeOffset timeStamp) => Add(new LogEntry(entryType, sourceId, message, eventId, category, timeStamp));
        /// <summary>
        /// Removes the <see cref=" ILogEntry"/> from the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="item">The <see cref="ILogEntry"/> to remove.</param>
        /// <returns><b>True</b> if <see cref="ILogEntry"/> is successfully removed; otherwise, <b>false</b>. This method also returns <b>false</b> if item was not found in the <see cref="Logging.Logs"/>.</returns>
        public bool Remove(ILogEntry item) => _Entries.Remove(item);
        /// <summary>
        /// Removes all <see cref="ILogEntry"/>s.
        /// </summary>
        public void Clear() => _Entries.Clear();
        /// <summary>
        /// Copies the elements of the <see cref="Logging.Logs"/> to an System.Array, starting at a particular System.Array index.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from the <see cref="Logging.Logs"/>. The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(ILogEntry[] array, int arrayIndex) => _Entries.CopyTo(array, arrayIndex);
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        public IEnumerator<ILogEntry> GetEnumerator() => _Entries.GetEnumerator();
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that iterates through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => _Entries.GetEnumerator();
        #endregion
    }
}
