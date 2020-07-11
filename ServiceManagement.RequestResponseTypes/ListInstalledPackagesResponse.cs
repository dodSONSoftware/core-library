using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about installed packages.
    /// </summary>
    [Serializable]
    public class ListInstalledPackagesResponse
    {
        #region Ctor
        private ListInstalledPackagesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ListInstalledPackagesResponse"/> with the given installed packages.
        /// </summary>
        /// <param name="installedNonDependencyPackages">A collection of installed non-dependency packages.</param>
        /// <param name="installedDependencyPackages">A collection of installed dependency packages.</param>
        /// <param name="installedDisabledPackages">A collection of installed disabled packages.</param>
        /// <param name="installedOrphanedDependencyPackages">A collection of installed orphaned dependency packages.</param>
        /// <param name="installedPackagesWithMissingDependencies">A collection of installed packages with missing dependency packages.</param>
        public ListInstalledPackagesResponse(IEnumerable<Installation.IInstalledPackage> installedNonDependencyPackages, 
                                             IEnumerable<Installation.IInstalledPackage> installedDependencyPackages, 
                                             IEnumerable<Installation.IInstalledPackage> installedDisabledPackages, 
                                             IEnumerable<Installation.IInstalledPackage> installedOrphanedDependencyPackages,
                                             IEnumerable<Installation.IInstalledPackage> installedPackagesWithMissingDependencies)
            : this()
        {
            if (installedNonDependencyPackages == null)
            {
                throw new ArgumentNullException(nameof(installedNonDependencyPackages));
            }
            InstalledNonDependencyPackages = new List<Installation.IInstalledPackage>(installedNonDependencyPackages);
            //
            if (installedDependencyPackages == null)
            {
                throw new ArgumentNullException(nameof(installedDependencyPackages));
            }
            InstalledDependencyPackages = new List<Installation.IInstalledPackage>(installedDependencyPackages);
            //
            if (installedDisabledPackages == null)
            {
                throw new ArgumentNullException(nameof(installedDisabledPackages));
            }
            InstalledDisabledPackages = new List<Installation.IInstalledPackage>(installedDisabledPackages);
            //
            if (installedOrphanedDependencyPackages == null)
            {
                throw new ArgumentNullException(nameof(installedOrphanedDependencyPackages));
            }
            InstalledOrphanedDependencyPackages = new List<Installation.IInstalledPackage>(installedOrphanedDependencyPackages);
            //
            if (installedPackagesWithMissingDependencies == null)
            {
                throw new ArgumentNullException(nameof(installedPackagesWithMissingDependencies));
            }
            InstalledPackagesWithMissingDependencies = new List<Installation.IInstalledPackage>(installedPackagesWithMissingDependencies);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// A collection of installed non-dependency packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstalledNonDependencyPackages
        {
            get;
        }
        /// <summary>
        /// A collection of installed dependency packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstalledDependencyPackages
        {
            get;
        }
        /// <summary>
        /// A collection of installed disabled packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstalledDisabledPackages
        {
            get;
        }
        /// <summary>
        /// A collection of installed orphaned dependency packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstalledOrphanedDependencyPackages
        {
            get;
        }
        /// <summary>
        /// A collection of installed packages with missing dependency packages.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> InstalledPackagesWithMissingDependencies
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Total Installed Packages={InstalledNonDependencyPackages.Count() + InstalledDependencyPackages.Count()}, Non-Dependency Packages={InstalledNonDependencyPackages.Count()}, Dependency Packages={InstalledDependencyPackages.Count()}, Disabled Packages={InstalledDisabledPackages.Count()}, Orphaned Dependency Packages={InstalledOrphanedDependencyPackages.Count()}, Packages With Missing Dependency Packages={InstalledPackagesWithMissingDependencies.Count()}";
        #endregion
    }
}
