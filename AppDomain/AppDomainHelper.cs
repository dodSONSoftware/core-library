using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Utilizes a separate <see cref="System.AppDomain"/> to load assemblies and search for types and execute functions.
    /// </summary>
    /// <remarks>
    /// These methods will load any other required assemblies, into a new <see cref="System.AppDomain"/>.
    /// When these methods are finished, the <see cref="System.AppDomain"/> will be unloaded, removing all of the loaded assemblies.
    /// Thereby, leaving the host <see cref="System.AppDomain"/> without a bunch of unnecessary assemblies.
    /// </remarks>
    public static class AppDomainHelper
    {
        #region AppDomain
        /// <summary>
        /// Will attempt to load the <paramref name="assemblyFilename"/> into a new <see cref="System.AppDomain"/>, search for the specified <paramref name="typeName"/> and execute the <paramref name="func"/> passing the discovered <paramref name="typeName"/>.
        /// </summary>
        /// <remarks>
        /// This method will load the <paramref name="typeName"/>'s <see cref="Type"/>, and any other required assemblies, into a new <see cref="System.AppDomain"/>.
        /// When this method is finished, the <see cref="System.AppDomain"/> will be unloaded, removing all of the loaded assemblies.
        /// </remarks>
        /// <param name="assemblyFilename">The assembly file to load and search.</param>
        /// <param name="typeName">The name of the type to search for.</param>
        /// <param name="privateBinPath">A list of directories that are probed for private assemblies. Each directory should be separated by semicolon [;].</param>
        /// <param name="state">An object passed into the <paramref name="func"/>.</param>
        /// <param name="func">The function to execute when the type is discovered.</param>
        /// <param name="exception">If any errors occur, a <b>null</b> will be returned and this exception will be populated.</param>
        /// <returns>If the type was found, the results of the <paramref name="func"/> are returned; otherwise <b>null</b> is returned along with a <b>null</b> exception.</returns>
        /// <example>
        /// The following code example will use the <see cref="dodSON.Core.AppDomain.AppDomainHelper.QueryType(string, string, string, object, Func{object, Type, object}, out Exception)"/> function to search for a type not found in it's assembly.
        /// The function, running in a separate application domain, will load several assemblies. These assemblies can be seen in the second list.
        /// The third list shows that those assemblies were not loaded into the main application domain.
        /// <para><br/>Place the different assemblies into different folders.</para>
        /// <para>Application Folder: Main_Executable, The InterfaceLibrary</para>
        /// <para>Code Folder: The TypeLibrary</para>
        /// <para>CustomCode Folder: The CustomCodeLibrary</para>
        /// <para><br/>Create a console application and add the following code:</para>
        /// <code>
        /// static void Main(string[] args)
        /// {
        ///     // location of assembly to search
        ///     var TypeLibraryAssemblyFilename = @"C:\(WORKING)\Dev\AppDomainHelper\Code\TypeLibrary.dll";   // ######## BE SURE TO CHANGE THIS ########
        ///     //
        ///     DisplayLoadedAssemblies();
        ///     // location of assembly to search
        ///     var assemblyFilename = TypeLibraryAssemblyFilename;
        ///     // type to search for
        ///     var typeName = "TypeLibrary.Alpha";
        ///     // any paths that contain required assemblies
        ///     var privateBinPath = @"C:\(WORKING)\Dev\AppDomainHelper\CustomCode";   // ######## BE SURE TO CHANGE THIS ########
        ///     // an object that can be passed into the function
        ///     var state = Tuple.Create("AlphaId", "AlphaName");
        ///     // the function to execute in the separate application domain
        ///     var func = new Func&lt;object, Type, object&gt;(
        ///                (state_, type_) =&gt;
        ///                {
        ///                    Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, Discovered Type={type_.AssemblyQualifiedName}");
        ///                    // create instance of discovered type
        ///                    var parameterTypes = new Type[] { typeof(string), typeof(string) };
        ///                    var st = (Tuple&lt;string, string&gt;)state_;
        ///                    var parameters = new object[] { st.Item1, st.Item2 };
        ///                    var typeInstance = (InterfaceLibrary.IAlpha)dodSON.Core.Common.InstantiationHelper.InvokeCtor(type_, parameterTypes, parameters);
        ///                    // execute instance method
        ///                    var returnMessage = typeInstance.Hello($"Message From Application Domain #{System.AppDomain.CurrentDomain.Id}");
        ///                    //
        ///                    DisplayLoadedAssemblies();
        ///                    return returnMessage;
        ///                });
        ///     // execute the function in a separate application domain
        ///     var result = dodSON.Core.Common.AppDomainHelper.QueryType(assemblyFilename,
        ///                                                               typeName,
        ///                                                               privateBinPath,
        ///                                                               state,
        ///                                                               func,
        ///                                                               out Exception queryTypeException);
        ///     // report results
        ///     Console.WriteLine($"Reporting results in Application Domain #{System.AppDomain.CurrentDomain.Id}");
        ///     if (queryTypeException != null)
        ///     {
        ///         Console.WriteLine($"Exception = {queryTypeException.InnerException.Message}");
        ///     }
        ///     else
        ///     {
        ///         Console.WriteLine($"Results = \"{result}\"");
        ///     }
        ///     // 
        ///     DisplayLoadedAssemblies();
        ///     // 
        ///     Console.WriteLine("press anykey&gt;");
        ///     Console.ReadKey(true);
        /// }
        /// 
        /// private static void DisplayLoadedAssemblies()
        /// {
        ///     Console.WriteLine();
        ///     Console.WriteLine("--------------------------------");
        ///     Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}");
        ///     var count = 0;
        ///     foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        ///     {
        ///         Console.WriteLine($"#{++count} = {assembly.FullName}, {assembly.Location}");
        ///     }
        ///     Console.WriteLine("--------------------------------");
        ///     Console.WriteLine();
        /// }
        /// </code>    
        /// <para><br/>The final screen shot.</para>
        /// <code>
        /// // --------------------------------
        /// // Application Domain #1
        /// // #1 = mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll
        /// // #2 = ConsoleApp13, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\ConsoleApp13.exe
        /// // #3 = dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\dodSON.Core.dll
        /// // --------------------------------
        /// // 
        /// // Application Domain #2, Discovered Type=TypeLibrary.Alpha, TypeLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
        /// // 
        /// // --------------------------------
        /// // Application Domain #2
        /// // #1 = mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll
        /// // #2 = dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\dodSON.Core.dll
        /// // #3 = ConsoleApp13, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\ConsoleApp13.exe
        /// // #4 = TypeLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, C:\(WORKING)\Dev\AppDomainHelper\Code\TypeLibrary.dll
        /// // #5 = InterfaceLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\InterfaceLibrary.dll
        /// // #6 = CustomCodeLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, C:\(WORKING)\Dev\AppDomainHelper\CustomCode\CustomCodeLibrary.dll
        /// // --------------------------------
        /// // 
        /// // Reporting results in Application Domain #1
        /// // Results = "Hello from inside AlphaName in Application Domain #2. (Id=AlphaId), Message="Message From Application Domain #2""
        /// // 
        /// // --------------------------------
        /// // Application Domain #1
        /// // #1 = mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll
        /// // #2 = ConsoleApp13, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\ConsoleApp13.exe
        /// // #3 = dodSON.Core, Version=1.1.0.0, Culture=neutral, PublicKeyToken=null, c:\users\user\documents\visual studio 2017\Projects\ConsoleApp13\ConsoleApp13\bin\Debug\dodSON.Core.dll
        /// // #4 = System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System.Core\v4.0_4.0.0.0__b77a5c561934e089\System.Core.dll
        /// // #5 = System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089, C:\WINDOWS\Microsoft.Net\assembly\GAC_MSIL\System\v4.0_4.0.0.0__b77a5c561934e089\System.dll
        /// // --------------------------------
        /// // 
        /// // press anykey&gt;
        /// </code>      
        /// <para><br/><b>The InterfaceLibrary</b></para>
        /// <para>Create a console application and add the following code:</para>
        /// <code>
        /// public interface IAlpha
        /// {
        ///     string Id { get; }
        ///     string Name { get; }
        ///     string Hello(string message);
        /// }
        /// </code>  
        /// <para><b>The TypeLibrary</b></para>
        /// <para>Create a console application and add the following code:</para>
        /// <code>
        /// public class Alpha
        ///       : InterfaceLibrary.IAlpha
        /// {
        ///     #region Ctor
        ///     private Alpha() { }
        ///
        ///     public Alpha(string id, string name) : this()
        ///     {
        ///         _CustomClass = new CustomCodeLibrary.CustomClass(id);
        ///         Name = name;
        ///     }
        ///     #endregion
        ///     #region Private Fields
        ///     private CustomCodeLibrary.CustomClass _CustomClass = null;
        ///     #endregion
        ///     #region IAlpha Methods
        ///
        ///     public string Id { get { return _CustomClass.Id; } }
        ///
        ///     public string Name { get; }
        ///
        ///     public string Hello(string message)
        ///     {
        ///         return $"Hello from inside {Name} in Application Domain #{System.AppDomain.CurrentDomain.Id}. (Id={Id}), Message=\"{message}\"";
        ///     }
        ///     #endregion
        /// }
        /// </code>    
        /// <para><br/><b>The CustomCodeLibrary</b></para>
        /// <para>Create a console application and add the following code:</para>
        /// <code>
        /// public class CustomClass
        /// {
        ///     #region Ctor
        ///     private CustomClass() { }
        /// 
        ///     public CustomClass(string id) : this()
        ///     {
        ///         Id = id;
        ///     }
        ///     #endregion
        ///     #region Public Methods
        ///     public string Id { get; }
        ///     #endregion
        /// }
        /// </code>
        /// </example>
        public static object QueryType(string assemblyFilename,
                                       string typeName,
                                       string privateBinPath,
                                       object state,
                                       Func<object, Type, object> func,
                                       out Exception exception)
        {
            // checks 
            if (string.IsNullOrWhiteSpace(assemblyFilename)) { throw new ArgumentNullException(nameof(assemblyFilename)); }
            if (string.IsNullOrWhiteSpace(typeName)) { throw new ArgumentNullException(nameof(typeName)); }
            if (func == null) { throw new ArgumentNullException(nameof(func)); }
            //            
            var proxySettings = new dodSON.Core.AppDomain.TypeProxySettings(typeof(DiscoveryAssistant).AssemblyQualifiedName,
                                                                            null,
                                                                            null,
                                                                            System.IO.Path.GetDirectoryName(typeof(DiscoveryAssistant).Assembly.Location),
                                                                            privateBinPath);
            var pluginFactory = new Addon.PluginFactory(proxySettings);
            //
            var plugin = pluginFactory.Load();
            plugin.Start();
            //
            var result = ((DiscoveryAssistant)plugin).FindType(assemblyFilename, typeName, state, func, out exception);
            //
            plugin.Stop();
            pluginFactory.Unload();
            //
            return result;
        }
        /// <summary>
        /// Will create a new <see cref="System.AppDomain"/> and execute the <paramref name="func"/> in it, returning the results.
        /// </summary>
        /// <remarks>
        /// This method will execute the <paramref name="func"/> in a new <see cref="System.AppDomain"/>; any required assemblies will be loaded into that <see cref="System.AppDomain"/>.
        /// When this method is finished, the <see cref="System.AppDomain"/> will be unloaded, removing all of the loaded assemblies.
        /// </remarks>
        /// <param name="privateBinPath">A list of directories that are probed for private assemblies. Each directory should be separated by semicolon [;].</param>
        /// <param name="state">An object passed into the <paramref name="func"/>.</param>
        /// <param name="func">The function to execute in the separate <see cref="System.AppDomain"/>.</param>
        /// <param name="exception">If any errors occur, a <b>null</b> will be returned and this exception will be populated.</param>
        /// <returns>The results of the <paramref name="func"/>.</returns>
        /// <example>
        /// The following code example will use the <see cref="dodSON.Core.AppDomain.AppDomainHelper.Execute(string, object, Func{object, object}, out Exception)"/> function to execute code in a separate application domain.
        /// The function, running in a separate application domain, will receive a <b>state</b>; this <b>state</b> will be modified to demonstrate passing and mutating <b>state</b>.
        /// In order for a <b>state</b> to be mutable, it must inherit from <see cref="MarshalByRefObject"/>; this will allow it to use proxies to communicate across the application domain boundaries.
        /// <para><br/>Create a console application and add the following code:</para>
        /// <code>
        /// [Serializable]
        /// public class StateClass
        ///     : MarshalByRefObject
        /// {
        ///     #region Ctor
        ///     private StateClass() : base() { }
        /// 
        ///     public StateClass(string id, string name, double value1, long value2, bool value3) : this()
        ///     {
        ///         Id = id;
        ///         Name = name;
        ///         Value1 = value1;
        ///         Value2 = value2;
        ///         Value3 = value3;
        ///     }
        ///     #endregion
        ///     #region Public Methods
        ///     public string Id { get; set; }
        ///     public string Name { get; set; }
        ///     public double Value1 { get; set; }
        ///     public long Value2 { get; set; }
        ///     public bool Value3 { get; set; }
        ///     #endregion
        /// }
        /// 
        /// static void Main(string[] args)
        /// {
        ///     // an object that can be passed into, and out of, the function
        ///     var state = new StateClass("ID", "NAME", 3.141, 14, false);
        ///     Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, State = [{state.Id}, {state.Name}, {state.Value1}, {state.Value2}, {state.Value3}]");
        /// 
        ///     // the function to execute in the separate application domain
        ///     var privateBinPath = "";
        ///     var func = new Func&lt;object, object&gt;(
        ///                (state_) =&gt;
        ///                {
        ///                    // display state
        ///                    var stateRef = (StateClass)state_;
        ///                    Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, State = [{stateRef.Id}, {stateRef.Name}, {stateRef.Value1}, {stateRef.Value2}, {stateRef.Value3}]");
        ///                    // change state
        ///                    stateRef.Id = "SimpleId";
        ///                    stateRef.Name = $"SimpleName";
        ///                    stateRef.Value1 = Math.E;
        ///                    stateRef.Value2 = dodSON.Core.Common.ByteCountHelper.FromKilobytes(1);
        ///                    stateRef.Value3 = true;
        ///                    // display state
        ///                    Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, State = [{stateRef.Id}, {stateRef.Name}, {stateRef.Value1}, {stateRef.Value2}, {stateRef.Value3}]");
        ///                    // return greetings
        ///                    return $"Hello from Application Domain #{System.AppDomain.CurrentDomain.Id}";
        ///                });
        ///     // execute function in separate application domain
        ///     var funcResult = (string)dodSON.Core.Common.AppDomainHelper.Execute(privateBinPath, state, func, out Exception executeException);
        ///     // display results
        ///     Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, Function Results = {funcResult}");
        ///     Console.WriteLine($"Application Domain #{AppDomain.CurrentDomain.Id}, State = [{state.Id}, {state.Name}, {state.Value1}, {state.Value2}, {state.Value3}]");
        ///     // 
        ///     Console.WriteLine("press anykey&gt;");
        ///     Console.ReadKey(true);
        /// }
        /// </code>    
        /// <para><br/>The final screen shot.</para>
        /// <code>
        /// // Application Domain #1, State = [ID, NAME, 3.141, 14, False]
        /// // 
        /// // Application Domain #2, State = [ID, NAME, 3.141, 14, False]
        /// // Application Domain #2, State = [SimpleId, SimpleName, 2.71828182845905, 1024, True]
        /// // 
        /// // Application Domain #1, Function Results = Hello from Application Domain #2
        /// // Application Domain #1, State = [SimpleId, SimpleName, 2.71828182845905, 1024, True]
        /// // press anykey>
        /// </code>      
        /// </example>
        public static object Execute(string privateBinPath, object state, Func<object, object> func, out Exception exception)
        {
            // checks
            if (func == null) { throw new ArgumentNullException(nameof(func)); }
            //
            var proxySettings = new dodSON.Core.AppDomain.TypeProxySettings(typeof(DiscoveryAssistant).FullName,
                                                                            null,
                                                                            null,
                                                                            System.IO.Path.GetDirectoryName(typeof(DiscoveryAssistant).Assembly.Location),
                                                                            privateBinPath);
            var pluginFactory = new Addon.PluginFactory(proxySettings);
            //
            var plugin = pluginFactory.Load();
            plugin.Start();
            //
            var result = ((DiscoveryAssistant)plugin).Execute(state, func, out exception);
            //
            plugin.Stop();
            pluginFactory.Unload();
            //
            return result;
        }
        #endregion
    }
    #region Class: internal class DiscoveryAssistant
    /// <summary>
    /// Facilitates executions of methods in a separate <see cref="System.AppDomain"/>.
    /// </summary>
    internal class DiscoveryAssistant
        : Addon.PluginBase
    {
        #region Ctor
        public DiscoveryAssistant() : base() { }
        #endregion
        #region Addon.PluginBase Methods
        protected override void OnStart() { }
        protected override void OnStop() { }
        #endregion
        #region Public Methods
        public object FindType(string assemblyFilename, string typeName, object state, Func<object, Type, object> func, out Exception exception)
        {
            exception = null;
            try
            {
                var dude = System.Reflection.Assembly.LoadFile(assemblyFilename).GetType(typeName);
                if (dude != null)
                {
                    return func(state, dude);
                }
                return null;
            }
            catch (Exception ex)
            {
                exception = ex;
                return null;
            }
        }
        public object Execute(object state, Func<object, object> func, out Exception exception)
        {
            exception = null;
            try { return func(state); }
            catch (Exception ex) { exception = ex; }
            return null;
        }
        #endregion
    }
    #endregion
}
