using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Conveys information about a file being transfered to the service manager.
    /// </summary>
    [Serializable]
    public class PutFileInformation
    {
        #region Ctor
        private PutFileInformation()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="PutFileInformation"/> object with the given parameters.
        /// </summary>
        /// <param name="filename">The name of the file being transfered.</param>
        /// <param name="fileLength">The length, in bytes, of the <paramref name="filename"/>.</param>
        /// <param name="hash">The hash of the <paramref name="filename"/>.</param>
        public PutFileInformation(string filename,
                                  long fileLength,
                                  byte[] hash)
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
        /// The hash value created from the <see cref="Filename"/> using a <see cref="System.Security.Cryptography.HashAlgorithm"/> based hash generator.
        /// </summary>
        public byte[] Hash
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Filename={Filename}, File Length={Common.ByteCountHelper.ToString(FileLength)} ({FileLength:N0})";
        #endregion
    }
}