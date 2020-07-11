using System;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will display random numbers
    /// </summary>
    public class RandomNumberCommand
        : dodSON.Core.CommandLineInterface.IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="RandomNumberCommand"/>.
        /// </summary>
        public RandomNumberCommand() { }
        #endregion
        #region Private Fields
        private readonly System.Random _Rnd = new System.Random();
        #endregion
        #region Public Properties
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => "Generates random numbers.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.HelpHeader(Description);
                result.AppendLine(dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.FormatHelp("random", "returns a double random number between 0 and 1."));
                result.AppendLine(dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.FormatHelp("random [min # | #] [max # | #]", "returns a integer random number between (min), inclusive, and (max), exclusive."));
                return result.ToString();
            }
        }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        public void Execute(IList<string> args)
        {
            var min = -1;
            var max = -1;
            using (var stream = new dodSON.Core.CommandLineInterface.ArgsGenerator(args))
            {
                while (stream.MoveNext())
                {
                    if (dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.CleanArgument(stream.Current) == "min")
                    {
                        if (stream.MoveNext())
                        {
                            min = Convert.ToInt32(stream.Current);
                        }
                    }
                    else if (dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.CleanArgument(stream.Current) == "max")
                    {
                        if (stream.MoveNext())
                        {
                            max = Convert.ToInt32(stream.Current);
                        }
                    }
                    else
                    {
                        var dude = Convert.ToInt32(stream.Current);
                        if (min == -1) { min = dude; }
                        else { max = dude; }
                    }
                }
            }
            //
            double value = 0;
            if ((min == -1) && (max == -1))
            {
                // no arguments
                value = _Rnd.NextDouble();
            }
            else
            {
                // check it arguments not found
                if ((min == -1) || (max == -1)) { throw new Exception("Missing argument. Must have 2 arguments; minimum and maximum values."); }
                // arguments found
                value = _Rnd.Next(min, max);
            }
            // output the results
            Console.WriteLine(value);
        }
        #endregion
    }
}
