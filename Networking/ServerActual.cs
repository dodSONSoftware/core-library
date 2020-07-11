using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines the properties and methods exposed by the server.
    /// </summary>
    [System.ServiceModel.ServiceBehavior(InstanceContextMode = System.ServiceModel.InstanceContextMode.Single)]
    internal class ServerActual
        : IService
    {
        #region Events
        /// <summary>
        /// Fired when activity <see cref="Logging.Logs"/> are available.
        /// </summary>
        public event EventHandler<Networking.ActivityLogsEventArgs> ActivityLogsEvent;
        /// <summary>
        /// Attempts to broadcast the <paramref name="response"/> containing the <see cref="Logging.Logs"/>.
        /// </summary>
        /// <param name="response">The <see cref="Networking.ActivityLogsEventArgs"/> containing the <see cref="Logging.Logs"/> to broadcast.</param>
        protected void RaiseActivityLogsEvent(Networking.ActivityLogsEventArgs response) => ActivityLogsEvent?.Invoke(this, response);
        #endregion
        #region Ctor
        private ServerActual()
        {
        }
        /// <summary>
        /// Creates a new instance of <b>Actual Server</b>.
        /// </summary>
        /// <param name="serverId">The unique id for this server.</param>
        /// <param name="transportController">Used to control the transportation layer.</param>
        /// <param name="overrideTypesFilter">A collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.</param>
        /// <param name="logSourceId">The value used by the <see cref="ServerBase"/> for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.</param>
        public ServerActual(string serverId,
                             ITransportController transportController,
                             IEnumerable<IPayloadTypeInfo> overrideTypesFilter,
                             string logSourceId)
            : this()
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }
            _ServerId = serverId;
            OverrideTypesFilter = overrideTypesFilter ?? throw new ArgumentNullException(nameof(overrideTypesFilter));
            TransporterController = transportController ?? throw new ArgumentNullException(nameof(transportController));
            if (string.IsNullOrWhiteSpace(logSourceId))
            {
                throw new ArgumentNullException(nameof(logSourceId));
            }
            LogSourceId = logSourceId;
        }
        #endregion
        #region Private Fields
        private readonly string _ServerId = "";
        private readonly object _SyncRootRegisteredCallbackClients = new object();
        private readonly Dictionary<string, DataHolder> _RegisteredCallbackClients = new Dictionary<string, DataHolder>();
        private readonly object _SyncRootTransporterController = new object();
        //
        private readonly Converters.TypeSerializer<TransportStatistics> _StatstisticsConvertor = new Converters.TypeSerializer<TransportStatistics>();
        private readonly Converters.TypeSerializer<List<ITransportStatistics>> _ListConvertor = new Converters.TypeSerializer<List<ITransportStatistics>>();
        #endregion
        #region Public Methods
        /// <summary>
        /// The value used by the <see cref="ServerBase"/> for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.
        /// </summary>
        public string LogSourceId
        {
            get; private set;
        }
        /// <summary>
        /// Determines if detailed information about the client is shown during registration. <b>True</b> will show the detailed information.
        /// </summary>
        public bool ShowRegistrationDetails { get; set; } = true;
        /// <summary>
        /// A collection of all registered clients.
        /// </summary>
        public IEnumerable<IClientConfiguration> RegisteredClients
        {
            get
            {
                lock (_SyncRootRegisteredCallbackClients)
                {
                    var list = from x in _RegisteredCallbackClients.Values
                               select x.ClientConfiguration;
                    return list;
                }
            }
        }
        /// <summary>
        /// Gets a registered client by id.
        /// </summary>
        /// <param name="clientId">The client id to search for.</param>
        /// <returns>The <see cref="IClientConfiguration"/> that matches the <paramref name="clientId"/>; or <b>null</b>.</returns>
        public IClientConfiguration RegisteredClientById(string clientId)
        {
            lock (_SyncRootRegisteredCallbackClients)
            {
                if (_RegisteredCallbackClients.ContainsKey(clientId))
                {
                    return _RegisteredCallbackClients[clientId].ClientConfiguration;
                }
                return null;
            }
        }
        /// <summary>
        /// Used to control the transportation layer.
        /// </summary>
        public ITransportController TransporterController
        {
            get;
        }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.
        /// </summary>
        public IEnumerable<IPayloadTypeInfo> OverrideTypesFilter
        {
            get;
        }
        /// <summary>
        /// Gets a collection of registered clients and their callback service.
        /// </summary>
        public IEnumerable<Tuple<IClientConfiguration, IServiceCallback>> RegisteredClientsCallbacks
        {
            get
            {
                lock (_SyncRootRegisteredCallbackClients)
                {
                    return from x in _RegisteredCallbackClients
                           select new Tuple<IClientConfiguration, IServiceCallback>(x.Value.ClientConfiguration, x.Value.CallbackClient);
                }
            }
        }
        #endregion
        #region IService Methods
        /// <summary>
        /// Called by clients to register with the server.
        /// </summary>
        /// <param name="registrationType">The type of registration requested.</param>
        /// <param name="data">Information required to register with the server.</param>
        /// <returns>Results relating to the registration.</returns>
        public byte[] RegistrationChannel(RegistrationTypeEnum registrationType, byte[] data)
        {
            // ******** RECIEVED REGISTRATION REQUEST FROM CLIENT ********

            var results = TransporterController.RegistrationController.TryAddDataFromRegistrationChannel(data,
                                                                                                         out IClientConfiguration clientConfiguration);
            // complete actual registering/unregistering
            if (clientConfiguration != null)
            {
                if (registrationType == RegistrationTypeEnum.Unregister)
                {

                    // TODO: this is (-close-) to where the statistics for each client should be logged.
                    //       various problems exist: no logger, cannot execute a command while processing a command, ...
                    // SEE: dodSON.Core.ComponentManagement.ComponentExtensionBase.OnStop()
                    //      dodSON.Core.ComponentManagement.ComponentPluginBase.OnStop()


                    //// ######## THIS DOES NOT WORK ########
                    //Converters.TypeSerializer<ITransportStatistics> transportStatisticsConvertor = new Converters.TypeSerializer<ITransportStatistics>();
                    //var stats = transportStatisticsConvertor.FromByteArray(
                    //                          TransporterController.TransportConfiguration.Compressor.Decompress(
                    //                              TransporterController.TransportConfiguration.Encryptor.Decrypt(
                    //                                  _RegisteredCallbackClients[clientConfiguration.Id].CallbackClient.GetTransportStatistics())));
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Shutting Down Client, Id={stats.ClientServerId}");
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Total Incoming Bytes={Common.ByteCountHelper.ToString(stats.IncomingBytes)} ({stats.IncomingBytes:N0})");
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Total Incoming Envelopes={stats.IncomingEnvelopes:N0}");
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Total Outgoing Bytes={Common.ByteCountHelper.ToString(stats.OutgoingBytes)} ({stats.OutgoingBytes:N0})");
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Total Outgoing Envelopes={stats.OutgoingEnvelopes:N0}");
                    ////logs.Add(Logging.LogEntryType.Information, LogSourceId, $"^^^^ Client Closed Successfully.");
                    ////


                    // unregister client
                    Unregister(clientConfiguration.Id);
                }
                else
                {
                    // register client
                    Register(clientConfiguration);
                }
            }
            else
            {
                // TODO: missing client configuration; not sure what to do here
            }
            return results;
        }
        /// <summary>
        /// Transmits the <see cref="TransportEnvelope"/> to the server.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to send to the server.</param>
        public void PushEnvelopeToServer(TransportEnvelope envelope)
        {
            // ******** RECIEVED ENVELOPE FROM CLIENT ********

            if (envelope != null)
            {
                System.Threading.Tasks.Task.Run(() =>
                {
                    // extract header, update stats
                    ITransportEnvelopeHeader header = null;
                    lock (_SyncRootTransporterController)
                    {
                        TransporterController.TransportStatisticsLive.IncomingEnvelopes++;
                        TransporterController.TransportStatisticsLive.IncomingBytes += (envelope.Header.Length + envelope.Payload.Length);
                        header = TransporterController.ExtractHeader(envelope);
                    }
                    // get list of registered clients
                    List<KeyValuePair<string, DataHolder>> workerList = null;
                    var removableClients = new List<string>();
                    lock (_SyncRootRegisteredCallbackClients)
                    {
                        workerList = new List<KeyValuePair<string, DataHolder>>(_RegisteredCallbackClients);
                    }
                    // send envelope to each registered client, that will accept it
                    System.Threading.Tasks.Parallel.ForEach(workerList, new Action<KeyValuePair<string, DataHolder>>(
                            (x) =>
                            {
                                try
                                {
                                    // handle type-based traffic filtering
                                    if (CanSendEnvelope(header, x.Value.ClientConfiguration, OverrideTypesFilter))
                                    {
                                        lock (_SyncRootTransporterController)
                                        {
                                            TransporterController.TransportStatisticsLive.OutgoingEnvelopes++;
                                            TransporterController.TransportStatisticsLive.OutgoingBytes += (envelope.Header.Length + envelope.Payload.Length);
                                        }
                                        InternalSendEnvelopeToClient(envelope, x.Value);
                                    }
                                    else
                                    {
                                    }
                                }
                                catch
                                {
                                    removableClients.Add(x.Value.ClientConfiguration.Id);
                                }
                            }));
                    // **** unregister unresponsive clients
                    foreach (var clientId in removableClients)
                    {
                        if (clientId != null)
                        {
                            lock (_SyncRootRegisteredCallbackClients)
                            {
                                if (_RegisteredCallbackClients.ContainsKey(clientId))
                                {
                                    _RegisteredCallbackClients.Remove(clientId);
                                }
                            }
                        }
                    }
                });
            }
            else
            {
                // TODO: A null envelope? what could have caused this and what should I do about it?
            }
        }
        #endregion
        #region Private Methods
        private void InternalSendEnvelopeToClient(TransportEnvelope envelope, DataHolder registeredClient)
        {
            registeredClient.CallbackClient.PushEnvelopeToClient(envelope);
            registeredClient.LastUsed = DateTime.Now;
        }
        private bool CanSendEnvelope(ITransportEnvelopeHeader header,
                                     IClientConfiguration clientConfiguration,
                                     IEnumerable<IPayloadTypeInfo> overrideTypesFilter)
        {
            var sendMessage = false;
            // **** check for override message
            if (IsOverrideMessage(header, overrideTypesFilter))
            {
                sendMessage = true;
            }
            else
            {
                // **** check self-sent message flag
                if (clientConfiguration.Id == ExtractSourceClientId(header.ClientId))
                {
                    if ((clientConfiguration.ReceiveSelfSentMessages) &&
                        (IsMessageTypeInReceivableTypesFilter(header, clientConfiguration)))
                    {
                        sendMessage = true;
                    }
                }
                else
                {
                    var headerTargetClientId = ExtractTargetClientId(header);
                    if (!string.IsNullOrWhiteSpace(headerTargetClientId))
                    {
                        // ######## TARGETED MESSAGE
                        // check for valid receiver
                        if (clientConfiguration.Id == headerTargetClientId)
                        {
                            if (IsMessageTypeInReceivableTypesFilter(header, clientConfiguration))
                            {
                                sendMessage = true;
                            }
                        }
                    }
                    else
                    {
                        // ######## NON-TARGETED MESSAGE
                        if (IsMessageTypeInReceivableTypesFilter(header, clientConfiguration))
                        {
                            sendMessage = true;
                        }
                    }
                }
            }
            //
            return sendMessage;
        }

        private string ExtractSourceClientId(string clientId)
        {
            if (clientId.Contains(","))
            {
                return clientId.Substring(clientId.LastIndexOf(",") + 1);
            }
            return clientId;
        }
        private string ExtractTargetClientId(ITransportEnvelopeHeader header)
        {
            if (header.TargetId.Contains(","))
            {
                return header.TargetId.Substring(header.TargetId.LastIndexOf(",") + 1);
            }
            return header.TargetId;
        }

        private bool IsOverrideMessage(ITransportEnvelopeHeader header, IEnumerable<IPayloadTypeInfo> overrideTypesFilter)
        {
            return (from x in overrideTypesFilter
                    where header.IsEnvelopeType(x)
                    select x).FirstOrDefault() != null;
        }
        private bool IsMessageTypeInReceivableTypesFilter(ITransportEnvelopeHeader header,
                                                          IClientConfiguration clientConfiguration)
        {
            // empty receivable types filter equates to getting all messages
            if (clientConfiguration.ReceivableTypesFilter.Count() == 0)
            {
                return true;
            }
            // check receivable types filter
            return (from x in clientConfiguration.ReceivableTypesFilter
                    where header.IsEnvelopeType(x)
                    select x).FirstOrDefault() != null;
        }

        private void Register(IClientConfiguration clientConfiguration)
        {
            if (clientConfiguration == null)
            {
                throw new ArgumentNullException("clientConfiguration");
            }
            lock (_SyncRootRegisteredCallbackClients)
            {
                if (_RegisteredCallbackClients.ContainsKey(clientConfiguration.Id))
                {
                    // remove old registration
                    _RegisteredCallbackClients.Remove(clientConfiguration.Id);
                }
                // log it
                var logs = new Logging.Logs();
                NetworkShared.LogClientConfigurationDetails(logs, LogSourceId, clientConfiguration, ShowRegistrationDetails);
                RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_Register, logs));
                // add client to registration 
                var callbackChannel = System.ServiceModel.OperationContext.Current.GetCallbackChannel<IServiceCallback>();
                if (callbackChannel == null)
                {
                    throw new NullReferenceException("ServerActual.Register()-->creating callbackChannel failed.");
                }
                _RegisteredCallbackClients.Add(clientConfiguration.Id,
                                               new DataHolder()
                                               {
                                                   LastUsed = DateTime.Now,
                                                   ClientConfiguration = clientConfiguration,
                                                   CallbackClient = callbackChannel
                                               });
            }
        }
        private void Unregister(string clientId)
        {
            lock (_SyncRootRegisteredCallbackClients)
            {
                if (_RegisteredCallbackClients.ContainsKey(clientId))
                {
                    // log it
                    var logs = new Logging.Logs();
                    logs.Add(Logging.LogEntryType.Information, LogSourceId, $"Unregistering Client={clientId}, Date Started={_RegisteredCallbackClients[clientId].DateCreated}, Runtime={(DateTime.Now - _RegisteredCallbackClients[clientId].DateCreated)}");
                    RaiseActivityLogsEvent(new ActivityLogsEventArgs(ActivityLogsEventType.Network_Unregister, logs));
                    // remove client from registration
                    _RegisteredCallbackClients.Remove(clientId);
                }
            }
        }
        #endregion
        #region Private Class: DataHolder
        [Serializable]
        private class DataHolder
        {
            public DateTime DateCreated { get; } = DateTime.Now;
            public DateTime LastUsed
            {
                get; set;
            }
            public IServiceCallback CallbackClient
            {
                get; set;
            }
            public IClientConfiguration ClientConfiguration
            {
                get; set;
            }
        }
        #endregion
    }
}
