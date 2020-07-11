using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines what activity a group of <see cref="Logging.ILog"/>s represent.
    /// </summary>
    public enum ActivityLogsEventType
    {
        // ######## NETWORK

        /// <summary>
        /// Opening a communication channel.
        /// </summary>
        Network_Open = 0,
        /// <summary>
        /// Registering a communication channel.
        /// </summary>
        Network_Register,
        /// <summary>
        /// Unregistering a communication channel.
        /// </summary>
        Network_Unregister,
        /// <summary>
        /// Closing a communication channel.
        /// </summary>
        Network_Close,
        /// <summary>
        /// Pinging all clients on the communication system.
        /// </summary>
        Network_PingAll,
        /// <summary>
        /// Restarting all clients on the communication system.
        /// </summary>
        Network_RestartAll,
        /// <summary>
        /// Closing all clients on the communication system.
        /// </summary>
        Network_CloseAll,
        /// <summary>
        /// Requesting all clients respond with their current statistical information.
        /// </summary>
        Network_RequestAllStatistics
    }

}
