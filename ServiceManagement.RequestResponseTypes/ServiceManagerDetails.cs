using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains detailed information about the Service Manager.
    /// </summary>
    [Serializable]
    public class ServiceManagerDetails
    {
        #region Ctor
        private ServiceManagerDetails()
        {
        }
        /// <summary>
        /// Creates a new <see cref="ServiceManagerDetails"/> object.
        /// </summary>
        /// <param name="serviceManager">The Service Manager to get details from.</param>
        public ServiceManagerDetails(IServiceManager serviceManager)
            : this()
        {
            if (serviceManager == null)
            {
                throw new ArgumentNullException(nameof(serviceManager));
            }

            // service manager
            ServiceManagerId = serviceManager.ServiceManagerId;
            ServiceManagerStartTime = serviceManager.StartTime;
            ServiceManagerRunTime = serviceManager.RunTime;
            ServiceManagerLogSourceId = serviceManager.LogSourceId;
            ServiceManagerAuditMessages = serviceManager.AuditMessages;
            ServiceManagerAuditDebugMessages = serviceManager.AuditDebugMessages;
            ServiceManagerReceivedRequests = serviceManager.ReceivedRequests;
            ServiceManagerWorkingRequests = serviceManager.WorkingRequests;
            ServiceManagerProcessedRequests = serviceManager.ProcessedRequests;
            ServiceManagerRequestsResponseTimeout = serviceManager.RequestsResponseTimeout;
            ServiceManagerSessionCacheTimeLimit = serviceManager.SessionCacheTimeLimit;
            ServiceManagerRequestsPurgeInterval = serviceManager.RequestsPurgeInterval;
            ServiceManagerExecuteRemainingRequestsOnStop = serviceManager.ExecuteRemainingRequestsOnStop;
          
            // component manager
            ComponentManagerLogSourceId = serviceManager.ComponentManager.LogSourceId;
            ComponentManagerStartTime = serviceManager.ComponentManager.StartTime;
            ComponentManagerRunTime = serviceManager.ComponentManager.RunTime;
            ComponentManagerFolderAccessItems = serviceManager.ComponentManager.FolderAccessItems;

            // services
            ServiceManagerServices = new ServiceManagerServices(serviceManager);

            // communication controller
            CommunicationControllerType = serviceManager.ComponentManager.CommunicationController.GetType().AssemblyQualifiedName;
            CommunicationControllerClientType = serviceManager.ComponentManager.CommunicationController.ClientType.AssemblyQualifiedName;
            CommunicationControllerLogSourceId = serviceManager.ComponentManager.CommunicationController.LogSourceId;
            CommunicationControllerServerType = serviceManager.ComponentManager.CommunicationController.ServerType.AssemblyQualifiedName;
            CommunicationControllerIPAddress = serviceManager.ComponentManager.CommunicationController.SharedChannelAddress.IPAddress;
            CommunicationControllerName = serviceManager.ComponentManager.CommunicationController.SharedChannelAddress.Name;
            CommunicationControllerPort = serviceManager.ComponentManager.CommunicationController.SharedChannelAddress.Port;
            CommunicationControllerUri= serviceManager.ComponentManager.MainServer.AddressUri.AbsoluteUri;

            // component controller
            ComponentControllerType = serviceManager.ComponentManager.ComponentController.GetType().AssemblyQualifiedName;
            ComponentControllerCustomConfigurationFilename = serviceManager.ComponentManager.ComponentController.CustomConfigurationFilename;
            ComponentControllerCustomConfigurationSerializer = serviceManager.ComponentManager.ComponentController.CustomConfigurationSerializer.GetType().AssemblyQualifiedName;
            ComponentControllerLogSourceId = serviceManager.ComponentManager.ComponentController.LogSourceId;

            // installer controller
            InstallerType = serviceManager.ComponentManager.Installer.GetType().AssemblyQualifiedName;
            InstallerNonDependencyPackages = new List<Installation.IInstalledPackage>(from x in serviceManager.ComponentManager.Installer.InstalledNonDependencyPackages select x);
            InstallerDependencyPackages = new List<Installation.IInstalledPackage>(from x in serviceManager.ComponentManager.Installer.InstalledDependencyPackages select x);
            InstallerDisabledPackages = new List<Installation.IInstalledPackage>(from x in serviceManager.ComponentManager.Installer.InstalledDisabledPackages select x);
            InstallerOrphanedDependencyPackages = new List<Installation.IInstalledPackage>(from x in serviceManager.ComponentManager.Installer.InstalledOrphanedDependencyPackages select x);
            InstallerPackagesWithMissingDependencies = new List<Installation.IInstalledPackage>(from x in serviceManager.ComponentManager.Installer.InstalledPackagesWithMissingDependencies select x);
            InstallerLogSourceId = serviceManager.ComponentManager.Installer.LogSourceId;

            // log controller
            LogControllerType = serviceManager.ComponentManager.LogController.GetType().AssemblyQualifiedName;
            LogControllerLogSourceId = serviceManager.ComponentManager.LogController.LogSourceId;
            LogControllerLogCount = serviceManager.ComponentManager.LogController.Statistics.TotalLogCount;

            // package controller
            PackageProviderType = serviceManager.ComponentManager.PackageProvider.GetType().AssemblyQualifiedName;
            PackageProviderDependencyPackages = new List<Packaging.IPackage>(from x in serviceManager.ComponentManager.PackageProvider.DependencyPackages select x);
            PackageProviderNonDependencyPackages = new List<Packaging.IPackage>(from x in serviceManager.ComponentManager.PackageProvider.NonDependencyPackages select x);
            PackageProviderPackageFileStoreProvider = serviceManager.ComponentManager.PackageProvider.PackageFileStoreProvider.GetType().AssemblyQualifiedName;
        }
        #endregion
        #region Public Properties
        // service manager
        /// <summary>
        /// The Service Manager's id.
        /// </summary>
        public string ServiceManagerId
        {
            get;
        }
        /// <summary>
        /// The DateTimeOffset when the IServiceManager started.
        /// </summary>
        public DateTimeOffset ServiceManagerStartTime
        {
            get;
        }
        /// <summary>
        /// Returns the amount of time the IServiceManager has been running.
        /// </summary>
        TimeSpan ServiceManagerRunTime
        {
            get;
        }
        /// <summary>
        /// Returns the Service Manager's source id when creating log entries.
        /// </summary>
        public string ServiceManagerLogSourceId
        {
            get;
        }
        /// <summary>
        /// Gets whether messages will be audited in the logs.
        /// </summary>
        public bool ServiceManagerAuditMessages
        {
            get;
        }
        /// <summary>
        /// Gets whether debug information will be audited in the logs.
        /// </summary>
        public bool ServiceManagerAuditDebugMessages
        {
            get;
        }
        /// <summary>
        /// The number of requests received.
        /// </summary>
        public long ServiceManagerReceivedRequests
        {
            get;

        }
        /// <summary>
        /// The number of requests currently being managed by the IServiceManager.
        /// </summary>
        public long ServiceManagerWorkingRequests
        {
            get;
        }
        /// <summary>
        /// The number of requests that have been processed.
        /// </summary>
        public long ServiceManagerProcessedRequests
        {
            get;
        }
        /// <summary>
        /// The amount of time a cached item will remain in the cache before timing out and being purged.
        /// </summary>
        public TimeSpan ServiceManagerRequestsResponseTimeout
        {
            get;
        }
        /// <summary>
        /// The amount of time a session will remain in the cache, without activity, before timing out.
        /// </summary>
        public TimeSpan ServiceManagerSessionCacheTimeLimit
        {
            get;
        }
        /// <summary>
        /// The amount of time to wait before checking the cache for any items that can be purged.
        /// </summary>
        public TimeSpan ServiceManagerRequestsPurgeInterval
        {
            get;
        }
        /// <summary>
        /// If <b>true</b> all remaining cached requests will have their Process executed; otherwise, setting this to <b>false</b> will terminate all remaining cached requests without executing their Process. 
        /// </summary>
        public bool ServiceManagerExecuteRemainingRequestsOnStop
        {
            get;
        }

        // component manager
        /// <summary>
        /// Returns the Component Manager's source id when creating log entries.
        /// </summary>
        public string ComponentManagerLogSourceId
        {
            get;
        }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="ComponentManagement.IComponentManager"/> started.
        /// </summary>
        public DateTimeOffset ComponentManagerStartTime
        {
            get;
        }
        /// <summary>
        /// The duration that the <see cref="ComponentManagement.IComponentManager"/> has run. 
        /// </summary>
        public TimeSpan ComponentManagerRunTime
        {
            get;
        }
        /// <summary>
        /// A collection of folders.
        /// </summary>
        ComponentManagement.IFolderAccessItems ComponentManagerFolderAccessItems
        {
            get;
        }

        // services
        /// <summary>
        /// Contains detailed information about each <see cref="IService"/> in the <see cref="IServiceManager"/>.
        /// </summary>
        public ServiceManagerServices ServiceManagerServices
        {
            get;
        }

        // communication controller
        /// <summary>
        /// The type of the ComponentManager's CommunicationController.
        /// </summary>
        public string CommunicationControllerType
        {
            get;
        }
        /// <summary>
        /// The <see cref="Type"/> representing the client; this <see cref="Type"/> must implement the <see cref="Networking.IClient"/> interface.
        /// </summary>
        public string CommunicationControllerClientType
        {
            get;
        }
        /// <summary>
        /// Returns the Communication Controller's source id when creating log entries.
        /// </summary>
        public string CommunicationControllerLogSourceId
        {
            get;
        }
        /// <summary>
        /// The <see cref="Type"/> representing the server; this <see cref="Type"/> must implement the <see cref="Networking.IServer"/> interface.
        /// </summary>
        public string CommunicationControllerServerType
        {
            get;
        }
        /// <summary>
        /// The Internet Protocol address (IPAddress) in string form.
        /// </summary>
        public string CommunicationControllerIPAddress
        {
            get;
        }
        /// <summary>
        /// The name of the connection for the IPAddress.
        /// </summary>
        public string CommunicationControllerName
        {
            get;
        }
        /// <summary>
        /// The port number for the IPAddress.
        /// </summary>
        public int CommunicationControllerPort
        {
            get;
        }
        /// <summary>
        /// The CommunicationController Server's <see cref="Uri"/>.
        /// </summary>
        public string CommunicationControllerUri
        {
            get;
        }

        // component controller
        /// <summary>
        /// The type of the ComponentManager's ComponentController.
        /// </summary>
        public string ComponentControllerType
        {
            get;
        }
        /// <summary>
        /// The filename of the custom configuration file.
        /// </summary>
        public string ComponentControllerCustomConfigurationFilename
        {
            get;
        }
        /// <summary>
        /// The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.
        /// </summary>
        public string ComponentControllerCustomConfigurationSerializer
        {
            get;
        }
        /// <summary>
        /// Returns the Component Controller's source id when creating log entries.
        /// </summary>
        public string ComponentControllerLogSourceId
        {
            get;
        }

        // install controller
        /// <summary>
        /// The type of the ComponentManager's Installer.
        /// </summary>
        public string InstallerType
        {
            get;
        }
        /// <summary>
        /// Gets the installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstallerNonDependencyPackages
        {
            get;
        }
        /// <summary>
        /// Gets the installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstallerDependencyPackages
        {
            get;
        }
        /// <summary>
        /// Gets the installed packages where the <see cref="Packaging.IPackageConfiguration.IsEnabled"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstallerDisabledPackages
        {
            get;
        }
        /// <summary>
        /// Gets the installed packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b> and the package is not referenced by any other package.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstallerOrphanedDependencyPackages
        {
            get;
        }
        /// <summary>
        /// Gets the count of installed packages that are missing dependency packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstallerPackagesWithMissingDependencies
        {
            get;
        }
        /// <summary>
        /// Returns the Installer's source id when creating log entries.
        /// </summary>
        public string InstallerLogSourceId
        {
            get;
        }

        // log controller
        /// <summary>
        /// The type of the ComponentManager's LogController.
        /// </summary>
        public string LogControllerType
        {
            get;
        }
        /// <summary>
        /// Returns the Log Controller's source id when creating log entries.
        /// </summary>
        public string LogControllerLogSourceId
        {
            get;
        }
        /// <summary>
        /// The number of log entries in the log and the archived logs.
        /// </summary>
        public int LogControllerLogCount
        {
            get;

        }

        // package controller
        /// <summary>
        /// The type of the ComponentManager's PackageProvider.
        /// </summary>
        public string PackageProviderType
        {
            get;
        }
        /// <summary>
        ///  Gets the packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>true</b>.
        /// </summary>
        public IEnumerable<Packaging.IPackage> PackageProviderDependencyPackages
        {
            get;
        }
        /// <summary>
        ///  Gets the packages where the <see cref="Packaging.IPackageConfiguration.IsDependencyPackage"/> flag is <b>false</b>.
        /// </summary>
        public IEnumerable<Packaging.IPackage> PackageProviderNonDependencyPackages
        {
            get;
        }
        /// <summary>
        /// The type of <see cref="FileStorage.ICompressedFileStore"/> used to open packages.
        /// </summary>
        public string PackageProviderPackageFileStoreProvider
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Service Manager Details, ServiceManagerId={ServiceManagerId}, ProcessedRequests={ServiceManagerProcessedRequests:N0}, StartTime={ServiceManagerStartTime}, RunTime={ServiceManagerRunTime}";
        #endregion
    }
}
