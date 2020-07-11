using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines properties and methods used to control the transportation layer.
    /// </summary>
    public interface ITransportController
        : Configuration.IConfigurable
    {
        /// <summary>
        /// Gets configuration information required by the transportation layer.
        /// </summary>
        ITransportConfiguration TransportConfiguration { get; }
        /// <summary>
        /// Used to register and unregister clients.
        /// </summary>
        IRegistrationController RegistrationController { get; }
        /// <summary>
        /// Gets the actual <see cref="ITransportStatistics"/> from the <see cref="TransportController"/>.
        /// </summary>
        ITransportStatistics TransportStatisticsLive { get; }
        /// <summary>
        /// Gets a clone of the <see cref="ITransportStatistics"/> from the <see cref="TransportController"/>.
        /// </summary>
        ITransportStatistics TransportStatisticsSnapshot { get; }
        /// <summary>
        /// Prepares and converts a <see cref="IMessage"/> into a collection of <see cref="TransportEnvelope"/>s.
        /// </summary>
        /// <param name="message">The <see cref="IMessage"/> to prepare to transmission.</param>
        /// <returns>A collection of <see cref="TransportEnvelope"/>s created from the <paramref name="message"/>.</returns>
        IEnumerable<TransportEnvelope> PrepareMessageForTransport(IMessage message);
        /// <summary>
        /// Takes <see cref="TransportEnvelope"/>s and puts them together into a whole <see cref="IMessage"/>.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to process.</param>
        /// <returns>If enough chucks have arrived to create a whole message; then that message will be returned; otherwise, <b>null</b>.</returns>
        IMessage AddEnvelopeFromTransport(TransportEnvelope envelope);
        /// <summary>
        /// The number of envelope groups currently in the envelope cache.
        /// </summary>
        int EnvelopeCacheCount { get; }
        /// <summary>
        /// Will clear the envelope cache.
        /// </summary>
        /// <returns>A collection of <see cref="ITransportEnvelopeHeader"/>s with its accompanying collection of <see cref="TransportEnvelope"/>s.</returns>
        IEnumerable<Tuple<ITransportEnvelopeHeader, IEnumerable<TransportEnvelope>>> PurgeEnvelopeCache();
        /// <summary>
        /// The number of message ids currently in the seen messages cache.
        /// </summary>
        int SeenMessagesCacheCount { get; }
        /// <summary>
        /// Will clear the seen messages cache.
        /// </summary>
        /// <returns>A collection of message ids which have already passed.</returns>
        IEnumerable<string> PurgeSeenMessagesCache();
        /// <summary>
        /// Extracts the Envelope's header.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to retrieve the header from.</param>
        /// <returns>The <see cref="ITransportEnvelopeHeader"/> belonging to the given <see cref="TransportEnvelope"/>.</returns>
        ITransportEnvelopeHeader ExtractHeader(TransportEnvelope envelope);
        ///// <summary>
        ///// Updates the <see cref="TransportEnvelope"/> with the <paramref name="newHeader"/>.
        ///// </summary>
        ///// <param name="envelope">The <see cref="TransportEnvelope"/> to update.</param>
        ///// <param name="newHeader">The new <see cref="ITransportEnvelopeHeader"/>.</param>
        //void UpdateHeader(TransportEnvelope envelope, ITransportEnvelopeHeader newHeader);
    }
}
