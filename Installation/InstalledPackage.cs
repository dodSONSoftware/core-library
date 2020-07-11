using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Represents a single installed package.
    /// </summary>
    [Serializable]
    public class InstalledPackage
        : IInstalledPackage
    {
        #region Ctor
        private InstalledPackage() { }
        /// <summary>
        /// Instantiates a new installed package.
        /// </summary>
        /// <param name="installPath">The path where this installed package is installed.</param>
        /// <param name="package">The package representing this installed package.</param>
        public InstalledPackage(string installPath,
                                Packaging.IPackage package)
            : this()
        {
            if (string.IsNullOrWhiteSpace(installPath)) { throw new ArgumentNullException(nameof(installPath)); }
            InstallPath = installPath;
            Package = package ?? throw new ArgumentNullException(nameof(package));
        }
        #endregion
        #region IInstalledPackageInformation Methods
        /// <summary>
        /// The root path where this installed package is installed.
        /// </summary>
        public string InstallPath { get; private set; }
        /// <summary>
        /// The <see cref="Packaging.IPackage"/> representing this installed package.
        /// </summary>
        public Packaging.IPackage Package { get; private set; }
        #endregion
        #region Overrides
        /// <summary>
        /// Returns <see cref="Packaging.Package.FullyQualifiedPackageName"/>.
        /// </summary>
        /// <returns>Returns <see cref="Packaging.Package.FullyQualifiedPackageName"/>.</returns>
        public override string ToString() { return Package.FullyQualifiedPackageName; }
        #endregion
    }
}
