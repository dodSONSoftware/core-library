using System;

namespace dodSON.Core.Networking
{

    // TODO: consider adding >> bool Handled { get; set; }

    /// <summary>
    /// Defines a message which can be transported through a communication channel.
    /// </summary>
    public interface IMessage
    {
        /// <summary>
        /// The unique id for this message.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The list of servers this message has been processed by.
        /// </summary>
        string ServerIds { get; set; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.
        /// </summary>
        string ClientId { get; set; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.
        /// </summary>
        string TargetId { get; set; }
        /// <summary>
        /// Type information about the <see cref="Payload"/>.
        /// </summary>
        IPayloadTypeInfo TypeInfo { get; }
        /// <summary>
        /// The data being transported.
        /// </summary>
        byte[] Payload { get; }
        /// <summary>
        /// Converts the <see cref="Payload"/> into the type <typeparamref name="T"/> or null if unable to convert to desired type.
        /// </summary>
        /// <typeparam name="T">The type to convert the <see cref="Payload"/> to.</typeparam>
        /// <returns>The <see cref="Payload"/> converted into type <typeparamref name="T"/> or null if unable to convert to desired type.</returns>
        T PayloadMessage<T>();
    }
}
