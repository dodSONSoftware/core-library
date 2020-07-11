using System;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Defines a dependency package.
    /// </summary>
    public interface IDependencyPackage
        : Configuration.IConfigurable
    {
        /// <summary>
        /// The dependency package name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The minimum version required for the dependency package.
        /// </summary>
        Version MinimumVersion { get; }
        /// <summary>
        /// <para>The specific version desired.</para>
        /// <para>Optional, if supplied, the <see cref="Installation"/> types will attempt to supply this version if available.</para>
        /// </summary>
        /// <remarks>
        /// Can be used to patch into alternative packages, when a newer or different dependency package breaks your package. To install multiple packages with the same name, but different versions, you will need to install it <see cref="Installation.InstallType.SideBySide"/>; See the <see cref="Installation"/> namespace for more information.
        /// </remarks>
        Version SpecificVersion { get; set; }
    }
}
