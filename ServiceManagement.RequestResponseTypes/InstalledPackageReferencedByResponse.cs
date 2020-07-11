using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains a list of installed packages which reference the specified installed package.
    /// </summary>
    [Serializable]
    public class InstalledPackageReferencedByResponse
    {
        #region Ctor
        private InstalledPackageReferencedByResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="InstalledPackageReferencedByResponse"/> with the given installed package and reference list.
        /// </summary>
        /// <param name="request">The installed package to get the list of referencing installed packages for.</param>
        /// <param name="referenceList">The list of installed packages referencing the specified installed package.</param>
        public InstalledPackageReferencedByResponse(InstalledPackageReferencedBy request,
                                                    IEnumerable<Installation.IInstalledPackage> referenceList)
            : this()
        {
            SourcePackage = request ?? throw new ArgumentNullException(nameof(request));
            //
            if (referenceList == null)
            {
                throw new ArgumentNullException(nameof(referenceList));
            }
            ReferencingInstalledPackages = new List<Installation.IInstalledPackage>(referenceList);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The installed package to get the list of referencing installed packages for.
        /// </summary>
        public InstalledPackageReferencedBy SourcePackage
        {
            get;
        }
        /// <summary>
        /// The list of installed packages which reference the specified installed package.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> ReferencingInstalledPackages
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={SourcePackage.Name}, Version={SourcePackage.Version}, Reference Count={ReferencingInstalledPackages.Count()}";
        #endregion
    }
}
