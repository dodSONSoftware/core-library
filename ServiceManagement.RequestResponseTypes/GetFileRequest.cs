using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Initiates a get file process.
    /// </summary>
    [Serializable]
    public class GetFileRequest
    {
        #region Ctor
        private GetFileRequest()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="GetFileRequest"/> object with the given parameters.
        /// </summary>
        /// <param name="publicKey">The requester's public key.</param>
        /// <param name="filename">A string representation of this object.</param>
        public GetFileRequest(string publicKey,
                              string filename)
            : this()
        {
            if (string.IsNullOrWhiteSpace(publicKey))
            {
                throw new ArgumentNullException(nameof(publicKey));
            }
            PublicKey = publicKey;
            //
            if (string.IsNullOrWhiteSpace(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }
            Filename = filename;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The requester's public key.
        /// </summary>
        public string PublicKey
        {
            get;
        }
        /// <summary>
        /// The name of the file to get.
        /// </summary>
        public string Filename
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Filename={Filename}";
        #endregion
    }
}
