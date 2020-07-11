using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Requests logs from the Service Manager's logging system.
    /// </summary>
    [Serializable]
    public class GetLogs
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="GetLogs"/> with the specified elements.
        /// </summary>
        /// <param name="entryTypes">Defines <see cref="Logging.ILogEntry.EntryType"/>s to include in filter.</param>
        /// <param name="sourceIds">Defines <see cref="Logging.ILogEntry.SourceId"/>s to include in filter.</param>
        /// <param name="eventIds">Defines <see cref="Logging.ILogEntry.EntryType"/>s to include in filter.</param>
        /// <param name="categories">Defines <see cref="Logging.ILogEntry.Category"/>s to include in filter.</param>
        /// <param name="fromDate">The earliest date for <see cref="Logging.ILogEntry"/>s to be include in filter.</param>
        /// <param name="toDate">The latest date for <see cref="Logging.ILogEntry"/>s to be include in filter.</param>
        public GetLogs(IEnumerable<Logging.LogEntryType> entryTypes,
                       IEnumerable<string> sourceIds,
                       IEnumerable<int> eventIds,
                       IEnumerable<ushort> categories,
                       DateTimeOffset fromDate,
                       DateTimeOffset toDate)
        {
            EntryTypes = entryTypes ?? new Logging.LogEntryType[0];
            Sources = sourceIds ?? new string[0];
            EventIds = eventIds ?? new int[0];
            Categories = categories ?? new ushort[0];
            FromDate = fromDate;
            ToDate = toDate;
        }
        /// <summary>
        /// Instantiates a new <see cref="GetLogs"/> to get all logs.
        /// </summary>
        public GetLogs()
            : this(null, null, null, null, DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="GetLogs"/> to get all logs from a single source.
        /// </summary>
        /// <param name="source">Defines <see cref="Logging.ILogEntry.SourceId"/>s to include in filter.</param>
        public GetLogs(string source)
            : this(null, new string[] { source }, null, null, DateTimeOffset.MinValue, DateTimeOffset.MaxValue)
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="GetLogs"/> to get all logs from a single source between the given dates.
        /// </summary>
        /// <param name="source">>Defines <see cref="Logging.ILogEntry.SourceId"/>s to include in filter.</param>
        /// <param name="fromDate">The earliest date for <see cref="Logging.ILogEntry"/>s to be include in filter.</param>
        /// <param name="toDate">The latest date for <see cref="Logging.ILogEntry"/>s to be include in filter.</param>
        public GetLogs(string source,
                       DateTimeOffset fromDate,
                       DateTimeOffset toDate)
            : this(null, new string[] { source }, null, null, fromDate, toDate)
        {
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Defines <see cref="Logging.ILogEntry.EntryType"/>s to include in filter.
        /// </summary>
        public IEnumerable<Logging.LogEntryType> EntryTypes
        {
            get;
        }
        /// <summary>
        /// Defines <see cref="Logging.ILogEntry.SourceId"/>s to include in filter.
        /// </summary>
        public IEnumerable<string> Sources
        {
            // see [ dodSON.Core.ServiceManagement.ExecuteServicesCommand(...) ] for the reason to have an (internal set)
            get; internal set;
        }
        /// <summary>
        /// Defines <see cref="Logging.ILogEntry.EntryType"/>s to include in filter.
        /// </summary>
        public IEnumerable<int> EventIds
        {
            get;
        }
        /// <summary>
        /// Defines <see cref="Logging.ILogEntry.Category"/>s to include in filter.
        /// </summary>
        public IEnumerable<ushort> Categories
        {
            get;
        }
        /// <summary>
        /// The earliest date for <see cref="Logging.ILogEntry"/>s to be include in filter.
        /// </summary>
        public DateTimeOffset FromDate
        {
            get;
        }
        /// <summary>
        /// The latest date for <see cref="Logging.ILogEntry"/>s to be include in filter.
        /// </summary>
        public DateTimeOffset ToDate
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            var results = new StringBuilder();
            // entry types
            if (EntryTypes.Count() > 0)
            {
                AddTitle("EntryTypes");
                foreach (var item in EntryTypes)
                {
                    AddItem(item.ToString());
                }
            }
            // sources
            if (Sources.Count() > 0)
            {
                AddTitle("Sources");
                foreach (var item in Sources)
                {
                    AddItem(item);
                }
            }
            // event ids
            if (EventIds.Count() > 0)
            {
                AddTitle("EventIds");
                foreach (var item in EventIds)
                {
                    AddItem(item.ToString());
                }
            }
            // categories
            if (Categories.Count() > 0)
            {
                AddTitle("Categories");
                foreach (var item in Categories)
                {
                    AddItem(item.ToString());
                }
            }
            // from date
            AddTitle("FromDate");
            AddItem(FromDate.ToString());
            // to date
            AddTitle("ToDate");
            AddItem(ToDate.ToString());
            //
            return results.ToString();

            // ---------------- internal functions

            void AddTitle(string title)
            {
                if (results.Length > 0)
                {
                    results.Append("; ");
                }
                results.Append(title + "=");
            }

            void AddItem(string item)
            {
                if ((results.Length > 0) && (results[results.Length - 1] != '='))
                {
                    results.Append(", ");
                }
                results.Append(item);
            }
        }
        #endregion
    }
}
