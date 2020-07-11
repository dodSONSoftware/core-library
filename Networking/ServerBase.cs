using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides the base class for a communication channel server.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example will create two named pipes servers (ServerS1, ServerS2) and an HTTP server (ServerRS).
    /// The ServerRS will act as both an HTTP relay server and as a host to more clients.
    /// Two bridges will be created; the first bridge will connect ServerS1 to ServerRS and the other bridge will connect ServerS2 to ServerRS.
    /// This will create a network of three servers:
    /// <br/><br/>
    /// ServerS1 (NamedPipe) &lt;--&gt; ServerRS (HTTP) &lt;--&gt; ServerS2 (NamedPipe)
    /// <br/><br/>
    /// Multiple clients will be started on all servers. 
    /// All clients will respond by sending a new message directly back to the client that sent the original message.
    /// <br/><br/>
    /// Once started, a single non-client targeted message will be broadcast from each client.
    /// The client log shows all clients receiving the non-client targeted message and then sending and receiving messages afterwards.
    /// The console application will then loop until anykey is press.
    /// <br/><br/>
    /// This demonstrates the functionality of the Bridge and the proper handling of client targeted messages.
    /// <br/><br/>
    /// Two logs will be generated, one for server activity and another for client activity.
    /// </para>
    /// <note type="note">
    /// It may be required for this application to be run as an administrator or configure a URL registration, and add a Firewall exception for the URL your service will be using. 
    /// See the following link for further information: <a href="http://go.microsoft.com/fwlink/?LinkId=70353">Configuring HTTP and HTTPS</a>  
    /// </note>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// class Program
    /// {
    ///     private static dodSON.Core.Logging.ICachedLog ClientLog;
    ///     private static string ClientLogFilename;
    ///     private static dodSON.Core.Logging.ICachedLog ServerLog;
    ///     private static string ServerLogFilename;
    ///     private static DateTime _LastDisplayEvent;
    ///     private static long _LastServerIncomingEnvelopes;
    ///     private static long _LastServerIncomingBytes;
    ///     private static long _LastServerOutgoingEnvelopes;
    ///     private static long _LastServerOutgoingBytes;
    ///     private static Dictionary&lt;string, ClientStatsInformation&gt; _ClientStatsInfoList = new Dictionary&lt;string, ClientStatsInformation&gt;();
    ///     private static bool SendMessages = true;
    ///     private static TimeSpan RelayMessageDelay = TimeSpan.FromSeconds(0.20);
    /// 
    /// 
    ///     static void Main(string[] args)
    ///     {
    ///         // ################################################
    ///         // ######## BE SURE TO CHANGE THESE VALUES ########
    ///         // ########
    ///         // ######## THE outputRootPath DIRECTORY WILL BE DELETED ########
    ///         // ######## DO NOT DIRECT THE outputRootPath DIRECTORY TO A FOLDER WITH ANY PRE-EXISTING FILES ########
    ///         // ######## BEST PRACTICE WOULD BE TO SET THE outputRootPath DIRECTORY TO A NEW FOLDER ########
    ///         // ########
    /// 
    ///         string outputRootPath = @"C:\(WORKING)\Dev\Networking5";
    ///         dodSON.Core.Configuration.IConfigurationSerializer&lt;StringBuilder&gt; configurationSerializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///         string configurationFileExtenstion = "xml";
    ///         //
    ///         int serverS1ClientCount = 8;
    ///         int serverS2ClientCount = 8;
    ///         int serverRSClientCount = 8;
    /// 
    ///         // ################################################
    /// 
    ///         // ******** Validate outputRootPath is Available and Clear
    ///         // ******** WARNING ---- This will delete all files and sub-directories in 'outputRootPath' ---- ********
    ///         if (System.IO.Directory.Exists(outputRootPath))
    ///         {
    ///             var recursive = true;
    ///             System.IO.Directory.Delete(outputRootPath, recursive);
    ///             dodSON.Core.Threading.ThreadingHelper.Sleep(250);
    ///         }
    ///         System.IO.Directory.CreateDirectory(outputRootPath);
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         DateTimeOffset startTime = DateTimeOffset.Now;
    ///         Console.Clear();
    ///         Console.WriteLine("dodSON.Core Networking Example");
    ///         Console.WriteLine("dodSON Software Core Library");
    ///         Console.WriteLine("----------------------------");
    ///         Console.WriteLine();
    /// 
    ///         // ******** Create and Open Server Log
    ///         ServerLogFilename = System.IO.Path.Combine(outputRootPath, @"ServerLog.txt");
    ///         Console.WriteLine($"Opening Server Log: {ServerLogFilename}");
    ///         bool ServerWriteLogEntriesUsingLocalTime = true;
    ///         dodSON.Core.Logging.ILog serverLogActual = new dodSON.Core.Logging.FileEventLog.Log(ServerLogFilename, ServerWriteLogEntriesUsingLocalTime);
    ///         bool serverAutoFlushLogs = true;
    ///         int serverFlushMaximumLogs = 10;
    ///         TimeSpan serverFlushTimeLimit = TimeSpan.FromSeconds(5);
    ///         ServerLog = new dodSON.Core.Logging.CachedLog(serverLogActual, serverAutoFlushLogs, serverFlushMaximumLogs, serverFlushTimeLimit);
    ///         ServerLog.Open();
    /// 
    ///         // ******** Create and Open Client Log
    ///         ClientLogFilename = System.IO.Path.Combine(outputRootPath, @"ClientLog.txt");
    ///         Console.WriteLine($"Opening Client Log: {ClientLogFilename}");
    ///         Console.WriteLine();
    ///         bool clientWriteLogEntriesUsingLocalTime = true;
    ///         bool autoTruncateLogFile = true;
    ///         long maxLogSizeBytes = dodSON.Core.Common.ByteCountHelper.FromGigabytes(1);
    ///         int logsToRetain = 0;
    ///         dodSON.Core.Logging.ILog clientLogActual = new dodSON.Core.Logging.FileEventLog.Log(ClientLogFilename, clientWriteLogEntriesUsingLocalTime, autoTruncateLogFile, maxLogSizeBytes, logsToRetain);
    ///         bool clientAutoFlushLogs = true;
    ///         int clientFlushMaximumLogs = 1000;
    ///         TimeSpan clientFlushTimeLimit = TimeSpan.FromSeconds(20);
    ///         ClientLog = new dodSON.Core.Logging.CachedLog(clientLogActual, clientAutoFlushLogs, clientFlushMaximumLogs, clientFlushTimeLimit);
    ///         ClientLog.Open();
    /// 
    ///         // ******** 
    ///         var logThreadWorker = new dodSON.Core.Threading.ThreadWorker(TimeSpan.FromSeconds(1),
    ///                                                                      (ct) =&gt;
    ///                                                                      {
    ///                                                                          if (ServerLog.IsFlushable)
    ///                                                                          {
    ///                                                                              ServerLog.FlushLogs();
    ///                                                                          }
    ///                                                                          if (ClientLog.IsFlushable)
    ///                                                                          {
    ///                                                                              ClientLog.FlushLogs();
    ///                                                                          }
    ///                                                                      });
    ///         logThreadWorker.Start();
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Transport Configuration
    ///         // create channel address
    ///         string ipAddress = dodSON.Core.Networking.NetworkingHelper.DefaultIpAddress;
    ///         int server1_Port = dodSON.Core.Networking.NetworkingHelper.RecommendedMinumumPortValue;
    ///         int server2_Port = dodSON.Core.Networking.NetworkingHelper.RecommendedMinumumPortValue + 1;
    ///         string server1_Name = "ServerS1";
    ///         string server2_Name = "ServerS2";
    ///         dodSON.Core.Networking.IChannelAddress channelAddressS1 = new dodSON.Core.Networking.ChannelAddress(ipAddress, server1_Port, server1_Name);
    ///         dodSON.Core.Networking.IChannelAddress channelAddressS2 = new dodSON.Core.Networking.ChannelAddress(ipAddress, server2_Port, server2_Name);
    /// 
    ///         // create transport configuration
    ///         System.Security.Cryptography.HashAlgorithm transportHashAlgorithm = new System.Security.Cryptography.SHA512Managed();
    ///         System.Security.SecureString transportPassword = new System.Security.SecureString();
    ///         char[] password = "BadPa$$w0rd".ToCharArray();
    ///         dodSON.Core.Cryptography.CryptographyExtensions.AppendChars(transportPassword, password, true);
    ///         byte[] transportSalt = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(128);
    ///         dodSON.Core.Cryptography.ISaltedPassword transportSaltedPassword = new dodSON.Core.Cryptography.SaltedPassword(transportHashAlgorithm, transportPassword, transportSalt);
    ///         Type transportSymmetricAlgorithmType = typeof(System.Security.Cryptography.RijndaelManaged);
    ///         dodSON.Core.Cryptography.IEncryptorConfiguration transportEncryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(transportSaltedPassword, transportSymmetricAlgorithmType);
    ///         Type compressorType = typeof(dodSON.Core.Compression.DeflateStreamCompressor);
    ///         bool useChunking = true;
    ///         int chunkSize = dodSON.Core.Networking.NetworkingHelper.MinimumTransportEnvelopeChunkSize;
    ///         TimeSpan cacheTimeLimit = TimeSpan.FromSeconds(90);
    ///         dodSON.Core.Networking.ITransportConfiguration transportConfiguration = new dodSON.Core.Networking.TransportConfiguration(transportEncryptorConfiguration, compressorType, useChunking, chunkSize, cacheTimeLimit);
    /// 
    ///         // ******** Create Challenge Controller
    ///         // create challenge controller
    ///         byte[] actualEvidence = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(128);
    ///         dodSON.Core.Networking.IChallengeController passwordChallengeController = new dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController(actualEvidence);
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Server S1
    ///         string idS1 = "ServerS1";
    ///         string logSourceId_S1 = idS1;
    ///         dodSON.Core.Networking.IServerConfiguration serverS1Configuration = new dodSON.Core.Networking.ServerConfiguration(idS1);
    ///         dodSON.Core.Networking.IServer serverS1 = CreateNP_TunnelingRegistrationServer(channelAddressS1,
    ///                                                                                         serverS1Configuration,
    ///                                                                                         transportConfiguration,
    ///                                                                                         passwordChallengeController,
    ///                                                                                         (s, e) =&gt;
    ///                                                                                         {
    ///                                                                                             // Skip logging any NETWORK_REQUESTALLSTATISTICS; the main application's main loop makes this request every cycle (thats a lot). 
    ///                                                                                             // Recording these events would flood the log will a lot of noise.
    ///                                                                                             if (e.Header != dodSON.Core.Networking.ActivityLogsEventType.Network_RequestAllStatistics)
    ///                                                                                             {
    ///                                                                                                 ServerLog.Write(e.Logs);
    ///                                                                                             }
    ///                                                                                         },
    ///                                                                                         logSourceId_S1);
    ///         // save server configuration as XML
    ///         string serverConfigurationOutputFilename1 = System.IO.Path.Combine(outputRootPath, $@"{idS1}.configuration.{configurationFileExtenstion}");
    ///         System.IO.File.WriteAllText(serverConfigurationOutputFilename1, configurationSerializer.Serialize(serverS1.Configuration).ToString());
    /// 
    ///         // ******** Open Server S1
    ///         Console.WriteLine($"Opening Server: [ {idS1} ]");
    ///         if (serverS1.Open(out Exception serverOpenException1) != dodSON.Core.Networking.ChannelStates.Open)
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open server. {serverOpenException1?.Message}");
    ///         }
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Server S2
    ///         string idS2 = "ServerS2";
    ///         string logSourceId_S2 = idS2;
    ///         dodSON.Core.Networking.IServerConfiguration serverS2Configuration = new dodSON.Core.Networking.ServerConfiguration(idS2);
    ///         dodSON.Core.Networking.IServer serverS2 = CreateNP_TunnelingRegistrationServer(channelAddressS2,
    ///                                                                                         serverS2Configuration,
    ///                                                                                         transportConfiguration,
    ///                                                                                         passwordChallengeController,
    ///                                                                                         (s, e) =&gt;
    ///                                                                                         {
    ///                                                                                             // Skip logging any NETWORK_REQUESTALLSTATISTICS; the main application's main loop makes this request every cycle (thats a lot). 
    ///                                                                                             // Recording these events would flood the log will a lot of noise.
    ///                                                                                             if (e.Header != dodSON.Core.Networking.ActivityLogsEventType.Network_RequestAllStatistics)
    ///                                                                                             {
    ///                                                                                                 ServerLog.Write(e.Logs);
    ///                                                                                             }
    ///                                                                                         },
    ///                                                                                         logSourceId_S2);
    ///         // save server configuration as XML
    ///         string serverConfigurationOutputFilename2 = System.IO.Path.Combine(outputRootPath, $@"{idS2}.configuration.{configurationFileExtenstion}");
    ///         System.IO.File.WriteAllText(serverConfigurationOutputFilename2, configurationSerializer.Serialize(serverS2.Configuration).ToString());
    /// 
    ///         // ******** Open Server S2
    ///         Console.WriteLine($"Opening Server: [ {idS2} ]");
    ///         if (serverS2.Open(out Exception serverOpenException2) != dodSON.Core.Networking.ChannelStates.Open)
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open server. {serverOpenException2?.Message}");
    ///         }
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Server RS
    ///         string idRS = "ServerRS";
    ///         string logSourceId_RS = idS2;
    ///         dodSON.Core.Networking.IServerConfiguration serverRSConfiguration = new dodSON.Core.Networking.ServerConfiguration(idRS);
    ///         int relayServer_Port = 55000;
    ///         dodSON.Core.Networking.IChannelAddress channelAddressRS = new dodSON.Core.Networking.ChannelAddress(dodSON.Core.Networking.NetworkingHelper.DefaultIpAddress, relayServer_Port, idRS);
    ///         dodSON.Core.Networking.IServer serverRS = CreateHTTP_TunnelingRegistrationServer(channelAddressRS,
    ///                                                                                          serverRSConfiguration,
    ///                                                                                          transportConfiguration,
    ///                                                                                          passwordChallengeController,
    ///                                                                                          (s, e) =&gt;
    ///                                                                                          {
    ///                                                                                              // Skip logging any NETWORK_REQUESTALLSTATISTICS; the main application's main loop makes this request every cycle (thats a lot). 
    ///                                                                                              // Recording these events would flood the log will a lot of noise.
    ///                                                                                              if (e.Header != dodSON.Core.Networking.ActivityLogsEventType.Network_RequestAllStatistics)
    ///                                                                                              {
    ///                                                                                                  ServerLog.Write(e.Logs);
    ///                                                                                              }
    ///                                                                                          },
    ///                                                                                          idRS);
    ///         // save server configuration as XML
    ///         string serverConfigurationOutputFilename3 = System.IO.Path.Combine(outputRootPath, $@"{idRS}.configuration.{configurationFileExtenstion}");
    ///         System.IO.File.WriteAllText(serverConfigurationOutputFilename3, configurationSerializer.Serialize(serverRS.Configuration).ToString());
    /// 
    ///         // ******** Open Server S1
    ///         Console.WriteLine($"Opening Server: [ {idRS} ]");
    ///         if (serverRS.Open(out Exception serverOpenException3) != dodSON.Core.Networking.ChannelStates.Open)
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open server. {serverOpenException3?.Message}");
    ///         }
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Bridge 1
    ///         // ######## TEST --&gt; instantiate a bridge from a configuration loaded from a file and de-serialized into a IConfigurationGroup
    /// 
    ///         // create internal client (NamedPipe)
    ///         string clientS1_BridgeInternalClientId_Id = "S1BridgeInternalClient";
    ///         dodSON.Core.Networking.IClient internalClientS1_BIC1 = CreateNP_Client(clientS1_BridgeInternalClientId_Id, serverS1.Address, serverS1.TransportController.RegistrationController);
    /// 
    ///         // create external client (HTTP)
    ///         string clientS1_BridgeExternalClientId_Id = "S1BridgeExternalClient";
    ///         dodSON.Core.Networking.IClient externalClientS1_BEC1 = CreateHTTP_Client(clientS1_BridgeExternalClientId_Id, serverRS.Address, serverRS.TransportController.RegistrationController);
    /// 
    ///         // create temp bridge
    ///         dodSON.Core.Networking.Bridge temporary_bridgeBC1 = new dodSON.Core.Networking.Bridge(internalClientS1_BIC1, externalClientS1_BEC1);
    /// 
    ///         // save temp bridge configuration as an XML file
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, ($"Bridge-01.configuration.{configurationFileExtenstion}")), configurationSerializer.Serialize(temporary_bridgeBC1.Configuration).ToString());
    /// 
    ///         // load bridge from temp XML file and de-serialize it
    ///         var bridge01_Text = System.IO.File.ReadAllText(System.IO.Path.Combine(outputRootPath, ($"Bridge-01.configuration.{configurationFileExtenstion}")));
    ///         dodSON.Core.Configuration.IConfigurationGroup bridge01_Config = configurationSerializer.Deserialize(new StringBuilder(bridge01_Text));
    /// 
    ///         // create bridge 1
    ///         dodSON.Core.Networking.Bridge bridgeBC1 = new dodSON.Core.Networking.Bridge(bridge01_Config);
    ///         // connect to both message buses
    ///         bridgeBC1.InternalClient.MessageBus += SharedClient_MessageBus;
    ///         bridgeBC1.ExternalClient.MessageBus += SharedClient_MessageBus;
    /// 
    ///         // open bridge 1
    ///         Console.WriteLine($"Opening Bridge: [ Bridge #1 ]");
    ///         if (bridgeBC1.Open(out Exception clientBC1OpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open bridge1. {clientBC1OpenException?.Message}");
    ///         }
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Create Bridge 2
    ///         // create internal client (NamedPipe)
    ///         string clientS2_BridgeInternalClientId_Id = "S2BridgeInternalClient";
    ///         dodSON.Core.Networking.IClient internalClientS2_BIC1 = CreateNP_Client(clientS2_BridgeInternalClientId_Id, serverS2.Address, serverS2.TransportController.RegistrationController);
    /// 
    ///         // create external client (HTTP)
    ///         string clientS2_BridgeExternalClientId_Id = "S2BridgeExternalClient";
    ///         dodSON.Core.Networking.IClient externalClientS2_BEC2 = CreateHTTP_Client(clientS2_BridgeExternalClientId_Id, serverRS.Address, serverRS.TransportController.RegistrationController);
    /// 
    ///         // create bridge 2
    ///         dodSON.Core.Networking.Bridge bridgeBC2 = new dodSON.Core.Networking.Bridge(internalClientS2_BIC1, externalClientS2_BEC2);
    /// 
    ///         // save bridge 2 configuration as XML
    ///         System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, ($"Bridge-02.configuration.{configurationFileExtenstion}")), configurationSerializer.Serialize(bridgeBC2.Configuration).ToString());
    /// 
    ///         // open bridge 2
    ///         Console.WriteLine($"Opening Bridge: [ Bridge #2 ]");
    ///         if (bridgeBC2.Open(out Exception clientBC2OpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open bridge2. {clientBC2OpenException?.Message}");
    ///         }
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         List&lt;dodSON.Core.Networking.IClient&gt; allClients = new List&lt;dodSON.Core.Networking.IClient&gt;();
    /// 
    ///         // ****************************************************************************************************
    ///         // ******** Create ServerS1 Clients
    ///         for (int i = 0; i &lt; serverS1ClientCount; i++)
    ///         {
    ///             string client_Id = $"S1Client{i + 1}";
    ///             dodSON.Core.Networking.IClient client = CreateNP_Client(client_Id, serverS1.Address, serverS1.TransportController.RegistrationController);
    /// 
    ///             // save client configuration as XML
    ///             System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, ($"Client-{client.Id}.configuration.{configurationFileExtenstion}")), configurationSerializer.Serialize(client.Configuration).ToString());
    /// 
    ///             // open Client C1
    ///             Console.WriteLine($"Opening S1 Client: [ {client.Id} ]");
    ///             if (client.Open(out Exception clientOpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///             {
    ///                 Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open client. {clientOpenException?.Message}");
    ///             }
    ///             client.MessageBus += Client_MessageBus;
    ///             allClients.Add(client);
    ///         }
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    ///         // ******** Create ServerS2 Clients
    ///         for (int i = 0; i &lt; serverS2ClientCount; i++)
    ///         {
    ///             string client_Id = $"S2Client{i + 1}";
    ///             dodSON.Core.Networking.IClient client = CreateNP_Client(client_Id, serverS2.Address, serverS2.TransportController.RegistrationController);
    /// 
    ///             // save client configuration as XML
    ///             System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, ($"Client-{client.Id}.configuration.{configurationFileExtenstion}")), configurationSerializer.Serialize(client.Configuration).ToString());
    /// 
    ///             // open Client C1
    ///             Console.WriteLine($"Opening S2 Client: [ {client.Id} ]");
    ///             if (client.Open(out Exception clientOpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///             {
    ///                 Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open client. {clientOpenException?.Message}");
    ///             }
    ///             client.MessageBus += Client_MessageBus;
    ///             allClients.Add(client);
    ///         }
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    ///         // ******** Create ServerRS Clients
    ///         for (int i = 0; i &lt; serverRSClientCount; i++)
    ///         {
    ///             string client_Id = $"RSClient{i + 1}";
    ///             dodSON.Core.Networking.IClient client = CreateHTTP_Client(client_Id, serverRS.Address, serverRS.TransportController.RegistrationController);
    /// 
    ///             // save client configuration as XML
    ///             System.IO.File.WriteAllText(System.IO.Path.Combine(outputRootPath, ($"Client-{client.Id}.configuration.{configurationFileExtenstion}")), configurationSerializer.Serialize(client.Configuration).ToString());
    /// 
    ///             // open Client C1
    ///             Console.WriteLine($"Opening RS Client: [ {client.Id} ]");
    ///             if (client.Open(out Exception clientOpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///             {
    ///                 Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot open client. {clientOpenException?.Message}");
    ///             }
    ///             client.MessageBus += Client_MessageBus;
    ///             allClients.Add(client);
    ///         }
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         Console.Write($"{Environment.NewLine}Press anykey to continue&gt;");
    ///         Console.ReadKey(true);
    ///         Console.WriteLine();
    /// 
    ///         // ******** Send Non-Targeted Message
    ///         // ******** message sent from S1Client1; every client should receive this initial message
    /// 
    ///         Console.WriteLine($"{Environment.NewLine}--------------------------------");
    ///         Console.WriteLine($"Sending Non-Client Targeted Message from every client.");
    ///         string targetClientId = "";
    ///         dodSON.Core.Networking.IPayloadTypeInfo typeInfo = new dodSON.Core.Networking.PayloadTypeInfo(typeof(string));
    ///         foreach (var client in allClients)
    ///         {
    ///             client.SendMessage(new dodSON.Core.Networking.Message(Guid.NewGuid().ToString(),
    ///                                                                   client.Id,
    ///                                                                   targetClientId,
    ///                                                                   typeInfo,
    ///                                                                   (new dodSON.Core.Converters.TypeSerializer&lt;string&gt;()).ToByteArray($"Message from {client.Id}")));
    ///         }
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Wait for user to press anykey to terminate application
    ///         var stWatch = System.Diagnostics.Stopwatch.StartNew();
    ///         Console.WriteLine($"{Environment.NewLine}Let it run a bit to allow the messages to flow back a forth. A few dozen seconds should be enough.");
    ///         while (!Console.KeyAvailable)
    ///         {
    ///             // Console.Write($"\r{stWatch.Elapsed}   press anykey&gt;               \b\b\b\b\b\b\b\b\b\b\b\b\b\b\b");
    /// 
    ///             Console.Clear();
    ///             //DisplayNetworkStats(startTime, serverS1);
    ///             DisplayNetworkStats(startTime, serverRS);
    ///             //DisplayNetworkStats(startTime, serverS2);
    ///             dodSON.Core.Threading.ThreadingHelper.Sleep(1000);
    ///         }
    ///         Console.ReadKey(true);
    ///         //Console.Write("\r--------------------------------                ");
    ///         Console.WriteLine($"{Environment.NewLine}");
    /// 
    ///         // ******** Stop Message Production
    ///         SendMessages = false;
    /// 
    ///         // ******** Wait a bit for any residual messages to reach their final destinations; admittedly, not the best way to do this.
    ///         dodSON.Core.Threading.ThreadingHelper.Sleep(TimeSpan.FromSeconds(3));
    /// 
    ///         // ******** Close All Servers 
    ///         Console.WriteLine($"Closing Server: [ {serverRS.Id} ]");
    ///         if (!serverRS.Close(out Exception serverRSCloseException))
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot close server. {serverRSCloseException?.Message}");
    ///         }
    ///         Console.WriteLine($"Closing Server: [ {serverS1.Id} ]");
    ///         if (!serverS1.Close(out Exception serverS1CloseException))
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot close server. {serverS1CloseException?.Message}");
    ///         }
    ///         Console.WriteLine($"Closing Server: [ {serverS2.Id} ]");
    ///         if (!serverS2.Close(out Exception serverS2CloseException))
    ///         {
    ///             Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}ERROR: Cannot close server. {serverS2CloseException?.Message}");
    ///         }
    /// 
    ///         // ******** Close All Logs
    ///         Console.WriteLine();
    ///         Console.WriteLine($"Closing Client Log: {ClientLogFilename}");
    ///         ClientLog.Close();
    ///         Console.WriteLine($"Closing Server Log: {ServerLogFilename}");
    ///         ServerLog.Close();
    /// 
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** End of Application
    ///         Console.WriteLine();
    ///         Console.WriteLine("================================");
    ///         Console.Write("press anykey&gt;");
    ///         Console.ReadKey(true);
    ///         Console.WriteLine();
    ///     }
    /// 
    ///     // This will send any message it gets back to the original sender
    ///     private static void Client_MessageBus(object sender, dodSON.Core.Networking.MessageEventArgs e)
    ///     {
    ///         if (SendMessages)
    ///         {
    ///             Task.Factory.StartNew(() =&gt;
    ///             {
    ///                 dodSON.Core.Threading.ThreadingHelper.Sleep(RelayMessageDelay);
    ///                 if (((dodSON.Core.Networking.IClient)sender).State == dodSON.Core.Networking.ChannelStates.Open)
    ///                 {
    ///                         // send message back
    ///                         dodSON.Core.Networking.IClient sendingClient = (sender as dodSON.Core.Networking.IClient);
    ///                     dodSON.Core.Networking.IMessage message = new dodSON.Core.Networking.Message(Guid.NewGuid().ToString(),
    ///                                                                                                  sendingClient.Id,
    ///                                                                                                  e.Message.ClientId,
    ///                                                                                                  new dodSON.Core.Networking.PayloadTypeInfo(typeof(string)),
    ///                                                                                                  (new dodSON.Core.Converters.TypeSerializer&lt;string&gt;()).ToByteArray($"Message from {sendingClient.Id}"));
    ///                     sendingClient.SendMessage(message);
    ///                 }
    ///             });
    ///         }
    ///     }
    /// 
    ///     // This will create a Named Pipe Null Registration Server
    ///     private static dodSON.Core.Networking.IServer CreateNP_NullRegistrationServer(dodSON.Core.Networking.ChannelAddress channelAddress,
    ///                                                                                   dodSON.Core.Networking.ServerConfiguration serverConfiguration,
    ///                                                                                   dodSON.Core.Networking.TransportConfiguration transportConfiguration,
    ///                                                                                   dodSON.Core.Networking.IChallengeController challengeController,
    ///                                                                                   Action&lt;object, dodSON.Core.Networking.ActivityLogsEventArgs&gt; activityLogsEventHandler,
    ///                                                                                   string logSourceId)
    ///     {
    ///         // create the server
    ///         dodSON.Core.Networking.IRegistrationController registrationController = new dodSON.Core.Networking.RegistrationControllers.NullRegistrationController(transportConfiguration, challengeController);
    ///         dodSON.Core.Networking.ITransportController serverTransportController = new dodSON.Core.Networking.TransportController(transportConfiguration, registrationController);
    ///         dodSON.Core.Networking.IServer server = new dodSON.Core.Networking.NamedPipes.Server(channelAddress, serverConfiguration, serverTransportController, logSourceId);
    ///         server.ActivityLogsEvent += new EventHandler&lt;dodSON.Core.Networking.ActivityLogsEventArgs&gt;(activityLogsEventHandler);
    ///         return server;
    ///     }
    /// 
    ///     // This will create a Named Pipe Tunneling Registration Server
    ///     private static dodSON.Core.Networking.IServer CreateNP_TunnelingRegistrationServer(dodSON.Core.Networking.IChannelAddress channelAddress,
    ///                                                                                         dodSON.Core.Networking.IServerConfiguration serverConfiguration,
    ///                                                                                         dodSON.Core.Networking.ITransportConfiguration transportConfiguration,
    ///                                                                                         dodSON.Core.Networking.IChallengeController challengeController,
    ///                                                                                         Action&lt;object, dodSON.Core.Networking.ActivityLogsEventArgs&gt; activityLogsEventHandler,
    ///                                                                                         string logSourceId)
    ///     {
    ///         // create registration controller
    ///         dodSON.Core.Networking.IRegistrationController registrationController = new dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController(transportConfiguration, challengeController);
    ///         dodSON.Core.Networking.ITransportController serverTransportController = new dodSON.Core.Networking.TransportController(transportConfiguration, registrationController);
    ///         // create the server
    ///         dodSON.Core.Networking.IServer server = new dodSON.Core.Networking.NamedPipes.Server(channelAddress, serverConfiguration, serverTransportController, logSourceId);
    ///         server.ActivityLogsEvent += new EventHandler&lt;dodSON.Core.Networking.ActivityLogsEventArgs&gt;(activityLogsEventHandler);
    ///         return server;
    ///     }
    /// 
    ///     // This will create a Named Pipe Client
    ///     private static dodSON.Core.Networking.IClient CreateNP_Client(string id,
    ///                                                                   dodSON.Core.Networking.IChannelAddress channelAddress,
    ///                                                                   dodSON.Core.Networking.IRegistrationController registrationController)
    ///     {
    ///         // create client configuration
    ///         bool recieveSelfSentMessages = false;
    ///         List&lt;dodSON.Core.Networking.IPayloadTypeInfo&gt; recievableAndTransmittableTypesFilter = new List&lt;dodSON.Core.Networking.IPayloadTypeInfo&gt;() { new dodSON.Core.Networking.PayloadTypeInfo(typeof(string)) };
    ///         dodSON.Core.Networking.IClientConfiguration configuration = new dodSON.Core.Networking.ClientConfiguration(id, recieveSelfSentMessages, recievableAndTransmittableTypesFilter, recievableAndTransmittableTypesFilter);
    ///         // create client
    ///         dodSON.Core.Networking.IClient client = new dodSON.Core.Networking.NamedPipes.Client(channelAddress, configuration, registrationController);
    ///         client.MessageBus += SharedClient_MessageBus;
    ///         return client;
    ///     }
    /// 
    ///     // --------------------------------
    /// 
    ///     // This will create a HTTP Null Registration Server
    ///     private static dodSON.Core.Networking.IServer CreateHTTP_NullRegistrationServer(dodSON.Core.Networking.ChannelAddress channelAddress,
    ///                                                                                     dodSON.Core.Networking.ServerConfiguration serverConfiguration,
    ///                                                                                     dodSON.Core.Networking.TransportConfiguration transportConfiguration,
    ///                                                                                     dodSON.Core.Networking.IChallengeController challengeController,
    ///                                                                                     Action&lt;object, dodSON.Core.Networking.ActivityLogsEventArgs&gt; activityLogsEventHandler,
    ///                                                                                     string logSourceId)
    ///     {
    ///         // create the server
    ///         dodSON.Core.Networking.IRegistrationController registrationController = new dodSON.Core.Networking.RegistrationControllers.NullRegistrationController(transportConfiguration, challengeController);
    ///         dodSON.Core.Networking.ITransportController serverTransportController = new dodSON.Core.Networking.TransportController(transportConfiguration, registrationController);
    ///         dodSON.Core.Networking.IServer server = new dodSON.Core.Networking.Http.Server(channelAddress, serverConfiguration, serverTransportController, logSourceId);
    ///         server.ActivityLogsEvent += new EventHandler&lt;dodSON.Core.Networking.ActivityLogsEventArgs&gt;(activityLogsEventHandler);
    ///         return server;
    ///     }
    /// 
    ///     // This will create a HTTP Tunneling Registration Server
    ///     private static dodSON.Core.Networking.IServer CreateHTTP_TunnelingRegistrationServer(dodSON.Core.Networking.IChannelAddress channelAddress,
    ///                                                                                          dodSON.Core.Networking.IServerConfiguration serverConfiguration,
    ///                                                                                          dodSON.Core.Networking.ITransportConfiguration transportConfiguration,
    ///                                                                                          dodSON.Core.Networking.IChallengeController challengeController,
    ///                                                                                          Action&lt;object, dodSON.Core.Networking.ActivityLogsEventArgs&gt; activityLogsEventHandler,
    ///                                                                                          string logSourceId)
    ///     {
    ///         // create registration controller
    ///         dodSON.Core.Networking.IRegistrationController registrationController = new dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController(transportConfiguration, challengeController);
    ///         dodSON.Core.Networking.ITransportController serverTransportController = new dodSON.Core.Networking.TransportController(transportConfiguration, registrationController);
    ///         // create the server
    ///         dodSON.Core.Networking.IServer server = new dodSON.Core.Networking.Http.Server(channelAddress, serverConfiguration, serverTransportController, logSourceId);
    ///         server.ActivityLogsEvent += new EventHandler&lt;dodSON.Core.Networking.ActivityLogsEventArgs&gt;(activityLogsEventHandler);
    ///         return server;
    ///     }
    /// 
    ///     // This will create a HTTP Client
    ///     private static dodSON.Core.Networking.IClient CreateHTTP_Client(string id,
    ///                                                                     dodSON.Core.Networking.IChannelAddress channelAddress,
    ///                                                                     dodSON.Core.Networking.IRegistrationController registrationController)
    ///     {
    ///         // create client configuration
    ///         bool recieveSelfSentMessages = false;
    ///         List&lt;dodSON.Core.Networking.IPayloadTypeInfo&gt; recievableAndTransmittableTypesFilter = new List&lt;dodSON.Core.Networking.IPayloadTypeInfo&gt;() { new dodSON.Core.Networking.PayloadTypeInfo(typeof(string)) };
    ///         dodSON.Core.Networking.IClientConfiguration configuration = new dodSON.Core.Networking.ClientConfiguration(id, recieveSelfSentMessages, recievableAndTransmittableTypesFilter, recievableAndTransmittableTypesFilter);
    ///         // create client
    ///         dodSON.Core.Networking.IClient client = new dodSON.Core.Networking.Http.Client(channelAddress, configuration, registrationController);
    ///         client.MessageBus += SharedClient_MessageBus;
    ///         return client;
    ///     }
    /// 
    ///     // --------------------------------
    /// 
    ///     // This is the message bus event handler shared by all clients; all it does is write the message to the client log
    ///     private static void SharedClient_MessageBus(object sender, dodSON.Core.Networking.MessageEventArgs e)
    ///     {
    ///         if (e.Message.TypeInfo.TypeName == typeof(string).AssemblyQualifiedName)
    ///         {
    ///             string message = (new dodSON.Core.Converters.TypeSerializer&lt;string&gt;()).FromByteArray(e.Message.Payload);
    ///             dodSON.Core.Networking.IClient senderAsClient = sender as dodSON.Core.Networking.IClient;
    ///             ClientLog.Write(dodSON.Core.Logging.LogEntryType.Information, senderAsClient?.Id, message);
    ///         }
    ///     }
    /// 
    ///     // This will display network statistics for the given IServer
    ///     private static void DisplayNetworkStats(DateTimeOffset startTime, dodSON.Core.Networking.IServer server)
    ///     {
    ///         dodSON.Core.Networking.ITransportStatisticsGroup allStats = server.RequestAllClientsTransportStatistics();
    ///         TimeSpan forDuration = DateTime.Now - _LastDisplayEvent;
    /// 
    ///         Console.WriteLine($"Uri     = {server.AddressUri}");
    ///         Console.WriteLine($"Runtime = {DateTimeOffset.Now - startTime}");
    ///         Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}");
    ///         // 
    ///         Console.WriteLine($"Server Incoming:");
    ///         double incomingEnvelopesPerSecond = allStats.ServerStatistics.IncomingAverageEnvelopesPerSecond(_LastServerIncomingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.IncomingEnvelopes}, envelopes/second = {incomingEnvelopesPerSecond:N2}");
    ///         double incomingBytesPerSecond = allStats.ServerStatistics.IncomingAverageBytesPerSecond(_LastServerIncomingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)incomingBytesPerSecond)} ({incomingBytesPerSecond:N2})");
    ///         Console.WriteLine($"Server Outgoing:");
    ///         double outgoingEnvelopesPerSecond = allStats.ServerStatistics.OutgoingAverageEnvelopesPerSecond(_LastServerOutgoingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.OutgoingEnvelopes}, envelopes/second = {outgoingEnvelopesPerSecond:N2}");
    ///         double outgoingBytesPerSecond = allStats.ServerStatistics.OutgoingAverageBytesPerSecond(_LastServerOutgoingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.OutgoingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)outgoingBytesPerSecond)} ({outgoingBytesPerSecond:N2})");
    ///         // 
    ///         _LastDisplayEvent = DateTime.Now;
    ///         _LastServerIncomingEnvelopes = allStats.ServerStatistics.IncomingEnvelopes;
    ///         _LastServerOutgoingEnvelopes = allStats.ServerStatistics.OutgoingEnvelopes;
    ///         _LastServerIncomingBytes = allStats.ServerStatistics.IncomingBytes;
    ///         _LastServerOutgoingBytes = allStats.ServerStatistics.OutgoingBytes;
    ///         // 
    ///         foreach (dodSON.Core.Networking.ITransportStatistics client in allStats.AllClientsStatistics)
    ///         {
    ///             ClientStatsInformation clientStatsInfo = new ClientStatsInformation();
    ///             if (_ClientStatsInfoList.ContainsKey(client.ClientServerId))
    ///             {
    ///                 clientStatsInfo = _ClientStatsInfoList[client.ClientServerId];
    ///             }
    ///             else
    ///             {
    ///                 _ClientStatsInfoList.Add(client.ClientServerId, clientStatsInfo);
    ///             }
    ///             // 
    ///             Console.WriteLine();
    ///             Console.WriteLine($"{client.ClientServerId} Incoming:");
    ///             double clientIncomingMessagesPerSecond = client.IncomingAverageMessagesPerSecond(clientStatsInfo.LastIncomingMessages, forDuration);
    ///             Console.WriteLine($"    Messages  = {client.IncomingMessages}, messages/second = {clientIncomingMessagesPerSecond:N2}");
    ///             double clientIncomingEnvelopesPerSecond = client.IncomingAverageEnvelopesPerSecond(clientStatsInfo.LastIncomingEnvelopes, forDuration);
    ///             Console.WriteLine($"    Envelopes = {client.IncomingEnvelopes}, envelopes/second = {clientIncomingEnvelopesPerSecond:N2}");
    ///             double clientIncomingBytesPerSecond = client.IncomingAverageBytesPerSecond(clientStatsInfo.LastIncomingBytes, forDuration);
    ///             Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientIncomingBytesPerSecond)} ({clientIncomingBytesPerSecond:N2})");
    ///             Console.WriteLine($"{client.ClientServerId} Outgoing:");
    ///             double clientOutgoingMessagesPerSecond = client.OutgoingAverageMessagesPerSecond(clientStatsInfo.LastOutgoingMessages, forDuration);
    ///             Console.WriteLine($"    Messages  = {client.OutgoingMessages}, messages/second = {clientOutgoingMessagesPerSecond:N2}");
    ///             double clientOutgoingEnvelopesPerSecond = client.OutgoingAverageEnvelopesPerSecond(clientStatsInfo.LastOutgoingEnvelopes, forDuration);
    ///             Console.WriteLine($"    Envelopes = {client.OutgoingEnvelopes}, envelopes/second = {clientOutgoingEnvelopesPerSecond:N2}");
    ///             double clientOutgoingBytesPerSecond = client.OutgoingAverageBytesPerSecond(clientStatsInfo.LastOutgoingBytes, forDuration);
    ///             Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.OutgoingBytes)}, Bytes/seconds = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientOutgoingBytesPerSecond)} ({clientOutgoingBytesPerSecond:N2})");
    ///             // 
    ///             clientStatsInfo.LastIncomingBytes = client.IncomingBytes;
    ///             clientStatsInfo.LastIncomingEnvelopes = client.IncomingEnvelopes;
    ///             clientStatsInfo.LastIncomingMessages = client.IncomingMessages;
    ///             clientStatsInfo.LastOutgoingBytes = client.OutgoingBytes;
    ///             clientStatsInfo.LastOutgoingEnvelopes = client.OutgoingEnvelopes;
    ///             clientStatsInfo.LastOutgoingMessages = client.OutgoingMessages;
    ///         }
    ///         Console.WriteLine($"{Environment.NewLine}--------------------------------");
    ///     }
    /// 
    ///     // A simple class to hold client statistical information
    ///     private class ClientStatsInformation
    ///     {
    ///         public long LastIncomingBytes { get; set; } = 0;
    ///         public long LastIncomingEnvelopes { get; set; } = 0;
    ///         public long LastIncomingMessages { get; set; } = 0;
    ///         public long LastOutgoingBytes { get; set; } = 0;
    ///         public long LastOutgoingEnvelopes { get; set; } = 0;
    ///         public long LastOutgoingMessages { get; set; } = 0;
    ///     }
    /// }
    /// </code>
    /// <para>Screen shots.</para>
    /// <code>
    /// // dodSON.Core Networking Example
    /// // dodSON Software Core Library
    /// // ----------------------------
    /// // 
    /// // Opening Server Log: C:\(WORKING)\Dev\Networking5\ServerLog.txt
    /// // Opening Client Log: C:\(WORKING)\Dev\Networking5\ClientLog.txt
    /// // 
    /// // Opening Server: [ ServerS1 ]
    /// // Opening Server: [ ServerS2 ]
    /// // Opening Server: [ ServerRS ]
    /// // 
    /// // Opening Bridge: [ Bridge #1 ]
    /// // Opening Bridge: [ Bridge #2 ]
    /// // 
    /// // Opening S1 Client: [ S1Client1 ]
    /// // Opening S1 Client: [ S1Client2 ]
    /// // Opening S1 Client: [ S1Client3 ]
    /// // Opening S1 Client: [ S1Client4 ]
    /// // Opening S1 Client: [ S1Client5 ]
    /// // Opening S1 Client: [ S1Client6 ]
    /// // Opening S1 Client: [ S1Client7 ]
    /// // Opening S1 Client: [ S1Client8 ]
    /// // 
    /// // Opening S2 Client: [ S2Client1 ]
    /// // Opening S2 Client: [ S2Client2 ]
    /// // Opening S2 Client: [ S2Client3 ]
    /// // Opening S2 Client: [ S2Client4 ]
    /// // Opening S2 Client: [ S2Client5 ]
    /// // Opening S2 Client: [ S2Client6 ]
    /// // Opening S2 Client: [ S2Client7 ]
    /// // Opening S2 Client: [ S2Client8 ]
    /// // 
    /// // Opening RS Client: [ RSClient1 ]
    /// // Opening RS Client: [ RSClient2 ]
    /// // Opening RS Client: [ RSClient3 ]
    /// // Opening RS Client: [ RSClient4 ]
    /// // Opening RS Client: [ RSClient5 ]
    /// // Opening RS Client: [ RSClient6 ]
    /// // Opening RS Client: [ RSClient7 ]
    /// // Opening RS Client: [ RSClient8 ]
    /// // 
    /// // 
    /// // Press anykey to continue&gt;
    /// </code>
    /// <code>
    /// // Uri     = http://localhost:55000/ServerRS
    /// // Runtime = 00:02:21.0959206
    /// // 
    /// // --------------------------------
    /// // 
    /// // Server Incoming:
    /// //     Envelopes = 8095, envelopes/second = 66.42
    /// //     Bytes     = 5.6 MB, Bytes/second = 47 KB (48,631.18)
    /// // Server Outgoing:
    /// //     Envelopes = 8311, envelopes/second = 66.42
    /// //     Bytes     = 5.8 MB, Bytes/second = 47 KB (48,631.18)
    /// // 
    /// // S1BridgeExternalClient Incoming:
    /// //     Messages  = 2103, messages/second = 43.02
    /// //     Envelopes = 2103, envelopes/second = 43.02
    /// //     Bytes     = 1.5 MB, Bytes/second = 31 KB (31,664.00)
    /// // S1BridgeExternalClient Outgoing:
    /// //     Messages  = 2082, messages/second = 17.36
    /// //     Envelopes = 2082, envelopes/second = 17.36
    /// //     Bytes     = 1.5 MB, Bytes/seconds = 12 KB (12,764.63)
    /// // 
    /// // S2BridgeExternalClient Incoming:
    /// //     Messages  = 2108, messages/second = 30.19
    /// //     Envelopes = 2108, envelopes/second = 30.19
    /// //     Bytes     = 1.5 MB, Bytes/second = 22 KB (22,220.35)
    /// // S2BridgeExternalClient Outgoing:
    /// //     Messages  = 2086, messages/second = 25.66
    /// //     Envelopes = 2086, envelopes/second = 25.66
    /// //     Bytes     = 1.5 MB, Bytes/seconds = 18 KB (18,863.15)
    /// // 
    /// // RSClient1 Incoming:
    /// //     Messages  = 512, messages/second = 6.04
    /// //     Envelopes = 512, envelopes/second = 6.04
    /// //     Bytes     = 360 KB, Bytes/second = 4 KB (4,407.84)
    /// // RSClient1 Outgoing:
    /// //     Messages  = 502, messages/second = 3.02
    /// //     Envelopes = 502, envelopes/second = 3.02
    /// //     Bytes     = 354 KB, Bytes/seconds = 2 KB (2,149.58)
    /// // 
    /// // RSClient2 Incoming:
    /// //     Messages  = 514, messages/second = 6.79
    /// //     Envelopes = 514, envelopes/second = 6.79
    /// //     Bytes     = 362 KB, Bytes/second = 5 KB (4,975.43)
    /// // RSClient2 Outgoing:
    /// //     Messages  = 504, messages/second = 5.28
    /// //     Envelopes = 504, envelopes/second = 5.28
    /// //     Bytes     = 355 KB, Bytes/seconds = 4 KB (3,804.03)
    /// // 
    /// // RSClient3 Incoming:
    /// //     Messages  = 510, messages/second = 3.77
    /// //     Envelopes = 510, envelopes/second = 3.77
    /// //     Bytes     = 359 KB, Bytes/second = 3 KB (2,777.54)
    /// // RSClient3 Outgoing:
    /// //     Messages  = 505, messages/second = 6.04
    /// //     Envelopes = 505, envelopes/second = 6.04
    /// //     Bytes     = 356 KB, Bytes/seconds = 4 KB (4,335.38)
    /// // 
    /// // RSClient4 Incoming:
    /// //     Messages  = 506, messages/second = 2.26
    /// //     Envelopes = 506, envelopes/second = 2.26
    /// //     Bytes     = 356 KB, Bytes/second = 2 KB (1,581.99)
    /// // RSClient4 Outgoing:
    /// //     Messages  = 503, messages/second = 5.28
    /// //     Envelopes = 503, envelopes/second = 5.28
    /// //     Bytes     = 355 KB, Bytes/seconds = 4 KB (3,791.95)
    /// // 
    /// // RSClient5 Incoming:
    /// //     Messages  = 504, messages/second = 1.51
    /// //     Envelopes = 504, envelopes/second = 1.51
    /// //     Bytes     = 354 KB, Bytes/second = 1 KB (1,050.64)
    /// // RSClient5 Outgoing:
    /// //     Messages  = 503, messages/second = 3.02
    /// //     Envelopes = 503, envelopes/second = 3.02
    /// //     Bytes     = 354 KB, Bytes/seconds = 2 KB (2,173.73)
    /// // 
    /// // RSClient6 Incoming:
    /// //     Messages  = 505, messages/second = 1.51
    /// //     Envelopes = 505, envelopes/second = 1.51
    /// //     Bytes     = 355 KB, Bytes/second = 1 KB (1,086.87)
    /// // RSClient6 Outgoing:
    /// //     Messages  = 504, messages/second = 6.79
    /// //     Envelopes = 504, envelopes/second = 6.79
    /// //     Bytes     = 355 KB, Bytes/seconds = 5 KB (4,842.59)
    /// // 
    /// // RSClient7 Incoming:
    /// //     Messages  = 506, messages/second = 0.00
    /// //     Envelopes = 506, envelopes/second = 0.00
    /// //     Bytes     = 356 KB, Bytes/second = 0 bytes (0.00)
    /// // RSClient7 Outgoing:
    /// //     Messages  = 496, messages/second = 5.28
    /// //     Envelopes = 496, envelopes/second = 5.28
    /// //     Bytes     = 350 KB, Bytes/seconds = 4 KB (3,779.88)
    /// // 
    /// // RSClient8 Incoming:
    /// //     Messages  = 504, messages/second = 0.75
    /// //     Envelopes = 504, envelopes/second = 0.75
    /// //     Bytes     = 355 KB, Bytes/second = 531 bytes (531.36)
    /// // RSClient8 Outgoing:
    /// //     Messages  = 502, messages/second = 6.79
    /// //     Envelopes = 502, envelopes/second = 6.79
    /// //     Bytes     = 354 KB, Bytes/seconds = 5 KB (4,806.36)
    /// // 
    /// // --------------------------------
    /// // 
    /// // 
    /// // Closing Server: [ ServerRS ]
    /// // Closing Server: [ ServerS1 ]
    /// // Closing Server: [ ServerS2 ]
    /// // 
    /// // Closing Client Log: C:\(WORKING)\Dev\Networking5\ClientLog.txt
    /// // Closing Server Log: C:\(WORKING)\Dev\Networking5\ServerLog.txt
    /// // 
    /// // ================================
    /// // press anykey&gt;
    /// </code>
    /// <para>The Server Log.</para>
    /// <code>
    /// // 2018-08-26 1:15:58 AM -05:00; Information; 0; 0; ServerS1; Opening Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:15:58 AM -05:00; Information; 0; 0; ServerS1; Server Id=ServerS1
    /// // 2018-08-26 1:15:58 AM -05:00; Information; 0; 0; ServerS1; Type=[dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2018-08-26 1:15:58 AM -05:00; Information; 0; 0; ServerS1; Uri=net.pipe://localhost/ServerS1-49152
    /// // 2018-08-26 1:15:58 AM -05:00; Information; 0; 0; ServerS1; IP Address=localhost, Name=ServerS1, Port=49152
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Registering Client=ServerInternalClient_99f23015-2c33-4806-ae28-a64dodSON7c5
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=True
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=ServerInternalClient_99f23015-2c33-4806-ae28-a64dodSON7c5
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS1; Server Opened Successfully. Elapsed Time=00:00:00.3445204
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Opening Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Server Id=ServerS2
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Type=[dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Uri=net.pipe://localhost/ServerS2-49153
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; IP Address=localhost, Name=ServerS2, Port=49153
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Registering Client=ServerInternalClient_61360fcc-5561-47b3-9f74-9afdodSON042
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=True
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=ServerInternalClient_61360fcc-5561-47b3-9f74-9afdodSON042
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerS2; Server Opened Successfully. Elapsed Time=00:00:00.1947748
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Opening Server, (dodSON.Core.Networking.Http.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Server Id=ServerRS
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Type=[dodSON.Core.Networking.Http.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Uri=http://localhost:55000/ServerRS
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; IP Address=localhost, Name=ServerRS, Port=55000
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Registering Client=ServerInternalClient_7835edc5-ca5d-40ba-a50e-fe4dodSON860
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=True
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (0)
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=ServerInternalClient_7835edc5-ca5d-40ba-a50e-fe4dodSON860
    /// // 2018-08-26 1:15:59 AM -05:00; Information; 0; 0; ServerRS; Server Opened Successfully. Elapsed Time=00:00:00.6374156
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1BridgeInternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1BridgeInternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Registering Client=S1BridgeExternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=S1BridgeExternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2BridgeInternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2BridgeInternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Registering Client=S2BridgeExternalClient
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:00 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=S2BridgeExternalClient
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client1
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client1
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client2
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client2
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client3
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client3
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client4
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:01 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client4
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client5
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client5
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client6
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client6
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client7
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client7
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Registering Client=S1Client8
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receive Own Messages=False
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Receivable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Transmittable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS1; Completed Registering Client=S1Client8
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client1
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:02 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client1
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client2
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client2
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client3
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client3
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client4
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client4
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client5
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:03 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client5
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client6
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client6
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client7
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client7
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Registering Client=S2Client8
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receive Own Messages=False
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Receivable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Transmittable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerS2; Completed Registering Client=S2Client8
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient1
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient1
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient2
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:04 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient2
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient3
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient3
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient4
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient4
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient5
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:05 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient5
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient6
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient6
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient7
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient7
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Registering Client=RSClient8
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receive Own Messages=False
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Receivable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Transmittable Types: (1)
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; #1=System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    /// // 2018-08-26 1:16:06 AM -05:00; Information; 0; 0; ServerRS; Completed Registering Client=RSClient8
    /// // 2018-08-26 1:18:23 AM -05:00; Information; 0; 0; ServerRS; Closing Server, (dodSON.Core.Networking.Http.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:18:23 AM -05:00; Information; 0; 0; ServerRS; Closing All Clients: (11)
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient1, Date Started=2018-08-26 1:16:04 AM, Runtime=00:02:20.3774493
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=S2BridgeExternalClient, Date Started=2018-08-26 1:16:00 AM, Runtime=00:02:24.5669031
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient2, Date Started=2018-08-26 1:16:04 AM, Runtime=00:02:20.5559702
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=S1BridgeExternalClient, Date Started=2018-08-26 1:16:00 AM, Runtime=00:02:25.0679033
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient3, Date Started=2018-08-26 1:16:05 AM, Runtime=00:02:20.3989697
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient4, Date Started=2018-08-26 1:16:05 AM, Runtime=00:02:20.1919683
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient5, Date Started=2018-08-26 1:16:05 AM, Runtime=00:02:19.7073791
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient6, Date Started=2018-08-26 1:16:06 AM, Runtime=00:02:19.5153781
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient8, Date Started=2018-08-26 1:16:06 AM, Runtime=00:02:19.1234094
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=RSClient7, Date Started=2018-08-26 1:16:06 AM, Runtime=00:02:19.3534083
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Unregistering Client=ServerInternalClient_7835edc5-ca5d-40ba-a50e-fe4dodSON860, Date Started=2018-08-26 1:15:59 AM, Runtime=00:02:25.8008880
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Shutting Down Server, Id=ServerRS
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Total Incoming Bytes=6.0 MB (6,297,120)
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Total Incoming Envelopes=8,642
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Total Outgoing Bytes=6.2 MB (6,449,904)
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Total Outgoing Envelopes=8,858
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerRS; Server Closed Successfully.
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerS1; Closing Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:18:25 AM -05:00; Information; 0; 0; ServerS1; Closing All Clients: (10)
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client3, Date Started=2018-08-26 1:16:01 AM, Runtime=00:02:25.1729032
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client6, Date Started=2018-08-26 1:16:02 AM, Runtime=00:02:24.5070391
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client5, Date Started=2018-08-26 1:16:02 AM, Runtime=00:02:24.7950255
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client7, Date Started=2018-08-26 1:16:02 AM, Runtime=00:02:24.3180278
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client4, Date Started=2018-08-26 1:16:01 AM, Runtime=00:02:25.0619028
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client1, Date Started=2018-08-26 1:16:01 AM, Runtime=00:02:25.7599031
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client2, Date Started=2018-08-26 1:16:01 AM, Runtime=00:02:25.5768880
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1BridgeInternalClient, Date Started=2018-08-26 1:16:00 AM, Runtime=00:02:26.6958844
    /// // 2018-08-26 1:18:26 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=S1Client8, Date Started=2018-08-26 1:16:02 AM, Runtime=00:02:24.2218641
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Unregistering Client=ServerInternalClient_99f23015-2c33-4806-ae28-a64dodSON7c5, Date Started=2018-08-26 1:15:59 AM, Runtime=00:02:27.9418852
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Shutting Down Server, Id=ServerS1
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Total Incoming Bytes=4.3 MB (4,472,928)
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Total Incoming Envelopes=6,181
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Total Outgoing Bytes=4.4 MB (4,608,992)
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Total Outgoing Envelopes=6,373
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS1; Server Closed Successfully.
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Closing Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Closing All Clients: (10)
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client3, Date Started=2018-08-26 1:16:03 AM, Runtime=00:02:24.5308624
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client2, Date Started=2018-08-26 1:16:03 AM, Runtime=00:02:24.7608636
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client6, Date Started=2018-08-26 1:16:04 AM, Runtime=00:02:23.9052974
    /// // 2018-08-26 1:18:27 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client5, Date Started=2018-08-26 1:16:03 AM, Runtime=00:02:24.1832511
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2BridgeInternalClient, Date Started=2018-08-26 1:16:00 AM, Runtime=00:02:27.3379049
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client7, Date Started=2018-08-26 1:16:04 AM, Runtime=00:02:23.7729850
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client4, Date Started=2018-08-26 1:16:03 AM, Runtime=00:02:24.4678609
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client1, Date Started=2018-08-26 1:16:02 AM, Runtime=00:02:25.1318611
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=S2Client8, Date Started=2018-08-26 1:16:04 AM, Runtime=00:02:23.6279810
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Unregistering Client=ServerInternalClient_61360fcc-5561-47b3-9f74-9afdodSON042, Date Started=2018-08-26 1:15:59 AM, Runtime=00:02:28.8978254
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Shutting Down Server, Id=ServerS2
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Total Incoming Bytes=4.3 MB (4,473,968)
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Total Incoming Envelopes=6,175
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Total Outgoing Bytes=4.4 MB (4,610,800)
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Total Outgoing Envelopes=6,367
    /// // 2018-08-26 1:18:28 AM -05:00; Information; 0; 0; ServerS2; Server Closed Successfully.
    /// </code>
    /// <para>The Client Log.</para>
    /// <code>
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient1; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient3; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client5
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient6; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient5; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient2; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient1; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient8; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient7; Message from RSClient2
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient3; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient2; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient5; Message from RSClient1
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; RSClient5; Message from RSClient3
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client8
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client7
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client6
    /// // 2018-08-26 1:16:29 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client7
    /// // .
    /// // .
    /// // .
    /// // .
    /// // .
    /// // .
    /// // .
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient5; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client6; Message from RSClient4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient5; Message from RSClient6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient5; Message from RSClient4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from RSClient4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from RSClient4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient6; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient7; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; RSClient8; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1Client4; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client1
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client4
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient6
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeExternalClient; Message from RSClient3
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client2
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2BridgeInternalClient; Message from S2Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client8
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1Client8; Message from RSClient7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:21 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client6; Message from RSClient1
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1BridgeInternalClient; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient7; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2BridgeExternalClient; Message from RSClient5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from S2Client2
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from S2Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client4
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client2
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client2
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client4
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client1
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient7; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client4
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client2; Message from S2Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client6; Message from S2Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client3; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client1; Message from S1Client4
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client5; Message from S2Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client4; Message from S2Client1
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client3
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient4; Message from RSClient6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client7; Message from S2Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client1; Message from S2Client5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client6; Message from S1Client1
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client6; Message from RSClient7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from S2Client1
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client2
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client3
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client3; Message from S2Client4
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client2; Message from S1Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client7; Message from S1Client5
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client5; Message from S1Client8
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S2Client8; Message from S2Client7
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client1; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; S1Client8; Message from S1Client6
    /// // 2018-08-26 1:18:22 AM -05:00; Information; 0; 0; RSClient8; Message from RSClient7
    /// </code>
    /// <para>The ServerRS Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Server"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.Http.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;ServerRS&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;55000&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ServerConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;ServerRS&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="OverrideTypesFilter" /&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="TransportController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="RegistrationController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ChallengeController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="TransportConfiguration"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="CacheTimeLimit" type="System.TimeSpan"&gt;0.00:01:30.0&lt;/item&gt;
    ///             &lt;item key="ChunkSize" type="System.Int32"&gt;512&lt;/item&gt;
    ///             &lt;item key="Compressor" type="System.RuntimeType, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"&gt;dodSON.Core.Compression.DeflateStreamCompressor, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="UseChunking" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="EncryptorConfigurations"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="Encryptor01"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.RijndaelManaged, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.EncryptorConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="SaltedPassword"&gt;
    ///                       &lt;items&gt;
    ///                         &lt;item key="HashAlgorithmType" type="System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"&gt;System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                         &lt;item key="PasswordSaltHash" type="System.Byte[]"&gt;AUAAv/+kyTR28ue6nOyXYZFkksPW1WXvUz0cyR3xEWNss7N6kVUMsCrlWopKVVWANUn4+VMmfxNdStvn6L6aFwrjzShK&lt;/item&gt;
    ///                         &lt;item key="Salt" type="System.Byte[]"&gt;AYAAf//zxIuck1PwMFq8NLVpAO4YV7tTkauSohnm90A1aLFrLytVkfAMD/2dvEFuPiWZ8ZmjkXBGVeGHw4YSD0YtvhxJy/qoBVL2ECS5fDZcAiDVm0fpJWGvHj0jMhv4uItIRXxZGuNhfpa6Fzycbx76cH8MMZvimZRWKjgiadgRl77q3Q==&lt;/item&gt;
    ///                         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.SaltedPassword, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
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
    /// <para>The ServerS1 Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Server"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;ServerS1&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ServerConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;ServerS1&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="OverrideTypesFilter" /&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="TransportController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="RegistrationController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ChallengeController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="TransportConfiguration"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="CacheTimeLimit" type="System.TimeSpan"&gt;0.00:01:30.0&lt;/item&gt;
    ///             &lt;item key="ChunkSize" type="System.Int32"&gt;512&lt;/item&gt;
    ///             &lt;item key="Compressor" type="System.RuntimeType, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"&gt;dodSON.Core.Compression.DeflateStreamCompressor, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="UseChunking" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="EncryptorConfigurations"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="Encryptor01"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.RijndaelManaged, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.EncryptorConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="SaltedPassword"&gt;
    ///                       &lt;items&gt;
    ///                         &lt;item key="HashAlgorithmType" type="System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"&gt;System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                         &lt;item key="PasswordSaltHash" type="System.Byte[]"&gt;AUAAv/+kyTR28ue6nOyXYZFkksPW1WXvUz0cyR3xEWNss7N6kVUMsCrlWopKVVWANUn4+VMmfxNdStvn6L6aFwrjzShK&lt;/item&gt;
    ///                         &lt;item key="Salt" type="System.Byte[]"&gt;AYAAf//zxIuck1PwMFq8NLVpAO4YV7tTkauSohnm90A1aLFrLytVkfAMD/2dvEFuPiWZ8ZmjkXBGVeGHw4YSD0YtvhxJy/qoBVL2ECS5fDZcAiDVm0fpJWGvHj0jMhv4uItIRXxZGuNhfpa6Fzycbx76cH8MMZvimZRWKjgiadgRl77q3Q==&lt;/item&gt;
    ///                         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.SaltedPassword, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
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
    /// <para>One of ServerRS Client's Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Client"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.Http.Client, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;ServerRS&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;55000&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ClientConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;RSClient1&lt;/item&gt;
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
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChallengeController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>One of ServerS1 Client's Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Client"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Client, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="ChannelAddress"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///         &lt;item key="Name" type="System.String"&gt;ServerS1&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ClientConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;S1Client1&lt;/item&gt;
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
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChallengeController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>ServerS1 Bridge Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="Bridge"&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="External Client"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.Http.Client, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChannelAddress"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///             &lt;item key="Name" type="System.String"&gt;ServerRS&lt;/item&gt;
    ///             &lt;item key="Port" type="System.Int32"&gt;55000&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="ClientConfiguration"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Id" type="System.String"&gt;S1BridgeExternalClient&lt;/item&gt;
    ///             &lt;item key="ReceiveSelfSentMessages" type="System.Boolean"&gt;False&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ReceivableTypesFilter"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="PayloadTypeInfo 1"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="TransmittableTypesFilter"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="PayloadTypeInfo 1"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="RegistrationController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ChallengeController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="Internal Client"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Client, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChannelAddress"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///             &lt;item key="Name" type="System.String"&gt;ServerS1&lt;/item&gt;
    ///             &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="ClientConfiguration"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Id" type="System.String"&gt;S1BridgeInternalClient&lt;/item&gt;
    ///             &lt;item key="ReceiveSelfSentMessages" type="System.Boolean"&gt;False&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ReceivableTypesFilter"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="PayloadTypeInfo 1"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="TransmittableTypesFilter"&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="PayloadTypeInfo 1"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="TypeName" type="System.String"&gt;System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="RegistrationController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="ChallengeController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf/9yLIynXHicQUGgcwyFnnLrFtWUd4lq/BPJCOg+yxrihi+3payf8Z0mozlA9dZosUYX4z7yVXHsonkX/pQqK2SPlQcjRRkfjxR02HoNsXHyjQE0NzfV4+ECWWI4CRviVs0bym1CACEonrP0clr8oMOgQgWEAwKKxncRxNRslWC+Vg==&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
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
    public abstract class ServerBase
        : IServer
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
        protected void RaiseMessageBusEvent(MessageEventArgs response) => MessageBus?.Invoke(this, response as MessageEventArgs);
        /// <summary>
        /// Fired when activity <see cref="Logging.Logs"/> are available.
        /// </summary>
        public event EventHandler<Networking.ActivityLogsEventArgs> ActivityLogsEvent;
        /// <summary>
        /// Attempts to broadcast the <paramref name="response"/> containing a header string and the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="response">The <see cref="Networking.ActivityLogsEventArgs"/> containing a header string and the <see cref="Logging.Logs"/> to broadcast.</param>
        protected void RaiseActivityLogsEvent(Networking.ActivityLogsEventArgs response) => ActivityLogsEvent?.Invoke(this, response);
        #endregion
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="ServerBase"/>.
        /// </summary>
        protected ServerBase()
        {
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        protected ServerBase(Configuration.IConfigurationGroup configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Server")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Server\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // ChannelAddress
            var foundChannelAddress = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "ChannelAddress", true);
            Address = (IChannelAddress)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(ChannelAddress), foundChannelAddress);
            // ServerConfiguration
            var foundServerConfiguration = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "ServerConfiguration", true);
            ServerConfiguration = (IServerConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(ServerConfiguration), foundServerConfiguration);
            // TransportController
            var foundTransportController = Core.Configuration.ConfigurationHelper.FindConfigurationGroup(configuration, "TransportController", true);
            TransportController = (ITransportController)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(TransportController), foundTransportController);
        }
        #endregion
        #region Private Fields
        private readonly object _ActivityLogsEventSyncLock = new object();
        private readonly Converters.TypeSerializer<ITransportStatisticsGroup> _ServerAllTransportStatisticsConvertor = new Converters.TypeSerializer<ITransportStatisticsGroup>();
        private readonly Converters.TypeSerializer<ITransportStatistics> _TransportStatisticsConvertor = new Converters.TypeSerializer<ITransportStatistics>();
        #endregion
        #region Public Methods
        /// <summary>
        /// The value used by the <see cref="ServerBase"/> for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.
        /// </summary>
        public string LogSourceId
        {
            get; private set;
        }
        /// <summary>
        /// The channel's unique id.
        /// </summary>
        public string Id => ServerConfiguration.Id;
        /// <summary>
        /// The channel's address.
        /// </summary>
        public IChannelAddress Address
        {
            get; private set;
        }
        /// <summary>
        /// The channel's address converted into a <see cref="Uri"/>.
        /// </summary>
        public abstract Uri AddressUri
        {
            get;
        }
        /// <summary>
        /// Controls the transport layer.
        /// </summary>
        public ITransportController TransportController
        {
            get; private set;
        }
        /// <summary>
        /// Server configuration information.
        /// </summary>
        public IServerConfiguration ServerConfiguration { get; private set; } = null;
        /// <summary>
        /// The current state of the communication channel.
        /// </summary>
        public ChannelStates State { get; private set; } = ChannelStates.Closed;
        /// <summary>
        /// Will prepare the server to join a communication system.
        /// </summary>
        /// <param name="channelAddress">Communication system addressing specifications.</param>
        /// <param name="configuration">This server's configuration.</param>
        /// <param name="transportController">This server's transport controller.</param>
        /// <param name="logSourceId">The value used by the <see cref="ServerBase"/> for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.</param>
        public void Initialize(IChannelAddress channelAddress,
                               IServerConfiguration configuration,
                               ITransportController transportController,
                               string logSourceId)
        {
            if (IsOpen)
            {
                throw new InvalidOperationException("The server is open; the server cannot be initialized while open.");
            }
            Address = channelAddress ?? throw new ArgumentNullException(nameof(channelAddress));
            ServerConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            TransportController = transportController ?? throw new ArgumentNullException(nameof(transportController));
            ((ITransportControllerAdvanced)TransportController).SetClientServerId(ServerConfiguration.Id);
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
        }
        /// <summary>
        /// Attempts to create a communication connection as either a server or a client.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong in the attempt.</param>
        /// <returns>The state of the channel's connection.</returns>
        public ChannelStates Open(out Exception exception)
        {
            //
            if (IsOpen)
            {
                throw new InvalidOperationException("The server is already open.");
            }
            //
            var stWatch = System.Diagnostics.Stopwatch.StartNew();
            var logs = new Logging.Logs();
            var thisType = this.GetType();
            var coreAssembly = System.Reflection.Assembly.GetAssembly(thisType);
            var parts = coreAssembly.FullName.Split(',');
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Opening Server, ({thisType.FullName}, {parts[0]}, v{parts[1].Split('=')[1]})");
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Server Id={ServerConfiguration.Id}");
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Type=[{this.GetType().AssemblyQualifiedName}]");
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Uri={AddressUri}");
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"IP Address={Address.IPAddress}, Name={Address.Name}, Port={Address.Port}");
            var serverOverrideTypesCount = ServerConfiguration.OverrideTypesFilter.Count();
            if (serverOverrideTypesCount > 0)
            {
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Override Types: ({serverOverrideTypesCount})");
                int count = 0;
                foreach (var item in ServerConfiguration.OverrideTypesFilter)
                {
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++count}={item.TypeName}");
                }
            }
            //
            State = ChannelStates.Opening;
            exception = null;
            if (OnOpen(out Exception ex))
            {
                NetworkShared.LogClientConfigurationDetails(logs, LogSourceId, InternalClient.ClientConfiguration, true);
                State = ChannelStates.Open;
                Service.ActivityLogsEvent += (object s, Networking.ActivityLogsEventArgs e) =>
                {
                    RaiseActivityLogsEvent(new ActivityLogsEventArgs(e.Header, e.Logs));
                };
            }
            else
            {
                exception = new Exception("Unable to open server.", ex);
                // this is to allow the Close() methods to execute
                State = ChannelStates.Closing;
                Close(out Exception ex2);
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Error: Unable to open server. Exception={ex.Message}");
            }
            if (exception == null)
            {
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Server Opened Successfully. Elapsed Time={stWatch.Elapsed}");
            }
            RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_Open, logs));
            return State;
        }
        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong.</param>
        /// <returns>The current <see cref="ChannelStates"/>.</returns>
        public bool Close(out Exception exception)
        {
            var stWatch = System.Diagnostics.Stopwatch.StartNew();
            var logs = new Logging.Logs();
            var thisType = this.GetType();
            var coreAssembly = System.Reflection.Assembly.GetAssembly(thisType);
            var parts = coreAssembly.FullName.Split(',');
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Closing {thisType.Name}, ({thisType.FullName}, {parts[0]}, v{parts[1].Split('=')[1]})");
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Closing All Clients: ({Service.RegisteredClientsCallbacks.Count()})");
            RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_Close, Converters.ConvertersHelper.Clone(logs)));
            logs.Clear();
            //
            if (!IsOpen)
            {
                throw new InvalidOperationException("The server is already closed.");
            }
            var results = true;
            exception = null;
            State = ChannelStates.Closing;
            try
            {
                InternalCloseAllClients();
            }
            catch (Exception ex)
            {
                results = false;
                exception = ex;
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Error: Problem encountered while closing all clients. Exception={ex.Message}");
            }
            finally
            {
                // regardless of whether closing all clients was successful, or not, OnClose() must be called
                OnClose();
                State = ChannelStates.Closed;
                if (exception == null)
                {
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Shutting Down Server, Id={TransportController.TransportStatisticsLive.ClientServerId}, Uri={AddressUri}");
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Total Incoming Bytes={Common.ByteCountHelper.ToString(TransportController.TransportStatisticsLive.IncomingBytes)} ({TransportController.TransportStatisticsLive.IncomingBytes:N0})");
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Total Incoming Envelopes={TransportController.TransportStatisticsLive.IncomingEnvelopes:N0}");
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Total Outgoing Bytes={Common.ByteCountHelper.ToString(TransportController.TransportStatisticsLive.OutgoingBytes)} ({TransportController.TransportStatisticsLive.OutgoingBytes:N0})");
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Total Outgoing Envelopes={TransportController.TransportStatisticsLive.OutgoingEnvelopes:N0}");
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Server Closed Successfully.");
                }
                RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_Close, logs));
            }
            return results;
        }
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        public void SendMessage(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (State == ChannelStates.Open)
            {
                PushMessageToInternalClient(message);
            }
            else
            {
                throw new InvalidOperationException("Unable to send message; the server is closed.");
            }
        }
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <typeparam name="T">The typeof the <paramref name="payload"/> being distributed.</typeparam>
        /// <param name="targetClientId">The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public void SendMessage<T>(string targetClientId, T payload) => SendMessage(new Message(Guid.NewGuid().ToString("N"), InternalClient.Id, targetClientId, new PayloadTypeInfo(typeof(T)), (new Converters.TypeSerializer<T>()).ToByteArray(payload)));
        /// <summary>
        /// Will attempt to communicate with every registered client.
        /// </summary>
        /// <param name="cancelToken">A token used to initiate canceling the <see cref="PingAllClients(Threading.ThreadCancelToken)"/> process.</param>
        /// <returns>A collection containing each client's id and a tuple with it's round trip duration and any possible exceptions that may have occurred.</returns>
        public Dictionary<string, Tuple<TimeSpan, Exception>> PingAllClients(Threading.ThreadCancelToken cancelToken)
        {
            var logs = new Logging.Logs();
            logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"Pinging All Clients: ({Service.RegisteredClientsCallbacks.Count()})");
            //
            var results = new Dictionary<string, Tuple<TimeSpan, Exception>>();
            // snapshot of the registered clients
            var list = new List<Tuple<IClientConfiguration, IServiceCallback>>(Service.RegisteredClientsCallbacks);
            var count = 0;
            foreach (var item in list)
            {
                if (cancelToken.CancelThread)
                {
                    break;
                }
                try
                {
                    var stopWatch = System.Diagnostics.Stopwatch.StartNew();
                    var clientDateCreated = item.Item2.Ping();
                    stopWatch.Stop();
                    results.Add(item.Item1.Id, new Tuple<TimeSpan, Exception>(stopWatch.Elapsed, null));
                    logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"#{++count}={item.Item1.Id}, Round Trip={stopWatch.Elapsed}, Date Started={clientDateCreated}, Runtime={(DateTime.Now - clientDateCreated)}");
                }
                catch (Exception ex)
                {
                    results.Add(item.Item1.Id, new Tuple<TimeSpan, Exception>(TimeSpan.MinValue, ex));
                }
            }
            logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"Completed Pinging All Clients.");
            RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_PingAll, logs));
            return results;
        }
        /// <summary>
        /// Requests that all registered clients restart.
        /// </summary>
        /// <param name="retryCount">The number of times a client should attempt to restart before it stops trying.</param>
        /// <param name="retryDuration">The duration to wait between retires.</param>
        public void RestartAllClients(int retryCount, TimeSpan retryDuration)
        {
            if (retryCount < 1)
            {
                throw new ArgumentOutOfRangeException("retryCount must be a positive value. (0 > retryCount <= Int32.MaxValue)");
            }
            if (retryDuration <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException("retryDuration must be a positive value. (TimeSpan.Zero > retryDuration <= TimeSpan.MaxValue)");
            }
            if (State == ChannelStates.Open)
            {
                var logs = new Logging.Logs
                {
                    { Logging.LogEntryType.Information, LogSourceId, $"Restarting All Clients: ({Service.RegisteredClientsCallbacks.Count()})" },
                    { Logging.LogEntryType.Information, LogSourceId, $"Retry Count={retryCount}" },
                    { Logging.LogEntryType.Information, LogSourceId, $"Retry Duration={retryDuration}" }
                };
                //
                InternalRestartAllClients(retryCount, retryDuration, logs);
                //
                RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_RestartAll, logs));
            }
            else
            {
                throw new InvalidOperationException("Unable to send command; the server is closed.");
            }
        }
        /// <summary>
        /// Requests that all registered clients close.
        /// </summary>
        public void CloseAllClients()
        {
            if (State == ChannelStates.Open)
            {
                var logs = new Logging.Logs();
                logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"Closing Clients: ({Service.RegisteredClientsCallbacks.Count()})");
                //
                InternalCloseAllClients();
                //
                logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"Completed Closing Clients.");
                RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_CloseAll, logs));
            }
            else
            {
                throw new InvalidOperationException("Unable to send command; the server is closed.");
            }
        }

        // TODO: consider adding a timeout

        /// <summary>
        /// Requests all registered clients report their <see cref="TransportStatistics"/>.
        /// </summary>
        /// <returns>A <see cref="ITransportStatisticsGroup"/> containing all registered clients communication statistics and server statistics.</returns>
        public ITransportStatisticsGroup RequestAllClientsTransportStatistics()
        {
            if (State == ChannelStates.Open)
            {
                var list = new List<ITransportStatistics>();
                // get all registered client's transport statistics
                var registeredClients = new List<Tuple<IClientConfiguration, IServiceCallback>>(Service.RegisteredClientsCallbacks);
                //
                var logs = new Logging.Logs();
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Request Clients Transport Statistics: ({registeredClients.Count})");
                var count = 0;
                //
                var internalClient = (from x in registeredClients
                                      where x.Item1.Id == InternalClient.Id
                                      select x).FirstOrDefault();
                if (internalClient != null)
                {
                    try
                    {
                        var internalClientStats = _TransportStatisticsConvertor.FromByteArray(
                                               TransportController.TransportConfiguration.Compressor.Decompress(
                                                   TransportController.TransportConfiguration.Encryptor.Decrypt(
                                                       internalClient.Item2.GetTransportStatistics())));
                        //
                        logs.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++count}={internalClientStats.ClientServerId}");
                        logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Date Started={internalClientStats.DateStarted} ({internalClientStats.RunTime})");
                        logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Incoming: Messages={internalClientStats.IncomingMessages}, Envelopes={internalClientStats.IncomingEnvelopes}, Bytes={Common.ByteCountHelper.ToString(internalClientStats.IncomingBytes)} ({internalClientStats.IncomingBytes})");
                        logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Outgoing: Messages={internalClientStats.OutgoingMessages}, Envelopes={internalClientStats.OutgoingEnvelopes}, Bytes={Common.ByteCountHelper.ToString(internalClientStats.OutgoingBytes)} ({internalClientStats.OutgoingBytes})");
                    }
                    catch { }
                }
                foreach (var item in registeredClients)
                {
                    // leave out the server's client
                    if (item.Item1.Id != InternalClient.Id)
                    {
                        try
                        {
                            var stats = _TransportStatisticsConvertor.FromByteArray(
                                        TransportController.TransportConfiguration.Compressor.Decompress(
                                            TransportController.TransportConfiguration.Encryptor.Decrypt(
                                                item.Item2.GetTransportStatistics())));
                            list.Add(stats);
                            //
                            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++count}={stats.ClientServerId}");
                            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Date Started={stats.DateStarted} ({stats.RunTime})");
                            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Incoming: Messages={stats.IncomingMessages}, Envelopes={stats.IncomingEnvelopes}, Bytes={Common.ByteCountHelper.ToString(stats.IncomingBytes)} ({stats.IncomingBytes})");
                            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Outgoing: Messages={stats.OutgoingMessages}, Envelopes={stats.OutgoingEnvelopes}, Bytes={Common.ByteCountHelper.ToString(stats.OutgoingBytes)} ({stats.OutgoingBytes})");
                        }
                        catch { }
                    }
                }
                //
                logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Completed Request Clients Transport Statistics.");
                RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_RequestAllStatistics, logs));
                //
                return new TransportStatisticsGroup(TransportController.TransportStatisticsSnapshot,
                                                    InternalClient.TransportController.TransportStatisticsSnapshot,
                                                    list);
            }
            else
            {
                throw new InvalidOperationException("Unable to send command; the server is closed.");
            }
        }
        #endregion
        #region Protected Methods
        /// <summary>
        /// When implemented, should perform any activities required when opening a server.
        /// </summary>
        /// <param name="exception">Contains any exceptions encountered; otherwise <b>null</b>.</param>
        /// <returns><b>True</b> if successful; otherwise <b>false</b>.</returns>
        protected abstract bool OnOpen(out Exception exception);
        /// <summary>
        /// When implemented, should perform any activities required when closing a server.
        /// </summary>
        protected abstract void OnClose();
        /// <summary>
        /// When implemented, should transmit a message using the server's internal client.
        /// </summary>
        /// <param name="message">The message to transmit using the server's internal client.</param>
        protected abstract void PushMessageToInternalClient(IMessage message);
        /// <summary>
        /// Gets the internal, <b>Actual Server</b>.
        /// </summary>
        protected abstract IService Service
        {
            get;
        }
        /// <summary>
        /// Gets the internal <see cref="IClient"/> used by the server to send and receive messages.
        /// </summary>
        protected abstract IClient InternalClient
        {
            get;
        }
        /// <summary>
        /// Will send a message using the <see cref="InternalClient"/>.
        /// </summary>
        /// <param name="sender">The type that invoked this method.</param>
        /// <param name="e">The <see cref="MessageEventArgs"/> containing the message to send.</param>
        protected void InternalClient_MessagePump(object sender, MessageEventArgs e)
        {
            // intercept messages from clients
            if (e.Message.TypeInfo.TypeName == NetworkingHelper.RequestAllTransportStatistics)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    InternalClient.SendMessage(new Message(Guid.NewGuid().ToString(),
                                                           InternalClient.Id,
                                                           e.Message.ClientId,
                                                           new PayloadTypeInfo(NetworkingHelper.ResponseAllTransportStatistics),
                                                           _ServerAllTransportStatisticsConvertor.ToByteArray(
                                                                    RequestAllClientsTransportStatistics())));
                });
            }
            else
            {
                RaiseMessageBusEvent(e);
            }
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Server");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // ChannelAddress
                result.Add(Address.Configuration);
                // ServerConfiguration
                result.Add(ServerConfiguration.Configuration);
                // TransportController
                result.Add(TransportController.Configuration);
                //
                return result;
            }
        }
        #endregion
        #region Private Methods
        private bool IsOpen => ((State == ChannelStates.Opening) ||
                                (State == ChannelStates.Registering) ||
                                (State == ChannelStates.Open) ||
                                (State == ChannelStates.Restarting) ||
                                (State == ChannelStates.Closing) ||
                                (State == ChannelStates.Unregistering));
        private void InternalRestartAllClients(int retryCount, TimeSpan retryDuration, Logging.Logs logs)
        {
            var list = new List<Tuple<IClientConfiguration, IServiceCallback>>(Service.RegisteredClientsCallbacks);
            var count = 0;
            foreach (var item in list)
            {
                // you must include the server's internal client in the restart cycle
                try
                {
                    //
                    logs.Add(dodSON.Core.Logging.LogEntryType.Information, LogSourceId, $"#{++count}={item.Item1.Id}");
                    item.Item2.InstructClientToRestart(retryCount, retryDuration);
                }
                catch { }
            }
        }
        private void InternalCloseAllClients()
        {
            var list = new List<Tuple<IClientConfiguration, IServiceCallback>>(Service.RegisteredClientsCallbacks);
            foreach (var item in list)
            {
                // DO NOT close the server's internal client
                if (item.Item1.Id != InternalClient.Id)
                {
                    try
                    {
                        item.Item2.InstructClientToClose();
                    }
                    catch { }
                }
            }
            // TODO: there has to be a better way to ensure each client is closed before proceeding
            Threading.ThreadingHelper.Sleep(TimeSpan.FromSeconds(3));
        }
        #endregion
    }
}
