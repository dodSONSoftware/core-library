using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace dodSON.Core.CommandLineInterface.Commands
{
    /// <summary>
    /// An <see cref="IConsoleCommand"/> which will perform various types of calculations.
    /// </summary>
    public class CalculatorCommand
         : IConsoleCommand
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new <see cref="ClearScreenCommand"/>.
        /// </summary>
        public CalculatorCommand() { }
        #endregion
        #region IConsoleCommand Methods
        /// <summary>
        /// A short statement about the Command Line Interface command.
        /// </summary>
        public string Description => "Performs various types of calculations.";
        /// <summary>
        /// A detailed explanation of the Command Line Interface command, it's arguments and their uses.
        /// </summary>
        public string Help
        {
            get
            {
                var result = CommandLineInterfaceHelper.HelpHeader(Description);
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc [func]", "parses, executes and returns the results of the given mathematical calculation."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc add", "adds a sequence of numbers."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc sub", "subtracts a sequence of numbers."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc mul", "multiplies a sequence of numbers."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc div", "divides a sequence of numbers."));
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp("calc mod", "modulo a sequence of numbers."));
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
            var commArgs = CommandArguments.Create(args);
            string result;
            switch (commArgs.Command)
            {
                case "func":
                    result = Function(commArgs.Arguments);
                    break;
                case "add":
                    result = Add(commArgs.Arguments);
                    break;
                case "sub":
                    result = Subtract(commArgs.Arguments);
                    break;
                case "mul":
                    result = Multiply(commArgs.Arguments);
                    break;
                case "div":
                    result = Divide(commArgs.Arguments);
                    break;
                case "mod":
                    result = Mod(commArgs.Arguments);
                    break;
                default:
                    result = Function(args);
                    break;
            }
            //
            Console.WriteLine(result);
        }
        #endregion
        #region Private Methods
        private string Function(IList<string> args) => (new DataTable()).Compute(CommandLineInterfaceHelper.ConvertToString(args), "").ToString();
        private string Add(IList<string> args)
        {
            var dude = CommandArguments.Create(args);
            var result = Convert.ToDouble(dude.Command);
            using (var stream = new ArgsGenerator(dude.Arguments))
            {
                foreach (var item in stream)
                {
                    result += Convert.ToDouble(item);
                }
            }
            return result.ToString();
        }
        private string Subtract(IList<string> args)
        {
            var dude = CommandArguments.Create(args);
            var result = Convert.ToDouble(dude.Command);
            using (var stream = new ArgsGenerator(dude.Arguments))
            {
                foreach (var item in stream)
                {
                    result -= Convert.ToDouble(item);
                }
            }
            return result.ToString();
        }
        private string Multiply(IList<string> args)
        {
            var dude = CommandArguments.Create(args);
            var result = Convert.ToDouble(dude.Command);
            using (var stream = new ArgsGenerator(dude.Arguments))
            {
                foreach (var item in stream)
                {
                    result *= Convert.ToDouble(item);
                }
            }
            return result.ToString();
        }
        private string Divide(IList<string> args)
        {
            var dude = CommandArguments.Create(args);
            var result = Convert.ToDouble(dude.Command);
            using (var stream = new ArgsGenerator(dude.Arguments))
            {
                foreach (var item in stream)
                {
                    result /= Convert.ToDouble(item);
                }
            }
            return result.ToString();
        }
        private string Mod(IList<string> args)
        {
            var dude = CommandArguments.Create(args);
            var result = Convert.ToDouble(dude.Command);
            using (var stream = new ArgsGenerator(dude.Arguments))
            {
                foreach (var item in stream)
                {
                    result %= Convert.ToDouble(item);
                }
            }
            return result.ToString();
        }
        #endregion
    }
}
