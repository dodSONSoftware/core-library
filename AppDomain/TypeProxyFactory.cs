using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Facilitates the creation, and control, of types in separate application domains with automatic lifetime service sponsorship.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to create, remotely, in a new <see cref="System.AppDomain"/>.</typeparam>
    /// <seealso cref="MarshalByRefObject"/>
    /// <example>
    /// The following code example will create a remote type factory in a new application domain, use it to create 
    /// a data holder and get its proxy. It will use the proxy to read and write properties and execute a method; 
    /// all of which will occur, for the data holder, in the remote application domain.
    /// <para>
    /// <b>Note:</b> Any type wanting to be remotely instantiated, must inherit from the abstract class <see cref="MarshalByRefObject"/>.
    /// </para>
    /// <para>
    /// First, create a shared assembly with the following code:
    /// </para>
    /// <code>
    /// namespace CodeExamplesForXMLCommentsSharedAssembly
    /// {
    ///     [Serializable]
    ///     public class DataHolder
    ///         : MarshalByRefObject
    ///     {
    ///         public DataHolder()
    ///         {
    ///             Alpha = "";
    ///             Beta = 0;
    ///             Gamma = 0;
    ///             Delta = new byte[0];
    ///         }
    ///
    ///         public string Alpha { get; set; }
    ///         public int Beta { get; set; }
    ///         public decimal Gamma { get; set; }
    ///         public byte[] Delta { get; set; }
    ///         public string Hello(string name) { return string.Format("Hello {0}. CurrentDomain.Id= {1}; DateTime.Now= {2}", 
    ///                                                                     name, System.AppDomain.CurrentDomain.Id, DateTime.Now); }
    ///     }
    /// }
    /// </code>
    /// <para>
    /// Next, create a console application and add the following code:
    /// </para>
    /// <code>
    /// // the name of the type to instantiate
    /// var typeName = typeof(CodeExamplesForXMLCommentsSharedAssembly.DataHolder).FullName;
    /// 
    /// // the full filename of the assembly containing the type to instantiate
    /// var assemblyFilename = System.Reflection.Assembly.GetAssembly(typeof(CodeExamplesForXMLCommentsSharedAssembly.DataHolder)).Location;
    /// 
    /// // any assemblies required to be loaded before the type is instantiated
    /// string[] preloadAssemblyFilenames = null;
    /// 
    /// // the path of the assembly containing the type to instantiate 
    /// var applicationBasePath = System.IO.Path.GetDirectoryName(assemblyFilename);
    /// 
    /// // a list of directories to probe for required assemblies.
    /// var privateBinPath = "";
    /// 
    /// // create the settings
    /// var settings = new dodSON.Core.AppDomains.TypeProxySettings(typeName, assemblyFilename, preloadAssemblyFilenames, applicationBasePath, privateBinPath);
    /// 
    /// // create the remote factory
    /// var factory = new dodSON.Core.AppDomains.TypeProxyFactory&lt;CodeExamplesForXMLCommentsSharedAssembly.DataHolder&gt;(settings);
    /// 
    /// // load the remote instance
    /// Console.WriteLine("CurrentDomain.Id= " + System.AppDomain.CurrentDomain.Id);
    /// Console.WriteLine("factory.IsLoaded= " + factory.IsLoaded);
    /// Console.WriteLine("Getting factory.Instance");
    /// var dude = factory.Instance;
    /// Console.WriteLine("factory.IsLoaded= " + factory.IsLoaded);
    /// 
    /// // check values
    /// Console.WriteLine("Displaying Values:");
    /// Console.WriteLine("dude.Alpha= " + dude.Alpha);
    /// Console.WriteLine("dude.Beta= " + dude.Beta);
    /// Console.WriteLine("dude.Gamma= " + dude.Gamma);
    /// Console.WriteLine("dude.Delta= " + dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(dude.Delta));
    /// 
    /// // mutate values
    /// Console.WriteLine("Mutating Values:");
    /// dude.Alpha = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio.";
    /// dude.Beta = 1024;
    /// dude.Gamma = 2.71828M;
    /// dude.Delta = new byte[] { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff };
    /// 
    /// // check values
    /// Console.WriteLine("dude.Alpha= " + dude.Alpha);
    /// Console.WriteLine("dude.Beta= " + dude.Beta);
    /// Console.WriteLine("dude.Gamma= " + dude.Gamma);
    /// Console.WriteLine("dude.Delta= " + dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(dude.Delta));
    /// Console.WriteLine("Executing Remote Function:");
    /// Console.WriteLine("dude.Hello(\"Randy\")= " + dude.Hello("Randy"));
    /// Console.WriteLine();
    /// Console.WriteLine("press anykey...");
    /// Console.ReadKey(true);
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // CurrentDomain.Id= 1
    /// // factory.IsLoaded= False
    /// // Getting factory.Instance
    /// // factory.IsLoaded= True
    /// // Displaying Values:
    /// // dude.Alpha=
    /// // dude.Beta= 0
    /// // dude.Gamma= 0
    /// // dude.Delta=
    /// // Mutating Values:
    /// // dude.Alpha= Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio.
    /// // dude.Beta= 1024
    /// // dude.Gamma= 2.71828
    /// // dude.Delta= 00102030405060708090A0B0C0D0E0F0FF
    /// // Executing Remote Function:
    /// // dude.Hello("Randy")= Hello Randy. CurrentDomain.Id= 2; DateTime.Now= 11/23/2015 3:47:32 PM
    /// //
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable()]
    public class TypeProxyFactory<T>
        : ITypeProxySponsorHelper
          where T : class
        // FYI: there could have been a new() applied here, but that would keep it from instantiating types through interfaces.
        //    : This type needs to be tested for instantiation of types through interfaces and well as classes.
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of TypeProxyFactory.
        /// </summary>
        public TypeProxyFactory() : this("", new string[0]) { }
        /// <summary>
        /// Initializes a new instance of TypeProxyFactory.
        /// </summary>
        /// <param name="privateBinPath">A list of directories that are probed for private assemblies. Each directory should be separated by semicolon [;].</param>
        /// <param name="preloadAssemblyFilenames">A list of assembly filenames that will be loaded before the proxy is instantiated.</param>
        public TypeProxyFactory(string privateBinPath,
                                 IEnumerable<string> preloadAssemblyFilenames)
        {
            var t = typeof(T);
            var typeName = t.FullName;
            var assemblyFilename = System.Reflection.Assembly.GetAssembly(t).Location;
            var applicationBasePath = System.IO.Path.GetDirectoryName(assemblyFilename);
            _Settings = new TypeProxySettings(typeName, assemblyFilename, preloadAssemblyFilenames, applicationBasePath, privateBinPath);
        }
        /// <summary>
        /// Initializes a new instance of TypeProxyFactory2.
        /// </summary>
        /// <param name="settings">The settings required to properly create and maintain the type in a separate <see cref="System.AppDomain"/>.</param>
        public TypeProxyFactory(TypeProxySettings settings) => _Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        #endregion
        #region Private Fields
        private T _Instance = null;
        private TypeProxySponsor _Sponsor = null;
        private readonly TypeProxySettings _Settings;
        #endregion
        #region Public Methods
        /// <summary>
        /// Returns the <see cref="System.AppDomain"/> for this <see cref="TypeProxyFactory{T}"/>. 
        /// </summary>
        public System.AppDomain AppDomain { get; private set; }
        /// <summary>
        /// <returns><b>True</b> if type  has been instantiated in a new <see cref="System.AppDomain"/>, otherwise <b>false</b>.</returns>
        /// </summary>
        public bool IsLoaded => _Instance != null;
        /// <summary>
        /// Returns a remote proxy for the instance of type instantiated in a new <see cref="System.AppDomain"/>.
        /// <note type="note">
        /// If the type <typeparamref name="T"/> has not been instantiated, this function will create a new <see cref=" System.AppDomain"/>, load any listed preload assemblies, register lifetime service sponsership and instantiate the type <typeparamref name="T"/>.
        /// </note>
        /// </summary>
        public T Instance => Load();
        /// <summary>
        /// Registers the lifetime service sponsorship and loads the <see cref="System.AppDomain"/>.
        /// </summary>
        /// <returns>A remote proxy for the instance of type instantiated in a new <see cref="System.AppDomain"/>.</returns>
        public T Load()
        {
            if (_Instance == null)
            {
                // create remote type factory
                var typeFactory = CreateTypeFactory();
                AppDomain = typeFactory.AppDomain;
                // preload all assemblies
                typeFactory.LoadAssemblies(_Settings.PreloadAssemblyFilenames);
                // create remote instance
                var typeName = _Settings.TypeName;
                // check and fix the ( typeName )
                if (_Settings.TypeName.Contains(','))
                {
                    typeName = _Settings.TypeName.Split(',')[0];
                }
                // create instance using the ( typeName ) and the ( TypeProxySettings.AssemblyFilename )
                _Instance = typeFactory.CreateInstance<T>(typeName, _Settings.AssemblyFilename);
                // add sponsor
                _Sponsor = new TypeProxySponsor(this);
                var leaseInfo = (System.Runtime.Remoting.Lifetime.ILease)System.Runtime.Remoting.RemotingServices.GetLifetimeService(_Instance as MarshalByRefObject);
                leaseInfo.Register(_Sponsor);
            }
            return _Instance;
        }
        /// <summary>
        /// Unregister the lifetime service sponsorship and unloads the <see cref="System.AppDomain"/>.
        /// </summary>
        public void Unload()
        {
            if (_Instance != null)
            {
                // remove sponsor
                var leaseInfo = (System.Runtime.Remoting.Lifetime.ILease)System.Runtime.Remoting.RemotingServices.GetLifetimeService(_Instance as MarshalByRefObject);
                leaseInfo.Unregister(_Sponsor);
                _Sponsor = null;
                // release instance
                _Instance = null;
                // unload appdomain
                System.AppDomain.Unload(AppDomain);
                AppDomain = null;
            }
        }
        #endregion
        #region Private Methods
        private AppDomainTypeFactory CreateTypeFactory()
        {
            if (_Settings.SetupInfo != null)
            {
                return AppDomainTypeFactory.Create(_Settings.SetupInfo);
            }
            else
            {
                return AppDomainTypeFactory.Create();
            }
        }
        #endregion
    }
}
