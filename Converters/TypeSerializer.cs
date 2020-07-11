using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Converters
{
    /// <summary>
    /// Provides methods to convert any, serializable, <see cref="Type"/> to, and from, strings, bytes arrays, streams, and xml formatted string.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> to convert. <b>Note:</b> This <see cref="Type"/> must be serializable.</typeparam>
    /// <example>
    /// The following code example will create a DataHolder class, populate it, convert it into xml and binary, and back again and test the results.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// [Serializable]
    /// public class DataHolder
    /// {
    ///     public DataHolder() { }
    ///
    ///     public string Alpha { get; set; }
    ///     public int Beta { get; set; }
    ///     public decimal Gamma { get; set; }
    ///     public byte[] Delta { get; set; }
    /// }
    /// 
    /// static void Main(string[] args)
    /// {
    ///     // create source data
    ///     var source = new DataHolder()
    ///     {
    ///         Alpha = "Randy",
    ///         Beta = 49,
    ///         Gamma = 3.1415927M,
    ///         Delta = new byte[] { 0x00, 0x10, 0x20, 0x30, 0x40, 0x50, 0x60, 0x70, 
    ///                              0x80, 0x90, 0xa0, 0xb0, 0xc0, 0xd0, 0xe0, 0xf0, 0xff }
    ///     };
    ///     
    ///     // create converter
    ///     var converter = new dodSON.Core.Converters.TypeSerializer&lt;DataHolder>();
    ///     
    ///     // convert source data to xml
    ///     Console.WriteLine("-------- Xml Serialization --------------------");
    ///     Console.WriteLine();
    ///     var sourceConverted = converter.ToXmlString(source);
    ///     
    ///     // display converted source data
    ///     Console.WriteLine(sourceConverted);
    ///     Console.WriteLine();
    ///     
    ///     // retrieve source data from converted data
    ///     var destination = converter.FromXmlString(sourceConverted);
    ///     
    ///     // test for similarities
    ///     var isSame = ((source.Alpha == destination.Alpha) &amp;&amp;
    ///                   (source.Beta == destination.Beta) &amp;&amp;
    ///                   (source.Gamma == destination.Gamma) &amp;&amp;
    ///                   (source.Delta.SequenceEqual(destination.Delta)));
    ///     Console.WriteLine(string.Format("Xml Conversion Results= {0}", isSame));
    ///     Console.WriteLine();
    ///     
    ///     // convert source data to binary
    ///     Console.WriteLine("-------- Binary Serialization --------------------");
    ///     Console.WriteLine();
    ///     var sourceConverted2 = converter.ToByteArray(source);
    ///     
    ///     // display converted source data
    ///     Console.WriteLine(dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(sourceConverted2));
    ///     Console.WriteLine();
    ///     
    ///     // retrieve source data from converted data
    ///     var destination2 = converter.FromByteArray(sourceConverted2);
    ///     
    ///     // test for similarities
    ///     var isSame2 = ((source.Alpha == destination2.Alpha) &amp;&amp;
    ///                   (source.Beta == destination2.Beta) &amp;&amp;
    ///                   (source.Gamma == destination2.Gamma) &amp;&amp;
    ///                   (source.Delta.SequenceEqual(destination2.Delta)));
    ///     Console.WriteLine(string.Format("Binary Conversion Results= {0}", isSame2));
    ///     Console.WriteLine();
    ///     
    ///     // demonstrate cloning
    ///     Console.WriteLine("-------- Cloning --------------------");
    ///     Console.WriteLine();
    ///     var clone = dodSON.Core.Converters.TypeSerializer&lt;DataHolder>.Clone(source);
    ///     
    ///     // test for similarities
    ///     var isCloneSame = ((source.Alpha == clone.Alpha) &amp;&amp;
    ///                       (source.Beta == clone.Beta) &amp;&amp;
    ///                       (source.Gamma == clone.Gamma) &amp;&amp;
    ///                       (source.Delta.SequenceEqual(clone.Delta)));
    ///     Console.WriteLine(string.Format("(source==clone)= {0}", source == clone));
    ///     Console.WriteLine(string.Format("Clone Results= {0}", isCloneSame));
    ///     
    ///     // mutate clone
    ///     clone.Alpha = "dodSON";
    ///     clone.Beta = 823543;
    ///     clone.Gamma = 21.991148575128552669238503682957M;
    ///     clone.Delta = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(64);
    ///     
    ///     // test clone
    ///     var isCloneSame2 = ((source.Alpha == clone.Alpha) &amp;&amp;
    ///                       (source.Beta == clone.Beta) &amp;&amp;
    ///                       (source.Gamma == clone.Gamma) &amp;&amp;
    ///                       (source.Delta.SequenceEqual(clone.Delta)));
    ///     Console.WriteLine(string.Format("Mutated Clone Results= {0}", isCloneSame2));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///
    /// // This code produces output similar to the following:
    ///
    /// // -------- Xml Serialization --------------------
    /// // 
    /// // &lt;?xml version="1.0" encoding="utf-8"?>
    /// // &lt;DataHolder xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
    /// //   &lt;Alpha>Randy&lt;/Alpha>
    /// //   &lt;Beta>49&lt;/Beta>
    /// //   &lt;Gamma>3.1415927&lt;/Gamma>
    /// //   &lt;Delta>ABAgMEBQYHCAkKCwwNDg8P8=&lt;/Delta>
    /// // &lt;/DataHolder>
    /// // 
    /// // Xml Conversion Results= True
    /// // 
    /// // -------- Binary Serialization --------------------
    /// // 
    /// // 0001000000FFFFFFFF01000000000000000C0200000051436F64654578616D706C6573466F72584D
    /// // 4C436F6D6D656E74732C2056657273696F6E3D312E302E302E302C2043756C747572653D6E657574
    /// // 72616C2C205075626C69634B6579546F6B656E3D6E756C6C05010000002D436F64654578616D706C
    /// // 6573466F72584D4C436F6D6D656E74732E50726F6772616D2B44617461486F6C6465720400000016
    /// // 3C416C7068613E6B5F5F4261636B696E674669656C64153C426574613E6B5F5F4261636B696E6746
    /// // 69656C64163C47616D6D613E6B5F5F4261636B696E674669656C64163C44656C74613E6B5F5F4261
    /// // 636B696E674669656C64010000070805020200000006030000000552616E64793100000009332E31
    /// // 34313539323709040000000F04000000110000000200102030405060708090A0B0C0D0E0F0FF0B00
    /// // 00000000000000000000000000000000000000000000000000000000000000000000000000000000
    /// // 00000000000000000000000000000000000000000000000000000000000000000000000000000000
    /// // 00000000000000000000000000000000000000000000000000000000000000000000000000000000
    /// // 00000000000000000000000000000000000000000000000000000000000000000000000000000000
    /// // 0000000000000000000000000000000000000000000000000000000000000000
    /// // 
    /// // Binary Conversion Results= True
    /// // 
    /// // -------- Cloning --------------------
    /// // 
    /// // (source==clone)= False
    /// // Clone Results= True
    /// // Mutated Clone Results= False
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable]
    public class TypeSerializer<T>
        : ITypeSerializer<T>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of TypeSerializer.
        /// </summary>
        public TypeSerializer() { }
        #endregion
        #region ITypeSerializer<T> Methods
        /// <summary>
        /// Converts the <paramref name="source"/> instance, of type <typeparamref name="T"/>, into a string.
        /// </summary>
        /// <param name="source">The instance to convert.</param>
        /// <returns>The string of the instance <paramref name="source"/>.</returns>
        public string ToString(T source)
        {
            // check for errors
            if (source == null) { throw new ArgumentNullException("source"); }
            // T -> byte[] -> base64 string
            return Convert.ToBase64String(ToByteArray(source));
        }
        /// <summary>
        /// Converts the <paramref name="source"/> instance, of type <typeparamref name="T"/>, into a byte array.
        /// </summary>
        /// <param name="source">The instance to convert.</param>
        /// <returns>The byte array of the instance <paramref name="source"/>.</returns>
        public byte[] ToByteArray(T source)
        {
            // check for errors
            if (source == null) { throw new ArgumentNullException("source"); }
            // create memory stream
            using (var ms = new System.IO.MemoryStream())
            {
                // serialize the source object into the memory stream
                (new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()).Serialize(ms, source);
                // return the memory stream as byte[]
                return ms.GetBuffer();
            }
        }
        /// <summary>
        /// Converts the <paramref name="source"/> instance, of type <typeparamref name="T"/>, into a byte array and writes it to the <paramref name="stream"/>.
        /// </summary>
        /// <param name="source">The instance to convert.</param>
        /// <param name="stream">The stream to write the converted <paramref name="source"/> to.</param>
        /// <returns>The number of bytes written to the <paramref name="stream"/>.</returns>
        public int ToStream(T source, System.IO.Stream stream)
        {
            // check for errors
            if (source == null) { throw new ArgumentNullException("source"); }
            if (stream == null) { throw new ArgumentNullException("stream"); }
            if (!stream.CanWrite) { throw new ArgumentException("stream", "Parameter stream is a non-writable stream."); }
            // convert to byte[] buffer
            var buffer = ToByteArray(source);
            // write buffer into stream
            stream.Write(buffer, 0, buffer.Length);
            // return buffer length
            return buffer.Length;
        }
        /// <summary>
        /// Converts the <paramref name="source"/> instance, of type <typeparamref name="T"/>, into a xml string.
        /// </summary>
        /// <param name="source">The instance to convert.</param>
        /// <returns>The xml string of the instance <paramref name="source"/>.</returns>
        /// <remarks>The type <typeparamref name="T"/> must be xml serializable.</remarks>
        public string ToXmlString(T source)
        {
            // create result
            var result = new StringBuilder(2560);
            // create worker objects
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var writer = new System.IO.StringWriter())
            {
                // serialize the source into the writer
                serializer.Serialize(writer, source);
                writer.Flush();
                // copy the writer to result
                result.Append(writer.ToString());
                // close the writer
                writer.Close();
            }
            // fix for changing UTF-16 to UTF-8
            result = result.Replace(System.Text.Encoding.Unicode.WebName, System.Text.Encoding.UTF8.WebName, 0, 56);
            // return results
            return result.ToString();
        }

        // ********************************

        /// <summary>
        /// Converts the string, <paramref name="content"/>, into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The string to be converted.</param>
        /// <returns>A type <typeparamref name="T"/> constructed from the string data in <paramref name="content"/>.</returns>
        public T FromString(string content)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(content)) { throw new ArgumentNullException("content", "Parameter content cannot be null or empty."); }
            // base64 string -> byte[] -> T
            return FromByteArray(Convert.FromBase64String(content));
        }
        /// <summary>
        /// Converts the byte array, <paramref name="content"/>, into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The byte array to be converted.</param>
        /// <returns>A type <typeparamref name="T"/> constructed from the byte array data in <paramref name="content"/>.</returns>
        public T FromByteArray(byte[] content)
        {
            // check for errors
            if (content == null) { throw new ArgumentNullException("content"); }
            // create memory stream using the content byte[]
            using (var ms = new System.IO.MemoryStream(content))
            {
                ms.Position = 0;
                // return: deserialize the memory stream
                return (T)(new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()).Deserialize(ms);
            }
        }
        /// <summary>
        /// Converts the entire stream, <paramref name="content"/>, into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The stream to be converted.</param>
        /// <returns>A type <typeparamref name="T"/> constructed from the entire <paramref name="content"/> stream.</returns>
        /// <remarks>This will read all of the bytes to the end of the stream.</remarks>
        public T FromStream(System.IO.Stream content)=> FromStream(content, (int)content.Length);
        /// <summary>
        /// Reads the <paramref name="length"/> of bytes from the <paramref name="content"/> stream and converts them into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The stream to be read.</param>
        /// <param name="length">The number of bytes to read from the <paramref name="content"/> stream.</param>
        /// <returns>A type <typeparamref name="T"/> constructed from the <paramref name="length"/> of bytes read from the <paramref name="content"/> stream.</returns>
        public T FromStream(System.IO.Stream content, int length)
        {
            // check for errors
            if (content == null) { throw new ArgumentNullException("content"); }
            //if ((content.Length - content.Position) < length) { throw new ArgumentOutOfRangeException("length", "Parameter length defines a size which is too large for the stream to accommodate."); }
            if (!content.CanRead) { throw new ArgumentException("content", "Parameter content is a non-readable stream."); }
            // create byte[] buffer
            var buffer = new byte[length];
            // read length number of bytes into buffer
            content.Read(buffer, 0, length);
            // return converted byte[]
            return FromByteArray(buffer);
        }
        /// <summary>
        /// Converts the xml string, <paramref name="content"/>, into a type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="content">The xml string to be converted.</param>
        /// <returns>A type <typeparamref name="T"/> constructed from the xml string data in <paramref name="content"/>.</returns>
        public T FromXmlString(string content)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(content)) { throw new ArgumentNullException("content", "Parameter content cannot be null or empty."); }
            // create worker objects
            var result = default(T);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (var reader = new System.IO.StringReader(content))
            {
                result = (T)serializer.Deserialize(reader);
            }
            // return result
            return result;
        }
        #endregion
    }
}
