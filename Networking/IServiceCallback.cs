using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines properties and methods implemented by the <b>Actual Client</b> which can then be called by the server.
    /// </summary>
    [ServiceContract]
    public interface IServiceCallback
    {
        /// <summary>
        /// Can be used to determine if the <b>Actual Client</b> is alive.
        /// </summary>
        /// <returns>A <see cref="DateTimeOffset"/> representing the time the client was created.</returns>
        [OperationContract]
        DateTimeOffset Ping();

        /// <summary>
        /// Returns the <b>Actual Client</b>'s current transportation statistics.
        /// </summary>
        /// <returns>Serialized data representing the <b>Actual Client</b>'s current transportation statistics.</returns>
        [OperationContract]
        byte[] GetTransportStatistics();

        /// <summary>
        /// Commands the <b>Actual Client</b> to close.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void InstructClientToClose();

        /// <summary>
        /// Commands the <b>Actual Client</b> to restart.
        /// </summary>
        /// <param name="retryCount">The number of times to </param>
        /// <param name="retryDuration">the time to wait between retries.</param>
        [OperationContract(IsOneWay = true)]
        void InstructClientToRestart(int retryCount, TimeSpan retryDuration);

        /// <summary>
        /// Transmits a <see cref="TransportEnvelope"/> to the <b>Actual Client</b>.
        /// </summary>
        /// <param name="envelope">A piece of an <see cref="IMessage"/> to be transmitted to the <b>Actual Client</b>.</param>
        [OperationContract(IsOneWay = true)]
        void PushEnvelopeToClient(TransportEnvelope envelope);
    }
}
