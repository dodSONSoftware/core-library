using System;
using System.Text;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines common controls for <see cref="ComponentPluginBase"/>d and <see cref="ComponentExtensionBase"/>d base types.
    /// </summary>
    /// <seealso cref="ComponentPluginBase"/>
    /// <seealso cref="ComponentExtensionBase"/>
    public interface IComponent
        : Addon.IAddon
    {
        /// <summary>
        /// The id for this component. This value should be unique.
        /// </summary>
        string Id
        {
            get;
        }
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> name for this component. 
        /// </summary>
        string FullyQualifiedName
        {
            get;
        }
        /// <summary>
        /// The functional name of the <see cref="IComponent"/>; this should reflect the basic functionally of the component for a group a similar components.
        /// <para/>
        /// Multiple <see cref="IComponent"/>s can, and should, use the same <see cref="ComponentDesignation"/> to facilitate finding <see cref="IComponent"/>s on the network based on their function.
        /// Setting this to null, or an empty string, will opt out of the Component Designation Discovery system.
        /// </summary>
        /// <see cref="ComponentManagementHelper.DiscoverComponents(Networking.IClient, string, TimeSpan, System.Threading.CancellationToken, out bool, out string)"/>
        string ComponentDesignation
        {
            get;
        }
        /// <summary>
        /// The specific name for the component.
        /// <para/>
        /// This should be unique for each <see cref="IComponent"/> to help differentiated between <see cref="IComponent"/>s with the same <see cref="ComponentDesignation"/>.
        /// Setting this to null, or an empty string, will opt out of the Component Designation Discovery system.
        /// </summary>
        string ComponentName
        {
            get;
        }
        /// <summary>
        /// The value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IClient"/> used to communicate with the network.
        /// </summary>
        Networking.IClient Client
        {
            get;
        }
        /// <summary>
        /// A <see cref="Networking.IClient"/> made for temporary, unrestricted, communications. This client is not opened by default and should be closed when not being used.
        /// </summary>
        Networking.IClient WorkingClient
        {
            get;
        }
        /// <summary>
        /// A <see cref="Networking.IClientConfiguration"/> used when creating the <see cref="WorkingClient"/>.
        /// </summary>
        Networking.IClientConfiguration WorkingClientConfiguration
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IClientConfiguration"/> for this component.
        /// </summary>
        Networking.IClientConfiguration ClientConfiguration
        {
            get;
        }
        /// <summary>
        /// The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.
        /// </summary>
        Packaging.IPackageConfiguration PackageConfiguration
        {
            get;
        }
        /// <summary>
        /// A custom <see cref="Configuration.IConfigurationGroup"/> found in the installed package.
        /// </summary>
        Configuration.IConfigurationGroup CustomConfiguration
        {
            get;
        }
        /// <summary>
        /// The root path where the <see cref="Installation.IInstaller"/> installs packages.
        /// </summary>
        string InstallRootPath
        {
            get;
        }
        /// <summary>
        /// The install directory path where this component's package has been installed.
        /// </summary>
        string InstallPath
        {
            get;
        }
        /// <summary>
        /// A temporary folder for the <see cref="IComponent"/> to use.
        /// This folder, and all of its contents, will be deleted when the <see cref="IComponent"/> is stopped.
        /// </summary>
        string TemporaryPath
        {
            get;
        }
        /// <summary>
        /// A permanent folder for the <see cref="IComponent"/> to use.
        /// This folder, and any of its contents, will remain intact between starting and stopping of this <see cref="IComponent"/>.
        /// </summary>
        string LongTermStoragePath
        {
            get;
        }
        /// <summary>
        /// A collection of folders.
        /// </summary>
        IFolderAccessItems FolderAccessItems
        {
            get;
        }
        /// <summary>
        /// Prepares the component to be handled by the <see cref="ComponentController"/>.
        /// </summary>
        /// <param name="logMarshal">The <see cref="LogMarshal"/> provided by the <see cref="LogController"/>.</param>
        /// <param name="client">The <see cref="Networking.IClient"/> used by this component.</param>
        /// <param name="workingClient">A <see cref="Networking.IClient"/> made for temporary, unrestricted, communications. This client is not opened by default and should be closed when not being used.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="networkLogSourceId">The value used as the source id when creating network specific log entries.</param>
        /// <param name="packageConfiguration">The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.</param>
        /// <param name="customConfiguration">The custom configuration, if found; otherwise, null.</param>
        /// <param name="installRootPath">The root path where the <see cref="Installation.IInstaller"/> installs packages.</param>
        /// <param name="installPath">The install directory path where this component's package has been installed.</param>
        /// <param name="temporaryPath">A temporary folder for the <see cref="IComponent"/> to use.</param>
        /// <param name="longTermStoragePath">A permanent folder for the <see cref="IComponent"/> to use.</param>
        /// <param name="folderAccessItems">A collection of folders.</param>
        void Initialize(LogMarshal logMarshal,
                        Networking.IClient client,
                        Networking.IClient workingClient,
                        string logSourceId,
                        string networkLogSourceId,
                        Packaging.IPackageConfiguration packageConfiguration,
                        Configuration.IConfigurationGroup customConfiguration,
                        string installRootPath,
                        string installPath,
                        string temporaryPath,
                        string longTermStoragePath,
                        IFolderAccessItems folderAccessItems);
        /// <summary>
        /// Used to write logs into the logging system.
        /// </summary>
        Logging.ILogWriter Log
        {
            get;
        }
        /// <summary>
        /// Used to send a <see cref="Networking.IMessage"/>s into the communication system.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> contained in the <paramref name="payload"/>.</typeparam>
        /// <param name="clientTargetId">The id of the <see cref="Networking.IClient"/> to receive the message; <b>null</b> to send to message to every <see cref="Networking.IClient"/>.</param>
        /// <param name="payload">The <see cref="Type"/> converted into a byte array.</param>
        void SendMessage<T>(string clientTargetId, T payload);
        /// <summary>
        /// Used to send a <see cref="Networking.IMessage"/> into the communication system.
        /// </summary>
        /// <param name="message">The <see cref="Networking.IMessage"/> to send.</param>
        void SendMessage(Networking.IMessage message);
    }
}
