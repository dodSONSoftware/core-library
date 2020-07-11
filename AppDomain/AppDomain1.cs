using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.AppDomain
{
    /// <summary>
    /// Represents an <see cref="Action"/> executed in a separate <see cref="AppDomain"/>.
    /// </summary>
    /// <example>
    /// The following code example will create an AppDomain and give it an action to execute in a separate application domain.
    /// <br/>
    /// First, it will display the AppDomain's properties, execute the action and then display the AppDomain's properties again.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// //  create an AppDomain to execute an action in a separate application domain
    /// var appDomain = new dodSON.Core.AppDomains.AppDomain(() =&gt;
    /// {
    ///     var stWatch = System.Diagnostics.Stopwatch.StartNew();
    ///     Console.WriteLine($"AppDomain Id={System.AppDomain.CurrentDomain.Id}, Thread Id={System.Threading.Thread.CurrentThread.ManagedThreadId}");
    ///     dodSON.Core.Threading.ThreadingHelper.Sleep(3000);
    ///     //
    ///     stWatch.Stop();
    ///     Console.WriteLine($"Runtime={dodSON.Core.Common.DateTimeHelper.FormatTimeSpanVerbose(stWatch.Elapsed)}");
    /// });
    /// 
    /// // check the AppDomain's properties
    /// Console.WriteLine($"IsCompleted = {appDomain.IsCompleted}");
    /// Console.WriteLine($"IsFaulted   = {appDomain.IsFaulted}");
    /// var ex1 = appDomain.Exception != null ? appDomain.Exception.Message : "NULL";
    /// Console.WriteLine($"Exception      = {ex1}{Environment.NewLine}");
    /// 
    /// // execute the action 
    /// var runDude = appDomain.Run();
    /// 
    /// // check the AppDomain's properties
    /// Console.WriteLine($"{Environment.NewLine}Same Reference = {runDude == appDomain}");
    /// Console.WriteLine($"IsCompleted    = {runDude.IsCompleted}");
    /// Console.WriteLine($"IsFaulted      = {runDude.IsFaulted}");
    /// var ex2 = runDude.Exception != null ? runDude.Exception.Message : "NULL";
    /// Console.WriteLine($"Exception      = {ex2}");
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
    /// IsCompleted = False
    /// IsFaulted   = False
    /// Exception      = NULL
    /// 
    /// AppDomain Id=2, Thread Id=1
    /// Runtime=3.001 seconds
    /// 
    /// Same Reference = True
    /// IsCompleted    = True
    /// IsFaulted      = False
    /// Exception      = NULL
    /// 
    /// ================================
    /// press any key&gt;
    /// </code>
    /// </example>
    [Serializable()]
    public class AppDomain
    {
        #region Ctor
        private AppDomain() { }
        /// <summary>
        /// Instantiates an AppDomain with the given <see cref="Action"/>.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute in a separate <see cref="AppDomain"/>.</param>
        public AppDomain(Action action) : this() => _Action = action ?? throw new ArgumentNullException(nameof(action));
        /// <summary>
        /// Instantiates an AppDomain with the given <see cref="Action"/> and set of directory paths to probe for assemblies.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute in a separate <see cref="AppDomain"/>.</param>
        /// <param name="privateBinPaths">The directory paths to probe for assemblies.</param>
        public AppDomain(Action action, string[] privateBinPaths) : this(action)
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
        private readonly Action _Action;
        private readonly string _PrivateBinPaths;
        #endregion
        #region Public Properties
        /// <summary>
        /// Gets a value that indicates whether the <see cref="Action"/> has completed.
        /// </summary>
        public bool IsCompleted { get; private set; }
        /// <summary>
        /// Gets a value that indicates whether the <see cref="Action"/> has generated an <see cref="System.Exception"/>.
        /// </summary>
        public bool IsFaulted { get; private set; }
        /// <summary>
        /// If <see cref="IsFaulted"/> is <b>true</b> this will contain the <see cref="System.Exception"/> generated, or <b>null</b>.
        /// </summary>
        public Exception Exception { get; private set; }
        #endregion
        #region Public Methods
        /// <summary>
        /// Will execute the <see cref="Action"/> in a separate <see cref="AppDomain"/>.
        /// </summary>
        /// <returns>An AppDomain that represents the work executed.</returns>
        public AppDomain Run()
        {
            IsCompleted = false;
            IsFaulted = false;
            Exception = null;
            var privateBinPath = _PrivateBinPaths;
            object state = null;
            Func<object, object> func = new Func<object, object>(x =>
            {
                _Action.Invoke();
                return null;
            });
            AppDomainHelper.Execute(privateBinPath, state, func, out Exception AppDomainRunException);
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
