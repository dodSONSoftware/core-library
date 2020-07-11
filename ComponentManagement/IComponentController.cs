using System;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines access to and control of an extension and plugin component control system.
    /// </summary>
    /// <seealso cref="Addon"/>
    /// <seealso cref="IComponent"/>
    /// <seealso cref="ComponentPluginBase"/>
    /// <seealso cref="ComponentExtensionBase"/>
    public interface IComponentController
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Indicates the operating state of the <see cref="IComponentController"/>.
        /// </summary>
        bool IsRunning
        {
            get;
        }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="IComponentController"/> started; <see cref="DateTimeOffset.MinValue"/> if never started.
        /// </summary>
        DateTimeOffset StartTime
        {
            get;
        }
        /// <summary>
        /// The last <see cref="DateTimeOffset"/> when the <see cref="IComponentController"/> stopped; <see cref="DateTimeOffset.MinValue"/> if never stopped.
        /// </summary>
        DateTimeOffset StopTime
        {
            get;
        }
        /// <summary>
        /// The duration that the <see cref="IComponentController"/> has run. 
        /// </summary>
        TimeSpan RunTime
        {
            get;
        }
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId
        {
            get;
        }
        /// <summary>
        /// The filename of the custom configuration file.
        /// </summary>
        string CustomConfigurationFilename
        {
            get;
        }
        /// <summary>
        /// The <see cref="Core.Configuration.IConfigurationSerializer{StringBuilder}"/> used to serialize and deserialize the custom configuration file.
        /// </summary>
        Configuration.IConfigurationSerializer<StringBuilder> CustomConfigurationSerializer
        {
            get;
        }
        /// <summary>
        /// Extension in the <see cref="IComponentController"/>.
        /// </summary>
        IEnumerable<ComponentFactory> ExtensionComponents
        {
            get;
        }
        /// <summary>
        /// Adds a <see cref="ComponentExtensionBase"/>d component to <see cref="ExtensionComponents"/>.
        /// </summary>
        /// <param name="component">The <see cref="ComponentExtensionBase"/>d component to add.</param>
        /// <returns>The <see cref="ComponentExtensionBase"/>d component added.</returns>
        ComponentFactory AddExtension(ComponentFactory component);
        /// <summary>
        /// Plugins in the <see cref="IComponentController"/>.
        /// </summary>
        IEnumerable<ComponentFactory> PluginComponents
        {
            get;
        }
        /// <summary>
        /// Adds a <see cref="ComponentPluginBase"/>d component to <see cref="PluginComponents"/>.
        /// </summary>
        /// <param name="component">The <see cref="ComponentPluginBase"/>d component to add.</param>
        /// <returns>The <see cref="ComponentPluginBase"/>d component added.</returns>
        ComponentFactory AddPlugin(ComponentFactory component);
        /// <summary>
        /// Removes a <see cref="ComponentPluginBase"/>d component, by id, from <see cref="PluginComponents"/>.
        /// </summary>
        /// <param name="id">The id of the <see cref="ComponentPluginBase"/>d component to remove.</param>
        /// <returns>The <see cref="ComponentPluginBase"/>d component removed, or <b>null</b> if the component could not be found.</returns>
        ComponentFactory RemovePlugin(string id);
        /// <summary>
        /// Prepares the <see cref="IComponentController"/> for operation.
        /// </summary>
        /// <param name="logController">The <see cref="ILogController"/> used to output generated logs.</param>
        /// <param name="communicationController">The <see cref="ICommunicationController"/> used to provide each <see cref="ComponentFactory"/> an <see cref="Networking.IClient"/>.</param>
        /// <param name="installer">The <see cref="Installation.IInstaller"/> used to provide dependency package information for each <see cref="ComponentFactory"/>.</param>
        /// <param name="folderAccessItems">A collection of folders.</param>
        void Initialize(ILogController logController,
                        ICommunicationController communicationController,
                        Installation.IInstaller installer,
                        IFolderAccessItems folderAccessItems);
        /// <summary>
        /// Will scan installed packages searching for any extensions and plugins that are not currently in the <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/> lists and start them.
        /// </summary>
        void Rescan();
        /// <summary>
        /// Will scan installed packages searching for any extensions and plugins that are not currently in the <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/> lists and start them, waiting <paramref name="intraComponentStartupDelay"/> between starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        void Rescan(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Starts the <see cref="IComponentController"/>.
        /// </summary>
        void Start();
        /// <summary>
        /// Starts the <see cref="IComponentController"/> waiting <paramref name="intraComponentStartupDelay"/> between starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        void Start(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will start all stopped extensions and plugins.
        /// </summary>
        void StartAllStoppedComponents();
        /// <summary>
        /// Will start all stopped extensions and plugins waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        void StartAllStoppedComponents(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will start all stopped extensions waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        void StartAllStoppedExtensions(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will start all stopped plugins waiting <paramref name="intraComponentStartupDelay"/> between starting each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between starting each <see cref="ComponentFactory"/>.</param>
        void StartAllStoppedPlugins(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/>.
        /// </summary>
        void Restart();
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/> and <see cref="PluginComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        void Restart(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="ExtensionComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        void RestartAllExtenstions(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Will <see cref="Stop()"/> and <see cref="Start()"/> all <see cref="PluginComponents"/>, waiting <paramref name="intraComponentStartupDelay"/> between stopping and starting each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping and starting each <see cref="ComponentFactory"/>.</param>
        void RestartAllPlugins(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Stops the <see cref="IComponentController"/>.
        /// </summary>
        void Stop();
        /// <summary>
        /// Stops the <see cref="IComponentController"/> waiting <paramref name="intraComponentStartupDelay"/> between stopping each <see cref="ComponentFactory"/>.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each <see cref="ComponentFactory"/>.</param>
        void Stop(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Stops all extensions and plugins waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each extension.</param>
        void StopAllComponents(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Stops all extensions waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each extension.</param>
        void StopAllExtensions(TimeSpan intraComponentStartupDelay);
        /// <summary>
        /// Stops all plugins waiting <paramref name="intraComponentStartupDelay"/> between stopping each one.
        /// </summary>
        /// <param name="intraComponentStartupDelay">The <see cref="TimeSpan"/> amount of time to wait between stopping each plugins.</param>
        /// <param name="removeAfterStopping">Indicates whether the plugin will be removed from the system. <b>True</b> will remove the plugin; otherwise, <b>false</b> will leave the pluging in the system.</param>
        void StopAllPlugins(TimeSpan intraComponentStartupDelay, bool removeAfterStopping);
    }
}
