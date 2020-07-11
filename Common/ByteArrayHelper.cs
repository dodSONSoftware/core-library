using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to transform byte arrays.
    /// </summary>
    /// <example>
    /// The following code example will create a byte array, split it into parts, display each part, restore the parts into a single byte array, then, it will convert a byte array to a hex string and back.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create source byte array
    ///     var source = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(249);
    /// 
    ///     // split into parts
    ///     var parts = dodSON.Core.Common.ByteArrayHelper.SplitByteArray(source, 32);
    /// 
    ///     // display parts
    ///     foreach (var part in parts)
    ///     {
    ///         Console.WriteLine(dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToString(part, dodSON.Core.Common.ByteArrayHelper.ConversionBase.Hexadecimal));
    ///     }
    /// 
    ///     // restore parts into single byte array
    ///     var restored = dodSON.Core.Common.ByteArrayHelper.RestoreByteArray(parts);
    /// 
    ///     // test restored vs source
    ///     var isSame = source.SequenceEqual(restored);
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("isSame= {0}", isSame));
    ///     Console.WriteLine(string.Format("restored.Length= {0}", restored.Length));
    ///     Console.WriteLine();
    /// 
    ///     // convert a byte array to hex string and back
    ///     var hexSource = dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToString(source, dodSON.Core.Common.ByteArrayHelper.ConversionBase.Hexadecimal);
    /// 
    ///     // display hexSource
    ///     Console.WriteLine(hexSource);
    /// 
    ///     // restore hexSource
    ///     var hexRestored = dodSON.Core.Common.ByteArrayHelper.ConvertStringToByteArray(hexSource, dodSON.Core.Common.ByteArrayHelper.ConversionBase.Hexadecimal);
    /// 
    ///     // test hex conversion
    ///     var hexIsSame = source.SequenceEqual(hexRestored);
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("hexIsSame= {0}", hexIsSame));
    ///     Console.WriteLine(string.Format("hexRestored.Length= {0}", hexRestored.Length));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press any key>");
    ///     Console.ReadKey(true);
    /// }
    ///
    /// // This code produces output similar to the following:
    /// 
    /// // 4C18450E93699152270A714F892E5E5CB4D804CE20604DAF9B82EE10338302EE
    /// // FE244FA385FD1D0415B0C0079C9CA0928B08497A039305E5BFD7879F510730D5
    /// // 790A3AA98C5490BB557E8EDE8EBA57AC7813AC803B8C1CF6A39285D2DBE46859
    /// // 7D64E6B32D5FB625F033E142154F3D306A2298EF70FE5AD66AFEACF33D04F614
    /// // 6383395AD1513768D9EF7230D387859C4770F7FC3A710A4EB4D675BE58E71F69
    /// // 04498A4A7DDF7081F5FD1743FE8524F43464A009F332088810EC9B99A73BA9DD
    /// // D308BB82125D1F86E6DFEAFC804133DB06498A6AB16DF73C8F88F11D5A57CBA1
    /// // FF9DC6522577B7FE4062607E2DE7FC542BDCCDF66E554A281A
    /// // 
    /// // isSame= True
    /// // restored.Length= 249
    /// // 
    /// // 4C18450E93699152270A714F892E5E5CB4D804CE20604DAF9B82EE10338302EE
    /// // FE244FA385FD1D0415B0C0079C9CA0928B08497A039305E5BFD7879F510730D5
    /// // 790A3AA98C5490BB557E8EDE8EBA57AC7813AC803B8C1CF6A39285D2DBE46859
    /// // 7D64E6B32D5FB625F033E142154F3D306A2298EF70FE5AD66AFEACF33D04F614
    /// // 6383395AD1513768D9EF7230D387859C4770F7FC3A710A4EB4D675BE58E71F69
    /// // 04498A4A7DDF7081F5FD1743FE8524F43464A009F332088810EC9B99A73BA9DD
    /// // D308BB82125D1F86E6DFEAFC804133DB06498A6AB16DF73C8F88F11D5A57CBA1
    /// // FF9DC6522577B7FE4062607E2DE7FC542BDCCDF66E554A281A
    /// // 
    /// // hexIsSame= True
    /// // hexRestored.Length= 249
    /// // 
    /// // press any key>
    /// </code>
    /// </example>
    public static class ByteArrayHelper
    {
        #region Static Public Methods

        // ######## CONVERT

        /// <summary>
        /// Defines the types of numeric conversions available to the <see cref="ConvertByteArrayToString(byte[], ConversionBase, string)"/> and <see cref="ConvertStringToByteArray(string, ConversionBase)"/> methods.
        /// </summary>
        public enum ConversionBase
        {
            /// <summary>
            /// Converts bytes into, and from, binary representations.
            /// </summary>
            Binary = 2,
            /// <summary>
            /// Converts bytes into, and from, octal representations.
            /// </summary>
            Octal = 8,
            /// <summary>
            /// Converts bytes into, and from, decimal representations.
            /// </summary>
            Decimal = 10,
            /// <summary>
            /// Converts bytes into, and from, hexadecimal representations.
            /// </summary>
            Hexadecimal = 16,
            /// <summary>
            /// Converts bytes into, and from, a base64 representation.
            /// </summary>
            Base64 = 64
        }
        // ----------------
        /// <summary>
        /// Will transform a byte array into a string containing either binary, octal, decimal, hexadecimal or base64 values.
        /// </summary>
        /// <param name="source">The byte array to transform.</param>
        /// <param name="conversionBase">The numeric base to convert the byte array values into.</param>
        /// <param name="byteInterstitialSpacing">The string which will be inserted between each byte value.</param>        
        /// <returns>A string containing either binary, octal, decimal, hexadecimal or base64 values of the bytes in the <paramref name="source"/> byte array.</returns>
        /// <remarks>
        /// For example:
        /// <code>
        /// Source Value (byte[]) = 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0F
        /// 
        /// Returned Value (binary) = "00000000 00000001 00000010 00000011 00000100 00000101 00000110 00000111 00001000 00001001 00001010 00001011 00001100 00001101 00001110 00001111"
        /// Returned Value (octal) = "000 001 002 003 004 005 006 007 010 011 012 013 014 015 016 017"
        /// Returned Value (decimal) = "000 001 002 003 004 005 006 007 008 009 010 011 012 013 014 015"
        /// Returned Value (hexadecimal) = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F"
        /// Returned Value (base64) = "AAECAwQFBgcICQoLDA0ODw=="
        /// </code>
        /// </remarks>
        public static string ConvertByteArrayToString(byte[] source,
                                                      ConversionBase conversionBase,
                                                      string byteInterstitialSpacing = null)
        {
            // convert Base64
            if (conversionBase == ConversionBase.Base64)
            {
                return Convert.ToBase64String(source);
            }
            else
            {
                // fix the interstitial characters
                if (byteInterstitialSpacing == null)
                {
                    byteInterstitialSpacing = "";
                }
                // get the string width of each byte representation
                var width = 2;
                switch (conversionBase)
                {
                    case ConversionBase.Binary:
                        width = 8;
                        break;
                    case ConversionBase.Octal:
                        width = 3;
                        break;
                    case ConversionBase.Decimal:
                        width = 3;
                        break;
                    case ConversionBase.Hexadecimal:
                        width = 2;
                        break;
                    default:
                        break;
                }
                // convert each byte into the desired base value, padded with leading zeros to the proper string width and separate each one with the interstitial characters
                return string.Join(byteInterstitialSpacing,
                                   source.Select(x => Convert.ToString(x, (int)conversionBase).ToUpper().PadLeft(width, '0')));
            }
        }
        /// <summary>
        /// Will transform a string containing binary, octal, decimal, hexadecimal or base64 values into a byte array.
        /// </summary>
        /// <param name="source">The string to transform.</param>
        /// <param name="conversionBase">The numeric base the string values represents.</param>
        /// <returns>A byte array containing the byte values in the string.</returns>
        /// <remarks>
        /// This method assumes that the string provided is made up of sets of numeric representations of either binary, octal, decimal, hexadecimal or base64 values.
        /// <para/><para/>
        /// For example:
        /// <code>
        /// Source Value (binary) = "00000000 00000001 00000010 00000011 00000100 00000101 00000110 00000111 00001000 00001001 00001010 00001011 00001100 00001101 00001110 00001111"
        /// Source Value (octal) = "000 001 002 003 004 005 006 007 010 011 012 013 014 015 016 017"
        /// Source Value (decimal) = "000 001 002 003 004 005 006 007 008 009 010 011 012 013 014 015"
        /// Source Value (hexadecimal) = "00 01 02 03 04 05 06 07 08 09 0A 0B 0C 0D 0E 0F"
        /// Source Value (base64) = "AAECAwQFBgcICQoLDA0ODw=="
        /// 
        /// Returned Value (byte[]) = 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C, 0x0D, 0x0F
        /// </code>
        /// </remarks>
        public static byte[] ConvertStringToByteArray(string source,
                                                      ConversionBase conversionBase)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                // return empty byte array if source is null or empty
                return new byte[0];
            }
            // convert Base64
            if (conversionBase == ConversionBase.Base64)
            {
                return Convert.FromBase64String(source);
            }
            else
            {
                // get the string width of each byte representation and get a set of valid characters
                var width = 2;
                var validCharacters = "";
                switch (conversionBase)
                {
                    case ConversionBase.Binary:
                        width = 8;
                        validCharacters = "01";
                        break;
                    case ConversionBase.Octal:
                        width = 3;
                        validCharacters = "01234567";
                        break;
                    case ConversionBase.Decimal:
                        width = 3;
                        validCharacters = "0123456789";
                        break;
                    case ConversionBase.Hexadecimal:
                        width = 2;
                        validCharacters = "0123456789abcdefABCDEF";
                        break;
                    default:
                        break;
                }
                // remove all non-valid characters
                var sourceHold = new StringBuilder(source.Length);
                for (int i = 0; i < source.Length; i++)
                {
                    if (validCharacters.Contains(source[i]))
                    {
                        sourceHold.Append(source[i]);
                    }
                }
                var source2 = sourceHold.ToString();
                // check
                if (!string.IsNullOrWhiteSpace(source2))
                {
                    if ((source2.Length % width) == 0)
                    {
                        // process string: convert to byte array
                        return Enumerable.Range(0, int.MaxValue / width)                        // iterate the maximum length of a string divided by (width) (because there are (width) characters for each byte segment)
                                         .Select(i => i * width)                                // get the starting index for each the char segment (step by (width) characters each time)
                                         .TakeWhile(i => i < source2.Length)                    // only work the length of the source string
                                         .Select(i => source2.Substring(i, width))              // get the binary string segments (take the next (width) characters)
                                         .Select(s => Convert.ToByte(s, (int)conversionBase))   // convert to byte (convert the (width) characters into a byte)
                                         .ToArray();                                            // put it all into a byte array (put the byte into an array) }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Input string length is invalid. Length={source.Length:N0}. Must be divisible by {width}. (not counting invalid characters, which are ignored)");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Input string is invalid. Must contain fixed-sized blocks of the following characters: \"{validCharacters}\"; Character Block Size={width}");
                }
            }
        }
        /// <summary>
        /// Attempts to transform a string containing binary, octal, decimal, hexadecimal or base64 values into a byte array.
        /// </summary>
        /// <param name="source">The string to transform.</param>
        /// <param name="conversionBase">The numeric base the string values represents.</param>
        /// <param name="convertedByteArray">A byte array containing the byte values in the <paramref name="source"/> string.</param>
        /// <param name="exception">The <see cref="Exception"/> generated if this function returns <b>false</b>.</param>
        /// <returns>Whether the conversion was successful or not. <b>True</b> indicates successful conversion, <b>false</b> indicates a failure and the <paramref name="exception"/> will contain the error generated.</returns>
        public static bool TryConvertStringToByteArray(string source,
                                                       ConversionBase conversionBase,
                                                       out byte[] convertedByteArray,
                                                       out Exception exception)
        {
            try
            {
                convertedByteArray = ConvertStringToByteArray(source, conversionBase);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                convertedByteArray = new byte[0];
                exception = ex;
                return false;
            }
        }
        /// <summary>
        /// Attempts to transform a byte array into a string containing either binary, octal, decimal, hexadecimal or base64 values.
        /// </summary>
        /// <param name="source">The byte array to transform.</param>
        /// <param name="conversionBase">The numeric base to convert the byte array values into.</param>
        /// <param name="convertedString">The string containing the byte values in the <paramref name="source"/> byte array.</param>
        /// <param name="exception">The <see cref="Exception"/> generated if this function returns <b>false</b>.</param>
        /// <param name="byteInterstitialSpacing">The string which will be inserted between each byte value.</param>        
        /// <returns>Whether the conversion was successful or not. <b>True</b> indicates successful conversion, <b>false</b> indicates a failure and the <paramref name="exception"/> will contain the error generated.</returns>
        public static bool TryConvertByteArrayToString(byte[] source,
                                                       ConversionBase conversionBase,
                                                       out string convertedString,
                                                       out Exception exception,
                                                       string byteInterstitialSpacing = null)
        {
            try
            {
                convertedString = ConvertByteArrayToString(source, conversionBase, byteInterstitialSpacing);
                exception = null;
                return true;
            }
            catch (Exception ex)
            {
                convertedString = "";
                exception = ex;
                return false;
            }
        }

        // ######## SPLIT AND COMBINE

        /// <summary>
        /// Will split the <paramref name="source"/> byte array into an IEnumerable&lt;byte[]&gt; with each part's length less-than or equal to the <paramref name="maximumPartSize"/>.
        /// </summary>
        /// <param name="source">The byte array to split into parts.</param>
        /// <param name="maximumPartSize">The maximum size of each part.</param>
        /// <returns>The byte array split into parts.</returns>
        public static IEnumerable<byte[]> SplitByteArray(byte[] source, int maximumPartSize)
        {
            // iterate through the array stepping by the part size length
            for (int i = 0; i < source.Length; i += maximumPartSize)
            {
                // get the next piece
                byte[] byteArrayPiece;
                if ((i + maximumPartSize) <= source.Length)
                {
                    // a whole piece
                    byteArrayPiece = new byte[maximumPartSize];
                    Array.Copy(source, i, byteArrayPiece, 0, maximumPartSize);
                }
                else
                {
                    // a partial piece
                    var remainingLength = (source.Length - i);
                    byteArrayPiece = new byte[remainingLength];
                    Array.Copy(source, i, byteArrayPiece, 0, remainingLength);
                }
                // return piece
                yield return byteArrayPiece;
            }
        }
        /// <summary>
        /// Will restore an IEnumerable&lt;byte[]&gt; into a single byte array.
        /// </summary>
        /// <param name="source">The IEnumerable&lt;byte[]&gt; of byte arrays to restore.</param>
        /// <returns>The restored byte array.</returns>
        public static byte[] RestoreByteArray(IEnumerable<byte[]> source)
        {
            // determine total length of the pieces
            int length = 0;
            foreach (var item in source)
            {
                length += item.Length;
            }
            // put all pieces into a single array
            var resultIndex = 0;
            var result = new byte[length];
            for (int i = 0; i < source.Count(); i++)
            {
                Array.Copy(source.ElementAt(i), 0, result, resultIndex, source.ElementAt(i).Length);
                resultIndex += source.ElementAt(i).Length;
            }
            // return single array
            return result.ToArray();
        }
        #endregion
    }
}
