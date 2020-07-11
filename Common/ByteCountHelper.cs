using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to convert a count of bytes into, and from, a count of Bits, Kilobytes, Megabytes, Gigabytes and Terabytes.
    /// </summary>
    /// <example>
    /// The following code example will demonstrate the execution of the ByteCountHelper type.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// var size = dodSON.Core.Common.ByteCountHelper.FromBits(68719476736);
    /// var sizeToBits = dodSON.Core.Common.ByteCountHelper.ToBits(size);
    /// var sizeToKilobytes = dodSON.Core.Common.ByteCountHelper.ToKilobytes(size);
    /// var sizeToMegabytes = dodSON.Core.Common.ByteCountHelper.ToMegabytes(size);
    /// var sizeToGigabytes = dodSON.Core.Common.ByteCountHelper.ToGigabytes(size);
    /// var sizeToTerabytes = dodSON.Core.Common.ByteCountHelper.ToTerabytes(size);
    /// 
    /// // display conversions
    /// Console.WriteLine(string.Format("ToBits     = {0,20:N3} bits", sizeToBits));
    /// Console.WriteLine(string.Format("  Bytes    = {0,20:N3} Bytes", size));
    /// Console.WriteLine(string.Format("ToKilobytes= {0,20:N3} KB", sizeToKilobytes));
    /// Console.WriteLine(string.Format("ToMegabytes= {0,20:N3} MB", sizeToMegabytes));
    /// Console.WriteLine(string.Format("ToGigabytes= {0,20:N3} GB", sizeToGigabytes));
    /// Console.WriteLine(string.Format("ToTerabytes= {0,20:N3} TB", sizeToTerabytes));
    /// Console.WriteLine();
    /// var fromBits = dodSON.Core.Common.ByteCountHelper.FromBits((long)sizeToBits);
    /// var fromKilobytes = dodSON.Core.Common.ByteCountHelper.FromKilobytes(sizeToKilobytes);
    /// var fromMegabytes = dodSON.Core.Common.ByteCountHelper.FromMegabytes(sizeToMegabytes);
    /// var fromGigabytes = dodSON.Core.Common.ByteCountHelper.FromGigabytes(sizeToGigabytes);
    /// var fromTerabytes = dodSON.Core.Common.ByteCountHelper.FromTerabytes(sizeToTerabytes);
    /// 
    /// // display conversions
    /// Console.WriteLine(string.Format("fromBits     = {0,20:N0} Bytes", fromBits));
    /// Console.WriteLine(string.Format("  Bytes      = {0,20:N0} Bytes", size));
    /// Console.WriteLine(string.Format("fromKilobytes= {0,20:N0} Bytes", fromKilobytes));
    /// Console.WriteLine(string.Format("fromMegabytes= {0,20:N0} Bytes", fromMegabytes));
    /// Console.WriteLine(string.Format("fromGigabytes= {0,20:N0} Bytes", fromGigabytes));
    /// Console.WriteLine(string.Format("fromTerabytes= {0,20:N0} Bytes", fromTerabytes));
    /// Console.WriteLine();
    /// Console.WriteLine("press anykey...");
    /// Console.ReadKey(true);
    /// 
    /// // This code produces output similar to the following:
    /// // Actual, come to think of it, it better produce the EXTACT same results as below!
    /// 
    /// // ToBits     =   68,719,476,736.000 bits
    /// //   Bytes    =    8,589,934,592.000 Bytes
    /// // ToKilobytes=        8,388,608.000 KB
    /// // ToMegabytes=            8,192.000 MB
    /// // ToGigabytes=                8.000 GB
    /// // ToTerabytes=                0.008 TB
    /// // 
    /// // fromBits     =        8,589,934,592 Bytes
    /// //   Bytes      =        8,589,934,592 Bytes
    /// // fromKilobytes=        8,589,934,592 Bytes
    /// // fromMegabytes=        8,589,934,592 Bytes
    /// // fromGigabytes=        8,589,934,592 Bytes
    /// // fromTerabytes=        8,589,934,592 Bytes
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>
    public static class ByteCountHelper
    {
        #region Static Public Methods
        // **** To
        /// <summary>
        /// Converts the given bytes into the equivalent number of bits.
        /// </summary>
        /// <param name="bytes">The number of bytes to convert.</param>
        /// <returns>A decimal representing the equivalent number of bits.</returns>
        public static decimal ToBits(long bytes) => (decimal)((decimal)bytes * (decimal)8);
        /// <summary>
        /// Converts The given bytes into the equivalent number of kilobytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to convert.</param>
        /// <returns>A decimal representing the equivalent number of kilobytes.</returns>
        public static decimal ToKilobytes(long bytes) => (decimal)((decimal)bytes / (decimal)Kilobyte);
        /// <summary>
        /// Converts the given bytes into the equivalent number of megabytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to convert.</param>
        /// <returns>A decimal representing equivalent number of megabytes.</returns>
        public static decimal ToMegabytes(long bytes) => (decimal)((decimal)bytes / (decimal)Megabyte);
        /// <summary>
        /// Converts the given bytes into the equivalent number of gigabytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to convert.</param>
        /// <returns>A decibel representing equivalent number of gigabytes.</returns>
        public static decimal ToGigabytes(long bytes) => (decimal)((decimal)bytes / (decimal)Gigabyte);
        /// <summary>
        /// Converts to given bytes into the equivalent number of terabytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to convert.</param>
        /// <returns>A decimal representing the equivalent number of terabytes.</returns>
        public static decimal ToTerabytes(long bytes) => (decimal)((decimal)bytes / (decimal)Terabyte);
        // **** From
        /// <summary>
        /// Converts the given bits into the equivalent number of bytes.
        /// </summary>
        /// <param name="bits">The number of bits to convert.</param>
        /// <returns>A long representing the equivalent number of bytes.</returns>
        public static long FromBits(long bits)
        {
            if ((bits % 8) != 0)
            {
                throw new ArgumentOutOfRangeException("bits", "Parameter 'bits' must be evenly divisible by 8.");
            }
            return (bits / 8L);
        }
        /// <summary>
        /// Converts the given kilobytes into the equivalent number of bytes.
        /// </summary>
        /// <param name="kilobytes">The number of kilobytes to convert.</param>
        /// <returns>A long representing the equivalent number of bytes.</returns>
        public static long FromKilobytes(decimal kilobytes) => (long)(kilobytes * Kilobyte);
        /// <summary>
        /// Converts the given megabytes into the equivalent number of bytes.
        /// </summary>
        /// <param name="megabytes">The number of megabytes to convert.</param>
        /// <returns>A long representing the equivalent number of bytes.</returns>
        public static long FromMegabytes(decimal megabytes) => (long)(megabytes * Megabyte);
        /// <summary>
        /// Converts the given gigabytes into the equivalent number of bytes.
        /// </summary>
        /// <param name="gigabytes">The number of gigabytes to convert.</param>
        /// <returns>A long representing equivalent number of bytes.</returns>
        public static long FromGigabytes(decimal gigabytes) => (long)(gigabytes * Gigabyte);
        /// <summary>
        /// Converts the given terabytes into the equivalent number of bytes.
        /// </summary>
        /// <param name="terabytes">The number of terabytes to convert.</param>
        /// <returns>A long representing the equivalent number of bytes.</returns>there
        public static long FromTerabytes(decimal terabytes) => (long)(terabytes * Terabyte);
        // **** Constants
        /// <summary>
        /// Represents the number of bytes in 1 kilobyte.<br/>
        /// (1 kilobyte = 1024 bytes)
        /// </summary>
        public static long Kilobyte => 1024;
        /// <summary>
        /// Represents the number of bytes in 1 megabyte.<br/>
        /// (1 megabyte = 1,048,576 bytes)
        /// </summary>
        public static long Megabyte => 1048576;
        /// <summary>
        /// Represents the number of bytes in 1 gigabyte.<br/> 
        /// (1 gigabyte = 1,073,741,824 bytes)
        /// </summary>
        public static long Gigabyte => 1073741824;
        /// <summary>
        /// Represent the number of bytes and 1 terabyte.<br/>
        /// (1 terabyte = 1,099,511,627,776 bytes)
        /// </summary>
        public static long Terabyte => 1099511627776;
        // **** ToString
        /// <summary>
        /// Converts the given bytes into a formatted string representing either a count of bytes, kilobytes, megabytes, gigabytes or terabytes for the given bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to represent.</param>
        /// <returns>A formatted string representing either a count of bytes, kilobytes, megabytes, gigabytes or terabytes for the given bytes.</returns>
        public static string ToString(long bytes) => ToString(bytes, 0, 1, 2, 2);
        /// <summary>
        /// Converts the given bytes into a formatted string representing either a count of bytes, kilobytes, megabytes, gigabytes or terabytes for the given bytes.
        /// </summary>
        /// <param name="bytes">The number of bytes to represent.</param>
        /// <param name="kiloBytesDecimalPlaces">The number of decimal places used when displaying a kilobyte value.</param>
        /// <param name="megaBytesDecimalPlaces">The number of decimal places used when displaying a megabyte value.</param>
        /// <param name="gigaBytesDecimalPlaces">The number of decimal places used when displaying a gigabyte value.</param>
        /// <param name="teraBytesDecimalPlaces">The number of decimal places used when displaying a terabyte value.</param>
        /// <returns>A formatted string representing either a count of bytes, kilobytes, megabytes, gigabytes or terabytes for the given bytes.</returns>
        public static string ToString(long bytes,
                                      int kiloBytesDecimalPlaces,
                                      int megaBytesDecimalPlaces,
                                      int gigaBytesDecimalPlaces,
                                      int teraBytesDecimalPlaces)
        {
            if (bytes < Kilobyte)
            {
                return string.Format("{0:N0} bytes", bytes.ToString("N0"));
            }
            else if (bytes < Megabyte)
            {
                return string.Format("{0:N0} KB", ToKilobytes(bytes).ToString(string.Format("N{0}", kiloBytesDecimalPlaces)));
            }
            else if (bytes < Gigabyte)
            {
                return string.Format("{0:N0} MB", ToMegabytes(bytes).ToString(string.Format("N{0}", megaBytesDecimalPlaces)));
            }
            else if (bytes < Terabyte)
            {
                return string.Format("{0:N0} GB", ToGigabytes(bytes).ToString(string.Format("N{0}", gigaBytesDecimalPlaces)));
            }
            else
            {
                return string.Format("{0:N0} TB", ToTerabytes(bytes).ToString(string.Format("N{0}", teraBytesDecimalPlaces)));
            }
        }
        #endregion
    }
}
