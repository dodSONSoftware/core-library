using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Indicates the current state of the communication channel.
    /// </summary>
    public enum ChannelStates
    {
        /// <summary>
        /// Attempting to create an initial connection.
        /// </summary>
        Opening,
        /// <summary>
        /// Attempting to register with the server.
        /// </summary>
        Registering,
        /// <summary>
        /// Successfully connected and registered with the server. Ready to send and receive <see cref="IMessage"/>s.
        /// </summary>
        Open,
        /// <summary>
        /// Restarting. This will unregister with the server and close the connection; then it will attempt to reconnect and re-register with the server.
        /// </summary>
        Restarting,
        /// <summary>
        /// Communication channel is closing.
        /// </summary>
        Closing,
        /// <summary>
        /// Communication channel is unregistering from the server.
        /// </summary>
        Unregistering,
        /// <summary>
        /// Communication channel has successfully unregister with the server and closed the connection.
        /// </summary>
        Closed
    }
}
