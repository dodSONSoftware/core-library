using System;
using System.Text;

namespace dodSON.Core.Configuration
{
    /// <summary>
    /// Defines methods to convert <see cref="IConfigurationGroup"/>s to and from byte arrays using a <see cref="StringBuilder"/>-based <see cref="IConfigurationSerializer{T}"/> to perform the serialization.
    /// </summary>
    public class BinaryConfigurationSerializer
        : IConfigurationSerializer<byte[]>
    {
        #region Public Static Methods
        /// <summary>
        /// Creates a clone of the <paramref name="source"/> using <see cref="BinaryConfigurationSerializer"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to clone.</param>
        /// <returns>A clone of the <paramref name="source"/> using <see cref="BinaryConfigurationSerializer"/>.</returns>
        public static IConfigurationGroup Clone(IConfigurationGroup source)
        {
            var serializer = new BinaryConfigurationSerializer();
            return serializer.Deserialize(serializer.Serialize(source));
        }
        /// <summary>
        /// Will load the <paramref name="filename"/> and try to deserialize it using an <see cref="BinaryConfigurationSerializer"/>.
        /// </summary>
        /// <param name="filename">The file to load and deserialize.</param>
        /// <returns>The <see cref="IConfigurationGroup"/> deserialized from the text in the <paramref name="filename"/>.</returns>
        public static IConfigurationGroup LoadFile(string filename)
        {
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException($"File not found.", filename); }
            //
            return (new BinaryConfigurationSerializer()).Deserialize(System.IO.File.ReadAllBytes(filename));
        }
        /// <summary>
        /// Will serialize the <paramref name="source"/> using an <see cref="BinaryConfigurationSerializer"/> and save it to the <paramref name="filename"/>.
        /// </summary>
        /// <param name="source">The <see cref="IConfigurationGroup"/> to serialize and save.</param>
        /// <param name="filename">The path and filename to save the <paramref name="source"/> to.</param>
        /// <param name="overwrite">Determines if the file should be written over if it already exists. <b>True</b> will overwrite the file; <b>false</b> will throw an exception.</param>
        /// <returns>Whether the save operation was successful. <b>True</b> indicates the save operation succeeded; <b>false</b> will be expressed by throwing an exception.</returns>
        public static void SaveFile(IConfigurationGroup source,
                                    string filename,
                                    bool overwrite)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            if (!overwrite && System.IO.File.Exists(filename)) { throw new ArgumentException($"File already exists.", nameof(overwrite)); }
            //
            System.IO.File.WriteAllBytes(filename, (new BinaryConfigurationSerializer()).Serialize(source));
        }
        #endregion
        #region Ctor
        /// <summary>
        /// <para>Initializes a new instance of the BinaryConfigurationSerializer class.</para>
        /// </summary>
        /// <remarks>
        /// Using this constructor will default to using a <see cref="dodSON.Core.Compression.DeflateStreamCompressor"/> for compression and decompression tasks.
        /// </remarks>
        /// <seealso cref="dodSON.Core.Compression">dodSON.Core.Compression Namespace</seealso>
        /// <seealso cref="dodSON.Core.Compression.DeflateStreamCompressor"/>
        public BinaryConfigurationSerializer() { }
        /// <summary>
        /// Instantiates an new instance using the given <paramref name="serializer"/> to properly serializes and deserialize <see cref="IConfigurationGroup"/>s.
        /// </summary>
        /// <param name="serializer"></param>
        public BinaryConfigurationSerializer(IConfigurationSerializer<StringBuilder> serializer) : this() => Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public BinaryConfigurationSerializer(Configuration.IConfigurationGroup configuration)
            : this()
        {
            // check 
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            if (configuration.Key != "Serializer") { throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Serializer\". Configuration Key={configuration.Key}", nameof(configuration)); }

            // test for, OPTIONAL, "Serializer" item; use default if not found.
            try
            {
                var serializerType = (Type)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Serializer", typeof(Type)).Value;
                Serializer = (IConfigurationSerializer<StringBuilder>)Core.Common.InstantiationHelper.InvokeDefaultCtor(serializerType);
            }
            catch { }
        }
        #endregion
        #region Private Fields
        private readonly Compression.ICompressor _Compressor = new Compression.DeflateStreamCompressor();
        #endregion
        #region IConfigurationSerializer<StringBuilder> Methods
        /// <summary>
        /// Converts the specified configuration to a byte array.
        /// </summary>
        /// <param name="configuration">The configuration to convert.</param>
        /// <returns>The configuration converted into a byte array.</returns>
        public byte[] Serialize(IConfigurationGroup configuration)
        {
            if (configuration == null) { throw new ArgumentNullException(nameof(configuration)); }
            return _Compressor.Compress(System.Text.Encoding.ASCII.GetBytes(Serializer.Serialize(configuration).ToString()));

        }
        /// <summary>
        /// Converts the specified <paramref name="source"/>, a byte array, to an <see cref="IConfigurationGroup"/>.
        /// </summary>
        /// <param name="source">The object, a byte array, to convert into an <see cref="IConfigurationGroup"/>.</param>
        /// <returns>The object converted into an <see cref="IConfigurationGroup"/>.</returns>
        public IConfigurationGroup Deserialize(byte[] source)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length == 0) { throw new ArgumentNullException(nameof(source), $"Parameter {nameof(source)} cannot be empty."); }
            return Serializer.Deserialize(new System.Text.StringBuilder(System.Text.Encoding.ASCII.GetString(_Compressor.Decompress(source))));
        }
        #endregion
        #region IConfigurationProvider Methods
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public Configuration.IConfigurationGroup Configuration
        {
            get
            {
                var result = new Configuration.ConfigurationGroup("Serializer");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                //
                result.Items.Add("Serializer", Serializer.GetType(), typeof(Type));
                //
                return result;
            }
        }
        #endregion
        #region Private Methods
        private IConfigurationSerializer<StringBuilder> Serializer { get; } = new XmlConfigurationSerializer();
        #endregion
    }
}
