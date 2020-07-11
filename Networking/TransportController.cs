using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{

    // TODO: there is no code to monitor and purge the ( _EnvelopeCache, _SeenMessagesCache )
    //      however, a thread may not be necessary; if a little memory usage can be tolerated.
    //      be sure to set the cache time limits to proper values to keep memory usage to a minimum.
    //
    //      see: ITransportConfiguration.EnvelopeCacheTimeLimit
    //      see: ITransportConfiguration.SeenMessageCacheTimeLimit



    /// <summary>
    /// Provides properties and methods used to control the transportation layer.
    /// </summary>
    [Serializable]
    public class TransportController
        : ITransportController,
          ITransportControllerAdvanced
    {
        #region Ctor
        // registrationController--> used by server & client
        // transportConfiguration--> only used by the server
        private TransportController()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="TransportController"/>.
        /// </summary>
        /// <param name="transportConfiguration">The transportation configuration for this <see cref="TransportController"/>.</param>
        /// <param name="registrationController">The registration controller used by this <see cref="TransportController"/>.</param>
        public TransportController(ITransportConfiguration transportConfiguration,
                                   IRegistrationController registrationController)
            : this()
        {
            if (registrationController == null)
            {
                registrationController = new RegistrationControllers.NullRegistrationController((byte[])null);
            }
            RegistrationController = registrationController;
            TransportConfiguration = transportConfiguration ?? throw new ArgumentNullException(nameof(transportConfiguration));
        }

        internal TransportController(IRegistrationController registrationController)
           : this()
        {
            if (registrationController == null)
            {
                registrationController = new RegistrationControllers.NullRegistrationController((byte[])null);
            }
            RegistrationController = registrationController;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public TransportController(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "TransportController")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"TransportController\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // _TransportConfiguration
            if (configuration.ContainsKey("TransportConfiguration"))
            {
                TransportConfiguration = (ITransportConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(TransportConfiguration), configuration["TransportConfiguration"]);
            }
            // _RegistrationController 
            if (!configuration.ContainsKey("RegistrationController"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"RegistrationController\".", nameof(configuration));
            }
            if ((!configuration["RegistrationController"].ContainsKey("TransportConfiguration")) &&
                (configuration.ContainsKey("TransportConfiguration")))
            {
                configuration["RegistrationController"].Add(configuration["TransportConfiguration"]);
            }
            RegistrationController = (IRegistrationController)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(null, configuration["RegistrationController"]);
        }
        #endregion
        #region Private Fields
        private string _ClientServerId = "";
        private object _SyncRootEnvelopeCache = new object();
        private Cache.ICache<TransportEnvelopeGroup> _EnvelopeCache = new Cache.Cache<TransportEnvelopeGroup>();
        private object _SyncRootSeenMessagesCache = new object();
        private Cache.ICache<string> _SeenMessagesCache = new Cache.Cache<string>();
        private object _SyncRootTransportStatistics = new object();
        private ITransportStatistics _TransportStatistics = new TransportStatistics();
        #endregion
        #region ITransportController Methods
        /// <summary>
        /// Gets configuration information required by the transportation layer.
        /// </summary>
        public ITransportConfiguration TransportConfiguration { get; private set; } = null;
        /// <summary>
        /// Used to register and unregister clients.
        /// </summary>
        public IRegistrationController RegistrationController { get; } = null;
        /// <summary>
        /// Gets the actual <see cref="ITransportStatistics"/> from the <see cref="TransportController"/>.
        /// </summary>
        public ITransportStatistics TransportStatisticsLive
        {
            get
            {
                lock (_SyncRootTransportStatistics)
                {
                    return PopulateTransportStatistics(_TransportStatistics);
                }
            }
        }
        /// <summary>
        /// Gets a clone of the <see cref="ITransportStatistics"/> from the <see cref="TransportController"/>.
        /// </summary>
        public ITransportStatistics TransportStatisticsSnapshot
        {
            get
            {
                lock (_SyncRootTransportStatistics)
                {
                    return Converters.ConvertersHelper.Clone(PopulateTransportStatistics(_TransportStatistics));
                }
            }
        }
        /// <summary>
        /// Prepares and converts a <see cref="IMessage"/> into a collection of <see cref="TransportEnvelope"/>s.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to prepare to transmission.</param>
        /// <returns>A collection of <see cref="TransportEnvelope"/>s created from the <paramref name="message"/>.</returns>
        public IEnumerable<TransportEnvelope> PrepareMessageForTransport(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            // update statistics
            lock (_SyncRootTransportStatistics)
            {
                _TransportStatistics.OutgoingMessages++;
            }
            // convert message to byte[]
            var worker = (new dodSON.Core.Converters.TypeSerializer<IMessage>()).ToByteArray(message);
            // compress byte[]
            worker = TransportConfiguration.Compressor.Compress(worker);
            // encrypt byte[]
            worker = TransportConfiguration.Encryptor.Encrypt(worker);
            // chunk byte[]
            IEnumerable<byte[]> chunks = null;
            if (TransportConfiguration.UseChunking)
            {
                chunks = dodSON.Core.Common.ByteArrayHelper.SplitByteArray(worker, TransportConfiguration.ChunkSize);
            }
            else
            {
                chunks = new List<byte[]>();
                ((List<byte[]>)chunks).Add(worker);
            }
            // create envelopes            
            var chunkIndex = 0;
            var converter = new dodSON.Core.Converters.TypeSerializer<ITransportEnvelopeHeader>();
            foreach (var item in chunks)
            {
                var header = new TransportEnvelopeHeader(message.Id,
                                                         message.ServerIds,
                                                         message.ClientId,
                                                         message.TargetId,
                                                         message.TypeInfo,
                                                         ++chunkIndex,
                                                         chunks.Count());
                var encryptedHeader = TransportConfiguration.Encryptor.Encrypt(TransportConfiguration.Compressor.Compress(converter.ToByteArray(header)));
                // update statistics
                lock (_SyncRootTransportStatistics)
                {
                    _TransportStatistics.OutgoingEnvelopes++;
                    _TransportStatistics.OutgoingBytes += encryptedHeader.Length + item.Length;
                }
                // 
                yield return new TransportEnvelope(encryptedHeader, item);
            }
        }
        /// <summary>
        /// Takes <see cref="TransportEnvelope"/>s and puts them together into a whole <see cref="IMessage"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to process.</param>
        /// <returns>If enough chucks have arrived to create a whole message; then that message will be returned; otherwise, <b>null</b>.</returns>
        public IMessage AddEnvelopeFromTransport(TransportEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException("envelope");
            }
            // update statistics
            lock (_SyncRootTransportStatistics)
            {
                _TransportStatistics.IncomingEnvelopes++;
                _TransportStatistics.IncomingBytes += (envelope.Header.Length + envelope.Payload.Length);
            }
            //
            envelope.HeaderActual = ExtractHeader(envelope);
            IMessage message = null;
            lock (_SyncRootEnvelopeCache)
            {
                // #### add envelope to proper group
                if (_EnvelopeCache.ContainsKey(envelope.HeaderActual.Id))
                {
                    // update existing group
                    _EnvelopeCache[envelope.HeaderActual.Id].Payload.Envelopes.Add(envelope);
                }
                else
                {
                    // create new group
                    var group = new TransportEnvelopeGroup(envelope.HeaderActual.Id, envelope.HeaderActual);
                    group.Envelopes.Add(envelope);
                    var cacheItem = new Cache.CacheItem<TransportEnvelopeGroup>(group,
                                                        new Cache.ValidateTime<TransportEnvelopeGroup>(DateTime.Now.Add(TransportConfiguration.EnvelopeCacheTimeLimit)));
                    _EnvelopeCache.AddItem(envelope.HeaderActual.Id, cacheItem);
                }

                // #### check for completed message
                string processedMessageId = "";
                foreach (var cacheItem in _EnvelopeCache.Items)
                {
                    var expectedTotal = cacheItem.Payload.Header.ChunkTotal;
                    var total = cacheItem.Payload.Envelopes.Count;
                    if (expectedTotal == total)
                    {
                        processedMessageId = cacheItem.Payload.Id;
                        var worker = Common.ByteArrayHelper.RestoreByteArray(from x in cacheItem.Payload.Envelopes
                                                                             orderby x.HeaderActual.ChunkIndex ascending
                                                                             select x.Payload);
                        worker = TransportConfiguration.Encryptor.Decrypt(worker);
                        worker = TransportConfiguration.Compressor.Decompress(worker);
                        message = (new dodSON.Core.Converters.TypeSerializer<IMessage>()).FromByteArray(worker);
                        // update statistics
                        lock (_SyncRootTransportStatistics)
                        {
                            _TransportStatistics.IncomingMessages++;
                        }
                    }
                }
                // #### remove processed message
                if (_EnvelopeCache.ContainsKey(processedMessageId))
                {
                    _EnvelopeCache.RemoveItem(processedMessageId);
                }
                // courtesy purge
                _EnvelopeCache.Purge();

                // #### check if message has already been processed
                if (message != null)
                {
                    // #### primary gate --> cached message id
                    lock (_SyncRootSeenMessagesCache)
                    {
                        // check cache for seen message
                        if (_SeenMessagesCache.ContainsKey(message.Id))
                        {
                            // reset cached item
                            _SeenMessagesCache[message.Id].Validator.Reset();
                            // message stops here.
                            return null;
                        }
                        // add message.Id to cache
                        _SeenMessagesCache.AddItem(message.Id, new Cache.CacheItem<string>(message.Id, new Cache.ValidateTime<string>(DateTime.Now + TransportConfiguration.SeenMessageCacheTimeLimit)));
                        // courtesy purge
                        _SeenMessagesCache.Purge();
                    }

                    // #### fail-safe gate --> server id list
                    // check if server id in ServerIds list
                    if (message.ServerIds.Split(',').Contains(TransportConfiguration.ServerId))
                    {
                        // message stops here.
                        return null;
                    }
                    // add server id to ServerIds list
                    var comma = (message.ServerIds.Length == 0) ? "" : ",";
                    message.ServerIds += comma + TransportConfiguration.ServerId;
                }

                // #### allow message to continue...
                return message;
            }
        }
        /// <summary>
        /// The number of envelope groups currently in the envelope cache.
        /// </summary>
        public int EnvelopeCacheCount
        {
            get
            {
                lock (_SyncRootEnvelopeCache)
                {
                    return _EnvelopeCache.Count;
                }
            }
        }
        /// <summary>
        /// Will clear the envelope cache.
        /// </summary>
        /// <returns>A collection of <see cref="ITransportEnvelopeHeader"/>s with its accompanying collection of <see cref="TransportEnvelope"/>s.</returns>
        public IEnumerable<Tuple<ITransportEnvelopeHeader, IEnumerable<TransportEnvelope>>> PurgeEnvelopeCache()
        {
            lock (_SyncRootEnvelopeCache)
            {
                foreach (var item in _EnvelopeCache.Purge())
                {
                    yield return new Tuple<ITransportEnvelopeHeader, IEnumerable<TransportEnvelope>>(item.Payload.Header, item.Payload.Envelopes);
                }
            }
        }
        /// <summary>
        /// The number of message ids currently in the seen messages cache.
        /// </summary>
        public int SeenMessagesCacheCount
        {
            get
            {
                lock (_SyncRootSeenMessagesCache)
                {
                    return _SeenMessagesCache.Count;
                }
            }
        }
        /// <summary>
        /// Will clear the seen messages cache.
        /// </summary>
        /// <returns>A collection of message ids which have already passed.</returns>
        public IEnumerable<string> PurgeSeenMessagesCache()
        {
            lock (_SyncRootSeenMessagesCache)
            {
                foreach (var item in _SeenMessagesCache.Purge())
                {
                    yield return item.Payload;
                }
            }
        }
        /// <summary>
        /// Extracts the Envelope's header.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to retrieve the header from.</param>
        /// <returns>The <see cref="ITransportEnvelopeHeader"/> belonging to the given <see cref="TransportEnvelope"/>.</returns>
        public ITransportEnvelopeHeader ExtractHeader(TransportEnvelope envelope)
        {
            return (new Converters.TypeSerializer<ITransportEnvelopeHeader>())
                        .FromByteArray(TransportConfiguration.Compressor
                        .Decompress(TransportConfiguration.Encryptor
                        .Decrypt(envelope.Header)));
        }
        ///// <summary>
        ///// Updates the <see cref="TransportEnvelope"/> with the <paramref name="newHeader"/>.
        ///// </summary>
        ///// <param name="envelope">The <see cref="TransportEnvelope"/> to update.</param>
        ///// <param name="newHeader">The new <see cref="ITransportEnvelopeHeader"/>.</param>
        //public void UpdateHeader(TransportEnvelope envelope,
        //                         ITransportEnvelopeHeader newHeader)
        //{
        //    envelope.Header = _TransportConfiguration.Encryptor
        //                            .Encrypt(_TransportConfiguration.Compressor
        //                            .Compress((new Converters.TypeSerializer<ITransportEnvelopeHeader>())
        //                            .ToByteArray(newHeader)));
        //}
        #endregion
        #region ITransportControllerAdvanced Methods
        /// <summary>
        /// Replaces the current <see cref="ITransportController.TransportConfiguration"/> with the given <see cref="ITransportConfiguration"/>.
        /// </summary>
        /// <param name="transportConfiguration">The new <see cref="ITransportConfiguration"/>.</param>
        void ITransportControllerAdvanced.SetTransportConfiguration(ITransportConfiguration transportConfiguration) => TransportConfiguration = transportConfiguration ?? throw new ArgumentNullException("transportConfiguration");
        /// <summary>
        /// Set the id of the host client or server.
        /// </summary>
        /// <param name="id">The new, client or server, id.</param>
        void ITransportControllerAdvanced.SetClientServerId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException("id");
            }
            _ClientServerId = id;
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
                var result = new Configuration.ConfigurationGroup("TransportController");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // TransportConfiguration
                if (TransportConfiguration != null)
                {
                    result.Add(TransportConfiguration.Configuration);
                }
                // RegistrationController
                result.Add(RegistrationController.Configuration);
                // remove RegistrationController from registration 
                if ((TransportConfiguration != null) &&
                    (result["RegistrationController"].ContainsKey("TransportConfiguration")))
                {
                    result["RegistrationController"].Remove("TransportConfiguration");
                }
                return result;
            }
        }
        #endregion
        #region Private Methods
        private ITransportStatistics PopulateTransportStatistics(ITransportStatistics transportStatistics)
        {
            // TODO: populate transportStatistics? this need to be checked out, I think all the stats are already here...

            transportStatistics.ClientServerId = _ClientServerId;
            return transportStatistics;
        }
        #endregion
        #region Private Class: TransportEnvelopeGroup
        [Serializable]
        private class TransportEnvelopeGroup
        {
            private TransportEnvelopeGroup() => Envelopes = new List<TransportEnvelope>();
            public TransportEnvelopeGroup(string id, ITransportEnvelopeHeader header)
                : this()
            {
                Id = id;
                Header = header;
            }

            public string Id
            {
                get; set;
            }
            public ITransportEnvelopeHeader Header
            {
                get; set;
            }
            public List<TransportEnvelope> Envelopes
            {
                get; set;
            }
        }
        #endregion
    }
}
