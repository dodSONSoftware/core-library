using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents a communication channel.
    /// </summary>
    public interface IChannel
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The channel's unique id.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The channel's address.
        /// </summary>
        IChannelAddress Address { get; }
        /// <summary>
        /// The channel's address converted into a <see cref="Uri"/>.
        /// </summary>
        Uri AddressUri { get; }
        /// <summary>
        /// Controls the transport layer.
        /// </summary>
        ITransportController TransportController { get; }
        /// <summary>
        /// The current state of the communication channel.
        /// </summary>
        ChannelStates State { get; }
        /// <summary>
        /// Attempts to create a communication connection as either a server or a client.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong in the attempt.</param>
        /// <returns>The state of the channel's connection.</returns>
        ChannelStates Open(out Exception exception);
        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong.</param>
        /// <returns><b>True</b> if the client was successfully closed; otherwise, <b>false</b> if anything went wrong.</returns>
        bool Close(out Exception exception);
        /// <summary>
        /// Fired when a message is received.
        /// </summary>
        event EventHandler<MessageEventArgs> MessageBus;
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to send.</param>
        void SendMessage(IMessage message);
        /// <summary>
        /// Puts an <see cref="IMessage"/> into the communication system to be properly distributed.
        /// </summary>
        /// <typeparam name="T">The typeof the <paramref name="payload"/> being distributed.</typeparam>
        /// <param name="targetClientId">The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.</param>
        /// <param name="payload">The data being transported.</param>
        void SendMessage<T>(string targetClientId, T payload);
    }
}
