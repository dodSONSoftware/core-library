using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Networking information.
    /// </summary>
    [Serializable]
    public static class NetworkingHelper
    {
        #region Public Static Constants
        /// <summary>
        /// The default address for ip addresses. Value= localhost
        /// </summary>
        public static readonly string DefaultIpAddress = "localhost";
        /// <summary>
        /// The minimum port value allowed. Value= 0
        /// </summary>
        public static readonly int MinumumPortValue = 0;
        /// <summary>
        /// The recommended minimum port value allowed. Value= 49152
        /// </summary>
        public static readonly int RecommendedMinumumPortValue = 49152;
        /// <summary>
        /// The maximum port value allowed. Value= 65535
        /// </summary>
        public static readonly int MaximumPortValue = 65535;
        /// <summary>
        /// The minimum size, in bytes, of a <see cref="TransportEnvelope"/> chunk. Value= 512
        /// </summary>
        public static readonly int MinimumTransportEnvelopeChunkSize = 512;
        /// <summary>
        /// The maximum size, in bytes, of a <see cref="TransportEnvelope"/> chunk. Value= 60000
        /// </summary>
        public static readonly int MaximumNamedPipeTransportEnvelopeChunkSize = 60000;
        /// <summary>
        /// The prefix for all internal responses.
        /// </summary>
        internal static readonly string RequestResponsePrefix = "807B062F-3D60-43AE-B05A-392A8EDF3372-";
        /// <summary>
        /// The value used as the <see cref="IPayloadTypeInfo.TypeName"/> when the message contains a transportation statistics request.
        /// </summary>
        internal static readonly string RequestAllTransportStatistics = RequestResponsePrefix + "DF848D67-37CB-4187-A7EB-31107AB53A4F";
        /// <summary>
        /// /// The value used as the <see cref="IPayloadTypeInfo.TypeName"/> when the message contains a transportation statistics response.
        /// </summary>
        internal static readonly string ResponseAllTransportStatistics = RequestResponsePrefix + "DB7F1DBA-4C5D-4786-986A-B71B4EE3A585";
        #endregion
    }
}
