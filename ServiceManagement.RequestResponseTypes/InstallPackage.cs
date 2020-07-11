using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about the package to be installed.
    /// </summary>
    [Serializable]
    public class InstallPackage
    {
        #region Ctor
        private InstallPackage()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="InstallPackage"/> with the given name and version.
        /// </summary>
        /// <param name="name">The name of the package to install.</param>
        /// <param name="version">The version of the package to install.</param>
        public InstallPackage(string name,
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
        /// The name of the package to install.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The version of the package to install.
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