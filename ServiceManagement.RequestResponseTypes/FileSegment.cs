using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains a file segment.
    /// </summary>
    [Serializable]
    public class FileSegment
    {
        #region Ctor
        private FileSegment()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="FileSegment"/> object with the given parameters.
        /// </summary>
        /// <param name="index">The sequence for this file segment.</param>
        /// <param name="totalBytes">The total number of bytes in this file.</param>
        /// <param name="sentBytes">The number of bytes sent so far.</param>
        /// <param name="payload">The file segment.</param>
        public FileSegment(long index, 
                           long totalBytes, 
                           long sentBytes, 
                           byte[] payload)
        {
            Index = index;
            TotalBytes = totalBytes;
            SentBytes = sentBytes;
            Payload = payload;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The sequence for this file segment.
        /// </summary>
        public long Index
        {
            get;
        }
        /// <summary>
        /// The total number of file segments.
        /// </summary>
        public long TotalBytes
        {
            get;
        }
        /// <summary>
        /// The number of bytes sent so far.
        /// </summary>
        public long SentBytes
        {
            get;
        }
        /// <summary>
        /// The file segment.
        /// </summary>
        public byte[] Payload
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Index={Index}, TotalBytes={TotalBytes}, SentBytes={SentBytes}, Payload.Length={Payload.Length}";
        #endregion
    }
}
