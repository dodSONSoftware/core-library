using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Represents a message which can be transported through a communication channel.
    /// </summary>
    [Serializable]
    public class Message
        : IMessage
    {
        #region Public Methods
        #endregion
        #region Ctor
        private Message()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="Message"/> with the given information.
        /// </summary>
        /// <param name="id">The unique id for this message.</param>
        /// <param name="serverIds">The list of servers this message has been processed by.</param>
        /// <param name="clientId">The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.</param>
        /// <param name="targetId">The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.</param>
        /// <param name="typeInfo">Type information about the <see cref="Payload"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public Message(string id,
                       string serverIds,
                       string clientId,
                       string targetId,
                       IPayloadTypeInfo typeInfo,
                       byte[] payload)
            : this()
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            Id = id;
            ServerIds = serverIds;
            ClientId = clientId;
            TargetId = targetId;
            TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            Payload = payload;
        }
        /// <summary>
        /// Instantiates a new <see cref="Message"/> with the given information.
        /// </summary>
        /// <param name="id">The unique id for this message.</param>
        /// <param name="clientId">The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.</param>
        /// <param name="targetId">The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.</param>
        /// <param name="typeInfo">Type information about the <see cref="Payload"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public Message(string id,
                       string clientId,
                       string targetId,
                       IPayloadTypeInfo typeInfo,
                       byte[] payload) : this(id, "", clientId, targetId, typeInfo, payload) { }
        /// <summary>
        /// Instantiates a new <see cref="Message"/> with the given information.
        /// </summary>
        /// <param name="id">The unique id for this message.</param>
        /// <param name="clientId">The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.</param>
        /// <param name="typeInfo">Type information about the <see cref="Payload"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public Message(string id,
                       string clientId,
                       IPayloadTypeInfo typeInfo,
                       byte[] payload) : this(id, "", clientId, "", typeInfo, payload) { }
        /// <summary>
        /// Instantiates a new <see cref="Message"/> with the given information.
        /// </summary>
        /// <param name="clientId">The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.</param>
        /// <param name="typeInfo">Type information about the <see cref="Payload"/>.</param>
        /// <param name="payload">The data being transported.</param>
        public Message(string clientId,
                       IPayloadTypeInfo typeInfo,
                       byte[] payload) : this(Guid.NewGuid().ToString("N"), "", clientId, "", typeInfo, payload) { }
        #endregion
        #region Private Fields
        private string _ClientId = "";
        #endregion
        #region Public Methods
        /// <summary>
        /// The unique id for this message.
        /// </summary>
        public string Id
        {
            get;
        }
        /// <summary>
        /// The list of servers this message has been processed by.
        /// </summary>
        public string ServerIds
        {
            get; set;
        }
        /// <summary>
        /// The id of the <see cref="IClient"/> which sent the <see cref="IMessage"/>.
        /// </summary>
        public string ClientId
        {
            get => _ClientId;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }
                _ClientId = value;
            }
        }
        /// <summary>
        /// The id of the <see cref="IClient"/> which should receive the <see cref="IMessage"/>.
        /// </summary>
        public string TargetId
        {
            get; set;
        }
        /// <summary>
        /// Type information about the <see cref="Payload"/>.
        /// </summary>
        public IPayloadTypeInfo TypeInfo
        {
            get;
        }
        /// <summary>
        /// The data being transported.
        /// </summary>
        public byte[] Payload
        {
            get;
        }
        /// <summary>
        /// Converts the <see cref="Payload"/> into the type <typeparamref name="T"/> or null if unable to convert to desired type.
        /// </summary>
        /// <typeparam name="T">The type to convert the <see cref="Payload"/> to.</typeparam>
        /// <returns>The <see cref="Payload"/> converted into type <typeparamref name="T"/> or null if unable to convert to desired type.</returns>
        public T PayloadMessage<T>()
        {
            try
            {
                return (new Converters.TypeSerializer<T>()).FromByteArray(Payload);
            }
            catch
            {
                return default(T);
            }
        }
        #endregion
    }
}
