using System;
using System.Collections.Generic;
using System.Text;

namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will clear the console screen.
    /// </summary>
    public class ClearScreenCommand
         : IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="ClearScreenCommand"/>.
        /// </summary>
        public ClearScreenCommand() { }
        #endregion
        #region IConsoleCommand Methods
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => "Clears the screen.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = CommandLineInterfaceHelper.HelpHeader(Description);
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("cls", Description));
                // ----------
                return result.ToString();
            }
        }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        public void Execute(IList<string> args)
        {
            Console.Clear();
            Console.WriteLine(CommandLineInterfaceShared.UIHeader);
        }
        #endregion
    }
}
