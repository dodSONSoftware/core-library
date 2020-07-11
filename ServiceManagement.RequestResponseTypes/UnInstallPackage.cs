using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the package to be uninstalled.
    /// </summary>
    [Serializable]
    public class UnInstallPackage
    {
        #region Ctor
        private UnInstallPackage()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="UnInstallPackage"/> with the given name and version.
        /// </summary>
        /// <param name="name">The name of the package to uninstall.</param>
        /// <param name="version">The version of the package to uninstall.</param>
        public UnInstallPackage(string name,
                                Version version)
            : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            Name = name;
            //
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The name of the package to uninstall.
        /// </summary>
        public string Name
        {
            get; 
        }
        /// <summary>
        /// The version of the package to uninstall.
        /// </summary>
        public Version Version
        {
            get; 
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={Name}, Version={Version}";
        #endregion
    }
}
