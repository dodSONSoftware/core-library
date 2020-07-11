using System;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Represents a single record in an event log.
    /// </summary>
    /// <example>
    /// <para>The following example will create and open a logger; with the logger it will create or clear the logs within, write a few, new, log entries, display all the log entries and finally close the log.</para>
    /// <para>For ease of use, the example uses a <see cref="FileEventLog"/> class to perform the logging tasks. Be sure to change the <b>logFilename</b> variable.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // some variables needed for the example
    ///     int numberOfLogEntriesToCreate = 1000;
    ///     Random rnd = new Random();
    ///     Dictionary&lt;dodSON.Core.Logging.LogEntryType, int&gt; logEntryTypesAndWeights = new Dictionary&lt;dodSON.Core.Logging.LogEntryType, int&gt;();
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.Debug, 30);
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.Information, 60);
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.Warning, 5);
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.Error, 5);
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.AuditSuccess, 2);
    ///     logEntryTypesAndWeights.Add(dodSON.Core.Logging.LogEntryType.AuditFailure, 2);
    ///     Dictionary&lt;string, int&gt; logSources = new Dictionary&lt;string, int&gt;();
    ///     logSources.Add("Proxima Centauri", 4);
    ///     logSources.Add("Sirius B", 8);
    ///     logSources.Add("Procyon", 11);
    ///
    ///     // ##### be sure to change these as necessary #####
    ///     string logFilename = @"C:\(WORKING)\Dev\LogTests\TestLog001.txt";
    ///     bool writeLogEntriesUsingLocalTime = true;
    ///     bool autoTruncateLogFile = true;
    ///     long maxLogSizeBytes = dodSON.Core.Common.ByteCountHelper.FromMegabytes(100);
    ///     int logsToRetain = 100;
    ///
    ///     // create an instance of the logger
    ///     dodSON.Core.Logging.ILog logger = new dodSON.Core.Logging.FileEventLog.Log(logFilename, writeLogEntriesUsingLocalTime, autoTruncateLogFile, maxLogSizeBytes, logsToRetain);
    ///
    ///     // open the logger
    ///     logger.Open();
    ///
    ///     // either create or clear the log
    ///     if (!logger.Exists) { logger.Create(); }
    ///     else { logger.Clear(); }
    ///
    ///     // create a few entries
    ///     for (int i = 0; i&lt;numberOfLogEntriesToCreate; i++)
    ///     {
    ///         // create entry
    ///         dodSON.Core.Logging.LogEntryType rndEntryType = dodSON.Core.Common.SequenceHelper.RandomItemFromSequenceByWeight(logEntryTypesAndWeights.Keys, (v) =&gt; { return logEntryTypesAndWeights[v]; });
    ///         string rndSourceId = dodSON.Core.Common.SequenceHelper.RandomItemFromSequenceByWeight(logSources.Keys, (v) =&gt; { return logSources[v]; });
    ///         string message = $"Hello Log; #{rnd.Next(100000, 999999)}";
    ///         int eventId = rnd.Next(1, 9);
    ///         ushort category = (ushort)(eventId * 100);
    ///         dodSON.Core.Logging.ILogEntry entry = new dodSON.Core.Logging.LogEntry(rndEntryType, rndSourceId, message, eventId, category);
    ///         
    ///         // write message to log
    ///         logger.Write(entry);
    ///     }
    /// 
    ///     // display log messages
    ///     foreach (dodSON.Core.Logging.ILogEntry logEntry in logger.Read())
    ///     {
    ///         Console.WriteLine(logEntry);
    ///     }
    ///     Console.WriteLine("------------------------------");
    ///     Console.WriteLine($"{logger.LogCount:N0} entries.");
    /// 
    ///     // close logger
    ///     logger.Close();
    /// 
    ///     Console.WriteLine();
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    ///     Console.WriteLine();
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // .
    /// // .
    /// // .
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 1; 100; Sirius B; Hello Log%&lt;3B&gt; #496982
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Procyon; Hello Log%&lt;3B&gt; #811960
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Procyon; Hello Log%&lt;3B&gt; #217238
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #509346
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 1; 100; Proxima Centauri; Hello Log%&lt;3B&gt; #190325
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Procyon; Hello Log%&lt;3B&gt; #710845
    /// // 2016-11-04 12:40:59 PM -05:00; Error; 6; 600; Sirius B; Hello Log%&lt;3B&gt; #812632
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 2; 200; Sirius B; Hello Log%&lt;3B&gt; #585625
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Procyon; Hello Log%&lt;3B&gt; #660050
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 2; 200; Procyon; Hello Log%&lt;3B&gt; #225372
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Sirius B; Hello Log%&lt;3B&gt; #893642
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 8; 800; Procyon; Hello Log%&lt;3B&gt; #539673
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 1; 100; Procyon; Hello Log%&lt;3B&gt; #670686
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 7; 700; Sirius B; Hello Log%&lt;3B&gt; #334307
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Procyon; Hello Log%&lt;3B&gt; #579824
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 1; 100; Sirius B; Hello Log%&lt;3B&gt; #856666
    /// // 2016-11-04 12:40:59 PM -05:00; Warning; 3; 300; Proxima Centauri; Hello Log%&lt;3B&gt; #831567
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Sirius B; Hello Log%&lt;3B&gt; #554589
    /// // 2016-11-04 12:40:59 PM -05:00; AuditSuccess; 8; 800; Proxima Centauri; Hello Log%&lt;3B&gt; #101966
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 6; 600; Sirius B; Hello Log%&lt;3B&gt; #173117
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Sirius B; Hello Log%&lt;3B&gt; #644062
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 5; 500; Procyon; Hello Log%&lt;3B&gt; #541573
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Procyon; Hello Log%&lt;3B&gt; #515834
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #971148
    /// // 2016-11-04 12:40:59 PM -05:00; AuditFailure; 3; 300; Sirius B; Hello Log%&lt;3B&gt; #458433
    /// // 2016-11-04 12:40:59 PM -05:00; Error; 8; 800; Procyon; Hello Log%&lt;3B&gt; #350966
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Sirius B; Hello Log%&lt;3B&gt; #496532
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Sirius B; Hello Log%&lt;3B&gt; #742746
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Sirius B; Hello Log%&lt;3B&gt; #590710
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 1; 100; Procyon; Hello Log%&lt;3B&gt; #207029
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 6; 600; Proxima Centauri; Hello Log%&lt;3B&gt; #186830
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Procyon; Hello Log%&lt;3B&gt; #109847
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #278889
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #112654
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Procyon; Hello Log%&lt;3B&gt; #263154
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 6; 600; Sirius B; Hello Log%&lt;3B&gt; #288970
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 4; 400; Proxima Centauri; Hello Log%&lt;3B&gt; #265496
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 1; 100; Procyon; Hello Log%&lt;3B&gt; #634108
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Proxima Centauri; Hello Log%&lt;3B&gt; #164615
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 6; 600; Procyon; Hello Log%&lt;3B&gt; #554701
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 6; 600; Procyon; Hello Log%&lt;3B&gt; #153967
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Proxima Centauri; Hello Log%&lt;3B&gt; #486920
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 7; 700; Procyon; Hello Log%&lt;3B&gt; #123360
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 4; 400; Sirius B; Hello Log%&lt;3B&gt; #692783
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 6; 600; Procyon; Hello Log%&lt;3B&gt; #600157
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 2; 200; Proxima Centauri; Hello Log%&lt;3B&gt; #742085
    /// // 2016-11-04 12:40:59 PM -05:00; AuditFailure; 8; 800; Proxima Centauri; Hello Log%&lt;3B&gt; #845072
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Procyon; Hello Log%&lt;3B&gt; #611571
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 6; 600; Procyon; Hello Log%&lt;3B&gt; #792461
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Procyon; Hello Log%&lt;3B&gt; #435167
    /// // 2016-11-04 12:40:59 PM -05:00; Warning; 4; 400; Proxima Centauri; Hello Log%&lt;3B&gt; #888115
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 1; 100; Procyon; Hello Log%&lt;3B&gt; #468210
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Sirius B; Hello Log%&lt;3B&gt; #157458
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 8; 800; Procyon; Hello Log%&lt;3B&gt; #834455
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Sirius B; Hello Log%&lt;3B&gt; #800734
    /// // 2016-11-04 12:40:59 PM -05:00; Error; 6; 600; Proxima Centauri; Hello Log%&lt;3B&gt; #826176
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Proxima Centauri; Hello Log%&lt;3B&gt; #386721
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 4; 400; Procyon; Hello Log%&lt;3B&gt; #138155
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 7; 700; Procyon; Hello Log%&lt;3B&gt; #277069
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 3; 300; Procyon; Hello Log%&lt;3B&gt; #792637
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Proxima Centauri; Hello Log%&lt;3B&gt; #928743
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #969568
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 1; 100; Procyon; Hello Log%&lt;3B&gt; #610742
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 2; 200; Procyon; Hello Log%&lt;3B&gt; #862874
    /// // 2016-11-04 12:40:59 PM -05:00; Debug; 5; 500; Proxima Centauri; Hello Log%&lt;3B&gt; #915397
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 4; 400; Sirius B; Hello Log%&lt;3B&gt; #398872
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Procyon; Hello Log%&lt;3B&gt; #590447
    /// // 2016-11-04 12:40:59 PM -05:00; Information; 5; 500; Proxima Centauri; Hello Log%&lt;3B&gt; #881798
    /// // ------------------------------
    /// // 1,000 entries.
    /// // 
    /// // press anykey&gt;
    /// </code>
    /// </example>
    [Serializable]
    public class LogEntry
        : ILogEntry
    {
        #region Private Constants
        /// <summary>
        /// The maximum length allowed for the message string.
        /// </summary>
        private static readonly int _MaximumEntryMessageLength_ = 32512;
        #endregion
        #region Public Static Methods
        /// <summary>
        /// Converts the string representation of a log entry into its <see cref="LogEntry"/> equivalent. The return value indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="line">The string containing the log entry to convert.</param>
        /// <param name="entry">When the method returns, it contains either the converted <see cref="LogEntry"/> or null, <b>Nothing</b> in VB.</param>
        /// <returns><b>True</b> if <paramref name="line"/> was converted successfully; otherwise, <b>false</b>.</returns>
        public static bool TryParse(string line, out LogEntry entry)
        {
            try
            {
                entry = LoggingHelper.ParseLogLine(line);
                return (entry != null);
            }
            catch
            {
                entry = null;
                return false;
            }
        }
        /// <summary>
        /// Converts the string representation of a log entry into its <see cref="LogEntry"/> equivalent.
        /// </summary>
        /// <param name="line">The string containing the log entry to convert.</param>
        /// <returns>The <paramref name="line"/> converted into a <see cref="LogEntry"/> or null, <b>Nothing</b> in VB.</returns>
        public static LogEntry Parse(string line)
        {
            return LoggingHelper.ParseLogLine(line);
        }
        /// <summary>
        /// Converts the <see cref="ILogEntry"/> into its equivalent string representation.
        /// </summary>
        /// <param name="entry">The <see cref="ILogEntry"/> to convert.</param>
        /// <param name="writeLocalTime">Sets whether the date is written to the log relative to UTC or the local computer's timezone.</param>
        /// <returns>The string representation of the <paramref name="entry"/>.</returns>
        public static string Format(ILogEntry entry, bool writeLocalTime)
        {
            return LoggingHelper.FormatLogLine(entry, writeLocalTime);
        }
        #endregion
        #region Ctor
        private LogEntry()
        {
        }
        /// <summary>
        /// Initializes a new instance of the LogEntry class. 
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        /// <param name="timeStamp">The date and time when this log entry was created.</param>
        public LogEntry(LogEntryType entryType,
                        string sourceId,
                        string message,
                        int eventId,
                        ushort category,
                        DateTimeOffset timeStamp)
            : this()
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.Length > _MaximumEntryMessageLength_)
            {
                throw new ArgumentOutOfRangeException(nameof(message), $"Message too long. Maximum message length is {_MaximumEntryMessageLength_:N0} characters.");
            }
            EntryType = entryType;
            if (string.IsNullOrWhiteSpace(sourceId))
            {
                throw new ArgumentNullException(nameof(sourceId));
            }
            SourceId = sourceId;
            Message = message;
            EventId = eventId;
            Category = category;
            Timestamp = timeStamp;
        }
        /// <summary>
        /// Initializes a new instance of the LogEntry class. 
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        public LogEntry(LogEntryType entryType,
                        string sourceId,
                        string message,
                        int eventId,
                        ushort category) : this(entryType, sourceId, message, eventId, category, DateTimeOffset.UtcNow) { }
        /// <summary>
        /// Initializes a new instance of the LogEntry class. 
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public LogEntry(LogEntryType entryType,
                        string sourceId,
                        string message,
                        int eventId) : this(entryType, sourceId, message, eventId, 0, DateTimeOffset.UtcNow) { }
        /// <summary>
        /// Initializes a new instance of the LogEntry class. 
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public LogEntry(LogEntryType entryType,
                        string sourceId,
                        string message) : this(entryType, sourceId, message, 0, 0, DateTimeOffset.UtcNow) { }

        #endregion
        #region ILogEntry Methods
        /// <summary>
        /// The type of event for this log entry.
        /// </summary>
        public LogEntryType EntryType
        {
            get;
        }
        /// <summary>
        /// A user-defined id which represents the source of the log entry.
        /// </summary>
        public string SourceId
        {
            get;
        }
        /// <summary>
        /// The message for this log entry.
        /// </summary>
        public string Message
        {
            get;
        }
        /// <summary>
        /// The event identifier for this log entry.
        /// </summary>
        public int EventId
        {
            get;
        }
        /// <summary>
        /// The category number for this log entry.
        /// </summary>
        public ushort Category
        {
            get;
        }
        /// <summary>
        /// The date and time when this log entry was created.
        /// </summary>
        public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
        #endregion
        #region Overrides
        /// <summary>
        /// The string representation of the value of this instance, converted to string using the <see cref="Format(ILogEntry,bool)"/> method.
        /// </summary>
        /// <returns>The string representation of the value of this instance.</returns>
        public override string ToString() => LogEntry.Format(this, true);
        #endregion
    }
}
