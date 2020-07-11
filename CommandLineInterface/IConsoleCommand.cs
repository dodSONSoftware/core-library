using System;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// Defines functionality to be a Command Line Interface command. 
    /// </summary>
    public interface IConsoleCommand
    {
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        string Help { get; }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        void Execute(IList<string> args);
    }
}
