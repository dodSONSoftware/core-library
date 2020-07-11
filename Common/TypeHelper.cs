using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to assist with <see cref="Type"/> and <see cref="Type"/> related issues.
    /// </summary>
    public static class TypeHelper
    {
        /// <summary>
        /// Custom formats a string containing the basic elements of a type for human-readable display. Format ({Type}, {Assembly}, {Version})
        /// <para/>
        /// For example: (dodSON.Core.ComponentManagement.ComponentManager, dodSON.Core, v1.1.0.0)
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to generate the format for.</param>
        /// <returns>A string containing the basic elements of a type formatted for display.</returns>
        public static string FormatDisplayName(Type type)
        {
            var parts = System.Reflection.Assembly.GetAssembly(type).FullName.Split(',');
            return $"({type.FullName}, {parts[0]}, v{parts[1].Split('=')[1]})";
        }
        /// <summary>
        /// Custom formats a string containing the basic elements of a type for identification. Format [{Type}, {Version}, {Assembly Filename}]
        /// <para/>
        /// For example: [Worker01.WorkerPlugin, v1.0.0.0, Worker01.dll]
        /// </summary>
        /// <param name="typeName">The fully-qualified name of the type.</param>
        /// <param name="filename">The name of the file containing the <paramref name="typeName"/>.</param>
        /// <param name="rootInstallerPath">The root path where the <see cref="Installation.IInstaller"/> installs packages.</param>
        /// <param name="typeInstallPath">The actual path where the specific package is installed.</param>
        /// <returns>A string containing the basic elements of a type formatted for identification.</returns>
        public static string FormatId(string typeName,
                                      string filename,
                                      string rootInstallerPath,
                                      string typeInstallPath)
        {
            var parts = typeName.Split(',');
            var location = typeInstallPath.Substring(rootInstallerPath.Length + 1) + "\\" + System.IO.Path.GetFileName(filename);
            return $"[{parts[0]}, v{parts[2].Split('=')[1]}, {location}]";
        }


        // TODO: this needs to be further researched, reviewed and worked over...

        /// <summary>
        /// Will validate <paramref name="source"/> to see if it is a proper Fully-Qualified Assembly Name.
        /// </summary>
        /// <param name="source">The string to validate.</param>
        /// <returns><b>True</b> if <paramref name="source"/> is a valid Fully-Qualified Assembly Name; otherwise, <b>false</b>.</returns>
        [Obsolete("This function is a Work-In-Progress; therefore, results are inconclusive. DO NOT USE AT THIS TIME.")]
        public static bool ValidateTypeString(string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return false;
            }
            //
            var namespaceRegex = @"(?<ns>[a-zA-Z_][a-zA-Z\d_]+(\.[a-zA-Z_][a-zA-Z\d_]+)*)";
            var typeRegex = @"(?<t>[a-zA-Z_][a-zA-Z\d_]+(\.[a-zA-Z_][a-zA-Z\d_]+)*)";
            var versionRegex = @"(?<v>Version=(\d+\.){3}\d+)";
            var cultureRegex = "(?<c>Culture=[^,]+)";
            var tokenRegex = @"(,\s*(?<pkt>PublicKeyToken=([a-fA-F\d]{16}|null)))?";
            //var regexExpression = $@"^{namespaceRegex},\s*{typeRegex},\s*{versionRegex},\s*{cultureRegex}{tokenRegex}";
            var regexExpression = $@"^{namespaceRegex}(,\s*{typeRegex})?(,\s*{versionRegex})?(,\s*{cultureRegex})?{tokenRegex}";
            //
            try
            {
                (new System.Configuration.RegexStringValidator(regexExpression)).Validate(source);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
