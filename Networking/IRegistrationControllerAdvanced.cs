using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines more advanced control methods for the <see cref="IRegistrationController"/>.
    /// </summary>
    public interface IRegistrationControllerAdvanced
    {
        /// <summary>
        /// Replaces the current <see cref="ITransportController.TransportConfiguration"/> with the given <see cref="ITransportConfiguration"/>.
        /// </summary>
        /// <param name="transportConfiguration">The new <see cref="ITransportConfiguration"/>.</param>
        void SetTransportConfiguration(ITransportConfiguration transportConfiguration);
    }
}
