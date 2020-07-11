using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: perhaps this type should implement the IDispose interface.
//       this type contains a [System.ServiceModel.DuplexChannelFactory<T>] but does not implement the IDisposable interface. 
//       as long as Close(...) is called, the [System.ServiceModel.DuplexChannelFactory<T>] will be disposed.


namespace dodSON.Core.Networking
{
    /// <summary>
    /// The base class which simplifies the creation of client resources used in the <see cref="dodSON.Core.Networking"/> namespace.
    /// </summary>
    [Serializable]
    public abstract class BasicClientBase
        : ClientBase,
          IDisposable
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new, empty <see cref="BasicClientBase"/>.
        /// </summary>
        protected BasicClientBase() : base() { }
        /// <summary>
        /// Instantiates a new <see cref="BasicClientBase"/>.
        /// </summary>
        /// <param name="channelAddress">This client's address.</param>
        /// <param name="configuration">This client's configuration.</param>
        /// <param name="registrationController">This client's registration controller.</param>
        protected BasicClientBase(IChannelAddress channelAddress,
                               IClientConfiguration configuration,
                               IRegistrationController registrationController)
            : this()
        {
            Initialize(channelAddress, configuration, registrationController);
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        protected BasicClientBase(Configuration.IConfigurationGroup configuration) : base(configuration)
        {
            Initialize(Address, ClientConfiguration, TransportController.RegistrationController);
        }
        #endregion
        #region Private Fields
        [NonSerialized] private System.ServiceModel.DuplexChannelFactory<IService> _Factory = null;
        [NonSerialized] private IService _ServiceProxy = null;
        [NonSerialized] private ClientActual _ClientActual = null;
        #endregion
        #region Abstract Methods
        /// <summary>
        /// The <see cref="System.ServiceModel.Channels.Binding"/> used for this network resource.
        /// </summary>
        protected abstract System.ServiceModel.Channels.Binding Binding
        {
            get;
        }
        #endregion
        #region Core.ClientBase Methods
        /// <summary>
        /// Attempts to create a communication connection.
        /// </summary>
        /// <param name="exception">Returns an <see cref="Exception"/> if anything should go wrong in the attempt.</param>
        /// <returns>The state of the channel's connection.</returns>
        protected override bool OnOpen(out Exception exception)
        {
            exception = null;
            bool result = false;
            if (_Factory == null)
            {
                try
                {
                    _ClientActual = new ClientActual(base.ClientConfiguration,
                                                     base.TransportController,
                                                     new Action<MessageEventArgs>((e) =>
                                                     {
                                                         // OnSendMessage
                                                         base.SendMessageOnMessageBus(e);
                                                     }),
                                                     new Action(() =>
                                                     {
                                                         // OnCloseClient
                                                         Task.Run(() =>
                                                         {
                                                             base.Close(out Exception ex);
                                                         });
                                                     }),
                                                     new Action<int, TimeSpan>((retryCount, retryDuration) =>
                                                     {
                                                         // OnRestartClient
                                                         Task.Run(() =>
                                                         {
                                                             // close client
                                                             base.Close(out Exception exClose);
                                                             // attempt to restart client
                                                             for (int i = 0; i < retryCount; i++)
                                                             {
                                                                 Threading.ThreadingHelper.Sleep(retryDuration);
                                                                 var openState = base.Open(out Exception exState);
                                                                 if (openState == ChannelStates.Open)
                                                                 {
                                                                     break;
                                                                 }
                                                             }
                                                         });
                                                     }));
                    _Factory = new System.ServiceModel.DuplexChannelFactory<IService>(new System.ServiceModel.InstanceContext(_ClientActual),
                                                                                      Binding,
                                                                                      new System.ServiceModel.EndpointAddress(AddressUri));
                    _ServiceProxy = _Factory.CreateChannel();
                    result = true;
                }
                catch (Exception ex)
                {
                    DestroyConnection();
                    exception = new Exception("Unable to establish connection with server at " + Address, ex);
                }
            }
            return result;
        }
        /// <summary>
        /// Closes the connection.
        /// </summary>
        protected override void OnClose() => DestroyConnection();
        /// <summary>
        /// Takes an <see cref="TransportEnvelope"/> and sends it to the server.
        /// </summary>
        /// <param name="envelope">The <see cref="TransportEnvelope"/> to transmit to the server.</param>
        /// <param name="exception">Contains any exceptions encountered; otherwise <b>null</b>.</param>
        /// <returns><b>True</b> if successful; otherwise <b>false</b>.</returns>
        protected override bool PushEnvelopeToServer(TransportEnvelope envelope,
                                                     out Exception exception)
        {
            // ******** SEND ENVELOPE TO SERVER ********

            bool results = false;
            if (_ServiceProxy == null)
            {
                exception = new Exception("The client has been closed.");
            }
            else
            {
                exception = null;
                try
                {
                    _ServiceProxy.PushEnvelopeToServer(envelope);
                    results = true;
                }
                catch (Exception ex)
                {
                    DestroyConnection();
                    exception = new Exception($"Unable to establish connection with server at {Address}", ex);
                }
            }
            return results;
        }
        /// <summary>
        /// Exposes services provided by the server.
        /// </summary>
        protected override IService ServiceProxy => _ServiceProxy;
        #endregion
        #region Private Methods
        private void DestroyConnection()
        {
            if (_Factory != null)
            {
                try
                {
                    _Factory.Close();
                }
                catch { _Factory.Abort(); }
                ((IDisposable)_Factory).Dispose();
                _Factory = null;
            }
            _ServiceProxy = null;
            _ClientActual = null;
        }
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
        /// Ensures that the internal <see cref="IClient"/> is closed and the internal <see cref="System.ServiceModel.DuplexChannelFactory{T}"/> is disposed.
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
