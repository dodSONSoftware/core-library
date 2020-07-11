using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the <see cref="RequestResponseCommands.UnInstallAllPackages"/> command.
    /// </summary>
    [Serializable]
    public class UnInstallAllPackagesResponse
    {
        #region Ctor
        private UnInstallAllPackagesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="UnInstallAllPackagesResponse"/> with the given information.
        /// </summary>
        /// <param name="count">The number of installed packages.</param>
        public UnInstallAllPackagesResponse(int count)
            : this()
        {
            Count = count;
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The number of installed packages before the uninstall all command executed.
        /// </summary>
        public int Count
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Uninstalled Packages={Count}";
        #endregion
    }
}
