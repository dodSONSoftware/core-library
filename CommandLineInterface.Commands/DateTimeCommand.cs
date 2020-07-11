using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will display the current date and time..
    /// </summary>
    public class DateTimeCommand
         : IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="DateTimeCommand"/>.
        /// </summary>
        public DateTimeCommand() { }
        #endregion
        #region IConsoleCommand Methods
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => "Displays the current date and time.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = CommandLineInterfaceHelper.HelpHeader(Description);
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("now", "displays the current date and time."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("now [date | d]", "displays the current date."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("now [time | t]", "displays the current time."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("now [utc | zulu | z]", "displays the current date and time in universal coordinated time."));
                return result.ToString();
            }
        }
        /// <summary>
        /// Will invoke the Command Line Interface command with the given arguments.
        /// </summary>
        /// <param name="args">The arguments to feed this Command Line Interface command.</param>
        public void Execute(IList<string> args)
        {
            var results = "";
            var utc = false;
            var date = false;
            var time = false;
            foreach (var arg in new ArgsGenerator(args))
            {
                switch (arg)
                {
                    case "utc":
                    case "zulu":
                    case "z":
                        utc = true;
                        break;
                    case "date":
                    case "d":
                        date = true;
                        break;
                    case "time":
                    case "t":
                        time = true;
                        break;
                }
            }
            //
            if ((!date && !time) || (date && time))
            {
                // neither or both date and time
                if (utc) { results = DateTimeOffset.UtcNow.ToString(); }
                else { results = DateTimeOffset.Now.ToString(); }
            }
            else if (date)
            {
                // date
                if (utc) { results = DateTimeOffset.UtcNow.ToString("D"); }
                else { results = DateTimeOffset.Now.ToString("D"); }
            }
            else if (time)
            {
                // time
                if (utc) { results = DateTimeOffset.UtcNow.ToString("T"); }
                else { results = DateTimeOffset.Now.ToString("T"); }
            }
            //
            Console.WriteLine(results);
        }
        #endregion
    }
}
