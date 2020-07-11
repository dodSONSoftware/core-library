using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines more advanced control methods for the <see cref="ITransportController"/>.
    /// </summary>
    public interface ITransportControllerAdvanced
    {
        /// <summary>
        /// Replaces the current <see cref="ITransportController.TransportConfiguration"/> with the given <see cref="ITransportConfiguration"/>.
        /// </summary>
        /// <param name="transportConfiguration">The new <see cref="ITransportConfiguration"/>.</param>
        void SetTransportConfiguration(ITransportConfiguration transportConfiguration);
        /// <summary>
        /// Set the id of the host client or server.
        /// </summary>
        /// <param name="id">The new, client or server, id.</param>
        void SetClientServerId(string id);
    }
}
