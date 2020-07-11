using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Commands the File Transfer Sender to change its transport style.
    /// </summary>
    [Serializable]
    public class FileTransferThrottle
    {
        #region Ctor
        private FileTransferThrottle()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="FileTransferThrottle"/> with the given parameters.
        /// </summary>
        /// <param name="segmentLength">The length, in bytes, that each file segment will be split into.</param>
        /// <param name="segmentTransferInterstitialDelay">The amount of time to wait between sending each file segment.</param>
        public FileTransferThrottle(long segmentLength,
                                    TimeSpan segmentTransferInterstitialDelay)
        {
            SegmentLength = segmentLength;
            SegmentTransferInterstitialDelay = segmentTransferInterstitialDelay;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The length, in bytes, that each file segment will be split into.
        /// </summary>
        public long SegmentLength
        {
            get;
        }
        /// <summary>
        /// The amount of time to wait between sending each file segment.
        /// </summary>
        public TimeSpan SegmentTransferInterstitialDelay
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"SegmentLength={Common.ByteCountHelper.ToString(SegmentLength)} ({SegmentLength:N0}), SegmentTransferInterstitialDelay={SegmentTransferInterstitialDelay}";
        #endregion
    }
}
