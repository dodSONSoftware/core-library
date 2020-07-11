using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Represents an <see cref="Func{T, TResult}"/> executed in a separate <see cref="AppDomain"/>.
    /// </summary>
    /// <typeparam name="T">The type of the value injected into this AppDomain.</typeparam>
    /// <typeparam name="TResult">The type of the result produced by this AppDomain.</typeparam>
    /// <example>
    /// The following code example will create an AppDomain and give it a function, with a single boolean input parameter, to execute in a separate application domain.
    /// <br/>
    /// First, it will display the AppDomain's properties, execute the function inputing a <b>false</b>, to not generating an <see cref="Exception"/>, display the AppDomain's properties again, 
    /// execute the function again, inputing a <b>true</b>, to generate an <see cref="System.Exception"/>, and then finally it will display the AppDomain's properties one final time.
    /// <br/>
    /// If the input value is <b>true</b>, then an <see cref="System.Exception"/> will be generated, terminating the function; otherwise, if the input value is <b>false</b>, 
    /// the function will complete normally and return a string and no <see cref="System.Exception"/> will be generated.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// //  create an AppDomain to execute a function in a separate application domain
    /// var appDomain = new dodSON.Core.AppDomains.AppDomain&lt;bool, string&gt;(x =&gt;
    /// {
    ///     var stWatch = System.Diagnostics.Stopwatch.StartNew();
    ///     Console.WriteLine($"AppDomain Id={System.AppDomain.CurrentDomain.Id}, Thread Id={System.Threading.Thread.CurrentThread.ManagedThreadId}");
    ///     var result = $"Value={x}";
    ///     Console.WriteLine(result);
    /// 
    ///     // generate exception if input value is TRUE
    ///     if (x) { throw new Exception("Fabricated Exception"); }
    ///     
    ///     // wait a bit...
    ///     dodSON.Core.Threading.ThreadingHelper.Sleep(3000);
    ///     
    ///     // display run time
    ///     stWatch.Stop();
    ///     Console.WriteLine($"Runtime={dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(stWatch.Elapsed)}");
    /// 
    ///     // return results
    ///     return result;
    /// });
    /// 
    /// Console.WriteLine($"Executing Function without Exception");
    /// Console.WriteLine($"----------------------------------------{Environment.NewLine}");
    /// 
    /// // check the AppDomain's properties
    /// Console.WriteLine($"IsCompleted = {appDomain.IsCompleted}");
    /// Console.WriteLine($"Result      = {appDomain.Result ?? "NULL"}");
    /// Console.WriteLine($"IsFaulted   = {appDomain.IsFaulted}");
    /// var ex1 = appDomain.Exception != null ? appDomain.Exception.Message : "NULL";
    /// Console.WriteLine($"Exception   = {ex1}{Environment.NewLine}");
    /// 
    /// // execute the function
    /// var runDude = appDomain.Run(false);
    /// 
    /// // check the AppDomain's properties
    /// Console.WriteLine($"{Environment.NewLine}Same Reference = {runDude == appDomain}");
    /// Console.WriteLine($"IsCompleted    = {runDude.IsCompleted}");
    /// Console.WriteLine($"Result         = {appDomain.Result ?? "NULL"}");
    /// Console.WriteLine($"IsFaulted      = {runDude.IsFaulted}");
    /// var ex2 = runDude.Exception != null ? runDude.Exception.Message : "NULL";
    /// Console.WriteLine($"Exception      = {ex2}");
    /// 
    /// Console.WriteLine($"{Environment.NewLine}Executing Function with Exception");
    /// Console.WriteLine($"----------------------------------------{Environment.NewLine}");
    /// 
    /// // execute the function again
    /// var runDude2 = appDomain.Run(true);
    /// 
    /// // check the AppDomain's properties
    /// Console.WriteLine($"{Environment.NewLine}Same Reference = {runDude2 == appDomain}");
    /// Console.WriteLine($"IsCompleted    = {runDude2.IsCompleted}");
    /// Console.WriteLine($"Result         = {appDomain.Result ?? "NULL"}");
    /// Console.WriteLine($"IsFaulted      = {runDude2.IsFaulted}");
    /// var ex3 = runDude2.Exception != null ? runDude2.Exception.Message : "NULL";
    /// Console.WriteLine($"Exception      = {ex3}");
    /// 
    /// // #### end-of-code
    /// Console.WriteLine($"{Environment.NewLine}================================");
    /// Console.Write("press any key&gt;");
    /// Console.ReadKey(true);
    ///
    /// 
    /// 
    /// // This code produces output similar to the following:
    ///
    /// Executing Function without Exception
    /// ----------------------------------------
    /// 
    /// IsCompleted = False
    /// Result      = NULL
    /// IsFaulted   = False
    /// Exception   = NULL
    /// 
    /// AppDomain Id=2, Thread Id=1
    /// Value=False
    /// Runtime=3.001 seconds
    /// 
    /// Same Reference = True
    /// IsCompleted    = True
    /// Result         = Value=False
    /// IsFaulted      = False
    /// Exception      = NULL
    /// 
    /// Executing Function with Exception
    /// ----------------------------------------
    /// 
    /// AppDomain Id=3, Thread Id=1
    /// Value=True
    /// 
    /// Same Reference = True
    /// IsCompleted    = True
    /// Result         = NULL
    /// IsFaulted      = True
    /// Exception      = Fabricated Exception
    /// 
    /// ================================
    /// press any key&gt;
    /// </code>
    /// </example>
    [Serializable()]
    public class AppDomain<T, TResult>
    {
        #region Ctor
        private AppDomain() { }
        /// <summary>
        /// Instantiates an AppDomain with the given <see cref="Func{T, TResult}"/>.
        /// </summary>
        /// <param name="func">The <see cref="Func{T, TResult}"/> to execute in a separate <see cref="AppDomain"/>.</param>
        public AppDomain(Func<T, TResult> func) : this() => _Func = func ?? throw new ArgumentNullException(nameof(func));
        /// <summary>
        /// Instantiates an AppDomain with the given <see cref="Func{T, TResult}"/> and set of directory paths to probe for assemblies.
        /// </summary>
        /// <param name="func">The <see cref="Func{T, TResult}"/> to execute in a separate <see cref="AppDomain"/>.</param>
        /// <param name="privateBinPaths">The directory paths to probe for assemblies.</param>
        public AppDomain(Func<T, TResult> func, string[] privateBinPaths) : this(func)
        {
            if ((privateBinPaths == null) || (privateBinPaths.Length == 0))
            {
                _PrivateBinPaths = "";
            }
            else
            {
                var result = new StringBuilder();
                foreach (var item in privateBinPaths) { result.Append(item + ";"); }
                --result.Length;
                _PrivateBinPaths = result.ToString();
            }
        }
        #endregion
        #region Private Fields
        private readonly Func<T, TResult> _Func;
        private readonly string _PrivateBinPaths;
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets a value that indicates whether the <see cref="Func{T, TResult}"/> has completed.
        /// </summary>
        public bool IsCompleted { get; private set; }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="Func{T, TResult}"/> has generated an <see cref="System.Exception"/>.
        /// </summary>
        public bool IsFaulted { get; private set; }
        /// <summary>
        /// If <see cref="IsFaulted"/> is <b>true</b> this will contain the <see cref="System.Exception"/> generated, or <b>null</b>.
        /// </summary>
        public Exception Exception { get; private set; }
        /// <summary>
        /// Gets the value returned by the <see cref="Func{T, TResult}"/>.
        /// </summary>
        public TResult Result { get; private set; }
        #endregion
        #region Public Methods
        /// <summary>
        /// Will execute the <see cref="Func{T, TResult}"/> in a separate <see cref="AppDomain"/>.
        /// </summary>
        /// <returns>An AppDomain that represents the work executed.</returns>
        /// <param name="value">The value to pass into the <see cref="Func{T, TResult}"/>.</param>
        public AppDomain<T, TResult> Run(T value)
        {
            IsCompleted = false;
            IsFaulted = false;
            Exception = null;
            var privateBinPath = _PrivateBinPaths;
            object state = value;
            Func<object, object> func = new Func<object, object>(x =>
            {
                return _Func.Invoke((T)x);
            });
            Result = (TResult)AppDomainHelper.Execute(privateBinPath, state, func, out Exception AppDomainRunException);
            if (AppDomainRunException != null)
            {
                IsFaulted = true;
                Exception = AppDomainRunException;
            }
            IsCompleted = true;
            return this;
        }
        #endregion
    }
}
