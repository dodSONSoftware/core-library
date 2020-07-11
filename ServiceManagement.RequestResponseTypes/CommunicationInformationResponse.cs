using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the communication system.
    /// </summary>
    [Serializable]
    public class CommunicationInformationResponse
    {
        #region Ctor
        private CommunicationInformationResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="CommunicationInformationResponse"/> with the given parameters.
        /// </summary>
        /// <param name="serverType">The <see cref="Type"/> representing the server; this <see cref="Type"/> must implement the <see cref="Networking.IServer"/> interface.</param>
        /// <param name="statistics">Gets the <see cref="Networking.ITransportStatistics"/> for the <see cref="Networking.TransportController"/>.</param>
        public CommunicationInformationResponse(string serverType,
                                                Networking.ITransportStatistics statistics)
            : this()
        {
            ServerType = serverType;
            Statistics = statistics;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The <see cref="Type"/> representing the server; this <see cref="Type"/> must implement the <see cref="Networking.IServer"/> interface.
        /// </summary>
        string ServerType
        {
            get;
        }
        /// <summary>
        /// Gets the <see cref="Networking.ITransportStatistics"/> for the <see cref="Networking.TransportController"/>.
        /// </summary>
        Networking.ITransportStatistics Statistics
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"DateStarted={Statistics.DateStarted}, RunTime={Statistics.RunTime}, ServerType={ServerType}";
        #endregion
    }
}
