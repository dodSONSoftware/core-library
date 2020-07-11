using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to work with <see cref="String"/>.
    /// </summary>
    public static class StringHelper
    {
        #region Private Static Read Only Fields
        /// <summary>
        /// Contains pairs of strings; the <b>Key</b> represents the string to be tokenized, the <b>Value</b> represents the token.
        /// </summary>
        /// <remarks>
        /// This is the format of data that the <see cref="TokenizeString(string, Dictionary{string, string})"/> and <see cref="UnTokenizeString(string, Dictionary{string, string})"/> methods require.
        /// <br/>
        /// <code language="" title="Currently contains the following Key/Value Pairs">
        /// <![CDATA[
        /// Environment.NewLine         %0D
        /// "                           %22]]>
        /// </code>
        /// </remarks>
        public static readonly Dictionary<string, string> DefaultTokenReplacementPairs = new Dictionary<string, string>();
        #endregion
        #region Static Ctor
        static StringHelper()
        {
            DefaultTokenReplacementPairs.Add(Environment.NewLine, $"%{Convert.ToInt32(Environment.NewLine[0]):X2}");
            DefaultTokenReplacementPairs.Add("\"", $"%{Convert.ToInt32('\"'):X2}");
        }
        #endregion
        #region Public Static Methods
        /// <summary>
        /// Will convert specific elements within a string to tokens.
        /// </summary>
        /// <param name="str">The <see cref="String"/> to tokenize.</param>
        /// <param name="replacements">The tokenizable values.</param>
        /// <returns>A tokenized string.</returns>
        public static string TokenizeString(string str, Dictionary<string, string> replacements)
        {
            if (str != null)
            {
                foreach (var item in replacements)
                {
                    str = str.Replace(item.Key, item.Value);
                }
            }
            return str;
        }
        /// <summary>
        /// Will convert specific tokens in a string to their non-tokenized values.
        /// </summary>
        /// <param name="str">The <see cref="String"/> to Un-tokenize.</param>
        /// <param name="replacements">The tokenizable values.</param>
        /// <returns>A Un-tokenized string.</returns>
        public static string UnTokenizeString(string str, Dictionary<string, string> replacements)
        {
            if (str != null)
            {
                foreach (var item in replacements)
                {
                    str = str.Replace(item.Value, item.Key);
                }
            }
            return str;
        }
        /// <summary>
        /// Creates a string from the <paramref name="source"/> padded on either side with <paramref name="paddingChar"/> to center the <paramref name="source"/> within the given <paramref name="width"/>.
        /// </summary>
        /// <param name="source">The string to center.</param>
        /// <param name="width">The width of the resultant string.</param>
        /// <param name="paddingChar">The character to pad on either side of the <paramref name="source"/> to center the <paramref name="source"/>.</param>
        /// <returns>The <paramref name="source"/> padded on either side with <paramref name="paddingChar"/> to center the <paramref name="source"/> within the given <paramref name="width"/>.</returns>
        public static string PadCenter(string source, int width, char paddingChar)
        {
            if ((source == null) || (source.Length >= width))
            {
                return source;
            }
            return source.PadLeft((((width - source.Length) / 2) + source.Length), paddingChar).PadRight(width, paddingChar);
        }
        /// <summary>
        /// Creates a string without <see cref="Environment.NewLine"/>s.
        /// </summary>
        /// <param name="source">The string to format.</param>
        /// <returns>A string without <see cref="Environment.NewLine"/>s.</returns>
        public static string FormatStringIntoSingleLine(string source) => source.Replace(Environment.NewLine, " ");
        #endregion
    }
}
