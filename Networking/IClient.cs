using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents a communication channel client.
    /// </summary>
    public interface IClient
        : IChannel
    {
        /// <summary>
        /// Client configuration information.
        /// </summary>
        IClientConfiguration ClientConfiguration { get; }
        /// <summary>
        /// Will prepare the client to join a communication system.
        /// </summary>
        /// <param name="channelAddress">Communication system addressing specifications.</param>
        /// <param name="configuration">This client's configuration.</param>
        /// <param name="registrationController">This client's registration controller.</param>
        void Initialize(IChannelAddress channelAddress, IClientConfiguration configuration, IRegistrationController registrationController);
        /// <summary>
        /// Commands the client to request from the server all transport statistics from the server and all connected clients.
        /// </summary>
        /// <returns>Statistical information about the communication system.</returns>
        ITransportStatisticsGroup RequestAllTransportStatistics();
    }
}
