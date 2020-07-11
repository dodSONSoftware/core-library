using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents a piece of an <see cref="IMessage"/>.
    /// </summary>
    [Serializable]
    public class TransportEnvelope
    {
        #region Ctor
        private TransportEnvelope()
        {
        }
        /// <summary>
        /// Creates a TransportEnvelope with the given header and payload.
        /// </summary>
        /// <param name="header">A serialized representation of a <see cref="ITransportEnvelopeHeader"/>.</param>
        /// <param name="payload">A piece of an <see cref="IMessage"/>'s payload.</param>
        public TransportEnvelope(byte[] header,
                                 byte[] payload)
            : this()
        {
            Header = header ?? throw new ArgumentNullException(nameof(header));
            Payload = payload;
        }
        #endregion
        #region Private Fields
        private ITransportEnvelopeHeader _HeaderActual = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets a serialized representation of the <see cref="HeaderActual"/>.
        /// </summary>
        public byte[] Header
        {
            get; set;
        }
        /// <summary>
        /// The header describing this piece of an <see cref="IMessage"/>'s payload.
        /// </summary>
        public ITransportEnvelopeHeader HeaderActual
        {
            get => _HeaderActual;
            set => _HeaderActual = value ?? throw new ArgumentNullException(nameof(value));
        }
        /// <summary>
        /// The piece of an <see cref="IMessage"/>'s payload carried by this <see cref="TransportEnvelope"/>.
        /// </summary>
        public byte[] Payload
        {
            get;
        }
        #endregion
    }
}
