using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Provides a mechanism to record round trip times for network request and responses.
    /// </summary>
    [Serializable]
    public class Ping
    {
        #region Ctor
        /// <summary>
        /// Creates a Ping object with the current date time.
        /// </summary>
        public Ping() { }
        #endregion
        #region Public Properties
        /// <summary>
        /// The date time this Ping object was created.
        /// </summary>
        public DateTimeOffset StartTime { get; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Sets the <see cref="ReceivedTime"/> to the current date time.
        /// </summary>
        public void MarkReceivedTime() => ReceivedTime = DateTimeOffset.UtcNow;
        /// <summary>
        /// The date time indicating the moment the Ping object was received by the target.
        /// </summary>
        public DateTimeOffset ReceivedTime { get; private set; }

        /// <summary>
        /// Sets the <see cref="EndTime"/> to the current date time.
        /// </summary>
        public void MarkEndTime() => EndTime = DateTimeOffset.UtcNow;
        /// <summary>
        /// The date time indicating the moment the Ping object was returned.
        /// </summary>
        public DateTimeOffset EndTime { get; private set; }

        /// <summary>
        /// The difference in time between the <see cref=" StartTime"/> and the <see cref="ReceivedTime"/>.
        /// </summary>
        public TimeSpan TravelToTime { get => (ReceivedTime - StartTime); }
        /// <summary>
        /// The difference in time between the <see cref=" ReceivedTime"/> and the <see cref="EndTime"/>.
        /// </summary>
        public TimeSpan TravelFromTime { get => (EndTime - ReceivedTime); }
        /// <summary>
        /// The difference in time between the <see cref=" StartTime"/> and the <see cref="EndTime"/>.
        /// </summary>
        public TimeSpan TotalTravelTime { get => (EndTime - StartTime); }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this Ping object.
        /// </summary>
        /// <returns>A string representation of this Ping object.</returns>
        public override string ToString() => $"Ping, Round Trip={TotalTravelTime}, TravelToTime={TravelToTime}, TravelFromTime={TravelFromTime}";   //, StartTime={StartTime}, ReceivedTime={ReceivedTime}, EndTime={EndTime}";
        #endregion
    }
}
