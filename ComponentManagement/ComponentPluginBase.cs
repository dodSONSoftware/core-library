using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines the base class for all components that will be running in a separate <see cref="System.AppDomain"/>.
    /// </summary>
    /// <seealso cref="Addon"/>
    /// <seealso cref="Addon.PluginBase"/>
    /// <seealso cref="IComponent"/>
    /// <seealso cref="MarshalByRefObject"/>
    [Serializable()]
    public abstract class ComponentPluginBase
        : Addon.PluginBase,
          IComponent
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected ComponentPluginBase() : base() { }
        #endregion
        #region Private Fields
        private LogMarshal _LogMarshal;
        private string _NetworkLogSourceId;
        #endregion
        #region IComponent Methods
        /// <summary>
        /// The id for this component. This value should be unique.
        /// </summary>
        public string Id => Common.TypeHelper.FormatId(this.GetType().AssemblyQualifiedName, this.GetType().Assembly.Location, InstallRootPath, InstallPath);
        /// <summary>
        /// The <see cref="Type.AssemblyQualifiedName"/> name for this component. 
        /// </summary>
        public string FullyQualifiedName => this.GetType().AssemblyQualifiedName;
        /// <summary>
        /// The value used as the source id when creating log entries.
        /// </summary>
        public string LogSourceId
        {
            get; private set;
        }
        /// <summary>
        /// The <see cref="Networking.IClient"/> used to communicate with the network.
        /// </summary>
        public Networking.IClient Client
        {
            get; private set;
        }
        /// <summary>
        /// A <see cref="Networking.IClient"/> made for temporary, unrestricted, communications. This client is not opened by default and should be closed when not being used.
        /// </summary>
        public Networking.IClient WorkingClient
        {
            get; private set;
        }
        /// <summary>
        /// The <see cref="Networking.IClientConfiguration"/> for this component.
        /// </summary>
        public abstract Networking.IClientConfiguration ClientConfiguration
        {
            get;
        }
        /// <summary>
        /// The <see cref="Packaging.IPackageConfiguration"/> for the package this component was located in.
        /// </summary>
        public Packaging.IPackageConfiguration PackageConfiguration
        {
            get; private set;
        }
        /// <summary>
        /// A custom <see cref="Configuration.IConfigurationGroup"/> found in the installed package.
        /// </summary>
        public Configuration.IConfigurationGroup CustomConfiguration
        {
            get; private set;
        }
        /// <summary>
        /// The root path where the <see cref="Installation.IInstaller"/> installs packages.
        /// </summary>
        public string InstallRootPath
        {
            get; private set;
        }
        /// <summary>
        /// The install directory path where this component's package has been installed.
        /// </summary>
        public string InstallPath
        {
            get; private set;
        }
        /// <summary>
        /// A temporary folder for the <see cref="IComponent"/> to use.
        /// This folder, and all of its contents, will be deleted when the <see cref="IComponent"/> is stopped.
        /// </summary>
        public string TemporaryPath
        {
            get; private set;
        }
        /// <summary>
        /// A permanent folder for the <see cref="IComponent"/> to use.
        /// This folder, and any of its contents, will remain intact between starting and stopping of this <see cref="IComponent"/>.
        /// </summary>
        public string LongTermStoragePath
        {
            get; private set;
        }
        /// <summary>
        /// A collection of folders.
        /// </summary>
        public IFolderAccessItems FolderAccessItems
        {
            get; private set;
        }
        /// <summary>
        /// Prepares the component to be handled by the <see cref="ComponentController"/>.
        /// </summary>
        /// <param name="logMarshal">The <see cref="LogMarshal"/> provided by the <see cref="LogController"/>.</param>
        /// <param name="client">The <see cref="Networking.IClient"/> made for this component.</param>
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
        public void Initialize(LogMarshal logMarshal,
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
                               IFolderAccessItems folderAccessItems)
        {
            _LogMarshal = logMarshal ?? throw new ArgumentNullException(nameof(logMarshal));
            Client = client ?? throw new ArgumentNullException(nameof(client));
            WorkingClient = workingClient ?? throw new ArgumentNullException(nameof(workingClient));
            if (Client.ClientConfiguration.ReceivableTypesFilter.Count() > 0)
            {
                // add required message types to receivable message types filter
                var list = new List<Networking.IPayloadTypeInfo>(Client.ClientConfiguration.ReceivableTypesFilter);
                list.Add(new Networking.PayloadTypeInfo(typeof(SupportsComponentDesignationRequest)));
                Client.ClientConfiguration.ReplaceReceivableTypesFilter(list);
            }
            //
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
            //
            if (string.IsNullOrWhiteSpace(networkLogSourceId))
            {
                throw new ArgumentNullException(nameof(networkLogSourceId));
            }
            _NetworkLogSourceId = networkLogSourceId;
            //
            PackageConfiguration = packageConfiguration ?? throw new ArgumentNullException(nameof(packageConfiguration));
            CustomConfiguration = customConfiguration;
            //
            if (string.IsNullOrWhiteSpace(installRootPath))
            {
                throw new ArgumentNullException(nameof(installRootPath));
            }
            InstallRootPath = installRootPath;
            //
            if (string.IsNullOrWhiteSpace(installPath))
            {
                throw new ArgumentNullException(nameof(installPath));
            }
            InstallPath = installPath;
            //
            if (string.IsNullOrWhiteSpace(temporaryPath))
            {
                throw new ArgumentNullException(nameof(temporaryPath));
            }
            TemporaryPath = temporaryPath;
            //
            if (string.IsNullOrWhiteSpace(longTermStoragePath))
            {
                throw new ArgumentNullException(nameof(longTermStoragePath));
            }
            LongTermStoragePath = longTermStoragePath;
            //
            FolderAccessItems = folderAccessItems ?? throw new ArgumentNullException(nameof(folderAccessItems));
        }
        /// <summary>
        /// Used to write logs into the logging system.
        /// </summary>
        public Logging.ILogWriter Log => _LogMarshal;
        /// <summary>
        /// Used to send a <see cref="Networking.IMessage"/>s into the communication system.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> contained in the <paramref name="payload"/>.</typeparam>
        /// <param name="clientTargetId">The id of the <see cref="Networking.IClient"/> to receive the message; <b>null</b> to send to message to every <see cref="Networking.IClient"/>.</param>
        /// <param name="payload">The <see cref="Type"/> converted into a byte array.</param>
        public void SendMessage<T>(string clientTargetId, T payload)
        {
            if (Client.State == Networking.ChannelStates.Open)
            {
                var payloadTypeInfo = new Networking.PayloadTypeInfo(typeof(T));
                var byteArrayPayload = (new Converters.TypeSerializer<T>()).ToByteArray(payload);
                Client.SendMessage(new Networking.Message(Guid.NewGuid().ToString(),
                                                           Client.Id,
                                                           clientTargetId,
                                                           payloadTypeInfo,
                                                           byteArrayPayload));
            }
        }
        /// <summary>
        /// Used to send a <see cref="Networking.IMessage"/> into the communication system.
        /// </summary>
        /// <param name="message">The <see cref="Networking.IMessage"/> to send.</param>
        public void SendMessage(Networking.IMessage message)
        {
            if (Client.State == Networking.ChannelStates.Open)
            {
                Client.SendMessage(message);
            }
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Called during the component's startup sequence. Will call the abstract method <see cref="OnComponentStarted"/> at the proper time.
        /// </summary>
        protected override void OnStart()
        {
            var logs = new Logging.Logs();
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Starting Plugin Component {Id}");
            // open the client
            if (Client.State == Networking.ChannelStates.Closed)
            {
                if (Client.Open(out Exception clientOpenException) != Networking.ChannelStates.Open)
                {
                    throw new Exception("Could not open client.", clientOpenException);
                }
            }
            else
            {
                throw new Exception($"Could not open client. Client State={Client.State}");
            }
            // connect to the message bus, enqueue SupportsComponentDesignationRequest messages; otherwise, allow the message to pass
            Client.MessageBus += new EventHandler<Networking.MessageEventArgs>((s, e) =>
            {
                // check for auto SupportsComponentDesignationRequest messages
                if (e.Message.TypeInfo.TypeName.Equals(typeof(SupportsComponentDesignationRequest).AssemblyQualifiedName))
                {
                    if (!string.IsNullOrWhiteSpace(ComponentDesignation))
                    {
                        var request = e.Message.PayloadMessage<SupportsComponentDesignationRequest>();
                        if ((request != null) && (request.ComponentDesignation.Equals(ComponentDesignation)))
                        {
                            Log.Write(Logging.LogEntryType.Information, LogSourceId, $"{typeof(SupportsComponentDesignationRequest).Name} received, ComponentDesignation={ComponentDesignation}, ClientId={e.Message.ClientId}, ProcessId={request.ProcessId}");
                            SendMessage(e.Message.ClientId, new SupportsComponentDesignationResponse(ComponentDesignation, ComponentName, Id, request.ProcessId));
                        }
                    }
                }
                // allow the messages to pass
                OnMessageReceived(e.Message);
            });
            // call abstract start
            OnComponentStarted(logs);
            // write all startup logs
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Started {Id}");
            Log.Write(logs);
        }
        /// <summary>
        /// Called during the component's shutdown sequence. Will call the abstract method <see cref="OnComponentStopping"/> at the proper time.
        /// </summary>
        protected override void OnStop()
        {
            var logs = new Logging.Logs();
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Stopping Plugin Component {Id}");
            // call abstract stop
            OnComponentStopping(logs);
            // close client
            if (Client.State == Networking.ChannelStates.Open)
            {
                if (!Client.Close(out Exception clientCloseException))
                {
                    throw new Exception("Could not close client.", clientCloseException);
                }
            }
            else
            {
                throw new Exception($"Could not close client. Client State={Client.State}");
            }
            //
            // ######## log statistics for the client; SEE: dodSON.Core.Networking.ServerActual.RegistrationChannel(byte[] data)
            //
            var stats = Client.TransportController.TransportStatisticsLive;
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Shutting Down Client, Id={stats.ClientServerId}, Uri={Client.AddressUri}");
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Total Incoming Bytes={Common.ByteCountHelper.ToString(stats.IncomingBytes)} ({stats.IncomingBytes:N0})");
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Total Incoming Envelopes={stats.IncomingEnvelopes:N0}");
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Total Outgoing Bytes={Common.ByteCountHelper.ToString(stats.OutgoingBytes)} ({stats.OutgoingBytes:N0})");
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Total Outgoing Envelopes={stats.OutgoingEnvelopes:N0}");
            logs.Add(Logging.LogEntryType.Information, _NetworkLogSourceId, $"Client Closed Successfully.");
            // release client
            Client = null;
            // write all logs
            logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Plugin Component Stopped {Id}, Start Time={this.DateLastStarted}, Run Time={this.LastRunDuration}");
            Log.Write(logs);
        }
        #endregion
        #region Abstract Methods
        /// <summary>
        /// Inheritors should override this method when they need to initialize their type; after the component's subsystems have started.
        /// </summary>
        /// <param name="log">The <see cref="Logging.Logs"/> to write any logs to.</param>
        protected abstract void OnComponentStarted(Logging.Logs log);
        /// <summary>
        /// Inheritors should override this method when they need to shut down their type; before the component's subsystems shutdown.
        /// </summary>
        /// <param name="log">The <see cref="Logging.Logs"/> to write any logs to.</param>
        protected abstract void OnComponentStopping(Logging.Logs log);
        /// <summary>
        /// Will be called when a <see cref="Networking.IMessage"/> is received.
        /// </summary>
        /// <param name="message">The message received from the communication system.</param>
        protected abstract void OnMessageReceived(Networking.IMessage message);
        /// <summary>
        /// The functional name of the <see cref="IComponent"/>; this should reflect the basic functionally of the component for a group a similar components.
        /// <para/>
        /// Multiple <see cref="IComponent"/>s can, and should, use the same <see cref="ComponentDesignation"/> to facilitate finding <see cref="IComponent"/>s on the network based on their function.
        /// Setting this to null, or an empty string, will opt out of the Component Designation Discovery system.
        /// </summary>
        /// <see cref="ComponentManagementHelper.DiscoverComponents(Networking.IClient, string, TimeSpan, System.Threading.CancellationToken, out bool, out string)"/>
        public abstract string ComponentDesignation
        {
            get;
        }
        /// <summary>
        /// The specific name for the component.
        /// <para/>
        /// This should be unique for each <see cref="IComponent"/> to help differentiated between <see cref="IComponent"/>s with the same <see cref="ComponentDesignation"/>.
        /// Setting this to null, or an empty string, will opt out of the Component Designation Discovery system.
        /// </summary>
        public abstract string ComponentName
        {
            get;
        }
        /// <summary>
        /// A <see cref="Networking.IClientConfiguration"/> used when creating the <see cref="WorkingClient"/>.
        /// </summary>
        public abstract Networking.IClientConfiguration WorkingClientConfiguration
        {
            get;
        }
        #endregion
    }
}
