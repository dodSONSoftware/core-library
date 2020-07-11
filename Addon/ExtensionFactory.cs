using System;
using System.Linq;

namespace dodSON.Core.Addon
{
    /// <summary>
    /// Defines controls for loading, unloading and accessing <see cref="IAddon"/>s as extensions loaded into the current <see cref="System.AppDomain"/>.
    /// </summary>
    /// <example>
    /// This example will create an extension, check a few of its properties, execute a custom method and shutdown the extension.
    /// <para>
    /// First, create an assembly project and add the following code.
    /// </para>
    /// <code>
    /// namespace CodeExamplesForXMLCommentsSharedAssembly
    /// {
    /// 
    ///     public interface IAddonExample
    ///     {
    ///         string Hello(string name);
    ///     }
    ///
    ///     public class ExtensionExample
    ///         : dodSON.Core.Addon.ExtensionBase,
    ///           IAddonExample
    ///     {
    ///         public ExtensionExample()
    ///             : base() { }
    ///         //
    ///         protected override void OnStart()
    ///         {
    ///             Console.WriteLine("ExtensionExample.OnStart()");
    ///         }
    ///         protected override void OnStop()
    ///         {
    ///             Console.WriteLine("ExtensionExample.OnStop()");
    ///         }
    ///         //
    ///         public string Hello(string name)
    ///         {
    ///             return string.Format("Hello {0}. The CurrentDomain.Id is {1} and the time is {2}", name, System.AppDomain.CurrentDomain.Id, DateTime.Now);
    ///         }
    ///     }
    /// }
    /// </code>
    /// <para>
    /// Then, create a console application and add the following code.
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // the name of the type to instantiate
    ///     string typeName = typeof(CodeExamplesForXMLCommentsSharedAssembly.ExtensionExample).FullName;
    ///     
    ///     // the full filename of the assembly containing the type to instantiate
    ///     string assemblyFilename = System.Reflection.Assembly.GetAssembly(typeof(CodeExamplesForXMLCommentsSharedAssembly.ExtensionExample)).Location;
    ///     
    ///     // the full path containing the assembly containing the type to instantiate
    ///     string applicationBasePath = System.IO.Path.GetDirectoryName(assemblyFilename);
    ///     
    ///     // used if you require assemblies that reside in different paths. (i.e. dependency assemblies)
    ///     string privateBinPath = "";
    ///     
    ///     // create the ExtensionFactory
    ///     var extensionFactory = new dodSON.Core.Addon.ExtensionFactory(typeName, assemblyFilename, applicationBasePath, privateBinPath);
    ///     
    ///     // create the extension
    ///     Console.WriteLine("CurrentDomain.Id= " + System.AppDomain.CurrentDomain.Id);
    ///     Console.WriteLine("IsLoaded= " + extensionFactory.IsLoaded);
    ///     Console.WriteLine("Loading");
    ///     var extension = extensionFactory.Load();
    ///     Console.WriteLine("IsLoaded= " + extensionFactory.IsLoaded);
    ///     
    ///     //
    ///     if (extensionFactory.IsLoaded)
    ///     {
    ///         // start the extension
    ///         Console.WriteLine("IsRunning= " + extension.IsRunning);
    ///         Console.WriteLine("Starting");
    ///         extension.Start();
    ///         Console.WriteLine("IsRunning= " + extension.IsRunning);
    ///         if (extension.IsRunning)
    ///         {
    ///             var dude = extension as CodeExamplesForXMLCommentsSharedAssembly.IAddonExample;
    ///             if (dude != null)
    ///             {
    ///                 Console.WriteLine("Executing Custom Method");
    ///                 // execute custom method on extension which will be executed in the same AppDomain
    ///                 Console.WriteLine(dude.Hello("Randy"));
    ///             }
    ///             // stop the extension
    ///             extension.Stop();
    ///         }
    ///         // unload the extension
    ///         extensionFactory.Unload();
    ///     }
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // CurrentDomain.Id= 1
    /// // IsLoaded= False
    /// // Loading
    /// // IsLoaded= True
    /// // IsRunning= False
    /// // Starting
    /// // ExtensionExample.OnStart()
    /// // IsRunning= True
    /// // Executing Custom Method
    /// // Hello Randy. The CurrentDomain.Id is 1 and the time is 11/20/2015 5:30:34 PM
    /// // ExtensionExample.OnStop()
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable()]
    public class ExtensionFactory
        : IAddonFactory
    {
        #region Ctor
        /// <summary>
        /// A private constructor.
        /// </summary>
        private ExtensionFactory() { }
        /// <summary>
        /// Initializes a new instance of the PluginFactory.
        /// </summary>
        /// <param name="typeName">The <see cref="System.Type"/> of extension to instantiate.</param>
        /// <param name="assemblyName">The name of the assembly file, .dll, which contains the <paramref name="typeName"/> <see cref="System.Type"/> to instantiate.</param>
        /// <param name="extensionBaseDirectory">The path where the <paramref name="assemblyName"/> file can be found.</param>
        /// <param name="privateBinPath">A list of semicolon (;) separated paths containing other required assemblies.</param>
        public ExtensionFactory(string typeName,
                                string assemblyName,
                                string extensionBaseDirectory,
                                string privateBinPath)
            : this()
        {
            if (string.IsNullOrWhiteSpace(typeName)) { throw new ArgumentNullException(nameof(typeName)); }
            if (string.IsNullOrWhiteSpace(assemblyName)) { throw new ArgumentNullException(nameof(assemblyName)); }
            if (string.IsNullOrWhiteSpace(extensionBaseDirectory)) { throw new ArgumentNullException(nameof(extensionBaseDirectory)); }
            _TypeName = typeName;
            _AssemblyName = assemblyName;
            _ExtensionBaseDirectory = extensionBaseDirectory;
            _PrivateBinPath = privateBinPath;
        }
        #endregion
        #region Private Fields
        private string _TypeName = "";
        private string _AssemblyName = "";
        private readonly string _ExtensionBaseDirectory = "";
        private string _PrivateBinPath = "";
        #endregion
        #region Public Methods
        /// <summary>
        /// Returns whether the <see cref="IAddon"/> is loaded. Returns <b>true</b> if the <see cref="IAddon"/> is loaded; otherwise, <b>false</b>.
        /// </summary>
        public bool IsLoaded
        {
            get { return Addon != null; }
        }
        /// <summary>
        /// Loads and returns the <see cref="IAddon"/>.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>.</returns>
        public IAddon Load()
        {
            if (Addon == null)
            {
                System.AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(appDomain_AssemblyResolve);
                var typeName = _TypeName;
                if (_TypeName.Contains(',')) { typeName = _TypeName.Split(',')[0]; }
                Addon = (IAddon)Activator.CreateInstance(_AssemblyName, typeName).Unwrap();                
                System.AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(appDomain_AssemblyResolve);
            }
            return Addon;
        }
        /// <summary>
        /// Unloads the <see cref="IAddon"/>.
        /// </summary>
        public void Unload()
        { }
        /// <summary>
        /// Returns the loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.</returns>
        public IAddon Addon { get; private set; }
        #endregion
        #region Private Methods
        private System.Reflection.Assembly appDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args.Name))
                {
                    // get assembly's simple name
                    var parts = args.Name.Split(new char[] { ',' });
                    if ((parts != null) && (parts.Length > 0))
                    {
                        // **** search current appdomain
                        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                        {
                            if (assembly.FullName.StartsWith(parts[0], StringComparison.InvariantCultureIgnoreCase))
                            {
                                // return already loaded assembly
                                return assembly;
                            }
                        }
                        // **** load from known assembly file
                        {
                            var filename = System.IO.Path.Combine(_ExtensionBaseDirectory, parts[0]);
                            if (System.IO.File.Exists(filename))
                            {
                                // KNOWN ISSUE: LoadFrom(...) will not properly load the same extensions from different locations.
                                //          It will always load the first extension loaded...
                                return System.Reflection.Assembly.LoadFrom(filename);
                            }
                        }
                        // NOTE: technically this property should contain only subdirectories relative to the BaseDirectory
                        //       I don't, necessarily, use it that way...
                        // search each directory in PrivateBinPath [this is my string inside PrivateBinPath (full path directories, separated by semicolons)]
                        if (!string.IsNullOrWhiteSpace(_PrivateBinPath))
                        {
                            foreach (var path in _PrivateBinPath.Split(new char[] { ';' }))
                            {
                                var filename = System.IO.Path.Combine(path, parts[0]);
                                if (System.IO.File.Exists(filename))
                                {
                                    // KNOWN ISSUE: LoadFrom(...) will not properly load the same extensions from different locations.
                                    //          It will always load the first extension loaded...
                                    return System.Reflection.Assembly.LoadFrom(filename);
                                }
                            }
                        }
                    }
                }
            }
            // swallow all exceptions and return null; null indicates an unresolved assembly. (an exception will be thrown by the system)
            catch (Exception) { }
            return null;
        }
        #endregion
    }
}
