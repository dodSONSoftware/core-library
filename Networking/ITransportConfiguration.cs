using System;
using System.Collections.Generic;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines configuration information required by the transportation layer.
    /// </summary>
    public interface ITransportConfiguration
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The unique id for the server.
        /// </summary>
        string ServerId { get; }
        /// <summary>
        /// The type of compressor to use when compressing messages.
        /// </summary>
        Type CompressorType { get; }
        /// <summary>
        /// Gets an instance of the <see cref="CompressorType"/>.
        /// </summary>
        Compression.ICompressor Compressor { get; }
        /// <summary>
        /// Gets the collection of encryption configurations used when encrypting messages.
        /// </summary>
        IEnumerable<Cryptography.IEncryptorConfiguration> EncryptorConfigurations { get; }
        /// <summary>
        /// Gets an <see cref="Cryptography.IEncryptor"/> instance using the <see cref="EncryptorConfigurations"/>.
        /// </summary>
        Cryptography.IEncryptor Encryptor { get; }
        /// <summary>
        /// Determines if the transport layer will break messages into pieces before sending.
        /// </summary>
        bool UseChunking { get; }
        /// <summary>
        /// If <see cref="UseChunking"/> is <b>true</b>, messages will be broken into pieces <see cref="ChunkSize"/> or smaller.
        /// </summary>
        int ChunkSize { get; }
        /// <summary>
        /// The amount of time an incomplete message will remain before the system abandons the message.
        /// </summary>
        TimeSpan EnvelopeCacheTimeLimit { get; }
        /// <summary>
        /// The amount of time a message's id will remain in the 'already been seen' cache before the system abandons the message's id.
        /// </summary>
        TimeSpan SeenMessageCacheTimeLimit { get; }
    }
}
