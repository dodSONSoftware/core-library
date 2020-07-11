using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines the properties and methods exposed by the server.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IServiceCallback))]
    public interface IService
    {
        /// <summary>
        /// Fired when activity <see cref="Logging.Logs"/> are available.
        /// </summary>
        event EventHandler<Networking.ActivityLogsEventArgs> ActivityLogsEvent;
        /// <summary>
        /// Called by clients to register with the server.
        /// </summary>
        /// <param name="registrationType">The type of registration requested.</param>
        /// <param name="data">Information required to register with the server.</param>
        /// <returns>Results relating to the registration.</returns>
        [OperationContract]
        byte[] RegistrationChannel(RegistrationTypeEnum registrationType, byte[] data);
        /// <summary>
        /// Transmits the <see cref="TransportEnvelope"/> to the server.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to send to the server.</param>
        [OperationContract(IsOneWay = true)]
        void PushEnvelopeToServer(TransportEnvelope envelope);
        /// <summary>
        /// Gets a collection of registered clients and their callback service.
        /// </summary>
        IEnumerable<Tuple<IClientConfiguration, IServiceCallback>> RegisteredClientsCallbacks
        {
            get;
        }
    }
}
