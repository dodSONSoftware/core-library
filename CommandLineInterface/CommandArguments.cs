using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// Represents a command and associated arguments.
    /// </summary>
    public class CommandArguments
    {
        #region Static Methods
        /// <summary>
        /// Creates a new <see cref="CommandArguments"/> with the given <paramref name="args"/>.
        /// </summary>
        /// <param name="args">The arguments to process.</param>
        /// <returns>A <see cref="CommandArguments"/> with a command its and associated arguments.</returns>
        public static CommandArguments Create(IList<string> args) => Create(args, false);
        /// <summary>
        /// Creates a new <see cref="CommandArguments"/> with the given <paramref name="args"/> and <paramref name="translateAliases"/>.
        /// </summary>
        /// <param name="args">The arguments to process.</param>
        /// <param name="translateAliases">
        /// <b>True</b> will check for aliases, if found, the command will be changed to the aliased command.
        /// <b>False</b> will clean the command, but not alter it.
        /// </param>
        /// <returns>A <see cref="CommandArguments"/> with a command its and associated arguments.</returns>
        public static CommandArguments Create(IList<string> args, bool translateAliases)
        {
            if (args.Count == 0)
            {
                // no command
                return new CommandArguments("", new List<string>());
            }
            // get command
            var command = CommandLineInterfaceHelper.CleanArgument(args[0]);
            // check if translation is allowed
            if (translateAliases) { command = TranslateAlias(command); }
            // get remaining arguments
            var list = new List<string>();
            foreach (var item in args.Skip(1))
            {
                var arg = item.Trim();
                if (!string.IsNullOrWhiteSpace(arg)) { list.Add(arg); }
            }
            // return command and arguments
            return new CommandArguments(command, list);

            // ################################
            string TranslateAlias(string alias_)
            {
                // check for known command names
                if (CommandLineInterfaceShared.Commands.ContainsKey(alias_))
                {
                    return alias_;
                }
                // check for aliases
                foreach (var command_ in CommandLineInterfaceShared.Commands)
                {
                    foreach (var candidateAlias in command_.Value.Aliases)
                    {
                        if (alias_.Equals(candidateAlias)) { return command_.Value.Command; }
                    }
                }
                // failed to find command or alias
                return alias_;
            }
        }
        #endregion
        #region Ctor
        private CommandArguments() { }
        /// <summary>
        /// Instantiates a <see cref="CommandArguments"/> with the given <paramref name="command"/> and <paramref name="args"/>.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="args">The arguments.</param>
        public CommandArguments(string command,
                                IList<string> args) : this()
        {
            Command = command;
            if (args != null) { Arguments = args; }
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The command.
        /// </summary>
        public string Command { get; private set; }
        /// <summary>
        /// The arguments.
        /// </summary>
        public IList<string> Arguments { get; private set; } = new string[0];
        /// <summary>
        /// Converts the <see cref="Arguments"/> into a string.
        /// </summary>
        public string ArgsString => CommandLineInterfaceHelper.ConvertToString(Arguments);
        #endregion
        #region Overrides
        /// <summary>
        /// Converts the value of this instance to a string.
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public override string ToString()
        {
            return Command + " " + ArgsString;
        }
        #endregion
    }
}
