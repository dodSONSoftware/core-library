using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.CommandLineInterface
{
    /// <summary>
    /// The Command Line Interface's main message pump. 
    /// It registers new commands and processes the user interface.
    /// </summary>
    /// <example>
    /// <para>
    /// The following code example will create a new command line interface command called random.
    /// The random command will generate a random value between 0 and 1.
    /// With command line arguments, it can produce random numbers between any two values.
    /// </para>
    /// First, create the random number command class. It will implement the <b>dodSON.Core.CommandLineInterface.IConsoleCommand</b> and generate random numbers.
    /// <br/><br/>
    /// Type 
    /// <br/>
    /// $help random
    /// <br/>
    /// to bring up a help page describing the random function and it's command line arguments.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// public class RandomNumberCommand
    ///     : dodSON.Core.CommandLineInterface.IConsoleCommand
    /// {
    ///     #region Ctor
    ///     public RandomNumberCommand() { }
    ///     #endregion
    ///     #region Private Fields
    ///     private readonly System.Random _Rnd = new System.Random();
    ///     #endregion
    ///     #region Public Properties
    ///     // A short description about the console command.
    ///     public string Description =&gt; "Generates random numbers.";
    ///     
    ///     // A description and the various command line arguments and their uses.
    ///     public string Help
    ///     {
    ///         get
    ///         {
    ///             var result = dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.HelpHeader(Description);
    ///             result.AppendLine(dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.FormatHelp("random", "returns a double random number between 0 and 1."));
    ///             result.AppendLine(dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.FormatHelp("random [min # | #] [max # | #]", "returns a integer random number between (min), inclusive, and (max), exclusive."));
    ///             return result.ToString();
    ///         }
    ///     }
    ///     
    ///     // The function invoked to perform the actions of the console command.
    ///     public void Execute(IList&lt;string&gt; args)
    ///     {
    ///         // the minimum and maximum values
    ///         var min = -1;
    ///         var max = -1;
    ///         
    ///         // create an argument generator
    ///         using (var stream = new dodSON.Core.CommandLineInterface.ArgsGenerator(args))
    ///         {
    ///             // loop through all arguments
    ///             while (stream.MoveNext())
    ///             {
    ///                 // check for the 'min' argument
    ///                 if (dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.CleanArgument(stream.Current) == "min")
    ///                 {
    ///                     // get the minimum value
    ///                     if (stream.MoveNext())
    ///                     {
    ///                         min = Convert.ToInt32(stream.Current);
    ///                     }
    ///                 }
    ///                 // check for the 'max' argument
    ///                 else if (dodSON.Core.CommandLineInterface.CommandLineInterfaceHelper.CleanArgument(stream.Current) == "max")
    ///                 {
    ///                     // get the maximum value
    ///                     if (stream.MoveNext())
    ///                     {
    ///                         max = Convert.ToInt32(stream.Current);
    ///                     }
    ///                 }
    ///                 else
    ///                 {
    ///                     // if no named arguments, then attempt to put the value into minimum first, then maximum
    ///                     
    ///                     // read the argument's value
    ///                     var dude = Convert.ToInt32(stream.Current);
    ///                     
    ///                     // test if minimum value has been read
    ///                     if (min == -1) { min = dude; }
    ///                     
    ///                     // otherwise, set the maximum value
    ///                     else { max = dude; }
    ///                 }
    ///             }
    ///         }
    ///         
    ///         // get the random value
    ///         double value = 0;
    ///         
    ///         // test if there were any arguments
    ///         if ((min == -1) &amp;&amp; (max == -1))
    ///         {
    ///             // no arguments
    ///             value = _Rnd.NextDouble();
    ///         }
    ///         else
    ///         {
    ///             // check if both arguments were found
    ///             if ((min == -1) || (max == -1)) { throw new Exception("Missing argument. Must have 2 arguments; minimum and maximum values."); }
    ///             
    ///             // both arguments found
    ///             value = _Rnd.Next(min, max);
    ///         }
    ///         
    ///         // output the results
    ///         Console.WriteLine(value);
    ///     }
    ///     #endregion
    /// }
    /// </code>
    /// <para>
    /// Next, create the main console application. It will start the Command-Line-Interface message pump.
    /// <br/><br/>
    /// Type 'help' for help and 'exit' to terminate the application.
    /// </para>
    /// <code>
    /// private static void Main(string[] _) =&gt; new MessagePump(CommandClasses, "Playground_General_CLI").Start();
    /// 
    /// private static IEnumerable&lt;MessagePumpCommand&gt; CommandClasses =&gt; new List&lt;MessagePumpCommand&gt;()
    /// {
    ///     new MessagePumpCommand("random", typeof(RandomNumberCommand), "rnd")
    /// };
    /// 
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // Playground_General_CLI
    /// // Command-Line Interface   v1.0.0.0
    /// // copyright (c) 2020 dodSON Software
    /// // 
    /// // 
    /// // &gt;&gt;&gt;help
    /// // ----------------------------------------------- ABOUT ----------------------------------------------
    /// // 
    /// // Playground_General_CLI
    /// // Command-Line Interface   v1.0.0.0
    /// // copyright (c) 2020 dodSON Software
    /// // 
    /// // ----------------------------------------------- HELP -----------------------------------------------
    /// // 
    /// // USAGE: [help] COMMAND [ARGUMENTS]
    /// // 
    /// // &gt;&gt;&gt;COMMAND [ARGUMENTS]                  To execute a specific command.
    /// // &gt;&gt;&gt;help                                 To see this help screen.
    /// // &gt;&gt;&gt;help COMMAND                         To see the help screen for a specific command.
    /// // 
    /// // 
    /// // All [ARGUMENTS] must be separated by spaces; this includes key/value pairs and lists.
    /// // Examples:
    /// // &gt;&gt;&gt;random min 0 max 100
    /// //         This will set the random command's min and max arguments to 0 and 100, respectfully.
    /// // 
    /// // &gt;&gt;&gt;calc add 10 5 8 14 32 26 19
    /// //         This will add the list of numbers together.
    /// // 
    /// // --------------------------------------------- COMMANDS ---------------------------------------------
    /// // 
    /// // calc                                              Performs various types of calculations.
    /// // cls                                               Clears the screen.
    /// // commands [list,l,c]                               Displays a list of available commands.
    /// // exit                                              Terminates the Command-Line Interface.
    /// // help [?]                                          Displays help information.
    /// // random [rnd]                                      Generates random numbers.
    /// // 
    /// // 
    /// // ----------------------------------------------------------------------------------------------------
    /// // 
    /// // 
    /// // &gt;&gt;&gt;help random
    /// // 
    /// // Generates random numbers.
    /// // 
    /// // USAGE:
    /// // random                                            returns a double random number between 0 and 1.
    /// // random [min # | #] [max # | #]                    returns a integer random number between (min), inclusive, and (max), exclusive.
    /// // 
    /// // 
    /// // &gt;&gt;&gt;random
    /// // 0.0109324352866655
    /// // 
    /// // &gt;&gt;&gt;random
    /// // 0.213840178313591
    /// // 
    /// // &gt;&gt;&gt;random
    /// // 0.315294049827053
    /// // 
    /// // &gt;&gt;&gt;random 1 100
    /// // 42
    /// // 
    /// // &gt;&gt;&gt;random 1 100
    /// // 26
    /// // 
    /// // &gt;&gt;&gt;random 1 100
    /// // 11
    /// // 
    /// // &gt;&gt;&gt;random min 100 max 110
    /// // 104
    /// // 
    /// // &gt;&gt;&gt;random min 100 max 110
    /// // 107
    /// // 
    /// // &gt;&gt;&gt;random min 100 max 110
    /// // 109
    /// // 
    /// // &gt;&gt;&gt;rnd
    /// // 0.46740560162226
    /// // 
    /// // &gt;&gt;&gt;
    /// </code>
    /// </example>    
    public class MessagePump
    {
        #region Ctor
        private MessagePump() { }
        /// <summary>
        /// Instantiates a new <see cref="MessagePump"/> with the given commands.
        /// </summary>
        /// <param name="commands">A set of key/value pairs representing the commands, and the command types to instantiate, when a command is requested.</param>
        public MessagePump(IEnumerable<MessagePumpCommand> commands) : this()
        {
            // add standard commands
            CommandLineInterfaceShared.Commands.Add("cls", new MessagePumpCommand("cls", typeof(Commands.ClearScreenCommand)));
            CommandLineInterfaceShared.Commands.Add("exit", new MessagePumpCommand("exit", typeof(Commands.ExitCommand)));
            CommandLineInterfaceShared.Commands.Add("help", new MessagePumpCommand("help", typeof(Commands.HelpCommand), "?"));
            CommandLineInterfaceShared.Commands.Add("commands", new MessagePumpCommand("commands", typeof(Commands.CommandsCommand), "list", "l", "c"));
            CommandLineInterfaceShared.Commands.Add("random", new MessagePumpCommand("random", typeof(Commands.RandomNumberCommand), "rnd"));
            CommandLineInterfaceShared.Commands.Add("calc", new MessagePumpCommand("calc", typeof(Commands.CalculatorCommand)));
            CommandLineInterfaceShared.Commands.Add("now", new MessagePumpCommand("now", typeof(Commands.DateTimeCommand), "date", "time"));
            // add custom commands
            if (commands != null)
            {
                foreach (var item in commands) { CommandLineInterfaceShared.Commands.Add(item.Command, item); }
            }
        }
        /// <summary>
        /// Instantiates a new <see cref="MessagePump"/> with the given commands and title.
        /// </summary>
        /// <param name="commands">A set of key/value pairs representing the commands, and the command types to instantiate, when a command is requested.</param>
        /// <param name="title">The title displayed when started and when the screen is cleared.</param>
        public MessagePump(IEnumerable<MessagePumpCommand> commands, string title) : this(commands)
        {
            if (!string.IsNullOrWhiteSpace(title)) { CommandLineInterfaceShared.Title = title; }
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Starts the Command Line Interface's main message pump.
        /// </summary>
        public void Start()
        {
            // display header
            Console.WriteLine(CommandLineInterfaceShared.UIHeader);
            Console.WriteLine();
            // start UI message loop
            while (true)
            {
                // get command line from user
                Console.Write(CommandLineInterfaceHelper.CommandLinePrompt);
                var line = CleanLine(Console.ReadLine());

                // extract command line arguments
                var parts = CommandLineInterfaceHelper.ConvertToArgs(line);

                // extract command and arguments
                var commArgs = CommandArguments.Create(parts);

                // process command
                if (!string.IsNullOrWhiteSpace(commArgs.Command))
                {
                    // check for exit command
                    if (commArgs.Command == "exit") { break; }
                    // process command
                    try
                    {
                        CommandLineInterfaceHelper.GetCommandObject(commArgs.Command)?.Execute(commArgs.Arguments);
                    }
                    catch (Exception ex)
                    {
                        CommandLineInterfaceHelper.DisplayError(ex.Message);
                    }
                    // blank line after invocation
                    Console.WriteLine();
                }
            }

            // ######## internal methods

            string CleanLine(string line_)
            {
                // remove tabs
                line_ = line_.Replace('\t', ' ');
                // return processed line
                return line_;
            }
        }
        #endregion
    }
}
