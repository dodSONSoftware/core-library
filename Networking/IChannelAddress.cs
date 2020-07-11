using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Networking
{
    /// <summary>
    /// Defines the elements required to create a connection in the communication system.
    /// </summary>
    public interface IChannelAddress
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The Internet Protocol (IP) address in string form.
        /// </summary>
        string IPAddress { get; }
        /// <summary>
        /// The port number for the <see cref="IPAddress"/>.
        /// </summary>
        int Port { get; }
        /// <summary>
        /// The name of the connection.
        /// </summary>
        string Name { get; }
    }
}
