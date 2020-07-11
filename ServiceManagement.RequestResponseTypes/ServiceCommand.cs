using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Attempts to execute a command against a specific service.
    /// </summary>
    [Serializable]
    public class ServiceCommand
    {
        #region Ctor
        private ServiceCommand()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ServiceCommand"/> with the given parameters.
        /// </summary>
        /// <param name="serviceId">The id of the service to command.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="data">The data, if any, associated with this service command response. If no data is available, this value will be <b>null</b>.</param>
        public ServiceCommand(string serviceId,
                              ServicesCommands command,
                              object data)
            : this()
        {
            if (string.IsNullOrWhiteSpace(serviceId))
            {
                throw new ArgumentNullException(nameof(serviceId));
            }
            ServiceId = serviceId;
            //
            Command = command;
            //
            Data = data;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The id of the service to command.
        /// </summary>
        public string ServiceId
        {
            get;
        }
        /// <summary>
        /// The command to execute.
        /// </summary>
        public ServicesCommands Command
        {
            get;
        }
        /// <summary>
        /// The data, if any, associated with this service command response. If no data is available, this value will be <b>null</b>.
        /// </summary>
        public object Data
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Command={Command}, ServiceId={ServiceId}, Has Data={Data != null}";
        #endregion
    }
}
