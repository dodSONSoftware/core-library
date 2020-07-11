using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: perhaps this type should implement the IDispose interface.
//       this type contains a [System.ServiceModel.ServiceHost] but does not implement the IDisposable interface. 
//       as long as Close(...) is called, the [System.ServiceModel.ServiceHost] will be disposed.


namespace dodSON.Core.Networking
{
    /// <summary>
    /// The base class which simplifies the creation of server resources used in the <see cref="dodSON.Core.Networking"/> namespace.
    /// </summary>
    [Serializable]
    public abstract class BasicServerBase
        : ServerBase,
          IDisposable
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new, empty <see cref="BasicServerBase"/>.
        /// </summary>
        protected BasicServerBase() : base() { }
        /// <summary>
        /// Instantiates a new <see cref="BasicServerBase"/>.
        /// </summary>
        /// <param name="channelAddress">The client's address.</param>
        /// <param name="configuration">The client's configuration.</param>
        /// <param name="transportController">Controls the transportation layer.</param>
        /// <param name="logSourceId">The value used for the <see cref="Logging.ILogEntry.SourceId"/> when generating <see cref="Logging.ILog"/>s.</param>
        protected BasicServerBase(IChannelAddress channelAddress,
                      IServerConfiguration configuration,
                      ITransportController transportController,
                      string logSourceId)
            : this()
        {
            Initialize(channelAddress, configuration, transportController, logSourceId);
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        protected BasicServerBase(Configuration.IConfigurationGroup configuration) : base(configuration)
        {
            Initialize(Address, ServerConfiguration, TransportController, LogSourceId);
        }
        #endregion
        #region Private Fields
        private System.ServiceModel.ServiceHost _HostActual = null;
        private ServerActual _InternalService = null;
        private IClient _InternalClient = null;
        #endregion
        #region Abstract Methods
        /// <summary>
        /// The <see cref="System.ServiceModel.Channels.Binding"/> used for this network resource.
        /// </summary>
        protected abstract System.ServiceModel.Channels.Binding Binding
        {
            get;
        }
        /// <summary>
        /// A new <see cref="IClient"/> required by the server to use as its internal client. (Do not cache the client; every call should create a new client.)
        /// </summary>
        /// <param name="clientName">The name of the new client.</param>
        /// <param name="receiveSelfSentMessages">Determines if a client will receives messages that it send. When <b>true</b> messages sent by the client will be received on the message bus; otherwise, no messages sent by the client will be received by the client.</param>
        /// <returns>A new client. (Do not cache the client; every call should create a new client.)</returns>
        protected abstract IClient Client(string clientName, bool receiveSelfSentMessages);
        #endregion
        #region ServerBase Methods
        /// <summary>
        /// Attempts to create a communication connection.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong in the attempt.</param>
        /// <returns>The state of the channel's connection.</returns>
        protected override bool OnOpen(out Exception exception)
        {
            exception = null;
            bool result = false;
            if (_HostActual == null)
            {
                try
                {
                    _InternalService = new ServerActual(Id, TransportController, ServerConfiguration.OverrideTypesFilter, LogSourceId);
                    _HostActual = new System.ServiceModel.ServiceHost(_InternalService, new Uri(string.Format("net.tcp://{0}", Address.IPAddress)));

                    _HostActual.AddServiceEndpoint(typeof(IService), Binding, AddressUri);
                    _HostActual.Open();
                    //
                    var parts = Guid.NewGuid().ToString().Split('-');
                    var serverName = "SiC" + string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}", parts[0], parts[1], parts[2], parts[3], parts[4].Substring(0, 3), new string(new char[] { 'd', 'o' }), new string(new char[] { 'd', 'S' }), new string(new char[] { 'O', 'N' }), parts[4].Substring(4, 3));
                    bool receiveSelfSentMessages = true;
                    _InternalClient = Client(serverName, receiveSelfSentMessages);
                    //
                    ((ITransportConfigurationAdvanced)TransportController.TransportConfiguration).SetServerId(ServerConfiguration.Id);

                    // ################
                    // TODO: Problem ID: dc95c9d1ab4a422cbca3834e1361aa0 : The Fix
                    //                   The Fix. Update the TransportController.RegistrationController with the correct TransportConfiguration. 
                    //                            Current hypothesis is that somehow the referenced registration controller are different in the two places when it should be one.
                    //                            This problem manifests itself ONLY when the objects are created using the IConfiguration system. 
                    //                            Using regular instantiation (standard Ctors) seems to make a single reference; using the IConfiguration system seems to cause two separate instances.
                    //                            The answer is apparent, the IConfiguration system is creating two of these items.
                    ((IRegistrationControllerAdvanced)TransportController.RegistrationController).SetTransportConfiguration(TransportController.TransportConfiguration);
                    // ################

                    ((Networking.ITransportControllerAdvanced)_InternalClient.TransportController).SetClientServerId(_InternalClient.Id);
                    _InternalClient.MessageBus += new EventHandler<MessageEventArgs>(InternalClient_MessagePump);
                    //
                    if (_InternalClient.Open(out Exception ex) == ChannelStates.Open)
                    {
                        result = true;
                    }
                    else
                    {
                        exception = new Exception("Unable to connect to internal client.", ex);
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    try
                    {
                        OnClose();
                    }
                    catch { }
                }
            }
            return result;
        }
        /// <summary>
        /// Closes the connection.
        /// </summary>
        protected override void OnClose()
        {
            if (_InternalClient != null)
            {
                _InternalClient.Close(out Exception exception);
                _InternalClient = null;
            }
            //
            if (_HostActual != null)
            {
                try
                {
                    _HostActual.Close();
                }
                catch { _HostActual.Abort(); }
                ((IDisposable)_HostActual).Dispose();
                _HostActual = null;
            }
        }
        /// <summary>
        /// Transmits a message using the server's internal client.
        /// </summary>
        /// <param name="message">The message to transmit using the server's internal client.</param>
        protected override void PushMessageToInternalClient(IMessage message)
        {
            if (_InternalClient != null)
            {
                _InternalClient.SendMessage(message);
            }
        }
        /// <summary>
        /// Gets the internal, <b>Actual Server</b>.
        /// </summary>
        protected override IService Service => _InternalService;
        /// <summary>
        /// Gets the internal <see cref="IClient"/> used by the server to send and receive messages.
        /// </summary>
        protected override IClient InternalClient => _InternalClient;
        #endregion
        #region IDisposable Methods
        /// <summary>
        /// Provides a mechanism for releasing resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Ensures that the internal <see cref="IClient"/> is closed and the internal <see cref="System.ServiceModel.ServiceHost"/> is disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Close(out Exception ex);
            }
        }
        #endregion
    }
}
