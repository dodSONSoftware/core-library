using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the file download information.
    /// </summary>
    [Serializable]
    public class GetFileRequestResponse
    {
        #region Ctor
        private GetFileRequestResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="PutFileRequestResponse"/> object with the given parameters.
        /// </summary>
        /// <param name="filename">The name of the file being transfered.</param>
        /// <param name="fileLength">The length, in bytes, of the <see cref="Filename"/>.</param>
        /// <param name="hash">The hash value created from the <see cref="Filename"/> file using a <see cref="System.Security.Cryptography.HashAlgorithm"/> based hash generator.</param>
        /// <param name="hashAlgorithmType">The type string representation of the <see cref="System.Security.Cryptography.HashAlgorithm"/> based hash generator used to generate the <see cref="Hash"/> of the <see cref="Filename"/>.</param>
        /// <param name="chunkLength">Chuck Length.</param>
        /// <param name="interstitialDelay">Interstitial Delay.</param>
        public GetFileRequestResponse(string filename,
                                      long fileLength,
                                      byte[] hash,
                                      string hashAlgorithmType,
                                      long chunkLength,
                                      TimeSpan interstitialDelay)
            : this()
        {
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            Filename = filename;
            //
            FileLength = fileLength;
            //
            if ((hash == null) || (hash.Length == 0))
            {
                throw new ArgumentNullException(nameof(hash));
            }
            Hash = hash;
            //
            if (string.IsNullOrWhiteSpace(hashAlgorithmType))
            {
                throw new ArgumentNullException(nameof(hashAlgorithmType));
            }
            HashAlgorithmType = hashAlgorithmType;
            //
            ChuckLength = chunkLength;
            //
            InterstitialDelay = interstitialDelay;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the file being transfered.
        /// </summary>
        public string Filename
        {
            get;
        }
        /// <summary>
        /// The length, in bytes, of the <see cref="Filename"/>.
        /// </summary>
        public long FileLength
        {
            get;
        }
        /// <summary>
        /// The hash value created from the <see cref="Filename"/> file using a <see cref="System.Security.Cryptography.HashAlgorithm"/> based hash generator.
        /// </summary>
        public byte[] Hash
        {
            get;
        }
        /// <summary>
        /// The type string representation of the <see cref="System.Security.Cryptography.HashAlgorithm"/> based hash generator used to generate the <see cref="Hash"/> of the <see cref="Filename"/>.
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
        /// Chuck Length.
        /// </summary>
        public long ChuckLength
        {
            get; set;
        }
        /// <summary>
        /// Interstitial Delay.
        /// </summary>
        public TimeSpan InterstitialDelay
        {
            get; set;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Filename={Filename}, FileLength={Common.ByteCountHelper.ToString(FileLength)} ({FileLength:N0}), HashAlgorithmType={HashAlgorithmType}";
        #endregion
    }
}
