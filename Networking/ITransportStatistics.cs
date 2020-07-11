using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines statistics about the communication system.
    /// </summary>
    public interface ITransportStatistics
    {
        /// <summary>
        /// The Id of the client or server these statistic relate to.
        /// </summary>
        string ClientServerId { get; set; }
        /// <summary>
        /// The time the statistics gathering was started.
        /// </summary>
        DateTimeOffset DateStarted { get; }
        /// <summary>
        /// The duration that the statistics were gathered.
        /// </summary>
        TimeSpan RunTime { get; }
        /// <summary>
        /// The total number of bytes received.
        /// </summary>
        long IncomingBytes { get; set; }
        /// <summary>
        /// The total number of envelopes received.
        /// </summary>
        long IncomingEnvelopes { get; set; }
        /// <summary>
        /// The total number of messages received.
        /// </summary>
        long IncomingMessages { get; set; }
        /// <summary>
        /// The average number of bytes received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingBytes">The last total number of bytes received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingBytes"/> value was recorded.</param>
        /// <returns>The average number of bytes received per second within the specified duration.</returns>
        double IncomingAverageBytesPerSecond(long lastIncomingBytes, TimeSpan forDuration);
        /// <summary>
        /// The average number of envelopes received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingEnvelopes">The last total number of envelopes received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingEnvelopes"/> value was recorded.</param>
        /// <returns>The average number of envelopes received per second within the specified duration.</returns>
        double IncomingAverageEnvelopesPerSecond(long lastIncomingEnvelopes, TimeSpan forDuration);
        /// <summary>
        /// The average number of messages received per second within the specified duration.
        /// </summary>
        /// <param name="lastIncomingMessages">The last total number of messages received.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastIncomingMessages"/> value was recorded.</param>
        /// <returns>The average number of messages received per second within the specified duration.</returns>
        double IncomingAverageMessagesPerSecond(long lastIncomingMessages, TimeSpan forDuration);
        /// <summary>
        /// The total number of bytes sent.
        /// </summary>
        long OutgoingBytes { get; set; }
        /// <summary>
        /// The total number of envelopes sent.
        /// </summary>
        long OutgoingEnvelopes { get; set; }
        /// <summary>
        /// The total number of messages sent.
        /// </summary>
        long OutgoingMessages { get; set; }
        /// <summary>
        /// The average number of bytes sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingBytes">The last total number of bytes sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingBytes"/> value was recorded.</param>
        /// <returns>The average number of bytes sent per second within the specified duration.</returns>
        double OutgoingAverageBytesPerSecond(long lastOutgoingBytes, TimeSpan forDuration);
        /// <summary>
        /// The average number of envelopes sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingEnvelopes">The last total number of envelopes sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingEnvelopes"/> value was recorded.</param>
        /// <returns>The average number of envelopes sent per second within the specified duration.</returns>
        double OutgoingAverageEnvelopesPerSecond(long lastOutgoingEnvelopes, TimeSpan forDuration);
        /// <summary>
        /// The average number of messages sent per second within the specified duration.
        /// </summary>
        /// <param name="lastOutgoingMessages">The last total number of messages sent.</param>
        /// <param name="forDuration">The <see cref="TimeSpan"/> since the <paramref name="lastOutgoingMessages"/> value was recorded.</param>
        /// <returns>The average number of messages sent per second within the specified duration.</returns>
        double OutgoingAverageMessagesPerSecond(long lastOutgoingMessages, TimeSpan forDuration);
    }
}
