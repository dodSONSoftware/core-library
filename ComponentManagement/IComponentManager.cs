using System;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines access to and control of a component management system.
    /// </summary>
    /// <seealso cref="Packaging"/>
    /// <seealso cref="Installation"/>
    /// <seealso cref="Logging"/>
    /// <seealso cref="Networking"/>
    /// <seealso cref="Addon"/>
    /// <seealso cref="ComponentExtensionBase"/>
    /// <seealso cref="ComponentPluginBase"/>
    public interface IComponentManager
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Indicates the operating state of the <see cref="IComponentManager"/>.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IComponentManager"/> started.
        /// </summary>
        DateTimeOffset StartTime { get; }
        /// <summary>
        /// The duration that the <see cref="IComponentManager"/> has run. 
        /// </summary>
        TimeSpan RunTime { get; }
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId { get; }
        /// <summary>
        /// Initializes and prepares all sub-systems, finds and starts all <see cref="ComponentExtensionBase"/> and <see cref="ComponentPluginBase"/> components.
        /// </summary>
        /// <param name="clearLogBeforeStarting">Determines if the logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        /// <param name="clearArchivedLogsBeforeStarting">Determines if the archived logs should be cleared before starting the <see cref="ComponentManager"/>.</param>
        void Start(bool clearLogBeforeStarting, bool clearArchivedLogsBeforeStarting);
        /// <summary>
        /// Will shutdown all <see cref="ComponentExtensionBase"/> and <see cref="ComponentPluginBase"/> components and terminate all sub-systems.
        /// </summary>
        void Stop();
        /// <summary>
        /// Provides access and controls to a configurable, single-entry, memory-cached, archive-enabled logging mechanism.
        /// </summary>
        ILogController LogController { get; }
        /// <summary>
        /// Provides access and controls to a configurable, type-based messaging communication system.
        /// </summary>
        ICommunicationController CommunicationController { get; }
        /// <summary>
        /// The <see cref="Networking.IServer"/> used by this <see cref="IComponentManager"/> for it's intra-component communication system.
        /// </summary>
        Networking.IServer MainServer { get; }
        /// <summary>
        /// Provides methods to create and connect to packages, iterate packages, find packages and read and write package configuration files.
        /// </summary>
        Packaging.IPackageProvider PackageProvider { get; }
        /// <summary>
        /// Provides settings needed to determine how to install packages.
        /// </summary>
        Installation.InstallationSettings InstallationSettings { get; }
        /// <summary>
        /// Defines methods to install and uninstall packages, iterate installed packages, find installed packages and read and write installed package configuration files.
        /// </summary>
        Installation.IInstaller Installer { get; }
        /// <summary>
        /// Provides access and controls to an extension and plugin component control system.
        /// </summary>
        IComponentController ComponentController { get; }
        /// <summary>
        /// A collection of folders.
        /// </summary>
        IFolderAccessItems FolderAccessItems { get; }
    }
}
