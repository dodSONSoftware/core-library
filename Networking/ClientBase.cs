using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides the base class for a communication channel client.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example will load a server from a configuration file and start the server.
    /// <br/>
    /// Once started, several clients will be loaded from configuration files and registered with the server. 
    /// A thread for each client will use that client to send messages to the server.
    /// <br/>
    /// The console application will then loop until the escape key is press, displaying some networking statistics.
    /// There are several other commands available, feel free to play with them.
    /// <br/>
    /// Two logs will be generated, one for server activity and the other for all of the messages received.
    /// The server log contains the results of any requested commands.
    /// </para>
    /// <note type="note">
    /// To create the server and client configuration files for this example, see the example in <see cref="ServerBase"/>.
    /// </note>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// // ********************************
    /// private static dodSON.Core.Logging.ICachedLog ServerLog;
    /// private static string ServerLogFilename;
    /// private static dodSON.Core.Logging.ICachedLog ClientLog;
    /// private static string ClientLogFilename;
    /// 
    /// private static long _LastServerIncomingBytes;
    /// private static long _LastServerOutgoingBytes;
    /// private static long _LastServerIncomingEnvelopes;
    /// private static long _LastServerOutgoingEnvelopes;
    /// 
    /// private class ClientStatsInformation
    /// {
    ///     public long LastIncomingBytes;
    ///     public long LastIncomingEnvelopes;
    ///     public long LastIncomingMessages;
    ///     public long LastOutgoingBytes;
    ///     public long LastOutgoingEnvelopes;
    ///     public long LastOutgoingMessages;
    /// }
    /// private static Dictionary&lt;string, ClientStatsInformation&gt; _ClientStatsInfoList = new Dictionary&lt;string, ClientStatsInformation&gt;();
    /// private static DateTime _LastDisplayEvent = DateTime.Now;
    /// // ********************************
    /// 
    /// private static void Main(string[] args)
    /// 
    /// {
    ///     // ################################################
    ///     // ######## BE SURE TO CHANGE THESE VALUES ########
    ///     // ########
    ///     // ######## THE outputRootPath DIRECTORY MUST EXIST AND CONTAIN SERVER AND CLIENT CONFIGURATION FILES ########
    ///     // ######## SEE THE NOTES ACCOMPANYING THIS EXAMPLE FOR AN EXPLANATION OF HOW TO CREATE THE REQUIRED FILES ########
    /// 
    ///     var outputRootPath = @"C:\(WORKING)\Dev\Networking";
    ///     var configurationSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///     var configurationFileExtenstion = "xml";
    /// 
    ///     // ################################################
    /// 
    ///     // ******** Validate outputRootPath is Available and Clear
    /// 
    ///     if (!System.IO.Directory.Exists(outputRootPath))
    ///     {
    ///         throw new Exception($"Missing outputRootPath={outputRootPath} Folder. See the notes accompanying this example for an explanation of how to create the required files.");
    ///     }
    /// 
    ///     // ******** Create and Open Server Log
    /// 
    ///     ServerLogFilename = System.IO.Path.Combine(outputRootPath, @"ServerLog.txt");
    ///     var ServerWriteLogEntriesUsingLocalTime = true;
    ///     var serverLogActual = new dodSON.Core.Logging.FileEventLog.Log(ServerLogFilename, ServerWriteLogEntriesUsingLocalTime);
    ///     var serverAutoFlushLogs = true;
    ///     var serverFlushMaximumLogs = 10;
    ///     var serverFlushTimeLimit = TimeSpan.FromSeconds(5);
    ///     ServerLog = new dodSON.Core.Logging.CachedLog(serverLogActual, serverAutoFlushLogs, serverFlushMaximumLogs, serverFlushTimeLimit);
    ///     ServerLog.Open();
    ///     ServerLog.Clear();
    /// 
    ///     // ******** Create and Open Client Log
    /// 
    ///     ClientLogFilename = System.IO.Path.Combine(outputRootPath, @"ClientLog.txt");
    ///     var clientWriteLogEntriesUsingLocalTime = true;
    ///     var autoTruncateLogFile = true;
    ///     var maxLogSizeBytes = dodSON.Core.Common.ByteCountHelper.FromMegabytes(10);
    ///     var logsToRetain = 0;
    ///     var clientLogActual = new dodSON.Core.Logging.FileEventLog.Log(ClientLogFilename, clientWriteLogEntriesUsingLocalTime, autoTruncateLogFile, maxLogSizeBytes, logsToRetain);
    ///     var clientAutoFlushLogs = true;
    ///     var clientFlushMaximumLogs = 1000;
    ///     var clientFlushTimeLimit = TimeSpan.FromSeconds(20);
    ///     ClientLog = new dodSON.Core.Logging.CachedLog(clientLogActual, clientAutoFlushLogs, clientFlushMaximumLogs, clientFlushTimeLimit);
    ///     ClientLog.Open();
    ///     ClientLog.Clear();
    /// 
    ///     // ******** 
    /// 
    ///     var logThreadWorker = new dodSON.Core.Threading.ThreadWorker(TimeSpan.FromSeconds(1),
    ///                                                                  (ct) =&gt;
    ///                                                                  {
    ///                                                                      if (ServerLog.IsFlushable) { ServerLog.FlushLogs(); }
    ///                                                                      if (ClientLog.IsFlushable) { ClientLog.FlushLogs(); }
    ///                                                                  });
    ///     logThreadWorker.Start();
    /// 
    ///     // ******** Load Server
    /// 
    ///     var serverConfigurationFilename = System.IO.Path.Combine(outputRootPath, $@"Server.configuration.{configurationFileExtenstion}");
    ///     var server = LoadServer(serverConfigurationFilename, configurationSerializer);
    /// 
    ///     // ******** Open Server
    /// 
    ///     var clientThreadWorkers = new List&lt;dodSON.Core.Threading.ThreadWorker&gt;();
    ///     if (server.Open(out Exception serverOpenException))
    ///     {
    ///         // ******** Load and Open Clients
    ///         foreach (var filename in System.IO.Directory.GetFiles(outputRootPath, $"Client-*.{configurationFileExtenstion}", System.IO.SearchOption.TopDirectoryOnly))
    ///         {
    ///             var client = LoadClient(filename, configurationSerializer);
    ///             if (client.Open(out Exception client1OpenException) != dodSON.Core.Networking.ChannelStates.Open) { Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open client. {client1OpenException?.Message}"); }
    ///             var clientMessageFrequency = TimeSpan.FromSeconds(0.1);
    ///             var clientThreadWorker = new dodSON.Core.Threading.ThreadWorker(clientMessageFrequency,
    ///                                                                            (cancelToken) =&gt;
    ///                                                                            {
    ///                                                                                if (client.State == dodSON.Core.Networking.ChannelStates.Open)
    ///                                                                                {
    ///                                                                                    var messageStr = $"Message from {client.Id}.";
    ///                                                                                    client.SendMessage(new dodSON.Core.Networking.Message(Guid.NewGuid().ToString(),
    ///                                                                                                                                           client.Id,
    ///                                                                                                                                           "",
    ///                                                                                                                                           new dodSON.Core.Networking.PayloadTypeInfo(typeof(string)),
    ///                                                                                                                                           (new dodSON.Core.Converters.TypeSerializer&lt;string&gt;()).ToByteArray(messageStr)));
    ///                                                                                }
    ///                                                                            });
    ///             // add client worker to list
    ///             clientThreadWorkers.Add(clientThreadWorker);
    ///         }
    /// 
    ///         // ******** Start Worker Threads
    /// 
    ///         foreach (var workerThread in clientThreadWorkers)
    ///         {
    ///             workerThread.Start();
    ///         }
    /// 
    ///         // ******** Loop Until User Terminated
    /// 
    ///         var startTime = DateTimeOffset.Now;
    ///         var displayRefreshTimer = System.Diagnostics.Stopwatch.StartNew();
    ///         var loopIt = true;
    ///         while (loopIt)
    ///         {
    ///             if (displayRefreshTimer.Elapsed &gt; TimeSpan.FromSeconds(1))
    ///             {
    ///                 displayRefreshTimer.Restart();
    ///                 RefreshDisplay(startTime, server);
    ///             }
    ///             if (Console.KeyAvailable)
    ///             {
    ///                 switch (Console.ReadKey(true).Key)
    ///                 {
    ///                     case ConsoleKey.Escape:
    ///                         loopIt = false;
    ///                         break;
    ///                     case ConsoleKey.F1:
    ///                         var cancelToken = new dodSON.Core.Threading.ThreadCancelToken();
    ///                         server.PingAllClients(cancelToken);
    ///                         break;
    ///                     case ConsoleKey.F4:
    ///                         server.RestartAllClients(5, TimeSpan.FromSeconds(5));
    ///                         break;
    ///                     default:
    ///                         break;
    ///                 }
    ///             }
    ///         }
    /// 
    ///         // ******** Final Display
    /// 
    ///         displayRefreshTimer.Stop();
    ///         RefreshDisplay(startTime, server);
    /// 
    ///         // ******** Stop Worker Threads
    /// 
    ///         foreach (var workerThread in clientThreadWorkers)
    ///         {
    ///             workerThread.Stop();
    ///         }
    ///         logThreadWorker.Stop();
    /// 
    ///         // ******** Close Server
    /// 
    ///         if (server.Close(out Exception serverCloseException) != dodSON.Core.Networking.ChannelStates.Closed) { Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot close server. {serverCloseException?.Message}"); }
    ///     }
    ///     else
    ///     {
    ///         Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open server. {serverOpenException?.Message}");
    ///     }
    /// 
    ///     // ******** Close Logs
    /// 
    ///     ClientLog.Close();
    ///     ServerLog.Close();
    /// 
    ///     // ****************************************************************************************************
    ///     Console.WriteLine("================================");
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    ///     Console.WriteLine();
    /// }
    /// 
    /// private static void RefreshDisplay(DateTimeOffset startTime,
    ///                                    dodSON.Core.Networking.IServer server)
    /// {
    ///     Console.Clear();
    ///     Console.WriteLine("Networking Example");
    ///     Console.WriteLine("dodSON Software Core Library");
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Server Log={ServerLogFilename}");
    ///     Console.WriteLine($"Client Log={ClientLogFilename}");
    ///     Console.WriteLine();
    ///     DisplayNetworkStats(startTime, server);
    ///     Console.WriteLine();
    ///     Console.WriteLine("press [F1]=Ping");
    ///     Console.WriteLine("press [F4]=Restart Clients");
    ///     Console.WriteLine("press [Esc]=Exit");
    /// }
    /// 
    /// private static dodSON.Core.Networking.IServer LoadServer(string filename,
    ///                                                          dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; serializer)
    /// {
    ///     if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException(filename, filename); }
    ///     var configuration = serializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(filename)));
    ///     var server = (dodSON.Core.Networking.IServer)dodSON.Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfiguration(null, configuration);
    ///     server.ActivityLogsEvent += Server_ActivityLogsEventHandler;
    ///     return server;
    /// }
    /// 
    /// private static dodSON.Core.Networking.IClient LoadClient(string filename,
    ///                                                          dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; serializer)
    /// {
    ///     if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException(filename, filename); }
    ///     var configuration = serializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(filename)));
    ///     var client = (dodSON.Core.Networking.IClient)dodSON.Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfiguration(null, configuration);
    ///     client.MessageBus += SharedClient_MessageBusHandler;
    ///     return client;
    /// }
    /// 
    /// private static void Server_ActivityLogsEventHandler(object sender, dodSON.Core.Networking.ActivityLogsEventArgs e)
    /// {
    ///     if (e.Header != "RequestAllClientsTransportStatistics") { ServerLog.Write(e.Logs); }
    /// }
    /// 
    /// private static void SharedClient_MessageBusHandler(object sender, dodSON.Core.Networking.MessageEventArgs e)
    /// {
    ///     if (e.Message.TypeInfo.TypeName == typeof(string).AssemblyQualifiedName)
    ///     {
    ///         var message = (new dodSON.Core.Converters.TypeSerializer&lt;string&gt;()).FromByteArray(e.Message.Payload);
    ///         var senderAsClient = sender as dodSON.Core.Networking.IClient;
    ///         ClientLog.Write(dodSON.Core.Logging.LogEntryType.Information, senderAsClient?.Id, message);
    ///     }
    /// }
    /// 
    /// private static void DisplayNetworkStats(DateTimeOffset startTime, dodSON.Core.Networking.IServer server)
    /// {
    ///     var allStats = server.RequestAllClientsTransportStatistics();
    ///     var forDuration = DateTime.Now - _LastDisplayEvent;
    /// 
    ///     Console.WriteLine($"Uri     = {server.AddressUri}");
    ///     Console.WriteLine($"Runtime = {DateTimeOffset.Now - startTime}");
    ///     Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}");
    ///     // 
    ///     Console.WriteLine($"Server Incoming:");
    ///     var incomingEnvelopesPerSecond = allStats.ServerStatistics.IncomingAverageEnvelopesPerSecond(_LastServerIncomingEnvelopes, forDuration);
    ///     Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.IncomingEnvelopes}, envelopes/second = {incomingEnvelopesPerSecond:N2}");
    ///     var incomingBytesPerSecond = allStats.ServerStatistics.IncomingAverageBytesPerSecond(_LastServerIncomingBytes, forDuration);
    ///     Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)incomingBytesPerSecond)} ({incomingBytesPerSecond:N2})");
    ///     Console.WriteLine($"Server Outgoing:");
    ///     var outgoingEnvelopesPerSecond = allStats.ServerStatistics.OutgoingAverageEnvelopesPerSecond(_LastServerOutgoingEnvelopes, forDuration);
    ///     Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.OutgoingEnvelopes}, envelopes/second = {outgoingEnvelopesPerSecond:N2}");
    ///     var outgoingBytesPerSecond = allStats.ServerStatistics.OutgoingAverageBytesPerSecond(_LastServerOutgoingBytes, forDuration);
    ///     Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.OutgoingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)outgoingBytesPerSecond)} ({outgoingBytesPerSecond:N2})");
    ///     // 
    ///     _LastDisplayEvent = DateTime.Now;
    ///     _LastServerIncomingEnvelopes = allStats.ServerStatistics.IncomingEnvelopes;
    ///     _LastServerOutgoingEnvelopes = allStats.ServerStatistics.OutgoingEnvelopes;
    ///     _LastServerIncomingBytes = allStats.ServerStatistics.IncomingBytes;
    ///     _LastServerOutgoingBytes = allStats.ServerStatistics.OutgoingBytes;
    ///     // 
    ///     foreach (var client in allStats.AllClientsStatistics)
    ///     {
    ///         var clientStatsInfo = new ClientStatsInformation();
    ///         if (_ClientStatsInfoList.ContainsKey(client.ClientServerId)) { clientStatsInfo = _ClientStatsInfoList[client.ClientServerId]; }
    ///         else { _ClientStatsInfoList.Add(client.ClientServerId, clientStatsInfo); }
    ///         // 
    ///         Console.WriteLine();
    ///         Console.WriteLine($"{client.ClientServerId} Incoming:");
    ///         var clientIncomingMessagesPerSecond = client.IncomingAverageMessagesPerSecond(clientStatsInfo.LastIncomingMessages, forDuration);
    ///         Console.WriteLine($"    Messages  = {client.IncomingMessages}, messages/second = {clientIncomingMessagesPerSecond:N2}");
    ///         var clientIncomingEnvelopesPerSecond = client.IncomingAverageEnvelopesPerSecond(clientStatsInfo.LastIncomingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {client.IncomingEnvelopes}, envelopes/second = {clientIncomingEnvelopesPerSecond:N2}");
    ///         var clientIncomingBytesPerSecond = client.IncomingAverageBytesPerSecond(clientStatsInfo.LastIncomingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientIncomingBytesPerSecond)} ({clientIncomingBytesPerSecond:N2})");
    ///         Console.WriteLine($"{client.ClientServerId} Outgoing:");
    ///         var clientOutgoingMessagesPerSecond = client.OutgoingAverageMessagesPerSecond(clientStatsInfo.LastOutgoingMessages, forDuration);
    ///         Console.WriteLine($"    Messages  = {client.OutgoingMessages}, messages/second = {clientOutgoingMessagesPerSecond:N2}");
    ///         var clientOutgoingEnvelopesPerSecond = client.OutgoingAverageEnvelopesPerSecond(clientStatsInfo.LastOutgoingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {client.OutgoingEnvelopes}, envelopes/second = {clientOutgoingEnvelopesPerSecond:N2}");
    ///         var clientOutgoingBytesPerSecond = client.OutgoingAverageBytesPerSecond(clientStatsInfo.LastOutgoingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.OutgoingBytes)}, Bytes/seconds = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientOutgoingBytesPerSecond)} ({clientOutgoingBytesPerSecond:N2})");
    ///         // 
    ///         clientStatsInfo.LastIncomingBytes = client.IncomingBytes;
    ///         clientStatsInfo.LastIncomingEnvelopes = client.IncomingEnvelopes;
    ///         clientStatsInfo.LastIncomingMessages = client.IncomingMessages;
    ///         clientStatsInfo.LastOutgoingBytes = client.OutgoingBytes;
    ///         clientStatsInfo.LastOutgoingEnvelopes = client.OutgoingEnvelopes;
    ///         clientStatsInfo.LastOutgoingMessages = client.OutgoingMessages;
    ///     }
    ///     Console.WriteLine($"{Environment.NewLine}--------------------------------");
    /// }
    /// </code>
    /// <para>The final screen shot.</para>
    /// <code>
    /// // Networking Example
    /// // dodSON Software Core Library
    /// // 
    /// // Server Log=C:\(WORKING)\Dev\Networking\ServerLog.txt
    /// // Client Log=C:\(WORKING)\Dev\Networking\ClientLog.txt
    /// // 
    /// // Uri     = net.pipe://localhost/TestServerChannel-49152
    /// // Runtime = 00:00:32.4809686
    /// // 
    /// // --------------------------------
    /// // 
    /// // Server Incoming:
    /// //     Envelopes = 963, envelopes/second = 33.48
    /// //     Bytes     = 632 KB, Bytes/second = 22 KB (22,498.86)
    /// // Server Outgoing:
    /// //     Envelopes = 3852, envelopes/second = 133.92
    /// //     Bytes     = 2.5 MB, Bytes/second = 88 KB (89,995.44)
    /// // 
    /// // Alpha Incoming:
    /// //     Messages  = 963, messages/second = 33.48
    /// //     Envelopes = 963, envelopes/second = 33.48
    /// //     Bytes     = 632 KB, Bytes/second = 22 KB (22,498.86)
    /// // Alpha Outgoing:
    /// //     Messages  = 321, messages/second = 11.16
    /// //     Envelopes = 321, envelopes/second = 11.16
    /// //     Bytes     = 211 KB, Bytes/seconds = 7 KB (7,499.62)
    /// // 
    /// // Beta Incoming:
    /// //     Messages  = 963, messages/second = 33.48
    /// //     Envelopes = 963, envelopes/second = 33.48
    /// //     Bytes     = 632 KB, Bytes/second = 22 KB (22,498.86)
    /// // Beta Outgoing:
    /// //     Messages  = 321, messages/second = 11.16
    /// //     Envelopes = 321, envelopes/second = 11.16
    /// //     Bytes     = 211 KB, Bytes/seconds = 7 KB (7,499.62)
    /// // 
    /// // Gamma Incoming:
    /// //     Messages  = 963, messages/second = 33.48
    /// //     Envelopes = 963, envelopes/second = 33.48
    /// //     Bytes     = 632 KB, Bytes/second = 22 KB (22,498.86)
    /// // Gamma Outgoing:
    /// //     Messages  = 321, messages/second = 11.16
    /// //     Envelopes = 321, envelopes/second = 11.16
    /// //     Bytes     = 211 KB, Bytes/seconds = 7 KB (7,499.62)
    /// // 
    /// // --------------------------------
    /// // 
    /// // press [F1]=Ping
    /// // press [F4]=Restart Clients
    /// // press [Esc]=Exit
    /// // ================================
    /// // press anykey>
    /// </code>
    /// <para>The Server Log.</para>
    /// <code>
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Opening Server.
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Server Id=TestServerId
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Type=[dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Uri=net.pipe://localhost/TestServerChannel-49152
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; IP Address=localhost
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Name=TestServerChannel
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Port=49152
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; Override Types: (1)
    /// // 2017-10-04 12:26:21 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Registering Client=ServerInternalClient_a4988631-5311-4c7d-b489-c34dodSON051
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receive Own Messages=True
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receivable Types: (0)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Transmittable Types: (0)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Completed Registering Client=ServerInternalClient_a4988631-5311-4c7d-b489-c34dodSON051
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Server Opened Successfully.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Registering Client=Alpha
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receive Own Messages=False
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receivable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Transmittable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Completed Registering Client=Alpha
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Registering Client=Beta
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receive Own Messages=False
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receivable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Transmittable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Completed Registering Client=Beta
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Registering Client=Gamma
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receive Own Messages=False
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Receivable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Transmittable Types: (1)
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Server; Completed Registering Client=Gamma
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; Pinging All Clients: (4)
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; #1=ServerInternalClient_a4988631-5311-4c7d-b489-c34dodSON051, Round Trip=00:00:00.0030585, Date Started=2017-10-04 12:26:21 AM -05:00, Age=00:00:22.2119261
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; #2=Alpha, Round Trip=00:00:00.0012117, Date Started=2017-10-04 12:26:22 AM -05:00, Age=00:00:21.9769251
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; #3=Beta, Round Trip=00:00:00.0009926, Date Started=2017-10-04 12:26:22 AM -05:00, Age=00:00:21.8099113
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; #4=Gamma, Round Trip=00:00:00.0008949, Date Started=2017-10-04 12:26:22 AM -05:00, Age=00:00:21.6709250
    /// // 2017-10-04 12:26:44 AM -05:00; Information; 0; 0; Server; Completed Pinging All Clients.
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Closing Server: (4)
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Unregistering Client=Alpha, Date Started=2017-10-04 12:26:22 AM, Age=00:00:32.8382216
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Unregistering Client=ServerInternalClient_a4988631-5311-4c7d-b489-c34dodSON051, Date Started=2017-10-04 12:26:22 AM, Age=00:00:33.0242212
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Unregistering Client=Beta, Date Started=2017-10-04 12:26:22 AM, Age=00:00:32.6872076
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Unregistering Client=Gamma, Date Started=2017-10-04 12:26:22 AM, Age=00:00:32.5411913
    /// // 2017-10-04 12:26:55 AM -05:00; Information; 0; 0; Server; Server Closed Successfully.
    /// </code>
    /// <para>Part of the Client Log.</para>
    /// <code>
    /// // .
    /// // .
    /// // .
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Gamma.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Alpha; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Gamma; Message from Beta.
    /// // 2017-10-04 12:26:22 AM -05:00; Information; 0; 0; Beta; Message from Gamma.
    /// // 2017-10-04 12:26:23 AM -05:00; Information; 0; 0; Alpha; Message from Alpha.
    /// // 2017-10-04 12:26:23 AM -05:00; Information; 0; 0; Gamma; Message from Alpha.
    /// // 2017-10-04 12:26:23 AM -05:00; Information; 0; 0; Beta; Message from Alpha.
    /// // .
    /// // .
    /// // .
    /// // .
    /// </code>
    /// <para>The Server Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Server"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;TestServerChannel&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ServerConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;TestServerId&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="OverrideTypesFilter" /&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="TransportController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportController, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="RegistrationController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.EncryptedRegistrationController, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ChallengeController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/+aPQCkXLrmby5Ee//8USEcGKTgZYZwhYjnf7q96tc2ERK1/CC2JxboeI+ckekKOlRQhqEQJnKL9mD8Ftjtr0sW/j6iqHKoEHLIgj+ozu+Akd37fVmhf6ZhdNwEY4ee52/kfJ44uKMj+aU/DDi16MgFonXlhIPuMyehxyxQ1HDiLQ==&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="Encryptor"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.StackableEncryptor, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="EncryptorConfigurations"&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="Encryptor01"&gt;
    ///                       &lt;items&gt;
    ///                         &lt;item key="HashAlgorithm" type="System.Type"&gt;System.Security.Cryptography.MD5Cng, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                         &lt;item key="PasswordSaltHash" type="System.String"&gt;vGFxpTfD4RCXk5qWQF0WHg==&lt;/item&gt;
    ///                         &lt;item key="Salt" type="System.String"&gt;A28u6IshCJzEcNgyAyUYyDN8Xi6TYShyHPZEvnJ1783VBXwOeW0UIBF5U+XHxitcXKZw8wcoNokk954UNOJHuG+EsFQDt1e7cUMehK/Ko35dDmLQzqIEQmGkOTRjhIF1HpYeta2ZbOJKdzSTlBJCwbws5h5ky9EucKNH1HxxTsI=&lt;/item&gt;
    ///                         &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.TripleDESCryptoServiceProvider, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                       &lt;/items&gt;
    ///                     &lt;/group&gt;
    ///                   &lt;/groups&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="TransportConfiguration"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="CacheTimeLimit" type="System.TimeSpan"&gt;00:01:30&lt;/item&gt;
    ///             &lt;item key="ChunkSize" type="System.Int32"&gt;512&lt;/item&gt;
    ///             &lt;item key="Compressor" type="System.Type"&gt;dodSON.Core.Compression.DeflateStreamCompressor, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportConfiguration, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="UseChunking" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="EncryptorConfigurations"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="Encryptor01"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.RijndaelManaged, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.EncryptorConfiguration, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="SaltedPassword"&gt;
    ///                       &lt;items&gt;
    ///                         &lt;item key="HashAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                         &lt;item key="PasswordSaltHash" type="System.Byte[]"&gt;AUAAv///qEN2jv42QDbbq2nxrfqI/QYCSh2JciIBTzoqBMPG5fsyowtCKZztsJBP1johyyOFeCReTlDJlaL7DGy9HaNv&lt;/item&gt;
    ///                         &lt;item key="Salt" type="System.Byte[]"&gt;AYAAf/8dv9lA9DKfeQPFPx5BU5hbRkBxjHVrRAkj/ATZ/W9+GTqqg5bQe1eRxmd4WKW07iOZm2lXV1vrPrkA7QwD7T2xTUAsp38RRlxl9v/4j/y+JdCdMWnUBgsXMYnri2kE1xYc4E6xXacXDze++OHRu1z8xiWgpuEBC1Ua/4xp0NvjHw==&lt;/item&gt;
    ///                         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.SaltedPassword, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                       &lt;/items&gt;
    ///                     &lt;/group&gt;
    ///                   &lt;/groups&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>The 'Alpha' Client Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Client"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Client, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;TestServerChannel&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ClientConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;Alpha&lt;/item&gt;
    ///         &lt;item key="ReceiveSelfSentMessages" type="System.Boolean"&gt;False&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ReceivableTypesFilter"&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="PayloadTypeInfo 1"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="TransmittableTypesFilter"&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="PayloadTypeInfo 1"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="RegistrationController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.EncryptedRegistrationController, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChallengeController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/+aPQCkXLrmby5Ee//8USEcGKTgZYZwhYjnf7q96tc2ERK1/CC2JxboeI+ckekKOlRQhqEQJnKL9mD8Ftjtr0sW/j6iqHKoEHLIgj+ozu+Akd37fVmhf6ZhdNwEY4ee52/kfJ44uKMj+aU/DDi16MgFonXlhIPuMyehxyxQ1HDiLQ==&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Encryptor"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.StackableEncryptor, dodSON.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="EncryptorConfigurations"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="Encryptor01"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="HashAlgorithm" type="System.Type"&gt;System.Security.Cryptography.MD5Cng, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                     &lt;item key="PasswordSaltHash" type="System.String"&gt;vGFxpTfD4RCXk5qWQF0WHg==&lt;/item&gt;
    ///                     &lt;item key="Salt" type="System.String"&gt;A28u6IshCJzEcNgyAyUYyDN8Xi6TYShyHPZEvnJ1783VBXwOeW0UIBF5U+XHxitcXKZw8wcoNokk954UNOJHuG+EsFQDt1e7cUMehK/Ko35dDmLQzqIEQmGkOTRjhIF1HpYeta2ZbOJKdzSTlBJCwbws5h5ky9EucKNH1HxxTsI=&lt;/item&gt;
    ///                     &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.TripleDESCryptoServiceProvider, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// </example>
    [Serializable]
    public abstract class ClientBase
        : IClient
    {
        #region Events
        /// <summary>
        /// Fired when a message is received.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageBus;
        /// <summary>
        /// Attempts to put the <paramref name="response"/> message on the message bus.
        /// </summary>
        /// <param name="response">The <see cref="MessageEventArgs"/> containing the <see cref="IMessage"/> to send.</param>
        protected void SendMessageOnMessageBus(MessageEventArgs response)
        {
            var message = response.Message;
            if (message.TypeInfo.TypeName == NetworkingHelper.ResponseAllTransportStatistics)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    _DownloadedTransportStatistics = _ServerAllTransportStatisticsConvertor.FromByteArray(message.Payload);
                    _WaitHandleMessageReceivedAndNoted?.Set();
                });
            }
            else
            {
                MessageBus?.Invoke(this, response as MessageEventArgs);
            }
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="ClientBase"/>.
        /// </summary>
        protected ClientBase()
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        protected ClientBase(Configuration.IConfigurationGroup configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Client")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Client\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // ChannelAddress
            var foundChannelAddress = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "ChannelAddress", true);
            Address = (IChannelAddress)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(ChannelAddress), foundChannelAddress);
            // ClientConfiguration
            var foundClientConfiguration = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "ClientConfiguration", true);
            ClientConfiguration = (IClientConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(ClientConfiguration), foundClientConfiguration);
            // RegistrationController
            var foundRegistrationController = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "RegistrationController", true);
            var registrationController = (IRegistrationController)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(null, foundRegistrationController);
            TransportController = new TransportController(registrationController);
        }
        #endregion
        #region Private Fields
        [NonSerialized] private ITransportStatisticsGroup _DownloadedTransportStatistics = null;
        [NonSerialized] private readonly object _SyncLockRequestAllTransportStatisticsReentrantBlock = new object();
        [NonSerialized] private System.Threading.AutoResetEvent _WaitHandleMessageReceivedAndNoted;  // internal use only; it is being disposed of properly
        [NonSerialized] private readonly TimeSpan _RequestAllTransportStatisticsTimeLimit = TimeSpan.FromSeconds(45);
        [NonSerialized] private readonly Converters.TypeSerializer<ITransportStatisticsGroup> _ServerAllTransportStatisticsConvertor = new Converters.TypeSerializer<ITransportStatisticsGroup>();
        #endregion
        #region Public Methods
        /// <summary>
        /// The client's unique channel id.
        /// </summary>
        public string Id => ClientConfiguration.Id;
        /// <summary>
        /// The channel's address.
        /// </summary>
        public IChannelAddress Address { get; private set; } = null;
        /// <summary>
        /// The channel's address converted into a <see cref="Uri"/>.
        /// </summary>
        public abstract Uri AddressUri
        {
            get;
        }
        /// <summary>
        /// Client configuration information.
        /// </summary>
        public IClientConfiguration ClientConfiguration { get; private set; } = null;
        /// <summary>
        /// Controls the transport layer.
        /// </summary>
        public ITransportController TransportController { get; private set; } = null;
        /// <summary>
        /// The current state of the communication channel.
        /// </summary>
        public ChannelStates State { get; private set; } = ChannelStates.Closed;
        /// <summary>
        /// Will prepare the client to join a communication system.
        /// </summary>
        /// <param name="channelAddress">Communication system addressing specifications.</param>
        /// <param name="clientConfiguration">This client's configuration.</param>
        /// <param name="registrationController">This client's registration controller.</param>
        public void Initialize(IChannelAddress channelAddress,
                               IClientConfiguration clientConfiguration,
                               IRegistrationController registrationController)
        {
            if (IsOpen)
            {
                throw new InvalidOperationException("The client is open; the client cannot be initialized while open.");
            }
            Address = channelAddress ?? throw new ArgumentNullException(nameof(channelAddress));
            ClientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
            if (registrationController == null)
            {
                throw new ArgumentNullException(nameof(registrationController));
            }
            TransportController = new TransportController(registrationController);
            ((ITransportControllerAdvanced)TransportController).SetClientServerId(Id);
        }
        /// <summary>
        /// Attempts to create a communication connection as either a client.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong in the attempt.</param>
        /// <returns>The state of the channel's connection.</returns>
        public ChannelStates Open(out Exception exception)
        {
            if (IsOpen)
            {
                throw new InvalidOperationException("The client is already open.");
            }
            State = ChannelStates.Opening;
            exception = null;
            if (OnOpen(out Exception ex))
            {
                State = ChannelStates.Registering;
                if (TransportController.RegistrationController.Register(ServiceProxy, this, (ITransportControllerAdvanced)TransportController))
                {
                    State = ChannelStates.Open;
                }
                else
                {
                    exception = new Exception("Unable to register client.");
                    State = ChannelStates.Unregistering;
                    Close(out Exception ex2);
                }
            }
            else
            {
                exception = new Exception("Unable to open client.", ex);
                State = ChannelStates.Unregistering;
                Close(out Exception ex2);
            }
            return State;
        }
        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong.</param>
        /// <returns><b>True</b> if the client was successfully closed; otherwise, <b>false</b> if anything went wrong.</returns>
        public bool Close(out Exception exception)
        {
            var results = true;
            exception = null;
            try
            {
                if (IsOpen)
                {
                    State = ChannelStates.Unregistering;
                    try
                    {
                        TransportController.RegistrationController.Unregister(ServiceProxy, this);
                    }
                    finally
                    {
                        // regardless of whether registration was successful, or not, OnClose() must be called
                        State = ChannelStates.Closing;
                        OnClose();
                    }
                }
            }
            catch (Exception ex)
            {
                results = false;
                exception = ex;
            }
            finally
            {
                State = ChannelStates.Closed;
            }
            return results;
        }
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        public void SendMessage(IMessage message)
        {
            // ******** SEND ENVELOPES TO SERVER ********

            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (State == ChannelStates.Open)
            {
                foreach (var envelope in TransportController.PrepareMessageForTransport(message))
                {
                    if (!PushEnvelopeToServer(envelope, out Exception ex))
                    {
                        // transport error
                        throw new Exception("ClientBase.SendMessage(IMessage)--> Transport Error: " + ex.Message);
                    }
                }
            }
            else
            {
                // client closed
                throw new InvalidOperationException("Unable to send message; the client is closed.");
            }
        }
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <typeparam name="T">The typeof the <paramref name="payload"/> being distributed.</typeparam>
        /// <param name="targetClientId">The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public void SendMessage<T>(string targetClientId, T payload) => SendMessage(new Message(Guid.NewGuid().ToString("N"), Id, targetClientId, new PayloadTypeInfo(typeof(T)), (new Converters.TypeSerializer<T>()).ToByteArray(payload)));
        /// <summary>
        /// Commands the client to request from the server all transport statistics from the server and all connected clients.
        /// </summary>
        /// <returns>Statistical information about the communication system.</returns>
        public ITransportStatisticsGroup RequestAllTransportStatistics()
        {
            lock (_SyncLockRequestAllTransportStatisticsReentrantBlock)
            {
                using (_WaitHandleMessageReceivedAndNoted = new System.Threading.AutoResetEvent(false))
                {
                    SendMessage(new Message(Guid.NewGuid().ToString(),
                                            Id,
                                            TransportController.TransportConfiguration.ServerId,
                                            new PayloadTypeInfo(NetworkingHelper.RequestAllTransportStatistics),
                                            null));
                    // wait for signal that statistics were received or until the time limit expires
                    _WaitHandleMessageReceivedAndNoted.WaitOne(_RequestAllTransportStatisticsTimeLimit);
                }
                //
                if (_DownloadedTransportStatistics != null)
                {
                    var result = Converters.ConvertersHelper.Clone(_DownloadedTransportStatistics);
                    _DownloadedTransportStatistics = null;
                    return result;
                }
            }
            return null;
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// When implemented, should perform any activities required when opening a client.
        /// </summary>
        /// <param name="exception">Contains any exceptions encountered; otherwise <b>null</b>.</param>
        /// <returns><b>True</b> if successful; otherwise <b>false</b>.</returns>
        protected abstract bool OnOpen(out Exception exception);
        /// <summary>
        /// When implemented, should perform any activities required when closing a client.
        /// </summary>
        protected abstract void OnClose();
        /// <summary>
        /// When implemented, should take an <see cref="TransportEnvelope"/> and send it to the server.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to transmit to the server.</param>
        /// <param name="exception">Contains any exceptions encountered; otherwise <b>null</b>.</param>
        /// <returns><b>True</b> if successful; otherwise <b>false</b>.</returns>
        protected abstract bool PushEnvelopeToServer(TransportEnvelope envelope, out Exception exception);
        /// <summary>
        /// Exposes services provided by the server.
        /// </summary>
        protected abstract IService ServiceProxy
        {
            get;
        }
        /// <summary>
        /// Returns <b>true</b> if the client is <see cref="ChannelStates.Open"/>; otherwise, <b>false</b>.
        /// </summary>
        protected bool IsOpen => ((State == ChannelStates.Opening) ||
                                  (State == ChannelStates.Registering) ||
                                  (State == ChannelStates.Open) ||
                                  (State == ChannelStates.Restarting) ||
                                  (State == ChannelStates.Closing) ||
                                  (State == ChannelStates.Unregistering));
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Client");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // ChannelAddress
                result.Add(Address.Configuration);
                // ServerConfiguration
                result.Add(ClientConfiguration.Configuration);
                // Registration Controller; cloning allows us to remove the Transport Configuration before exporting the configuration
                var clone = Converters.ConvertersHelper.Clone(TransportController.RegistrationController);
                ((IRegistrationControllerAdvanced)clone).SetTransportConfiguration(null);
                result.Add(clone.Configuration);
                //
                return result;
            }
        }
        #endregion
    }
}
