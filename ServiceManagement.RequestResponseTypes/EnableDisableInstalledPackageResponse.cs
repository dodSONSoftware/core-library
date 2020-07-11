using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about a <see cref="RequestResponseCommands.EnableDisableInstalledPackage"/> command.
    /// </summary>
    [Serializable]
    public class EnableDisableInstalledPackageResponse
    {
        #region Ctor
        private EnableDisableInstalledPackageResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="EnableDisableInstalledPackageResponse"/> with the given message and <see cref="EnableDisableInstalledPackage"/> request.
        /// </summary>
        /// <param name="message">A message relaying the results of the <see cref="RequestResponseCommands.EnableDisableInstalledPackage"/> command.</param>
        /// <param name="requestData">The <see cref="RequestResponseTypes.EnableDisableInstalledPackage"/> request.</param>
        public EnableDisableInstalledPackageResponse(string message,
                                                     RequestResponseTypes.EnableDisableInstalledPackage requestData)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message;
            //
            EnableDisableInstalledPackage = requestData ?? throw new ArgumentNullException(nameof(requestData));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// A message relaying the results of the <see cref="RequestResponseCommands.EnableDisableInstalledPackage"/> command.
        /// </summary>
        public string Message
        {
            get;
        }
        /// <summary>
        /// The <see cref="RequestResponseTypes.EnableDisableInstalledPackage"/> request.
        /// </summary>
        public RequestResponseTypes.EnableDisableInstalledPackage EnableDisableInstalledPackage
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Message={Message}, {EnableDisableInstalledPackage}";
        #endregion
    }
}
