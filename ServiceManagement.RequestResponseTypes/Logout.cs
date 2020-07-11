using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Initiates a logout for the given session.
    /// </summary>
    [Serializable]
    public class Logout
    {
        #region Ctor
        private Logout()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="Logout"/> object with the given parameters.
        /// </summary>
        /// <param name="sessionId">The sessionId to search for and log out of.</param>
        public Logout(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentNullException(sessionId);
            }
            SessionId = sessionId;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The sessionId to search for and log out of.
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
        public override string ToString() => $"SessionId={SessionId}";
        #endregion
    }
}
