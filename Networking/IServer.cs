using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines a communication channel server.
    /// </summary>
    public interface IServer
        : IChannel
    {
        /// <summary>
        /// Fired when activity <see cref="Logging.Logs"/> are available.
        /// </summary>
        event EventHandler<Networking.ActivityLogsEventArgs> ActivityLogsEvent;
        /// <summary>
        /// Server configuration information.
        /// </summary>
        IServerConfiguration ServerConfiguration { get; }
        /// <summary>
        /// Will prepare the server to join a communication system.
        /// </summary>
        /// <param name="channelAddress">Communication system addressing specifications.</param>
        /// <param name="configuration">This server's configuration.</param>
        /// <param name="transportController">This server's transport controller.</param>
        /// <param name="logSourceId">The value used by the <see cref="ServerBase"/> for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.</param>
        void Initialize(IChannelAddress channelAddress, IServerConfiguration configuration, ITransportController transportController, string logSourceId);
        /// <summary>
        /// Will attempt to communicate with every registered client.
        /// </summary>
        /// <param name="cancelToken">A token used to initiate canceling the <see cref="PingAllClients(Threading.ThreadCancelToken)"/> process.</param>
        /// <returns>A collection containing each client's id and a tuple with it's round trip duration and any possible exceptions that may have occurred.</returns>
        Dictionary<string, Tuple<TimeSpan, Exception>> PingAllClients(Threading.ThreadCancelToken cancelToken);
        /// <summary>
        /// Requests that all registered clients restart.
        /// </summary>
        /// <param name="retryCount">The number of times a client should attempt to restart before it stops trying.</param>
        /// <param name="retryDuration">The duration to wait between retires.</param>
        void RestartAllClients(int retryCount, TimeSpan retryDuration);
        /// <summary>
        /// Requests that all registered clients close.
        /// </summary>
        void CloseAllClients();
        /// <summary>
        /// Requests all registered clients report their <see cref="TransportStatistics"/>.
        /// </summary>
        /// <returns>A <see cref="ITransportStatisticsGroup"/> containing all registered clients communication statistics and server statistics.</returns>
        ITransportStatisticsGroup RequestAllClientsTransportStatistics();
    }
}
