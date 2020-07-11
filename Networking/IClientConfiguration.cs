using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines configuration information for a client.
    /// </summary>
    public interface IClientConfiguration
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The client's unique id.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.
        /// </summary>
        bool ReceiveSelfSentMessages { get; }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will accept.
        /// </summary>
        System.Collections.Generic.IEnumerable<IPayloadTypeInfo> ReceivableTypesFilter { get; }
        /// <summary>
        /// Will replace the <see cref="ReceivableTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this client will accept.</param>
        void ReplaceReceivableTypesFilter(System.Collections.Generic.IEnumerable<IPayloadTypeInfo> list);
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this client will send.
        /// </summary>
        System.Collections.Generic.IEnumerable<IPayloadTypeInfo> TransmittableTypesFilter { get; }
        /// <summary>
        /// Will replace the <see cref="TransmittableTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this client will send.</param>
        void ReplaceTransmittableTypesFilter(System.Collections.Generic.IEnumerable<IPayloadTypeInfo> list);
    }
}
