using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Logging
{
    /// <summary>
    /// Will split <see cref="ILogEntry"/>s into separate <see cref="ILog"/>s.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example will create a Log Splitter and adds a set of LogFilters.
    /// <br/><br/>
    /// It will write 500 logs to the Log Splitter.
    /// The log filters will separate each log entry based on it's EventId, which cycles between 0 and 9.
    /// This has the effect of creating 10 log files; each log file containing only log entries where the log entry's EventId is the same.
    /// <br/><br/>
    /// After the Log Splitter completes writing the log entries to the various logs, the example will create a Log Combiner with all of the logs used by the Log Splitter.
    /// The Log Combiner will read all the log entries from all the logs it was given, sorted by time (ascending), writing those log entries to the OutputLog.log file.
    /// <br/><br/>
    /// All sub log files should contain 50 log entries each.
    /// All sharing the same EventId.
    /// <br/>
    /// The OutputLog.log file should contain 500 log entries, sorted by time (ascending); in other words, all the 10 logs combined.
    /// </para>
    /// <para>
    /// <br/>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// class Program
    /// {
    ///     private static dodSON.Core.Configuration.IConfigurationSerializer&lt;byte[]&gt; _ConfigurationBinarySerializer = new dodSON.Core.Configuration.BinaryConfigurationSerializer();
    ///     private static dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; _ConfigurationXmlSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///     private static dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; _ConfigurationIniSerializer = new dodSON.Core.Configuration.IniConfigurationSerializer();
    ///     private static dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; _ConfigurationCsvSerializer = new dodSON.Core.Configuration.CsvConfigurationSerializer();
    /// 
    ///     static void Main(string[] args)
    ///     {
    ///         // ################################################################
    ///         // ######## CHANGE THESE VALUE     
    ///         var outputRootPath = @"C:\(WORKING)\Dev\LogSplitter";
    ///         var LogsToWrite = 500;
    ///         // ########
    ///         // ################################################################
    /// 
    ///
    /// 
    ///         // ******** Validate outputRootPath is Available and Clear
    ///         if (!System.IO.Directory.Exists(outputRootPath))
    ///         {
    ///             throw new InvalidOperationException($"Directory '{outputRootPath}' must exist.");
    ///         }
    ///         // ********
    /// 
    ///         Console.WriteLine($"--------------------");
    ///         Console.WriteLine($"Testing Log Splitter");
    ///         Console.WriteLine($"--------------------");
    ///         Console.WriteLine($"{Environment.NewLine}Creating Log Splitter");
    ///         var logSplitter = CreateLogSplitter(outputRootPath);
    /// 
    ///         //
    ///         Console.WriteLine($"{Environment.NewLine}Saving Configuration Files");
    ///         System.IO.File.WriteAllBytes(System.IO.Path.Combine(outputRootPath, "LogSplitter.configuration.bin"), _ConfigurationBinarySerializer.Serialize(logSplitter.Configuration));
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogSplitter.configuration.csv"), _ConfigurationCsvSerializer.Serialize(logSplitter.Configuration).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogSplitter.configuration.ini"), _ConfigurationIniSerializer.Serialize(logSplitter.Configuration).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogSplitter.configuration.classic.ini"), (new dodSON.Core.Configuration.IniConfigurationSerializer()).Serialize(logSplitter.Configuration, false).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogSplitter.configuration.xml"), _ConfigurationXmlSerializer.Serialize(logSplitter.Configuration).ToString());
    /// 
    ///         // start
    ///         // ----------------------------------------------------------------
    /// 
    ///         // open log
    ///         Console.WriteLine($"{Environment.NewLine}    IsOpen={logSplitter.IsOpen}");
    ///         Console.WriteLine($"Opening Logs");
    ///         logSplitter.Open();
    ///         Console.WriteLine($"    IsOpen={logSplitter.IsOpen}");
    ///         Console.WriteLine($"    Log Count={logSplitter.LogCount}");
    /// 
    ///         // write logs
    ///         Console.Write($"{Environment.NewLine}Writing Logs");
    ///         DateTimeOffset startTimeStamp = DateTimeOffset.MinValue.ToLocalTime();
    ///         var cursorLeft = Console.CursorLeft;
    ///         var cursorTop = Console.CursorTop;
    ///         for (int index = 0; index &lt; LogsToWrite; index++)
    ///         {
    ///             var now = DateTimeOffset.Now;
    ///             var content = $"Index={index + 1}";
    ///             int eventId = index % 10;
    ///             ushort category = (ushort)index;
    ///             int logEntryIndex = index % 6;
    ///             string logSourceId = $"Test";
    ///             DateTimeOffset timeStamp = startTimeStamp.AddHours(index).ToLocalTime();
    ///             logSplitter.Write(dodSON.Core.Logging.LogEntryType.Information, logSourceId, content, eventId, category);
    ///             //
    ///             Console.SetCursorPosition(cursorLeft, cursorTop);
    ///             var percentageComplete = ((double)index / (double)LogsToWrite) * 100.0;
    ///             Console.WriteLine($"{new string(' ', 50)}\rWriting Log #{index + 1:N0} of {LogsToWrite:N0}        {percentageComplete:N1}%");
    ///             dodSON.Core.Threading.ThreadingHelper.Sleep(1);
    ///         }
    ///         Console.SetCursorPosition(cursorLeft, cursorTop);
    ///         Console.WriteLine($"{new string(' ', 50)}\rWriting Log #{LogsToWrite:N0} of {LogsToWrite:N0}        {100:N1}%");
    ///         Console.WriteLine($"{Environment.NewLine}    IsOpen={logSplitter.IsOpen}");
    ///         Console.WriteLine($"    Log Count={logSplitter.LogCount}");
    ///         Console.WriteLine($"{Environment.NewLine}--------------------");
    /// 
    /// 
    ///         // create and test the Log Combiner
    ///         Console.WriteLine($"Testing Log Combiner");
    ///         Console.WriteLine($"--------------------");
    ///         Console.WriteLine($"{Environment.NewLine}Creating Log Combiner");
    ///         dodSON.Core.Logging.ILogCombiner logCombiner = CreateLogCombiner(outputRootPath, logSplitter.LogFilters.Select(x =&gt; x.Log));
    /// 
    ///         // save log combiner configuration
    ///         Console.WriteLine($"{Environment.NewLine}Saving Log Combiner Configuration Files");
    ///         System.IO.File.WriteAllBytes(System.IO.Path.Combine(outputRootPath, "LogCombiner.configuration.bin"), _ConfigurationBinarySerializer.Serialize(logCombiner.Configuration));
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogCombiner.configuration.csv"), _ConfigurationCsvSerializer.Serialize(logCombiner.Configuration).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogCombiner.configuration.ini"), _ConfigurationIniSerializer.Serialize(logCombiner.Configuration).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogCombiner.configuration.classic.ini"), (new dodSON.Core.Configuration.IniConfigurationSerializer()).Serialize(logCombiner.Configuration, false).ToString());
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, "LogCombiner.configuration.xml"), _ConfigurationXmlSerializer.Serialize(logCombiner.Configuration).ToString());
    /// 
    ///         // read logs sorted by time (ascending)
    ///         Console.WriteLine($"{Environment.NewLine}Reading All Logs");
    ///         var outputLogger = CreateFileLogger(System.IO.Path.Combine(outputRootPath, "OutputLog.log"), true);
    ///         foreach (var log in from x in logCombiner.Read()
    ///                             orderby x.Timestamp ascending
    ///                             select x)
    ///         {
    ///             outputLogger.Write(log);
    ///         }
    ///         outputLogger.Close();
    /// 
    ///         // close logs
    ///         Console.WriteLine($"{Environment.NewLine}    IsOpen={logSplitter.IsOpen}");
    ///         Console.WriteLine($"Closing Logs");
    ///         logSplitter.Close();
    ///         Console.WriteLine($"    IsOpen={logSplitter.IsOpen}");
    ///         Console.WriteLine($"    Log Count={logSplitter.LogCount}");
    /// 
    ///         // ----------------------------------------------------------------
    /// 
    ///         Console.WriteLine($"{Environment.NewLine}--------------------");
    ///         Console.WriteLine($"Test Complete.");
    ///         Console.WriteLine($"{Environment.NewLine}================================");
    ///         Console.Write("press anykey&gt;");
    ///         Console.ReadKey(true);
    ///         Console.WriteLine();
    ///     }
    /// 
    ///     private static dodSON.Core.Logging.LogBase CreateFileLogger(string logFilename, bool writeLogEntriesUsingLocalTime)
    ///     {
    ///         // create actual logger
    ///         var dude = new dodSON.Core.Logging.FileEventLog.Log(logFilename, writeLogEntriesUsingLocalTime);
    ///         dude.Create();
    ///         return dude;
    ///     }
    /// 
    ///     private static dodSON.Core.Logging.ILogSplitter CreateLogSplitter(string rootPath)
    ///     {
    ///         // build LogFilters
    ///         var logFilters = new List&lt;dodSON.Core.Logging.LogFilter&gt;();
    ///         //
    ///         for (int i = 0; i &lt; 10; i++)
    ///         {
    ///             logFilters.Add(new dodSON.Core.Logging.LogFilter(new Splitter(i, i + 1), CreateFileLogger(System.IO.Path.Combine(rootPath, $"SplitLog #{i}.log"), true)));
    ///         }
    ///         // return log splitter
    ///         var clsLogResults = new dodSON.Core.Logging.LogSplitter(logFilters);
    ///         clsLogResults.Create();
    ///         return clsLogResults;
    ///     }
    /// 
    ///     private static dodSON.Core.Logging.ILogSplitter LoadLogSplitter(string rootPath, out string configurationFilename)
    ///     {
    ///         configurationFilename = System.IO.Path.Combine(rootPath, "LogSplitter.configuration.xml");
    ///         var llsResults = new dodSON.Core.Logging.LogSplitter(_ConfigurationXmlSerializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(configurationFilename))));
    ///         llsResults.Create();
    ///         return llsResults;
    ///     }
    /// 
    ///     private static dodSON.Core.Logging.ILogCombiner CreateLogCombiner(string rootPath, IEnumerable&lt;dodSON.Core.Logging.ILog&gt; logs)
    ///     {
    ///         return new dodSON.Core.Logging.LogCombiner(logs);
    ///     }
    /// 
    ///     private static dodSON.Core.Logging.ILogCombiner LoadLogCombiner(string llcRootPath, out string llcConfigurationFilename)
    ///     {
    ///         llcConfigurationFilename = System.IO.Path.Combine(llcRootPath, "LogCombiner.configuration.xml");
    ///         return new dodSON.Core.Logging.LogCombiner(_ConfigurationXmlSerializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(llcConfigurationFilename))));
    ///     }
    /// }
    /// 
    /// public class Splitter
    ///     : dodSON.Core.Logging.ILoggable,
    ///       dodSON.Core.Configuration.IConfigurable
    /// {
    ///     #region Ctor
    ///     private Splitter()
    ///     {
    ///     }
    /// 
    ///     public Splitter(int inclusiveMinimum, int exclusiveMax)
    ///         : this()
    ///     {
    ///         _InclusiveMinimum = inclusiveMinimum;
    ///         _ExclusiveMax = exclusiveMax;
    ///     }
    /// 
    ///     public Splitter(dodSON.Core.Configuration.IConfigurationGroup configuration)
    ///         : this()
    ///     {
    ///         // check 
    ///         if (configuration == null)
    ///         {
    ///             throw new ArgumentNullException(nameof(configuration));
    ///         }
    ///         if (configuration.Key != "LogEntryValidator")
    ///         {
    ///             throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"LogEntryValidator\". Configuration Key={configuration.Key}", nameof(configuration));
    ///         }
    ///         // _InclusiveMinimum
    ///         _InclusiveMinimum = (int)dodSON.Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "InclusiveMinimum", typeof(int)).Value;
    ///         // _ExclusiveMax
    ///         _ExclusiveMax = (int)dodSON.Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ExclusiveMax", typeof(int)).Value;
    ///     }
    ///     #endregion
    ///     #region Private Fields
    ///     private int _InclusiveMinimum;
    ///     private int _ExclusiveMax;
    ///     #endregion
    ///     #region dodSON.Core.Logging.ILoggable Methods
    ///     public bool IsValid(dodSON.Core.Logging.ILogEntry logEntry)
    ///     {
    ///         return ((logEntry.EventId &gt;= _InclusiveMinimum) &amp;&amp; (logEntry.EventId &lt; _ExclusiveMax));
    ///     }
    ///     #endregion
    ///     #region dodSON.Core.Configuration.IConfigurable Methods
    ///     /// &lt;summary&gt;
    ///     /// Will populate an &lt;see cref="Core.Configuration.IConfigurationGroup"/&gt; containing data needed to serialize the target object. 
    ///     /// &lt;/summary&gt;
    ///     public dodSON.Core.Configuration.IConfigurationGroup Configuration
    ///     {
    ///         get
    ///         {
    ///             var result = new dodSON.Core.Configuration.ConfigurationGroup("LogEntryValidator");
    ///             result.Items.Add("Type", this.GetType(), typeof(Type));
    ///             // _InclusiveMinimum
    ///             result.Items.Add("InclusiveMinimum", _InclusiveMinimum, _InclusiveMinimum.GetType());
    ///             // _ExclusiveMax
    ///             result.Items.Add("ExclusiveMax", _ExclusiveMax, _ExclusiveMax.GetType());
    ///             // 
    ///             return result;
    ///         }
    ///     }
    ///     #endregion
    /// } 
    /// </code>
    /// <para>The Final Screen Shot:</para>
    /// <br/>
    /// <code>
    /// // --------------------
    /// // Testing Log Splitter
    /// // --------------------
    /// // 
    /// // Creating Log Splitter
    /// // 
    /// // Saving Configuration Files
    /// // 
    /// //     IsOpen=False
    /// // Opening Logs
    /// //     IsOpen=True
    /// //     Log Count=0
    /// // 
    /// // Writing Log #500 of 500        100.0%
    /// // 
    /// //     IsOpen=True
    /// //     Log Count=500
    /// // 
    /// // --------------------
    /// // Testing Log Combiner
    /// // --------------------
    /// // 
    /// // Creating Log Combiner
    /// // 
    /// // Saving Log Combiner Configuration Files
    /// // 
    /// // Reading All Logs
    /// // 
    /// //     IsOpen=True
    /// // Closing Logs
    /// //     IsOpen=False
    /// //     Log Count=500
    /// // 
    /// // --------------------
    /// // Test Complete.
    /// // 
    /// // ================================
    /// // press anykey&gt; 
    /// </code>
    /// <para>The SplitLog #0.log File:</para>
    /// <br/>
    /// <code>
    /// // 636771457116076276; 2018-11-07 12:01:51 AM -06:00; Information; 0; 0; Test; Index=1
    /// // 636771457117026216; 2018-11-07 12:01:51 AM -06:00; Information; 0; 10; Test; Index=11
    /// // 636771457117486375; 2018-11-07 12:01:51 AM -06:00; Information; 0; 20; Test; Index=21
    /// // 636771457117956382; 2018-11-07 12:01:51 AM -06:00; Information; 0; 30; Test; Index=31
    /// // 636771457118386228; 2018-11-07 12:01:51 AM -06:00; Information; 0; 40; Test; Index=41
    /// // 636771457118886250; 2018-11-07 12:01:51 AM -06:00; Information; 0; 50; Test; Index=51
    /// // 636771457119356240; 2018-11-07 12:01:51 AM -06:00; Information; 0; 60; Test; Index=61
    /// // 636771457119916487; 2018-11-07 12:01:51 AM -06:00; Information; 0; 70; Test; Index=71
    /// // 636771457120396244; 2018-11-07 12:01:52 AM -06:00; Information; 0; 80; Test; Index=81
    /// // 636771457120996252; 2018-11-07 12:01:52 AM -06:00; Information; 0; 90; Test; Index=91
    /// // 636771457121536210; 2018-11-07 12:01:52 AM -06:00; Information; 0; 100; Test; Index=101
    /// // 636771457122056243; 2018-11-07 12:01:52 AM -06:00; Information; 0; 110; Test; Index=111
    /// // 636771457122636376; 2018-11-07 12:01:52 AM -06:00; Information; 0; 120; Test; Index=121
    /// // 636771457123176236; 2018-11-07 12:01:52 AM -06:00; Information; 0; 130; Test; Index=131
    /// // 636771457123646284; 2018-11-07 12:01:52 AM -06:00; Information; 0; 140; Test; Index=141
    /// // 636771457124096630; 2018-11-07 12:01:52 AM -06:00; Information; 0; 150; Test; Index=151
    /// // 636771457124656230; 2018-11-07 12:01:52 AM -06:00; Information; 0; 160; Test; Index=161
    /// // 636771457125206234; 2018-11-07 12:01:52 AM -06:00; Information; 0; 170; Test; Index=171
    /// // 636771457125936249; 2018-11-07 12:01:52 AM -06:00; Information; 0; 180; Test; Index=181
    /// // 636771457126426384; 2018-11-07 12:01:52 AM -06:00; Information; 0; 190; Test; Index=191
    /// // 636771457127066254; 2018-11-07 12:01:52 AM -06:00; Information; 0; 200; Test; Index=201
    /// // 636771457127606362; 2018-11-07 12:01:52 AM -06:00; Information; 0; 210; Test; Index=211
    /// // 636771457128066249; 2018-11-07 12:01:52 AM -06:00; Information; 0; 220; Test; Index=221
    /// // 636771457128506372; 2018-11-07 12:01:52 AM -06:00; Information; 0; 230; Test; Index=231
    /// // 636771457128986244; 2018-11-07 12:01:52 AM -06:00; Information; 0; 240; Test; Index=241
    /// // 636771457129506363; 2018-11-07 12:01:52 AM -06:00; Information; 0; 250; Test; Index=251
    /// // 636771457129996382; 2018-11-07 12:01:52 AM -06:00; Information; 0; 260; Test; Index=261
    /// // 636771457130456228; 2018-11-07 12:01:53 AM -06:00; Information; 0; 270; Test; Index=271
    /// // 636771457130916384; 2018-11-07 12:01:53 AM -06:00; Information; 0; 280; Test; Index=281
    /// // 636771457131606241; 2018-11-07 12:01:53 AM -06:00; Information; 0; 290; Test; Index=291
    /// // 636771457132136227; 2018-11-07 12:01:53 AM -06:00; Information; 0; 300; Test; Index=301
    /// // 636771457132656212; 2018-11-07 12:01:53 AM -06:00; Information; 0; 310; Test; Index=311
    /// // 636771457133216259; 2018-11-07 12:01:53 AM -06:00; Information; 0; 320; Test; Index=321
    /// // 636771457133696383; 2018-11-07 12:01:53 AM -06:00; Information; 0; 330; Test; Index=331
    /// // 636771457134196380; 2018-11-07 12:01:53 AM -06:00; Information; 0; 340; Test; Index=341
    /// // 636771457134946211; 2018-11-07 12:01:53 AM -06:00; Information; 0; 350; Test; Index=351
    /// // 636771457135476376; 2018-11-07 12:01:53 AM -06:00; Information; 0; 360; Test; Index=361
    /// // 636771457135936240; 2018-11-07 12:01:53 AM -06:00; Information; 0; 370; Test; Index=371
    /// // 636771457136396260; 2018-11-07 12:01:53 AM -06:00; Information; 0; 380; Test; Index=381
    /// // 636771457136856229; 2018-11-07 12:01:53 AM -06:00; Information; 0; 390; Test; Index=391
    /// // 636771457137746289; 2018-11-07 12:01:53 AM -06:00; Information; 0; 400; Test; Index=401
    /// // 636771457138336635; 2018-11-07 12:01:53 AM -06:00; Information; 0; 410; Test; Index=411
    /// // 636771457138816374; 2018-11-07 12:01:53 AM -06:00; Information; 0; 420; Test; Index=421
    /// // 636771457139306632; 2018-11-07 12:01:53 AM -06:00; Information; 0; 430; Test; Index=431
    /// // 636771457139806362; 2018-11-07 12:01:53 AM -06:00; Information; 0; 440; Test; Index=441
    /// // 636771457140296224; 2018-11-07 12:01:54 AM -06:00; Information; 0; 450; Test; Index=451
    /// // 636771457140746269; 2018-11-07 12:01:54 AM -06:00; Information; 0; 460; Test; Index=461
    /// // 636771457141291206; 2018-11-07 12:01:54 AM -06:00; Information; 0; 470; Test; Index=471
    /// // 636771457141821357; 2018-11-07 12:01:54 AM -06:00; Information; 0; 480; Test; Index=481
    /// // 636771457142341191; 2018-11-07 12:01:54 AM -06:00; Information; 0; 490; Test; Index=491 
    /// </code>
    /// <para>The SplitLog #7.log File:</para>
    /// <br/>
    /// <code>
    /// // 636771457116726238; 2018-11-07 12:01:51 AM -06:00; Information; 7; 7; Test; Index=8
    /// // 636771457117356388; 2018-11-07 12:01:51 AM -06:00; Information; 7; 17; Test; Index=18
    /// // 636771457117816237; 2018-11-07 12:01:51 AM -06:00; Information; 7; 27; Test; Index=28
    /// // 636771457118266377; 2018-11-07 12:01:51 AM -06:00; Information; 7; 37; Test; Index=38
    /// // 636771457118756368; 2018-11-07 12:01:51 AM -06:00; Information; 7; 47; Test; Index=48
    /// // 636771457119196231; 2018-11-07 12:01:51 AM -06:00; Information; 7; 57; Test; Index=58
    /// // 636771457119776219; 2018-11-07 12:01:51 AM -06:00; Information; 7; 67; Test; Index=68
    /// // 636771457120216228; 2018-11-07 12:01:52 AM -06:00; Information; 7; 77; Test; Index=78
    /// // 636771457120806230; 2018-11-07 12:01:52 AM -06:00; Information; 7; 87; Test; Index=88
    /// // 636771457121366217; 2018-11-07 12:01:52 AM -06:00; Information; 7; 97; Test; Index=98
    /// // 636771457121936213; 2018-11-07 12:01:52 AM -06:00; Information; 7; 107; Test; Index=108
    /// // 636771457122496383; 2018-11-07 12:01:52 AM -06:00; Information; 7; 117; Test; Index=118
    /// // 636771457123016234; 2018-11-07 12:01:52 AM -06:00; Information; 7; 127; Test; Index=128
    /// // 636771457123506377; 2018-11-07 12:01:52 AM -06:00; Information; 7; 137; Test; Index=138
    /// // 636771457123966658; 2018-11-07 12:01:52 AM -06:00; Information; 7; 147; Test; Index=148
    /// // 636771457124516230; 2018-11-07 12:01:52 AM -06:00; Information; 7; 157; Test; Index=158
    /// // 636771457125026239; 2018-11-07 12:01:52 AM -06:00; Information; 7; 167; Test; Index=168
    /// // 636771457125636225; 2018-11-07 12:01:52 AM -06:00; Information; 7; 177; Test; Index=178
    /// // 636771457126296382; 2018-11-07 12:01:52 AM -06:00; Information; 7; 187; Test; Index=188
    /// // 636771457126736251; 2018-11-07 12:01:52 AM -06:00; Information; 7; 197; Test; Index=198
    /// // 636771457127466239; 2018-11-07 12:01:52 AM -06:00; Information; 7; 207; Test; Index=208
    /// // 636771457127936223; 2018-11-07 12:01:52 AM -06:00; Information; 7; 217; Test; Index=218
    /// // 636771457128376229; 2018-11-07 12:01:52 AM -06:00; Information; 7; 227; Test; Index=228
    /// // 636771457128836394; 2018-11-07 12:01:52 AM -06:00; Information; 7; 237; Test; Index=238
    /// // 636771457129366367; 2018-11-07 12:01:52 AM -06:00; Information; 7; 247; Test; Index=248
    /// // 636771457129856211; 2018-11-07 12:01:52 AM -06:00; Information; 7; 257; Test; Index=258
    /// // 636771457130306232; 2018-11-07 12:01:53 AM -06:00; Information; 7; 267; Test; Index=268
    /// // 636771457130776372; 2018-11-07 12:01:53 AM -06:00; Information; 7; 277; Test; Index=278
    /// // 636771457131376248; 2018-11-07 12:01:53 AM -06:00; Information; 7; 287; Test; Index=288
    /// // 636771457132006248; 2018-11-07 12:01:53 AM -06:00; Information; 7; 297; Test; Index=298
    /// // 636771457132486384; 2018-11-07 12:01:53 AM -06:00; Information; 7; 307; Test; Index=308
    /// // 636771457133056246; 2018-11-07 12:01:53 AM -06:00; Information; 7; 317; Test; Index=318
    /// // 636771457133556232; 2018-11-07 12:01:53 AM -06:00; Information; 7; 327; Test; Index=328
    /// // 636771457134026215; 2018-11-07 12:01:53 AM -06:00; Information; 7; 337; Test; Index=338
    /// // 636771457134726245; 2018-11-07 12:01:53 AM -06:00; Information; 7; 347; Test; Index=348
    /// // 636771457135316376; 2018-11-07 12:01:53 AM -06:00; Information; 7; 357; Test; Index=358
    /// // 636771457135796374; 2018-11-07 12:01:53 AM -06:00; Information; 7; 367; Test; Index=368
    /// // 636771457136276237; 2018-11-07 12:01:53 AM -06:00; Information; 7; 377; Test; Index=378
    /// // 636771457136716415; 2018-11-07 12:01:53 AM -06:00; Information; 7; 387; Test; Index=388
    /// // 636771457137406257; 2018-11-07 12:01:53 AM -06:00; Information; 7; 397; Test; Index=398
    /// // 636771457138186631; 2018-11-07 12:01:53 AM -06:00; Information; 7; 407; Test; Index=408
    /// // 636771457138666711; 2018-11-07 12:01:53 AM -06:00; Information; 7; 417; Test; Index=418
    /// // 636771457139146384; 2018-11-07 12:01:53 AM -06:00; Information; 7; 427; Test; Index=428
    /// // 636771457139656361; 2018-11-07 12:01:53 AM -06:00; Information; 7; 437; Test; Index=438
    /// // 636771457140156370; 2018-11-07 12:01:54 AM -06:00; Information; 7; 447; Test; Index=448
    /// // 636771457140616220; 2018-11-07 12:01:54 AM -06:00; Information; 7; 457; Test; Index=458
    /// // 636771457141111344; 2018-11-07 12:01:54 AM -06:00; Information; 7; 467; Test; Index=468
    /// // 636771457141661196; 2018-11-07 12:01:54 AM -06:00; Information; 7; 477; Test; Index=478
    /// // 636771457142181360; 2018-11-07 12:01:54 AM -06:00; Information; 7; 487; Test; Index=488
    /// // 636771457142691348; 2018-11-07 12:01:54 AM -06:00; Information; 7; 497; Test; Index=498 
    /// </code>
    /// <para>The OutputLog.log File (partial):</para>
    /// <br/>
    /// <code>
    /// // 636771457116076276; 2018-11-07 12:01:51 AM -06:00; Information; 0; 0; Test; Index=1
    /// // 636771457116156241; 2018-11-07 12:01:51 AM -06:00; Information; 1; 1; Test; Index=2
    /// // 636771457116206246; 2018-11-07 12:01:51 AM -06:00; Information; 2; 2; Test; Index=3
    /// // 636771457116276226; 2018-11-07 12:01:51 AM -06:00; Information; 3; 3; Test; Index=4
    /// // 636771457116336241; 2018-11-07 12:01:51 AM -06:00; Information; 4; 4; Test; Index=5
    /// // 636771457116526247; 2018-11-07 12:01:51 AM -06:00; Information; 5; 5; Test; Index=6
    /// // 636771457116636258; 2018-11-07 12:01:51 AM -06:00; Information; 6; 6; Test; Index=7
    /// // 636771457116726238; 2018-11-07 12:01:51 AM -06:00; Information; 7; 7; Test; Index=8
    /// // 636771457116846304; 2018-11-07 12:01:51 AM -06:00; Information; 8; 8; Test; Index=9
    /// // 636771457116976244; 2018-11-07 12:01:51 AM -06:00; Information; 9; 9; Test; Index=10
    /// // 636771457117026216; 2018-11-07 12:01:51 AM -06:00; Information; 0; 10; Test; Index=11
    /// // 636771457117086226; 2018-11-07 12:01:51 AM -06:00; Information; 1; 11; Test; Index=12
    /// // 636771457117126379; 2018-11-07 12:01:51 AM -06:00; Information; 2; 12; Test; Index=13
    /// // 636771457117166382; 2018-11-07 12:01:51 AM -06:00; Information; 3; 13; Test; Index=14
    /// // 636771457117216262; 2018-11-07 12:01:51 AM -06:00; Information; 4; 14; Test; Index=15
    /// // 636771457117276385; 2018-11-07 12:01:51 AM -06:00; Information; 5; 15; Test; Index=16
    /// // 636771457117316380; 2018-11-07 12:01:51 AM -06:00; Information; 6; 16; Test; Index=17
    /// // 636771457117356388; 2018-11-07 12:01:51 AM -06:00; Information; 7; 17; Test; Index=18
    /// // 636771457117396263; 2018-11-07 12:01:51 AM -06:00; Information; 8; 18; Test; Index=19
    /// // 636771457117446232; 2018-11-07 12:01:51 AM -06:00; Information; 9; 19; Test; Index=20
    /// // 636771457117486375; 2018-11-07 12:01:51 AM -06:00; Information; 0; 20; Test; Index=21
    /// // 636771457117526376; 2018-11-07 12:01:51 AM -06:00; Information; 1; 21; Test; Index=22
    /// // 636771457117586220; 2018-11-07 12:01:51 AM -06:00; Information; 2; 22; Test; Index=23
    /// // 636771457117636387; 2018-11-07 12:01:51 AM -06:00; Information; 3; 23; Test; Index=24
    /// // 636771457117676379; 2018-11-07 12:01:51 AM -06:00; Information; 4; 24; Test; Index=25
    /// // 636771457117726229; 2018-11-07 12:01:51 AM -06:00; Information; 5; 25; Test; Index=26
    /// // 636771457117776244; 2018-11-07 12:01:51 AM -06:00; Information; 6; 26; Test; Index=27
    /// // 636771457117816237; 2018-11-07 12:01:51 AM -06:00; Information; 7; 27; Test; Index=28
    /// // 636771457117856370; 2018-11-07 12:01:51 AM -06:00; Information; 8; 28; Test; Index=29
    /// // 636771457117906245; 2018-11-07 12:01:51 AM -06:00; Information; 9; 29; Test; Index=30
    /// // 636771457117956382; 2018-11-07 12:01:51 AM -06:00; Information; 0; 30; Test; Index=31
    /// // 636771457117996373; 2018-11-07 12:01:51 AM -06:00; Information; 1; 31; Test; Index=32
    /// // .
    /// // .
    /// // .
    /// // 636771457141341227; 2018-11-07 12:01:54 AM -06:00; Information; 1; 471; Test; Index=472
    /// // 636771457141401236; 2018-11-07 12:01:54 AM -06:00; Information; 2; 472; Test; Index=473
    /// // 636771457141451199; 2018-11-07 12:01:54 AM -06:00; Information; 3; 473; Test; Index=474
    /// // 636771457141491349; 2018-11-07 12:01:54 AM -06:00; Information; 4; 474; Test; Index=475
    /// // 636771457141551350; 2018-11-07 12:01:54 AM -06:00; Information; 5; 475; Test; Index=476
    /// // 636771457141601201; 2018-11-07 12:01:54 AM -06:00; Information; 6; 476; Test; Index=477
    /// // 636771457141661196; 2018-11-07 12:01:54 AM -06:00; Information; 7; 477; Test; Index=478
    /// // 636771457141711204; 2018-11-07 12:01:54 AM -06:00; Information; 8; 478; Test; Index=479
    /// // 636771457141771345; 2018-11-07 12:01:54 AM -06:00; Information; 9; 479; Test; Index=480
    /// // 636771457141821357; 2018-11-07 12:01:54 AM -06:00; Information; 0; 480; Test; Index=481
    /// // 636771457141871192; 2018-11-07 12:01:54 AM -06:00; Information; 1; 481; Test; Index=482
    /// // 636771457141931206; 2018-11-07 12:01:54 AM -06:00; Information; 2; 482; Test; Index=483
    /// // 636771457141981367; 2018-11-07 12:01:54 AM -06:00; Information; 3; 483; Test; Index=484
    /// // 636771457142031360; 2018-11-07 12:01:54 AM -06:00; Information; 4; 484; Test; Index=485
    /// // 636771457142091369; 2018-11-07 12:01:54 AM -06:00; Information; 5; 485; Test; Index=486
    /// // 636771457142141356; 2018-11-07 12:01:54 AM -06:00; Information; 6; 486; Test; Index=487
    /// // 636771457142181360; 2018-11-07 12:01:54 AM -06:00; Information; 7; 487; Test; Index=488
    /// // 636771457142221217; 2018-11-07 12:01:54 AM -06:00; Information; 8; 488; Test; Index=489
    /// // 636771457142281208; 2018-11-07 12:01:54 AM -06:00; Information; 9; 489; Test; Index=490
    /// // 636771457142341191; 2018-11-07 12:01:54 AM -06:00; Information; 0; 490; Test; Index=491
    /// // 636771457142391195; 2018-11-07 12:01:54 AM -06:00; Information; 1; 491; Test; Index=492
    /// // 636771457142451353; 2018-11-07 12:01:54 AM -06:00; Information; 2; 492; Test; Index=493
    /// // 636771457142491345; 2018-11-07 12:01:54 AM -06:00; Information; 3; 493; Test; Index=494
    /// // 636771457142531344; 2018-11-07 12:01:54 AM -06:00; Information; 4; 494; Test; Index=495
    /// // 636771457142601197; 2018-11-07 12:01:54 AM -06:00; Information; 5; 495; Test; Index=496
    /// // 636771457142651353; 2018-11-07 12:01:54 AM -06:00; Information; 6; 496; Test; Index=497
    /// // 636771457142691348; 2018-11-07 12:01:54 AM -06:00; Information; 7; 497; Test; Index=498
    /// // 636771457142741205; 2018-11-07 12:01:54 AM -06:00; Information; 8; 498; Test; Index=499
    /// // 636771457142801349; 2018-11-07 12:01:54 AM -06:00; Information; 9; 499; Test; Index=500 
    /// </code>
    /// <para>The LogSplitter Configuration:</para>
    /// <br/>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="LogSplitter"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogSplitter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="LogFilters"&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="LogFilter 1"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #0.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;1&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 10"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #9.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;10&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;9&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 2"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #1.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;2&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;1&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 3"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #2.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;3&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;2&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 4"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #3.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;4&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;3&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 5"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #4.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;5&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;4&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 6"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #5.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;6&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;5&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 7"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #6.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;7&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;6&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 8"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #7.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;8&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;7&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogFilter 9"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogFilter, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="Log"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///                 &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #8.log&lt;/item&gt;
    ///                 &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///                 &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="LogEntryValidator"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ExclusiveMax" type="System.Int32"&gt;9&lt;/item&gt;
    ///                 &lt;item key="InclusiveMinimum" type="System.Int32"&gt;8&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSONCore_LogSplitterExample.Splitter, dodSONCore_LogSplitterExample, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt; 
    /// </code>
    /// <para>The LogCombiner Configuration:</para>
    /// <br/>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="LogCombiner"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.LogCombiner, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="Logs"&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="Log 1"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #0.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 10"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #9.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 2"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #1.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 3"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #2.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 4"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #3.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 5"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #4.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 6"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #5.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 7"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #6.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 8"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #7.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log 9"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\LogSplitter\SplitLog #8.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt; 
    /// </code>
    /// </example>
    public class LogSplitter
        : ILogSplitter
    {
        #region Ctor
        private LogSplitter()
        {
        }
        /// <summary>
        /// Instantiates a new LogSplitter with the specified <see cref="LogFilter"/>s.
        /// </summary>
        /// <param name="logFilters">Determines if a single log will be processed by multiple log filters.</param>
        public LogSplitter(IEnumerable<LogFilter> logFilters)
            : this()
        {
            if (logFilters == null)
            {
                throw new ArgumentNullException(nameof(logFilters));
            }
            _LogFilters = new List<LogFilter>(logFilters);
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public LogSplitter(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Log")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Log\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // group: LogFilters
            if (configuration.ContainsKey("LogFilters"))
            {
                foreach (var item in configuration["LogFilters"])
                {
                    ((Configuration.IConfigurationGroupAdvanced)item).SetKey("LogFilter");
                    _LogFilters.Add(new LogFilter(item));
                }
            }
        }

        #endregion
        #region Private Fields
        private readonly List<LogFilter> _LogFilters = new List<LogFilter>();
        #endregion
        #region ILogSplitter Methods

        // #### ILOGSPLITTER
        /// <summary>
        /// Returns all <see cref=" LogFilter"/>s in the <see cref="ILogSplitter"/>.
        /// </summary>
        public IEnumerable<LogFilter> LogFilters => _LogFilters;

        // #### ILOG
        /// <summary>
        /// Will create empty logs for all of the Logs defines by the <see cref="LogFilters"/>.
        /// </summary>
        public void Create()
        {
            lock (SyncObject)
            {
                foreach (var logFilter in InternalLogFilters(false))
                {
                    logFilter.Log.Create();
                }
            }
        }
        /// <summary>
        /// Will delete the log.
        /// </summary>
        /// <remarks>This will permanently delete all log entries in the log; this action <i>cannot</i> be undone.</remarks>
        public void Delete()
        {
            lock (SyncObject)
            {
                foreach (var logFilter in InternalLogFilters(false))
                {
                    logFilter.Log.Delete();
                }
            }
        }
        /// <summary>
        /// Opens an existing log, or creates a new one if log not found.
        /// </summary>
        public void Open()
        {
            lock (SyncObject)
            {
                foreach (var logFilter in InternalLogFilters(false))
                {
                    logFilter.Log.Open();
                }
            }
        }
        /// <summary>
        /// Closes the log.
        /// </summary>
        public void Close()
        {
            lock (SyncObject)
            {
                foreach (var logFilter in InternalLogFilters(false))
                {
                    logFilter.Log.Close();
                }
            }
        }
        /// <summary>
        /// Will remove all log entries from the log.
        /// </summary>
        /// <remarks>
        /// This will permanently delete all log entries from the log; this action <i>cannot</i> be undone.
        /// <para>This only clears the log, it will <i>not</i> delete it.</para>
        /// </remarks>
        /// <returns>The number of log entries removed.</returns>
        public int Clear()
        {
            var count = 0;
            lock (SyncObject)
            {
                count = 0;
                foreach (var logFilter in InternalLogFilters(false))
                {
                    count += logFilter.Log.Clear();
                }
            }
            return count;
        }

        // #### ILOG INFORMATION
        /// <summary>
        /// Returns the unique id for this <see cref="ILog"/>.
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString("N");
        /// <summary>
        /// Get the current statistics for the log.
        /// </summary>
        public ILogStatistics Statistics => new LogStatistics();
        /// <summary>
        /// Returns an object that can be used for synchronizing access to the underlying logging system across multiple threads.
        /// </summary>
        public object SyncObject { get; } = new object();
        /// <summary>
        /// Gets a value indicating whether the log exists. <b>True</b> indicates it was found, <b>false</b> indicates it was <i>not</i> found.
        /// </summary>
        public bool Exists => true;
        /// <summary>
        /// Returns whether the log has been opened. <b>True</b> if the log is open, otherwise, <b>false</b>.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                lock (SyncObject)
                {
                    foreach (var logFilter in InternalLogFilters(false))
                    {
                        if (!logFilter.Log.IsOpen)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
        }
        /// <summary>
        /// Gets the number of log entries contained in the log.
        /// </summary>
        public int LogCount => InternalLogFilters(false).Select(x => x.Log.LogCount).Aggregate(0, (total, count) => total += count);

        // #### ILOG READ
        /// <summary>
        /// Reads each log entry from the log.
        /// </summary>
        /// <returns>Returns all log entries in the log.</returns>
        public IEnumerable<ILogEntry> Read() => Read(x => true);
        /// <summary>
        /// Returns each log entry in the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.
        /// </summary>
        /// <param name="logEntryFilterPredicate"></param>
        /// <returns>Every log entry from the log which returns <b>true</b> from the <paramref name="logEntryFilterPredicate"/>.</returns>
        public IEnumerable<ILogEntry> Read(Func<ILogEntry, bool> logEntryFilterPredicate)
        {
            lock (SyncObject)
            {
                // read all logs (once)
                foreach (var logFilter in InternalLogFilters(false))
                {
                    foreach (var logEntry in logFilter.Log.Read(logEntryFilterPredicate))
                    {
                        yield return logEntry;
                    }
                }
            }
        }

        // #### ILOG WRITE
        /// <summary>
        /// Writes a log entry to the log.
        /// </summary>
        /// <param name="logEntry">The log entry to write to this log.</param>
        public void Write(ILogEntry logEntry) => Write(new ILogEntry[] { logEntry });
        /// <summary>
        /// Writes a group of log entries to the log.
        /// </summary>
        /// <param name="logs">The log entries to write to this log.</param>
        public void Write(IEnumerable<ILogEntry> logs)
        {
            lock (SyncObject)
            {
                // iterate through all log entries
                foreach (var logEntry in logs)
                {
                    // write to all log filters
                    foreach (var logFilter in InternalLogFilters(true))
                    {
                        // if the log filter thinks the log entry is valid, write it to the log filter's log
                        if (logFilter.LogEntryValidator.IsValid(logEntry))
                        {
                            logFilter.Log.Write(logEntry);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Writes all of the logs in <paramref name="logs"/> to this log.
        /// </summary>
        /// <param name="logs">The source of the logs to write.</param>
        public void Write(Logs logs) => Write(logs as IEnumerable<ILogEntry>);
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        /// <param name="category">The category number for this log entry.</param>
        public void Write(LogEntryType entryType, string sourceId, string message, int eventId, ushort category) => Write(new LogEntry(entryType, sourceId, message, eventId, category));
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        /// <param name="eventId">The event identifier for this log entry.</param>
        public void Write(LogEntryType entryType, string sourceId, string message, int eventId) => Write(new LogEntry(entryType, sourceId, message, eventId));
        /// <summary>
        /// Creates a log entry with the specified information and writes it to this log.
        /// </summary>
        /// <param name="entryType">The type of event for this log entry.</param>
        /// <param name="sourceId">A user-defined id which represents the source of the log entry.</param>
        /// <param name="message">The message for this log entry.</param>
        public void Write(LogEntryType entryType, string sourceId, string message) => Write(new LogEntry(entryType, sourceId, message));
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Log");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // group: LogFilters
                var logFilters = result.Add("LogFilters");
                var count = 0;
                // write all log filters to the configuration
                foreach (var item in InternalLogFilters(true))
                {
                    // clone the item, change its key, save clone to dependencies list
                    // you do not want to change the key in the original item
                    var clone = Core.Configuration.XmlConfigurationSerializer.Clone(item.Configuration);
                    ((Configuration.IConfigurationGroupAdvanced)clone).SetKey($"LogFilter {++count}");
                    logFilters.Add(clone);
                }
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Will return all Log Filters without repetition.
        /// </summary>
        /// <param name="allowMultipleLogProcessing">Determines if a single log will be processed by multiple log filters.</param>
        private IEnumerable<LogFilter> InternalLogFilters(bool allowMultipleLogProcessing)
        {
            // keep from processing a log more than once
            var processedLogs = new List<string>();
            // iterate through all log filters
            foreach (var logFilter in LogFilters)
            {
                if (allowMultipleLogProcessing)
                {
                    // iterate through all log entries
                    yield return logFilter;
                }
                else
                {
                    if (!processedLogs.Contains(logFilter.Log.Id))
                    {
                        processedLogs.Add(logFilter.Log.Id);
                        // iterate through all log entries
                        yield return logFilter;
                    }
                }
            }
        }
        #endregion
    }
}
