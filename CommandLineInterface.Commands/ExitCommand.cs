using System;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will exit the Command Line Interface's message pump.
    /// </summary>
    public class ExitCommand
         : IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="ExitCommand"/>.
        /// </summary>
        public ExitCommand() { }
        #endregion
        #region IConsoleCommand Methods
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => $"Terminates the {CommandLineInterfaceShared.AppName}.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = CommandLineInterfaceHelper.HelpHeader(Description);
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("exit", Description));
                // ----------
                return result.ToString();
            }
        }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        public void Execute(IList<string> args) { throw new Exception("The [ExitCommand] has been invoked. This should never happen!"); }
        #endregion
    }
}
