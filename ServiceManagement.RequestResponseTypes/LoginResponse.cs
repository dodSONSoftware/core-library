using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Provides a mechanism to relay login information.
    /// </summary>
    [Serializable]
    public class LoginResponse
    {
        #region Ctor
        private LoginResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="LoginResponse"/>.
        /// </summary>
        /// <param name="loginSuccessful">Indicates whether the login attempt was successful or not.</param>
        /// <param name="description">States the success or failure of the login attempt.</param>
        /// <param name="serviceManagerId">The id of the responding <see cref="IServiceManager"/>.</param>
        /// <param name="sessionId">The id for the session create on a successful login.</param>
        public LoginResponse(bool loginSuccessful, string description, string serviceManagerId, string sessionId)
            : this()
        {
            // loginSuccessful
            LoginSuccessful = loginSuccessful;
            // loginCancelResults
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }
            Description = description;
            // serviceManagerId
            ServiceManagerId = serviceManagerId ?? "";
            // sessionId
            SessionId = sessionId;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Indicates whether the login attempt was successful or not.
        /// </summary>
        public bool LoginSuccessful
        {
            get;
        }
        /// <summary>
        /// States the success or failure of the login attempt.
        /// </summary>
        public string Description
        {
            get;
        }
        /// <summary>
        /// The id of the responding <see cref="IServiceManager"/>.
        /// </summary>
        public string ServiceManagerId
        {
            get;
        }
        /// <summary>
        /// The id for the session created on a successful login.
        /// </summary>
        public string SessionId
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"LoginSuccessful={LoginSuccessful}, Description={Description}, ServiceManagerId={ServiceManagerId}, SessionId={SessionId}";
        #endregion
    }
}
