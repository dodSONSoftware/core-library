using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Defines the types of installations that are available.
    /// </summary>
    public enum InstallType
    {
        /// <summary>
        /// Indicates to the <see cref="IInstaller"/> to allow packages with the same name and different version to be installed together, i.e. side by side.
        /// </summary>
        SideBySide = 0,
        /// <summary>
        /// Indicates to the <see cref="IInstaller"/> to attempt to use only the highest versions of any named package. 
        /// However, if a dependency package contains a <see cref="Packaging.IDependencyPackage.SpecificVersion"/>, then that specific version will be installed, if possible.
        /// </summary>
        /// <seealso cref="Installation"/>
        HighestVersionOnly
    }
}
