using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Defines constants representing the types of messages contained in an <see cref="ILogEntry"/>.
    /// </summary>
    /// <example>
    /// <para>The following example will read a log, filtering for specific entries based on the log's <see cref="LogEntryType"/>.</para>
    /// <para>See <see cref="LogEntry"/> for examples of creating and populating logs. This example assumes the log has entries of various <see cref="LogEntryType"/>s.</para>
    /// <para>For ease of use, the example uses a <see cref="FileEventLog"/> class to perform the logging tasks. Be sure to change the <b>logFilename</b> variable to a file that has been populated with <see cref="ILogEntry"/>s.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     ##### be sure to change these as necessary #####
    ///     string logFilename = @"C:\(WORKING)\Dev\LogTests\TestLog001.txt";
    ///     bool writeLogEntriesUsingLocalTime = true;
    ///     
    ///     // create an instance of the logger
    ///     dodSON.Core.Logging.ILog logger = new dodSON.Core.Logging.FileEventLog.Log(logFilename, writeLogEntriesUsingLocalTime);
    ///     
    ///     // open the logger
    ///     logger.Open();
    ///     
    ///     // test for log's existence
    ///     if (logger.Exists)
    ///     {
    ///         // list of event type to filter
    ///         var filter = new dodSON.Core.Logging.LogEntryType[] { dodSON.Core.Logging.LogEntryType.Debug,
    ///                                                               dodSON.Core.Logging.LogEntryType.Error };
    ///         var total = 0;
    ///         var count = 0;
    ///         foreach (var log in logger.Read())
    ///         {
    ///             ++total;
    ///             if (filter.Contains(log.EntryType))
    ///             {
    ///                 ++count;
    ///                 Console.WriteLine($"\"{log.EntryType}\" \"{log.Timestamp.ToString("F")}\" \"{log.Message}\"");
    ///             }
    ///         }
    ///             Console.WriteLine();
    ///             Console.WriteLine($"{count:N0}/{total:N0} filtered/total entries.");
    ///         }
    ///         else
    ///         {
    ///             Console.WriteLine($"Log file not found. {logFilename}");
    ///         }
    ///         Console.Write("press anykey&gt;");
    ///         Console.ReadKey(true);
    ///         Console.WriteLine();
    ///     }
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // .
    /// // .
    /// // .
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #330577"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #242996"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #610887"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #449965"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #242581"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #607660"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #463173"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #709145"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #562859"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #563242"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #391549"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #231582"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #362844"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #728418"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #956075"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #269338"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #995523"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #423337"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #616973"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #894832"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #953559"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #313867"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #266095"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #331662"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #213086"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #869694"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #289182"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #907569"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #300887"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #766817"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #822013"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #219673"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #288807"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #669315"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #104864"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #744993"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #338566"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #812632"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #539673"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #334307"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #856666"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #173117"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #541573"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #350966"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #634108"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #153967"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #123360"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #692783"
    /// // "Error" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #826176"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #138155"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #610742"
    /// // "Debug" "Friday, November 04, 2016 12:40:59 PM" "Hello Log; #915397"
    /// // 
    /// // 310/1,000 filtered/total entries.
    /// // press anykey&gt;
    /// </code>
    /// </example>
    public enum LogEntryType
    {
        /// <summary>
        /// A debug event. This indicates custom, usually esoterically technical, developer-centric information about the internal working of the code base.
        /// </summary>
        Debug = 0,
        /// <summary>
        /// An information event. This indicates a significant, successful operation.
        /// </summary>
        Information,
        /// <summary>
        /// A warning event. This indicates a problem that is not immediately significant, but that may signify conditions that could cause future problems.
        /// </summary>
        Warning,
        /// <summary>
        /// An error event. This indicates a significant problem the user should know about; usually a loss of functionality or data.
        /// </summary>
        Error,
        /// <summary>
        /// A failed audit event.
        /// </summary>
        AuditFailure,
        /// <summary>
        /// A successful audit event.
        /// </summary>
        AuditSuccess
    }
}
