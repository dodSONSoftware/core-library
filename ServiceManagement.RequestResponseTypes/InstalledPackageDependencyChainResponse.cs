using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.ServiceManagement.RequestResponseTypes
{
    /// <summary>
    /// Contains the dependency chain information for the specified installed package.
    /// </summary>
    [Serializable]
    public class InstalledPackageDependencyChainResponse
    {
        #region Ctor
        private InstalledPackageDependencyChainResponse()
        {
        }
        /// <summary>
        /// Instantiates a new <see cref="InstalledPackageDependencyChainResponse"/> with the given installed package and dependency chain.
        /// </summary>
        /// <param name="request">The installed package to get the dependency chain for.</param>
        /// <param name="chain">The dependency chain for the installed package.</param>
        public InstalledPackageDependencyChainResponse(InstalledPackageDependencyChain request,
                                                       IEnumerable<Installation.IInstalledPackage> chain)
            : this()
        {
            SourcePackage = request ?? throw new ArgumentNullException(nameof(request));
            //
            if (chain == null)
            {
                throw new ArgumentNullException(nameof(chain));
            }
            DependencyChain = new List<Installation.IInstalledPackage>(chain);
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The installed package to get the dependency chain for.
        /// </summary>
        public InstalledPackageDependencyChain SourcePackage
        {
            get;
        }
        /// <summary>
        /// The dependency chain for the installed package.
        /// </summary>
        public IEnumerable<Installation.IInstalledPackage> DependencyChain
        {
            get;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// A string representation of this object.
        /// </summary>
        /// <returns>A string representation of this object.</returns>
        public override string ToString() => $"Name={SourcePackage.Name}, Version={SourcePackage.Version}, Chain Count={DependencyChain.Count()}";
        #endregion
    }
}
