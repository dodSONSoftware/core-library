using System;
using System.Collections.Generic;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines statistical information about the communication system.
    /// </summary>
    public interface ITransportStatisticsGroup
    {
        /// <summary>
        /// Statistics about the server.
        /// </summary>
        ITransportStatistics ServerStatistics { get; }
        /// <summary>
        /// Statistics about the sever's internal client.
        /// </summary>
        ITransportStatistics ServersClientStatistics { get; }
        /// <summary>
        /// A collection containing statistics about each client connected to the server at the time of the request.
        /// </summary>
        IEnumerable<ITransportStatistics> AllClientsStatistics { get; }
    }
}
