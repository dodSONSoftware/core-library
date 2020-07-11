using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines advanced functionality required by types in the dodSON.Core.Networking namespace, but not generally used by the typical consumer.
    /// </summary>
    public interface ITransportConfigurationAdvanced
    {
        /// <summary>
        /// Sets the Id of the server.
        /// </summary>
        /// <param name="id">The Id for the server.</param>
        void SetServerId(string id);
    }
}
