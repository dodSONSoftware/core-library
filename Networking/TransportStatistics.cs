using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides statistics about the communication system.
    /// </summary>
    [Serializable]
    public class TransportStatistics
        : ITransportStatistics
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="TransportStatistics"/>.
        /// </summary>
        public TransportStatistics()
        {
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The Id of the client or server these statistic relate to.
        /// </summary>
        public string ClientServerId
        {
            get; set;
        }
        /// <summary>
        /// The time the statistics gathering was started.
        /// </summary>
        public DateTimeOffset DateStarted { get; } = DateTimeOffset.Now;
        /// <summary>
        /// The duration that the statistics were gathered.
        /// </summary>
        public TimeSpan RunTime => (DateTime.Now - DateStarted);
        /// <summary>
        /// The total number of bytes received.
        /// </summary>
        public long IncomingBytes
        {
            get; set;
        }
        /// <summary>
        /// The total number of envelopes received.
        /// </summary>
        public long IncomingEnvelopes
        {
            get; set;
        }
        /// <summary>
        /// The total number of messages received.
        /// </summary>
        public long IncomingMessages
        {
            get; set;
        }
        /// <summary>
        /// The average number of bytes received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingBytes">The last total number of bytes received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingBytes"/> value was recorded.</param>
        /// <returns>The average number of bytes received per second within the specified duration.</returns>
        public double IncomingAverageBytesPerSecond(long lastIncomingBytes, TimeSpan forDuration) => ((IncomingBytes - lastIncomingBytes) / forDuration.TotalSeconds);
        /// <summary>
        /// The average number of envelopes received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingEnvelopes">The last total number of envelopes received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingEnvelopes"/> value was recorded.</param>
        /// <returns>The average number of envelopes received per second within the specified duration.</returns>
        public double IncomingAverageEnvelopesPerSecond(long lastIncomingEnvelopes, TimeSpan forDuration) => ((IncomingEnvelopes - lastIncomingEnvelopes) / forDuration.TotalSeconds);
        /// <summary>
        /// The average number of messages received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingMessages">The last total number of messages received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingMessages"/> value was recorded.</param>
        /// <returns>The average number of messages received per second within the specified duration.</returns>
        public double IncomingAverageMessagesPerSecond(long lastIncomingMessages, TimeSpan forDuration) => ((IncomingMessages - lastIncomingMessages) / forDuration.TotalSeconds);
        /// <summary>
        /// The total number of bytes sent.
        /// </summary>
        public long OutgoingBytes
        {
            get; set;
        }
        /// <summary>
        /// The total number of envelopes sent.
        /// </summary>
        public long OutgoingEnvelopes
        {
            get; set;
        }
        /// <summary>
        /// The total number of messages sent.
        /// </summary>
        public long OutgoingMessages
        {
            get; set;
        }
        /// <summary>
        /// The average number of bytes sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingBytes">The last total number of bytes sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingBytes"/> value was recorded.</param>
        /// <returns>The average number of bytes sent per second within the specified duration.</returns>
        public double OutgoingAverageBytesPerSecond(long lastOutgoingBytes, TimeSpan forDuration) => ((OutgoingBytes - lastOutgoingBytes) / forDuration.TotalSeconds);
        /// <summary>
        /// The average number of envelopes sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingEnvelopes">The last total number of envelopes sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingEnvelopes"/> value was recorded.</param>
        /// <returns>The average number of envelopes sent per second within the specified duration.</returns>
        public double OutgoingAverageEnvelopesPerSecond(long lastOutgoingEnvelopes, TimeSpan forDuration) => ((OutgoingEnvelopes - lastOutgoingEnvelopes) / forDuration.TotalSeconds);
        /// <summary>
        /// The average number of messages sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingMessages">The last total number of messages sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingMessages"/> value was recorded.</param>
        /// <returns>The average number of messages sent per second within the specified duration.</returns>
        public double OutgoingAverageMessagesPerSecond(long lastOutgoingMessages, TimeSpan forDuration) => ((OutgoingMessages - lastOutgoingMessages) / forDuration.TotalSeconds);
        #endregion
    }
}
