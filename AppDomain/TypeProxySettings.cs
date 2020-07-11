using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Defines elements needed for the <see cref="TypeProxyFactory{T}"/> and the <see cref="TypeProxyFactory{T}"/> to help it properly create and maintain <see cref="Type"/>s in another <see cref="System.AppDomain"/>.
    /// </summary>
    [Serializable]
    public class TypeProxySettings
    {
        #region Ctor
        private TypeProxySettings() { }
        /// <summary>
        /// Initializes a new instance of TypeProxySettings.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="Type"/> to instantiate.</param>
        /// <param name="assemblyFilename">The filename of the assembly where the <paramref name="typeName"/> is located. Null, or empty, will search the executing assembly.</param>
        /// <param name="preloadAssemblyFilenames">A list of <see cref="Type"/>s that will be loaded before the <paramref name="typeName"/> is instantiated. Null equates to an empty list.</param>
        /// <param name="setupInfo">Assembly binding information that will be added to the new <see cref="System.AppDomain"/>. Null will use the current <see cref="System.AppDomain"/>'s <see cref="System.AppDomainSetup"/>.</param>
        public TypeProxySettings(string typeName,
                                 string assemblyFilename,
                                 IEnumerable<string> preloadAssemblyFilenames,
                                 System.AppDomainSetup setupInfo)
            : this()
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(typeName)) { throw new ArgumentNullException(nameof(typeName)); }
            TypeName = typeName;
            AssemblyFilename = (string.IsNullOrWhiteSpace(assemblyFilename)) ? null : assemblyFilename;
            PreloadAssemblyFilenames = preloadAssemblyFilenames ?? new string[0];
            SetupInfo = setupInfo ?? throw new ArgumentNullException(nameof(setupInfo));
        }
        /// <summary>
        /// Initializes a new instance of TypeProxySettings.
        /// </summary>
        /// <param name="typeName">The name of the <see cref="Type"/> to instantiate.</param>
        /// <param name="assemblyFilename">The filename of the assembly where the <paramref name="typeName"/> is located.</param>
        /// <param name="preloadAssemblyFilenames">A list of <see cref="Type"/>s that will be loaded before the <paramref name="typeName"/> is instantiated.</param>
        /// <param name="applicationBasePath">The name of the directory containing the application.</param>
        /// <param name="privateBinPath">A list of directories that are probed for private assemblies. Each directory should be separated by semicolon [;].</param>
        public TypeProxySettings(string typeName,
                                 string assemblyFilename,
                                 IEnumerable<string> preloadAssemblyFilenames,
                                 string applicationBasePath,
                                 string privateBinPath) : this(typeName, assemblyFilename, preloadAssemblyFilenames, new AppDomainSetup() { ApplicationBase = applicationBasePath, PrivateBinPath = privateBinPath }) { }
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets the name of the <see cref="Type"/> to instantiate.
        /// </summary>
        public string TypeName { get; }
        /// <summary>
        /// Gets the filename of the assembly where the <see cref="TypeName"/> is located.
        /// </summary>
        public string AssemblyFilename { get; }
        /// <summary>
        /// Gets a list of <see cref="Type"/>s that will be loaded before the <see cref="TypeName"/> is instantiated.
        /// </summary>
        public IEnumerable<string> PreloadAssemblyFilenames { get; }
        /// <summary>
        /// Gets the assembly binding information that will be added to the new <see cref="System.AppDomain"/>.
        /// </summary>
        public System.AppDomainSetup SetupInfo { get; }
        #endregion
    }
}
