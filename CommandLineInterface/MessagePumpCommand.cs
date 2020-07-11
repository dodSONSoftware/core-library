using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// Contains information required to be a command in the Command-Line-Interface.
    /// </summary>
    public class MessagePumpCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="MessagePumpCommand"/>.
        /// </summary>
        private MessagePumpCommand() { }
        /// <summary>
        /// Instantiates a new <see cref="MessagePumpCommand"/> with the given parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="consoleCommandType">The <see cref="IConsoleCommand"/> type to instantiate for help information and to perform work.</param>
        /// <param name="aliases">A list a aliases this command can respond to.</param>
        public MessagePumpCommand(string command,
                                  Type consoleCommandType,
                                  params string[] aliases) : this()
        {
            if (string.IsNullOrWhiteSpace(command)) { throw new ArgumentNullException(nameof(command)); }
            Command = command.Trim().ToLower();
            if (consoleCommandType == null) { throw new ArgumentNullException(nameof(consoleCommandType)); }
            ConsoleCommandType = consoleCommandType;
            if (aliases != null) { Aliases = aliases.Select(x => x.Trim().ToLower()).ToList(); }
        }
        #endregion
        #region Public Properties
        /// <summary>
        /// The command.
        /// </summary>
        public string Command { get; private set; }
        /// <summary>
        /// A list a aliases this command can respond to.
        /// </summary>
        public IList<string> Aliases { get; private set; } = new List<string>();
        /// <summary>
        /// The <see cref="IConsoleCommand"/> type to instantiate for help information and to perform work.
        /// </summary>
        public Type ConsoleCommandType { get; private set; }
        #endregion
    }
}
