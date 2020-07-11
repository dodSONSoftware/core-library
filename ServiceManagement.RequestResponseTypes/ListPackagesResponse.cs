using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains information about packages.
    /// </summary>
    [Serializable]
    public class ListPackagesResponse
    {
        #region Ctor
        private ListPackagesResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="ListPackagesResponse"/> with the given packages.
        /// </summary>
        /// <param name="nonDependencyPackages">A list of non-dependency packages.</param>
        /// <param name="dependencyPackages">A list of dependency packages.</param>
        public ListPackagesResponse(IEnumerable<Packaging.IPackage> nonDependencyPackages,
                                    IEnumerable<Packaging.IPackage> dependencyPackages)
            : this()
        {
            if (nonDependencyPackages == null)
            {
                throw new ArgumentNullException(nameof(nonDependencyPackages));
            }
            NonDependencyPackages = new List<Packaging.IPackage>(nonDependencyPackages);
            //
            if (dependencyPackages == null)
            {
                throw new ArgumentNullException(nameof(dependencyPackages));
            }
            DependencyPackages = new List<Packaging.IPackage>(dependencyPackages);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// A collection of non-dependency packages.
        /// </summary>
        public IEnumerable<Packaging.IPackage> NonDependencyPackages
        {
            get;
        }
        /// <summary>
        /// A collection of dependency packages.
        /// </summary>
        public IEnumerable<Packaging.IPackage> DependencyPackages
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Total Packages={NonDependencyPackages.Count() + DependencyPackages.Count()}, Non-Dependency Packages={NonDependencyPackages.Count()}, Dependency Packages={DependencyPackages.Count()}";
        #endregion
    }
}
