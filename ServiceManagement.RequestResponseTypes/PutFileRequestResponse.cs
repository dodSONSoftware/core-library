using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the <see cref="IServiceManager"/>'s response to a <see cref="RequestResponseCommands.PutFileRequest"/> command.
    /// </summary>
    [Serializable]
    public class PutFileRequestResponse
    {
        #region Ctor
        private PutFileRequestResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="PutFileRequestResponse"/> object with the given parameters.
        /// </summary>
        /// <param name="xmlPublicKey">The Public Key to use to encode a <see cref="RequestResponseTypes.PutFileInformation"/> object.</param>
        /// <param name="hashAlgorithmType">The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create a hash.</param>
        /// <param name="chunkLength">The length, in bytes, that each file segment will be split into.</param>
        /// <param name="chunkTransferInterstitialDelay">The amount of time to wait between sending each file segment.</param>
        public PutFileRequestResponse(string xmlPublicKey,
                                      string hashAlgorithmType,
                                      long chunkLength,
                                      TimeSpan chunkTransferInterstitialDelay)
            : this()
        {
            if (string.IsNullOrWhiteSpace(xmlPublicKey))
            {
                throw new ArgumentNullException(nameof(xmlPublicKey));
            }
            XmlPublicKey = xmlPublicKey;
            //
            if (string.IsNullOrWhiteSpace(hashAlgorithmType))
            {
                throw new ArgumentNullException(nameof(hashAlgorithmType));
            }
            HashAlgorithmType = hashAlgorithmType;
            //
            ChunkLength = chunkLength;
            //
            ChunkTransferInterstitialDelay = chunkTransferInterstitialDelay;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The Public Key to use to encode the <see cref="RequestResponseTypes.PutFileInformation"/> object when used with the <see cref="RequestResponseCommands.PutFileRequest"/> command.
        /// </summary>
        public string XmlPublicKey
        {
            get;
        }
        /// <summary>
        /// The type of <see cref="System.Security.Cryptography.HashAlgorithm"/> used to create a hash.
        /// </summary>
        public string HashAlgorithmType
        {
            get;
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="HashAlgorithmType"/>.
        /// </summary>
        public System.Security.Cryptography.HashAlgorithm HashAlgorithm => Common.InstantiationHelper.InvokeDefaultCtor(Type.GetType(HashAlgorithmType, true)) as System.Security.Cryptography.HashAlgorithm;
        /// <summary>
        /// The length, in bytes, that each file segment will be split into.
        /// </summary>
        public long ChunkLength
        {
            get;
        }
        /// <summary>
        /// The amount of time to wait between sending each file segment.
        /// </summary>
        public TimeSpan ChunkTransferInterstitialDelay
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"XmlPublicKey.Length={XmlPublicKey.Length}, HashAlgorithmType={HashAlgorithmType}, ChunkLength={ChunkLength}, ChunkTransferInterstitialDelay={ChunkTransferInterstitialDelay}";
        #endregion
    }
}
