using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Represents a type factory that can be used to create any number of types in a remote application domain.
    /// </summary>
    /// <seealso cref="System.MarshalByRefObject"/>
    [Serializable()]
    public class AppDomainTypeFactory
        : MarshalByRefObject,
          ITypeProxySponsorHelper
    {
        #region Factory Methods
        // Private Static Constants

        // TODO: is there is a better, more reliable way to get this information...

        private static readonly string _AppDomainTypeFactory_AssemblyName_ = "dodSON.Core";
        private static readonly string _AppDomainTypeFactory_TypeName_ = "dodSON.Core.AppDomains.AppDomainTypeFactory";
        private static readonly string _AssumedAssemblyFilenameExtension_ = ".dll";

        // Public Static Factory Methods
        /// <summary>
        /// Initializes a new instance of AppDomainTypeFactory using the given <see cref="System.AppDomainSetup"/>.
        /// </summary>
        /// <param name="setupInfo">Assembly binding information that will be added to the new <see cref="System.AppDomain"/>.</param>
        /// <returns>A new instance of AppDomainTypeFactory using the given <see cref="System.AppDomainSetup"/>.</returns>
        public static AppDomainTypeFactory Create(System.AppDomainSetup setupInfo)
        {
            if (setupInfo == null)
            {
                throw new ArgumentNullException(nameof(setupInfo));
            }
            var appDomain = GetNewAppDomain(setupInfo);
            var instance = (AppDomainTypeFactory)appDomain.CreateInstanceAndUnwrap(_AppDomainTypeFactory_AssemblyName_, _AppDomainTypeFactory_TypeName_);
            // add sponsor
            var sponsor = new TypeProxySponsor(instance);
            var leaseInfo = (System.Runtime.Remoting.Lifetime.ILease)System.Runtime.Remoting.RemotingServices.GetLifetimeService(instance as MarshalByRefObject);
            leaseInfo.Register(sponsor);
            return instance;
        }
        /// <summary>
        /// Initializes a new instance of AppDomainTypeFactory using the current <see cref="System.AppDomain"/>'s <see cref="System.AppDomainSetup"/>.
        /// </summary>
        /// <returns>A new instance of AppDomainTypeFactory using the current <see cref="System.AppDomain"/>'s <see cref="System.AppDomainSetup"/>.</returns>
        public static AppDomainTypeFactory Create()
        {
            return Create(System.AppDomain.CurrentDomain.SetupInformation);
        }
        // Private Static Methods
        private static System.AppDomain GetNewAppDomain(System.AppDomainSetup setupInfo)
        {
            var delta = "";
            foreach (var byt in Common.ByteArrayHelper.ConvertStringToByteArray("646F64534F4E", Common.ByteArrayHelper.ConversionBase.Hexadecimal))
            {
                delta += (char)byt;
            }
            var parts = Guid.NewGuid().ToString("d").Split('-');
            var appDomain = System.AppDomain.CreateDomain(parts[0] + parts[1] + parts[2] + delta + parts[4].Substring(0, parts[4].Length - 2),
                                                          null,
                                                          setupInfo);
            appDomain.AssemblyResolve += new ResolveEventHandler(AppDomain_AssemblyResolve);
            return appDomain;
        }
        private static System.Reflection.Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(args.Name))
                {
                    // get assembly's simple name
                    var parts = args.Name.Split(new char[] { ',' });
                    if ((parts != null) && (parts.Length > 0))
                    {
                        // NOTE: this may be redundant; but I'm leaving it...
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
                        // search BaseDirectory
                        {
                            var filename = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, parts[0]);
                            if (!filename.EndsWith(_AssumedAssemblyFilenameExtension_))
                            {
                                filename = filename + _AssumedAssemblyFilenameExtension_;
                            }
                            if (System.IO.File.Exists(filename))
                            {
                                // KNOWN ISSUE: LoadFrom(...) will not properly load the same extensions from different locations.
                                //          It will always load the first extension loaded...
                                return System.Reflection.Assembly.LoadFrom(filename);
                            }
                        }
                        // NOTE: technically this property should contain only subdirectories relative to the BaseDirectory
                        //       I don't, necessarily, use it that way...
                        // search each directory in PrivateBinPath
                        if (!string.IsNullOrWhiteSpace(System.AppDomain.CurrentDomain.SetupInformation.PrivateBinPath))
                        {
                            foreach (var path in System.AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(new char[] { ';' }))
                            {
                                var filename = System.IO.Path.Combine(path, parts[0]);
                                if (!filename.EndsWith(_AssumedAssemblyFilenameExtension_))
                                {
                                    filename = filename + _AssumedAssemblyFilenameExtension_;
                                }
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
            // swallow all exceptions and return null; null indicates an unresolved assembly. (an exception will/should be thrown)
            catch (Exception) { }
            return null;
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of AppDomainTypeFactory.
        /// <note type="note">
        /// Do not use this constructor. It is required if it to be instantiated in a remote <see cref="System.AppDomain"/>.
        /// </note>
        /// </summary>
        public AppDomainTypeFactory() : base() { }
        #endregion
        #region Public Methods
        /// <summary>
        /// Instantiates a <see cref="Type"/> by looking in the <paramref name="assemblyFilename"/> for the type <paramref name="typeName"/> and returns the instance cast as type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to cast Instantiated <see cref="Type"/> to.</typeparam>
        /// <param name="typeName">The name of the <see cref="Type"/> to instantiate.</param>
        /// <param name="assemblyFilename">The filename of the assembly where the <paramref name="typeName"/> is located.</param>
        /// <returns>The instantiated <see cref="Type"/>.</returns>
        /// <seealso cref="System.MarshalByRefObject"/>
        public T CreateInstance<T>(string typeName,
                                   string assemblyFilename)
        {
            return (T)Activator.CreateInstance(assemblyFilename, typeName).Unwrap();
        }
        /// <summary>
        /// Instantiates the type <typeparamref name="T"/> in the remote <see cref="System.AppDomain"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to instantiate.</typeparam>
        /// <returns>The instantiated <see cref="Type"/>.</returns>
        /// <seealso cref="System.MarshalByRefObject"/>
        public T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance<T>();
        }
        /// <summary>
        /// Load the specified assembly into the remote <see cref="System.AppDomain"/>.
        /// </summary>
        /// <param name="assemblyFilename"></param>
        public void LoadAssembly(string assemblyFilename)
        {
            System.Reflection.Assembly.LoadFrom(assemblyFilename);
        }
        /// <summary>
        /// Loads the specified assemblies into the remote <see cref="System.AppDomain"/>. 
        /// </summary>
        /// <param name="assemblyFilenames"></param>
        public void LoadAssemblies(IEnumerable<string> assemblyFilenames)
        {
            foreach (var filename in assemblyFilenames)
            {
                System.Reflection.Assembly.LoadFrom(filename);
            }
        }
        /// <summary>
        /// Gets the remote <see cref="System.AppDomain"/>.
        /// </summary>
        public System.AppDomain AppDomain => System.AppDomain.CurrentDomain;
        #endregion
        #region ITypeProxySponsorHelper Methods
        /// <summary>
        /// Returns <b>true</b>; the logic here is, this <see cref="Type"/> will exist for the entire lifetime of this <see cref="System.AppDomain"/>; therefore, it is always loaded.
        /// </summary>
        public bool IsLoaded => true;
    }
    #endregion
}

