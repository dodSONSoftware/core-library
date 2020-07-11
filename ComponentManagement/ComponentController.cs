using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides access to and control of an extension and plugin component control system.
    /// </summary>
    /// <seealso cref="Addon"/>
    /// <seealso cref="IComponent"/>
    /// <seealso cref="ComponentPluginBase"/>
    /// <seealso cref="ComponentExtensionBase"/>
    public class ComponentController
        : IComponentController
    {
        #region Public Constants
        /// <summary>
        /// The file pattern-matching value used in the search for assemblies. Current Value="*.dll"
        /// </summary>
        public static readonly string AssemblyFileSearchPattern = "*.dll";
        /// <summary>
        /// The directory search option used in the search for assemblies. Current Value=System.IO.SearchOption.TopDirectoryOnly
        /// </summary>
        public static readonly System.IO.SearchOption AssemblyDirectorySearchOption = System.IO.SearchOption.TopDirectoryOnly;
        #endregion
        #region Ctor
        private ComponentController()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="ComponentController"/>.
        /// </summary>
        /// <param name="logSourceId">The value used as the source id when creating log entries.</param>
        /// <param name="customConfigurationFilename">The filename of the custom configuration file in the installed package. <b>Null</b> is allowed if no custom configuration file is wanted.</param>
        /// <param name="customConfigurationSerializerType">The <see cref="Type"/> of configuration serializer used to read and write the custom configuration file. <b>Null</b> is allowed if no custom configuration file is wanted.</param>
        public ComponentController(string logSourceId,
                                   string customConfigurationFilename,
                                   Type customConfigurationSerializerType)
            : this()
        {
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
            CustomConfigurationFilename = customConfigurationFilename;
            _CustomConfigurationSerializerType = customConfigurationSerializerType;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public ComponentController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "ComponentController")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"ComponentController\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // LogSourceId
            LogSourceId = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogSourceId", typeof(string)).Value;
            // CustomConfigurationFilename
            try
            {
                CustomConfigurationFilename = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "CustomConfigurationFilename", typeof(string)).Value;
            }
            catch { }
            // CustomConfigurationSerializerType
            try
            {
                _CustomConfigurationSerializerType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "CustomConfigurationSerializerType", typeof(Type)).Value);
            }
            catch { }
        }
        #endregion
        #region Private Fields
        private ILogController _LogController;
        private Installation.IInstaller _Installer;
        private ICommunicationController _CommunicationController;
        private Dictionary<string, ComponentFactory> _ExtensionComponents;
        private Dictionary<string, ComponentFactory> _PluginComponents;
        private readonly Type _CustomConfigurationSerializerType;
        private IFolderAccessItems _FolderAccessItems;
        #endregion
        #region Public Methods
        /// <summary>
        /// Indicates the operating state of the <see cref="IComponentController"/>.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IComponentController"/> started; <see cref="DateTimeOffset.MinValue"/> if never started.
        /// </summary>
        public DateTimeOffset StartTime { get; private set; } = DateTimeOffset.MinValue;
        /// <summary>
        /// The last <see cref="DateTimeOffset"/> when the <see cref="IComponentController"/> stopped; <see cref="DateTimeOffset.MinValue"/> if never stopped.
        /// </summary>
        public DateTimeOffset StopTime { get; private set; } = DateTimeOffset.MinValue;
        /// <summary>
        /// The duration that the <see cref="IComponentController"/> has run. 
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
        /// The filename of the custom configuration file.
        /// </summary>
        public string CustomConfigurationFilename
        {
            get;
        }
        /// <summary>
        /// The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.
        /// </summary>
        public Configuration.IConfigurationSerializer<StringBuilder> CustomConfigurationSerializer
        {
            get
            {
                if (_CustomConfigurationSerializerType != null)
                {
                    return (Common.InstantiationHelper.InvokeDefaultCtor(_CustomConfigurationSerializerType) as Configuration.IConfigurationSerializer<StringBuilder>);
                }
                return null;
            }
        }
        /// <summary>
        /// Extension in the <see cref="IComponentController"/>.
        /// </summary>
        public IEnumerable<ComponentFactory> ExtensionComponents => _ExtensionComponents.Values;
        /// <summary>
        /// Adds a <see cref="ComponentExtensionBase"/>d component to <see cref="ExtensionComponents"/>.
        /// </summary>
        /// <remarks>
        /// Adding a <see cref="ComponentFactory"/> to <see cref="ExtensionComponents"/> list will <i>NOT</i> start the <see cref="ComponentFactory"/>.
        /// Starting, and stopping, the <see cref="ComponentFactory"/> is the responsibility of the calling code.
        /// See the <see cref="ComponentController"/>'s <see cref="Start()"/>, <see cref="Stop()"/> or <see cref="Restart()"/> methods.
        /// It is also possible to start and stop individual <see cref="ComponentFactory"/>s using the <see cref="ComponentFactory"/>'s <see cref="ComponentFactory.Start()"/> and <see cref="ComponentFactory.Stop()"/> methods.
        /// </remarks>
        /// <param name="component">The <see cref="ComponentExtensionBase"/>d component to add.</param>
        /// <returns>The <see cref="ComponentExtensionBase"/>d component added.</returns>
        public ComponentFactory AddExtension(ComponentFactory component)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (_ExtensionComponents.ContainsKey(component.Id))
            {
                throw new Exception($"Extension already exists.");
            }
            component.Initialize(_LogController.LogMarshal, _CommunicationController, component.Id);
            _ExtensionComponents.Add(component.Id, component);
            return component;
        }
        /// <summary>
        /// Plugins in the <see cref="IComponentController"/>.
        /// </summary>
        public IEnumerable<ComponentFactory> PluginComponents => _PluginComponents.Values;
        /// <summary>
        /// Adds a <see cref="ComponentPluginBase"/>d component to <see cref="PluginComponents"/>.
        /// </summary>
        /// <remarks>
        /// Adding a <see cref="ComponentFactory"/> to <see cref="PluginComponents"/> list will <i>NOT</i> start the <see cref="ComponentFactory"/>.
        /// Starting, and stopping, the <see cref="ComponentFactory"/> is the responsibility of the calling code.
        /// See the <see cref="ComponentController"/>'s <see cref="Start()"/>, <see cref="Stop()"/> or <see cref="Restart()"/> methods.
        /// It is also possible to start and stop individual <see cref="ComponentFactory"/>s using the <see cref="ComponentFactory"/>'s <see cref="ComponentFactory.Start()"/> and <see cref="ComponentFactory.Stop()"/> methods.
        /// </remarks>
        /// <param name="component">The <see cref="ComponentPluginBase"/>d component to add.</param>
        /// <returns>The <see cref="ComponentPluginBase"/>d component added.</returns>
        public ComponentFactory AddPlugin(ComponentFactory component)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            if (_PluginComponents.ContainsKey(component.Id))
            {
                throw new Exception($"Plugin already exists.");
            }
            component.Initialize(_LogController.LogMarshal, _CommunicationController, component.Id);
            _PluginComponents.Add(component.Id, component);
            return component;
        }
        /// <summary>
        /// Removes a <see cref="ComponentPluginBase"/>d component, by id, from <see cref="PluginComponents"/>.
        /// </summary>
        /// <remarks>
        /// Removing a <see cref="ComponentFactory"/> from <see cref="PluginComponents"/> will <i>NOT</i> stop the <see cref="ComponentFactory"/>.
        /// Starting, and stopping, the <see cref="ComponentFactory"/> is the responsibility of the calling code.
        /// See the <see cref="ComponentController"/>'s <see cref="Start()"/>, <see cref="Stop()"/> or <see cref="Restart()"/> methods.
        /// It is also possible to start and stop individual <see cref="ComponentFactory"/>s using the <see cref="ComponentFactory"/>'s <see cref="ComponentFactory.Start()"/> and <see cref="ComponentFactory.Stop()"/> methods.
        /// </remarks>
        /// <param name="id">The id of the <see cref="ComponentPluginBase"/>d component to remove.</param>
        /// <returns>The <see cref="ComponentPluginBase"/>d component removed, or <b>null</b> if the component could not be found.</returns>
        public ComponentFactory RemovePlugin(string id)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            if (!_PluginComponents.ContainsKey(id))
            {
                throw new Exception($"Plugin component not found.");
            }
            var temp = _PluginComponents[id];
            _PluginComponents.Remove(id);
            return temp;
        }
        /// <summary>
        /// Prepares the <see cref="IComponentController"/> for operation.
        /// </summary>
        /// <param name="logController">The <see cref="ILogController"/> used to output generated logs.</param>
        /// <param name="communicationController">The <see cref="ICommunicationController"/> used to provide each <see cref="ComponentFactory"/> an <see cref="Networking.IClient"/>.</param>
        /// <param name="installer">The <see cref="Installation.IInstaller"/> used to provide dependency package information for each <see cref="ComponentFactory"/>.</param>
        /// <param name="folderAccessItems">A collection of folders.</param>
        public void Initialize(ILogController logController,
                               ICommunicationController communicationController,
                               Installation.IInstaller installer,
                               IFolderAccessItems folderAccessItems)
        {
            _LogController = logController ?? throw new ArgumentNullException(nameof(logController));
            _CommunicationController = communicationController ?? throw new ArgumentNullException(nameof(communicationController));
            _Installer = installer ?? throw new ArgumentNullException(nameof(installer));
            _FolderAccessItems = folderAccessItems ?? throw new ArgumentNullException(nameof(folderAccessItems));
        }
        /// <summary>
        /// Will scan installed packages searching for any extensions and plugins that are not currently in the <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/> lists and start them.
        /// </summary>
        public void Rescan() => Rescan(TimeSpan.Zero);
        /// <summary>
        /// Will scan installed packages searching for any extensions and plugins that are not currently in the <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/> lists and start them, waiting <paramref name="intraComponentStartupDelay"/> between starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        public void Rescan(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            var startTime = DateTime.Now;
            // find all components
            FindAllComponents(out IList<ComponentFactory> extensionComponents, out IList<ComponentFactory> pluginComponents);
            // isolate new components
            var newExtensionComponents = new Dictionary<string, ComponentFactory>();
            var newPluginComponents = new Dictionary<string, ComponentFactory>();
            foreach (var item in extensionComponents)
            {
                if (!_ExtensionComponents.ContainsKey(item.Id))
                {
                    newExtensionComponents.Add(item.Id, item);
                }
            }
            foreach (var item in pluginComponents)
            {
                if (!_PluginComponents.ContainsKey(item.Id))
                {
                    newPluginComponents.Add(item.Id, item);
                }
            }
            // log results; extensions first
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Rescanning installed packages for currently unloaded extensions and plugins.");
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"New extension components={newExtensionComponents.Count()}");
            var extensionCounter = 0;
            foreach (var item in newExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={item.Value.Id}");
            }
            // log results; plugins
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"New plugin components={newPluginComponents.Count()}");
            var pluginCounter = 0;
            foreach (var item in newPluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={item.Value.Id}");
            }
            // start any new extensions
            foreach (var item in newExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extension Component {item.Value.Id}");
                item.Value.Initialize(_LogController.LogMarshal, _CommunicationController, item.Value.Id);
                item.Value.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Started {item.Value.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
                _ExtensionComponents.Add(item.Value.Id, item.Value);
            }
            // start any new plugins
            foreach (var item in newPluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {item.Value.Id}");
                item.Value.Initialize(_LogController.LogMarshal, _CommunicationController, item.Value.Id);
                item.Value.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {item.Value.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
                _PluginComponents.Add(item.Value.Id, item.Value);
            }
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Rescan Completed. Elapsed Time={DateTimeOffset.Now - startTime}");
        }
        /// <summary>
        /// Starts the <see cref="IComponentController"/>.
        /// </summary>
        public void Start() => Start(TimeSpan.Zero);
        /// <summary>
        /// Starts the <see cref="IComponentController"/> waiting <paramref name="intraComponentStartupDelay"/> between starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        public void Start(TimeSpan intraComponentStartupDelay)
        {
            if (IsRunning)
            {
                throw new Exception("ComponentController is already running.");
            }
            IsRunning = true;
            StartTime = DateTimeOffset.Now;
            // discover components
            var logs = DiscoverComponents();
            // log results; extensions first
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension components={ExtensionComponents.Count()}");
            _LogController.LogMarshal.Write(logs[0]);
            // log results; plugins
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin components={PluginComponents.Count()}");
            _LogController.LogMarshal.Write(logs[1]);
            // start extensions first
            foreach (var extensionComponent in from x in ExtensionComponents
                                               orderby x.PackageConfiguration.Priority ascending
                                               select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extension Component {extensionComponent.Id}");
                extensionComponent.Initialize(_LogController.LogMarshal, _CommunicationController, extensionComponent.Id);
                extensionComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Started {extensionComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // start plugins after extensions
            foreach (var pluginComponent in from x in PluginComponents
                                            orderby x.PackageConfiguration.Priority ascending
                                            select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {pluginComponent.Id}");
                pluginComponent.Initialize(_LogController.LogMarshal, _CommunicationController, pluginComponent.Id);
                pluginComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {pluginComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Will start all stopped extensions and plugins.
        /// </summary>
        public void StartAllStoppedComponents() => StartAllStoppedComponents(TimeSpan.Zero);
        /// <summary>
        /// Will start all stopped extensions and plugins waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        public void StartAllStoppedComponents(TimeSpan intraComponentStartupDelay)
        {
            StartAllStoppedExtensions(intraComponentStartupDelay);
            StartAllStoppedPlugins(intraComponentStartupDelay);
        }
        /// <summary>
        /// Will start all stopped extensions waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        public void StartAllStoppedExtensions(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController must be running.");
            }
            //
            var startTime = DateTime.Now;
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extensions.");
            // start extensions first
            foreach (var extensionComponent in from x in ExtensionComponents
                                               orderby x.PackageConfiguration.Priority ascending
                                               select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extension Component {extensionComponent.Id}");
                extensionComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Started {extensionComponent.Id}");
                Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            //
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extensions Completed. Elapsed Time={DateTimeOffset.Now - startTime}");
        }
        /// <summary>
        /// Will start all stopped plugins waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        public void StartAllStoppedPlugins(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController must be running.");
            }
            //
            var startTime = DateTime.Now;
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugins.");
            // start plugins after extensions
            foreach (var pluginComponent in from x in PluginComponents
                                            orderby x.PackageConfiguration.Priority ascending
                                            select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {pluginComponent.Id}");
                pluginComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {pluginComponent.Id}");
                Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            //
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugins Completed. Elapsed Time={DateTimeOffset.Now - startTime}");
        }
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/>.
        /// </summary>
        public void Restart() => Restart(TimeSpan.Zero);
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        public void Restart(TimeSpan intraComponentStartupDelay)
        {
            // FYI: I decided NOT to execute the RestartAllExtenstions(...) and RestartAllPlugins(...) functions. This allows for better control and better looking logs.

            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            _LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Restarting all Extensions and Plugins");
            // #### stop all plugins and extensions
            // log results; plugins first
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin components={PluginComponents.Count()}");
            var pluginCounter = 0;
            foreach (var item in PluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={item.Id}");
            }
            // log results; extensions last
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension components={ExtensionComponents.Count()}");
            var extensionCounter = 0;
            foreach (var item in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={item.Id}");
            }
            // stop plugins first
            foreach (var pluginComponent in PluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Plugin Component {pluginComponent.Id}");
                pluginComponent.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Stopped {pluginComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // stop extensions last
            foreach (var extensionComponent in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Extension Component {extensionComponent.Id}");
                extensionComponent.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Stopped {extensionComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // #### start all extensions and plugins
            // start extensions first
            foreach (var extensionComponent in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extension Component {extensionComponent.Id}");
                extensionComponent.Initialize(_LogController.LogMarshal, _CommunicationController, extensionComponent.Id);
                extensionComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Started {extensionComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // start plugins after extensions
            foreach (var pluginComponent in PluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {pluginComponent.Id}");
                pluginComponent.Initialize(_LogController.LogMarshal, _CommunicationController, pluginComponent.Id);
                pluginComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {pluginComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        public void RestartAllExtenstions(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            _LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Restarting all Extensions");
            // log results
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension components={ExtensionComponents.Count()}");
            var extensionCounter = 0;
            foreach (var item in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={item.Id}");
            }
            // stop all extensions 
            foreach (var extensionComponent in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Extension Component {extensionComponent.Id}");
                extensionComponent.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Stopped {extensionComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // start extensions
            foreach (var extensionComponent in ExtensionComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Extension Component {extensionComponent.Id}");
                extensionComponent.Initialize(_LogController.LogMarshal, _CommunicationController, extensionComponent.Id);
                extensionComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Started {extensionComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="PluginComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        public void RestartAllPlugins(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            _LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Restarting all Plugins");
            // log results
            _LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin components={PluginComponents.Count()}");
            var pluginCounter = 0;
            foreach (var item in PluginComponents)
            {
                _LogController.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={item.Id}");
            }
            // stop all plugins
            foreach (var pluginComponent in PluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Plugin Component {pluginComponent.Id}");
                pluginComponent.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Stopped {pluginComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // start all plugins
            foreach (var pluginComponent in PluginComponents)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {pluginComponent.Id}");
                pluginComponent.Initialize(_LogController.LogMarshal, _CommunicationController, pluginComponent.Id);
                pluginComponent.Start();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {pluginComponent.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Stops the <see cref="IComponentController"/>.
        /// </summary>
        public void Stop() => Stop(TimeSpan.Zero);
        /// <summary>
        /// Stops the <see cref="IComponentController"/> waiting <paramref name="intraComponentStartupDelay"/> between stopping each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each <see cref="ComponentFactory"/>.</param>
        public void Stop(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            IsRunning = false;
            StopTime = DateTimeOffset.Now;
            // log results; plugins first
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin components={PluginComponents.Count()}");
            var pluginCounter = 0;
            foreach (var item in from x in PluginComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={item.Id}");
            }
            // log results; extensions last
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension components={ExtensionComponents.Count()}");
            var extensionCounter = 0;
            foreach (var item in from x in ExtensionComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={item.Id}");
            }
            // stop plugins first
            foreach (var item in from x in PluginComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Plugin Component {item.Id}");
                item.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Stopped {item.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // stop extensions last
            foreach (var item in from x in ExtensionComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Extension Component {item.Id}");
                item.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Stopped {item.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Stops all extensions and plugins waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each component.</param>
        public void StopAllComponents(TimeSpan intraComponentStartupDelay)
        {
            StopAllExtensions(intraComponentStartupDelay);
            StopAllPlugins(intraComponentStartupDelay, true);
        }
        /// <summary>
        /// Stops all extensions waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each <see cref="ExtensionComponents"/>.</param>
        public void StopAllExtensions(TimeSpan intraComponentStartupDelay)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            // log results; extensions
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension components={ExtensionComponents.Count()}");
            var extensionCounter = 0;
            foreach (var item in from x in ExtensionComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={item.Id}");
            }
            // stop extensions
            foreach (var item in from x in ExtensionComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Extension Component {item.Id}");
                item.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Extension Component Stopped {item.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
        }
        /// <summary>
        /// Stops all plugins waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each <see cref="PluginComponents"/>.</param>
        /// <param name="removeAfterStopping">Indicates whether the plugin will be removed from the system. <b>True</b> will remove the plugin; otherwise, <b>false</b> will leave the pluging in the system.</param>
        public void StopAllPlugins(TimeSpan intraComponentStartupDelay, bool removeAfterStopping)
        {
            if (!IsRunning)
            {
                throw new Exception("ComponentController is not running.");
            }
            // log results; plugins
            _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin components={PluginComponents.Count()}");
            var pluginCounter = 0;
            foreach (var item in from x in PluginComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={item.Id}");
            }
            // stop plugins first
            var list = new List<ComponentFactory>();
            foreach (var item in from x in PluginComponents
                                 orderby x.PackageConfiguration.Priority descending
                                 select x)
            {
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Stopping Plugin Component {item.Id}");
                list.Add(item);
                item.Stop();
                _LogController.LogMarshal.Write(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Stopped {item.Id}");
                dodSON.Core.Threading.ThreadingHelper.Sleep(intraComponentStartupDelay);
            }
            // remove plugins
            if (removeAfterStopping)
            {
                foreach (var item in list)
                {
                    RemovePlugin(item.Id);
                }
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
                var result = new Configuration.ConfigurationGroup("ComponentController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // LogSourceId
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());
                // CustomConfigurationFilename
                if (!string.IsNullOrWhiteSpace(CustomConfigurationFilename))
                {
                    result.Items.Add("CustomConfigurationFilename", CustomConfigurationFilename, CustomConfigurationFilename.GetType());
                }
                // CustomConfigurationSerializerType
                if (_CustomConfigurationSerializerType != null)
                {
                    result.Items.Add("CustomConfigurationSerializerType", _CustomConfigurationSerializerType, _CustomConfigurationSerializerType.GetType());
                }
                return result;
            }
        }
        #endregion
        #region Private Methods
        private Logging.Logs[] DiscoverComponents()
        {
            // clear lists
            _ExtensionComponents = new Dictionary<string, ComponentFactory>();
            _PluginComponents = new Dictionary<string, ComponentFactory>();
            // create logs
            var extensionLogs = new Logging.Logs();
            var pluginLogs = new Logging.Logs();
            // find all components
            FindAllComponents(out IList<ComponentFactory> extensionComponents, out IList<ComponentFactory> pluginComponents);
            // process extensions
            var extensionCounter = 0;
            foreach (var component in from x in extensionComponents
                                      orderby x.PackageConfiguration.Priority ascending
                                      select x)
            {
                _ExtensionComponents.Add(component.Id, component);
                extensionLogs.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++extensionCounter}={component.Id}");
            }
            // process plugins
            var pluginCounter = 0;
            foreach (var component in from x in pluginComponents
                                      orderby x.PackageConfiguration.Priority ascending
                                      select x)
            {
                _PluginComponents.Add(component.Id, component);
                pluginLogs.Add(Logging.LogEntryType.Information, LogSourceId, $"#{++pluginCounter}={component.Id}");
            }
            // return logs
            return new Logging.Logs[] { extensionLogs, pluginLogs };
        }

        private void FindAllComponents(out IList<ComponentFactory> extensionComponents,
                                       out IList<ComponentFactory> pluginComponents)
        {
            // clear lists
            extensionComponents = new List<ComponentFactory>();
            pluginComponents = new List<ComponentFactory>();
            // types to search for
            var typeNames = new List<string>() { typeof(ComponentPluginBase).AssemblyQualifiedName,
                                                 typeof(ComponentExtensionBase).AssemblyQualifiedName
                                               };
            // for each INSTALLED, ENABLED, NON-DEPENDENCY package
            foreach (var installedPackage in from x in _Installer.InstalledNonDependencyPackages
                                             where x.Package.PackageConfiguration.IsEnabled
                                             select x)
            {
                string privateBinPath = DependencyChainToString(_Installer.DependencyChain(installedPackage));
                var state = Tuple.Create(installedPackage.InstallPath,
                                         AssemblyFileSearchPattern,
                                         AssemblyDirectorySearchOption,
                                         typeNames,
                                         privateBinPath,
                                         LogSourceId,
                                         CustomConfigurationFilename);
                var dude = AppDomain.AppDomainHelper.Execute(privateBinPath,
                                                             state,
                                                             (stateObj) =>
                                                             {
                                                                 // #### executed in a separate Application Domain ####
                                                                 var infoState = stateObj as Tuple<string, string, System.IO.SearchOption, List<string>, string, string, string>;
                                                                 var results = new Dictionary<string, List<Tuple<string, AppDomain.TypeProxySettings, StringBuilder, string>>>();
                                                                 // look for custom configuration file
                                                                 var customConfigurationErrorStr = "";
                                                                 var customConfigFilename = System.IO.Path.Combine(infoState.Item1, infoState.Item7);
                                                                 StringBuilder customConfigurationStrBuilder = null;
                                                                 if (System.IO.File.Exists(customConfigFilename))
                                                                 {
                                                                     try
                                                                     {
                                                                         customConfigurationStrBuilder = new StringBuilder(System.IO.File.ReadAllText(customConfigFilename));
                                                                     }
                                                                     catch (Exception ex)
                                                                     {
                                                                         customConfigurationErrorStr = $"Cannot read custom configuration file. File={customConfigFilename}, Exception={ex.Message}";
                                                                     }
                                                                 }
                                                                 // process each file matching the AssemblyFileSearchPattern pattern and within the restrictions of the AssemblyDirectorySearchOption
                                                                 foreach (var filename in System.IO.Directory.GetFiles(infoState.Item1, infoState.Item2, infoState.Item3))
                                                                 {
                                                                     try
                                                                     {
                                                                         // load assembly
                                                                         var assembly = System.Reflection.Assembly.LoadFile(filename);
                                                                         if (assembly != null)
                                                                         {
                                                                             // search each exported type
                                                                             foreach (var exportType in assembly.GetExportedTypes())
                                                                             {
                                                                                 // search for the known types
                                                                                 foreach (var typeName in infoState.Item4)
                                                                                 {
                                                                                     // process only PUBLIC, NON-ABSTRACT, CLASSES that implement the KNOWN TYPE (exportType)
                                                                                     var searchType = Type.GetType(typeName);
                                                                                     if ((exportType.IsPublic) &&
                                                                                         (!exportType.IsAbstract) &&
                                                                                         (exportType.IsClass) &&
                                                                                         (searchType.IsAssignableFrom(exportType)))
                                                                                     {
                                                                                         // create type proxy settings
                                                                                         var typeProxySettings = new AppDomain.TypeProxySettings(exportType.AssemblyQualifiedName,
                                                                                                                                                 filename,
                                                                                                                                                 null,
                                                                                                                                                 System.IO.Path.GetDirectoryName(filename),
                                                                                                                                                 infoState.Item5);
                                                                                         // add to results
                                                                                         if (!results.ContainsKey(typeName))
                                                                                         {
                                                                                             results.Add(typeName, new List<Tuple<string, AppDomain.TypeProxySettings, StringBuilder, string>>()
                                                                                             { Tuple.Create(infoState.Item6, typeProxySettings, customConfigurationStrBuilder, customConfigurationErrorStr) });
                                                                                         }
                                                                                         else
                                                                                         {
                                                                                             results[typeName].Add(Tuple.Create(infoState.Item6, typeProxySettings, customConfigurationStrBuilder, customConfigurationErrorStr));
                                                                                         }
                                                                                     }
                                                                                 }
                                                                             }
                                                                         }
                                                                     }
                                                                     catch { }
                                                                 }
                                                                 // 
                                                                 return results;
                                                                 // ########
                                                             }, out Exception executeException);
                if (dude != null)
                {
                    if (dude is Dictionary<string, List<Tuple<string, AppDomain.TypeProxySettings, StringBuilder, string>>> dict)
                    {
                        if (dict.ContainsKey(typeof(ComponentExtensionBase).AssemblyQualifiedName))
                        {
                            // process extensions first
                            foreach (var item in dict[typeof(ComponentExtensionBase).AssemblyQualifiedName])
                            {
                                if (!string.IsNullOrWhiteSpace(item.Item4))
                                {
                                    _LogController.Write(Logging.LogEntryType.Error, LogSourceId, item.Item4);
                                }
                                else
                                {
                                    Core.Configuration.IConfigurationGroup customConfiguration = null;
                                    if (item.Item3 != null)
                                    {
                                        customConfiguration = CustomConfigurationSerializer?.Deserialize(item.Item3);
                                    }
                                    extensionComponents.Add(new ComponentFactory(Common.TypeHelper.FormatId(item.Item2.TypeName, item.Item2.AssemblyFilename, _Installer.InstallationPath, installedPackage.InstallPath),
                                                                                 new Addon.ExtensionFactory(item.Item2.TypeName,
                                                                                                            item.Item2.AssemblyFilename,
                                                                                                            System.IO.Path.GetDirectoryName(item.Item2.AssemblyFilename),
                                                                                                            item.Item2.SetupInfo.PrivateBinPath),
                                                                                 installedPackage.Package.PackageConfiguration,
                                                                                 customConfiguration,
                                                                                 _Installer.InstallationPath,
                                                                                 installedPackage.InstallPath,
                                                                                 GetTemporaryPath(item),
                                                                                 GetLongTermStoragePath(item),
                                                                                 _FolderAccessItems));
                                }
                            }
                        }
                        if (dict.ContainsKey(typeof(ComponentPluginBase).AssemblyQualifiedName))
                        {
                            // process plugins after extensions
                            foreach (var item in dict[typeof(ComponentPluginBase).AssemblyQualifiedName])
                            {
                                if (!string.IsNullOrWhiteSpace(item.Item4))
                                {
                                    _LogController.Write(Logging.LogEntryType.Error, LogSourceId, item.Item4);
                                }
                                else
                                {
                                    Core.Configuration.IConfigurationGroup customConfiguration = null;
                                    if (item.Item3 != null)
                                    {
                                        customConfiguration = CustomConfigurationSerializer?.Deserialize(item.Item3);
                                    }
                                    pluginComponents.Add(new ComponentFactory(Common.TypeHelper.FormatId(item.Item2.TypeName, item.Item2.AssemblyFilename, _Installer.InstallationPath, installedPackage.InstallPath),
                                                                              new Addon.PluginFactory(item.Item2),
                                                                              installedPackage.Package.PackageConfiguration,
                                                                              customConfiguration,
                                                                              _Installer.InstallationPath,
                                                                              installedPackage.InstallPath,
                                                                              GetTemporaryPath(item),
                                                                              GetLongTermStoragePath(item),
                                                                              _FolderAccessItems));
                                }
                            }
                        }
                    }
                }
            }

            // ########################
            // Internal Functions

            string DependencyChainToString(IEnumerable<Installation.IInstalledPackage> collection)
            {
                var result = new StringBuilder(512);
                foreach (var item in collection)
                {
                    result.Append(item.InstallPath + ";");
                }
                if (result.Length > 0)
                {
                    --result.Length;
                }
                return result.ToString();
            }
        }


        // TODO: make these get information from a ( FileStorageController )

        private string GetTemporaryPath(Tuple<string, dodSON.Core.AppDomain.TypeProxySettings, StringBuilder, string> item)
        {
            return System.IO.Path.GetTempPath();
        }

        private string GetLongTermStoragePath(Tuple<string, dodSON.Core.AppDomain.TypeProxySettings, StringBuilder, string> item)
        {
            return @"C:\";
        }
        #endregion
    }
}
