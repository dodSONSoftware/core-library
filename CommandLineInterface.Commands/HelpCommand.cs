using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will display detailed help information about the Command Line Interface.
    /// </summary>
    public class HelpCommand
         : IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="HelpCommand"/>.
        /// </summary>
        public HelpCommand() { }
        #endregion
        #region IConsoleCommand Methods
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => "Displays help information.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = new StringBuilder();
                //
                result.AppendLine(CommandLineInterfaceHelper.SeparatorLine(" ABOUT "));
                result.AppendLine();
                result.AppendLine(CommandLineInterfaceShared.UIHeader);
                // 
                result.AppendLine(CommandLineInterfaceHelper.SeparatorLine(" HELP "));
                result.AppendLine();
                result.AppendLine($"USAGE: [help] COMMAND [ARGUMENTS]");
                result.AppendLine();
                result.AppendLine($"{CommandLineInterfaceHelper.CommandLinePrompt}COMMAND [ARGUMENTS]  \t\tTo execute a specific command.");
                result.AppendLine($"{CommandLineInterfaceHelper.CommandLinePrompt}help                 \t\tTo see this help screen.");
                result.AppendLine($"{CommandLineInterfaceHelper.CommandLinePrompt}help COMMAND         \t\tTo see the help screen for a specific command.");
                result.AppendLine();
                result.AppendLine();
                result.AppendLine($"All [ARGUMENTS] must be separated by spaces; this includes key/value pairs and lists.");
                result.AppendLine($"Examples:");
                result.AppendLine($"{CommandLineInterfaceHelper.CommandLinePrompt}random min 0 max 100");
                result.AppendLine($"\tThis will set the random command's min and max arguments to 0 and 100, respectfully.");
                result.AppendLine();
                result.AppendLine($"{CommandLineInterfaceHelper.CommandLinePrompt}calc add 10 5 8 14 32 26 19");
                result.AppendLine($"\tThis will add the list of numbers together.");
                result.AppendLine();
                //
                result.AppendLine(CommandLineInterfaceHelper.SeparatorLine(" COMMANDS "));
                result.AppendLine();
                result.AppendLine(CommandLineInterfaceShared.DisplayCommands(false));
                result.AppendLine();
                result.AppendLine(CommandLineInterfaceHelper.SeparatorLine());
                // 
                return result.ToString();
            }
        }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        public void Execute(IList<string> args)
        {
            if (args.Count == 0)
            {
                // empty help request
                Console.WriteLine(Help);
            }
            else
            {
                var commArgs = CommandArguments.Create(args);
                var command = commArgs.Command;
                if (command == "?") { command = "help"; }
                var dude = CommandLineInterfaceHelper.GetCommandObject(command);
                if (dude != null)
                {
                    Console.WriteLine(dude.Help);
                }
                // missing command object error handled in the GetCommandObject function
            }
        }
        #endregion
    }
}
