using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a <see cref="RequestResponseCommands.EnableDisablePackage"/> command.
    /// </summary>
    [Serializable]
    public class EnableDisablePackageResponse
    {
        #region Ctor
        private EnableDisablePackageResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="EnableDisablePackageResponse"/> with the given message and <see cref="EnableDisablePackage"/> request.
        /// </summary>
        /// <param name="message">A message relaying the results of the <see cref="RequestResponseCommands.EnableDisablePackage"/> command.</param>
        /// <param name="requestData">The <see cref="RequestResponseTypes.EnableDisablePackage"/> request.</param>
        public EnableDisablePackageResponse(string message, 
                                            RequestResponseTypes.EnableDisablePackage requestData)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message;
            //
            EnableDisablePackage = requestData ?? throw new ArgumentNullException(nameof(requestData));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// A message relaying the results of the <see cref="RequestResponseCommands.EnableDisablePackage"/> command.
        /// </summary>
        public string  Message
        {
            get; 
        }
        /// <summary>
        /// The <see cref="RequestResponseTypes.EnableDisablePackage"/> request.
        /// </summary>
        public RequestResponseTypes.EnableDisablePackage EnableDisablePackage
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Message={Message}, {EnableDisablePackage}";
        #endregion
    }
}
