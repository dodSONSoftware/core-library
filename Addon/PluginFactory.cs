using System;

namespace dodSON.Core.Addon
{
    /// <summary>
    /// Defines controls for loading, unloading and accessing <see cref="IAddon"/>s as plugins loaded into separate <see cref="System.AppDomain"/>s with automatic lifetime sponsorship management.
    /// </summary>
    /// <remarks>
    /// A plugin is a type instantiated in a new <see cref="System.AppDomain"/> with self-maintained lifetime sponsorship. It is important for the consumer to call <see cref="PluginFactory.Unload"/>, this will ensure the <see cref="IAddon"/> is stopped and the <see cref="System.AppDomain"/> is correctly dismantled.
    /// </remarks>
    /// <seealso cref="AppDomain"/>
    /// <seealso cref="AppDomain.TypeProxyFactory{T}"/>
    /// <seealso cref="AppDomain.ITypeProxySponsorHelper"/>
    /// <example>
    /// This example will create a plugin, check a few of its properties, execute a custom method and shutdown the plugin.
    /// <para>
    /// First, create an assembly project and add the following code.
    /// </para>
    /// <code>
    /// namespace CodeExamplesForXMLCommentsSharedAssembly
    /// {
    ///     public interface IAddonExample
    ///     {
    ///         string Hello(string name);
    ///     }
    ///
    ///     public class PluginExample
    ///         : dodSON.Core.Addon.PluginBase,
    ///           IAddonExample
    ///     {
    ///         public PluginExample()
    ///             : base() { }
    ///         //
    ///         protected override void OnStart()
    ///         {
    ///             Console.WriteLine("PluginExample.OnStart()");
    ///         }
    ///         protected override void OnStop()
    ///         {
    ///             Console.WriteLine("PluginExample.OnStop()");
    ///         }
    ///         //
    ///         public string Hello(string name)
    ///         {
    ///             return string.Format("Hello {0}. The CurrentDomain.Id is {1} and the time is {2}", name, 
    ///                                                                                                System.AppDomain.CurrentDomain.Id, 
    ///                                                                                                DateTime.Now);
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
    ///     // A better design would have the shared assembly contain the interface and a secondary assembly contain the derived class. 
    ///     // The main application domain can load the shared assembly, start the secondary application domain which will then
    ///     // load the secondary assembly to gain access to the derived class. Thereby, the main application domain can create an 
    ///     // instance of the derived class and execute methods against it without having to load the secondary assembly.
    ///     //
    ///     // the name of the type to instantiate
    ///     string typeName = typeof(CodeExamplesForXMLCommentsSharedAssembly.PluginExample).FullName;
    ///     
    ///     // the full filename of the assembly containing the type to instantiate
    ///     string assemblyFilename = System.Reflection.Assembly.GetAssembly(typeof(CodeExamplesForXMLCommentsSharedAssembly.PluginExample)).Location;
    ///     
    ///     // a list of assemblies required to be loaded before the type is instantiated
    ///     List&lt;string&gt; preloadAssemblyFilenames = null;
    ///     
    ///     // the full path containing the assembly containing the type to instantiate
    ///     string applicationBasePath = System.IO.Path.GetDirectoryName(assemblyFilename);
    ///     
    ///     // used if you require assemblies that reside in different paths. (i.e. dependency assemblies)
    ///     string privateBinPath = "";
    ///     
    ///     // create the TypeProxySettings
    ///     var settings = new dodSON.Core.AppDomains.TypeProxySettings(typeName, 
    ///                                                                          assemblyFilename, 
    ///                                                                          preloadAssemblyFilenames, 
    ///                                                                          applicationBasePath, 
    ///                                                                          privateBinPath);
    ///                                                                          
    ///     // create the PluginFactory
    ///     var pluginFactory = new dodSON.Core.Addon.PluginFactory(settings);
    ///     
    ///     // create the plugin
    ///     Console.WriteLine("CurrentDomain.Id= " + System.AppDomain.CurrentDomain.Id);
    ///     Console.WriteLine("IsLoaded= " + pluginFactory.IsLoaded);
    ///     Console.WriteLine("Loading");
    ///     var plugin = pluginFactory.Load();
    ///     Console.WriteLine("IsLoaded= " + pluginFactory.IsLoaded);
    ///     
    ///     //
    ///     if (pluginFactory.IsLoaded)
    ///     {
    ///         // start the plugin
    ///         Console.WriteLine("IsRunning= " + plugin.IsRunning);
    ///         Console.WriteLine("Starting");
    ///         plugin.Start();
    ///         Console.WriteLine("IsRunning= " + plugin.IsRunning);
    ///         if (plugin.IsRunning)
    ///         {
    ///             var dude = plugin as CodeExamplesForXMLCommentsSharedAssembly.IAddonExample;
    ///             if (dude != null)
    ///             {
    ///                 Console.WriteLine("Executing Custom Method");
    ///                 // execute custom method on plugin which will be executed in the remote AppDomain
    ///                 Console.WriteLine(dude.Hello("Randy"));
    ///             }
    ///             // stop the plugin
    ///             plugin.Stop();
    ///         }
    ///         // unload the plugin
    ///         pluginFactory.Unload();
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
    /// // PluginExample.OnStart()
    /// // IsRunning= True
    /// // Executing Custom Method
    /// // Hello Randy. The CurrentDomain.Id is 2 and the time is 11/20/2015 5:13:10 PM
    /// // PluginExample.OnStop()
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable()]
    public class PluginFactory
        : IAddonFactory
    {
        #region Ctor
        private PluginFactory() { }
        /// <summary>
        /// Initializes a new instance of the PluginFactory.
        /// </summary>
        /// <param name="settings">Settings to control the creation of the type and its proxy.</param>
        public PluginFactory(AppDomain.TypeProxySettings settings)
            : this()
        {
            _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        #endregion
        #region Private Fields
        private readonly AppDomain.TypeProxySettings _Settings = null;
        private AppDomain.TypeProxyFactory<IAddon> _Factory = null;
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
        /// Loads and returns the <see cref="IAddon"/>, which will be created in a separate <see cref="System.AppDomain"/> with automatic lifetime sponsorship management.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>.</returns>
        /// <seealso cref="dodSON.Core.AppDomain"/>
        /// <seealso cref="dodSON.Core.AppDomain.TypeProxyFactory{T}"/>
        /// <seealso cref="dodSON.Core.AppDomain.ITypeProxySponsorHelper"/>
        public IAddon Load()
        {
            if (Addon == null)
            {
                _Factory = new AppDomain.TypeProxyFactory<IAddon>(_Settings);
                Addon = _Factory.Instance;
            }
            return Addon;
        }
        /// <summary>
        /// Unloads the <see cref="IAddon"/> and properly shuts down the remote <see cref="System.AppDomain"/>.
        /// </summary>
        public void Unload()
        {
            if (_Factory != null)
            {
                Addon = null;
                _Factory.Unload();
                _Factory = null;
            }
        }
        /// <summary>
        /// Returns the loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.</returns>
        public IAddon Addon { get; private set; }
        #endregion
    }
}
