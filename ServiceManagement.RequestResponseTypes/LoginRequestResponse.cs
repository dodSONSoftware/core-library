using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Relays information pertaining to a <see cref="RequestResponseCommands.LoginRequest"/> command.
    /// </summary>
    [Serializable]
    public class LoginRequestResponse
    {
        #region Ctor
        private LoginRequestResponse()
        {
        }
        /// <summary>
        ///  Creates a new instance of the <see cref="LoginRequestResponse"/> class with the given Public Key.
        /// </summary>
        /// <param name="xmlPublicKey">The Public Key to use to encode the <see cref="dodSON.Core.ServiceManagement.RequestResponseTypes.LoginResponse"/> type.</param>
        public LoginRequestResponse(string xmlPublicKey)
            : this()
        {
            if (string.IsNullOrWhiteSpace(xmlPublicKey))
            {
                throw new ArgumentNullException(nameof(xmlPublicKey));
            }
            XmlPublicKey = xmlPublicKey;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The Public Key to use to encode the <see cref="LoginResponse"/> type when used with the <see cref="RequestResponseCommands.Login"/> command.
        /// </summary>
        public string XmlPublicKey
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
