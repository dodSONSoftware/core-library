using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a <see cref="ComponentManagement.ComponentFactory"/>.
    /// </summary>
    [Serializable]
    public class ServiceInformation
    {
        #region Ctor
        private ServiceInformation()
        {
        }
        /// <summary>
        /// Extracts information from the <paramref name="componentFactory"/>.
        /// </summary>
        /// <param name="componentFactory">The <see cref="ComponentManagement.ComponentFactory"/> to extract information from.</param>
        /// <param name="isExtension">Indicates whether the <paramref name="componentFactory"/> represents an Extension or a Plugin. <b>True</b> indicates an Extension; otherwise, <b>false</b> indicates a Plugin.</param>
        public ServiceInformation(ComponentManagement.ComponentFactory componentFactory,
                                  bool isExtension)
        {
            if (componentFactory == null)
            {
                throw new ArgumentNullException(nameof(componentFactory));
            }
            // #### parameters
            IsExtension = isExtension;

            // #### default component state: ComponentFactory
            ComponentState = ComponentState.ComponentFactory;

            // #### ComponentFactory specific information
            Id = componentFactory.Id;
            IsRunning = componentFactory.IsRunning;
            CustomConfiguration = componentFactory.CustomConfiguration;
            PackageConfiguration = componentFactory.PackageConfiguration.Configuration;

            // #### the component must be running to get the remaining information; not null indicates that it is running
            if (componentFactory.Component != null)
            {
                // #### component state: Component
                ComponentState = ComponentState.Component;

                // #### IComponent specific information
                FullyQualifiedName = componentFactory.FullyQualifiedName;
                LogSourceId = componentFactory.Component.LogSourceId;
                ClientConfiguration = componentFactory.ClientConfiguration.Configuration;
                DateLastStarted = componentFactory.Component.DateLastStarted;
                DateLastStopped = componentFactory.Component.DateLastStopped;
                LastRunDuration = componentFactory.Component.LastRunDuration;
                OverallRunDuration = componentFactory.Component.OverallRunDuration;
                StartCount = componentFactory.Component.StartCount;
                StopCount = componentFactory.Component.StopCount;

                // #### IService specific information
                if (componentFactory.Component is IService)
                {
                    // #### component state: Service
                    ComponentState = ComponentState.Service;
                    //var asIService = componentFactory.Component as IService;

                }
            }
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Indicates the state of a <see cref="RequestResponseTypes.ServiceInformation"/> object and what type of component it is.
        /// </summary>
        public ComponentState ComponentState
        {
            get;
        }
        // #### ComponentFactory specific information
        /// <summary>
        /// The unique id representing this <see cref="ComponentManagement.ComponentFactory"/>
        /// </summary>
        public string Id
        {
            get;
        }
        /// <summary>
        /// Returns whether the <see cref="Addon.IAddon"/> is running. <b>True</b> indicates the <see cref="Addon.IAddonFactory.Addon"/> is loaded and running; otherwise, it returns <b>false</b>.
        /// </summary>
        public bool IsRunning
        {
            get;
        }
        /// <summary>
        /// Returns whether this <see cref="Addon.IAddon"/> is an Extension or a Plugin. <b>True</b> indicates an Extension; otherwise, <b>false</b> indicates a Plugin.
        /// </summary>
        public bool IsExtension
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
        /// The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.
        /// </summary>
        public Configuration.IConfigurationGroup PackageConfiguration
        {
            get;
        }

        // #### IComponent specific information
        /// <summary>
        /// The fully-qualified name for this component.
        /// </summary>
        public string FullyQualifiedName
        {
            get;
        }
        /// <summary>
        /// The human-friendly name for this component.
        /// </summary>
        public string DisplayName
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
        /// The <see cref="Networking.IClientConfiguration"/> for this component.
        /// </summary>
        public Configuration.IConfigurationGroup ClientConfiguration
        {
            get;
        }
        /// <summary>
        /// The date the component was started.
        /// </summary>
        public DateTime DateLastStarted
        {
            get;
        }
        /// <summary>
        /// The date the component was stopped.
        /// </summary>
        public DateTime DateLastStopped
        {
            get;
        }
        /// <summary>
        /// Returns the duration the component has been running since it was last started.
        /// </summary>
        public TimeSpan LastRunDuration
        {
            get;
        }
        /// <summary>
        /// Returns the total duration the component has been running.
        /// </summary>
        public TimeSpan OverallRunDuration
        {
            get;
        }
        /// <summary>
        /// Returns the number of times the component has been started.
        /// </summary>
        public int StartCount
        {
            get;
        }
        /// <summary>
        /// Returns the number of times the component has been stopped.
        /// </summary>
        public int StopCount
        {
            get;
        }

        // #### IService specific information

        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"ComponentState={ComponentState}, IsRunning={IsRunning}, IsExtension={IsExtension}, Id={Id}";
        #endregion
    }
}
