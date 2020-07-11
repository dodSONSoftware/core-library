using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Facilitates communication by servicing System Requests, querying the <see cref="ComponentManagement.IComponentManager"/> and it's <see cref="IService"/>s, performing a variety of commands and returning System Responses.
    /// </summary>
    /// <example>
    /// <para>
    /// The following example will first create a Component Manager and inject it into a new Service Manager and then create a worker client registered to the Component Manager’s Main Server communication channel. 
    /// The worker client will be used to log into the Service Manager and send commands and receive responses from the Service Manager. 
    /// Only clients that log into the Service Manager can send commands to the Service Manager, as long as it has the correct SessionId and that Session is still alive.
    /// <br/><br/>
    /// Once the Service Manager is started and the worker client has successfully logged in, the user will be presented with a looping display of the Service Manager’s Main Server’s Network Statistics along with a few commands.
    /// <br/><br/>
    /// All commands will have their results entered into the log.
    /// <br/><br/>
    /// The Service Manager commands include <b>IsAlive</b>, which takes a string with the SessionId to check and returns a boolean. 
    /// The <b>Echo</b> command simply returns the data it received, unchanged. The <b>Ping</b> command will take a dodSON.Core.ServiceManagement.RequestResponseTypes.Ping object, execute its MarkReceivedTime() function and return it; it is important to execute the returned Ping object’s MarkEndTime() function just as soon as it is received, that way the other properties of the Ping object will be correct. 
    /// <br/><br/>
    /// The <b>Service Manager Details</b> command will return a dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_Details object populated with information about the responding Service Manager and its Services, separated into Extension and Plugin lists.
    /// <br/><br/>
    /// </para>
    /// <note type="note">
    /// To create the packages, first use the code below to create the Playground_Extension01.dll and the Playground_Worker01.dll.
    /// Use the configurations below to create the package.config.xml and the Custom.configuration.xml files. 
    /// Feel free to remove the dependency properties from the package.config.xml file, they are not actual required; in fact, the dependency package were empty, expect for the package.config.xml file.
    /// <br/><br/>
    /// See the dodSON.Core.Packaging namespace for information about creating packages. 
    /// Specifically, the <b>dodSON.Core.Packaging.PackageProvider</b> and the <b>dodSON.Core.Packaging.ConnectedPackage</b> classes which provide methods to create and control packages.
    /// <br/><br/>
    /// Also, other clients are not strictly required for this example to work, but, without other clients the Service Manager Services function will contain empty Extensions or Plugins list. 
    /// However, all of the commands will still function whether there are any clients or not.
    /// <br/><br/>
    /// The package should include the following files:
    /// <br/><br/>
    /// <ul>
    ///     <li>dodSON.Core.dll</li>
    ///     <li>Playground_Extension01.dll (code included below)</li>
    ///     <li>Playground_Worker01.dll (code included below)</li>
    ///     <li>package.config.xml (included below)</li>
    ///     <li>Custom.configuration.xml (included below) (is optional)</li>
    /// </ul>
    /// </note>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // ################################################################
    ///     // ######## CHANGE THIS VALUE     
    ///     // ######## Make sure this directory is empty; except for the packages.
    ///     // ######## See the ( dodSON.Core.Packaging ) namespace for information on creating packages.
    ///     // ########
    ///     // ######## Package Path = rootPath + "Packages"
    ///     var rootPath = @"C:\(WORKING)\Dev\ServiceManagement";
    ///     // ################################################################
    /// 
    ///     // --------------------------------
    /// 
    ///     if (!System.IO.Directory.Exists(rootPath))
    ///     {
    ///         throw new System.IO.FileNotFoundException($"path={rootPath}");
    ///     }
    ///     // --------------------------------
    /// 
    ///     // #### create Component Manager
    /// 
    ///     Console.Clear();
    ///     Console.WriteLine("Service Manager Example");
    ///     Console.WriteLine("dodSON Software Core Library");
    ///     Console.WriteLine($"Copyright (c) dodSON Software 2016-2018");
    ///     Console.WriteLine($"=======================================");
    ///     Console.WriteLine($"{Environment.NewLine}Creating Component Manager.");
    ///     dodSON.Core.ComponentManagement.IComponentManager componentManager = CreateComponentManager(rootPath, out string logFilename);
    ///     LogFilename = logFilename;
    ///     Console.WriteLine($"Log = {logFilename}");
    /// 
    ///     // #### output Component Manager configuration
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}Saving Component Manager Configuration Files.");
    ///     System.IO.File.WriteAllBytes(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.bin"), (new dodSON.Core.Configuration.BinaryConfigurationSerializer()).Serialize(componentManager.Configuration));
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.csv"), (new dodSON.Core.Configuration.CsvConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.ini"), (new dodSON.Core.Configuration.IniConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.xml"), (new dodSON.Core.Configuration.XmlConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    /// 
    ///     // #### create Service Manager
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}Creating Service Manager.");
    ///     byte[] serviceManagerLoginEvidence = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(1024);
    ///     TimeSpan serviceManagerRequestsPurgeInterval = TimeSpan.FromSeconds(5);
    ///     TimeSpan serviceManagerRequestTimeout = TimeSpan.FromSeconds(10);
    ///     bool serviceManagerExecuteRemainingRequestsOnStop = false;
    ///     TimeSpan serviceManagerSessionTimeLimit = TimeSpan.FromSeconds(30);
    ///     string serviceManagerLogSourceId = "ServiceManager";
    ///     bool serviceManagerAuditMessages = true;
    ///     bool serviceManagerAuditDebugMessages = false;
    ///     string serviceManagerServiceManagerId = $"ServiceManager_{Guid.NewGuid().ToString("N")}";
    ///     //
    ///     dodSON.Core.ServiceManagement.IServiceManager serviceManager = new dodSON.Core.ServiceManagement.ServiceManager(componentManager,
    ///                                                                                                                     serviceManagerLoginEvidence,
    ///                                                                                                                     serviceManagerRequestsPurgeInterval,
    ///                                                                                                                     serviceManagerRequestTimeout,
    ///                                                                                                                     serviceManagerExecuteRemainingRequestsOnStop,
    ///                                                                                                                     serviceManagerSessionTimeLimit,
    ///                                                                                                                     serviceManagerLogSourceId,
    ///                                                                                                                     serviceManagerAuditMessages,
    ///                                                                                                                     serviceManagerAuditDebugMessages,
    ///                                                                                                                     serviceManagerServiceManagerId);
    ///     // #### start Service Manager
    /// 
    ///     var MainSourceId = "Application";
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}    IsRunning={serviceManager.IsRunning}");
    ///     Console.WriteLine($"Starting Service Manager.");
    ///     if (!serviceManager.TryStart(out Exception serviceManagerTryStartException))
    ///     {
    ///         throw new Exception($"Unable to start ServiceManager: {serviceManagerTryStartException.Message}");
    ///     }
    ///     Console.WriteLine($"    IsRunning={serviceManager.IsRunning}");
    ///     serviceManager.MessageBus += ServiceManager_MessageBus;
    /// 
    ///     // #### create login client
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}Creating Login Client.");
    ///     var clientConfiguration = new dodSON.Core.Networking.ClientConfiguration("LoginClient", false);
    ///     var loginClient = serviceManager.ComponentManager.CommunicationController.CreateSharedClient(clientConfiguration);
    ///     if (loginClient.Open(out Exception workingClientOpenException) != dodSON.Core.Networking.ChannelStates.Open)
    ///     {
    ///         throw workingClientOpenException;
    ///     }
    /// 
    ///     // #### output login client configuration
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}Saving Login Client Configuration Files.");
    ///     System.IO.File.WriteAllBytes(System.IO.Path.Combine(rootPath, "LoginClient.configuration.bin"), (new dodSON.Core.Configuration.BinaryConfigurationSerializer()).Serialize(loginClient.Configuration));
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "LoginClient.configuration.csv"), (new dodSON.Core.Configuration.CsvConfigurationSerializer()).Serialize(loginClient.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "LoginClient.configuration.ini"), (new dodSON.Core.Configuration.IniConfigurationSerializer()).Serialize(loginClient.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "LoginClient.configuration.xml"), (new dodSON.Core.Configuration.XmlConfigurationSerializer()).Serialize(loginClient.Configuration).ToString());
    /// 
    ///     // #### log in to the service manager
    /// 
    ///     Console.Write($"{Environment.NewLine}Logging into Service Manager: ");
    ///     System.Threading.CancellationTokenSource cancellationTokenSource = new System.Threading.CancellationTokenSource();
    ///     var cancellationToken = cancellationTokenSource.Token;
    ///     var serviceManagerClientId = "";
    /// 
    ///     if (Login(loginClient,
    ///               serviceManagerClientId,
    ///               serviceManagerLoginEvidence,
    ///               TimeSpan.FromSeconds(35),
    ///               cancellationToken,
    ///               out string serviceManagerId,
    ///               out string resultsDescription,
    ///               out dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginResponse loginResponse))
    ///     {
    ///         // #### Login Successful
    /// 
    ///         Console.WriteLine($"{resultsDescription}");
    ///         Console.WriteLine($"{Environment.NewLine}ServiceManagerId={serviceManagerId}");
    ///         Console.WriteLine($"SessionId={loginResponse.SessionId}");
    ///         Console.WriteLine($"ServiceManagerClientId={serviceManagerClientId}");
    ///         Console.Write($"{Environment.NewLine}press anykey&gt;");
    ///         Console.ReadKey(true);
    ///         Console.WriteLine();
    /// 
    ///         // ****************************************************************************************************
    /// 
    ///         // ******** Loop Until User Terminated
    ///         var startTime = DateTimeOffset.Now;
    ///         var displayRefreshTimer = System.Diagnostics.Stopwatch.StartNew();
    ///         var loopIt = true;
    ///         while (loopIt)
    ///         {
    ///             if (displayRefreshTimer.Elapsed &gt; TimeSpan.FromSeconds(1))
    ///             {
    ///                 displayRefreshTimer.Restart();
    ///                 RefreshDisplay(startTime, componentManager.MainServer);
    ///             }
    ///             if (Console.KeyAvailable)
    ///             {
    ///                 switch (Console.ReadKey(true).Key)
    ///                 {
    ///                     case ConsoleKey.Escape:
    ///                         // #### Terminate Loop
    ///                         loopIt = false;
    ///                         break;
    /// 
    ///                     case ConsoleKey.F1:
    ///                         // #### Send a Network Level Ping Command
    ///                         var cancelToken = new dodSON.Core.Threading.ThreadCancelToken();
    ///                         componentManager.MainServer.PingAllClients(cancelToken);
    ///                         break;
    /// 
    ///                     case ConsoleKey.F4:
    ///                         // #### Send a Network Level Restart All Clients Command
    ///                         componentManager.MainServer.RestartAllClients(5, TimeSpan.FromSeconds(5));
    ///                         break;
    /// 
    ///                     case ConsoleKey.F5:
    ///                         // #### Service Manager IsAlive Command
    ///                         loginClient.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest(dodSON.Core.ServiceManagement.RequestResponseCommands.IsSessionAlive, loginResponse.SessionId));
    ///                         break;
    /// 
    ///                     case ConsoleKey.F6:
    ///                         // #### Service Manager Echo/Ping/&lt;String&gt; Command
    ///                         loginClient.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest(dodSON.Core.ServiceManagement.RequestResponseCommands.Echo, Guid.NewGuid()));
    ///                         // #### Service Manager Ping Command
    ///                         loginClient.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest(dodSON.Core.ServiceManagement.RequestResponseCommands.Ping, new dodSON.Core.ServiceManagement.RequestResponseTypes.Ping()));
    ///                         // #### Send a &lt; String &gt; Message
    ///                         loginClient.SendMessage("", "################ Hello from Main Application!");
    ///                         break;
    /// 
    ///                     case ConsoleKey.F8:
    ///                         // #### Service Manager Details Command
    ///                         // generate an 'Invalid ClientId' error
    ///                         serviceManager.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest(loginResponse.SessionId, dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Details));
    ///                         // generate an 'Invalid SessionId' error
    ///                         loginClient.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest("**** Bad Session Id ****", dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Details));
    ///                         // get ServiceManager_Details
    ///                         loginClient.SendMessage("", new dodSON.Core.ServiceManagement.ServiceRequest(loginResponse.SessionId, dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Details));
    ///                         break;
    /// 
    ///                     default:
    ///                         break;
    ///                 }
    ///             }
    ///         }
    /// 
    ///         // #### Final Display
    ///         displayRefreshTimer.Stop();
    ///         RefreshDisplay(startTime, componentManager.MainServer);
    ///     }
    ///     else
    ///     {
    ///         // #### Login Failed
    ///         Console.WriteLine($"{Environment.NewLine}{resultsDescription}");
    ///     }
    /// 
    ///     // ****************************************************************************************************
    ///     
    ///     // dispose of cancellationTokenSource
    ///     cancellationTokenSource.Dispose();
    ///        
    ///     // #### Close Login Client
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}{Environment.NewLine}=======================================");
    ///     Console.WriteLine($"{Environment.NewLine}Closing login Client.");
    ///     loginClient.Close(out Exception loginClientCloseException);
    /// 
    ///     // #### Stop the Service Manager
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}    IsRunning={serviceManager.IsRunning}");
    ///     Console.WriteLine($"Stopping Service Manager.");
    /// 
    ///     if (!serviceManager.TryStop(out Exception serviceManagerTryStopException))
    ///     {
    ///         throw new Exception($"Unable to stop ServiceManager: {serviceManagerTryStopException.Message}");
    ///     }
    ///     Console.WriteLine($"    IsRunning={serviceManager.IsRunning}; ReceivedRequests={serviceManager.ReceivedRequests}");
    /// 
    ///     // #### end-of-code
    /// 
    ///     Console.WriteLine($"{Environment.NewLine}=======================================");
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    ///     Console.WriteLine();
    /// 
    /// 
    /// 
    ///     // ########################
    ///     //     internal methods    
    ///     // ########################
    /// 
    ///     void ServiceManager_MessageBus(object sender, dodSON.Core.Networking.MessageEventArgs e)
    ///     {
    ///         var response = e.Message.PayloadMessage&lt;dodSON.Core.ServiceManagement.ServiceResponse&gt;();
    ///         if (response != null)
    ///         {
    ///             var logWriter = (sender as dodSON.Core.ServiceManagement.IServiceManager)?.LogWriter;
    ///             switch (response.Command)
    ///             {
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.Echo:
    ///                     // #### SERVICE MANAGER ECHO COMMAND
    ///                     var echoResponseDataTypeStr = (response.Data != null) ? $", DataType={response.Data.GetType().FullName}" : "";
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"dodSON.Core.ServiceManagement.RequestResponseCommands.Echo, HasData={response.Data != null}{echoResponseDataTypeStr}");
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.Ping:
    ///                     // #### SERVICE MANAGER PING COMMAND
    ///                     var pingObj = response.Data as dodSON.Core.ServiceManagement.RequestResponseTypes.Ping;
    ///                     if (pingObj != null)
    ///                     {
    ///                         pingObj.MarkEndTime();
    ///                         logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"{pingObj}");
    ///                     }
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.IsSessionAlive:
    ///                     // #### SERVICE MANAGER IS SESSION ALIVE COMMAND
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"dodSON.Core.ServiceManagement.RequestResponseCommands.IsSessionAlive, Results={(bool)response.Data}");
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.Error:
    ///                     // #### SERVICE MANAGER ERROR COMMAND
    ///                     var smError = response.Data as dodSON.Core.ServiceManagement.RequestResponseTypes.Error;
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"{smError}");
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_LoginRequest:
    ///                     // #### SERVICE MANAGER LOGIN REQUEST COMMAND
    ///                     var smLoginRequestResponse = response.Data as dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginRequestResponse;
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"{smLoginRequestResponse}");
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Login:
    ///                     // #### SERVICE MANAGER lOGIN COMMAND
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Login, DataType={response.Data.GetType().FullName}");
    ///                     break;
    /// 
    ///                 case dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Details:
    ///                     // #### SERVICE MANAGER DETAILS COMMAND
    ///                     var smDetails = response.Data as dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_Details;
    ///                     var smDetailsLogs = new dodSON.Core.Logging.Logs();
    ///                     smDetailsLogs.Add(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"{smDetails}");
    ///                     for (int i = 0; i &lt; smDetails.Services.Extensions.Count(); i++)
    ///                     {
    ///                         smDetailsLogs.Add(dodSON.Core.Logging.LogEntryType.Debug, MainSourceId, $"    Extension #{i + 1}={smDetails.Services.Extensions.ElementAt(i)}");
    ///                     }
    ///                     for (int i = 0; i &lt; smDetails.Services.Plugins.Count(); i++)
    ///                     {
    ///                         smDetailsLogs.Add(dodSON.Core.Logging.LogEntryType.Debug, MainSourceId, $"    Plugin #{i + 1}={smDetails.Services.Plugins.ElementAt(i)}");
    ///                     }
    ///                     logWriter.Write(smDetailsLogs);
    ///                     break;
    /// 
    ///                 default:
    ///                     // #### UNKOWN
    ///                     var defaultDataTypeStr = (response.Data != null) ? $", DataType={response.Data.GetType().FullName}" : "";
    ///                     logWriter.Write(dodSON.Core.Logging.LogEntryType.AuditSuccess, MainSourceId, $"________________________{response.GetType().FullName}:Command={response.Command}, HasData{response.Data != null}{defaultDataTypeStr}");
    ///                     break;
    ///             }
    ///         }
    ///     }
    /// }
    /// 
    /// // ------------------------------------------------------------------------------------------------------------------------
    /// 
    /// private static dodSON.Core.ComponentManagement.IComponentManager CreateComponentManager(string rootPath,
    ///                                                                                         out string logFilename)
    /// {
    ///     var timeStamp = DateTimeOffset.Now;
    /// 
    ///     // create actual logger
    ///     var logShortFilename = $"{timeStamp.ToString("yyyyMMdd_HHmmss")}.log";
    ///     logFilename = System.IO.Path.Combine(rootPath, logShortFilename);
    ///     var writeLogEntriesUsingLocalTime = true;
    ///     var logger = new dodSON.Core.Logging.FileEventLog.Log(logFilename, writeLogEntriesUsingLocalTime);
    /// 
    ///     // create a log archive store
    ///     var logArchiveBackendStorageZipFilename = System.IO.Path.Combine(rootPath, $"ComponentManagment.{logShortFilename}.archive.zip");
    ///     var logArchiveExtractionRootDirectory = System.IO.Path.GetTempPath();
    ///     var logArchiveSaveOriginalFilenames = false;
    ///     var logArchiveExtensionsToStore = new string[0];
    ///     var logArchiveFileStore = new dodSON.Core.FileStorage.MSdotNETZip.FileStore(logArchiveBackendStorageZipFilename, logArchiveExtractionRootDirectory, logArchiveSaveOriginalFilenames, logArchiveExtensionsToStore);
    /// 
    ///     // create a file archive filename factory
    ///     var archiveFilenameFactory = new dodSON.Core.ComponentManagement.ArchiveFilenameFactory();
    /// 
    ///     // create log controller settings
    ///     var logLogSourceId = "Logs";
    ///     var autoCacheFlushLogMax = 15;
    ///     var autoCacheFlushTimeLimit = TimeSpan.FromSeconds(10);
    ///     var writePrimaryLogEntriesUsingLocalTime = true;
    ///     var writeArchivedLogEntriesUsingLocalTime = false;
    ///     var autoArchiveEnabled = true;
    ///     var autoArchiveLogMax = 100000;
    ///     var truncateLogArchive = false;
    ///     var truncateLogArchiveMaximumFiles = 100;
    ///     var truncateLogArchivePercentageToRemove = 0;
    ///     var logControllerSettings = new dodSON.Core.ComponentManagement.LogControllerSettings(logLogSourceId,
    ///                                                                                           autoCacheFlushLogMax,
    ///                                                                                           autoCacheFlushTimeLimit,
    ///                                                                                           writePrimaryLogEntriesUsingLocalTime,
    ///                                                                                           autoArchiveEnabled,
    ///                                                                                           autoArchiveLogMax,
    ///                                                                                           writeArchivedLogEntriesUsingLocalTime,
    ///                                                                                           truncateLogArchive,
    ///                                                                                           truncateLogArchiveMaximumFiles,
    ///                                                                                           truncateLogArchivePercentageToRemove);
    /// 
    ///     // ******** CREATE LOG CONTROLLER
    ///     var logController = new dodSON.Core.ComponentManagement.LogController(logger, logArchiveFileStore, archiveFilenameFactory, logControllerSettings);
    /// 
    ///     // ******** CREATE COMMUNICATION CONTROLLER PARTS
    /// 
    ///     // create server configuration
    ///     var serverType = typeof(dodSON.Core.Networking.NamedPipes.Server);
    ///     var clientType = typeof(dodSON.Core.Networking.NamedPipes.Client);
    ///     var ipAddress = dodSON.Core.Networking.NetworkingHelper.DefaultIpAddress;
    ///     var port = dodSON.Core.Networking.NetworkingHelper.RecommendedMinumumPortValue;
    ///     var name = "ComponentManagementServer";
    ///     var sharedServerChannelAddress = new dodSON.Core.Networking.ChannelAddress(ipAddress, port, name);
    ///     var serverId = $"{name}_{Guid.NewGuid().ToString("N")}";
    ///     var sharedServerConfiguration = new dodSON.Core.Networking.ServerConfiguration(serverId);
    /// 
    ///     // create transport configuration
    ///     var transportHashAlgorithm = new System.Security.Cryptography.SHA512Managed();
    ///     var transportPassword = new System.Security.SecureString();
    ///     dodSON.Core.Cryptography.CryptographyExtensions.AppendChars(transportPassword, "BadPa$$w0rd".ToCharArray(), true);
    ///     var transportSalt = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(128);
    ///     var transportSaltedPassword = new dodSON.Core.Cryptography.SaltedPassword(transportHashAlgorithm, transportPassword, transportSalt);
    ///     var transportSymmetricAlgorithmType = typeof(System.Security.Cryptography.RijndaelManaged);
    ///     var transportEncryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(transportSaltedPassword, transportSymmetricAlgorithmType);
    ///     var compressorType = typeof(dodSON.Core.Compression.DeflateStreamCompressor);
    ///     var useChunking = true;
    ///     var chunkSize = dodSON.Core.Networking.NetworkingHelper.MinimumTransportEnvelopeChunkSize;
    ///     var envelopeCacheTimeLimit = TimeSpan.FromSeconds(5);
    ///     var seenMessageCacheTimeLimit = TimeSpan.FromSeconds(15);
    ///     var transportConfiguration = new dodSON.Core.Networking.TransportConfiguration(transportEncryptorConfiguration, compressorType, useChunking, chunkSize, envelopeCacheTimeLimit, seenMessageCacheTimeLimit);
    /// 
    ///     // ******** CREATE CHALLENGE CONTROLLER
    ///     var actualEvidence = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(96);
    ///     var passwordChallengeController = new dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController(actualEvidence);
    /// 
    ///     // ******** CREATE REGISTRATION CONTROLLER
    ///     var registrationHashAlgorithm = new System.Security.Cryptography.MD5Cng();
    ///     var registrationPassword = new System.Security.SecureString();
    ///     dodSON.Core.Cryptography.CryptographyExtensions.AppendChars(registrationPassword, "!BAD-PA$$W0RD!".ToCharArray(), true);
    ///     var registrationSalt = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(128);
    ///     var registrationSaltedPassword = new dodSON.Core.Cryptography.SaltedPassword(registrationHashAlgorithm, registrationPassword, registrationSalt);
    ///     var registrationSymmetricAlgorithmType = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///     var registrationEncryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(registrationSaltedPassword, registrationSymmetricAlgorithmType);
    ///     var registrationEncryptor = new dodSON.Core.Cryptography.StackableEncryptor(registrationEncryptorConfiguration);
    ///     var encryptedRegistrationController = new dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController(transportConfiguration, passwordChallengeController);
    /// 
    ///     // ******** CREATE TRANSPORT CONTROLLER
    ///     var transportController = new dodSON.Core.Networking.TransportController(transportConfiguration, encryptedRegistrationController);
    /// 
    ///     // ******** CREATE COMMUNICATION CONTROLLER
    ///     var communicationControllerLogSourceId = "Network";
    ///     var communicationController = new dodSON.Core.ComponentManagement.CommunicationController(serverType, clientType, sharedServerChannelAddress, sharedServerConfiguration, transportController, communicationControllerLogSourceId);
    /// 
    ///     // ******** CREATE INSTALLER
    ///     var installType = dodSON.Core.Installation.InstallType.HighestVersionOnly;
    ///     var cleanInstall = false;
    ///     var enablePackageUpdates = true;
    ///     var removeOrphanedPackages = false;
    ///     var updatePackagesBeforeInstalling = true;
    ///     var installationSettings = new dodSON.Core.Installation.InstallationSettings(installType, cleanInstall, enablePackageUpdates, removeOrphanedPackages, updatePackagesBeforeInstalling);
    ///     var installationRootPath = System.IO.Path.Combine(rootPath, $"Install");
    ///     // creating installation path
    ///     if (!System.IO.Directory.Exists(installationRootPath))
    ///     {
    ///         System.IO.Directory.CreateDirectory(installationRootPath);
    ///         dodSON.Core.Threading.ThreadingHelper.Sleep(100);
    ///     }
    ///     // 
    ///     var configurationFilename = "package.config.xml";
    ///     var serializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///     var installerLogSourceId = "Install";
    ///     var installer = new dodSON.Core.Installation.Installer(installationRootPath, configurationFilename, serializer, installerLogSourceId);
    /// 
    ///     // ******** CREATE PACKAGE PROVIDER
    ///     var packagesSourceDirectory = System.IO.Path.Combine(rootPath, "Packages");
    ///     var packagesExtractionDirectory = System.IO.Path.GetTempPath();
    ///     var packagesSaveOriginalFilenames = false;
    ///     var packagesFileStore = new dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore(packagesSourceDirectory, packagesExtractionDirectory, packagesSaveOriginalFilenames);
    ///     var packagesConfigurationFilename = configurationFilename;
    ///     var packagesSerializer = serializer;
    ///     var packagesFileStoreProvider = new dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider();
    ///     var packageProvider = new dodSON.Core.Packaging.PackageProvider(packagesFileStore, packagesConfigurationFilename, packagesSerializer, packagesFileStoreProvider);
    /// 
    ///     // ******** CREATE COMPONENT CONTROLLER
    ///     var componentControllerLogSourceId = "Components";
    ///     var componentControllerCustomConfigurationFilename = "Custom.configuration.xml";
    ///     var componentControllerCustomConfigurationSerializerType = typeof(dodSON.Core.Configuration.XmlConfigurationSerializer);
    ///     var componentController = new dodSON.Core.ComponentManagement.ComponentController(componentControllerLogSourceId, componentControllerCustomConfigurationFilename, componentControllerCustomConfigurationSerializerType);
    /// 
    ///     // ######## CREATE COMPONENT MANAGER
    ///     var componentManagerLogSourceId = "System";
    ///     return new dodSON.Core.ComponentManagement.ComponentManager(logController, componentManagerLogSourceId, packageProvider, installationSettings, installer, communicationController, componentController);
    /// }
    /// 
    /// // ------------------------------------------------------------------------------------------------------------------------
    /// 
    /// /// &lt;summary&gt;
    /// /// Performs all of the logic to log in to a IServiceManager.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="loginClient"&gt;The IClient to use to perform the communication with.&lt;/param&gt;
    /// /// &lt;param name="serviceManagerClientId"&gt;The ClientId of the IServiceManager to log in to.&lt;/param&gt;
    /// /// &lt;param name="loginEvidence"&gt;The byte array used to prove you have permission to access the IServiceManager.&lt;/param&gt;
    /// /// &lt;param name="requestTimeout"&gt;The length of time to wait before aborting a request and failing the login process.&lt;/param&gt;
    /// /// &lt;param name="cancellationToken"&gt;Allows the log in process to be canceled.&lt;/param&gt;
    /// /// &lt;param name="serviceManagerId"&gt;A value which uniquely identifies a IServiceManager.&lt;/param&gt;
    /// /// &lt;param name="resultsDescription"&gt;Details the success or failure of the log in attempt.&lt;/param&gt;
    /// /// &lt;param name="loginResults"&gt;If return value is true, a RequestResponseTypes.ServiceManager_LoginResponse; otherwise, if return value is false this value will be null.&lt;/param&gt;
    /// /// &lt;returns&gt;Returns whether the log in attempt was successful or not. True indicates a successful log in; false indicates a log in failure. See the resultsDescription for details.&lt;/returns&gt;
    /// public static bool Login(dodSON.Core.Networking.IClient loginClient,
    ///                          string serviceManagerClientId,
    ///                          byte[] loginEvidence,
    ///                          TimeSpan requestTimeout,
    ///                          System.Threading.CancellationToken cancellationToken,
    ///                          out string serviceManagerId,
    ///                          out string resultsDescription,
    ///                          out dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginResponse loginResults)
    /// {
    ///     // NOTE: This is a copy of the dodSON.Core.ServiceManagement.ServiceManagementHelper.Login(...) function.
    ///     //       It has been provided in this example for clarity with how the login process works.
    /// 
    ///     object localClientData = null;
    ///     string localServiceManagerId = "";
    ///     string groupId = Guid.NewGuid().ToString("N");
    ///     // 
    ///     try
    ///     {
    ///         // #### connect to the message bus, temporarily
    ///         loginClient.MessageBus += LocalMessageBus;
    /// 
    ///         // ######## LOGIN REQUEST
    /// 
    ///         // #### create thread to send and wait for "LoginRequest"
    ///         localClientData = null;
    ///         var loginRequestTask = Task.Factory.StartNew(() =&gt;
    ///         {
    ///             // send "LoginRequest" message to specific Service Manager
    ///             loginClient.SendMessage(serviceManagerClientId, new dodSON.Core.ServiceManagement.ServiceRequest("", groupId, dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_LoginRequest, "", null));
    ///             // wait for response
    ///             while (true)
    ///             {
    ///                 if (localClientData != null)
    ///                 {
    ///                     return (localClientData as dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginRequestResponse);
    ///                 }
    ///             }
    ///         }, cancellationToken);
    ///         // #### wait for thread to complete, timeout or get canceled
    ///         if (loginRequestTask.Wait((int)requestTimeout.TotalMilliseconds, cancellationToken))
    ///         {
    ///             // #### "LoginRequest" completed
    ///             var loginRequestResult = loginRequestTask.Result;
    ///             // #### create thread to send and wait for "Login"
    ///             localClientData = null;
    ///             var task = Task.Factory.StartNew&lt;object&gt;(() =&gt;
    ///             {
    /// 
    ///                 // ######## LOGIN
    /// 
    ///                 // #### create "Login"
    ///                 // create public/private keys
    /// 
    ///                 //var clientPublicPrivateKeys = dodSON.Core.ServiceManagement.ServiceManagementHelper.GeneratePublicPrivateKeys();
    ///                 var clientPublicPrivateKeys = GeneratePublicPrivateKeys();
    ///                 // create "Login"
    ///                 var loginRequest = new dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_Login(clientPublicPrivateKeys.Item1, loginEvidence);
    ///                 // convert "Login" to byte[]
    ///                 var loginRequestByteArray = (new dodSON.Core.Converters.TypeSerializer&lt;dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_Login&gt;()).ToByteArray(loginRequest);
    ///                 // *** prepare for "tunneling" transport
    ///                 // prepare "Login" for secure transport
    /// 
    ///                 //var transportData = dodSON.Core.ServiceManagement.ServiceManagementHelper.PrepareForTransport(loginRequestByteArray, loginRequestResult.XmlPublicKey);
    ///                 var transportData = PrepareForTransport(loginRequestByteArray, loginRequestResult.XmlPublicKey);
    ///                 // #### send "Login" message to specific Service Manager
    ///                 loginClient.SendMessage(serviceManagerClientId, new dodSON.Core.ServiceManagement.ServiceRequest("", groupId, dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Login, "", transportData));
    ///                 // #### wait for response
    ///                 while (true)
    ///                 {
    ///                     if (localClientData != null)
    ///                     {
    ///                         if (localClientData is List&lt;byte[]&gt;)
    ///                         {
    ///                             // #### received tunnel encrypted data
    ///                             // decrypt, using the private key, and restore the list of byte arrays to a single byte array 
    /// 
    ///                             //var restoredData = dodSON.Core.ServiceManagement.ServiceManagementHelper.RestoreFromTransport((List&lt;byte[]&gt;)localClientData, clientPublicPrivateKeys.Item2);
    ///                             var restoredData = RestoreFromTransport((List&lt;byte[]&gt;)localClientData, clientPublicPrivateKeys.Item2);
    ///                             // convert the decrypted and restored byte array into a 'ServiceManagerLoginResponse'
    ///                             return (new dodSON.Core.Converters.TypeSerializer&lt;dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginResponse&gt;()).FromByteArray(restoredData);
    ///                         }
    ///                         else if (localClientData is dodSON.Core.ServiceManagement.RequestResponseTypes.Error)
    ///                         {
    ///                             // #### error
    ///                             return (localClientData as dodSON.Core.ServiceManagement.RequestResponseTypes.Error);
    ///                         }
    ///                     }
    ///                 }
    ///             }, cancellationToken);
    ///             // #### wait for thread to complete, timeout or get canceled
    ///             if (task.Wait((int)requestTimeout.TotalMilliseconds, cancellationToken))
    ///             {
    ///                 // #### "Login" completed
    ///                 if (task.Result is dodSON.Core.ServiceManagement.RequestResponseTypes.Error)
    ///                 {
    ///                     // error from ServiceManager
    ///                     loginResults = null;
    ///                     serviceManagerId = localServiceManagerId;
    ///                     resultsDescription = $"Login Failed: {(task.Result as dodSON.Core.ServiceManagement.RequestResponseTypes.Error)?.Description}";
    ///                     return false;
    ///                 }
    ///                 else
    ///                 {
    ///                     // standard from Login System
    ///                     loginResults = task.Result as dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginResponse;
    ///                     serviceManagerId = localServiceManagerId;
    ///                     resultsDescription = "Login Successful";
    ///                     return true;
    ///                 }
    ///             }
    ///             else
    ///             {
    ///                 // "Login" timed out or was canceled
    ///                 loginResults = null;
    ///                 serviceManagerId = localServiceManagerId;
    ///                 resultsDescription = "Login command timed out or was canceled.";
    ///                 return false;
    ///             }
    ///         }
    ///         else
    ///         {
    ///             // "LoginRequest" timed out or was canceled
    ///             loginResults = null;
    ///             serviceManagerId = localServiceManagerId;
    ///             resultsDescription = "Login Request command timed out or was canceled.";
    ///             return false;
    ///         }
    ///     }
    ///     finally
    ///     {
    ///         // #### disconnect from the message bus
    ///         loginClient.MessageBus -= LocalMessageBus;
    ///     }
    /// 
    ///     // ########################
    ///     // internal methods
    ///     // ########################
    /// 
    ///     void LocalMessageBus(object sender, dodSON.Core.Networking.MessageEventArgs e)
    ///     {
    ///         var response = e.Message.PayloadMessage&lt;dodSON.Core.ServiceManagement.ServiceResponse&gt;();
    ///         if (response != null)
    ///         {
    ///             localServiceManagerId = response.ServiceManagerId;
    ///             // only work with these commands
    ///             if ((response.Command == dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_LoginRequest) ||
    ///                 (response.Command == dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Login) ||
    ///                 (response.Command == dodSON.Core.ServiceManagement.RequestResponseCommands.Error))
    ///             {
    ///                 localClientData = response.Data;
    ///             }
    ///         }
    ///     }
    /// }
    /// 
    /// // ################################
    /// 
    /// /// &lt;summary&gt;
    /// /// Generates a Public and a Private key pair used for asymmetrical encryption.
    /// /// &lt;/summary&gt;
    /// /// &lt;returns&gt;A &lt;see cref="Tuple{T1, T2}"/&gt; containing the generated Public and Private Keys, respectively.&lt;/returns&gt;
    /// public static Tuple&lt;string, string&gt; GeneratePublicPrivateKeys()
    /// {
    ///     // NOTE: This is a copy of the dodSON.Core.ServiceManagement.ServiceManagementHelper.GeneratePublicPrivateKeys() function.
    ///     //       Using the dodSON.Core.ServiceManagement.ServiceManagementHelper.DefaultRSAKeyLengthInBits as the dwKeySize value.
    ///     //       It has been provided in this example for clarity with how the keys are created.
    /// 
    ///     // create an asymmetric encryptor: RSA Algorithm with 2048 bit key
    ///     // NOTE : It would be better to use the dodSON.Core.ServiceManagement.ServiceManagementHelper.DefaultRSAKeyLengthInBits value.
    ///     int dwKeySize = 2048;
    ///     System.Security.Cryptography.RSACryptoServiceProvider encryptor = new System.Security.Cryptography.RSACryptoServiceProvider(dwKeySize);
    ///     // return a tuple with the XML Public Key and the XML Private Key, respectively.
    ///     return Tuple.Create(encryptor.ToXmlString(false), encryptor.ToXmlString(true));
    /// }
    /// 
    /// /// &lt;summary&gt;
    /// /// Prepares data by encrypting it into smaller chucks.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="data"&gt;The data to prepare.&lt;/param&gt;
    /// /// &lt;param name="xmlPublicKey"&gt;An XML string representation of a Public Key.&lt;/param&gt;
    /// /// &lt;returns&gt;A list of byte arrays encrypted and split into smaller chucks.&lt;/returns&gt;
    /// public static List&lt;byte[]&gt; PrepareForTransport(byte[] data, string xmlPublicKey)
    /// {
    ///     // NOTE: This is a copy of the dodSON.Core.ServiceManagement.ServiceManagementHelper.PrepareForTransport(...) function.
    ///     //       It has been provided in this example for clarity with how the data is encrypted and converted into a list of byte arrays.
    /// 
    ///     try
    ///     {
    ///         var list = new List&lt;byte[]&gt;();
    ///         // create encryptor from public key
    ///         System.Security.Cryptography.RSACryptoServiceProvider transportEncryptor = new System.Security.Cryptography.RSACryptoServiceProvider(dodSON.Core.ServiceManagement.ServiceManagementHelper.DefaultRSAKeyLengthInBits);
    ///         transportEncryptor.FromXmlString(xmlPublicKey);
    ///         //
    ///         foreach (var item in dodSON.Core.Common.ByteArrayHelper.SplitByteArray(data, dodSON.Core.ServiceManagement.ServiceManagementHelper.DefaultLoginTransportChuckSize))
    ///         {
    ///             list.Add(transportEncryptor.Encrypt(item, true));
    ///         }
    ///         return list;
    ///     }
    ///     catch /*(Exception ex)*/
    ///     {
    /// 
    ///         throw;
    ///     }
    /// }
    /// 
    /// /// &lt;summary&gt;
    /// /// Restores data by decrypting and joining all of the parts together.
    /// /// &lt;/summary&gt;
    /// /// &lt;param name="parts"&gt;A list of byte arrays encrypted and split into smaller chucks.&lt;/param&gt;
    /// /// &lt;param name="xmlPrivateKey"&gt;An XML string representation of a Private Key.&lt;/param&gt;
    /// /// &lt;returns&gt;A byte array decrypted and reassembled from the &lt;paramref name="parts"/&gt;.&lt;/returns&gt;
    /// public static byte[] RestoreFromTransport(List&lt;byte[]&gt; parts, string xmlPrivateKey)
    /// {
    ///     // NOTE: This is a copy of the dodSON.Core.ServiceManagement.ServiceManagementHelper.RestoreFromTransport(...) function.
    ///     //       It has been provided in this example for clarity with how the list of encrypted byte arrays are de-encrypted and converted into a single byte array.
    /// 
    ///     try
    ///     {
    ///         var list = new List&lt;byte[]&gt;();
    ///         // create encryptor from private key
    ///         System.Security.Cryptography.RSACryptoServiceProvider transportEncryptor = new System.Security.Cryptography.RSACryptoServiceProvider(dodSON.Core.ServiceManagement.ServiceManagementHelper.DefaultRSAKeyLengthInBits);
    ///         transportEncryptor.FromXmlString(xmlPrivateKey);
    ///         //
    ///         foreach (var item in parts)
    ///         {
    ///             list.Add(transportEncryptor.Decrypt(item, true));
    ///         }
    ///         return dodSON.Core.Common.ByteArrayHelper.RestoreByteArray(list);
    ///     }
    ///     catch /*(Exception ex)*/
    ///     {
    /// 
    ///         throw;
    ///     }
    /// }
    /// 
    /// // ------------------------------------------------------------------------------------------------------------------------
    /// 
    /// private static string LogFilename;
    /// 
    /// private static long _LastServerIncomingBytes;
    /// private static long _LastServerOutgoingBytes;
    /// private static long _LastServerIncomingEnvelopes;
    /// private static long _LastServerOutgoingEnvelopes;
    /// 
    /// // ********************************
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
    /// private static void RefreshDisplay(DateTimeOffset startTime,
    ///                                    dodSON.Core.Networking.IServer server)
    /// {
    ///     Console.Clear();
    ///     Console.WriteLine("Service Manager Example");
    ///     Console.WriteLine("dodSON Software Core Library");
    ///     Console.WriteLine($"Copyright (c) dodSON Software 2016-2018");
    ///     Console.WriteLine($"=======================================");
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Log={LogFilename}");
    ///     Console.WriteLine();
    ///     DisplayNetworkStats(startTime, server);
    ///     Console.WriteLine();
    ///     Console.WriteLine("press [F1]=Network Ping");
    ///     Console.WriteLine("press [F4]=Network Restart Clients");
    ///     Console.WriteLine();
    ///     Console.WriteLine("press [F5]=Service Manager IsAlive");
    ///     Console.WriteLine("press [F6]=Service Manager Echo/Ping/&lt;String&gt; Message");
    ///     Console.WriteLine("press [F8]=Service Manager Details");
    ///     Console.WriteLine();
    ///     Console.WriteLine("press [Esc]=Exit");
    ///     Console.WriteLine($"{Environment.NewLine}--------------------------------");
    ///     Console.Write($"{Environment.NewLine}press anykey&gt;");
    /// }
    /// 
    /// private static void DisplayNetworkStats(DateTimeOffset startTime, dodSON.Core.Networking.IServer server)
    /// {
    ///     var postPadding = "                     ";
    ///     var allStats = server.RequestAllClientsTransportStatistics();
    ///     var forDuration = DateTime.Now - _LastDisplayEvent;
    /// 
    ///     Console.WriteLine($"Uri     = {server.AddressUri}");
    ///     Console.WriteLine($"Runtime = {DateTimeOffset.Now - startTime}");
    ///     Console.WriteLine($"{Environment.NewLine}--------------------------------{Environment.NewLine}");
    ///     // 
    ///     Console.WriteLine($"Server Incoming:");
    ///     var incomingEnvelopesPerSecond = allStats.ServerStatistics.IncomingAverageEnvelopesPerSecond(_LastServerIncomingEnvelopes, forDuration);
    ///     Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.IncomingEnvelopes}, envelopes/second = {incomingEnvelopesPerSecond:N2}{postPadding}");
    ///     var incomingBytesPerSecond = allStats.ServerStatistics.IncomingAverageBytesPerSecond(_LastServerIncomingBytes, forDuration);
    ///     Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)incomingBytesPerSecond)} ({incomingBytesPerSecond:N2}){postPadding}");
    ///     Console.WriteLine($"Server Outgoing:");
    ///     var outgoingEnvelopesPerSecond = allStats.ServerStatistics.OutgoingAverageEnvelopesPerSecond(_LastServerOutgoingEnvelopes, forDuration);
    ///     Console.WriteLine($"    Envelopes = {allStats.ServerStatistics.OutgoingEnvelopes}, envelopes/second = {outgoingEnvelopesPerSecond:N2}{postPadding}");
    ///     var outgoingBytesPerSecond = allStats.ServerStatistics.OutgoingAverageBytesPerSecond(_LastServerOutgoingBytes, forDuration);
    ///     Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(allStats.ServerStatistics.OutgoingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)outgoingBytesPerSecond)} ({outgoingBytesPerSecond:N2}){postPadding}");
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
    ///         if (_ClientStatsInfoList.ContainsKey(client.ClientServerId))
    ///         {
    ///             clientStatsInfo = _ClientStatsInfoList[client.ClientServerId];
    ///         }
    ///         else
    ///         {
    ///             _ClientStatsInfoList.Add(client.ClientServerId, clientStatsInfo);
    ///         }
    ///         // 
    ///         Console.WriteLine();
    ///         Console.WriteLine($"{client.ClientServerId} Incoming:");
    ///         var clientIncomingMessagesPerSecond = client.IncomingAverageMessagesPerSecond(clientStatsInfo.LastIncomingMessages, forDuration);
    ///         Console.WriteLine($"    Messages  = {client.IncomingMessages}, messages/second = {clientIncomingMessagesPerSecond:N2}{postPadding}");
    ///         var clientIncomingEnvelopesPerSecond = client.IncomingAverageEnvelopesPerSecond(clientStatsInfo.LastIncomingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {client.IncomingEnvelopes}, envelopes/second = {clientIncomingEnvelopesPerSecond:N2}{postPadding}");
    ///         var clientIncomingBytesPerSecond = client.IncomingAverageBytesPerSecond(clientStatsInfo.LastIncomingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.IncomingBytes)}, Bytes/second = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientIncomingBytesPerSecond)} ({clientIncomingBytesPerSecond:N2}){postPadding}");
    ///         Console.WriteLine($"{client.ClientServerId} Outgoing:");
    ///         var clientOutgoingMessagesPerSecond = client.OutgoingAverageMessagesPerSecond(clientStatsInfo.LastOutgoingMessages, forDuration);
    ///         Console.WriteLine($"    Messages  = {client.OutgoingMessages}, messages/second = {clientOutgoingMessagesPerSecond:N2}{postPadding}");
    ///         var clientOutgoingEnvelopesPerSecond = client.OutgoingAverageEnvelopesPerSecond(clientStatsInfo.LastOutgoingEnvelopes, forDuration);
    ///         Console.WriteLine($"    Envelopes = {client.OutgoingEnvelopes}, envelopes/second = {clientOutgoingEnvelopesPerSecond:N2}{postPadding}");
    ///         var clientOutgoingBytesPerSecond = client.OutgoingAverageBytesPerSecond(clientStatsInfo.LastOutgoingBytes, forDuration);
    ///         Console.WriteLine($"    Bytes     = {dodSON.Core.Common.ByteCountHelper.ToString(client.OutgoingBytes)}, Bytes/seconds = {dodSON.Core.Common.ByteCountHelper.ToString((long)clientOutgoingBytesPerSecond)} ({clientOutgoingBytesPerSecond:N2}){postPadding}");
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
    /// <para>WorkerExtension Code.</para>
    /// <code>
    /// [Serializable()]
    /// public class WorkerExtension
    ///     : dodSON.Core.ServiceManagement.ServiceExtensionBase
    /// {
    ///     #region Ctor
    /// 
    ///     public WorkerExtension() : base() { }
    ///     #endregion
    ///     #region Private Fields
    ///     private string clientId = Guid.NewGuid().ToString();
    ///     private dodSON.Core.Threading.ThreadWorker _ThreadWorker;
    ///     #endregion
    ///     #region dodSON.Core.ComponentManagement.ComponentPluginBase Methods
    /// 
    ///     public override dodSON.Core.Networking.IClientConfiguration ClientConfiguration =&gt; new dodSON.Core.Networking.ClientConfiguration(clientId, false, null, null);
    /// 
    ///     protected override void OnComponentStarted()
    ///     {
    ///         base.OnComponentStarted();
    ///         //
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Hello from [-- ServiceSupervisor Service Extension--&gt;{base.PackageConfiguration.Name} --] in domain #{System.AppDomain.CurrentDomain.Id}");
    ///         //
    ///         if (CustomConfiguration == null)
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Custom Configuration File: Not Found.");
    ///         }
    ///         else
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Custom Configuration File: Found.");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Name= {CustomConfiguration.Items["Name"].Value.ToString()}");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Created Date= {CustomConfiguration.Items["Created Date"].Value.ToString()}");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Pi= {CustomConfiguration.Items["Pi"].Value.ToString()}");
    ///         }
    ///         //
    ///         foreach (var filename in InstallFileStore.GetAll())
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Debug, Id, $"RootFilename={filename.RootFilename}, OriginalFilename={filename.OriginalFilename}");
    ///         }
    ///         //
    ///         _ThreadWorker = new dodSON.Core.Threading.ThreadWorker(TimeSpan.FromSeconds(49), (cancelToken) =&gt; { SendMessage&lt;string&gt;("", $"Message from [ServiceManager-Extension] in domain #{AppDomain.CurrentDomain.Id}"); });
    ///         _ThreadWorker.Start();
    ///     }
    /// 
    ///     protected override void OnComponentStopping()
    ///     {
    ///         _ThreadWorker.Stop();
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Goodbye from [-- ServiceSupervisor Service Extension--&gt;{base.PackageConfiguration.Name} --] in domain #{System.AppDomain.CurrentDomain.Id}");
    ///     }
    /// 
    ///     protected override void OnMessageReceived(dodSON.Core.Networking.IMessage message)
    ///     {
    ///         if (message.TypeInfo.TypeName == typeof(string).AssemblyQualifiedName)
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"Message={message.PayloadMessage&lt;string&gt;()}");
    ///         }
    ///     }
    ///     #endregion
    /// } 
    /// </code>
    /// <para>WorkerPlugin Code.</para>
    /// <code>
    /// [Serializable()]
    /// public class WorkerPlugin
    /// 	: dodSON.Core.ServiceManagement.ServicePluginBase
    /// {
    ///     #region Ctor
    /// 
    ///     public WorkerPlugin() : base() { }
    ///     #endregion
    ///     #region Private Fields
    ///     private string clientId = Guid.NewGuid().ToString();
    ///     private dodSON.Core.Threading.ThreadWorker _ThreadWorker;
    ///     #endregion
    ///     #region dodSON.Core.ComponentManagement.ComponentPluginBase Methods
    /// 
    ///     public override dodSON.Core.Networking.IClientConfiguration ClientConfiguration =&gt; new dodSON.Core.Networking.ClientConfiguration(clientId, false, null, null);
    /// 
    ///     protected override void OnComponentStarted()
    ///     {
    ///         base.OnComponentStarted();
    ///         //
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Hello from [-- ServiceSupervisor Service Plugin--&gt;{base.PackageConfiguration.Name} --] in domain #{System.AppDomain.CurrentDomain.Id}");
    ///         // 
    ///         if (CustomConfiguration == null)
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Custom Configuration File: Not Found.");
    ///         }
    ///         else
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Custom Configuration File: Found.");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Name= {CustomConfiguration.Items["Name"].Value.ToString()}");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Created Date= {CustomConfiguration.Items["Created Date"].Value.ToString()}");
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Pi= {CustomConfiguration.Items["Pi"].Value.ToString()}");
    ///         }
    ///         //
    ///         foreach (var filename in InstallFileStore.GetAll())
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Debug, Id, $"RootFilename={filename.RootFilename}, OriginalFilename={filename.OriginalFilename}");
    ///         }
    ///         //
    ///         _ThreadWorker = new dodSON.Core.Threading.ThreadWorker(TimeSpan.FromSeconds(64), (cancelToken) =&gt; { SendMessage&lt;string&gt;("", $"Message from [ServiceManager-Plugin] in domain #{AppDomain.CurrentDomain.Id}"); });
    ///         _ThreadWorker.Start();
    ///     }
    /// 
    ///     protected override void OnComponentStopping()
    ///     {
    ///         _ThreadWorker.Stop();
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Goodbye from [-- ServiceSupervisor Service Plugin--&gt;{base.PackageConfiguration.Name} --] in domain #{System.AppDomain.CurrentDomain.Id}");
    ///     }
    /// 
    ///     protected override void OnMessageReceived(dodSON.Core.Networking.IMessage message)
    ///     {
    ///         if (message.TypeInfo.TypeName == typeof(string).AssemblyQualifiedName)
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"Message={message.PayloadMessage&lt;string&gt;()}");
    ///         }
    ///     }
    ///     #endregion
    /// } 
    /// </code>
    /// <para>Package Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="PackageConfiguration"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="IsDependencyPackage" type="System.Boolean"&gt;False&lt;/item&gt;
    ///     &lt;item key="IsEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     &lt;item key="Name" type="System.String"&gt;Package-A&lt;/item&gt;
    ///     &lt;item key="Priority" type="System.Double"&gt;300&lt;/item&gt;
    ///     &lt;item key="Version" type="System.Version"&gt;1.0.0.0&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="DependencyPackages"&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="DependencyPackage 1"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="MinimumVersion" type="System.Version"&gt;1.0.0.0&lt;/item&gt;
    ///             &lt;item key="Name" type="System.String"&gt;Package-X&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="DependencyPackage 2"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="MinimumVersion" type="System.Version"&gt;1.0.0.0&lt;/item&gt;
    ///             &lt;item key="Name" type="System.String"&gt;Package-Y&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>Custom Package Configuration.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="CustomConfiguration"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Name" type="System.String"&gt;My Custom Configuration&lt;/item&gt;
    ///     &lt;item key="Pi" type="System.Double"&gt;3.14159265358979&lt;/item&gt;
    ///     &lt;item key="Created Date" type="SystemDateTimeOffset"&gt;2018-11-03 17:01&lt;/item&gt;
    ///   &lt;/items&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>Initial Screen Shot.</para>
    /// <code>
    /// // Service Manager Example
    /// // dodSON Software Core Library
    /// // Copyright (c) dodSON Software 2016-2018
    /// // =======================================
    /// // 
    /// // Creating Component Manager.
    /// // Log = C:\(WORKING)\Dev\ServiceManagement\20181103_042758.log
    /// // 
    /// // Saving Component Manager Configuration Files.
    /// // 
    /// // Creating Service Manager.
    /// // 
    /// //     IsRunning=False
    /// // Starting Service Manager.
    /// //     IsRunning=True
    /// // 
    /// // Creating Login Client.
    /// // 
    /// // Saving Login Client Configuration Files.
    /// // 
    /// // Logging into Service Manager: Login Successful
    /// // 
    /// // ServiceManagerId=
    /// // SessionId=34ea630f83c64d0bbf7c5cbc14aa8293
    /// // ServiceManagerClientId=
    /// // 
    /// // press anykey&gt;
    /// </code>
    /// <para>Final Screen Shot.</para>
    /// <code>
    /// // Service Manager Example
    /// // dodSON Software Core Library
    /// // Copyright (c) dodSON Software 2016-2018
    /// // =======================================
    /// // 
    /// // Log=C:\(WORKING)\Dev\ServiceManagement\20181103_042758.log
    /// // 
    /// // Uri     = net.pipe://localhost/ComponentManagementServer-49152
    /// // Runtime = 00:01:21.1599423
    /// // 
    /// // --------------------------------
    /// // 
    /// // Server Incoming:
    /// //     Envelopes = 71, envelopes/second = 0.00
    /// //     Bytes     = 53 KB, Bytes/second = 0 bytes (0.00)
    /// // Server Outgoing:
    /// //     Envelopes = 207, envelopes/second = 0.00
    /// //     Bytes     = 157 KB, Bytes/second = 0 bytes (0.00)
    /// // 
    /// // 11f0b4d4-6819-4f17-bf73-e19bae1821cf Incoming:
    /// //     Messages  = 18, messages/second = 0.00
    /// //     Envelopes = 36, envelopes/second = 0.00
    /// //     Bytes     = 28 KB, Bytes/second = 0 bytes (0.00)
    /// // 11f0b4d4-6819-4f17-bf73-e19bae1821cf Outgoing:
    /// //     Messages  = 2, messages/second = 0.00
    /// //     Envelopes = 2, envelopes/second = 0.00
    /// //     Bytes     = 2 KB, Bytes/seconds = 0 bytes (0.00)
    /// // 
    /// // 6091b4c7-34a2-4ce5-a298-0aa050eee066 Incoming:
    /// //     Messages  = 17, messages/second = 0.00
    /// //     Envelopes = 35, envelopes/second = 0.00
    /// //     Bytes     = 27 KB, Bytes/second = 0 bytes (0.00)
    /// // 6091b4c7-34a2-4ce5-a298-0aa050eee066 Outgoing:
    /// //     Messages  = 2, messages/second = 0.00
    /// //     Envelopes = 2, envelopes/second = 0.00
    /// //     Bytes     = 2 KB, Bytes/seconds = 0 bytes (0.00)
    /// // 
    /// // 130777e780fa4bd4b4cc014feb3c1847 Incoming:
    /// //     Messages  = 28, messages/second = 0.00
    /// //     Envelopes = 65, envelopes/second = 0.00
    /// //     Bytes     = 49 KB, Bytes/second = 0 bytes (0.00)
    /// // 130777e780fa4bd4b4cc014feb3c1847 Outgoing:
    /// //     Messages  = 16, messages/second = 0.00
    /// //     Envelopes = 35, envelopes/second = 0.00
    /// //     Bytes     = 26 KB, Bytes/seconds = 0 bytes (0.00)
    /// // 
    /// // LoginClient Incoming:
    /// //     Messages  = 16, messages/second = 0.00
    /// //     Envelopes = 33, envelopes/second = 0.00
    /// //     Bytes     = 25 KB, Bytes/second = 0 bytes (0.00)
    /// // LoginClient Outgoing:
    /// //     Messages  = 14, messages/second = 0.00
    /// //     Envelopes = 32, envelopes/second = 0.00
    /// //     Bytes     = 24 KB, Bytes/seconds = 0 bytes (0.00)
    /// // 
    /// // --------------------------------
    /// // 
    /// // press [F1]=Network Ping
    /// // press [F4]=Network Restart Clients
    /// // 
    /// // press [F5]=Service Manager IsAlive
    /// // press [F6]=Service Manager Echo/Ping/&lt;String&gt; Message
    /// // press [F8]=Service Manager Details
    /// // 
    /// // press [Esc]=Exit
    /// // 
    /// // --------------------------------
    /// // 
    /// // press anykey&gt;
    /// // 
    /// // =======================================
    /// // 
    /// // Closing login Client.
    /// // 
    /// //     IsRunning=True
    /// // Stopping Service Manager.
    /// //     IsRunning=False; ReceivedRequests=14
    /// // 
    /// // =======================================
    /// // press anykey&gt;
    /// </code>
    /// <para>Log.</para>
    /// <code>
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; ****************                     Start Log, 2018-11-03 4:27:59 AM -05:00                      ****************
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; Starting ComponentManager, (dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; .NET Version=4.0.30319
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; dodSON.Core Version=1.1.0.0
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; Executable Path=C:\Users\User\Documents\Visual Studio 2017\Projects\dodSONCore_ServiceManagerExample_1\dodSONCore_ServiceManagerExample_1\bin\Debug
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; Package Storage: Total=4, Enabled=4, Disabled=0
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; Installed Packages: Total=3, NonDependency=1, Dependency=2
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; System; Installed Packages: Disabled=0, Orphaned=0
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; Starting CommunicationController, (dodSON.Core.ComponentManagement.CommunicationController, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; Opening Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; Server Id=ComponentManagementServer_8c7e2a8064cc4049bc0f67b5d1e14c2b
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; Type=[dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; Uri=net.pipe://localhost/ComponentManagementServer-49152
    /// // 2018-11-03 4:27:59 AM -05:00; Information; 0; 0; Network; IP Address=localhost, Name=ComponentManagementServer, Port=49152
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Registering Client=SiCa5fa8509c52446b28b2a193dodSON1fd
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Receive Own Messages=True
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Completed Registering Client=SiCa5fa8509c52446b28b2a193dodSON1fd
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; Server Opened Successfully. Elapsed Time=00:00:01.7476834
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Network; CommunicationController Started. Elapsed Time=00:00:02.0817401
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Starting Installation, (dodSON.Core.Installation.Installer, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Install Root Path=C:\(WORKING)\Dev\ServiceManagement\Install
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Total installed packages=3
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Enabled packages=3
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; #2=Package-X, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-X_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; #3=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-Y_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Installing package and dependency chain for [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Install Path=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Installation Type=HighestVersionOnly
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Update Packages Before Installing=True
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Clean Install=False
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Package Update=True
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Remove Orphaned Packages=False
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating Package [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating files from package [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Using existing directory=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Files: OK=6
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating Package [Package-X, v1.0.0.0, Package-X-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating files from package [Package-X, v1.0.0.0, Package-X-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Using existing directory=C:\(WORKING)\Dev\ServiceManagement\Install\Package-X_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Files: OK=1
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating Package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Updating files from package [Package-Y, v1.0.0.0, Package-Y-1.0.0.0.zip]
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Using existing directory=C:\(WORKING)\Dev\ServiceManagement\Install\Package-Y_v1.0.0.0
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Files: OK=1
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Total packages processed:Updated=3
    /// // 2018-11-03 4:28:01 AM -05:00; Information; 0; 0; Install; Completed installation of package and dependency chain for [Package-A, v1.0.0.0, Package-A-1.0.0.0.zip], Elapsed Time=00:00:00.3590731
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; Total installed packages=3
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; Enabled packages=3
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; #2=Package-X, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-X_v1.0.0.0
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; #3=Package-Y, v1.0.0.0, C:\(WORKING)\Dev\ServiceManagement\Install\Package-Y_v1.0.0.0
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Install; Installation Completed. Elapsed Time=00:00:00.6671802
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Components; Starting ComponentController, (dodSON.Core.ComponentManagement.ComponentController, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Components; Extension components=1
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Components; #1=[Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Components; Plugin components=1
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; Components; #1=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2018-11-03 4:28:02 AM -05:00; Information; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Starting Extension Component [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; Network; Registering Client=11f0b4d4-6819-4f17-bf73-e19bae1821cf
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; Network; Receive Own Messages=False
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; Network; Completed Registering Client=11f0b4d4-6819-4f17-bf73-e19bae1821cf
    /// // 2018-11-03 4:28:04 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Hello from [-- ServiceSupervisor Service Extension--&gt;Package-A --] in domain #1
    /// // 2018-11-03 4:28:04 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Custom Configuration File: Found.
    /// // 2018-11-03 4:28:04 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Name= My Custom Configuration
    /// // 2018-11-03 4:28:04 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Created Date= 2018-11-03 17:01
    /// // 2018-11-03 4:28:04 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Pi= 3.14159265358979
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=Custom.configuration.xml, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Custom.configuration.xml
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=dodSON.Core.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\dodSON.Core.dll
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=Hello There.txt, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Hello There.txt
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=package.config.xml, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\package.config.xml
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=Playground_Extension01.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll
    /// // 2018-11-03 4:28:04 AM -05:00; Debug; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; RootFilename=Playground_Worker01.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Extension Component Started [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]
    /// // 2018-11-03 4:28:04 AM -05:00; Information; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Starting Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Network; Registering Client=6091b4c7-34a2-4ce5-a298-0aa050eee066
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Network; Receive Own Messages=False
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Network; Completed Registering Client=6091b4c7-34a2-4ce5-a298-0aa050eee066
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Hello from [-- ServiceSupervisor Service Plugin--&gt;Package-A --] in domain #3
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Custom Configuration File: Found.
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Name= My Custom Configuration
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Created Date= 2018-11-03 17:01
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Pi= 3.14159265358979
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=Custom.configuration.xml, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Custom.configuration.xml
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=dodSON.Core.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\dodSON.Core.dll
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=Hello There.txt, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Hello There.txt
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=package.config.xml, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\package.config.xml
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=Playground_Extension01.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll
    /// // 2018-11-03 4:28:06 AM -05:00; Debug; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; RootFilename=Playground_Worker01.dll, OriginalFilename=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Plugin Component Started [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; Components; ComponentController Started. Elapsed Time=00:00:04.8091580
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; System; ComponentManager Started. Elapsed Time=00:00:07.8335618
    /// // 2018-11-03 4:28:06 AM -05:00; Information; 0; 0; ServiceManager; Starting Service Manager, Id=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, (dodSON.Core.ServiceManagement.ServiceManager, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:28:06 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Message=Message from [ServiceManager-Plugin] in domain #3
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; Registering Client=130777e780fa4bd4b4cc014feb3c1847
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; Receive Own Messages=True
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; Receivable Types: (2)
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; #1=dodSON.Core.ServiceManagement.ServiceRequest, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; #2=dodSON.Core.ServiceManagement.ServiceResponse, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; Transmittable Types: (2)
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; #1=dodSON.Core.ServiceManagement.ServiceRequest, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; #2=dodSON.Core.ServiceManagement.ServiceResponse, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; Network; Completed Registering Client=130777e780fa4bd4b4cc014feb3c1847
    /// // 2018-11-03 4:28:08 AM -05:00; Information; 0; 0; ServiceManager; Service Manager Started, Id=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, Elapsed Time=00:00:01.5662472
    /// // 2018-11-03 4:28:09 AM -05:00; Information; 0; 0; Network; Registering Client=LoginClient
    /// // 2018-11-03 4:28:09 AM -05:00; Information; 0; 0; Network; Receive Own Messages=False
    /// // 2018-11-03 4:28:09 AM -05:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2018-11-03 4:28:09 AM -05:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2018-11-03 4:28:09 AM -05:00; Information; 0; 0; Network; Completed Registering Client=LoginClient
    /// // 2018-11-03 4:28:10 AM -05:00; AuditSuccess; 0; 0; ServiceManager; ServiceManagerLoginRequest, Processing, GroupKey=794dc28782544622b209d607672a32a8, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:10 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_LoginRequestResponse, XmlPublicKey.Length=415
    /// // 2018-11-03 4:28:11 AM -05:00; AuditSuccess; 0; 0; ServiceManager; ServiceManagerLogin, Results=Login Successful, GroupKey=794dc28782544622b209d607672a32a8, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, SessionCachedTimeLimit=00:00:30, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:11 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseCommands.ServiceManager_Login, DataType=System.Collections.Generic.List`1[[System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Request Clients Transport Statistics: (5)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; #1=SiCa5fa8509c52446b28b2a193dodSON1fd
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Date Started=2018-11-03 4:27:59 AM -05:00 (00:00:16.4553438)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Incoming: Messages=4, Envelopes=18, Bytes=15 KB (15168)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Outgoing: Messages=0, Envelopes=0, Bytes=0 bytes (0)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; #2=11f0b4d4-6819-4f17-bf73-e19bae1821cf
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Date Started=2018-11-03 4:28:02 AM -05:00 (00:00:13.6432572)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Incoming: Messages=3, Envelopes=17, Bytes=14 KB (14384)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Outgoing: Messages=1, Envelopes=1, Bytes=784 bytes (784)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; #3=6091b4c7-34a2-4ce5-a298-0aa050eee066
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Date Started=2018-11-03 4:28:04 AM -05:00 (00:00:11.8482498)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Incoming: Messages=2, Envelopes=16, Bytes=13 KB (13600)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Outgoing: Messages=1, Envelopes=1, Bytes=784 bytes (784)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; #4=130777e780fa4bd4b4cc014feb3c1847
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Date Started=2018-11-03 4:28:06 AM -05:00 (00:00:09.0467341)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Incoming: Messages=4, Envelopes=22, Bytes=18 KB (18464)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Outgoing: Messages=2, Envelopes=6, Bytes=5 KB (4864)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; #5=LoginClient
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Date Started=2018-11-03 4:28:08 AM -05:00 (00:00:07.4829415)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Incoming: Messages=2, Envelopes=6, Bytes=5 KB (4864)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Outgoing: Messages=2, Envelopes=16, Bytes=13 KB (13600)
    /// // 2018-11-03 4:28:15 AM -05:00; Information; 0; 0; Network; Completed Request Clients Transport Statistics.
    /// // 2018-11-03 4:28:20 AM -05:00; AuditSuccess; 0; 0; ServiceManager; IsSessionAlive, Result=True, SessionId is Valid, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:20 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseCommands.IsSessionAlive, Results=True
    /// // 2018-11-03 4:28:22 AM -05:00; AuditSuccess; 0; 0; ServiceManager; Echo, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, HasData=True, DataType=System.Guid
    /// // 2018-11-03 4:28:22 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Message=################ Hello from Main Application!
    /// // 2018-11-03 4:28:22 AM -05:00; AuditSuccess; 0; 0; ServiceManager; Ping, StartTime=2018-11-03 9:28:22 AM +00:00, TravelTime=00:00:00.0230012, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:22 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseCommands.Echo, HasData=True, DataType=System.Guid
    /// // 2018-11-03 4:28:22 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=################ Hello from Main Application!
    /// // 2018-11-03 4:28:22 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Ping, Round Trip=00:00:00.0580001, TravelToTime=00:00:00.0230012, TravelFromTime=00:00:00.0349989, StartTime=2018-11-03 9:28:22 AM +00:00, ReceivedTime=2018-11-03 9:28:22 AM +00:00, EndTime=2018-11-03 9:28:22 AM +00:00
    /// // 2018-11-03 4:28:24 AM -05:00; AuditFailure; 0; 0; ServiceManager; ServiceManager_Details, Invalid ClientId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=130777e780fa4bd4b4cc014feb3c1847, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:24 AM -05:00; AuditFailure; 0; 0; ServiceManager; ServiceManager_Details, Invalid SessionId, SessionId=**** Bad Session Id ****, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:24 AM -05:00; AuditSuccess; 0; 0; ServiceManager; ServiceManager_Details, Processing, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:24 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Error, Description=ServiceManager_Details, Invalid ClientId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=130777e780fa4bd4b4cc014feb3c1847, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:24 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Error, Description=ServiceManager_Details, Invalid SessionId, SessionId=**** Bad Session Id ****, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:28:24 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_Details, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, StartTime=2018-11-03 9:28:08 AM +00:00, RunTime00:00:16.0849432
    /// // 2018-11-03 4:28:24 AM -05:00; Debug; 0; 0; Application;     Extension #1=dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_ServiceDetails, Id=[Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll], IsRunning=True, DateLastStarted=2018-11-03 4:28:02 AM, InstallPath=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:24 AM -05:00; Debug; 0; 0; Application;     Plugin #1=dodSON.Core.ServiceManagement.RequestResponseTypes.ServiceManager_ServiceDetails, Id=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll], IsRunning=True, DateLastStarted=2018-11-03 4:28:04 AM, InstallPath=C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; Pinging All Clients: (5)
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; #1=SiCa5fa8509c52446b28b2a193dodSON1fd, Round Trip=00:00:00.0030429, Date Started=2018-11-03 4:27:59 AM -05:00, Runtime=00:00:27.6133582
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; #2=11f0b4d4-6819-4f17-bf73-e19bae1821cf, Round Trip=00:00:00.0012537, Date Started=2018-11-03 4:28:02 AM -05:00, Runtime=00:00:24.8002496
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; #3=6091b4c7-34a2-4ce5-a298-0aa050eee066, Round Trip=00:00:00.0049228, Date Started=2018-11-03 4:28:04 AM -05:00, Runtime=00:00:22.9532560
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; #4=130777e780fa4bd4b4cc014feb3c1847, Round Trip=00:00:00.0013739, Date Started=2018-11-03 4:28:06 AM -05:00, Runtime=00:00:20.1917437
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; #5=LoginClient, Round Trip=00:00:00.0010059, Date Started=2018-11-03 4:28:08 AM -05:00, Runtime=00:00:18.6269423
    /// // 2018-11-03 4:28:27 AM -05:00; Information; 0; 0; Network; Completed Pinging All Clients.
    /// // 2018-11-03 4:28:53 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from [ServiceManager-Extension] in domain #1
    /// // 2018-11-03 4:28:58 AM -05:00; AuditSuccess; 0; 0; ServiceManager; SessionTimeout, GroupKey=794dc28782544622b209d607672a32a8, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, CreatedDate=2018-11-03 4:28:11 AM, RunTime=00:00:46.7579457, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:10 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Message=Message from [ServiceManager-Plugin] in domain #3
    /// // 2018-11-03 4:29:26 AM -05:00; AuditFailure; 0; 0; ServiceManager; IsSessionAlive, Result=False, Invalid SessionId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:26 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseCommands.IsSessionAlive, Results=False
    /// // 2018-11-03 4:29:28 AM -05:00; AuditSuccess; 0; 0; ServiceManager; Ping, StartTime=2018-11-03 9:29:28 AM +00:00, TravelTime=00:00:00.0039992, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:28 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=################ Hello from Main Application!
    /// // 2018-11-03 4:29:28 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Message=################ Hello from Main Application!
    /// // 2018-11-03 4:29:28 AM -05:00; AuditSuccess; 0; 0; ServiceManager; Echo, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, HasData=True, DataType=System.Guid
    /// // 2018-11-03 4:29:28 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Ping, Round Trip=00:00:00.0430135, TravelToTime=00:00:00.0039992, TravelFromTime=00:00:00.0390143, StartTime=2018-11-03 9:29:28 AM +00:00, ReceivedTime=2018-11-03 9:29:28 AM +00:00, EndTime=2018-11-03 9:29:28 AM +00:00
    /// // 2018-11-03 4:29:28 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseCommands.Echo, HasData=True, DataType=System.Guid
    /// // 2018-11-03 4:29:30 AM -05:00; AuditFailure; 0; 0; ServiceManager; ServiceManager_Details, Invalid SessionId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=130777e780fa4bd4b4cc014feb3c1847, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:30 AM -05:00; AuditFailure; 0; 0; ServiceManager; ServiceManager_Details, Invalid SessionId, SessionId=**** Bad Session Id ****, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:30 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Error, Description=ServiceManager_Details, Invalid SessionId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=130777e780fa4bd4b4cc014feb3c1847, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:30 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Error, Description=ServiceManager_Details, Invalid SessionId, SessionId=**** Bad Session Id ****, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:30 AM -05:00; AuditFailure; 0; 0; ServiceManager; ServiceManager_Details, Invalid SessionId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:30 AM -05:00; AuditSuccess; 0; 0; Application; dodSON.Core.ServiceManagement.RequestResponseTypes.Error, Description=ServiceManager_Details, Invalid SessionId, SessionId=34ea630f83c64d0bbf7c5cbc14aa8293, ClientId=LoginClient, ServiceManagerId=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac
    /// // 2018-11-03 4:29:37 AM -05:00; Information; 0; 0; Network; Unregistering Client=LoginClient, Date Started=2018-11-03 4:28:09 AM, Runtime=00:01:27.4488111
    /// // 2018-11-03 4:29:37 AM -05:00; Information; 0; 0; ServiceManager; Stopping Service Manager, Id=ServiceManager_339d3057ea364b4b83acb6d2cec6bbac, (dodSON.Core.ServiceManagement.ServiceManager, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Network; Unregistering Client=130777e780fa4bd4b4cc014feb3c1847, Date Started=2018-11-03 4:28:08 AM, Runtime=00:01:30.0517896
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; System; Stopping ComponentManager, (dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Components; Stopping ComponentController, (dodSON.Core.ComponentManagement.ComponentController, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Components; Plugin components=1
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Components; #1=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Components; Extension components=1
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; Components; #1=[Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]
    /// // 2018-11-03 4:29:38 AM -05:00; Information; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Stopping Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2018-11-03 4:29:38 AM -05:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Goodbye from [-- ServiceSupervisor Service Plugin--&gt;Package-A --] in domain #3
    /// // 2018-11-03 4:29:39 AM -05:00; Information; 0; 0; Network; Unregistering Client=6091b4c7-34a2-4ce5-a298-0aa050eee066, Date Started=2018-11-03 4:28:06 AM, Runtime=00:01:32.7186109
    /// // 2018-11-03 4:29:39 AM -05:00; Information; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Plugin Component Stopped [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Worker01.dll], Start Time=2018-11-03 4:28:04 AM, Run Time=00:01:35.4951091
    /// // 2018-11-03 4:29:39 AM -05:00; Information; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Stopping Extension Component [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]
    /// // 2018-11-03 4:29:39 AM -05:00; Warning; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; #### Goodbye from [-- ServiceSupervisor Service Extension--&gt;Package-A --] in domain #1
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; Network; Unregistering Client=11f0b4d4-6819-4f17-bf73-e19bae1821cf, Date Started=2018-11-03 4:28:04 AM, Runtime=00:01:37.0148247
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll]; Extension Component Stopped [Playground_Extension01.WorkerExtension, C:\(WORKING)\Dev\ServiceManagement\Install\Package-A_v1.0.0.0\Playground_Extension01.dll], Start Time=2018-11-03 4:28:02 AM, Run Time=00:01:38.7928434
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; Components; ComponentController Stopped. Start Time=2018-11-03 4:27:59 AM -05:00, Runtime=00:01:42.0139389
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; Network; Stopping CommunicationController, (dodSON.Core.ComponentManagement.CommunicationController, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; Network; Closing Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2018-11-03 4:29:41 AM -05:00; Information; 0; 0; Network; Closing All Clients: (1)
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Unregistering Client=SiCa5fa8509c52446b28b2a193dodSON1fd, Date Started=2018-11-03 4:28:01 AM, Runtime=00:01:42.3879651
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Shutting Down Server, Id=ComponentManagementServer_8c7e2a8064cc4049bc0f67b5d1e14c2b, Uri=net.pipe://localhost/ComponentManagementServer-49152
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Total Incoming Bytes=53 KB (54,432)
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Total Incoming Envelopes=71
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Total Outgoing Bytes=157 KB (160,592)
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Total Outgoing Envelopes=207
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; Server Closed Successfully.
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; Network; CommunicationController Stopped. Date Started=2018-11-03 4:27:59 AM -05:00, Runtime=00:01:44.1539663
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; System; ComponentManager Stopped. Start Time=2018-11-03 4:27:59 AM -05:00, Runtime=00:01:44.4709656
    /// // 2018-11-03 4:29:43 AM -05:00; Information; 0; 0; System; ****************        End Log, Written Logs=216, Start Time=2018-11-03 4:27:59 AM -05:00        ****************
    /// </code>
    /// <para>Component Manager Configuration, XML version.</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="ComponentManager"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="LogSourceId" type="System.String"&gt;System&lt;/item&gt;
    ///     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="CommunicationController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Client" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Client, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="LogSourceId" type="System.String"&gt;Network&lt;/item&gt;
    ///         &lt;item key="Server" type="System.Type"&gt;dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="ServerId" type="System.String"&gt;ComponentManagementServer_8c7e2a8064cc4049bc0f67b5d1e14c2b&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.CommunicationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ServerChannel"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="IPAddress" type="System.String"&gt;localhost&lt;/item&gt;
    ///             &lt;item key="Name" type="System.String"&gt;ComponentManagementServer&lt;/item&gt;
    ///             &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="ServerOverrideTypesFilter" /&gt;
    ///         &lt;group key="TransportController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///           &lt;groups&gt;
    ///             &lt;group key="RegistrationController"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="ChallengeController"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="Evidence" type="System.Byte[]"&gt;D9594A42120F5F3D5B11339AD5906471D8A0C3D0CE8621A64B4FFF0405550DDE5978DBE4F5948DCDA42BABD40D8A2144BCCDF776DBA47DE4CF7D4919003335B831681184AED5A0AD91F7903FEDA68DCE45424ADA2FCA4E4E10925FF752D62BC8&lt;/item&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="TransportConfiguration"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="ChunkSize" type="System.Int32"&gt;512&lt;/item&gt;
    ///                 &lt;item key="Compressor" type="System.Type"&gt;dodSON.Core.Compression.DeflateStreamCompressor, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="EnvelopeCacheTimeLimit" type="System.TimeSpan"&gt;0.00:00:05.0&lt;/item&gt;
    ///                 &lt;item key="SeenMessageCacheTimeLimit" type="System.TimeSpan"&gt;0.00:00:15.0&lt;/item&gt;
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.TransportConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                 &lt;item key="UseChunking" type="System.Boolean"&gt;True&lt;/item&gt;
    ///               &lt;/items&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="EncryptorConfigurations"&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="Encryptor01"&gt;
    ///                       &lt;items&gt;
    ///                         &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.RijndaelManaged, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.EncryptorConfiguration, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                       &lt;/items&gt;
    ///                       &lt;groups&gt;
    ///                         &lt;group key="SaltedPassword"&gt;
    ///                           &lt;items&gt;
    ///                             &lt;item key="HashAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.SHA512Managed, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                             &lt;item key="PasswordSaltHash" type="System.Byte[]"&gt;B98A6E9CA7C669E0639D4A048B879DBB6C479C97D362A0A364274FA1E50D13F48A297C571BCB50F44CAA2376D5003C4C6C31DBF51025B08D5B17F847334ADB1D&lt;/item&gt;
    ///                             &lt;item key="Salt" type="System.Byte[]"&gt;6BC969827EC3A97686ED5491045AACAFAB802467672DB9EE951598FFC8CABA08424109AF493159A5D22077B8C659DE43F74B934356D5F386C3A71C8FF466C5DCAE4610E993AC28885DC81785186BA0DC5DE42A5344CECDDC8FB15BE0D0F2A91F0B072740B1D91E7B662974C3C65145F44A907CBEB580DA49B628A63A592CFED4&lt;/item&gt;
    ///                             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.SaltedPassword, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                           &lt;/items&gt;
    ///                         &lt;/group&gt;
    ///                       &lt;/groups&gt;
    ///                     &lt;/group&gt;
    ///                   &lt;/groups&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///           &lt;/groups&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ComponentController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="CustomConfigurationFilename" type="System.String"&gt;Custom.configuration.xml&lt;/item&gt;
    ///         &lt;item key="CustomConfigurationSerializerType" type="System.Type"&gt;dodSON.Core.Configuration.XmlConfigurationSerializer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="LogSourceId" type="System.String"&gt;Components&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.ComponentController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="InstallationSettings"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="CleanInstall" type="System.Boolean"&gt;False&lt;/item&gt;
    ///         &lt;item key="EnablePackageUpdates" type="System.Boolean"&gt;True&lt;/item&gt;
    ///         &lt;item key="InstallType" type="dodSON.Core.Installation.InstallType, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null"&gt;HighestVersionOnly&lt;/item&gt;
    ///         &lt;item key="RemoveOrphanedPackages" type="System.Boolean"&gt;False&lt;/item&gt;
    ///         &lt;item key="UpdatePackagesBeforeInstalling" type="System.Boolean"&gt;True&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="Installer"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="ConfigurationFilename" type="System.String"&gt;package.config.xml&lt;/item&gt;
    ///         &lt;item key="ConfigurationFileSerializer" type="System.Type"&gt;dodSON.Core.Configuration.XmlConfigurationSerializer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="InstallationRootPath" type="System.String"&gt;C:\(WORKING)\Dev\ServiceManagement\Install&lt;/item&gt;
    ///         &lt;item key="LogSourceId" type="System.String"&gt;Install&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Installation.Installer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="LogController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="ArchiveFilenameFactory" type="System.Type"&gt;dodSON.Core.ComponentManagement.ArchiveFilenameFactory, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.LogController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="FileStore"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="BackendStorageZipFilename" type="System.String"&gt;C:\(WORKING)\Dev\ServiceManagement\ComponentManagment.20181103_042758.log.archive.zip&lt;/item&gt;
    ///             &lt;item key="ExtractionRootDirectory" type="System.String"&gt;&lt;/item&gt;
    ///             &lt;item key="SaveOriginalFilenames" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.FileStorage.MSdotNETZip.FileStore, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\ServiceManagement\20181103_042758.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogControllerSettings"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoArchiveEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    ///             &lt;item key="AutoArchiveMaximumLogs" type="System.Int32"&gt;100000&lt;/item&gt;
    ///             &lt;item key="AutoFlushMaximumLogs" type="System.Int32"&gt;15&lt;/item&gt;
    ///             &lt;item key="AutoFlushTimeLimit" type="System.TimeSpan"&gt;0.00:00:10.0&lt;/item&gt;
    ///             &lt;item key="LogSourceId" type="System.String"&gt;Logs&lt;/item&gt;
    ///             &lt;item key="TruncateLogArchive" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="TruncateLogArchiveMaximumFiles" type="System.Int32"&gt;100&lt;/item&gt;
    ///             &lt;item key="TruncateLogArchivePercentageToRemove" type="System.Double"&gt;0&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.LogControllerSettings, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteArchivedLogEntriesUsingLocalTime" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="WritePrimaryLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="PackageProvider"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="ConfigurationFilename" type="System.String"&gt;package.config.xml&lt;/item&gt;
    ///         &lt;item key="ConfigurationFileSerializer" type="System.Type"&gt;dodSON.Core.Configuration.XmlConfigurationSerializer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="PackageFileStoreProvider" type="System.Type"&gt;dodSON.Core.Packaging.MSdotNETZip.PackageFileStoreProvider, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Packaging.PackageProvider, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="FileStore"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="ExtractionRootDirectory" type="System.String"&gt;&lt;/item&gt;
    ///             &lt;item key="SaveOriginalFilenames" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="SourceDirectory" type="System.String"&gt;C:\(WORKING)\Dev\ServiceManagement\Packages&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>Login Client Configuration, XML version.</para>
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
    ///         &lt;item key="Name" type="System.String"&gt;ComponentManagementServer&lt;/item&gt;
    ///         &lt;item key="Port" type="System.Int32"&gt;49152&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="ClientConfiguration"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Id" type="System.String"&gt;LoginClient&lt;/item&gt;
    ///         &lt;item key="ReceiveSelfSentMessages" type="System.Boolean"&gt;False&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ReceivableTypesFilter" /&gt;
    ///         &lt;group key="TransmittableTypesFilter" /&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="RegistrationController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.TunnellingRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="ChallengeController"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="Evidence" type="System.Byte[]"&gt;D9594A42120F5F3D5B11339AD5906471D8A0C3D0CE8621A64B4FFF0405550DDE5978DBE4F5948DCDA42BABD40D8A2144BCCDF776DBA47DE4CF7D4919003335B831681184AED5A0AD91F7903FEDA68DCE45424ADA2FCA4E4E10925FF752D62BC8&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// </example>
    [Serializable]
    public class ServiceManager
            : IServiceManager
    {
        #region Private Static Constants
        /// <summary>
        /// The minimum value allowed for the <see cref="MaximumLoginsAllowed"/> property.
        /// </summary>
        private static readonly int _MinimumLoginsAllowed_ = 1;
        /// <summary>
        /// The minimum number of logs to read for each log chunk sent to the client.
        /// </summary>
        private static readonly int _MinimumLogsDownloadChunkCount_ = 10;
        /// <summary>
        /// The maximum number of logs to read for each log chunk sent to the client.
        /// </summary>
        private static readonly int _MaximumLogsDownloadChunkCount_ = 100;
        /// <summary>
        /// The minimum delay to wait between sending log chunks to the client.
        /// </summary>
        private static readonly TimeSpan _MinimumlogsDownloadInterstitialDelay_ = TimeSpan.Zero;
        #endregion
        #region Events
        /// <summary>
        /// Fired when a message is received.
        /// </summary>
        public event EventHandler<Networking.MessageEventArgs> MessageBus;
        /// <summary>
        /// Attempts to broadcast the <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="Networking.MessageEventArgs"/> to broadcast.</param>
        protected void RaiseMessageBusEvent(Networking.MessageEventArgs response) => MessageBus?.Invoke(this, response);
        #endregion
        #region Ctor
        private ServiceManager()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="ServiceManager"/> using the given <see cref="ComponentManagement.IComponentManager"/> as the component controller.
        /// </summary>
        /// <param name="serviceManagerId">The value which uniquely identifies this <see cref="ServiceManager"/>.</param>
        /// <param name="loginEvidence">Challenge evidence required to login and access this <see cref="ServiceManager"/>.</param>
        /// <param name="maximumLoginsAllowed">The maximum number of clients allowed to log in at any one time.</param>
        /// <param name="requestsPurgeInterval">The amount of time to wait before checking the cache for any item that can be purged.</param>
        /// <param name="requestsResponseTimeout">The amount of time a cached item will remain in the cache before timing out and being purged on the next <paramref name="requestsPurgeInterval"/>.</param>
        /// <param name="sessionCacheTimeLimit">The amount of time a session will remain in the cache, without activity, before timing out.</param>
        /// <param name="executeRemainingRequestsOnStop">When running <see cref="TryStop(out Exception)"/> or <see cref="TryRestart(TimeSpan, bool, bool, out Exception)"/>, if this value is <b>true</b>, then all remaining cached requests will have their <see cref="Cache.ICacheProcessorItem.Process"/> executed, otherwise, setting this to <b>false</b> will terminate all remaining cached requests <i>without</i> executing their <see cref="Cache.ICacheProcessorItem.Process"/>.</param>
        /// <param name="componentManager">The <see cref="ComponentManagement.IComponentManager"/> to use as the component controller.</param>
        /// <param name="servicesRestartDelay">The amount of time to wait after, and between, starting services, stopping services, scanning for new services and restarting services.</param>
        /// <param name="allowLogsDownload">Determines if the service manager will allow logs to be downloaded.</param>
        /// <param name="logsDownloadChunkCount">The number of <see cref="Logging.ILogEntry"/>s to include with each chunk of <see cref="Logging.Logs"/> sent.</param>
        /// <param name="logsDownloadInterstitialDelay">The amount of time to wait between downloading log chunks.</param>
        /// <param name="allowFileUpload">Determines if this service manager will allow files to be transfered to its <see cref="FileTransferRootPath"/>.</param>
        /// <param name="allowFileDownload">Determines if this service manager will allow files to be downloaded.</param>
        /// <param name="fileTransferRootPath">The root directory to write uploaded files to.</param>
        /// <param name="fileTransferHashAlgorithmType">The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create the verification hash when transferring a file.</param>
        /// <param name="fileTransferSegmentLength">The length, in bytes, that each file segment will be split into.</param>
        /// <param name="fileTransferInterstitialDelay">The amount of time to wait between sending and receiving file segments.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="auditMessages">Sets whether messages will be audited in the logs</param>
        /// <param name="auditDebugMessages">Sets whether debug information will be audited in the logs.</param>
        public ServiceManager(string serviceManagerId,
                              byte[] loginEvidence,
                              int maximumLoginsAllowed,
                              TimeSpan requestsPurgeInterval,
                              TimeSpan requestsResponseTimeout,
                              TimeSpan sessionCacheTimeLimit,
                              bool executeRemainingRequestsOnStop,
                              ComponentManagement.IComponentManager componentManager,
                              TimeSpan servicesRestartDelay,
                              bool allowLogsDownload,
                              int logsDownloadChunkCount,
                              TimeSpan logsDownloadInterstitialDelay,
                              bool allowFileUpload,
                              bool allowFileDownload,
                              string fileTransferRootPath,
                              string fileTransferHashAlgorithmType,
                              long fileTransferSegmentLength,
                              TimeSpan fileTransferInterstitialDelay,
                              string logSourceId,
                              bool auditMessages,
                              bool auditDebugMessages)
            : this()
        {
            // #### SERVICE MANAGER
            if (string.IsNullOrWhiteSpace(serviceManagerId))
            {
                throw new ArgumentNullException(nameof(serviceManagerId));
            }
            ServiceManagerId = serviceManagerId;
            // ----
            if ((loginEvidence == null) || (loginEvidence.Length == 0))
            {
                throw new ArgumentNullException(nameof(loginEvidence));
            }
            LoginEvidence = loginEvidence;
            // ----
            MaximumLoginsAllowed = maximumLoginsAllowed;
            // ----
            RequestsPurgeInterval = requestsPurgeInterval;
            RequestsResponseTimeout = requestsResponseTimeout;
            SessionCacheTimeLimit = sessionCacheTimeLimit;
            ExecuteRemainingRequestsOnStop = executeRemainingRequestsOnStop;

            // #### COMPONENT MANAGER
            ComponentManager = componentManager ?? throw new ArgumentNullException(nameof(componentManager));

            // ####
            ServicesRestartDelay = (servicesRestartDelay < TimeSpan.Zero) ? TimeSpan.Zero : servicesRestartDelay;

            // #### LOG DOWNLOADING
            AllowLogsDownload = allowLogsDownload;
            LogsDownloadChunkCount = (logsDownloadChunkCount < _MinimumLogsDownloadChunkCount_) ? _MinimumLogsDownloadChunkCount_ : ((logsDownloadChunkCount > _MaximumLogsDownloadChunkCount_) ? _MaximumLogsDownloadChunkCount_ : logsDownloadChunkCount);
            LogsDownloadInterstitialDelay = (logsDownloadInterstitialDelay < _MinimumlogsDownloadInterstitialDelay_) ? _MinimumlogsDownloadInterstitialDelay_ : logsDownloadInterstitialDelay;

            // #### FILE TRANSFER
            AllowFileUpload = allowFileUpload;
            AllowFileDownload = allowFileDownload;
            // ----
            if (string.IsNullOrWhiteSpace(fileTransferRootPath))
            {
                throw new ArgumentNullException(nameof(fileTransferRootPath));
            }
            if (!System.IO.Directory.Exists(fileTransferRootPath))
            {
                throw new System.IO.DirectoryNotFoundException($"Directory must exist: {fileTransferRootPath}");
            }
            FileTransferRootPath = fileTransferRootPath;
            //
            if (string.IsNullOrWhiteSpace(fileTransferHashAlgorithmType))
            {
                throw new ArgumentNullException(nameof(fileTransferHashAlgorithmType));
            }
            FileTransferHashAlgorithmType = fileTransferHashAlgorithmType;
            // ----
            FileTransferSegmentLength = fileTransferSegmentLength;
            FileTransferInterstitialDelay = fileTransferInterstitialDelay;

            // #### LOG
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
            // ----
            AuditMessages = auditMessages;
            AuditDebugMessages = auditDebugMessages;

            // #### #### #### #### ####
            // ####  DO THIS LAST  ####
            if (ComponentManager.IsRunning)
            {
                Initialize();
            }
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ServiceManager(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "ServiceManager")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ServiceManager\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // string ServiceManagerId
            ServiceManagerId = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ServiceManagerId", typeof(string)).Value;
            // byte[] LoginEvidence
            LoginEvidence = (byte[])Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LoginEvidence", typeof(byte[])).Value;
            // int MaximumLoginsAllowed
            MaximumLoginsAllowed = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "MaximumLoginsAllowed", typeof(int)).Value;
            // TimeSpan RequestsPurgeInterval
            RequestsPurgeInterval = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "RequestsPurgeInterval", typeof(TimeSpan)).Value;
            // TimeSpan RequestsResponseTimeout
            RequestsResponseTimeout = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "RequestsResponseTimeout", typeof(TimeSpan)).Value;
            // TimeSpan SessionCacheTimeLimit
            SessionCacheTimeLimit = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SessionCacheTimeLimit", typeof(TimeSpan)).Value;
            // bool ExecuteRemainingRequestsOnStop
            ExecuteRemainingRequestsOnStop = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ExecuteRemainingRequestsOnStop", typeof(bool)).Value;
            // TimeSpan ServicesRestartDelay
            ServicesRestartDelay = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ServicesRestartDelay", typeof(TimeSpan)).Value;
            // bool AllowLogsDownload
            AllowLogsDownload = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AllowLogsDownload", typeof(bool)).Value;
            // int LogsDownloadChunkCount
            LogsDownloadChunkCount = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogsDownloadChunkCount", typeof(int)).Value;
            // TimeSpan LogsDownloadInterstitialDelay
            LogsDownloadInterstitialDelay = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogsDownloadInterstitialDelay", typeof(TimeSpan)).Value;
            // bool AllowFileUpload
            AllowFileUpload = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AllowFileUpload", typeof(bool)).Value;
            // bool AllowFileDownload
            AllowFileDownload = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AllowFileDownload", typeof(bool)).Value;
            // string FileTransferRootPath
            FileTransferRootPath = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FileTransferRootPath", typeof(string)).Value;
            // string FileTransferHashAlgorithmType
            FileTransferHashAlgorithmType = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FileTransferHashAlgorithmType", typeof(string)).Value;
            // long FileTransferSegmentLength
            FileTransferSegmentLength = (long)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FileTransferSegmentLength", typeof(long)).Value;
            // TimeSpan FileTransferInterstitialDelay
            FileTransferInterstitialDelay = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "FileTransferInterstitialDelay", typeof(TimeSpan)).Value;
            // string LogSourceId
            LogSourceId = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogSourceId", typeof(string)).Value;
            // bool AuditMessages
            AuditMessages = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AuditMessages", typeof(bool)).Value;
            // bool AuditDebugMessages
            AuditDebugMessages = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "AuditDebugMessages", typeof(bool)).Value;
            // ComponentManager
            if (!configuration.ContainsKey("ComponentManager"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"ComponentManager\".", nameof(configuration));
            }
            ComponentManager = (ComponentManagement.ComponentManager)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(ComponentManagement.ComponentManager), configuration["ComponentManager"]);
        }
        #endregion
        #region Private Fields
        // The service manager's communication client name.
        private readonly string _ServiceManagerClientId = $"SrvMng{Guid.NewGuid().ToString("N")}";
        //
        private bool _HasBeenStarted = false;
        private readonly Cache.CacheProcessor _CacheProcessor = new Cache.CacheProcessor();
        private Networking.IClient _Client;
        private int _MaximumLoginsAllowed;
        // statistics
        private long _ReceivedRequests = 0;
        private long _ProcessedRequests = 0;
        private long _TotalThreads = 0;
        private long _WaitingThreads = 0;
        private long _ActiveThreads = 0;
        #endregion
        #region IServiceManager Methods
        /// <summary>
        /// A value which uniquely identifies a <see cref="IServiceManager"/>.
        /// </summary>
        public string ServiceManagerId
        {
            get;
        }
        //
        /// <summary>
        /// Challenge evidence required to login and access a <see cref="ServiceManager"/>.
        /// </summary>
        public byte[] LoginEvidence
        {
            get; set;
        }
        /// <summary>
        /// The maximum number of clients allowed to log in at any one time.
        /// </summary>
        public int MaximumLoginsAllowed
        {
            get => _MaximumLoginsAllowed;
            set => _MaximumLoginsAllowed = (value < _MinimumLoginsAllowed_) ? _MinimumLoginsAllowed_ : value;
        }
        //
        /// <summary>
        /// The amount of time to wait before checking the cache for any item that can be purged.
        /// </summary>
        public TimeSpan RequestsPurgeInterval
        {
            get;
        }
        /// <summary>
        /// The amount of time a cached item will remain in the cache before timing out and being purged on the next <see cref="RequestsPurgeInterval"/>.
        /// </summary>
        public TimeSpan RequestsResponseTimeout
        {
            get;
        }
        /// <summary>
        /// The amount of time a session will remain in the cache, without activity, before timing out.
        /// </summary>
        public TimeSpan SessionCacheTimeLimit
        {
            get;
        }
        /// <summary>
        /// If <b>true</b> all remaining cached requests will have their <see cref="Cache.ICacheProcessorItem.Process"/> executed; otherwise, setting this to <b>false</b> will terminate all remaining cached requests without executing their <see cref="Cache.ICacheProcessorItem.Process"/>.
        /// </summary>
        public bool ExecuteRemainingRequestsOnStop
        {
            get; set;
        }
        //
        /// <summary>
        /// The <see cref="ComponentManagement.IComponentManager"/> serviced by this <see cref="IServiceManager"/>.
        /// </summary>
        public ComponentManagement.IComponentManager ComponentManager
        {
            get;
        }
        //
        /// <summary>
        /// The amount of time to wait after, and between, starting services, stopping services, scanning for new services and restarting services.
        /// </summary>
        public TimeSpan ServicesRestartDelay
        {
            get;
        }
        //
        /// <summary>
        /// Indicates the operating state of the <see cref="IServiceManager"/>.
        /// </summary>
        public bool IsRunning => (ComponentManager.IsRunning && _HasBeenStarted);
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IServiceManager"/> started.
        /// </summary>
        public DateTimeOffset StartTime
        {
            get; private set;
        }
        /// <summary>
        /// Returns the amount of time this <see cref="IServiceManager"/> has been running.
        /// </summary>
        public TimeSpan RunTime => DateTimeOffset.UtcNow - StartTime.ToUniversalTime();
        /// <summary>
        /// Will start the <see cref="ComponentManager"/>, if it is not already running, and connect to it's communication system.
        /// </summary>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        public bool TryStart(bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting, out Exception exception)
        {
            try
            {
                if (!_HasBeenStarted)
                {
                    if (!ComponentManager.IsRunning)
                    {
                        ComponentManager.Start(clearLogBeforeStarting, clearArchivedLogsBeforeStarting);
                    }
                    Initialize();
                }
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Will stop and restart the <see cref="ComponentManagement.IComponentManager"/>.
        /// </summary>
        /// <param name="restartDelay">The time to wait after stopping before starting again.</param>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        public bool TryRestart(TimeSpan restartDelay, bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting, out Exception exception)
        {
            if (TryStop(out exception))
            {
                Threading.ThreadingHelper.Sleep(restartDelay);
                if (TryStart(clearLogBeforeStarting, clearArchivedLogsBeforeStarting, out exception))
                {
                    exception = null;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Will disconnect from the <see cref="ComponentManager"/>'s communication system and stop the <see cref="ComponentManager"/>, if it is running.
        /// </summary>
        /// <param name="exception">If <b>false</b> is returned, this value will hold the <see cref="Exception"/> that occurred.</param>
        /// <returns><b>True</b> if the operation was successful; otherwise <b>false</b>.</returns>
        public bool TryStop(out Exception exception)
        {
            try
            {
                if (_HasBeenStarted)
                {
                    AuditMessage(Logging.LogEntryType.Information, $"Stopping Service Manager, Id={ServiceManagerId}, {Common.TypeHelper.FormatDisplayName(this.GetType())}");
                    _CacheProcessor.Stop(ExecuteRemainingRequestsOnStop);
                    DisconnectFromCommBus();
                    if (ComponentManager.IsRunning)
                    {
                        ComponentManager.Stop();
                    }
                    _HasBeenStarted = false;
                    StartTime = DateTimeOffset.MinValue;
                }
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                exception = ex;
                return false;
            }
        }
        //
        /// <summary>
        /// The number of requests received.
        /// </summary>
        public long ReceivedRequests
        {
            get => Interlocked.Read(ref _ReceivedRequests);
            private set => Interlocked.Exchange(ref _ReceivedRequests, value);
        }
        /// <summary>
        /// The number of requests that have been processed.
        /// </summary>
        public long ProcessedRequests
        {
            get => Interlocked.Read(ref _ProcessedRequests);
            private set => Interlocked.Exchange(ref _ProcessedRequests, value);
        }
        /// <summary>
        /// The number of requests currently being managed by the <see cref="IServiceManager"/>.
        /// </summary>
        public long WorkingRequests => ReceivedRequests - ProcessedRequests;
        //
        /// <summary>
        /// Determines if the service manager will allow logs to be downloaded.
        /// </summary>
        public bool AllowLogsDownload
        {
            get; set;
        }
        /// <summary>
        /// The number of <see cref="Logging.ILogEntry"/>s to include with each chunk of <see cref="Logging.Logs"/> sent.
        /// </summary>
        public int LogsDownloadChunkCount
        {
            get; set;
        }
        /// <summary>
        /// The amount of time to wait between downloading log chunks.
        /// </summary>
        public TimeSpan LogsDownloadInterstitialDelay
        {
            get; set;
        }
        /// <summary>
        /// The number of clients currently logged into the service manager.
        /// </summary>
        public int LoginCount
        {
            get => (from x in _CacheProcessor.Items where (x.Payload as CachedServiceRequest).Command == RequestResponseCommands.Login select x).Count();
        }
        //
        /// <summary>
        /// Determines if this service manager will allow files to be transfered to its <see cref="FileTransferRootPath"/>.
        /// </summary>
        public bool AllowFileUpload
        {
            get; set;
        }
        /// <summary>
        /// Determines if this service manager will allow files to be downloaded.
        /// </summary>
        public bool AllowFileDownload
        {
            get; set;
        }
        /// <summary>
        /// The root directory path to write uploaded files to.
        /// </summary>
        public string FileTransferRootPath
        {
            get;
        }
        /// <summary>
        /// The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create the verification hash when transferring a file.
        /// </summary>
        public string FileTransferHashAlgorithmType
        {
            get; set;
        }
        //
        /// <summary>
        /// The length, in bytes, that each file segment will be split into.
        /// </summary>
        public long FileTransferSegmentLength
        {
            get; set;
        }
        /// <summary>
        /// the amount of time to wait between sending and receiving file segments during a file transfer.
        /// </summary>
        public TimeSpan FileTransferInterstitialDelay
        {
            get; set;
        }
        //
        /// <summary>
        /// The total number of background threads created.
        /// </summary>
        public long TotalThreads
        {
            get => Interlocked.Read(ref _TotalThreads);
            private set => Interlocked.Exchange(ref _TotalThreads, value);
        }
        /// <summary>
        /// The number of background threads currently executing, or waiting to be executed.
        /// </summary>
        public long WaitingThreads
        {
            get => Interlocked.Read(ref _WaitingThreads);
            private set => Interlocked.Exchange(ref _WaitingThreads, value);
        }
        /// <summary>
        /// The number of background threads currently executing.
        /// </summary>
        public long ActiveThreads
        {
            get => Interlocked.Read(ref _ActiveThreads);
            private set => Interlocked.Exchange(ref _ActiveThreads, value);
        }
        //
        /// <summary>
        /// Allows logs to be written to the <see cref="ComponentManager"/>'s log.
        /// </summary>
        public Logging.ILogWriter LogWriter => (ComponentManager.LogController.LogMarshal as Logging.ILogWriter);
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId
        {
            get;
        }
        /// <summary>
        /// Gets or sets whether messages will be audited in the logs.
        /// </summary>
        public bool AuditMessages
        {
            get; set;
        }
        /// <summary>
        /// Gets or sets whether debug information will be audited in the logs.
        /// </summary>
        public bool AuditDebugMessages
        {
            get; set;
        }
        #endregion
        #region Private Methods
        /// <summary>
        /// Instantiates an instance of the <see cref="FileTransferHashAlgorithm"/>.
        /// </summary>
        private System.Security.Cryptography.HashAlgorithm FileTransferHashAlgorithm => (System.Security.Cryptography.HashAlgorithm)Common.InstantiationHelper.InvokeDefaultCtor(Type.GetType(FileTransferHashAlgorithmType));
        /// <summary>
        /// Starts all of the subsystems this <see cref="IServiceManager"/> requires and prepares it for operation.
        /// </summary>
        private void Initialize()
        {
            AuditMessage(Logging.LogEntryType.Information, $"Starting Service Manager, Id={this.ServiceManagerId}, {Common.TypeHelper.FormatDisplayName(this.GetType())}");
            var stWatch = System.Diagnostics.Stopwatch.StartNew();
            ConnectToCommBus();
            ReceivedRequests = 0;
            ProcessedRequests = 0;
            _CacheProcessor.Start(RequestsPurgeInterval);
            _HasBeenStarted = true;
            StartTime = DateTimeOffset.UtcNow;
            AuditMessage(Logging.LogEntryType.Information, $"Service Manager Started, Id={this.ServiceManagerId}, Elapsed Time={stWatch.Elapsed}");
        }
        /// <summary>
        /// Attempts to find the cached request, by GroupKey, and then extract the cached request's data.
        /// </summary>
        /// <param name="request">The request to get the GroupKey from.</param>
        /// <param name="results">The cached request's data, or null.</param>
        /// <returns>A boolean indicating the success or failure of finding the requested information.</returns>
        private bool TryFindCachedRequest(ServiceRequest request, out CachedServiceRequest results)
        {
            // find cached request
            var cachedDude = _CacheProcessor.Find(request.GroupKey);
            if (cachedDude != null)
            {
                // get cached request
                results = cachedDude as CachedServiceRequest;
                // reset cached item
                results.Validater.Reset();
                return true;
            }
            results = null;
            return false;
        }
        /// <summary>
        /// Attempts to find the cached request, by GroupKey, and then extract the cached request's data.
        /// </summary>
        /// <typeparam name="T">The type to cast the cached request's data to.</typeparam>
        /// <param name="request">The request to get the GroupKey from.</param>
        /// <param name="cachedServiceRequest">The found cached request, or null.</param>
        /// <param name="cachedServiceRequestData">The cached request's data cast to a type of <typeparamref name="T"/>.</param>
        /// <returns>A boolean indicating the success or failure of finding the requested information.</returns>
        private bool TryFindCachedRequestData<T>(ServiceRequest request, out CachedServiceRequest cachedServiceRequest, out T cachedServiceRequestData)
            where T : class
        {
            // find cached request
            if (TryFindCachedRequest(request, out cachedServiceRequest))
            {
                // get cached request's data
                if (cachedServiceRequest.Data != null)
                {
                    cachedServiceRequestData = cachedServiceRequest.Data as T;
                    // returns whether it was able to find the data object AND cast it to type of <T>
                    return (cachedServiceRequestData != null);
                }
            }
            //
            cachedServiceRequestData = null;
            return false;
        }
        /// <summary>
        /// Puts an <see cref="Networking.IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <typeparam name="T">The typeof the <paramref name="payload"/> being distributed.</typeparam>
        /// <param name="targetClientId">The id of the <see cref="Networking.IClient"/> which should receive the <see cref="Networking.IMessage"/>.</param>
        /// <param name="payload">The data being transported.</param>
        private void SendMessage<T>(string targetClientId, T payload)
        {
            if (!IsRunning)
            {
                throw new NotSupportedException("Cannot send message. ServiceManager is not running.");
            }
            else if (_Client.State != Networking.ChannelStates.Open)
            {
                throw new InvalidOperationException($"Cannot send message. ServiceManager is in invalid state. State={_Client.State}");
            }
            else
            {
                _Client.SendMessage(targetClientId, payload);
            }
        }
        /// <summary>
        /// Searches the <see cref="ComponentManager"/> for the specified component.
        /// </summary>
        /// <param name="id">The id of the component to search for.</param>
        /// <param name="isExtension">Indicates whether the component is an extension or a plugin.</param>
        /// <returns>The <see cref="ComponentManager"/> being searched for or null.</returns>
        private ComponentManagement.ComponentFactory FindComponent(string id, out bool isExtension)
        {
            ComponentManagement.ComponentFactory found = null;
            isExtension = false;
            // search extensions
            foreach (var item in ComponentManager.ComponentController.ExtensionComponents)
            {
                if (item.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                {
                    isExtension = true;
                    found = item;
                }
            }
            if (found == null)
            {
                // search plugins
                foreach (var item in ComponentManager.ComponentController.PluginComponents)
                {
                    if (item.Id.Equals(id, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = item;
                    }
                }
            }
            // 
            return found;
        }
        #endregion
        #region Configuration.IConfigurationGroup Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("ServiceManager");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // string ServiceManagerId
                result.Items.Add("ServiceManagerId", ServiceManagerId, ServiceManagerId.GetType());
                // byte[] LoginEvidence
                result.Items.Add("LoginEvidence", LoginEvidence, LoginEvidence.GetType());
                // int MaximumLoginsAllowed
                result.Items.Add("MaximumLoginsAllowed", MaximumLoginsAllowed, MaximumLoginsAllowed.GetType());
                // TimeSpan RequestsPurgeInterval
                result.Items.Add("RequestsPurgeInterval", RequestsPurgeInterval, RequestsPurgeInterval.GetType());
                // TimeSpan RequestsResponseTimeout
                result.Items.Add("RequestsResponseTimeout", RequestsResponseTimeout, RequestsResponseTimeout.GetType());
                // TimeSpan SessionCacheTimeLimit
                result.Items.Add("SessionCacheTimeLimit", SessionCacheTimeLimit, SessionCacheTimeLimit.GetType());
                // bool ExecuteRemainingRequestsOnStop
                result.Items.Add("ExecuteRemainingRequestsOnStop", ExecuteRemainingRequestsOnStop, ExecuteRemainingRequestsOnStop.GetType());
                // TimeSpan ServicesRestartDelay
                result.Items.Add("ServicesRestartDelay", ServicesRestartDelay, ServicesRestartDelay.GetType());
                // bool AllowLogsDownload
                result.Items.Add("AllowLogsDownload", AllowLogsDownload, AllowLogsDownload.GetType());
                // int LogsDownloadChunkCount
                result.Items.Add("LogsDownloadChunkCount", LogsDownloadChunkCount, LogsDownloadChunkCount.GetType());
                // TimeSpan LogsDownloadInterstitialDelay
                result.Items.Add("LogsDownloadInterstitialDelay", LogsDownloadInterstitialDelay, LogsDownloadInterstitialDelay.GetType());
                // bool AllowFileUpload
                result.Items.Add("AllowFileUpload", AllowFileUpload, AllowFileUpload.GetType());
                // bool AllowFileDownload
                result.Items.Add("AllowFileDownload", AllowFileDownload, AllowFileDownload.GetType());
                // string FileTransferRootPath
                result.Items.Add("FileTransferRootPath", FileTransferRootPath, FileTransferRootPath.GetType());
                // string FileTransferHashAlgorithmType
                result.Items.Add("FileTransferHashAlgorithmType", FileTransferHashAlgorithmType, FileTransferHashAlgorithmType.GetType());
                // long FileTransferSegmentLength
                result.Items.Add("FileTransferSegmentLength", FileTransferSegmentLength, FileTransferSegmentLength.GetType());
                // TimeSpan FileTransferInterstitialDelay
                result.Items.Add("FileTransferInterstitialDelay", FileTransferInterstitialDelay, FileTransferInterstitialDelay.GetType());
                // string LogSourceId
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());
                // bool AuditMessages
                result.Items.Add("AuditMessages", AuditMessages, AuditMessages.GetType());
                // bool AuditDebugMessages
                result.Items.Add("AuditDebugMessages", AuditDebugMessages, AuditDebugMessages.GetType());
                // ComponentManager
                result.Add(ComponentManager.Configuration);
                //
                return result;
            }
        }
        #endregion
        #region BOILERPLATE: MESSAGE PROCESSING 
        private void ConnectToCommBus()
        {
            if (_Client == null)
            {
                // create receivable and transmittable type filters
                var receivableTypesFilter = new List<Networking.IPayloadTypeInfo>() { new Networking.PayloadTypeInfo(typeof(ServiceRequest)), new Networking.PayloadTypeInfo(typeof(ServiceResponse)) };
                var transmittableTypesFilter = new List<Networking.IPayloadTypeInfo>() { new Networking.PayloadTypeInfo(typeof(ServiceRequest)), new Networking.PayloadTypeInfo(typeof(ServiceResponse)) };
                // create client
                _Client = ComponentManager.CommunicationController.CreateSharedClient(new Networking.ClientConfiguration(_ServiceManagerClientId, false, receivableTypesFilter, transmittableTypesFilter));
                // connect to client's message bus
                _Client.MessageBus += MainServer_MessageBus;
                // open client
                if (_Client.Open(out Exception openException) != Networking.ChannelStates.Open)
                {
                    if (AuditDebugMessages)
                    {
                        AuditMessage(Logging.LogEntryType.Error, $"Starting Service Manager, Unable to open service manager client. {openException.Message}");
                        ComponentManager.LogController.LogMarshal.Flush();
                        Threading.ThreadingHelper.Sleep(1000);
                    }
                    throw openException;
                }
            }
        }

        private void DisconnectFromCommBus()
        {
            if (_Client != null)
            {
                // disconnect from the client
                _Client.MessageBus -= MainServer_MessageBus;
                // close the client
                _Client.Close(out Exception closeException);
                _Client = null;
            }
        }

        private void MainServer_MessageBus(object sender, Networking.MessageEventArgs e)
        {
            // #### check for ServiceRequest
            var request = e.Message.PayloadMessage<ServiceRequest>();
            if (request != null)
            {
                // process ServiceRequest
                Interlocked.Increment(ref _ReceivedRequests);
                Interlocked.Increment(ref _TotalThreads);
                Interlocked.Increment(ref _WaitingThreads);
                Task.Run(() =>
                {
                    // control statistics
                    Interlocked.Decrement(ref _WaitingThreads);
                    Interlocked.Increment(ref _ActiveThreads);
                    // audit debug message
                    AuditDebugMessage(request);
                    // process request
                    ProcessServiceRequest(request, e.Message.ClientId);
                    // control statistics
                    Interlocked.Decrement(ref _ActiveThreads);
                    Interlocked.Increment(ref _ProcessedRequests);
                });
            }
            // #### relay message to the local MessageBus
            RaiseMessageBusEvent(e);
        }

        private void SendServiceResponse(string targetClientId, ServiceRequest request, RequestResponseCommands command, object data)
        {
            // create response
            var response = new ServiceResponse(request.GroupKey, command, data);
            // audit response
            AuditDebugMessage(response);
            // send response to original client
            SendMessage(targetClientId, response);
        }

        private void SendServiceError(string targetClientId, ServiceRequest request, RequestResponseTypes.Error errorObj)
        {
            // create error response
            var response = new ServiceResponse(request.SessionId, request.GroupKey, RequestResponseCommands.Error, request.ServiceManagerId, errorObj);
            // audit response
            errorObj.Description += $", Command={request.Command}";
            AuditMessage(Logging.LogEntryType.Error, $"{errorObj}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            // send response to original client
            SendMessage(targetClientId, response);
        }
        #endregion
        #region BOILERPLATE: AUDIT
        private void AuditDebugMessage(ServiceRequest request)
        {
            if (AuditDebugMessages)
            {
                ComponentManager.LogController.LogMarshal.Write(Logging.LogEntryType.Debug, LogSourceId, $"Type={request.GetType().FullName}, {request}");
            }
        }

        private void AuditDebugMessage(ServiceResponse response)
        {
            if (AuditDebugMessages)
            {
                ComponentManager.LogController.LogMarshal.Write(Logging.LogEntryType.Debug, LogSourceId, $"Type={response.GetType().FullName}, {response}");
            }
        }

        private void AuditMessage(Logging.LogEntryType entryType, string message)
        {
            if (entryType == Logging.LogEntryType.Debug)
            {
                if (AuditDebugMessages)
                {
                    ComponentManager.LogController.LogMarshal.Write(entryType, LogSourceId, message);
                }
            }
            else
            {
                if (AuditMessages)
                {
                    ComponentManager.LogController.LogMarshal.Write(entryType, LogSourceId, message);
                }
            }
        }

        private void AuditMessageLogs(Logging.Logs logs)
        {
            if (AuditMessages)
            {
                ComponentManager.LogController.LogMarshal.Write(logs);
            }
        }
        #endregion
        #region BOILERPLATE: HANDLE BASIC REQUESTS, GATEWAY and REDIRECTION OF ALL OTHER SERVICE REQUESTS
        // ################ HANDLER and REDIRECTOR
        private void ProcessServiceRequest(ServiceRequest request, string targetClientId)
        {
            switch (request.Command)
            {
                // ######## HANDLE BASIC COMMANDS (no session id required)

                case RequestResponseCommands.Ping:
                    (request.Data as RequestResponseTypes.Ping)?.MarkReceivedTime();
                    AuditMessage(Logging.LogEntryType.Information, $"Ping, Client={targetClientId}, Travel Time From Client={(request.Data as RequestResponseTypes.Ping)?.TravelToTime}");
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.Ping, request.Data);
                    break;
                case RequestResponseCommands.IsSessionAlive:
                    ProcessCommand_IsSessionAlive(request, targetClientId);
                    break;

                // ######## HANDLE LOGIN COMMANDS (no session id required)

                case RequestResponseCommands.LoginRequest:
                    ProcessCommand_ServiceManager_LoginRequest(request, targetClientId);
                    break;
                case RequestResponseCommands.Login:
                    ProcessCommand_ServiceManager_Login(request, targetClientId);
                    break;

                // ######## HANDLE LOGOUT
                case RequestResponseCommands.Logout:
                    ProcessCommand_Logout(request, targetClientId);
                    break;

                // ######## REDIRECT ALL OTHER COMMANDs (session id required)

                default:
                    var commandName = request.Command.ToString();
                    // #### test for proper SESSIONID and CLIENTID
                    if (IsSessionAliveGateway(request.SessionId, targetClientId))
                    {
                        // #### valid SESSIONID and CLIENTID
                        // audit service request received 
                        AuditMessage(Logging.LogEntryType.Debug, $"{commandName}, SessionId={request.SessionId}");
                        // redirect request
                        RedirectServiceRequest(request, targetClientId);
                    }
                    break;
            }

            // ################################################
            // Internal Functions
            // ################################################

            bool IsSessionAliveGateway(string isaSessionId, string isaTargetClientId)
            {
                // #### Tests the SessionId and the ClientId for validity. 
                // #### Only allow requests from registered Clients with valid SessionIds through.

                // test if sessionId is currently in the cache
                if (_CacheProcessor.TryFind(isaSessionId, out CachedServiceRequest isaCachedItem))
                {
                    if (isaCachedItem.ClientId == isaTargetClientId)
                    {
                        // #### session found and targetClientId is valid
                        // reset cached item's validater
                        isaCachedItem.Validater.Reset();
                        return true;
                    }
                    else
                    {
                        // #### wrong clientId
                        // audit failure, invalid clientId
                        SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Invalid ClientId"));
                        return false;
                    }
                }
                // #### no session found
                // audit failure, no cached item
                SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Invalid SessionId"));
                return false;
            }
        }

        // ################ PROCESS: IS SESSION ALIVE

        private void ProcessCommand_IsSessionAlive(ServiceRequest request, string targetClientId)
        {
            RequestResponseTypes.IsSessionAlive responseObj = null;
            if (request.Data == null)
            {
                // no sessionId given
                AuditMessage(Logging.LogEntryType.Error, $"IsSessionAlive=False, SessionId is Null, ClientId={targetClientId}");
                responseObj = new RequestResponseTypes.IsSessionAlive(false, "SessionId is Null", null, targetClientId, DateTimeOffset.MinValue, TimeSpan.Zero);
            }
            else
            {
                var sessionId = (request.Data as string);
                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    // search cache for session id
                    if (_CacheProcessor.Contains(sessionId))
                    {
                        // found it
                        var dude = _CacheProcessor.Find(sessionId) as CachedServiceRequest;
                        var dudeStr = $"ClientId={targetClientId}, SessionId={sessionId}";
                        // check for proper clientId
                        if (targetClientId == dude.ClientId)
                        {
                            // send TRUE results
                            dude.Validater.Reset();
                            AuditMessage(Logging.LogEntryType.Information, $"IsSessionAlive=True, {dudeStr}, Created={dude.CreatedTimeUtc}, Runtime={dude.CachedTime}");
                            responseObj = new RequestResponseTypes.IsSessionAlive(true, "", sessionId, targetClientId, dude.CreatedTimeUtc, dude.CachedTime);
                        }
                        else
                        {
                            // send FALSE results
                            AuditMessage(Logging.LogEntryType.Error, $"IsSessionAlive=False, Invalid ClientId, {dudeStr}");
                            responseObj = new RequestResponseTypes.IsSessionAlive(false, "Invalid ClientId", sessionId, targetClientId, DateTimeOffset.MinValue, TimeSpan.Zero);
                        }
                    }
                    else
                    {
                        // not found
                        AuditMessage(Logging.LogEntryType.Error, $"IsSessionAlive=False, Invalid SessionId, ClientId={targetClientId}, SessionId={sessionId}");
                        responseObj = new RequestResponseTypes.IsSessionAlive(false, "Invalid SessionId", sessionId, targetClientId, DateTimeOffset.MinValue, TimeSpan.Zero);
                    }
                }
                else
                {
                    // sessionId not a <string>
                    AuditMessage(Logging.LogEntryType.Error, $"IsSessionAlive=False, SessionId is not a string, SessionId Type={request.Data.GetType().FullName}, ClientId={targetClientId}");
                    responseObj = new RequestResponseTypes.IsSessionAlive(false, "SessionId is not a string", sessionId, targetClientId, DateTimeOffset.MinValue, TimeSpan.Zero);
                }
            }
            //
            SendServiceResponse(targetClientId, request, RequestResponseCommands.IsSessionAlive, responseObj);
        }

        // ################ PROCESS: LOGIN REQUEST

        private void ProcessCommand_ServiceManager_LoginRequest(ServiceRequest request, string targetClientId)
        {
            // check if the maximum number of logins allowed has been reached
            var loginCounter = LoginCount;
            if (loginCounter >= _MaximumLoginsAllowed)
            {
                SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Login Denied. Maximum number of logins reached. Logins={loginCounter}, Maximum Allowed={MaximumLoginsAllowed}"));
                return;
            }
            // create public/private keys
            var publicPrivateKeys = Cryptography.CryptographyHelper.GeneratePublicPrivateKeys();
            // cache request 
            // #### STARTING THE LOGIN PROCESS ####
            var cachedLoginRequest = new CachedServiceRequest(request.GroupKey,
                                                              new Cache.ValidateTime<Cache.ICacheProcessorItem>(DateTime.Now.Add(RequestsResponseTimeout)),
                                                              request.Command,
                                                              targetClientId,
                                                              publicPrivateKeys,
                                                              (item) =>
                                                              {
                                                                  // #### LOGIN PROCESS HAS COMPLETED
                                                              });
            // cache request
            _CacheProcessor.Add(cachedLoginRequest);
            // send response
            AuditMessage(Logging.LogEntryType.Information, $"Login Request, Client={targetClientId}, GroupKey={request.GroupKey}");
            // respond with a "ServiceManagerLoginRequestResponse" type; passing the Public Key
            var responseObj = new RequestResponseTypes.LoginRequestResponse(publicPrivateKeys.Item1);
            SendServiceResponse(targetClientId, request, RequestResponseCommands.LoginRequestResponse, responseObj);
        }

        // ################ PROCESS: LOGIN

        private void ProcessCommand_ServiceManager_Login(ServiceRequest request, string targetClientId)
        {
            if (TryFindCachedRequestData(request, out CachedServiceRequest cachedServiceRequest, out Tuple<string, string> cachedServiceRequestData))
            {
                // get cached data
                var serviceManagerPublicKey = cachedServiceRequestData.Item1;
                var serviceManagerPrivateKey = cachedServiceRequestData.Item2;
                // ######## LET THE OLD CACHE ITEM GO
                cachedServiceRequest.Validater.MarkInvalid();
                // decrypt data
                var decryptedData = Cryptography.CryptographyHelper.RestoreFromTransport((request.Data as List<byte[]>), serviceManagerPrivateKey);
                var restoredData = (new Converters.TypeSerializer<RequestResponseTypes.Login>()).FromByteArray(decryptedData);
                // ######## GET CLIENT PUBLIC KEY AND LOGIN EVIDENCE
                var clientPublicKey = restoredData.XmlPublicKey;
                var evidence = restoredData.LoginEvidence;
                // ######## TEST LOGIN EVIDENCE FOR VALIDITY
                var sessionId = Guid.NewGuid().ToString("N");
                RequestResponseTypes.LoginResponse data;
                if (LoginEvidence.SequenceEqual(evidence))
                {
                    // ######## LOGIN SUCCESSFUL
                    var message = $"Login Successful, Client={targetClientId}, SessionId={sessionId}, GroupKey={request.GroupKey}";
                    data = new RequestResponseTypes.LoginResponse(true, message, ServiceManagerId, sessionId);
                    AuditMessage(Logging.LogEntryType.Information, message);
                    // ######## CREATE SESSION CACHE ITEM
                    var cacheItem = new CachedServiceRequest(sessionId,
                                                             new Cache.ValidateTime<Cache.ICacheProcessorItem>(DateTime.Now.Add(SessionCacheTimeLimit)),
                                                             request.Command,
                                                             targetClientId,
                                                             data,
                                                             (x) =>
                                                             {
                                                                 // #### session has timed-out due to session inactivity
                                                                 var clientId = (x as CachedServiceRequest).ClientId;
                                                                 var dateCreated = (x as CachedServiceRequest).CreatedTimeUtc.LocalDateTime;
                                                                 AuditMessage(Logging.LogEntryType.Information, $"Session Terminated, SessionId={sessionId}, Client={targetClientId}, Runtime={(x as CachedServiceRequest).CachedTime}");
                                                             });
                    _CacheProcessor.Add(cacheItem);
                    // ######## SEND LOGIN RESPONSE
                    // respond with a "ServiceManagerLoginResponse" type encrypted into a List of byte[]
                    // convert data to byte array
                    var dataBytes = (new Converters.TypeSerializer<RequestResponseTypes.LoginResponse>()).ToByteArray(data);
                    // encrypt data
                    var list = Cryptography.CryptographyHelper.PrepareForTransport(dataBytes, clientPublicKey);
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.LoginResponse, list);
                }
                else
                {
                    // ######## LOGIN FAILED
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Login Failed, Invalid Evidence, Evidence Length={evidence.Length}, Client={targetClientId}"));
                }
            }
            else
            {
                // log audit failure ( attempted login, invalid session id )
                SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Login Failed, Invalid GroupKey, Client={targetClientId}"));
            }
        }

        // ################ PROCESS: LOGOUT

        private void ProcessCommand_Logout(ServiceRequest request, string targetClientId)
        {
            if (request.Data == null)
            {
                // no sessionId given
                AuditMessage(Logging.LogEntryType.Error, $"Logout, SessionId is Null, ClientId={targetClientId}");
            }
            else
            {
                var logout = (request.Data as RequestResponseTypes.Logout);
                if (logout != null)
                {
                    // search cache for session id
                    if (_CacheProcessor.Contains(logout.SessionId))
                    {
                        // found it
                        var dude = _CacheProcessor.Find(logout.SessionId) as CachedServiceRequest;
                        var dudeStr = $"ClientId={targetClientId}, SessionId={logout.SessionId}";
                        // check for proper clientId
                        if (targetClientId == dude.ClientId)
                        {
                            // terminate session
                            AuditMessage(Logging.LogEntryType.Information, $"Logout, {dudeStr}, Created={dude.CreatedTimeUtc}, Runtime={dude.CachedTime}");
                            _CacheProcessor.Remove(dude).Process(dude);
                        }
                        else
                        {
                            // invalid client
                            AuditMessage(Logging.LogEntryType.Error, $"Logout, Invalid ClientId, {dudeStr}");
                        }
                    }
                    else
                    {
                        // not found
                        AuditMessage(Logging.LogEntryType.Error, $"Logout, Invalid SessionId, ClientId={targetClientId}, SessionId={logout.SessionId}");
                    }
                }
                else
                {
                    // sessionId not a <string>
                    AuditMessage(Logging.LogEntryType.Error, $"Logout, SessionId is not a string, SessionId Type={request.Data.GetType().FullName}, ClientId={targetClientId}");
                }
            }
        }
        #endregion
        #region PROCESS: SERVICE REQUESTS
        // ################ REDIRECT ALL REMIANING REQUESTS
        private void RedirectServiceRequest(ServiceRequest request, string targetClientId)
        {
            switch (request.Command)
            {
                // ########
                // ######## SERVICE MANAGER
                // ########

                case RequestResponseCommands.Cancel:
                    ProcessCommand_Cancel(request, targetClientId);
                    break;

                case RequestResponseCommands.ServiceManagerDetails:
                    AuditMessage(Logging.LogEntryType.Information, $"Service Manager Details, ServiceManagerId={ServiceManagerId}, ProcessedRequests={ProcessedRequests:N0}, StartTime={this.StartTime}, Runtime={this.RunTime}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.ServiceManagerDetails, new RequestResponseTypes.ServiceManagerDetails(this));
                    break;

                // ########
                // ######## PUT FILE (upload)
                // ########

                case RequestResponseCommands.PutFileRequest:
                    Process_PutFileRequest(request, targetClientId);
                    break;

                case RequestResponseCommands.PutFileInformation:
                    Process_PutFileInformation(request, targetClientId);
                    break;

                case RequestResponseCommands.FileSegment:
                    Process_PutFileSegment(request, targetClientId);
                    break;

                // ########
                // ######## GET FILE (download)
                // ########

                case RequestResponseCommands.GetFileRequest:
                    ProcessCommand_GetFileRequest(request, targetClientId);
                    break;

                case RequestResponseCommands.GetFileReady:
                    ProcessCommand_GetFileReady(request, targetClientId);
                    break;

                case RequestResponseCommands.FileTransferThrottle:
                    // it is assumed that the FileTransferThrottle command is for the GetFile process (download file), because the PutFile process (upload file) is handled elsewhere (by remote clients)
                    ProcessCommand_GetFileTransferThrottle(request, targetClientId);
                    break;

                // ########
                // ######## FOLDER/FILE CONTROL
                // ########

                case RequestResponseCommands.GetFolderInformation:
                    ProcessCommand_GetFolderInformation(request, targetClientId);
                    break;

                case RequestResponseCommands.FileCommand:
                    ProcessCommand_FileCommand(request, targetClientId);
                    break;

                // ########
                // ######## LOGS    
                // ########

                case RequestResponseCommands.GetLogs:
                    ProcessCommand_GetLogs(request, targetClientId);
                    break;

                // ########
                // ######## SERVICES
                // ########

                case RequestResponseCommands.ListServices:
                    ProcessCommand_ListServices(request, targetClientId);
                    break;

                case RequestResponseCommands.GetServiceInformation:
                    ProcessCommand_GetServiceInformation(request, targetClientId);
                    break;

                case RequestResponseCommands.ServiceCommand:
                    ProcessCommand_ServiceCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.StopAllServices:
                    ProcessCommand_StopAllServicesCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.StartAllServices:
                    ProcessCommand_StartAllServicesCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.ScanForComponents:
                    ProcessCommand_ScanForComponentsCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.RestartAllServices:
                    ProcessCommand_RestartAllServicesCommand(request, targetClientId);
                    break;

                // ########
                // ######## PACKAGING and INSTALLATION
                // ########

                case RequestResponseCommands.ListPackages:
                    ProcessCommand_ListPackagesCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.ListInstalledPackages:
                    ProcessCommand_ListInstalledPackagesCommand(request, targetClientId);
                    break;

                // -------- install and uninstall

                case RequestResponseCommands.InstallPackage:
                    ProcessCommand_InstallPackageCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.UnInstallPackage:
                    ProcessCommand_UnInstallPackageCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.UnInstallAllPackages:
                    ProcessCommand_UnInstallAllPackagesCommand(request, targetClientId);
                    break;

                // -------- enable and disable

                case RequestResponseCommands.EnableDisablePackage:
                    ProcessCommand_EnableDisablePackageCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.EnableDisableInstalledPackage:
                    ProcessCommand_EnableDisableInstalledPackageCommand(request, targetClientId);
                    break;

                // -------- installed package information

                case RequestResponseCommands.InstalledPackageDependencyChain:
                    ProcessCommand_InstalledPackageDependencyChainCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.InstalledPackageReferencedBy:
                    ProcessCommand_InstalledPackageReferencedByCommand(request, targetClientId);
                    break;

                // -------- custom configuration 

                case RequestResponseCommands.ReadInstalledCustomConfiguration:
                    ProcessCommand_ReadInstalledCustomConfigurationCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.WriteInstalledCustomConfiguration:
                    ProcessCommand_WriteInstalledCustomConfigurationCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.ReadPackageCustomConfiguration:
                    ProcessCommand_ReadPackageCustomConfigurationCommand(request, targetClientId);
                    break;

                case RequestResponseCommands.WritePackageCustomConfiguration:
                    ProcessCommand_WritePackageCustomConfigurationCommand(request, targetClientId);
                    break;

                // ########
                // ######## COMMUNICATION
                // ########

                case RequestResponseCommands.CommunicationInformation:
                    ProcessCommand_CommunicationInformation(request, targetClientId);
                    break;

                // ########
                // ########
                // ########

                default:
                    break;
            }
        }

        // ################ CANCEL (cancels cached group by GroupKey)

        /// <summary>
        /// Will seek out and remove a cached item by GroupKey
        /// </summary>
        /// <param name="request">The request containing the GroupKey for the process to cancel.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_Cancel(ServiceRequest request, string targetClientId)
        {
            // find cached result
            if (TryFindCachedRequest(request, out CachedServiceRequest result))
            {
                // remove from cache and execute process; this should cause a "canceling" effect with a running process which will Audit their own cancellation messages.
                _CacheProcessor.Remove(result).Process(result);
            }
        }

        // ################ PUT FILE (UPLOAD)

        /// <summary>
        /// Initiates a file transfer to the Service Manager. File Upload.
        /// Must contain the client's Public Key.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void Process_PutFileRequest(ServiceRequest request, string targetClientId)
        {
            // check if file upload is allowed
            if (!AllowFileUpload)
            {
                SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"File Upload Not Allowed"));
                return;
            }
            // get clientPublicKey
            var clientPublicKey = (request.Data as string);
            if (string.IsNullOrWhiteSpace(clientPublicKey))
            {

                // TODO: should this be an ERROR RESPONSE

                throw new Exception($"The 'ServiceManager_PutFileRequest' command must be accompanied by a string representing the client's public encryption key in its Data property.");
            }
            // create public/private keys
            var publicPrivateKeys = Cryptography.CryptographyHelper.GeneratePublicPrivateKeys();
            // cache request 
            // #### STARTING THE PUT FILE PROCESS ####
            var cachedLoginRequest = new CachedServiceRequest(request.GroupKey,
                                                              new Cache.ValidateTime<Cache.ICacheProcessorItem>(DateTime.Now.Add(RequestsResponseTimeout)),
                                                              request.Command,
                                                              targetClientId,
                                                              publicPrivateKeys,
                                                              (item) =>
                                                              {
                                                                  // #### PUT FILE PROCESS HAS COMPLETED
                                                                  var dude = (item as CachedServiceRequest).Data as Tuple<long, string, RequestResponseTypes.PutFileInformation>;
                                                                  // delete working directory if it exists
                                                                  if (System.IO.Directory.Exists(dude.Item2))
                                                                  {

                                                                      // FYI: it is an assumption to assume that because this directory exists an error has occurred.
                                                                      //        it is a pretty good assumption based on the current design, but it is an assumption, and it could be wrong.
                                                                      //        either way, I don't like it
                                                                      //        good candidate for redesign/re-factor.

                                                                      long actualLength = dude.Item1;
                                                                      var bytesUploadedStr = $"Uploaded={((double)actualLength / dude.Item3.FileLength) * 100.0:N1}% {Common.ByteCountHelper.ToString(actualLength)} ({actualLength:N0})";
                                                                      var message = $"File Upload Failed, Upload Stream Abandoned, {dude.Item3}, {bytesUploadedStr}, Elapsed Time={item.CachedTime}, SessionId={request.SessionId}, GroupId={request.GroupKey}";
                                                                      AuditMessage(Logging.LogEntryType.Error, message);
                                                                      // respond to client with an 'file upload error' error
                                                                      SendServiceResponse(targetClientId, request, RequestResponseCommands.PutFileComplete, new RequestResponseTypes.PutFileComplete(false, message, TimeSpan.Zero));

                                                                      // delete directory
                                                                      System.IO.Directory.Delete(dude.Item2, true);
                                                                  }
                                                              });
            // cache request
            _CacheProcessor.Add(cachedLoginRequest);
            // create response
            var response = new RequestResponseTypes.PutFileRequestResponse(publicPrivateKeys.Item1, FileTransferHashAlgorithmType, FileTransferSegmentLength, FileTransferInterstitialDelay);
            // convert data to byte array
            var dataBytes = (new Converters.TypeSerializer<RequestResponseTypes.PutFileRequestResponse>()).ToByteArray(response);
            // encrypt data
            var list = Cryptography.CryptographyHelper.PrepareForTransport(dataBytes, clientPublicKey);
            // log 
            AuditMessage(Logging.LogEntryType.Information, $"File Upload Request, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            // send response
            SendServiceResponse(targetClientId, request, RequestResponseCommands.PutFileRequestResponse, list);
        }

        /// <summary>
        /// Receives information about the file being transfered. 
        /// Encoded using the Public Key from in the PutFileRequest response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void Process_PutFileInformation(ServiceRequest request, string targetClientId)
        {
            // find cached request
            if (TryFindCachedRequestData(request, out CachedServiceRequest cachedServiceRequest, out Tuple<string, string> result))
            {
                // decrypt transport data
                var privateKey = result.Item2;
                var decryptedData = Cryptography.CryptographyHelper.RestoreFromTransport((request.Data as List<byte[]>), privateKey);
                // convert cached request data from a byte[] to a type
                var restoredData = (new Converters.TypeSerializer<RequestResponseTypes.PutFileInformation>()).FromByteArray(decryptedData);
                // create working folder
                var workingFileTransferPath = System.IO.Path.Combine(FileTransferRootPath, "UPLOAD" + Guid.NewGuid().ToString("N"));
                System.IO.Directory.CreateDirectory(workingFileTransferPath);
                // create cached request data (losing the public/private keys)
                cachedServiceRequest.Data = Tuple.Create((long)0, workingFileTransferPath, restoredData);
                // log 
                AuditMessage(Logging.LogEntryType.Information, $"File Upload Started, Filename={restoredData.Filename}, File Length={Common.ByteCountHelper.ToString(restoredData.FileLength)} ({restoredData.FileLength:N0}), SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                // send response
                SendServiceResponse(targetClientId, request, RequestResponseCommands.PutFileReady, null);
            }
        }

        /// <summary>
        /// Receives file segments and finally produces a final file.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void Process_PutFileSegment(ServiceRequest request, string targetClientId)
        {
            if (TryFindCachedRequestData(request, out CachedServiceRequest cachedServiceRequest, out Tuple<long, string, RequestResponseTypes.PutFileInformation> cachedServiceRequestData))
            {
                // extract request data
                var putFile = (request.Data as RequestResponseTypes.FileSegment);
                var accumulatedBytesSent = cachedServiceRequestData.Item1 + putFile.Payload.Length;
                var localFileTransferPath = cachedServiceRequestData.Item2;
                var filename = System.IO.Path.Combine(localFileTransferPath, $"{putFile.Index}");
                var putFileInformation = cachedServiceRequestData.Item3;
                // update cached data
                cachedServiceRequest.Data = Tuple.Create(accumulatedBytesSent, localFileTransferPath, putFileInformation);
                // save file segment
                System.IO.File.WriteAllBytes(filename, putFile.Payload);

                // ################################################################################################

                // ######## check to see if all segments have been received  
                if (accumulatedBytesSent == putFile.TotalBytes)
                {
                    // combine file segments into single file
                    var destFilename = System.IO.Path.Combine(FileTransferRootPath, putFileInformation.Filename);
                    long actualLength = 0;
                    // open destination stream
                    using (var outputStream = System.IO.File.OpenWrite(destFilename))
                    {
                        // iterate through all files in the local file transfer directory in alphabetical order (each filename is the current Ticks, according to the {putFile.Index} when the file was written)
                        foreach (var fileSegmentName in System.IO.Directory.GetFiles(localFileTransferPath).OrderBy(x => x))
                        {
                            // add length of actual files
                            actualLength += (new System.IO.FileInfo(fileSegmentName)).Length;
                            // open source stream
                            using (var fileSegmentStream = System.IO.File.OpenRead(fileSegmentName))
                            {
                                // copy source stream into destination stream
                                fileSegmentStream.CopyTo(outputStream);
                            }
                            // delete source file (file segment)
                            System.IO.File.Delete(fileSegmentName);
                        }
                        // flush and close (redundant, i know. but it gives me emotional security.)
                        outputStream.Flush();
                        outputStream.Close();
                    }

                    // TODO: research this
                    // in theory, this will allow time for the system to release file handles before attempting to delete files and directories.
                    // also, it allows time for the system to release the file handle of the newly created file before reading it.
                    Threading.ThreadingHelper.Sleep(TimeSpan.FromSeconds(1.5));

                    // delete working directory
                    FileStorage.FileStorageHelper.DeleteDirectory(localFileTransferPath);
                    // compute the new file's hash
                    bool match = false;
                    using (var hashAlgorithm = FileTransferHashAlgorithm)
                    {
                        match = Common.ChecksumHelper.CompareFile(destFilename, putFileInformation.Hash, hashAlgorithm);
                    }
                    // send a success or error message
                    if (match)
                    {
                        // ######## success
                        var dude = new RequestResponseTypes.PutFileComplete(true, $"File Upload Successful, {putFileInformation}, Elapsed Time={cachedServiceRequest.CachedTime}, SessionId={request.SessionId}, GroupKey={request.GroupKey}", cachedServiceRequest.CachedTime);
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.PutFileComplete, dude);
                        // log 
                        AuditMessage(Logging.LogEntryType.Information, dude.Description);
                    }
                    else
                    {
                        // ######## failure
                        var bytesUploadedStr = $"Uploaded={((double)actualLength / putFileInformation.FileLength) * 100.0:N1}% {Common.ByteCountHelper.ToString(actualLength)} ({actualLength:N0})";
                        var dude = new RequestResponseTypes.PutFileComplete(false, $"File Upload Failed, File Integrity Compromised, {putFileInformation}, {bytesUploadedStr}, Elapsed Time={cachedServiceRequest.CachedTime}, SessionId={request.SessionId}, GroupKey={request.GroupKey}", cachedServiceRequest.CachedTime);
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.PutFileComplete, dude);
                        // log 
                        AuditMessage(Logging.LogEntryType.Error, dude.Description);
                        // delete destination 
                        FileStorage.FileStorageHelper.DeleteFile(destFilename);
                    }
                    // let cached item go
                    cachedServiceRequest.Validater.MarkInvalid();
                }
            }
        }

        // ################  GET FILE (DOWNLOAD)

        /// <summary>
        /// Initiates a file transfer from the Service Manager. File Download. 
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetFileRequest(ServiceRequest request, string targetClientId)
        {
            // get request data
            if (request.Data is RequestResponseTypes.GetFileRequest requestData)
            {
                // check if file upload is allowed
                if (!AllowFileDownload)
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"File Download Not Allowed"));
                    return;
                }
                // check if file exists
                if (!System.IO.File.Exists(requestData.Filename))
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"File Not Found"));
                    return;
                }
                // send GetFileRequestReqponse response
                var filename = requestData.Filename;
                var fileLength = (new System.IO.FileInfo(filename)).Length;
                byte[] hash;
                using (var hashAlgorithm = FileTransferHashAlgorithm)
                {
                    hash = Common.ChecksumHelper.ComputeFile(filename, hashAlgorithm);
                }
                var response = new RequestResponseTypes.GetFileRequestResponse(filename, fileLength, hash, FileTransferHashAlgorithmType, FileTransferSegmentLength, FileTransferInterstitialDelay);

                // #### STARTING THE GET FILE PROCESS ####
                var cachedLoginRequest = new CachedServiceRequest(request.GroupKey,
                                                                  new Cache.ValidateTime<Cache.ICacheProcessorItem>(DateTime.Now.Add(RequestsResponseTimeout)),
                                                                  request.Command,
                                                                  targetClientId,
                                                                  response,
                                                                  (item) => { });
                // cache request
                _CacheProcessor.Add(cachedLoginRequest);
                // log 
                AuditMessage(Logging.LogEntryType.Information, $"File Download Request, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                // convert data to byte array
                var dataBytes = (new Converters.TypeSerializer<RequestResponseTypes.GetFileRequestResponse>()).ToByteArray(response);
                // encrypt data
                var list = Cryptography.CryptographyHelper.PrepareForTransport(dataBytes, requestData.PublicKey);
                // send response
                SendServiceResponse(targetClientId, request, RequestResponseCommands.GetFileRequestResponse, list);
            }
        }

        /// <summary>
        /// Indicates that the client is ready to receive File Segments to the Service Manager.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetFileReady(ServiceRequest request, string targetClientId)
        {
            if (TryFindCachedRequestData(request, out CachedServiceRequest cachedServiceRequest, out RequestResponseTypes.GetFileRequestResponse cachedRequestData))
            {
                // #### loop: send the file to the client in chunks
                // create byte[] buffer
                var bytes = new byte[FileTransferSegmentLength];
                // get file's total length
                var totalBytes = new System.IO.FileInfo(cachedRequestData.Filename).Length;
                long serviceChunkTransferChunkLength = FileTransferSegmentLength;
                TimeSpan serviceChunkTransferInterstitialDelay = FileTransferInterstitialDelay;
                var getFileStartTimeUtc = DateTimeOffset.UtcNow;
                long lastBytesSent = 0;
                // open source file stream
                var message1 = $"File Download Started, Source Filename={cachedRequestData.Filename}, File Size={Common.ByteCountHelper.ToString(cachedRequestData.FileLength)} ({cachedRequestData.FileLength:N0}), File Segment Length={Common.ByteCountHelper.ToString(FileTransferSegmentLength)} ({FileTransferSegmentLength:N0}), Interstitial Delay={FileTransferInterstitialDelay}, SessionId={request.SessionId}, GroupKey={request.GroupKey}";
                AuditMessage(Logging.LogEntryType.Information, message1);
                // send file segments
                using (var fs = System.IO.File.OpenRead(cachedRequestData.Filename))
                {
                    // ***************************   LOOP   ***************************
                    // ****************************************************************

                    // ################################################################
                    // Command = FileSegment
                    // Data = FileSegment
                    // ################################################################
                    // Response = CANCEL
                    //          = FileTransferThrottle
                    // ################################################################

                    // read the source file in chunks
                    for (long i = 0; i < totalBytes; i += serviceChunkTransferChunkLength)
                    {
                        // #### check for THROTTLE and CANCEL commands
                        if (TryFindCachedRequestData(request, out CachedServiceRequest loopCachedServiceRequest, out RequestResponseTypes.GetFileRequestResponse loopCachedRequestData))
                        {
                            // NOT CANCELED, get new chunk and interstitial delay values (possibly throttled)
                            serviceChunkTransferChunkLength = loopCachedRequestData.ChuckLength;
                            serviceChunkTransferInterstitialDelay = loopCachedRequestData.InterstitialDelay;
                        }
                        else
                        {
                            // CANCELED, terminate download (cached item not found)
                            var percentageStr = (i == 0) ? $"0.0%" : $"{((double)i / totalBytes) * 100.0:N1}%"; // no NANs
                            var message2 = $"File Download Canceled, Filename={cachedRequestData.Filename}, File Size={Common.ByteCountHelper.ToString(totalBytes)} ({totalBytes:N0}), Downloaded={percentageStr} {Common.ByteCountHelper.ToString(i)} ({i:N0}), Elapsed Time={DateTimeOffset.UtcNow - getFileStartTimeUtc}, SessionID={request.SessionId}, GroupKey={request.GroupKey}";
                            AuditMessage(Logging.LogEntryType.Error, message2);
                            return;
                        }
                        // test if the BUFFER needs to be resized
                        if (bytes.Length != serviceChunkTransferChunkLength)
                        {
                            Array.Resize(ref bytes, (int)serviceChunkTransferChunkLength);
                        }
                        // log debug message
                        AuditMessage(Logging.LogEntryType.Debug, $"File Download, {cachedRequestData}, Index={i}, Total Bytes={totalBytes:N0}");
                        // read file segment
                        lastBytesSent = fs.Read(bytes, 0, (int)serviceChunkTransferChunkLength);
                        // test if the READ buffer needs to be resized
                        if (lastBytesSent != serviceChunkTransferChunkLength)
                        {
                            Array.Resize(ref bytes, (int)lastBytesSent);
                        }
                        // create FileSegment object
                        var sentBytes = i + lastBytesSent;
                        var dude = new RequestResponseTypes.FileSegment(DateTimeOffset.UtcNow.Ticks, totalBytes, sentBytes, bytes);

                        // ################################################################
                        // Command = FileSegment
                        // Data = FileSegment
                        // ################################################################
                        // Response = N/A
                        // ################################################################

                        // send FileSegment to client
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.FileSegment, dude);
                        // keep session alive (this is trying to keep the session alive like the upload does automatically; it could fail if the interstitial delay is longer than the session timeout. Keeping the session alive is not required to download file segments, but it is expected.)
                        if (_CacheProcessor.TryFind(request.SessionId, out Cache.ICacheProcessorItem cacheProcessorItem))
                        {
                            cacheProcessorItem.Validater.Reset();
                        }
                        // wait 
                        Threading.ThreadingHelper.Sleep(serviceChunkTransferInterstitialDelay);
                    }
                    // final log (the service manager does not know if the download was successful or not)
                    AuditMessage(Logging.LogEntryType.Information, $"File Download Completed, Filename={cachedRequestData.Filename}, File Size={Common.ByteCountHelper.ToString(totalBytes)} ({totalBytes:N0}), Elapsed Time={DateTimeOffset.UtcNow - getFileStartTimeUtc}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                }
            }
        }

        /// <summary>
        /// Will Throttle a running File Download process.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetFileTransferThrottle(ServiceRequest request, string targetClientId)
        {
            if (TryFindCachedRequestData(request, out CachedServiceRequest cachedServiceRequest, out RequestResponseTypes.GetFileRequestResponse cachedServiceRequestData))
            {
                // get request
                if (request.Data is RequestResponseTypes.FileTransferThrottle requestData)
                {
                    // update file download 
                    AuditMessage(Logging.LogEntryType.Information, $"File Download Throttled, {requestData}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    cachedServiceRequestData.ChuckLength = requestData.SegmentLength;
                    cachedServiceRequestData.InterstitialDelay = requestData.SegmentTransferInterstitialDelay;
                }
            }
        }

        // ################ FOLDER/FILE CONTROL

        /// <summary>
        /// Will return information about a specific folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetFolderInformation(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.GetFolderInformationRequest requestData)
            {
                // check if folder access is allowed
                if (!ComponentManager.FolderAccessItems.IsFolderValid(requestData.FolderName))
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Folder Access Not Allowed. Folder={requestData.FolderName}"));
                    return;
                }
                // check if folder exists
                if (!System.IO.Directory.Exists(requestData.FolderName))
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Folder Not Found."));
                    return;
                }
                // #### create folder/file information
                // get folders
                var folders = new List<string>();
                foreach (var folder in System.IO.Directory.GetDirectories(requestData.FolderName, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    folders.Add(folder);
                }
                // get files
                var files = new List<RequestResponseTypes.FileInformation>();
                foreach (var file in System.IO.Directory.GetFiles(requestData.FolderName, "*", System.IO.SearchOption.TopDirectoryOnly))
                {
                    var fi = new System.IO.FileInfo(file);
                    files.Add(new RequestResponseTypes.FileInformation(file, fi.Length, fi.CreationTimeUtc, fi.LastWriteTimeUtc));
                }
                //
                var dude = new RequestResponseTypes.GetFolderInformationResponse(requestData.FolderName, folders.ToArray(), files.ToArray());
                // return folder/file information 
                AuditMessage(Logging.LogEntryType.Information, $"Get Folder Information, Folder={requestData.FolderName}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                SendServiceResponse(targetClientId, request, RequestResponseCommands.GetFolderInformationResponse, dude);
            }
        }

        /// <summary>
        /// Will execute a file command (copy, move or delete)
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_FileCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.FileCommand requestData)
            {
                // #### check for folder access compliance 
                if (!ComponentManager.FolderAccessItems.IsFolderValid(requestData.SourceFilename))
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Source File Access Not Allowed. {requestData}"));
                    return;
                }
                if (!string.IsNullOrWhiteSpace(requestData.DestinationFilename))
                {
                    if (!ComponentManager.FolderAccessItems.IsFolderValid(requestData.DestinationFilename))
                    {
                        SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Destination File Access Not Allowed. {requestData}"));
                        return;
                    }
                }
                // #### check for source file existence
                if (!System.IO.File.Exists(requestData.SourceFilename))
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Source File Not Found. {requestData}"));
                    return;
                }
                // #### perform file action
                if (!string.IsNullOrWhiteSpace(requestData.DestinationFilename))
                {
                    // #### COPY ####
                    // copy source to destination
                    try
                    {
                        System.IO.File.Copy(requestData.SourceFilename, requestData.DestinationFilename, true);
                    }
                    catch (Exception ex)
                    {
                        SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"System Exception. {Common.StringHelper.FormatStringIntoSingleLine(ex.Message)}, {requestData}"));
                        return;
                    }
                }
                // test for delete source
                if (requestData.DeleteSourceFile)
                {
                    // #### DELETE SOURCE ####
                    if (!FileStorage.FileStorageHelper.DeleteFile(requestData.SourceFilename))
                    {
                        SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"System Exception. Could not delete file '{requestData.SourceFilename}'., {requestData}"));
                        return;
                    }
                }
                // #### return results
                AuditMessage(Logging.LogEntryType.Information, $"File Command, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                SendServiceResponse(targetClientId, request, RequestResponseCommands.FileCommandResponse, requestData);
            }
        }

        // ################ LOGS

        /// <summary>
        /// Will retrieve logs based on the criteria supplied by the client.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetLogs(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.GetLogs requestData)
            {
                // check if log downloading is allowed
                if (!AllowLogsDownload)
                {
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Logs Downloading Not Allowed"));
                    return;
                }
                AuditMessage(Logging.LogEntryType.Information, $"GetLogs Started, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                // get the logs Enumerator
                var logs = ComponentManager.LogController.Read(x =>
                {
                    if (x == null)
                    {
                        return false;
                    }
                    // EntryTypes
                    var entryTypesOK = (requestData.EntryTypes.Count() == 0);
                    if (requestData.EntryTypes.Count() > 0)
                    {
                        entryTypesOK = requestData.EntryTypes.Contains(x.EntryType);
                    }
                    // Sources
                    var sourcesOK = (requestData.Sources.Count() == 0);
                    if (requestData.Sources.Count() > 0)
                    {
                        sourcesOK = requestData.Sources.Contains(x.SourceId);
                    }
                    // EventIds
                    var eventIdsOK = (requestData.EventIds.Count() == 0);
                    if (requestData.EventIds.Count() > 0)
                    {
                        eventIdsOK = requestData.EventIds.Contains(x.EventId);
                    }
                    // Categories
                    var categoriesOK = (requestData.Categories.Count() == 0);
                    if (requestData.Categories.Count() > 0)
                    {
                        categoriesOK = requestData.Categories.Contains(x.Category);
                    }
                    //
                    return ((entryTypesOK) && (sourcesOK) && (eventIdsOK) && (categoriesOK) && ((x.Timestamp >= requestData.FromDate) && (x.Timestamp <= requestData.ToDate)));
                });
                // add cached service response (for cancellation purposes)
                var cachedServiceRequest = new CachedServiceRequest(request.GroupKey,
                                                                    new Cache.ValidateTime<Cache.ICacheProcessorItem>(DateTime.Now.Add(RequestsResponseTimeout)),
                                                                    request.Command,
                                                                    targetClientId,
                                                                    null,
                                                                    (item) => { });
                // cache request (so it can be canceled)
                _CacheProcessor.Add(cachedServiceRequest);
                // loop
                bool wasCancelled = false;
                int logsSent = 0;
                DateTimeOffset lastDate = DateTimeOffset.MinValue;
                var list = GetLogChunks(logs, LogsDownloadChunkCount);
                foreach (var item in list)
                {
                    logsSent += item.Count;
                    lastDate = item.LastDate;
                    // send log chunk to client
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.GetLogsResponse, new RequestResponseTypes.GetLogsResponse(item));
                    // check for cancellation
                    if (!TryFindCachedRequest(request, out CachedServiceRequest loopCachedServiceRequest))
                    {
                        wasCancelled = true;
                        break;
                    }
                    // keep session alive (this is trying to keep the session alive; it could fail if the interstitial delay is longer than the session timeout. Keeping the session alive is not required to download logs, but it is expected.)
                    if (_CacheProcessor.TryFind(request.SessionId, out Cache.ICacheProcessorItem cacheProcessorItem))
                    {
                        cacheProcessorItem.Validater.Reset();
                    }
                    // wait 
                    Threading.ThreadingHelper.Sleep(LogsDownloadInterstitialDelay);
                }
                // send logs download complete message to client
                SendServiceResponse(targetClientId, request, RequestResponseCommands.GetLogsComplete, new RequestResponseTypes.GetLogsComplete(wasCancelled, logsSent, lastDate));
                // check for cancellation
                AuditMessage(Logging.LogEntryType.Information, $"GetLogs {(wasCancelled ? "Canceled" : "Completed")}, Logs Sent={logsSent}, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            }

            // ******** PRIVATE METHODS ********

            IEnumerable<Logging.Logs> GetLogChunks(IEnumerable<Logging.ILogEntry> source,
                                                   int chunkSize)
            {
                while (source.Any())
                {
                    yield return new Logging.Logs(source.Take(chunkSize));
                    source = source.Skip(chunkSize);
                }
            }
        }

        // ################ SERVICES

        /// <summary>
        /// Will return information about the <see cref="IService"/>s currently managed by the <see cref="IServiceManager"/>.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ListServices(ServiceRequest request, string targetClientId)
        {
            var services = new List<RequestResponseTypes.ServiceInformation>();
            // gather extensions
            foreach (var item in ComponentManager.ComponentController.ExtensionComponents)
            {
                services.Add(new RequestResponseTypes.ServiceInformation(item, true));
            }
            // gather plugins
            foreach (var item in ComponentManager.ComponentController.PluginComponents)
            {
                services.Add(new RequestResponseTypes.ServiceInformation(item, false));
            }
            // return services information 
            AuditMessage(Logging.LogEntryType.Information, $"List Services, Services={services.Count}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            SendServiceResponse(targetClientId, request, RequestResponseCommands.ListServicesResponse, new RequestResponseTypes.ListServicesResponse(ServiceManagerId, services));
        }

        /// <summary>
        /// Will return information about a single service.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_GetServiceInformation(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.GetServiceInformation requestData)
            {
                var found = FindComponent(requestData.ServiceId, out bool isExtension);
                //
                if (found != null)
                {
                    // return service information 
                    AuditMessage(Logging.LogEntryType.Information, $"Get Service Information, ServiceId={found.Id}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.GetServiceInformationResponse, new RequestResponseTypes.ServiceInformation(found, isExtension));
                }
                else
                {
                    // error: service not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Service not found, Id={requestData.ServiceId}"));
                }
            }
        }

        /// <summary>
        /// Will attempt to stop all running services.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_StopAllServicesCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.StopAllServices requestData)
            {
                var beforeExtensionCount = ComponentManager.ComponentController.ExtensionComponents.Count();
                var beforePluginCount = ComponentManager.ComponentController.PluginComponents.Count();
                if (!requestData.IncludeExtensions && !requestData.IncludePlugins)
                {
                    // nothing to do
                    var noResponse = new RequestResponseTypes.StopAllServicesResponse(beforeExtensionCount, beforePluginCount, beforeExtensionCount, beforePluginCount);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Stop All Services Completed: No extensions or plugins stopped, {noResponse}");
                    // respond
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.StopAllServicesResponse, noResponse);
                }
                else
                {
                    // stop extensions
                    if (requestData.IncludeExtensions)
                    {
                        ComponentManager.ComponentController.StopAllExtensions(ServicesRestartDelay);
                    }
                    // stop plugins
                    if (requestData.IncludePlugins)
                    {
                        ComponentManager.ComponentController.StopAllPlugins(ServicesRestartDelay, true);
                    }
                    // 
                    var afterExtensionCount = ComponentManager.ComponentController.ExtensionComponents.Count();
                    var afterPluginCount = ComponentManager.ComponentController.PluginComponents.Count();
                    //
                    var response = new RequestResponseTypes.StopAllServicesResponse(beforeExtensionCount, beforePluginCount, afterExtensionCount, afterPluginCount);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Stop All Services Completed: {response}");
                    // respond
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.StopAllServicesResponse, response);
                }
            }
        }

        /// <summary>
        /// Will attempt to start all stopped services.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_StartAllServicesCommand(ServiceRequest request, string targetClientId)
        {
            var totalExtensions = ComponentManager.ComponentController.ExtensionComponents.Count();
            var totalPlugins = ComponentManager.ComponentController.PluginComponents.Count();
            //
            var beforeExtensionCount = (from x in ComponentManager.ComponentController.ExtensionComponents where x.IsRunning select x).Count();
            var beforePluginCount = (from x in ComponentManager.ComponentController.PluginComponents where x.IsRunning select x).Count();
            //
            AuditMessage(Logging.LogEntryType.Information, $"Start All Services Started: Extensions: Total={totalExtensions}, Running={beforeExtensionCount}, Plugins: Total={totalPlugins}, Running={beforePluginCount}");
            //
            ComponentManager.ComponentController.StartAllStoppedComponents(ServicesRestartDelay);
            //
            var afterExtensionCount = (from x in ComponentManager.ComponentController.ExtensionComponents where x.IsRunning select x).Count();
            var afterPluginCount = (from x in ComponentManager.ComponentController.PluginComponents where x.IsRunning select x).Count();
            //
            var response = new RequestResponseTypes.StartAllServicesResponse(totalExtensions, totalPlugins, beforeExtensionCount, beforePluginCount, afterExtensionCount, afterPluginCount);
            // log it
            AuditMessage(Logging.LogEntryType.Information, $"Start All Services Completed: {response}");
            // respond
            SendServiceResponse(targetClientId, request, RequestResponseCommands.StartAllServicesResponse, response);
        }

        /// <summary>
        /// Scans for new extensions and plugins and starts them.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ScanForComponentsCommand(ServiceRequest request, string targetClientId)
        {
            var beforeExtensionCount = ComponentManager.ComponentController.ExtensionComponents.Count();
            var beforePluginCount = ComponentManager.ComponentController.PluginComponents.Count();
            //
            AuditMessage(Logging.LogEntryType.Information, $"Starting Component Scan, Extension Count={beforeExtensionCount}, Plugin Count={beforePluginCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            ComponentManager.ComponentController.Rescan(ServicesRestartDelay);
            //
            var afterExtensionCount = ComponentManager.ComponentController.ExtensionComponents.Count();
            var afterPluginCount = ComponentManager.ComponentController.PluginComponents.Count();
            AuditMessage(Logging.LogEntryType.Information, $"Completed Component Scan, Extension Count={afterExtensionCount}, Plugin Count={afterPluginCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            //
            SendServiceResponse(targetClientId, request, RequestResponseCommands.ScanForComponentsResponse, new RequestResponseTypes.ScanForComponentsResponse(beforeExtensionCount, beforePluginCount, afterExtensionCount, afterPluginCount));
        }

        /// <summary>
        /// Will attempt to stop and start all extensions and plugins.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_RestartAllServicesCommand(ServiceRequest request, string targetClientId)
        {
            var totalExtensions = ComponentManager.ComponentController.ExtensionComponents.Count();
            var totalPlugins = ComponentManager.ComponentController.PluginComponents.Count();
            //
            var beforeExtensionCount = (from x in ComponentManager.ComponentController.ExtensionComponents where x.IsRunning select x).Count();
            var beforePluginCount = (from x in ComponentManager.ComponentController.PluginComponents where x.IsRunning select x).Count();
            //
            AuditMessage(Logging.LogEntryType.Information, $"Restarting All Services, Extensions: Total={totalExtensions}, Running={beforeExtensionCount}, Plugins: Total={totalPlugins}, Running={beforePluginCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            ComponentManager.ComponentController.Restart(ServicesRestartDelay);
            //
            var afterExtensionCount = (from x in ComponentManager.ComponentController.ExtensionComponents where x.IsRunning select x).Count();
            var afterPluginCount = (from x in ComponentManager.ComponentController.PluginComponents where x.IsRunning select x).Count();
            //
            AuditMessage(Logging.LogEntryType.Information, $"Completed Restarting All Services, Extensions: Total={totalExtensions}, Running={afterExtensionCount}, Plugins: Total={totalPlugins}, Running={afterPluginCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            //
            SendServiceResponse(targetClientId, request, RequestResponseCommands.RestartAllServicesResponse, new RequestResponseTypes.RestartAllServicesResponse(totalExtensions, totalPlugins, beforeExtensionCount, beforePluginCount, afterExtensionCount, afterPluginCount));
        }

        /// <summary>
        /// Will execute a command against the desired service.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ServiceCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.ServiceCommand requestData)
            {
                var found = FindComponent(requestData.ServiceId, out bool isExtension);
                //
                if (found != null)
                {
                    // perform command 
                    AuditMessage(Logging.LogEntryType.Information, $"Service Command (executing), Command={requestData.Command}, ServiceId={found.Id}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    if (ExecuteServicesCommand(found, request, requestData, targetClientId, out string reason))
                    {
                        // service command successful
                        AuditMessage(Logging.LogEntryType.Information, $"Service Command Successful, Command={requestData.Command}, ServiceId={found.Id}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.ServiceCommandResponse, new RequestResponseTypes.ServiceCommandResponse(true, reason, requestData, null));
                    }
                    else
                    {
                        // service command failed
                        AuditMessage(Logging.LogEntryType.Information, $"Service Command Failed, Reason={reason}, Command={requestData.Command}, ServiceId={found.Id}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.ServiceCommandResponse, new RequestResponseTypes.ServiceCommandResponse(false, reason, requestData, null));
                    }
                }
                else
                {
                    // error: service not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Service not found, Command={requestData.Command}, Id={requestData.ServiceId}"));
                }
            }
        }

        /// <summary>
        /// Executes a command against a service.
        /// </summary>
        /// <param name="service">The service to execute the command against.</param>
        /// <param name="request">The request.</param>
        /// <param name="requestData">The command to execute.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        /// <param name="reason">The reason for the success or failure of the command.</param>
        /// <returns>Whether the command was executed successfully or not. <b>True</b> indicates success; otherwise, <b>false</b> indicates the command failed.</returns>
        private bool ExecuteServicesCommand(ComponentManagement.ComponentFactory service,
                                            ServiceRequest request,
                                            RequestResponseTypes.ServiceCommand requestData,
                                            string targetClientId,
                                            out string reason)
        {
            bool results = false;
            reason = "OK";
            //
            switch (requestData.Command)
            {
                case ServicesCommands.Stop:
                    if (service.IsRunning)
                    {
                        service.Stop();
                    }
                    results = true;
                    break;

                case ServicesCommands.Start:
                    if (!service.IsRunning)
                    {
                        service.Start();
                    }
                    results = true;
                    break;

                case ServicesCommands.Restart:
                    if (service.IsRunning)
                    {
                        service.Stop();
                        Threading.ThreadingHelper.Sleep(ServicesRestartDelay);
                    }
                    if (!service.IsRunning)
                    {
                        service.Start();
                    }
                    results = true;
                    break;

                case ServicesCommands.GetLogs:
                    if ((requestData.Data != null) && (requestData.Data is RequestResponseTypes.GetLogs))
                    {
                        // this ensures that the correct logs are retrieved. Relies on the (internal set) of the [ dodSON.Core.ServiceManagement.RequestResponseTypes.GetLogsCriteria ]
                        (requestData.Data as RequestResponseTypes.GetLogs).Sources = new string[] { service.Id };
                        ProcessCommand_GetLogs(new ServiceRequest(request.SessionId, request.GroupKey, RequestResponseCommands.GetLogs, request.ServiceManagerId, requestData.Data), targetClientId);
                        results = true;
                    }
                    else
                    {
                        reason = "Invalid request data. Should be type of [ dodSON.Core.ServiceManagement.RequestResponseTypes.GetLogsCriteria ]";
                        results = false;
                    }
                    break;

                default:
                    break;
            }
            //
            return results;
        }

        // ################ PACKAGING AND INSTALLATION

        /// <summary>
        /// Will return information about the packages in the packages folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ListPackagesCommand(ServiceRequest request, string targetClientId)
        {
            // gather packages information
            var dude = new RequestResponseTypes.ListPackagesResponse(ComponentManager.PackageProvider.NonDependencyPackages,
                                                                     ComponentManager.PackageProvider.DependencyPackages);

            // return packages information 
            AuditMessage(Logging.LogEntryType.Information, $"List Packages, {dude}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            SendServiceResponse(targetClientId, request, RequestResponseCommands.ListPackagesResponse, dude);
        }

        /// <summary>
        /// Will return information about the installed packages in the packages folder.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ListInstalledPackagesCommand(ServiceRequest request, string targetClientId)
        {
            var dude = new RequestResponseTypes.ListInstalledPackagesResponse(ComponentManager.Installer.InstalledNonDependencyPackages,
                                                                              ComponentManager.Installer.InstalledDependencyPackages,
                                                                              ComponentManager.Installer.InstalledDisabledPackages,
                                                                              ComponentManager.Installer.InstalledOrphanedDependencyPackages,
                                                                              ComponentManager.Installer.InstalledPackagesWithMissingDependencies);

            // return packages information
            AuditMessage(Logging.LogEntryType.Information, $"List Installed Packages, {dude}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            SendServiceResponse(targetClientId, request, RequestResponseCommands.ListInstalledPackagesResponse, dude);
        }

        /// <summary>
        /// Attempts to install the specified package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_InstallPackageCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.InstallPackage requestData)
            {
                // find package
                var package = ComponentManager.PackageProvider.FindPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    AuditMessage(Logging.LogEntryType.Information, $"Install Package Started: {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    var logs = ComponentManager.Installer.Install(package, ComponentManager.PackageProvider, ComponentManager.InstallationSettings);
                    var dude = new Logging.Logs
                    {
                        logs,
                        { Logging.LogEntryType.Information, LogSourceId, $"Install Package Completed: {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}" }
                    };
                    AuditMessageLogs(dude);
                    //
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.InstallPackageResponse, new RequestResponseTypes.InstallPackageResponse(true, "Package successfully Installed.", requestData.Name, requestData.Version));
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Attempts to uninstall the specified package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_UnInstallPackageCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.UnInstallPackage requestData)
            {
                // find installed package
                var installedPackage = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (installedPackage != null)
                {
                    // installed package found
                    AuditMessage(Logging.LogEntryType.Information, $"Uninstall Package Started: {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    //
                    var logs = ComponentManager.Installer.Uninstall(installedPackage, ComponentManager.InstallationSettings.RemoveOrphanedPackages);
                    var dude = new Logging.Logs
                    {
                        logs,
                        { Logging.LogEntryType.Information, LogSourceId, $"Uninstall Package Completed: {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}" }
                    };
                    AuditMessageLogs(dude);
                    //
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.UnInstallPackageResponse, new RequestResponseTypes.UnInstallPackageResponse(true, "Package successfully uninstalled.", requestData.Name, requestData.Version));
                }
                else
                {
                    // installed package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will uninstall all installed packages.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_UnInstallAllPackagesCommand(ServiceRequest request, string targetClientId)
        {
            // log start
            var beforeCount = ComponentManager.Installer.InstalledPackages.Count();
            AuditMessage(Logging.LogEntryType.Information, $"Uninstall All Packages Started, Installed Packages={beforeCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            // uninstall all packages and log it
            AuditMessageLogs(ComponentManager.Installer.UninstallAll());
            // log completion
            var afterCount = ComponentManager.Installer.InstalledPackages.Count();
            AuditMessage(Logging.LogEntryType.Information, $"Uninstall All Packages Completed, Uninstalled Packages={afterCount}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
            // send response
            SendServiceResponse(targetClientId, request, RequestResponseCommands.UnInstallAllPackagesResponse, new RequestResponseTypes.UnInstallAllPackagesResponse(beforeCount));
        }

        /// <summary>
        /// Will set the package's IsEnabled flag to the specified value.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_EnableDisablePackageCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.EnableDisablePackage requestData)
            {
                // find package
                var package = ComponentManager.PackageProvider.FindPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // only change if change is required
                    if (requestData.IsEnabled != package.PackageConfiguration.IsEnabled)
                    {
                        // change the isEnabled flag
                        package.PackageConfiguration.IsEnabled = requestData.IsEnabled;
                        // write package's configuration
                        ComponentManager.PackageProvider.WriteConfigurationFile(package, package.PackageConfiguration);
                        // log it
                        var messageFragment = (requestData.IsEnabled) ? "Enabled" : "Disabled";
                        AuditMessage(Logging.LogEntryType.Information, $"Package {messageFragment}, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.EnableDisablePackageResponse, new RequestResponseTypes.EnableDisablePackageResponse($"Package successfully {messageFragment}.", requestData));
                    }
                    else
                    {
                        // log it
                        var messageFragment = (requestData.IsEnabled) ? "Enabled" : "Disabled";
                        AuditMessage(Logging.LogEntryType.Information, $"Package {messageFragment}, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.EnableDisablePackageResponse, new RequestResponseTypes.EnableDisablePackageResponse($"Package successfully {messageFragment}.", requestData));
                    }
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will set the installed package's IsEnabled flag to the specified value.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_EnableDisableInstalledPackageCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.EnableDisableInstalledPackage requestData)
            {
                // find package
                var package = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // load installed package configuration
                    var configuration = ComponentManager.Installer.ReadConfigurationFile(package);
                    // only change if change is required
                    if (requestData.IsEnabled != configuration.IsEnabled)
                    {
                        // change the isEnabled flag
                        configuration.IsEnabled = requestData.IsEnabled;
                        // write package's configuration
                        ComponentManager.Installer.WriteConfigurationFile(package, configuration);
                        // log it
                        var messageFragment = (requestData.IsEnabled) ? "Enabled" : "Disabled";
                        AuditMessage(Logging.LogEntryType.Information, $"Installed Package {messageFragment}, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.EnableDisableInstalledPackageResponse, new RequestResponseTypes.EnableDisableInstalledPackageResponse($"Installed Package successfully {messageFragment}.", requestData));
                    }
                    else
                    {
                        // log it
                        var messageFragment = (requestData.IsEnabled) ? "Enabled" : "Disabled";
                        AuditMessage(Logging.LogEntryType.Information, $"Installed Package {messageFragment}, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                        // send response
                        SendServiceResponse(targetClientId, request, RequestResponseCommands.EnableDisableInstalledPackageResponse, new RequestResponseTypes.EnableDisableInstalledPackageResponse($"Installed Package successfully {messageFragment}.", requestData));
                    }
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Installed package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will return the dependency chain for the specified installed package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_InstalledPackageDependencyChainCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.InstalledPackageDependencyChain requestData)
            {
                // find package
                var package = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // get dependency chain for specified installed package
                    var chain = ComponentManager.Installer.DependencyChain(package);
                    AuditMessage(Logging.LogEntryType.Information, $"Dependency Chain for Installed Package, {requestData}, Chain Count={chain.Count()}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.InstalledPackageDependencyChainResponse, new RequestResponseTypes.InstalledPackageDependencyChainResponse(requestData, chain));
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Installed package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will return a list of installed packages which references the specified installed package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_InstalledPackageReferencedByCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.InstalledPackageReferencedBy requestData)
            {
                // find package
                var package = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // get list of referencing installed packages for specified installed package
                    var chain = ComponentManager.Installer.InstalledPackagesReferencingPackage(package);
                    AuditMessage(Logging.LogEntryType.Information, $"Installed Package Referenced By, {requestData}, Reference Count={chain.Count()}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.InstalledPackageReferencedByResponse, new RequestResponseTypes.InstalledPackageReferencedByResponse(requestData, chain));
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Installed package not found: {requestData}"));
                }
            }
        }

        // -------- custom configuration 

        /// <summary>
        /// Reads the custom configuration for the specified installed package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ReadInstalledCustomConfigurationCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.ReadInstalledCustomConfiguration requestData)
            {
                // find package
                var package = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // get custom configuration for specified installed package
                    var customConfiguration = ReadInstalledCustomConfigurationFile(package.InstallPath, ComponentManager.ComponentController.CustomConfigurationFilename, ComponentManager.ComponentController.CustomConfigurationSerializer);
                    //
                    var response = new RequestResponseTypes.ReadInstalledCustomConfigurationResponse(requestData, customConfiguration);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Read Installed Custom Configuration, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.ReadInstalledCustomConfigurationResponse, response);
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Installed package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will attempt to read and deserialize the specified custom configuration file. 
        /// Will return <b>null</b> if the file is not found or cannot be deserialized.
        /// </summary>
        /// <param name="installPath">The directory path to search.</param>
        /// <param name="customConfigurationFilename">The name of the file to search for.</param>
        /// <param name="customConfigurationSerializer">The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.</param>
        /// <returns>A <see cref="Configuration.IConfigurationGroup"/> representing the deserialized custom configuration, or <b>null</b> if the file is not found or cannot be deserialized.</returns>
        private Configuration.IConfigurationGroup ReadInstalledCustomConfigurationFile(string installPath,
                                                                                       string customConfigurationFilename,
                                                                                       Configuration.IConfigurationSerializer<StringBuilder> customConfigurationSerializer)
        {
            // check for path
            if (!System.IO.Directory.Exists(installPath))
            {
                return null;
            }
            // create full filename
            var fileName = System.IO.Path.Combine(installPath, customConfigurationFilename);
            // check for file
            if (!System.IO.File.Exists(fileName))
            {
                return null;
            }
            // attempt to deserialize file into IConfigurationGroup
            try
            {
                return customConfigurationSerializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(fileName)));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Writes the custom configuration to the specified installed package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_WriteInstalledCustomConfigurationCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.WriteInstalledCustomConfiguration requestData)
            {
                // find package
                var package = ComponentManager.Installer.FindInstalledPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // write custom configuration
                    var results = WriteInstalledCustomConfigurationCommand(package.InstallPath, ComponentManager.ComponentController.CustomConfigurationFilename, ComponentManager.ComponentController.CustomConfigurationSerializer, requestData.Configuration, out string reason);
                    //
                    var response = new RequestResponseTypes.WriteInstalledCustomConfigurationResponse(requestData, results, reason);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Write Installed Custom Configuration, {requestData}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.WriteInstalledCustomConfigurationResponse, response);
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Installed package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will attempt the serialize and write the custom configuration file to the specified installed package.
        /// </summary>
        /// <param name="installPath">The directory path to write to.</param>
        /// <param name="customConfigurationFilename">The name of the file to write to.</param>
        /// <param name="customConfigurationSerializer">The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.</param>
        /// <param name="configuration">The <see cref="Configuration.IConfigurationGroup"/> to serialize and write to the installed package.</param>
        /// <param name="reason">The reason for the success or failure of the write operation.</param>
        /// <returns><b>True</b> indicates the write operation was successful; otherwise, <b>false</b> indicates the write operation failed.</returns>
        private bool WriteInstalledCustomConfigurationCommand(string installPath,
                                                              string customConfigurationFilename,
                                                              Configuration.IConfigurationSerializer<StringBuilder> customConfigurationSerializer,
                                                              Configuration.IConfigurationGroup configuration,
                                                              out string reason)
        {
            // check for path
            if (!System.IO.Directory.Exists(installPath))
            {
                reason = $"Directory not found. Directory={installPath}";
                return false;
            }
            // create full filename
            var fileName = System.IO.Path.Combine(installPath, customConfigurationFilename);
            // write or delete the custom configuration file
            try
            {
                if (configuration == null)
                {
                    // delete file
                    FileStorage.FileStorageHelper.DeleteFile(fileName);
                }
                else
                {
                    // write file
                    System.IO.File.WriteAllText(fileName, customConfigurationSerializer.Serialize(configuration).ToString());
                }
                //
                reason = "OK";
                return true;
            }
            catch (Exception ex)
            {
                // error
                reason = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Reads the custom configuration for the specified package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_ReadPackageCustomConfigurationCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.ReadPackageCustomConfiguration requestData)
            {
                // find package
                var package = ComponentManager.PackageProvider.FindPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    // get custom configuration for specified package
                    var results = ReadPackageCustomConfiguration(package,
                                                                 requestData,
                                                                 ComponentManager.ComponentController.CustomConfigurationFilename,
                                                                 ComponentManager.ComponentController.CustomConfigurationSerializer,
                                                                 out Configuration.IConfigurationGroup configuration,
                                                                 out string reason);
                    //
                    var response = new RequestResponseTypes.ReadPackageCustomConfigurationResponse(requestData, configuration);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Read Package Custom Configuration, {response}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.ReadPackageCustomConfigurationResponse, response);
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will attempt to read the custom configuration from the specified package.
        /// </summary>
        /// <param name="package">The package to read from.</param>
        /// <param name="requestData">The request to be processed.</param>
        /// <param name="customConfigurationFilename">The name of the file to read.</param>
        /// <param name="customConfigurationSerializer">The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.</param>
        /// <param name="configuration">The <see cref="Configuration.IConfigurationGroup"/> read from the specified package.</param>
        /// <param name="reason">The reason for the success or failure of the read operation.</param>
        /// <returns><b>True</b> indicates the read operation was successful; otherwise, <b>false</b> indicates the read operation failed.</returns>
        private bool ReadPackageCustomConfiguration(Packaging.IPackage package,
                                                    RequestResponseTypes.ReadPackageCustomConfiguration requestData,
                                                    string customConfigurationFilename,
                                                    Configuration.IConfigurationSerializer<StringBuilder> customConfigurationSerializer,
                                                    out Configuration.IConfigurationGroup configuration,
                                                    out string reason)
        {
            var tempPath = System.IO.Path.GetTempPath();
            var tempFile = "";
            try
            {
                // get package
                using (var connectedPackage = ComponentManager.PackageProvider.Connect(package.RootFilename))
                {
                    // get file store item
                    var fileStoreItem = connectedPackage.FileStore.Find((x) => { return x.RootFilename == customConfigurationFilename; }).FirstOrDefault();
                    if (fileStoreItem != null)
                    {
                        // extract file
                        tempFile = fileStoreItem.Extract(tempPath, true);
                        if (System.IO.File.Exists(tempFile))
                        {
                            // deserialize file
                            configuration = customConfigurationSerializer.Deserialize(new StringBuilder(System.IO.File.ReadAllText(tempFile)));
                            reason = $"OK";
                            return true;
                        }
                        else
                        {
                            // unable to extract file
                            configuration = null;
                            reason = $"Unable to extract file. Root Filename={fileStoreItem.RootFilename}";
                            return false;
                        }
                    }
                    else
                    {
                        // file store item not found
                        configuration = null;
                        reason = $"Unable to find file in the file store. Root Filename={customConfigurationFilename}";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // error
                configuration = null;
                reason = ex.Message;
                return false;
            }
            finally
            {
                // delete file
                FileStorage.FileStorageHelper.DeleteFile(tempFile);
            }
        }

        /// <summary>
        /// Writes the custom configuration to the specified package.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_WritePackageCustomConfigurationCommand(ServiceRequest request, string targetClientId)
        {
            if (request.Data is RequestResponseTypes.WritePackageCustomConfiguration requestData)
            {
                // find package
                var package = ComponentManager.PackageProvider.FindPackage(requestData.Name, requestData.Version);
                if (package != null)
                {
                    // package found
                    var results = WritePackageCustomConfiguration(package,
                                                                  requestData,
                                                                  ComponentManager.ComponentController.CustomConfigurationFilename,
                                                                  ComponentManager.ComponentController.CustomConfigurationSerializer,
                                                                  out string reason);
                    //
                    var response = new RequestResponseTypes.WritePackageCustomConfigurationResponse(requestData, results, reason);
                    // log it
                    AuditMessage(Logging.LogEntryType.Information, $"Write Package Custom Configuration, {response}, ClientId={targetClientId}, SessionId={request.SessionId}, GroupKey={request.GroupKey}");
                    // send response
                    SendServiceResponse(targetClientId, request, RequestResponseCommands.WritePackageCustomConfigurationResponse, response);
                }
                else
                {
                    // package not found
                    SendServiceError(targetClientId, request, new RequestResponseTypes.Error($"Package not found: {requestData}"));
                }
            }
        }

        /// <summary>
        /// Will attempt to write the custom configuration to the specified package.
        /// </summary>
        /// <param name="package">The package to write to.</param>
        /// <param name="requestData">The request to be processed.</param>
        /// <param name="customConfigurationFilename">The name of the file to write.</param>
        /// <param name="customConfigurationSerializer">The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.</param>
        /// <param name="reason">The reason for the success or failure of the write operation.</param>
        /// <returns><b>True</b> indicates the write operation was successful; otherwise, <b>false</b> indicates the write operation failed.</returns>
        private bool WritePackageCustomConfiguration(Packaging.IPackage package,
                                                     RequestResponseTypes.WritePackageCustomConfiguration requestData,
                                                     string customConfigurationFilename,
                                                     Configuration.IConfigurationSerializer<StringBuilder> customConfigurationSerializer,
                                                     out string reason)
        {
            string tempFile = System.IO.Path.GetTempFileName();
            try
            {
                // get package
                using (var connectedPackage = ComponentManager.PackageProvider.Connect(package.RootFilename))
                {
                    // get original filename
                    var originalFileItem = connectedPackage.FileStore.Find((x) => { return x.RootFilename == customConfigurationFilename; }).FirstOrDefault();
                    var originalFilename = originalFileItem.OriginalFilename;


                    // save new configuration to tempFilePath
                    var newConfig = customConfigurationSerializer.Serialize(requestData.Configuration);
                    System.IO.File.WriteAllText(tempFile, newConfig.ToString());
                    // import new configuration to package
                    var fileInfo = new System.IO.FileInfo(tempFile);
                    var addItem = connectedPackage.FileStore.Add(customConfigurationFilename, tempFile, fileInfo.LastWriteTimeUtc, new System.IO.FileInfo(tempFile).Length);
                    // save file store
                    connectedPackage.FileStore.Save(false);



                    // restore original filename
                    var configFileItem = connectedPackage.FileStore.Find((x) => { return x.RootFilename == customConfigurationFilename; }).FirstOrDefault();
                    if ((configFileItem != null) && (!string.IsNullOrWhiteSpace(originalFilename)))
                    {
                        configFileItem.SetOriginalFilename(originalFilename);
                    }
                    if (!string.IsNullOrWhiteSpace(originalFilename))
                    {
                        addItem.SetOriginalFilename(originalFilename);
                    }
                }
                //
                reason = "OK";
                return true;
            }
            catch (Exception ex)
            {
                // error
                reason = ex.Message;
                return false;
            }
            finally
            {
                // delete file
                FileStorage.FileStorageHelper.DeleteFile(tempFile);
            }
        }

        // ################ COMMUNICATION 

        /// <summary>
        /// Will return information about the communication system.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="targetClientId">The client who sent the message.</param>
        private void ProcessCommand_CommunicationInformation(ServiceRequest request, string targetClientId)
        {
            var serverType = ComponentManager.CommunicationController.ServerType.AssemblyQualifiedName;
            var stats = ComponentManager.CommunicationController.TransportController.TransportStatisticsSnapshot;
            var dude = new RequestResponseTypes.CommunicationInformationResponse(serverType, stats);
            //
            AuditMessage(Logging.LogEntryType.Information, $"CommunicationInformation, DateStarted={stats.DateStarted}, RunTime={stats.RunTime}, ServerType={serverType}");
            // send response
            SendServiceResponse(targetClientId, request, RequestResponseCommands.CommunicationInformationResponse, new RequestResponseTypes.CommunicationInformationResponse(serverType, stats));
        }
        #endregion
    }
}
