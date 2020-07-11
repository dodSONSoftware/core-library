using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a file transfer operation.
    /// </summary>
    [Serializable]
    public class FileTransferFeedback
    {
        #region Ctor
        private FileTransferFeedback()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="FileTransferFeedback"/> object with the given parameters.
        /// </summary>
        /// <param name="sourceFilename">The name of the file being transmitted.</param>
        /// <param name="destFilename">The destination filename.</param>
        /// <param name="lastBytesTransfered">The number of bytes last transmitted.</param>
        /// <param name="totalBytes">The total number of bytes in the file.</param>
        /// <param name="totalTransferedBytes">The number of bytes transmitted so far.</param>
        public FileTransferFeedback(string sourceFilename,
                                    string destFilename,
                                    long lastBytesTransfered,
                                    long totalBytes,
                                    long totalTransferedBytes)
        {
            if (string.IsNullOrWhiteSpace(sourceFilename))
            {
                throw new ArgumentNullException(nameof(sourceFilename));
            }
            SourceFilename = sourceFilename;
            //
            if (string.IsNullOrWhiteSpace(destFilename))
            {
                throw new ArgumentNullException(nameof(destFilename));
            }
            DestFilename = destFilename;
            //
            LastBytesTransfered = lastBytesTransfered;
            TotalBytes = totalBytes;
            TotalTransferedBytes = totalTransferedBytes;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The name of the file being transmitted.
        /// </summary>
        public string SourceFilename
        {
            get;
        }
        /// <summary>
        /// The destination filename.
        /// </summary>
        public string DestFilename
        {
            get;
        }
        /// <summary>
        /// The number of bytes last transmitted.
        /// </summary>
        public long LastBytesTransfered
        {
            get;
        }
        /// <summary>
        /// The total number of bytes in the file.
        /// </summary>
        public long TotalBytes
        {
            get;
        }
        /// <summary>
        /// The number of bytes transmitted so far.
        /// </summary>
        public long TotalTransferedBytes
        {
            get;
        }
        /// <summary>
        /// The percentage of the file transmitted. Values range from 0 to 1.
        /// </summary>
        public double PercentTransfered => ((double)TotalTransferedBytes / TotalBytes);
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Transfered={PercentTransfered * 100:N1}%, SourceFilename={SourceFilename}, DestFilename={DestFilename}, LastBytesTransfered={Common.ByteCountHelper.ToString(LastBytesTransfered)} ({LastBytesTransfered}), TotalBytes={Common.ByteCountHelper.ToString(TotalBytes)} ({TotalBytes}), TotalTransferedBytes={Common.ByteCountHelper.ToString(TotalTransferedBytes)} ({TotalTransferedBytes})";
        #endregion
    }
}
