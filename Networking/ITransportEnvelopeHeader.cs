using System;
using System.Collections.Generic;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines header information required for each <see cref="TransportEnvelope"/> in order to be delivered and reassembled by the communication system.
    /// </summary>
    public interface ITransportEnvelopeHeader
    {
        /// <summary>
        /// The unique id.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// The list of servers that have processed this <see cref="IMessage"/>.
        /// </summary>
        string ServerIds { get; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/> this <see cref="TransportEnvelope"/> is part of.
        /// </summary>
        string ClientId { get; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which should receive the <see cref="TransportEnvelope"/>.
        /// </summary>
        string TargetId { get; }
        /// <summary>
        /// The type information about the payload.
        /// </summary>
        IPayloadTypeInfo PayloadTypeInfo { get; }
        /// <summary>
        /// The index for this <see cref="TransportEnvelope"/>. (One-based indexing)
        /// </summary>
        int ChunkIndex { get; }
        /// <summary>
        /// The total number of <see cref="TransportEnvelope"/>s in this <see cref="IMessage"/>. (One-based indexing)
        /// </summary>
        int ChunkTotal { get; }
        /// <summary>
        /// Determines if the <see cref="IPayloadTypeInfo"/> information matches the type in the payload.
        /// </summary>
        /// <param name="typeInfo">The <see cref="IPayloadTypeInfo"/> to test for.</param>
        /// <returns><b>True</b> if the payload type matches the <paramref name="typeInfo"/>; otherwise, <b>false</b>.</returns>
        bool IsEnvelopeType(IPayloadTypeInfo typeInfo);
        /// <summary>
        /// Determines if <typeparamref name="T"/> matches the type in the payload.
        /// </summary>
        /// <typeparam name="T">The type to test for.</typeparam>
        /// <returns><b>True</b> if the payload type matches the <typeparamref name="T"/>; otherwise, <b>false</b>.</returns>
        bool IsEnvelopeType<T>() where T : class;
    }
}
