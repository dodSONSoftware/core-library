using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Provides session information.
    /// </summary>
    [Serializable]
    public class IsSessionAlive
    {
        #region Ctor
        private IsSessionAlive()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="IsSessionAlive"/> object with the given parameters.
        /// </summary>
        /// <param name="results">Whether the <paramref name="sessionId"/> is alive or not.</param>
        /// <param name="failureDescription">Describes the reason for the failure.</param>
        /// <param name="sessionId">The session id to check.</param>
        /// <param name="clientId">The id of the requesting <see cref="Networking.IClient"/>.</param>
        /// <param name="createdUtc">When the session was created, in Universal Coordinated Time.</param>
        /// <param name="runtime">How long the session has been running.</param>
        public IsSessionAlive(bool results, 
                              string failureDescription, 
                              string sessionId, 
                              string clientId, 
                              DateTimeOffset createdUtc, 
                              TimeSpan runtime)
        {
            Results = results;
            FailureDescription = failureDescription;
            SessionId = sessionId;
            ClientId = clientId;
            CreatedUtc = createdUtc;
            RunTime = runtime;
        }
        #endregion
        /// <summary>
        /// Whether the <see cref="IsSessionAlive.SessionId"/> is alive or not.
        /// </summary>
        public bool Results
        {
            get;
        }
        /// <summary>
        /// Describes the reason for the failure.
        /// </summary>
        public string FailureDescription
        {
            get;
        }
        /// <summary>
        /// The session id to check.
        /// </summary>
        public string SessionId
        {
            get;
        }
        /// <summary>
        /// The id of the requesting <see cref="Networking.IClient"/>.
        /// </summary>
        public string ClientId
        {
            get;
        }
        /// <summary>
        /// When the session was created, in Universal Coordinated Time.
        /// </summary>
        public DateTimeOffset CreatedUtc
        {
            get;
        }
        /// <summary>
        /// How long the session has been running.
        /// </summary>
        public TimeSpan RunTime
        {
            get;
        }
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString()
        {
            var details = (string.IsNullOrWhiteSpace(FailureDescription)) ? "" : $", {FailureDescription}";
            var timingDetails = (CreatedUtc == DateTimeOffset.MinValue) ? "" : $", CreatedUtc={CreatedUtc}, Runtime={RunTime}";
            return $"IsSessionAlive={Results}{details}{timingDetails}, ClientId={ClientId}, SessionId={SessionId}";
        }
        #endregion
    }
}
