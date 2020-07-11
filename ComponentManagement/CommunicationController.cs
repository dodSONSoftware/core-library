using System;
using System.Collections.Generic;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Provides access to and control of a configurable, type-based message communication system with configurable compression, encryption and the ability to send a message to a specific client.
    /// </summary>
    /// <seealso cref="Networking"/>
    [Serializable]
    public class CommunicationController
        : ICommunicationController
    {
        #region Events
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
        private CommunicationController()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="CommunicationController"/>.
        /// </summary>
        /// <param name="serverType"></param>
        /// <param name="clientType"></param>
        /// <param name="sharedServerChannelAddress"></param>
        /// <param name="sharedServerConfiguration"></param>
        /// <param name="transportController"></param>
        /// <param name="logSourceId"></param>
        public CommunicationController(Type serverType,
                                       Type clientType,
                                       Networking.IChannelAddress sharedServerChannelAddress,
                                       Networking.IServerConfiguration sharedServerConfiguration,
                                       Networking.ITransportController transportController,
                                       string logSourceId)
            : this()
        {
            ServerType = serverType ?? throw new ArgumentNullException("serverType");
            ClientType = clientType ?? throw new ArgumentNullException("clientType");
            SharedChannelAddress = sharedServerChannelAddress ?? throw new ArgumentNullException("sharedServerChannelAddress");
            SharedServerConfiguration = sharedServerConfiguration ?? throw new ArgumentNullException("sharedServerConfiguration");
            TransportController = transportController ?? throw new ArgumentNullException("transportController");
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public CommunicationController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "CommunicationController")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"CommunicationController\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // log source id
            LogSourceId = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "LogSourceId", typeof(string)).Value;
            // _ServerType
            ServerType = (Type)Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Server", typeof(Type)).Value);
            // _ClientType
            ClientType = (Type)Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Client", typeof(Type)).Value);
            // _SharedServerChannelAddress
            if (!configuration.ContainsKey("ServerChannel"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"ServerChannel\".", nameof(configuration));
            }
            // ---- ip address
            if (!configuration["ServerChannel"].Items.ContainsKey("IPAddress"))
            {
                configuration["ServerChannel"].Items.Add("IPAddress", "localhost", typeof(string));
            }
            else
            {
                if (configuration["ServerChannel"].Items["IPAddress"].Value.ToString() == "")
                {
                    configuration["ServerChannel"].Items.Add("IPAddress", "localhost", typeof(string));
                }
            }
            if (!configuration["ServerChannel"].Items["IPAddress"].ValueType.StartsWith(typeof(string).FullName))
            {
                throw new Exception($"Configuration item invalid format. Configuration item \"ServerChannel:IPAddress\" must be a <{typeof(string)}>.");
            }
            string ipAddress = (string)configuration["ServerChannel"].Items["IPAddress"].Value;
            // ---- port
            if (!configuration["ServerChannel"].Items.ContainsKey("Port"))
            {
                throw new ArgumentException($"Configuration missing information. Configuration must have item: \"ServerChannel:Port\".", nameof(configuration));
            }
            if (!configuration["ServerChannel"].Items["Port"].ValueType.StartsWith(typeof(int).FullName))
            {
                throw new Exception($"Configuration item invalid format. Configuration item \"ServerChannel:Port\" must be a <{typeof(int)}>.");
            }
            int port = (int)configuration["ServerChannel"].Items["Port"].Value;
            // ---- name
            if (!configuration["ServerChannel"].Items.ContainsKey("Name"))
            {
                throw new ArgumentException($"Configuration missing information. Configuration must have item: \"ServerChannel:Name\".", nameof(configuration));
            }
            if (!configuration["ServerChannel"].Items["Name"].ValueType.StartsWith(typeof(string).FullName))
            {
                throw new Exception($"Configuration item invalid format. Configuration item \"ServerChannel:Name\" must be a <{typeof(string)}>.");
            }
            if (string.IsNullOrWhiteSpace((string)configuration["ServerChannel"].Items["Name"].Value))
            {
                throw new Exception($"Configuration invalid information. Configuration item: \"ServerChannel:Name\" cannot be empty.");
            }
            string name = (string)configuration["ServerChannel"].Items["Name"].Value;
            SharedChannelAddress = new Networking.ChannelAddress(ipAddress, port, name);
            // _SharedServerConfiguration
            if (!configuration.Items.ContainsKey("ServerId"))
            {
                throw new ArgumentException($"Configuration missing information. Configuration must have item: \"ServerId\".", nameof(configuration));
            }
            if (!configuration.Items["ServerId"].ValueType.StartsWith(typeof(string).FullName))
            {
                throw new Exception($"Configuration item invalid format. Configuration item \"ServerId\" must be a <{typeof(string)}>.");
            }
            string id = (string)configuration.Items["ServerId"].Value;
            if (!configuration.ContainsKey("ServerOverrideTypesFilter"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"ServerOverrideTypesFilter\".", nameof(configuration));
            }
            List<Networking.IPayloadTypeInfo> overrideTypesFilter = new List<Networking.IPayloadTypeInfo>();
            foreach (var item in configuration["ServerOverrideTypesFilter"].Items)
            {
                string typeName = (string)item.Value;
                overrideTypesFilter.Add(new Networking.PayloadTypeInfo(typeName));
            }
            SharedServerConfiguration = new Networking.ServerConfiguration(id, overrideTypesFilter);
            // _TransportController
            if (!configuration.ContainsKey("TransportController"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"TransportController\".", nameof(configuration));
            }
            TransportController = (Networking.ITransportController)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(Networking.TransportController), configuration["TransportController"]);
        }
        #endregion
        #region Private Fields
        private Networking.IServer _SharedServer = null;
        #endregion
        #region ICommunicationController Methods
        /// <summary>
        /// Indicates the operating state of the <see cref="ICommunicationController"/>.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_SharedServer != null)
                {
                    return _SharedServer.State == Networking.ChannelStates.Open;
                }
                return false;
            }
        }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="ICommunicationController"/> started.
        /// </summary>
        public DateTimeOffset StartTime { get; private set; } = DateTimeOffset.Now;
        /// <summary>
        /// The duration that the <see cref="ICommunicationController"/> has run. 
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
        /// The <see cref="Type"/> representing the server; this <see cref="Type"/> must implement the <see cref="Networking.IServer"/> interface.
        /// </summary>
        public Type ServerType
        {
            get;
        }
        /// <summary>
        /// The <see cref="Type"/> representing the client; this <see cref="Type"/> must implement the <see cref="Networking.IClient"/> interface.
        /// </summary>
        public Type ClientType
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IChannelAddress"/> used by the <see cref="CreateSharedServer"/> and the <see cref="CreateSharedClient(Networking.IClientConfiguration)"/> methods.
        /// </summary>
        public Networking.IChannelAddress SharedChannelAddress
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.IServerConfiguration"/> used by the <see cref="CreateSharedServer"/> method.
        /// </summary>
        public Networking.IServerConfiguration SharedServerConfiguration
        {
            get;
        }
        /// <summary>
        /// The <see cref="Networking.ITransportController"/> used by the <see cref="CreateSharedServer"/> and the <see cref="CreateSharedClient(Networking.IClientConfiguration)"/> methods.
        /// </summary>
        public Networking.ITransportController TransportController
        {
            get;
        }
        /// <summary>
        /// Will use the <see cref="SharedChannelAddress"/> and the <see cref="SharedServerConfiguration"/> to create a <see cref="Networking.IServer"/>.
        /// </summary>
        /// <returns>Returns a <see cref="Networking.IServer"/> using the <see cref="SharedChannelAddress"/> and the <see cref="SharedServerConfiguration"/>.</returns>
        public Networking.IServer CreateSharedServer()
        {
            if (_SharedServer == null)
            {
                StartTime = DateTimeOffset.Now;
                _SharedServer = InternalCreateServer(SharedChannelAddress, SharedServerConfiguration, LogSourceId);
                _SharedServer.ActivityLogsEvent += Server_ActivityLogsEvent;
            }
            return _SharedServer;
        }
        /// <summary>
        /// Will use the <see cref="SharedChannelAddress"/> and the given <see cref="Networking.IClientConfiguration"/> to create a <see cref="Networking.IClient"/> that will be able to communicate with the server created using the <see cref="CreateSharedServer"/> method.
        /// </summary>
        /// <param name="configuration">The <see cref="Networking.IClientConfiguration"/> used to configure the new <see cref="Networking.IClient"/>.</param>
        /// <returns>A <see cref="Networking.IClient"/> created using the <see cref="SharedChannelAddress"/>and the given <see cref="Networking.IClientConfiguration"/>.</returns>
        public Networking.IClient CreateSharedClient(Networking.IClientConfiguration configuration) => InternalCreateClient(SharedChannelAddress, configuration);
        /// <summary>
        /// Will use the given <see cref="Networking.IChannelAddress"/>, <see cref="Networking.IServerConfiguration"/> and LogSourceId to create a <see cref="Networking.IServer"/>.
        /// </summary>
        /// <param name="channelAddress">The <see cref="Networking.IChannelAddress"/> defining the required connection information.</param>
        /// <param name="configuration">The <see cref="Networking.IServerConfiguration"/> defining the required configuration information.</param>
        /// <param name="logSourceId"></param>
        /// <returns>A <see cref="Networking.IServer"/> created using the given <see cref="Networking.IChannelAddress"/>, <see cref="Networking.IServerConfiguration"/> and LogSourceId.</returns>
        public Networking.IServer CreateServer(Networking.IChannelAddress channelAddress, Networking.IServerConfiguration configuration, string logSourceId) => InternalCreateServer(channelAddress, configuration, logSourceId);
        /// <summary>
        /// Will use the given <see cref="Networking.IChannelAddress"/> and <see cref="Networking.IClientConfiguration"/> to create a <see cref="Networking.IClient"/>.
        /// </summary>
        /// <param name="channelAddress">The <see cref="Networking.IChannelAddress"/> defining the required connection information.</param>
        /// <param name="configuration">The <see cref="Networking.IClientConfiguration"/> defining the required configuration information.</param>
        /// <returns>A <see cref="Networking.IClient"/> created using the given <see cref="Networking.IChannelAddress"/> and <see cref="Networking.IClientConfiguration"/>.</returns>
        public Networking.IClient CreateClient(Networking.IChannelAddress channelAddress, Networking.IClientConfiguration configuration) => InternalCreateClient(channelAddress, configuration);
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("CommunicationController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // log source id
                result.Items.Add("LogSourceId", LogSourceId, LogSourceId.GetType());
                // ServerType/ClientType
                result.Items.Add("Server", ServerType, ServerType.GetType());
                result.Items.Add("Client", ClientType, ClientType.GetType());
                // _SharedServerChannelAddress
                result.Add("ServerChannel");
                result["ServerChannel"].Items.Add("IPAddress", SharedChannelAddress.IPAddress, SharedChannelAddress.IPAddress.GetType());
                result["ServerChannel"].Items.Add("Name", SharedChannelAddress.Name, SharedChannelAddress.Name.GetType());
                result["ServerChannel"].Items.Add("Port", SharedChannelAddress.Port, SharedChannelAddress.Port.GetType());
                // _SharedServerConfiguration
                result.Items.Add("ServerId", SharedServerConfiguration.Id, SharedServerConfiguration.Id.GetType());
                // ServerOverrideTypesFilter
                result.Add("ServerOverrideTypesFilter");
                var counter = 0;
                foreach (var item in SharedServerConfiguration.OverrideTypesFilter)
                {
                    result["ServerOverrideTypesFilter"].Items.Add($"Type{(++counter):000}", item.TypeName, typeof(Type));
                }
                // _TransportController
                result.Add(TransportController.Configuration);
                //
                return result;
            }
        }
        #endregion
        #region Private Methods
        private Networking.IServer InternalCreateServer(Networking.IChannelAddress channelAddress, Networking.IServerConfiguration configuration, string logSourceId)
        {
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            var server = Activator.CreateInstance(ServerType) as Networking.IServer;
            server.Initialize(channelAddress, configuration, TransportController, logSourceId);
            return server;
        }
        private void Server_ActivityLogsEvent(object sender, Networking.ActivityLogsEventArgs e) => RaiseActivityLogsEvent(e);
        private Networking.IClient InternalCreateClient(Networking.IChannelAddress channelAddress, Networking.IClientConfiguration configuration)
        {
            var client = Activator.CreateInstance(ClientType) as Networking.IClient;
            client.Initialize(channelAddress, configuration, TransportController.RegistrationController);
            return client;
        }
        #endregion
    }
}
