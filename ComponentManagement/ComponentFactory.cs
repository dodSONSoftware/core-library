using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides access to and control of an <see cref="IComponent"/> type; which could exist either in a separate <see cref="System.AppDomain"/> or in the same <see cref="System.AppDomain"/> as it's host.
    /// </summary>
    /// <seealso cref="Addon"/>
    /// <seealso cref="ComponentExtensionBase"/>
    /// <seealso cref="ComponentPluginBase"/>
    public class ComponentFactory
    {
        #region Ctor
        private ComponentFactory()
        {
        }
        /// <summary>
        /// Instantiates a new instance of a <see cref="ComponentFactory"/> with the given id and <see cref="Addon.IAddonFactory"/>.
        /// </summary>
        /// <param name="id">The unique id representing this <see cref="ComponentFactory"/>.</param>
        /// <param name="addonFactory">The <see cref="Addon.IAddonFactory"/> needed to create and control this extension, or plugin.</param>
        /// <param name="packageConfiguration">The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.</param>
        /// <param name="customConfiguration">The custom configuration, if found; otherwise, null.</param>
        /// <param name="installationRootPath">The root path where the <see cref="Installation.IInstaller"/> installs packages.</param>
        /// <param name="installPath">The install directory path where this component's package has been installed.</param>
        /// <param name="temporaryPath">A temporary folder for the <see cref="IComponent"/> to use.</param>
        /// <param name="longTermStoragePath">A permanent folder for the <see cref="IComponent"/> to use.</param>
        /// <param name="folderAccessItems">A collection of folders.</param>
        public ComponentFactory(string id,
                                Addon.IAddonFactory addonFactory,
                                Packaging.IPackageConfiguration packageConfiguration,
                                Configuration.IConfigurationGroup customConfiguration,
                                string installationRootPath,
                                string installPath,
                                string temporaryPath,
                                string longTermStoragePath,
                                IFolderAccessItems folderAccessItems) : this()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
            _AddonFactory = addonFactory ?? throw new ArgumentNullException(nameof(addonFactory));
            PackageConfiguration = packageConfiguration ?? throw new ArgumentNullException(nameof(packageConfiguration));
            CustomConfiguration = customConfiguration;
            //
            if (string.IsNullOrWhiteSpace(installationRootPath))
            {
                throw new ArgumentNullException(nameof(installationRootPath));
            }
            _InstallationRootPath = installationRootPath;
            //
            if (string.IsNullOrWhiteSpace(installPath))
            {
                throw new ArgumentNullException(nameof(installPath));
            }
            _InstallPath = installPath;
            //
            if (string.IsNullOrWhiteSpace(temporaryPath))
            {
                throw new ArgumentNullException(nameof(temporaryPath));
            }
            _TemporaryPath = temporaryPath;
            //
            if (string.IsNullOrWhiteSpace(longTermStoragePath))
            {
                throw new ArgumentNullException(nameof(longTermStoragePath));
            }
            _LongTermStoragePath = longTermStoragePath;
            //
            _FolderAccessItems = folderAccessItems ?? throw new ArgumentNullException(nameof(folderAccessItems));
        }
        #endregion
        #region Private Fields
        private LogMarshal _LogMarshal;
        private ICommunicationController _CommunicationController;
        private string _LogSourceId;
        private Addon.IAddonFactory _AddonFactory;
        private readonly string _InstallationRootPath;
        private readonly string _InstallPath;
        private readonly string _TemporaryPath;
        private readonly string _LongTermStoragePath;
        private readonly IFolderAccessItems _FolderAccessItems;
        #endregion
        #region Public Methods
        /// <summary>
        /// The unique id representing this <see cref="ComponentFactory"/>.
        /// </summary>
        public string Id
        {
            get;
        }
        /// <summary>
        /// Returns whether the <see cref="Addon.IAddon"/> is running. <b>True</b> indicates the <see cref="Addon.IAddonFactory.Addon"/> is loaded and running; otherwise, it returns <b>false</b>.
        /// </summary>
        public bool IsRunning => ((_AddonFactory.IsLoaded) && (_AddonFactory.Addon.IsRunning));
        /// <summary>
        /// Gets the <see cref="IComponent"/> from the <see cref="ComponentFactory"/> if the <see cref="IComponent"/> is running; otherwise, it will return <b>null</b>.
        /// </summary>
        public IComponent Component
        {
            get
            {
                if (IsRunning)
                {
                    return (_AddonFactory.Addon as IComponent);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// The fully-qualified name for this component. This value should be unique.
        /// </summary>
        public string FullyQualifiedName
        {
            get
            {
                if (_AddonFactory.IsLoaded)
                {
                    return ((IComponent)_AddonFactory.Addon).FullyQualifiedName;
                }
                else
                {
                    throw new Exception("Component not loaded.");
                }
            }
        }
        /// <summary>
        /// The <see cref="Networking.IClientConfiguration"/> for this component.
        /// </summary>
        public Networking.IClientConfiguration ClientConfiguration
        {
            get
            {
                if (_AddonFactory.IsLoaded)
                {
                    return ((IComponent)_AddonFactory.Addon).ClientConfiguration;
                }
                else
                {
                    throw new Exception("Component not loaded.");
                }
            }
        }
        /// <summary>
        /// The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.
        /// </summary>
        public Packaging.IPackageConfiguration PackageConfiguration
        {
            get;
        }
        /// <summary>
        /// A custom <see cref="Configuration.IConfigurationGroup"/> found in the installed package.
        /// </summary>
        public Configuration.IConfigurationGroup CustomConfiguration
        {
            get;
        }
        /// <summary>
        /// Initializes a component.
        /// </summary>
        /// <param name="logMarshal">The <see cref="LogMarshal"/> provided by the <see cref="LogController"/>.</param>
        /// <param name="communicationController">The <see cref="ICommunicationController"/> used by this component to create <see cref="Networking.IClient"/>s.</param>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        public void Initialize(LogMarshal logMarshal,
                               ICommunicationController communicationController,
                               string logSourceId)
        {
            _LogMarshal = logMarshal ?? throw new ArgumentNullException(nameof(logMarshal));
            _CommunicationController = communicationController ?? throw new ArgumentNullException(nameof(communicationController));
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            _LogSourceId = logSourceId;
        }
        /// <summary>
        /// Starts a component.
        /// </summary>
        public void Start()
        {
            _AddonFactory.Load();
            if (_AddonFactory.IsLoaded)
            {
                var plugin = (_AddonFactory.Addon as IComponent);
                var pluginClient = _CommunicationController.CreateSharedClient(plugin.ClientConfiguration);
                //var tempId = plugin.FullyQualifiedName.Split(',')[0];
                var temporaryWorkingClient = _CommunicationController.CreateSharedClient(plugin.WorkingClientConfiguration);
                plugin.Initialize(_LogMarshal,
                                  pluginClient,
                                  temporaryWorkingClient,
                                  _LogSourceId,
                                  _CommunicationController.LogSourceId,
                                  PackageConfiguration,
                                  CustomConfiguration,
                                  _InstallationRootPath,
                                  _InstallPath,
                                  _TemporaryPath,
                                  _LongTermStoragePath,
                                  _FolderAccessItems);
                plugin.Start();
            }
        }
        /// <summary>
        /// Stops a component.
        /// </summary>
        public void Stop()
        {
            if (_AddonFactory.IsLoaded)
            {
                _AddonFactory.Addon.Stop();
                _AddonFactory.Unload();
            }
        }
        #endregion
    }
}
