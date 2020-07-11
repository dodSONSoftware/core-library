using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides properties and methods which can be called by a server.
    /// </summary>
    [Serializable]
    internal class ClientActual
        : IServiceCallback
    {
        #region Ctor
        private ClientActual()
        {
        }
        /// <summary>
        /// Creates an instance of <b>Actual Client</b>.
        /// </summary>
        /// <param name="clientConfiguration">The client configuration.</param>
        /// <param name="transporterController">The transport layer controller.</param>
        /// <param name="messageChannelAction">The action to execute when a message is received.</param>
        /// <param name="closeClientAction">The action to execute when closing the client.</param>
        /// <param name="restartClientAction">The action to execute when a restart is requested.</param>
        public ClientActual(IClientConfiguration clientConfiguration,
                            ITransportController transporterController,
                            Action<MessageEventArgs> messageChannelAction,
                            Action closeClientAction,
                            Action<int, TimeSpan> restartClientAction)
            : this()
        {
            _ClientConfiguration = clientConfiguration ?? throw new ArgumentNullException(nameof(clientConfiguration));
            _TransporterController = transporterController ?? throw new ArgumentNullException(nameof(transporterController));
            _MessageChannelAction = messageChannelAction ?? throw new ArgumentNullException(nameof(messageChannelAction));
            _CloseClientAction = closeClientAction ?? throw new ArgumentNullException(nameof(closeClientAction));
            _RestartClientAction = restartClientAction ?? throw new ArgumentNullException(nameof(restartClientAction));
        }
        #endregion
        #region Private Fields
        private readonly DateTimeOffset _DateCreated = DateTimeOffset.Now;
        private readonly IClientConfiguration _ClientConfiguration = null;
        private readonly ITransportController _TransporterController = null;
        private readonly Action<MessageEventArgs> _MessageChannelAction = null;
        private readonly Action _CloseClientAction = null;
        private readonly Action<int, TimeSpan> _RestartClientAction = null;
        //
        private Converters.TypeSerializer<ITransportStatistics> _ListConvertor = new Converters.TypeSerializer<ITransportStatistics>();
        #endregion
        #region Public Methods
        /// <summary>
        /// Can be used to determine if the <b>Actual Client</b> is alive.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> representing the time the client was created.</returns>
        public DateTimeOffset Ping() => _DateCreated;
        /// <summary>
        /// Commands the <b>Actual Client</b> to close.
        /// </summary>
        public void InstructClientToClose() => _CloseClientAction();
        /// <summary>
        /// Commands the <b>Actual Client</b> to restart.
        /// </summary>
        /// <param name="retryCount">The number of times to </param>
        /// <param name="retryDuration">the time to wait between retries.</param>
        public void InstructClientToRestart(int retryCount, TimeSpan retryDuration) => _RestartClientAction(retryCount, retryDuration);
        /// <summary>
        /// Returns the <b>Actual Client</b>'s current transportation statistics.
        /// </summary>
        /// <returns>Serialized data representing the <b>Actual Client</b>'s current transportation statistics.</returns>
        public byte[] GetTransportStatistics() => _TransporterController.TransportConfiguration.Encryptor
                                                        .Encrypt(_TransporterController.TransportConfiguration.Compressor
                                                        .Compress(_ListConvertor
                                                        .ToByteArray(_TransporterController.TransportStatisticsSnapshot)));
        /// <summary>
        /// Transmits a <see cref="TransportEnvelope"/> to the <b>Actual Client</b>.
        /// </summary>
        /// <param name="envelope">A piece of an <see cref="IMessage"/> to be transmitted to the <b>Actual Client</b>.</param>
        public void PushEnvelopeToClient(TransportEnvelope envelope)
        {
            // ******** RECIEVED ENVELOPE FROM SERVER ********

            IMessage message = _TransporterController.AddEnvelopeFromTransport(envelope);
            if (message != null)
            {
                _MessageChannelAction(new MessageEventArgs(message));
            }
        }
        #endregion
    }
}
