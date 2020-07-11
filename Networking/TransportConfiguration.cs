using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides configuration information required by the transportation layer.
    /// </summary>
    [Serializable]
    public class TransportConfiguration
        : ITransportConfiguration,
          ITransportConfigurationAdvanced
    {
        #region Ctor
        private TransportConfiguration() { }
        /// <summary>
        /// Creates an instance of <see cref="TransportConfiguration"/>.
        /// </summary>
        /// <param name="encryptorConfiguration">An encryption configuration used to encrypt messages.</param>
        /// <param name="compressorType">The type of compressor to use when compressing messages.</param>
        /// <param name="useChunking">Determines if the transport layer will break messages into pieces before sending.</param>
        /// <param name="chunkSize">If <paramref name="useChunking"/> is <b>true</b>, messages will be broken into pieces <see cref="ChunkSize"/> or smaller.</param>
        /// <param name="envelopeCacheTimeLimit">The amount of time an incomplete message will remain before the system abandons the message.</param>
        /// <param name="seenMessageCacheTimeLimit">The amount of time a message's id will remain in the 'already been seen' cache before the system abandons the message's id.</param>
        public TransportConfiguration(Cryptography.IEncryptorConfiguration encryptorConfiguration,
                                      Type compressorType,
                                      bool useChunking,
                                      int chunkSize,
                                      TimeSpan envelopeCacheTimeLimit,
                                      TimeSpan seenMessageCacheTimeLimit) : this(new List<Cryptography.IEncryptorConfiguration>() { encryptorConfiguration }, compressorType, useChunking, chunkSize, envelopeCacheTimeLimit, seenMessageCacheTimeLimit)
        {
        }
        /// <summary>
        /// Creates an instance of <see cref="TransportConfiguration"/>.
        /// </summary>
        /// <param name="encryptorConfigurations">The encryption configurations used to encrypt messages.</param>
        /// <param name="compressorType">The type of compressor to use when compressing messages.</param>
        /// <param name="useChunking">Determines if the transport layer will break messages into pieces before sending.</param>
        /// <param name="chunkSize">If <paramref name="useChunking"/> is <b>true</b>, messages will be broken into pieces <see cref="ChunkSize"/> or smaller.</param>
        /// <param name="envelopeCacheTimeLimit">The amount of time an incomplete message will remain before the system abandons the message.</param>
        /// <param name="seenMessageCacheTimeLimit">The amount of time a message's id will remain in the 'already been seen' cache before the system abandons the message's id.</param>
        public TransportConfiguration(IEnumerable<Cryptography.IEncryptorConfiguration> encryptorConfigurations,
                                      Type compressorType,
                                      bool useChunking,
                                      int chunkSize,
                                      TimeSpan envelopeCacheTimeLimit,
                                      TimeSpan seenMessageCacheTimeLimit)
            : this()
        {
            if (chunkSize < NetworkingHelper.MinimumTransportEnvelopeChunkSize) { throw new ArgumentOutOfRangeException(string.Format("chunkSize too small. Minimum Chunk Size= {0}", NetworkingHelper.MinimumTransportEnvelopeChunkSize)); }
            if (envelopeCacheTimeLimit < TimeSpan.Zero) { throw new ArgumentOutOfRangeException("envelopeCacheTimeLimit cannot be less than TimeSpan.Zero."); }
            if (seenMessageCacheTimeLimit < TimeSpan.Zero) { throw new ArgumentOutOfRangeException("seenMessageCacheTimeLimit cannot be less than TimeSpan.Zero."); }
            var found = false;
            if (encryptorConfigurations != null) { foreach (var item in encryptorConfigurations) { if (item != null) { found = true; break; } } }
            if (found) { EncryptorConfigurations = encryptorConfigurations; }
            CompressorType = compressorType;
            UseChunking = useChunking;
            ChunkSize = chunkSize;
            EnvelopeCacheTimeLimit = envelopeCacheTimeLimit;
            SeenMessageCacheTimeLimit = seenMessageCacheTimeLimit;
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public TransportConfiguration(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "TransportConfiguration") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"TransportConfiguration\". Configuration Key={configuration.Key}", nameof(configuration)); }
            // Compressor
            CompressorType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Compressor", typeof(Type)).Value);
            // UseChunking
            UseChunking = (bool)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "UseChunking", typeof(bool)).Value; 
            // ChunkSize
            var chunkSize = (int)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "ChunkSize", typeof(int)).Value;
            if (chunkSize < NetworkingHelper.MinimumTransportEnvelopeChunkSize) { throw new ArgumentOutOfRangeException(string.Format("ChunkSize too small. Minimum Chunk Size= {0}", NetworkingHelper.MinimumTransportEnvelopeChunkSize)); }
            ChunkSize = chunkSize;
            // EnvelopeCacheTimeLimit 
            var envelopeCacheTimeLimit = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "EnvelopeCacheTimeLimit", typeof(TimeSpan)).Value;
            if (envelopeCacheTimeLimit < TimeSpan.Zero) { throw new ArgumentOutOfRangeException("EnvelopeCacheTimeLimit cannot be less than TimeSpan.Zero."); }
            EnvelopeCacheTimeLimit = envelopeCacheTimeLimit;
            // SeenMessageCacheTimeLimit 
            var seenMessageCacheTimeLimit = (TimeSpan)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SeenMessageCacheTimeLimit", typeof(TimeSpan)).Value;
            if (seenMessageCacheTimeLimit < TimeSpan.Zero) { throw new ArgumentOutOfRangeException("SeenMessageCacheTimeLimit cannot be less than TimeSpan.Zero."); }
            SeenMessageCacheTimeLimit = seenMessageCacheTimeLimit;
            // EncryptorConfigurations
            if (configuration.ContainsKey("EncryptorConfigurations"))
            {
                var configs = new List<Cryptography.IEncryptorConfiguration>();
                foreach (var group in configuration["EncryptorConfigurations"])
                {
                    ((dodSON.Core.Configuration.IConfigurationGroupAdvanced)group).SetKey("EncryptorConfiguration");
                    var dude = (Cryptography.IEncryptorConfiguration)Core.Configuration.ConfigurationHelper.InstantiateTypeFromConfigurationGroup(typeof(Cryptography.EncryptorConfiguration), group);
                    configs.Add(dude);
                }
                EncryptorConfigurations = configs;
            }
        }

        #endregion
        #region Private Fields
        [NonSerialized]
        private Cryptography.IEncryptor _EncryptorActual = null;
        [NonSerialized]
        private Compression.ICompressor _CompressorActual = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// The unique id for the server.
        /// </summary>
        public string ServerId { get; private set; }
        /// <summary>
        /// Gets an instance of the <see cref="CompressorType"/>.
        /// </summary>
        public dodSON.Core.Compression.ICompressor Compressor
        {
            get
            {
                if (_CompressorActual == null)
                {
                    if (CompressorType == null) { _CompressorActual = new Compression.NullCompressor(); }
                    else { _CompressorActual = Activator.CreateInstance(CompressorType) as dodSON.Core.Compression.ICompressor; }
                }
                return _CompressorActual;
            }
        }
        /// <summary>
        /// The type of compressor to use when compressing messages.
        /// </summary>
        public Type CompressorType { get; } = null;
        /// <summary>
        /// Gets an <see cref="Cryptography.IEncryptor"/> instance using the <see cref="EncryptorConfigurations"/>.
        /// </summary>
        public Cryptography.IEncryptor Encryptor
        {
            get
            {
                if (_EncryptorActual == null)
                {
                    if (EncryptorConfigurations == null) { _EncryptorActual = new Cryptography.NullEncryptor(); }
                    else { _EncryptorActual = new Cryptography.StackableEncryptor(EncryptorConfigurations); }
                }
                return _EncryptorActual;
            }
        }
        /// <summary>
        /// Gets the collection of encryption configurations used when encrypting messages.
        /// </summary>
        public IEnumerable<Cryptography.IEncryptorConfiguration> EncryptorConfigurations { get; }
        /// <summary>
        /// Determines if the transport layer will break messages into pieces before sending.
        /// </summary>
        public bool UseChunking { get; } = true;
        /// <summary>
        /// If <see cref="UseChunking"/> is <b>true</b>, messages will be broken into pieces <see cref="ChunkSize"/> or smaller.
        /// </summary>
        public int ChunkSize { get; } = Networking.NetworkingHelper.MaximumNamedPipeTransportEnvelopeChunkSize;
        /// <summary>
        /// The amount of time an incomplete message will remain before the system abandons the message.
        /// </summary>
        public TimeSpan EnvelopeCacheTimeLimit { get; } = TimeSpan.FromMinutes(55);
        /// <summary>
        /// The amount of time a message's id will remain in the 'already been seen' cache before the system abandons the message's id.
        /// </summary>
        public TimeSpan SeenMessageCacheTimeLimit { get; } = TimeSpan.FromMinutes(55);
        #endregion
        #region ITransportConfigurationAdvanced Methods
        /// <summary>
        /// Sets the Id of the server.
        /// </summary>
        /// <param name="id">The Id for the server.</param>
        void ITransportConfigurationAdvanced.SetServerId(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException(nameof(id)); }
            ServerId = id;
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
                var result = new Configuration.ConfigurationGroup("TransportConfiguration");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                result.Items.Add("EnvelopeCacheTimeLimit", EnvelopeCacheTimeLimit, EnvelopeCacheTimeLimit.GetType());
                result.Items.Add("SeenMessageCacheTimeLimit", SeenMessageCacheTimeLimit, SeenMessageCacheTimeLimit.GetType());
                result.Items.Add("ChunkSize", ChunkSize, ChunkSize.GetType());
                result.Items.Add("Compressor", CompressorType, CompressorType.GetType());
                result.Items.Add("UseChunking", UseChunking, UseChunking.GetType());
                if ((EncryptorConfigurations != null) && (EncryptorConfigurations.Count() > 0))
                {
                    result.Add("EncryptorConfigurations");
                    var encryptorCounter = 0;
                    foreach (var item in EncryptorConfigurations)
                    {
                        var encryptConfig = item.Configuration;
                        ((dodSON.Core.Configuration.IConfigurationGroupAdvanced)encryptConfig).SetKey("Encryptor" + (++encryptorCounter).ToString("00"));
                        result["EncryptorConfigurations"].Add(encryptConfig);
                    }
                }
                //
                return result;
            }
        }
        #endregion
    }
}
