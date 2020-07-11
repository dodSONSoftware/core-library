using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Requests the dependency chain for the specified installed package.
    /// </summary>
    [Serializable]
    public class InstalledPackageDependencyChain
    {
        #region Ctor
        private InstalledPackageDependencyChain()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="InstalledPackageDependencyChain"/> with the given name and version.
        /// </summary>
        /// <param name="name">The name of the package to get the dependency chain for.</param>
        /// <param name="version">The version of the package to get the dependency chain for.</param>
        public InstalledPackageDependencyChain(string name,
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
        /// The name of the package to get the dependency chain for.
        /// </summary>
        public string Name
        {
            get;
        }
        /// <summary>
        /// The version of the package to get the dependency chain for.
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
