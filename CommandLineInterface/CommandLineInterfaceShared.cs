using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.CommandLineInterface
{
    internal static class CommandLineInterfaceShared
    {
        #region Internal Fields
        internal static string Title = "dodSON Software Core Library";
        //
        internal static readonly string AppName = "Command-Line Interface";
        internal static readonly string Copyright = "copyright (c) 2020 dodSON Software";
        internal static readonly int SeparatorLineLength = 100;
        internal static readonly char SeparatorCharacter = '-';
        internal static readonly int HelpLineLength = 50;
        //
        internal static readonly Dictionary<string, MessagePumpCommand> Commands = new Dictionary<string, MessagePumpCommand>();
        #endregion        
        #region Internal Methods
        internal static string UIHeader
        {
            get
            {
                var results = new StringBuilder();
                //
                results.AppendLine($"{Title}");
                results.AppendLine($"{AppName}   v1.0.0.0");
                results.AppendLine($"{Copyright}");
                //
                return results.ToString();
            }
        }
        internal static string DisplayCommands(bool includeHeader)
        {
            var result = new StringBuilder();
            // header
            if (includeHeader)
            {
                result.AppendLine($"command{new string(' ', HelpLineLength - 7)}description");
                result.AppendLine(CommandLineInterfaceHelper.SeparatorLine());
            }
            // commands
            foreach (var item in Commands.OrderBy(x => x.Key))
            {
                result.AppendLine(CommandLineInterfaceHelper.FormatHelp(item.Value));
            }
            //
            return result.ToString();
        }
        #endregion
    }
}
