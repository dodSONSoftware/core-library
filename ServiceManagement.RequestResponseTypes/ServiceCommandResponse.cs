using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about  the execution of a <see cref="ServiceCommand"/>.
    /// </summary>
    [Serializable]
    public class ServiceCommandResponse
    {
        #region Ctor
        private ServiceCommandResponse()
        {
        }
        /// <summary>
        /// Instantiates a new instance of the <see cref="ServiceCommandResponse"/> object with the given parameters.
        /// </summary>
        /// <param name="commandSuccessful">Indicates whether the service command was successful or not. <b>True</b> indicates success; otherwise, <b>false</b> indicates failure.</param>
        /// <param name="explanation">The reason for the success or failure.</param>
        /// <param name="serviceCommand">The <see cref="ServicesCommands"/> being executed.</param>
        /// <param name="data">The data, if any, associated with this service command response. If no data is available, this value will be <b>null</b>.</param>
        public ServiceCommandResponse(bool commandSuccessful,
                                      string explanation,
                                      ServiceCommand serviceCommand,
                                      object data)
            : this()
        {
            CommandSuccessful = commandSuccessful;
            Explanation = explanation;
            ServiceCommand = serviceCommand ?? throw new ArgumentNullException(nameof(serviceCommand));
            Data = data;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// Indicates whether the service command was successful or not. <b>True</b> indicates success; otherwise, <b>false</b> indicates failure.
        /// </summary>
        public bool CommandSuccessful
        {
            get;
        }
        /// <summary>
        /// The reason for the success or failure.
        /// </summary>
        public string Explanation
        {
            get;
        }
        /// <summary>
        /// The <see cref="ServiceCommand"/> being executed.
        /// </summary>
        public ServiceCommand ServiceCommand
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
        public override string ToString() => $"CommandSuccessful={CommandSuccessful}, Explanation={Explanation}, {ServiceCommand}";
        #endregion
    }
}
