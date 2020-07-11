using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// Common methods used throughout the dodSON.Core.CommandLineInterface namespace.
    /// </summary>
    public static class CommandLineInterfaceHelper
    {
        #region Public Properties
        /// <summary>
        /// The string used to indicate the system is ready for user input.
        /// </summary>
        public static string CommandLinePrompt
        {
            get => _CommandLinePrompt;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) { value = ">>>"; }
                _CommandLinePrompt = value;
            }
        }
        #endregion
        #region Private Fields
        private static string _CommandLinePrompt = ">>>";
        private static readonly char[] _ArgumnentTags = new char[] { '-', '/', '\\' };
        #endregion
        #region UI Elements
        /// <summary>
        /// Creates a standard UI separator.
        /// </summary>
        /// <returns>A string which creates a separation between sections of a console output stream.</returns>
        public static string SeparatorLine() => new string(CommandLineInterfaceShared.SeparatorCharacter, CommandLineInterfaceShared.SeparatorLineLength);
        /// <summary>
        /// Creates a standard UI separator with the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to place in the middle of the separator line.</param>
        /// <returns>A string, containing the <paramref name="text"/>, which creates a separation between sections of a console output stream.</returns>
        public static string SeparatorLine(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) { return SeparatorLine(); }
            //
            var subLength = (CommandLineInterfaceShared.SeparatorLineLength / 2) - (text.Length / 2);
            var linePiece = new string(CommandLineInterfaceShared.SeparatorCharacter, subLength);
            return (linePiece + text + linePiece).Substring(0, CommandLineInterfaceShared.SeparatorLineLength);
        }
        /// <summary>
        /// Displays a standard error message.
        /// </summary>
        /// <param name="message">The error message to display.</param>
        public static void DisplayError(string message) => Console.WriteLine($"ERROR: {message}");
        /// <summary>
        /// Formats a <see cref="MessagePumpCommand"/> into a standard help line.
        /// </summary>
        /// <param name="item">The <see cref="MessagePumpCommand"/> to create a help line for.</param>
        /// <returns>A <see cref="MessagePumpCommand"/> formatted into a standard help line.</returns>
        public static string FormatHelp(MessagePumpCommand item)
        {
            var result = new StringBuilder();
            //
            var aliases = new StringBuilder();
            foreach (var alias in item.Aliases) { aliases.Append(alias + ","); }
            if (aliases.Length > 0)
            {
                --aliases.Length;
                var space = new string(' ', CommandLineInterfaceShared.HelpLineLength - item.Command.Length - 3 - aliases.Length);
                result.Append($"{item.Command} [{aliases}]{space}{CommandLineInterfaceHelper.GetCommandObject(item.Command).Description}");
            }
            else
            {
                var space = new string(' ', CommandLineInterfaceShared.HelpLineLength - item.Command.Length);
                result.Append($"{item.Command}{space}{CommandLineInterfaceHelper.GetCommandObject(item.Command).Description}");
            }
            //
            return result.ToString();
        }
        /// <summary>
        /// Formats a <paramref name="command"/> string and a <paramref name="description"/> string into a standard help line.
        /// </summary>
        /// <param name="command">The command to create the help line for.</param>
        /// <param name="description">The description of the command.</param>
        /// <returns>A string formatted into a standard help line using the <paramref name="command"/> and <paramref name="description"/>.</returns>
        public static string FormatHelp(string command, string description) => $"{command}{new string(' ', CommandLineInterfaceShared.HelpLineLength - command.Length)}{description}";
        /// <summary>
        /// Standard header for help screens.
        /// </summary>
        /// <param name="description">A short description of the help topic.</param>
        /// <returns>A <see cref="StringBuilder"/> containing the help header.</returns>
        public static StringBuilder HelpHeader(string description)
        {
            var result = new StringBuilder();
            //
            result.AppendLine();
            result.AppendLine(description);
            result.AppendLine();
            result.AppendLine("USAGE:");
            //
            return result;
        }
        #endregion
        #region Processing 

        // Commands and Arguments

        /// <summary>
        /// Processes a single argument, trimming whitespace, removing preceding argument tags (-,/ and \) and setting to lowercase.
        /// </summary>
        /// <param name="value">The argument to clean.</param>
        /// <returns>The clean argument.</returns>
        public static string CleanArgument(string value)
        {
            value = value.Trim().ToLower();
            while ((value.Length > 0) && _ArgumnentTags.Contains(value[0]))
            {
                value = value.Substring(1);
            }
            return value;
        }
        /// <summary>
        /// Search for and instantiates a <see cref="IConsoleCommand"/> from the command registry.
        /// </summary>
        /// <param name="commandArguments">The command to search for.</param>
        /// <returns>An instance of the <see cref="IConsoleCommand"/> from the command registry that matched the given command.</returns>
        public static IConsoleCommand GetCommandObject(CommandArguments commandArguments) => GetCommandObject(commandArguments.Command);
        /// <summary>
        /// Search for and instantiates a <see cref="IConsoleCommand"/> from the command registry.
        /// </summary>
        /// <param name="command">The command to search for.</param>
        /// <returns>An instance of the <see cref="IConsoleCommand"/> from the command registry that matched the given command.</returns>
        public static IConsoleCommand GetCommandObject(string command)
        {
            // find command in CommandClasses dictionary
            var dude = CommandLineInterfaceShared.Commands.FirstOrDefault(x => x.Key == command).Value;
            // test if found
            if (dude == null)
            {
                // no command found
                DisplayError($"Command not found. For a list of available commands type {CommandLineInterfaceHelper.CommandLinePrompt}commands");
                return null;
            }
            // found it, instantiate it...
            return dodSON.Core.Common.InstantiationHelper.InvokeDefaultCtor(dude.ConsoleCommandType) as IConsoleCommand;
        }

        // Converters

        /// <summary>
        /// Converts a list of arguments to a string.
        /// </summary>
        /// <param name="args">The arguments to convert to a string.</param>
        /// <returns>>A string whose value is the combined arguments.</returns>
        public static string ConvertToString(IList<string> args)
        {
            var result = new StringBuilder();
            foreach (var arg in args) { result.Append(arg + " "); }
            if (result.Length > 0) { --result.Length; }
            return result.ToString();
        }
        /// <summary>
        /// Converts the remaining arguments in a <see cref="ArgsGenerator"/> to a strings.
        /// </summary>
        /// <param name="generator">The <see cref="ArgsGenerator"/> to convert into a string.</param>
        /// <returns>A string containing the remaining arguments from the <paramref name="generator"/>.</returns>
        public static string ConvertToString(ArgsGenerator generator) => ConvertToString(generator.ToList());
        /// <summary>
        /// Converts the command and the arguments in a <see cref="CommandArguments"/> into a string.
        /// </summary>
        /// <param name="commArgs">The <see cref="CommandArguments"/> to convert.</param>
        /// <returns>A string containing the command and the arguments of the <see cref="CommandArguments"/>.</returns>
        public static string ConvertToString(CommandArguments commArgs)
        {
            var dude = new List<string>() { commArgs.Command };
            dude.AddRange(commArgs.Arguments);
            return ConvertToString(dude);
        }

        /// <summary>
        /// Converts a string into a list of arguments.
        /// </summary>
        /// <param name="line">The string to convert.</param>
        /// <returns>A list of arguments converted from the <paramref name="line"/>.</returns>
        public static IList<string> ConvertToArgs(string line) => (from x in line.Trim().Split(' ')
                                                                         where !string.IsNullOrWhiteSpace(x)
                                                                         select x).ToList();
        /// <summary>
        /// Converts the remaining arguments in a <see cref="ArgsGenerator"/> to a list of strings.
        /// </summary>
        /// <param name="generator">The <see cref="ArgsGenerator"/> to convert into a list of strings.</param>
        /// <returns>A list of strings containing the remaining arguments from the <paramref name="generator"/>.</returns>
        public static IList<string> ConvertToArgs(ArgsGenerator generator) => generator.ToList();
        /// <summary>
        /// Converts the command and the arguments in a <see cref="CommandArguments"/> into args.
        /// </summary>
        /// <param name="commArgs">The <see cref="CommandArguments"/> to convert.</param>
        /// <returns>A list containing the command and the arguments of the <see cref="CommandArguments"/>.</returns>
        public static IList<string> ConvertToArgs(CommandArguments commArgs) => ConvertToArgs(ConvertToString(commArgs));
        #endregion
    }
}
