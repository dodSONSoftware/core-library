using System;
using dodSON.Core.ServiceManagement;

namespace dodSON.Core.ComponentManagement
{
    /// <summary>
    /// Defines access to and control of a configurable, type-based message communication system.
    /// </summary>
    /// <seealso cref="Networking"/>
    public interface ICommunicationController
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Indicates the operating state of the <see cref="ICommunicationController"/>.
        /// </summary>
        bool IsRunning { get; }
        /// <summary>
        /// The <see cref="DateTimeOffset"/> when the <see cref="ICommunicationController"/> started.
        /// </summary>
        DateTimeOffset StartTime { get; }
        /// <summary>
        /// The duration that the <see cref="ICommunicationController"/> has run. 
        /// </summary>
        TimeSpan RunTime { get; }
        /// <summary>
        /// Returns the value used as the source id when creating log entries.
        /// </summary>
        string LogSourceId { get; }
        /// <summary>
        /// Fired when activity <see cref="Logging.Logs"/> are available.
        /// </summary>
        event EventHandler<Networking.ActivityLogsEventArgs> ActivityLogsEvent;
        /// <summary>
        /// The <see cref="Type"/> representing the server; this <see cref="Type"/> must implement the <see cref="Networking.IServer"/> interface.
        /// </summary>
        Type ServerType { get; }
        /// <summary>
        /// The <see cref="Type"/> representing the client; this <see cref="Type"/> must implement the <see cref="Networking.IClient"/> interface.
        /// </summary>
        Type ClientType { get; }
        /// <summary>
        /// The <see cref="Networking.IChannelAddress"/> used by the <see cref="CreateSharedServer"/> and the <see cref="CreateSharedClient(Networking.IClientConfiguration)"/> methods.
        /// </summary>
        Networking.IChannelAddress SharedChannelAddress { get; }
        /// <summary>
        /// The <see cref="Networking.IServerConfiguration"/> used by the <see cref="CreateSharedServer"/> method.
        /// </summary>
        Networking.IServerConfiguration SharedServerConfiguration { get; }
        /// <summary>
        /// The <see cref="Networking.ITransportController"/> used by the <see cref="CreateSharedServer"/> and the <see cref="CreateSharedClient(Networking.IClientConfiguration)"/> methods.
        /// </summary>
        Networking.ITransportController TransportController { get; }
        /// <summary>
        /// Will use the <see cref="SharedChannelAddress"/> and the <see cref="SharedServerConfiguration"/> to create a <see cref="Networking.IServer"/>.
        /// </summary>
        /// <returns>Returns a <see cref="Networking.IServer"/> using the <see cref="SharedChannelAddress"/> and the <see cref="SharedServerConfiguration"/>.</returns>
        Networking.IServer CreateSharedServer();
        /// <summary>
        /// Will use the <see cref="SharedChannelAddress"/> and the given <see cref="Networking.IClientConfiguration"/> to create a <see cref="Networking.IClient"/> that will communicate with the server created using the <see cref="CreateSharedServer"/> method.
        /// </summary>
        /// <param name="configuration">The <see cref="Networking.IClientConfiguration"/> used to configure the new <see cref="Networking.IClient"/>.</param>
        /// <returns>An <see cref="Networking.IServer"/> created using the <see cref="SharedChannelAddress"/> and the given <see cref="Networking.IClientConfiguration"/></returns>
        Networking.IClient CreateSharedClient(Networking.IClientConfiguration configuration);
        /// <summary>
        /// Will use the given <see cref="Networking.IChannelAddress"/>, <see cref="Networking.IServerConfiguration"/> and LogSourceId to create a <see cref="Networking.IServer"/>.
        /// </summary>
        /// <param name="channelAddress">The <see cref="Networking.IChannelAddress"/> defining the required connection information.</param>
        /// <param name="configuration">The <see cref="Networking.IServerConfiguration"/> defining the required configuration information.</param>
        /// <param name="logSourceId"></param>
        /// <returns>An <see cref="Networking.IServer"/> created using the given <see cref="Networking.IChannelAddress"/>, <see cref="Networking.IServerConfiguration"/> and LogSourceId.</returns>
        Networking.IServer CreateServer(Networking.IChannelAddress channelAddress, Networking.IServerConfiguration configuration, string logSourceId);
        /// <summary>
        /// Will use the given <see cref="Networking.IChannelAddress"/> and <see cref="Networking.IClientConfiguration"/> to create a <see cref="Networking.IClient"/>.
        /// </summary>
        /// <param name="channelAddress">The <see cref="Networking.IChannelAddress"/> defining the required connection information.</param>
        /// <param name="configuration">The <see cref="Networking.IClientConfiguration"/> defining the required configuration information.</param>
        /// <returns>A <see cref="Networking.IClient"/> created using the given <see cref="Networking.IChannelAddress"/> and <see cref="Networking.IClientConfiguration"/>.</returns>
        Networking.IClient CreateClient(Networking.IChannelAddress channelAddress, Networking.IClientConfiguration configuration);
    }
}
