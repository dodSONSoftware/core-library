using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Configuration information for a server.
    /// </summary>
    public interface IServerConfiguration
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The server's unique id.
        /// </summary>
        string Id { get; }
        /// <summary>
        /// A collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.
        /// </summary>
        System.Collections.Generic.IEnumerable<IPayloadTypeInfo> OverrideTypesFilter { get; }
        /// <summary>
        /// Will replace the <see cref="OverrideTypesFilter"/>.
        /// </summary>
        /// <param name="list">The new collection of <see cref="IPayloadTypeInfo"/> representing the types this server will send to clients regardless of the client's <see cref="IClientConfiguration.ReceivableTypesFilter"/>.</param>
        void ReplaceOverrideTypesFilter(System.Collections.Generic.IEnumerable<IPayloadTypeInfo> list);
    }
}
