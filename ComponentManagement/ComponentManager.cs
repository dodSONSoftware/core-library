using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides access to and control of a component management system.
    /// </summary>
    /// <seealso cref="Packaging"/>
    /// <seealso cref="Installation"/>
    /// <seealso cref="Logging"/>
    /// <seealso cref="Networking"/>
    /// <seealso cref="Addon"/>
    /// <seealso cref="ComponentExtensionBase"/>
    /// <seealso cref="ComponentPluginBase"/>
    /// <example>
    /// <para>
    /// The following example will create a Component Manager, serialize it's configuration to various types of files and start it.
    /// <br/>
    /// Once started, it will run until any key is pressed.
    /// <br/>
    /// All output will be directed to a log file; which should be located in the rootPath directory.
    /// <br/>
    /// Packages will need to be created and added to a Packages directory under the rootPath directory. (see entries below for each package's contents)
    /// <br/>
    /// </para>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // ################################################################
    ///     // ######## CHANGE THIS VALUE     
    ///     // ######## Make sure this directory is empty; except for the packages.
    ///     // ######## Package Path = rootPath + "Packages"
    ///     var rootPath = @"C:\(WORKING)\Dev\ComponentManagement3";
    ///     // ################################################################
    /// 
    ///     // create Component Manager
    ///     Console.WriteLine($"Creating Component Manager");
    ///     var componentManager = CreateComponentManager(rootPath, out string logFilename);
    ///     Console.WriteLine($"Log = {logFilename}");
    ///     
    ///     // output Component Manager configuration
    ///     System.IO.File.WriteAllBytes(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.bin"), (new dodSON.Core.Configuration.BinaryConfigurationSerializer()).Serialize(componentManager.Configuration));
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.csv"), (new dodSON.Core.Configuration.CsvConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.ini"), (new dodSON.Core.Configuration.IniConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    ///     System.IO.File.WriteAllText(System.IO.Path.Combine(rootPath, "ComponentManager.configuration.xml"), (new dodSON.Core.Configuration.XmlConfigurationSerializer()).Serialize(componentManager.Configuration).ToString());
    ///     
    ///     // starting Component Manager
    ///     componentManager.Start();
    /// 
    ///     // wait for key press
    ///     Console.WriteLine($"{Environment.NewLine}Component Manager Started");
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    ///     Console.WriteLine();
    /// 
    ///     // stop Component Manager
    ///     componentManager.Stop();
    /// 
    ///     // 
    ///     Console.WriteLine($"{Environment.NewLine}Component Manager Stopped");
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    ///     Console.WriteLine();
    /// }
    /// 
    /// public static dodSON.Core.ComponentManagement.IComponentManager CreateComponentManager(string rootPath,
    ///                                                                                        out string logFilename)
    /// {
    ///     var timeStamp = DateTimeOffset.Now;
    ///     // create actual logger
    ///     logFilename = System.IO.Path.Combine(rootPath, $"{timeStamp.ToString("yyyyMMdd_HHmmss")}.log");
    ///     var writeLogEntriesUsingLocalTime = true;
    ///     var logger = new dodSON.Core.Logging.FileEventLog.Log(logFilename, writeLogEntriesUsingLocalTime);
    /// 
    ///     // create a log archive store
    ///     var logArchiveBackendStorageZipFilename = System.IO.Path.Combine(rootPath, $"ComponentManagment.{timeStamp.ToString("yyyyMMddHHmmss")}.log.archive.zip");
    ///     var logArchiveExtractionRootDirectory = System.IO.Path.GetTempPath();
    ///     var logArchiveSaveOriginalFilenames = false;
    ///     var logArchiveExtensionsToStore = new string[0];
    ///     var logArchiveFileStore = new dodSON.Core.FileStorage.IonicZip.FileStore(logArchiveBackendStorageZipFilename, logArchiveExtractionRootDirectory, logArchiveSaveOriginalFilenames, logArchiveExtensionsToStore);
    /// 
    ///     // create a file archive filename factory
    ///     var archiveFilenameFactory = new MyArchiveFilenameFactory();
    /// 
    ///     // create log controller settings
    ///     var logLogSourceId = "Logs";
    ///     var autoCacheFlushLogMax = 30;
    ///     var autoCacheFlushTimeLimit = TimeSpan.FromSeconds(30);
    ///     var writePrimaryLogEntriesUsingLocalTime = true;
    ///     var autoArchiveEnabled = true;
    ///     var autoArchiveLogMax = 10000;
    ///     var writeArchivedLogEntriesUsingLocalTime = false;
    ///     var logControllerSettings = new dodSON.Core.ComponentManagement.LogControllerSettings(logLogSourceId, autoCacheFlushLogMax, autoCacheFlushTimeLimit, writePrimaryLogEntriesUsingLocalTime, autoArchiveEnabled, autoArchiveLogMax, writeArchivedLogEntriesUsingLocalTime);
    /// 
    ///     // ******** CREATE LOG CONTROLLER
    ///     var logController = new dodSON.Core.ComponentManagement.LogController(logger, logArchiveFileStore, archiveFilenameFactory, logControllerSettings);
    /// 
    ///     // create server configuration
    ///     var serverType = typeof(dodSON.Core.Networking.NamedPipes.Server);
    ///     var clientType = typeof(dodSON.Core.Networking.NamedPipes.Client);
    ///     var ipAddress = dodSON.Core.Networking.NetworkingHelper.DefaultIpAddress;
    ///     var port = dodSON.Core.Networking.NetworkingHelper.RecommendedMinumumPortValue;
    ///     var name = "ComponentManagementServer";
    ///     var sharedServerChannelAddress = new dodSON.Core.Networking.ChannelAddress(ipAddress, port, name);
    ///     var serverId = Guid.NewGuid().ToString();
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
    ///     var cacheTimeLimit = TimeSpan.FromSeconds(90);
    ///     var transportConfiguration = new dodSON.Core.Networking.TransportConfiguration(transportEncryptorConfiguration, compressorType, useChunking, chunkSize, cacheTimeLimit);
    /// 
    ///     // ******** CREATE CHALLENGE CONTROLLER
    ///     var actualEvidence = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(128);
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
    ///     var encryptedRegistrationController = new dodSON.Core.Networking.RegistrationControllers.EncryptedRegistrationController(transportConfiguration, registrationEncryptor, passwordChallengeController);
    /// 
    ///     // ******** CREATE TRANSPORT CONTROLLER
    ///     var transportController = new dodSON.Core.Networking.TransportController(transportConfiguration, encryptedRegistrationController);
    /// 
    ///     // ******** CREATE COMMUNICATION CONTROLLER
    ///     var communicationControllerLogSourceId = "Network";
    ///     var communicationController = new dodSON.Core.ComponentManagement.CommunicationController(serverType, clientType, sharedServerChannelAddress, sharedServerConfiguration, transportController, communicationControllerLogSourceId);
    /// 
    ///     // ******** CREATE INSTALLER
    ///     var installationSettings = new dodSON.Core.Installation.InstallationSettings(dodSON.Core.Installation.InstallType.HighestVersionOnly, false, true, false);
    ///     var installationRootPath = System.IO.Path.Combine(rootPath, $"Install");
    ///     // creating installation path
    ///     if (!System.IO.Directory.Exists(installationRootPath)) { System.IO.Directory.CreateDirectory(installationRootPath); dodSON.Core.Threading.ThreadingHelper.Sleep(100); }
    ///     //
    ///     var configurationFilename = "package.config.xml";
    ///     var serializer = new dodSON.Core.Configuration.XmlConfigurationSerializer();
    ///     var installerLogSourceId = "Install";
    ///     var installer = new dodSON.Core.Installation.Installer(installationRootPath, configurationFilename, serializer, installerLogSourceId);
    /// 
    ///     // ******** CREATE PACKAGING
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
    /// public class MyArchiveFilenameFactory
    ///     : dodSON.Core.ComponentManagement.IArchiveFilenameFactory
    /// {
    ///     public string GenerateFilename =&gt; $"Archived-{DateTimeOffset.Now.ToString("yyyyMMddHHmmss")}.log";
    /// }
    /// </code>
    /// <para>The Worker Class:</para>
    /// <br/>
    /// <para>Create an assembly project and add the following code.</para>
    /// <code>
    /// public class WorkerPlugin
    ///      : dodSON.Core.ComponentManagement.ComponentPluginBase
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
    ///     protected override void OnStartup()
    ///     {
    ///         // ################################################################
    ///         // ######## CHANGE THIS VALUE     
    ///         var rootPath = @"C:\(WORKING)\Dev\ComponentManagement3";
    ///         // ################################################################
    ///     
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Hello from {base.PackageConfiguration.Name} in domain #{System.AppDomain.CurrentDomain.Id}");
    ///         // 
    ///         if (CustomConfiguration == null)
    ///         {
    ///             Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Custom Configuration File: Not Found.");
    ///         }
    ///         else
    ///         {
    ///             var outputFilename = System.IO.Path.Combine(rootPath, "customConfig.{DateTimeOffset.Now.ToString("yyyyMMdd-HHmmss")}.xml");
    ///             System.IO.File.WriteAllText(outputFilename, (new dodSON.Core.Configuration.XmlConfigurationSerializer()).Serialize(CustomConfiguration).ToString());
    ///         }
    ///         // 
    ///         _ThreadWorker = new dodSON.Core.Threading.ThreadWorker(TimeSpan.FromSeconds(1), (cancelToken) =&gt; { SendMessage&lt;string&gt;("", $"Message from domain #{AppDomain.CurrentDomain.Id}"); });
    ///         _ThreadWorker.Start();
    ///     }
    /// 
    ///     protected override void OnShutdown()
    ///     {
    ///         _ThreadWorker.Stop();
    ///         Log.Write(dodSON.Core.Logging.LogEntryType.Warning, Id, $"#### Goodbye from {base.PackageConfiguration.Name} in domain #{System.AppDomain.CurrentDomain.Id}");
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
    /// <para>The Final Screen Shot:</para>
    /// <code>
    /// Creating Component Manager
    /// Log = C:\(WORKING)\Dev\ComponentManagement3\20171110_164926.log
    /// 
    /// Component Manager Started
    /// press anykey>
    /// 
    /// Component Manager Stopped
    /// press anykey>
    /// </code>
    /// <para>The Output Log:</para>
    /// <code>
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; ****************                     Start Log, 2017-11-10 4:49:26 PM -06:00                      ****************
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; Starting ComponentManager, (dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; .NET Version=4.0.30319
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; dodSON.Core Version=1.1.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; Executable Path=c:\users\user\documents\visual studio 2017\Projects\ConsoleApp17\ConsoleApp17\bin\Debug
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; Package Storage: Total=2, Enabled=2, Disabled=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; System; Installed Packages: Total=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Starting CommunicationController, (dodSON.Core.ComponentManagement.CommunicationController, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Opening Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Server Id=82ce52a1-a271-4b91-a622-5c9054821d72
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Type=[dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Uri=net.pipe://localhost/ComponentManagementServer-49152
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; IP Address=localhost, Name=ComponentManagementServer, Port=49152
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Registering Client=ServerInternalClient_7bdd7b54-8c2b-42a5-872c-09ddodSON3f2
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Receive Own Messages=True
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Completed Registering Client=ServerInternalClient_7bdd7b54-8c2b-42a5-872c-09ddodSON3f2
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; Server Opened Successfully. Elapsed Time=00:00:00.1429391
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Network; CommunicationController Started. Elapsed Time=00:00:00.2402928
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Starting Installation, (dodSON.Core.Installation.Installer, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Install Root Path=C:\(WORKING)\Dev\ComponentManagement3\Install
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Installing package and dependency chain for [Package-A, v1.0.0.0, Package-A_1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Install Path=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Installation Type=HighestVersionOnly
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Clean Install=False
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Package Update=True
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Remove Orphaned Packages=False
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding package [Package-A, v1.0.0.0, Package-A_1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Creating directory=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\package.config.xml
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\dodSON.Core.dll
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Custom.configuration.xml
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Files: Added=4
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total packages processed:Added=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Completed installation of package and dependency chain for [Package-A, v1.0.0.0, Package-A_1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Starting Installation, (dodSON.Core.Installation.Installer, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Install Root Path=C:\(WORKING)\Dev\ComponentManagement3\Install
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Installing package and dependency chain for [Package-B, v1.0.0.0, Package-B_v1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Install Path=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Installation Type=HighestVersionOnly
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Clean Install=False
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Package Update=True
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Remove Orphaned Packages=False
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding package [Package-B, v1.0.0.0, Package-B_v1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Creating directory=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\package.config.xml
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\dodSON.Core.dll
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Adding File=C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Files: Added=3
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total packages processed:Added=1
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Total installed packages=2
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Enabled packages=2
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; #1=Package-A, v1.0.0.0, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; #2=Package-B, v1.0.0.0, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Disabled packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Orphaned packages=0
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Completed installation of package and dependency chain for [Package-B, v1.0.0.0, Package-B_v1.0.0.0.zip]
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Install; Installation Completed. Elapsed Time=00:00:00.1633014
    /// // 2017-11-10 4:49:26 PM -06:00; Information; 0; 0; Components; Starting ComponentController, (dodSON.Core.ComponentManagement.ComponentController, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Components; Extension components=0
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Components; Plugin components=2
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Components; #1=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Components; #2=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Components; Starting Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Network; Registering Client=2db331ca-4347-4d6c-a394-0cd947f6a0b9
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Network; Receive Own Messages=False
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2017-11-10 4:49:27 PM -06:00; Information; 0; 0; Network; Completed Registering Client=2db331ca-4347-4d6c-a394-0cd947f6a0b9
    /// // 2017-11-10 4:49:27 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Hello from Package-A in domain #4
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Custom Configuration File: Found.
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Name= My Custom Configuration
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Created Date= 2017-11-10 16:00
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Pi= 3.14159265358979
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Components; Plugin Component Started [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Components; Starting Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Network; Registering Client=67dcc080-ca29-423c-93d0-3db7e64eb3c9
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Network; Receive Own Messages=False
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Network; Receivable Types: (0)
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Network; Transmittable Types: (0)
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Network; Completed Registering Client=67dcc080-ca29-423c-93d0-3db7e64eb3c9
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; #### Hello from Package-B in domain #5
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; #### Custom Configuration File: Not Found.
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Components; Plugin Component Started [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; Components; Component Controller Started. Elapsed Time=00:00:01.9095955
    /// // 2017-11-10 4:49:28 PM -06:00; Information; 0; 0; System; ComponentManager Started. Elapsed Time=00:00:02.3145105
    /// // 2017-11-10 4:49:28 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:29 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:29 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:30 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:30 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:31 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:31 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:32 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:32 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:33 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:33 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:34 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:34 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:35 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:35 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:36 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:36 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:37 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:37 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:38 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:38 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #5
    /// // 2017-11-10 4:49:39 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; Message=Message from domain #4
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; System; Stopping ComponentManager, (dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Stopping ComponentController, (dodSON.Core.ComponentManagement.ComponentController, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Plugin components=2
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; #1=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; #2=[Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Extension components=0
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Stopping Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:39 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll]; #### Goodbye from Package-A in domain #4
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Unregistering Client=2db331ca-4347-4d6c-a394-0cd947f6a0b9, Date Started=2017-11-10 4:49:27 PM, Runtime=00:00:11.2462076
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Plugin Component Stopped [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-A_v1.0.0.0\Playground_Worker01.dll], Start Time=2017-11-10 4:49:27 PM, Run Time=00:00:12.0308407
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Stopping Plugin Component [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]
    /// // 2017-11-10 4:49:39 PM -06:00; Warning; 0; 0; [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll]; #### Goodbye from Package-B in domain #5
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Unregistering Client=67dcc080-ca29-423c-93d0-3db7e64eb3c9, Date Started=2017-11-10 4:49:28 PM, Runtime=00:00:10.4617317
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Plugin Component Stopped [Playground_Worker01.WorkerPlugin, C:\(WORKING)\Dev\ComponentManagement3\Install\Package-B_v1.0.0.0\Playground_Worker01.dll], Start Time=2017-11-10 4:49:28 PM, Run Time=00:00:11.1680255
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Components; Component Controller Stopped. Start Time=2017-11-10 4:49:26 PM -06:00, Runtime=00:00:12.4248282
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Stopping CommunicationController, (dodSON.Core.ComponentManagement.CommunicationController, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Closing Server, (dodSON.Core.Networking.NamedPipes.Server, dodSON.Core, v1.1.0.0)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Closing All Clients: (1)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Unregistering Client=ServerInternalClient_7bdd7b54-8c2b-42a5-872c-09ddodSON3f2, Date Started=2017-11-10 4:49:26 PM, Runtime=00:00:12.6238910
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Shutting Down Server, Id=82ce52a1-a271-4b91-a622-5c9054821d72
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Total Incoming Bytes=16 KB (16,560)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Total Incoming Envelopes=23
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Total Outgoing Bytes=32 KB (32,400)
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Total Outgoing Envelopes=45
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; Server Closed Successfully.
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; Network; CommunicationController Stopped. Date Started=2017-11-10 4:49:26 PM -06:00, Runtime=00:00:12.7829017
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; System; ComponentManager Stopped. Start Time=2017-11-10 4:49:26 PM -06:00, Runtime=00:00:12.8649011
    /// // 2017-11-10 4:49:39 PM -06:00; Information; 0; 0; System; ****************        End Log, Written Logs=161, Start Time=2017-11-10 4:49:26 PM -06:00        ****************
    /// </code>
    /// <para>The Exported Component Manager Configuration File, XML version:</para>
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
    ///         &lt;item key="ServerId" type="System.String"&gt;82ce52a1-a271-4b91-a622-5c9054821d72&lt;/item&gt;
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
    ///                 &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.RegistrationControllers.EncryptedRegistrationController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///               &lt;/items&gt;
    ///               &lt;groups&gt;
    ///                 &lt;group key="ChallengeController"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="Evidence" type="System.Byte[]"&gt;AYAAf//D2HbStJc/oCMZfTwKVxHq1uCBWecJPQp+LmJWaPunSZiA3lQoFHplINIYFNtgmyEYKfUhW9ktVR+NMsFtrsxVCXjxEXQQTjmCpOsWi8Yj3LcINUPwol6ukjE090B4R4M3Extob9sGVz1hFyiFUvKt2XYprTCN1Kj/ZZ69hDn8ug==&lt;/item&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Networking.ChallengeControllers.PasswordChallengeController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                 &lt;/group&gt;
    ///                 &lt;group key="Encryptor"&gt;
    ///                   &lt;items&gt;
    ///                     &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Cryptography.StackableEncryptor, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///                   &lt;/items&gt;
    ///                   &lt;groups&gt;
    ///                     &lt;group key="EncryptorConfigurations"&gt;
    ///                       &lt;groups&gt;
    ///                         &lt;group key="Encryptor01"&gt;
    ///                           &lt;items&gt;
    ///                             &lt;item key="HashAlgorithm" type="System.Type"&gt;System.Security.Cryptography.MD5Cng, System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                             &lt;item key="PasswordSaltHash" type="System.String"&gt;Uwzi1aff712zZvrKuHELpA==&lt;/item&gt;
    ///                             &lt;item key="Salt" type="System.String"&gt;BzZzu4D3PXgBon2FQ8c/MYfD6ijbK8f8o3VQCRNxXgM9xOL/MT5nHD9VwZdbJWy09IS+sUpt3RfFCuPUXYOZ+vy6pr+dlZohv/+UQ/4h4+PQrEjqSzyHu1S2wPxb/uL6b8Hh18OzcXybqgr7zjskJej/Yx6/V/eiSV9gh+HnI4s=&lt;/item&gt;
    ///                             &lt;item key="SymmetricAlgorithmType" type="System.Type"&gt;System.Security.Cryptography.TripleDESCryptoServiceProvider, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089&lt;/item&gt;
    ///                           &lt;/items&gt;
    ///                         &lt;/group&gt;
    ///                       &lt;/groups&gt;
    ///                     &lt;/group&gt;
    ///                   &lt;/groups&gt;
    ///                 &lt;/group&gt;
    ///               &lt;/groups&gt;
    ///             &lt;/group&gt;
    ///             &lt;group key="TransportConfiguration"&gt;
    ///               &lt;items&gt;
    ///                 &lt;item key="CacheTimeLimit" type="System.TimeSpan"&gt;00:01:30&lt;/item&gt;
    ///                 &lt;item key="ChunkSize" type="System.Int32"&gt;512&lt;/item&gt;
    ///                 &lt;item key="Compressor" type="System.Type"&gt;dodSON.Core.Compression.DeflateStreamCompressor, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
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
    ///                             &lt;item key="PasswordSaltHash" type="System.Byte[]"&gt;AUAAv//mU7WuD7WdLu0NZ6GuskejCDZiLoOhry2htOmfshTmfkgRgu2Ohcot1cd5eTnQGRe6lLmL8HJITRQMNY7xEIcL&lt;/item&gt;
    ///                             &lt;item key="Salt" type="System.Byte[]"&gt;AYAAf//jA0NYgJpflGTao9HfRSUEn4PpWa2aJ4MOKH/w5NI38+p6T2s7fJD3CDIK+ff9+nXfganmWi0U7PeMHZh2A8yUQ9dcolukcjeVqXLmod70rEvwAqrEg5m4bwk/FdvUSDwU7v9pGxsG/v4kfzxBmLVbAb7Peqw8t6iOic3mt638fg==&lt;/item&gt;
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
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="Installer"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="ConfigurationFilename" type="System.String"&gt;package.config.xml&lt;/item&gt;
    ///         &lt;item key="ConfigurationFileSerializer" type="System.Type"&gt;dodSON.Core.Configuration.XmlConfigurationSerializer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="InstallationRootPath" type="System.String"&gt;C:\(WORKING)\Dev\ComponentManagement3\Install&lt;/item&gt;
    ///         &lt;item key="LogSourceId" type="System.String"&gt;Install&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Installation.Installer, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///     &lt;/group&gt;
    ///     &lt;group key="LogController"&gt;
    ///       &lt;items&gt;
    ///         &lt;item key="ArchiveFilenameFactory" type="System.Type"&gt;ConsoleApp17.Program+MyArchiveFilenameFactory, ConsoleApp17, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///         &lt;item key="Type" type="System.Type"&gt;dodSON.Core.ComponentManagement.LogController, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///       &lt;/items&gt;
    ///       &lt;groups&gt;
    ///         &lt;group key="FileStore"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="BackendStorageZipFilename" type="System.String"&gt;C:\(WORKING)\Dev\ComponentManagement3\ComponentManagment.20171110164926.log.archive.zip&lt;/item&gt;
    ///             &lt;item key="ExtractionRootDirectory" type="System.String"&gt;&lt;/item&gt;
    ///             &lt;item key="SaveOriginalFilenames" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.FileStorage.IonicZip.FileStore, dodSON.Core.FileStorage.IonicZipStore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="Log"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoTruncateLogFile" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="LogFilename" type="System.String"&gt;C:\(WORKING)\Dev\ComponentManagement3\20171110_164926.log&lt;/item&gt;
    ///             &lt;item key="LogsToRetain" type="System.Int32"&gt;0&lt;/item&gt;
    ///             &lt;item key="MaxLogSizeBytes" type="System.Int64"&gt;10240&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.Logging.FileEventLog.Log, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///             &lt;item key="WriteLogEntriesUsingLocalTime" type="System.Boolean"&gt;True&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///         &lt;group key="LogControllerSettings"&gt;
    ///           &lt;items&gt;
    ///             &lt;item key="AutoArchiveEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    ///             &lt;item key="AutoArchiveMaximumLogs" type="System.Int32"&gt;10000&lt;/item&gt;
    ///             &lt;item key="AutoFlushMaximumLogs" type="System.Int32"&gt;30&lt;/item&gt;
    ///             &lt;item key="AutoFlushTimeLimit" type="System.TimeSpan"&gt;00:00:30&lt;/item&gt;
    ///             &lt;item key="LogSourceId" type="System.String"&gt;Logs&lt;/item&gt;
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
    ///             &lt;item key="ExtractionRootDirectory" type="System.String"&gt;C:\Users\User\AppData\Local\Temp\&lt;/item&gt;
    ///             &lt;item key="SaveOriginalFilenames" type="System.Boolean"&gt;False&lt;/item&gt;
    ///             &lt;item key="SourceDirectory" type="System.String"&gt;C:\(WORKING)\Dev\ComponentManagement3\Packages&lt;/item&gt;
    ///             &lt;item key="Type" type="System.Type"&gt;dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore, dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null&lt;/item&gt;
    ///           &lt;/items&gt;
    ///         &lt;/group&gt;
    ///       &lt;/groups&gt;
    ///     &lt;/group&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;    
    /// </code>
    /// <para>Files in Package-A:</para>
    /// <code>
    /// package.config.xml
    /// Custom.configuration.xml
    /// dodSON.Core.dll
    /// Playground_Worker01.dll
    /// </code>
    /// <para>Package-A's Package Configuration File:</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="PackageConfiguration"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="IsDependencyPackage" type="System.Boolean"&gt;False&lt;/item&gt;
    ///     &lt;item key="IsEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     &lt;item key="Name" type="System.String"&gt;Package-A&lt;/item&gt;
    ///     &lt;item key="Version" type="System.Version"&gt;1.0.0.0&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="DependencyPackages" /&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>Package-A's Custom Configuration File:</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="CustomConfiguration"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="Name" type="System.String"&gt;My Custom Configuration&lt;/item&gt;
    ///     &lt;item key="Pi" type="System.Double"&gt;3.14159265358979&lt;/item&gt;
    ///     &lt;item key="Created Date" type="SystemDateTimeOffset"&gt;2017-11-10 16:00&lt;/item&gt;
    ///   &lt;/items&gt;
    /// &lt;/group&gt;
    /// </code>
    /// <para>Files in Package-B</para>
    /// <code>
    /// package.config.xml
    /// dodSON.Core.dll
    /// Playground_Worker01.dll
    /// </code>
    /// <para>Package-B's Package Configuration File:</para>
    /// <code>
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;group key="PackageConfiguration"&gt;
    ///   &lt;items&gt;
    ///     &lt;item key="IsDependencyPackage" type="System.Boolean"&gt;False&lt;/item&gt;
    ///     &lt;item key="IsEnabled" type="System.Boolean"&gt;True&lt;/item&gt;
    ///     &lt;item key="Name" type="System.String"&gt;Package-B&lt;/item&gt;
    ///     &lt;item key="Version" type="System.Version"&gt;1.0.0.0&lt;/item&gt;
    ///   &lt;/items&gt;
    ///   &lt;groups&gt;
    ///     &lt;group key="DependencyPackages" /&gt;
    ///   &lt;/groups&gt;
    /// &lt;/group&gt;
    /// </code>
    /// </example>
    public class ComponentManager
        : IComponentManager
    {
        #region Private Static Fields
        /// <summary>
        /// The width of the first and last lines written to the log.
        /// </summary>
        private static readonly int _HeaderTrailerLineWidth_ = 100;
        #endregion
        #region Ctor
        private ComponentManager()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ComponentManager"/>.
        /// </summary>
        /// <param name="logController">Provides access and controls to a configurable, single-entry, memory-cached, archive-enabled logging mechanism.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="customLogHeader">Custom <see cref="Logging.ILog"/>s which will be added to the beginning of each <see cref="IComponentManager"/> log session.</param>
        /// <param name="customLogTrailer">Custom <see cref="Logging.ILog"/>s which will be added to the end of each <see cref="IComponentManager"/> log session.</param>
        /// <param name="packageProvider">Provides methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.</param>
        /// <param name="installationSettings">Provides settings needed to determine how to install packages.</param>
        /// <param name="installer">Provides methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.</param>
        /// <param name="communicationController">Provides access and controls to a configurable, type-based messaging communication system.</param>
        /// <param name="componentController">Provides access and controls to an extension and plugin component control system.</param>
        /// <param name="folderAccessItems">A collection of folders.</param>
        public ComponentManager(ILogController logController,
                                string logSourceId,
                                Logging.Logs customLogHeader,
                                Logging.Logs customLogTrailer,
                                Packaging.IPackageProvider packageProvider,
                                Installation.InstallationSettings installationSettings,
                                Installation.IInstaller installer,
                                ICommunicationController communicationController,
                                IComponentController componentController,
                                IFolderAccessItems folderAccessItems)
            : this()
        {
            LogController = logController ?? throw new ArgumentNullException(nameof(logController));
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
            _CustomLogHeader = customLogHeader;
            _CustomLogTrailer = customLogTrailer;
            PackageProvider = packageProvider ?? throw new ArgumentNullException(nameof(packageProvider));
            InstallationSettings = installationSettings ?? throw new ArgumentNullException(nameof(installationSettings));
            Installer = installer ?? throw new ArgumentNullException(nameof(installer));
            CommunicationController = communicationController ?? throw new ArgumentNullException(nameof(communicationController));
            CommunicationController.ActivityLogsEvent += CommunicationController_ActivityLogsEvent;
            ComponentController = componentController ?? throw new ArgumentNullException(nameof(componentController));
            FolderAccessItems = folderAccessItems ?? throw new ArgumentNullException(nameof(folderAccessItems));
            ComponentController.Initialize(logController, CommunicationController, Installer, FolderAccessItems);
        }
        /// <summary>
        /// Instantiates a new <see cref="ComponentManager"/> without custom log headers and trailers.
        /// </summary>
        /// <param name="logController">Provides access and controls to a configurable, single-entry, memory-cached, archive-enabled logging mechanism.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="packageProvider">Provides methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.</param>
        /// <param name="installationSettings">Provides settings needed to determine how to install packages.</param>
        /// <param name="installer">Provides methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.</param>
        /// <param name="communicationController">Provides access and controls to a configurable, type-based messaging communication system.</param>
        /// <param name="componentController">Provides access and controls to an extension and plugin component control system.</param>
        /// <param name="folderAccessItems">A collection of folders</param>
        public ComponentManager(ILogController logController,
                                string logSourceId,
                                Packaging.IPackageProvider packageProvider,
                                Installation.InstallationSettings installationSettings,
                                Installation.IInstaller installer,
                                ICommunicationController communicationController,
                                IComponentController componentController,
                                IFolderAccessItems folderAccessItems) : this(logController, logSourceId, null, null, packageProvider, installationSettings, installer, communicationController, componentController, folderAccessItems) { }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ComponentManager(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "ComponentManager")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ComponentManager\". Configuration Key={configuration.Key}", nameof(configuration));
            }

            // log controller
            LogController = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<ILogController>(configuration, "LogController", true);

            // log source id
            LogSourceId = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationItem<string>(configuration, "LogSourceId", true);

            // TODO: add custom logging header/trailer filenames

            // custom log header filename
            // custom log trailer filename

            // package provider
            PackageProvider = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<Packaging.IPackageProvider>(configuration, "PackageProvider", true);

            // installer
            Installer = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<Installation.IInstaller>(configuration, "Installer", true);

            // installation settings
            InstallationSettings = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<Installation.InstallationSettings>(configuration, "InstallationSettings", true);

            // communication controller
            CommunicationController = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<ICommunicationController>(configuration, "CommunicationController", true);
            CommunicationController.ActivityLogsEvent += CommunicationController_ActivityLogsEvent;

            // component controller
            ComponentController = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<IComponentController>(configuration, "ComponentController", true);

            // folder access items
            FolderAccessItems = Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationSubGroup<IFolderAccessItems>(configuration, "FolderAccessItems", true);

            // ####
            ComponentController.Initialize(LogController, CommunicationController, Installer, FolderAccessItems);
        }
        #endregion
        #region Private Fields
        private DateTimeOffset _LastCommunicationRequestAllStatisticsRequestTime = DateTimeOffset.MinValue;
        private TimeSpan _CommunicationRequestAllStatisticsRequestDisplayTimeLimit = TimeSpan.FromHours(1);
        private Logging.Logs _CustomLogHeader;
        private Logging.Logs _CustomLogTrailer;
        #endregion
        #region Public Methods
        /// <summary>
        /// Indicates the operating state of the <see cref="IComponentManager"/>.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IComponentManager"/> started.
        /// </summary>
        public DateTimeOffset StartTime { get; private set; } = DateTimeOffset.Now;
        /// <summary>
        /// The duration that the <see cref="IComponentManager"/> has run. 
        /// </summary>
        public TimeSpan RunTime => (DateTimeOffset.Now - StartTime);
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId
        {
            get;
        }
        /// <summary>
        /// Initializes and prepares all sub-systems, finds and starts all <see cref="ComponentExtensionBase"/> and <see cref="ComponentPluginBase"/> components.
        /// </summary>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        public void Start(bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting)
        {
            if (IsRunning)
            {
                throw new Exception("ComponentManager is already running.");
            }
            //
            IsRunning = true;
            StartTime = DateTimeOffset.Now;
            var stWatch_StartupTimer = System.Diagnostics.Stopwatch.StartNew();

            // log source ids
            var communicationLogSourceId = CommunicationController.LogSourceId;
            var installationLogSourceId = Installer.LogSourceId;
            var componentLogSourceId = ComponentController.LogSourceId;

            // #### START LOGGING

            LogController.Start();
            if (clearLogBeforeStarting)
            {
                LogController.Clear(clearArchivedLogsBeforeStarting);
            }
            GenerateLogHeaderStart();

            // #### START THE SERVER

            // ----
            var commCtrlType = CommunicationController.GetType();
            LogController.Write(Logging.LogEntryType.Information, communicationLogSourceId, $"Starting {commCtrlType.Name}, {Common.TypeHelper.FormatDisplayName(commCtrlType)}");
            // ----

            MainServer = CommunicationController.CreateSharedServer();
            if (MainServer.Open(out Exception mainServerOpenException) != Networking.ChannelStates.Open)
            {
                throw new Exception("Could not open server.", mainServerOpenException);
            }

            // ----
            LogController.Write(Logging.LogEntryType.Information, communicationLogSourceId, $"{commCtrlType.Name} Started. Elapsed Time={stWatch_StartupTimer.Elapsed}");
            // ----

            // #### INSTALL THE PACKAGES

            // ----
            var installerType = Installer.GetType();
            LogController.Write(Logging.LogEntryType.Information, installationLogSourceId, $"Starting {installerType.Name}, {Common.TypeHelper.FormatDisplayName(installerType)}");
            LogController.Write(Logging.LogEntryType.Information, installationLogSourceId, $"Install Root Path={Installer.InstallationPath}");
            LogController.Write(Installer.LogInstalledPackages(LogSourceId));
            // ----

            var stWatch_Installer = System.Diagnostics.Stopwatch.StartNew();
            if (InstallationSettings.InstallType == Installation.InstallType.HighestVersionOnly)
            {
                // InstallType.HighestVersionOnly
                foreach (var package in from x in PackageProvider.PackagesEnabledHighestVersionsOnly
                                        where !x.PackageConfiguration.IsDependencyPackage
                                        orderby x.PackageConfiguration.Priority ascending
                                        select x)
                {
                    LogController.Write(Installer.Install(package, PackageProvider, InstallationSettings));
                }
            }
            else
            {
                // InstallType.SideBySide
                foreach (var package in from x in PackageProvider.NonDependencyPackages
                                        where x.PackageConfiguration.IsEnabled
                                        orderby x.PackageConfiguration.Priority ascending
                                        select x)
                {
                    LogController.Write(Installer.Install(package, PackageProvider, InstallationSettings));
                }
            }
            LogController.Write(Installer.LogInstalledPackages(LogSourceId));
            stWatch_Installer.Stop();

            // ----
            LogController.Write(Logging.LogEntryType.Information, installationLogSourceId, $"Installation Completed. Elapsed Time={stWatch_Installer.Elapsed}");
            // ----

            // #### START ALL COMPONENTS

            // ----
            var componentControllerType = ComponentController.GetType();
            LogController.Write(Logging.LogEntryType.Information, componentLogSourceId, $"Starting {componentControllerType.Name}, {Common.TypeHelper.FormatDisplayName(componentControllerType)}");
            // ----

            var stWatch_ComponentController = System.Diagnostics.Stopwatch.StartNew();
            ComponentController.Start();
            stWatch_ComponentController.Stop();

            // ----
            LogController.Write(Logging.LogEntryType.Information, componentLogSourceId, $"{componentControllerType.Name} Started. Elapsed Time={stWatch_ComponentController.Elapsed}");
            // ----

            stWatch_StartupTimer.Stop();
            GenerateLogHeaderEnd(stWatch_StartupTimer);
        }
        /// <summary>
        /// Will shutdown all <see cref="ComponentExtensionBase"/> and <see cref="ComponentPluginBase"/> components and terminate all sub-systems.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentManager is not running.");
            }
            //
            IsRunning = false;

            // log source ids
            var communicationLogSourceId = CommunicationController.LogSourceId;
            var componentLogSourceId = ComponentController.LogSourceId;

            //
            GenerateLogTrailerStart();

            // #### STOP ALL COMPONENTS

            // ----
            var componentControllerType = ComponentController.GetType();
            LogController.Write(Logging.LogEntryType.Information, componentLogSourceId, $"Stopping {componentControllerType.Name}, {Common.TypeHelper.FormatDisplayName(componentControllerType)}");
            // ----

            ComponentController.Stop();

            // ----
            LogController.Write(Logging.LogEntryType.Information, componentLogSourceId, $"{componentControllerType.Name} Stopped. Start Time={StartTime}, Runtime={DateTimeOffset.Now - StartTime}");
            // ----

            // #### STOP THE SERVER

            if (MainServer.State == Networking.ChannelStates.Open)
            {
                // ----
                var commType = CommunicationController.GetType();
                LogController.Write(Logging.LogEntryType.Information, communicationLogSourceId, $"Stopping {commType.Name}, {Common.TypeHelper.FormatDisplayName(commType)}");
                // ----

                if (!MainServer.Close(out Exception mainServerCloseException))
                {
                    LogController.Write(Logging.LogEntryType.Error, communicationLogSourceId, $"Unable to close server. Exception={mainServerCloseException.Message}");
                }

                // ----
                LogController.Write(Logging.LogEntryType.Information, communicationLogSourceId, $"{commType.Name} Stopped. Date Started={CommunicationController.StartTime}, Runtime={CommunicationController.RunTime}");
                // ----
            }


            // #### STOP LOGGING

            GenerateLogTrailerEnd();
            LogController.Stop();
        }
        /// <summary>
        /// Provides access and controls to a configurable, single-entry, memory-cached, archive-enabled logging mechanism.
        /// </summary>
        public ILogController LogController
        {
            get;
        }
        /// <summary>
        /// Provides access and controls to a configurable, type-based messaging communication system.
        /// </summary>
        public ICommunicationController CommunicationController
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IServer"/> used by this <see cref="IComponentManager"/> for it's intra-component communication system.
        /// </summary>
        public Networking.IServer MainServer
        {
            get; private set;
        }
        /// <summary>
        /// Provides methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.
        /// </summary>
        public Packaging.IPackageProvider PackageProvider
        {
            get;
        }
        /// <summary>
        /// Provides settings needed to determine how to install packages.
        /// </summary>
        public Installation.InstallationSettings InstallationSettings
        {
            get;
        }
        /// <summary>
        /// Provides methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.
        /// </summary>
        public Installation.IInstaller Installer
        {
            get;
        }
        /// <summary>
        /// Provides access and controls to an extension and plugin component control system.
        /// </summary>
        public IComponentController ComponentController
        {
            get;
        }
        /// <summary>
        /// A collection of folders.
        /// </summary>
        public IFolderAccessItems FolderAccessItems
        {
            get; private set;
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
                var result = new Configuration.ConfigurationGroup("ComponentManager");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // log controller
                result.Add(LogController.Configuration);
                // log source id
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());

                // TODO: add custom logging header/trailer filenames

                // custom log header filename
                // custom log trailer filename

                // package provider
                result.Add(PackageProvider.Configuration);
                // installer
                result.Add(Installer.Configuration);
                // installation settings
                result.Add(InstallationSettings.Configuration);
                // communication controller
                result.Add(CommunicationController.Configuration);
                // component controller
                result.Add(ComponentController.Configuration);
                // folder access items
                result.Add(FolderAccessItems.Configuration);
                // 
                return result;
            }
        }
        #endregion
        #region Private Methods
        private void CommunicationController_ActivityLogsEvent(object sender, Networking.ActivityLogsEventArgs e)
        {
            if (e.Header == Networking.ActivityLogsEventType.Network_RequestAllStatistics)
            {
                // limit the display of the 'RequestAllStatistics' event; otherwise it may flood the log
                if ((DateTimeOffset.Now - _LastCommunicationRequestAllStatisticsRequestTime) > _CommunicationRequestAllStatisticsRequestDisplayTimeLimit)
                {
                    LogController.Write(e.Logs);
                    _LastCommunicationRequestAllStatisticsRequestTime = DateTimeOffset.Now;
                }
            }
            else
            {
                LogController.Write(e.Logs);
            }
        }
        private void GenerateLogHeaderStart()
        {
            // log: default header
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"**************** {Common.StringHelper.PadCenter($"Start Log, {DateTimeOffset.Now}", _HeaderTrailerLineWidth_, ' ')} ****************");
            // log: custom header
            if (_CustomLogHeader != null)
            {
                LogController.Write(_CustomLogHeader);
            }
            // log: default header
            var thisType = this.GetType();
            var coreAssembly = System.Reflection.Assembly.GetAssembly(thisType);
            var parts = coreAssembly.FullName.Split(',');
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting {thisType.Name}, {Common.TypeHelper.FormatDisplayName(thisType)}");
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $".NET Version={coreAssembly.ImageRuntimeVersion.Substring(1)}");
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"{parts[0]}{parts[1]}");
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Executable Path={System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(this.GetType()).Location)}");
            // log: logging

            // log: communication

            // log: packaging
            var totalPackagesCount = PackageProvider.Packages.Count();
            var packagesText = new StringBuilder($"Package Storage: Total={totalPackagesCount}");
            if (totalPackagesCount > 0)
            {
                var totalEnabledPackagesCount = (from x in PackageProvider.Packages
                                                 where x.PackageConfiguration.IsEnabled
                                                 select x).Count();
                packagesText.Append($", Enabled={totalEnabledPackagesCount}, Disabled={totalPackagesCount - totalEnabledPackagesCount}");
            }
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, packagesText.ToString());
            // log: installer
            var totalInstalledPackagesCount = Installer.InstalledPackages.Count();
            var installedPackagesText = new StringBuilder($"Installed Packages: Total={totalInstalledPackagesCount}");
            if (totalInstalledPackagesCount > 0)
            {
                installedPackagesText.Append($", NonDependency={Installer.InstalledNonDependencyPackages.Count()}, ");
                installedPackagesText.Append($"Dependency={Installer.InstalledDependencyPackages.Count()}");
                LogController.Write(Logging.LogEntryType.Information, LogSourceId, installedPackagesText.ToString());
                // 
                installedPackagesText.Clear();
                installedPackagesText.Append($"Installed Packages: Disabled={Installer.InstalledDisabledPackages.Count()}, ");
                installedPackagesText.Append($"Orphaned={Installer.InstalledOrphanedDependencyPackages.Count()}");
                LogController.Write(Logging.LogEntryType.Information, LogSourceId, installedPackagesText.ToString());
            }
            else
            {
                LogController.Write(Logging.LogEntryType.Information, LogSourceId, "Installed Packages: Total=0");
            }
            // log: ?

        }
        private void GenerateLogHeaderEnd(System.Diagnostics.Stopwatch stWatch)
        {
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"{this.GetType().Name} Started. Elapsed Time={stWatch.Elapsed}");
        }
        private void GenerateLogTrailerStart()
        {
            var thisType = this.GetType();
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping {thisType.Name}, {Common.TypeHelper.FormatDisplayName(thisType)}");
        }
        private void GenerateLogTrailerEnd()
        {
            if (_CustomLogTrailer != null)
            {
                LogController.Write(_CustomLogTrailer);
            }
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"{this.GetType().Name} Stopped. Start Time={StartTime}, Runtime={RunTime}");
            // +1 is for the line being added.
            LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"**************** {Common.StringHelper.PadCenter($"End Log, Written Logs={LogController.Statistics.IncomingLogCount + 1}, Start Time={LogController.StartTime}", _HeaderTrailerLineWidth_, ' ')} ****************");
        }
        #endregion
    }
}
