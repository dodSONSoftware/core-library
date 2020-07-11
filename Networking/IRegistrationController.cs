using System;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines properties and methods used to control registering and unregistering clients.
    /// </summary>
    public interface IRegistrationController
        : IRegistrationControllerAdvanced,
          Configuration.IConfigurable
    {
        /// <summary>
        /// The evidence needed to register with the server.
        /// </summary>
        byte[] ChallengeEvidence { get; }
        /// <summary>
        /// Used to control access to the communication system.
        /// </summary>
        IChallengeController ChallengeController { get; }
        /// <summary>
        /// Configuration information used by the transportation layer.
        /// </summary>
        ITransportConfiguration TransportConfiguration { get; }


        // **** CLIENT-ONLY METHODS

        /// <summary>
        /// Will attempt to register a client with a server.
        /// </summary>
        /// <param name="serviceProxy">The server's service proxy. (Provides callback methods to the server.)</param>
        /// <param name="client">The client to register.</param>
        /// <param name="transportControllerAdvanced">Provides advanced access to the transport controller.</param>
        /// <returns><b>True</b> if registration succeeded; otherwise <b>false</b>.</returns>
        bool Register(IService serviceProxy, IClient client, ITransportControllerAdvanced transportControllerAdvanced);
        /// <summary>
        /// Will attempt to unregister a client from a server.
        /// </summary>
        /// <param name="serviceProxy">The server's service proxy. (Provides callback methods to the server.)</param>
        /// <param name="client">The client to unregister.</param>
        void Unregister(IService serviceProxy, IClient client);

        // **** SERVER-ONLY METHODS

        /// <summary>
        /// Attempts to register the client with the server.
        /// </summary>
        /// <param name="data">Information related to registration.</param>
        /// <param name="newRegisterableClientConfiguration">A <see cref="IClientConfiguration"/> for the newly registered client.</param>
        /// <returns>A response relative to the registration process.</returns>
        byte[] TryAddDataFromRegistrationChannel(byte[] data, out IClientConfiguration newRegisterableClientConfiguration);
    }
}
