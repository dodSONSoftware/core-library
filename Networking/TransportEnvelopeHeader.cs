using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides header information required for each <see cref="TransportEnvelope"/> in order to be delivered and reassembled by the communication system.
    /// </summary>
    [Serializable]
    public class TransportEnvelopeHeader
        : ITransportEnvelopeHeader
    {
        #region Ctor
        private TransportEnvelopeHeader() { }
        /// <summary>
        /// Instantiates a new <see cref="TransportEnvelopeHeader"/>.
        /// </summary>
        /// <param name="id">The unique id.</param>
        /// <param name="serverIds">The list of servers that have processed this <see cref="IMessage"/>.</param>
        /// <param name="clientId">The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/> this <see cref="TransportEnvelope"/> is part of.</param>
        /// <param name="targetId">The id of the <see cref="IClient"/> which should receive the <see cref="TransportEnvelope"/>.</param>
        /// <param name="payloadTypeInfo">Type information about the payload.</param>
        /// <param name="chunkIndex">The index for this <see cref="TransportEnvelope"/>. (One-based indexing)</param>
        /// <param name="chunkTotal">The total number of <see cref="TransportEnvelope"/>s in this <see cref="IMessage"/>. (One-based indexing)</param>
        public TransportEnvelopeHeader(string id,
                                       string serverIds,
                                       string clientId,
                                       string targetId,
                                       IPayloadTypeInfo payloadTypeInfo,
                                       int chunkIndex,
                                       int chunkTotal)
            : this()
        {
            if (string.IsNullOrWhiteSpace(id)) { throw new ArgumentNullException("id"); }
            if (string.IsNullOrWhiteSpace(clientId)) { throw new ArgumentNullException("clientId"); }
            if (chunkIndex < 1) { throw new ArgumentOutOfRangeException("chunkIndex", "chunkIndex must be greater than or equal to one. [1 >= chunkIndex <= chunkTotal]"); }
            if (chunkIndex > chunkTotal) { throw new ArgumentOutOfRangeException("chunkIndex", "chunkIndex must be less than or equal to chunkTotal. [1 >= chunkIndex <= chunkTotal]"); }
            if (chunkTotal < 1) { throw new ArgumentOutOfRangeException("chunkTotal", "chunkTotal must be greater than or equal to one. [1 >= chunkTotal <= int.MaxValue]"); }
            Id = id;
            ServerIds = serverIds;
            ClientId = clientId;
            TargetId = targetId;
            PayloadTypeInfo = payloadTypeInfo ?? throw new ArgumentNullException(nameof(payloadTypeInfo));
            ChunkIndex = chunkIndex;
            ChunkTotal = chunkTotal;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The unique id.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// The list of servers that have processed this <see cref="IMessage"/>.
        /// </summary>
        public string ServerIds { get; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/> this <see cref="TransportEnvelope"/> is part of.
        /// </summary>
        public string ClientId { get; }
        /// <summary>
        /// The id of the <see cref="IClient"/> which should receive the <see cref="TransportEnvelope"/>.
        /// </summary>
        public string TargetId { get; }
        /// <summary>
        /// The type information about the payload.
        /// </summary>
        public IPayloadTypeInfo PayloadTypeInfo { get; }
        /// <summary>
        /// The index for this <see cref="TransportEnvelope"/>.
        /// </summary>
        public int ChunkIndex { get; }
        /// <summary>
        /// The total number of <see cref="TransportEnvelope"/>s in this <see cref="IMessage"/>.
        /// </summary>
        public int ChunkTotal { get; }
        /// <summary>
        /// Determines if the <see cref="IPayloadTypeInfo"/> information matches the type in the payload.
        /// </summary>
        /// <param name="typeInfo">The <see cref="IPayloadTypeInfo"/> to test for.</param>
        /// <returns><b>True</b> if the payload type matches the <paramref name="typeInfo"/>; otherwise, <b>false</b>.</returns>
        public bool IsEnvelopeType(IPayloadTypeInfo typeInfo)=>IsEnvelopeType(typeInfo.TypeName);
        /// <summary>
        /// Determines if <typeparamref name="T"/> matches the type in the payload.
        /// </summary>
        /// <typeparam name="T">The type to test for.</typeparam>
        /// <returns><b>True</b> if the payload type matches the <typeparamref name="T"/>; otherwise, <b>false</b>.</returns>
        public bool IsEnvelopeType<T>() where T : class
        {
            var typeInfo = typeof(T);
            return IsEnvelopeType(typeInfo.AssemblyQualifiedName);
        }
        #endregion
        #region Private Methods
        private bool IsEnvelopeType(string typeName)=>(PayloadTypeInfo.TypeName.Equals(typeName, StringComparison.InvariantCultureIgnoreCase));
        #endregion
    }
}
