using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents statistical information about the communication system.
    /// </summary>
    [Serializable]
    public class TransportStatisticsGroup
        : ITransportStatisticsGroup
    {
        #region Ctor
        private TransportStatisticsGroup() { }
        /// <summary>
        /// Instantiates a new <see cref="TransportStatisticsGroup"/>.
        /// </summary>
        /// <param name="serverStatistics">Statistics about the server.</param>
        /// <param name="serversClientStatistics">Statistics about the sever's internal client.</param>
        /// <param name="allClientsStatistics">A collection containing statistics about each client connected to the server at the time of the request.</param>
        public TransportStatisticsGroup(ITransportStatistics serverStatistics,
                                        ITransportStatistics serversClientStatistics,
                                        IEnumerable<ITransportStatistics> allClientsStatistics)
            : this()
        {
            if (allClientsStatistics == null) { throw new ArgumentNullException("allClientsStatistics"); }
            ServerStatistics = serverStatistics ?? throw new ArgumentNullException("serverStatistics");
            ServersClientStatistics = serversClientStatistics ?? throw new ArgumentNullException("serversClientStatistics");
            _AllClientsStatistics = new List<ITransportStatistics>(allClientsStatistics);
        }
        #endregion
        #region Private Fields
        private List<ITransportStatistics> _AllClientsStatistics = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Statistics about the server.
        /// </summary>
        public ITransportStatistics ServerStatistics { get; } = null;
        /// <summary>
        /// Statistics about the sever's internal client.
        /// </summary>
        public ITransportStatistics ServersClientStatistics { get; } = null;
        /// <summary>
        /// A collection containing statistics about each client connected to the server at the time of the request.
        /// </summary>
        public IEnumerable<ITransportStatistics> AllClientsStatistics=> _AllClientsStatistics;
        #endregion
    }
}
