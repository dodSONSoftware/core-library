using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// Provides an argument generator for the set of given arguments.
    /// </summary>
    /// <example>
    /// The following code example will read a line from the console and process each argument.
    /// <br/><br/>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// // display basic instructions
    /// Console.WriteLine("Enter an empty line to exit.");
    /// Console.WriteLine("Enter the sub-command 'stop' to convert the remainder of the generator into a string.");
    /// Console.WriteLine();
    /// 
    /// // loop it
    /// while (true)
    /// {
    ///     // display user input ready indicator
    ///     Console.Write("$");
    ///     
    ///     // read line from console
    ///     string line = Console.ReadLine();
    /// 
    ///     // terminate on empty line
    ///     if (string.IsNullOrWhiteSpace(line)) { break; }
    /// 
    ///     // convert line to arguments
    ///     IList&lt;string&gt; args = dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.ConvertStringToArgs(line);
    /// 
    ///     // process arguments
    ///     dodSON.Core.CommandLineInterface.ArgsGenerator generator = new dodSON.Core.CommandLineInterface.ArgsGenerator(args);
    ///     int index = 0;
    ///     foreach (string arg in generator)
    ///     {
    ///         // check for sub-command 'stop'
    ///         if (arg.ToLower() == "stop")
    ///         {
    ///             // convert the remainder of the arguments in the generator into a string
    ///             string genStr = dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.ConvertArgsGeneratorToString(generator);
    ///             
    ///             // out the converted string to the console
    ///             Console.WriteLine($"\tSTOP:{genStr}");
    ///         }
    ///         else
    ///         {
    ///             // output the argument
    ///             Console.WriteLine($"\t{++index}:{arg}");
    ///         }
    ///     }
    ///     // blank line
    ///     Console.WriteLine();
    /// }
    /// // display termination message
    /// Console.WriteLine();
    /// Console.WriteLine("CLI terminated...");
    /// Console.ReadKey();
    /// 
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // Enter an empty line to exit.
    /// // Enter the sub-command 'stop' to convert the remainder of the generator into a string.
    /// // 
    /// // $1 2 3 4 5 6 7 8 9 0
    /// //         1:1
    /// //         2:2
    /// //         3:3
    /// //         4:4
    /// //         5:5
    /// //         6:6
    /// //         7:7
    /// //         8:8
    /// //         9:9
    /// //         10:0
    /// // 
    /// // $1 2 3 4 5 stop 6 7 8 9 0
    /// //         1:1
    /// //         2:2
    /// //         3:3
    /// //         4:4
    /// //         5:5
    /// //         STOP:6 7 8 9 0
    /// // 
    /// // $Alpha Beta Gamma stop Delta Epsilon
    /// //         1:Alpha
    /// //         2:Beta
    /// //         3:Gamma
    /// //         STOP:Delta Epsilon
    /// // 
    /// // $
    /// // 
    /// // CLI terminated...
    /// </code>
    /// </example>
    public class ArgsGenerator
        : IEnumerable<string>, IEnumerator<string>
    {
        #region Ctor
        private ArgsGenerator() { }
        /// <summary>
        /// Instantiates a new <see cref="ArgsGenerator"/> with the given arguments.
        /// </summary>
        /// <param name="args"></param>
        public ArgsGenerator(IList<string> args) : this()
        {
            if (args != null) { _Args = args; }
        }
        #endregion
        #region Private Fields
        private bool _HasReset = false;
        private int _Index = -1;
        private readonly IList<string> _Args = new string[0];
        #endregion
        #region IEnumerable<string> Methods
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public string Current { get; private set; } = default;
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Advances the enumerator to the next element in the collection.
        /// </summary>
        /// <returns>The next element in the collection.</returns>
        public bool MoveNext()
        {
            if (_HasReset) { return false; }
            //
            if (++_Index >= _Args.Count)
            {
                Reset();
                _HasReset = true;
                return false;
            }
            else
            {
                Current = _Args[_Index];
                return true;
            }
        }
        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset()
        {
            _HasReset = false;
            _Index = -1;
        }
        #endregion
        #region IEnumerable<string> Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<string> GetEnumerator() { return this; }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return this; }
        #endregion
    }
}
