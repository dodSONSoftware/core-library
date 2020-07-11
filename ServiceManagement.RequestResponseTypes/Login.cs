using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information required to log in to an <see cref="IServiceManager"/>.
    /// </summary>
    [Serializable]
    public class Login
    {
        #region Ctor
        private Login()
        {
        }
        /// <summary>
        /// Creates a new instance of the <see cref="Login"/> class.
        /// </summary>
        /// <param name="xmlPublicKey">The public key part of a public/private key pair used for one-way information encryption.</param>
        /// <param name="loginEvidence">The evidence required to log into a service manager.</param>
        public Login(string xmlPublicKey, byte[] loginEvidence)
            : this()
        {
            if (string.IsNullOrWhiteSpace(xmlPublicKey))
            {
                throw new ArgumentNullException(nameof(xmlPublicKey));
            }
            XmlPublicKey = xmlPublicKey;
            //
            if ((loginEvidence == null) || (loginEvidence.Length == 0))
            {
                throw new ArgumentNullException(nameof(loginEvidence));
            }
            LoginEvidence = loginEvidence;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The public key part of a public/private key pair used for one-way information encryption.
        /// </summary>
        public string XmlPublicKey
        {
            get;
        }
        /// <summary>
        /// The evidence required to log into a service manager.
        /// </summary>
        public byte[] LoginEvidence
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"XmlPublicKey.Length={XmlPublicKey.Length}";
        #endregion
    }
}
