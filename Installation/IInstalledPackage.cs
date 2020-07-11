using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Defines a single installed package.
    /// </summary>
    public interface IInstalledPackage
    {
        /// <summary>
        /// The root path where this installed package is installed.
        /// </summary>
        string InstallPath { get; }
        /// <summary>
        /// The <see cref="Packaging.IPackage"/> representing this installed package.
        /// </summary>
        Packaging.IPackage Package { get; }
    }
}
