using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Provides data about a message being sent through the messaging system.
    /// </summary>
    [Serializable]
    public class MessageEventArgs
          : EventArgs
    {
        #region Ctor
        private MessageEventArgs() : base() { }
        /// <summary>
        /// Creates an instance of the <see cref="MessageEventArgs"/> using the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message associated with the event.</param>
        public MessageEventArgs(IMessage message)
            : this()
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The message associated with the event.
        /// </summary>
        public IMessage Message { get; }
        #endregion
    }
}
