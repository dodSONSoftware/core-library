using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement
{
    /// <summary>
    /// Contains all of the coded Service commands.
    /// </summary>
    public enum ServicesCommands
    {
        /// <summary>
        /// Commands the service to stop.
        /// </summary>
        Stop = 0,

        /// <summary>
        /// Commands the service to start.
        /// </summary>
        Start = 1,

        /// <summary>
        /// Commands the service to stop and start.
        /// </summary>
        Restart = 2,

        /// <summary>
        /// Retrieves the logs for the specified service.
        /// See <see cref="RequestResponseTypes.GetLogs"/>.
        /// </summary>
        GetLogs = 3
    }
}
