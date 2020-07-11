using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Provides functionality for adding, inserting and removing filters and executing pipeline items.
    /// </summary>
    /// <example>
    /// <para>The following example will use a factory to create pipelines based on configurations then use the pipeline to process pipeline items. 
    /// The created pipelines will encode and decode a byte[] using various compression, encryption and obfuscation techniques. 
    /// Finally, it will cause an error to be generated.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create payload
    ///     var payloadByteLength = 32;
    ///     var originalPayload = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(payloadByteLength);
    ///     byte[] payload = new byte[originalPayload.Length];
    ///     originalPayload.CopyTo(payload, 0);
    ///
    ///     // create settings
    ///     var settings = new ProcessSettings()
    ///     {
    ///         EncodeData = true,
    ///         Compress = true,
    ///         EncryptionLevel = EncryptionLevel.LevelTwo,
    ///         ObfuscationLevel = ObfuscationLevel.Complex,
    ///         PreProcessFilterEvent = (s, e) =&gt; { Console.WriteLine($"        -&gt; Processing Filter: {e.FilterName}"); },
    ///         ExceptionEventHandler = (s, e) =&gt;
    ///         {
    ///             Console.WriteLine();
    ///             Console.WriteLine($"#### EXCEPTION ####");
    ///             Console.WriteLine($"  Filter   : {e.FilterName}");
    ///             Console.WriteLine($"  Exception: {e.Exception.Message.Replace(Environment.NewLine, " ")}");
    ///         }
    ///     };
    ///
    ///     var originalPayloadHexString = dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(originalPayload);
    ///     Console.WriteLine($"Original Payload = {originalPayloadHexString}"); Console.WriteLine();
    ///
    ///     // create pipeline #1 (encoder)
    ///     Console.WriteLine("Creating Pipeline (Encoder)");
    ///     var pipeline = MyPipelineFactory.Create(settings);
    ///
    ///     // create pipeline item
    ///     Console.WriteLine("Creating Pipeline Item");
    ///     var pipelineItem = new MyPipelineItem(payload);
    ///
    ///     // process pipeline item
    ///     Console.WriteLine("Executing Pipeline Item");
    ///     var results = pipeline.Execute(pipelineItem);
    ///     var encodedPayload = ((MyPipelineItem)results).Payload;
    ///     var encodedPayloadHexString = dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(encodedPayload);
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Encoded Payload  = {encodedPayloadHexString}"); Console.WriteLine();
    ///
    ///     // create pipeline #2 (decoder)
    ///     Console.WriteLine("Creating Pipeline (Decoder)");
    ///     settings.EncodeData = false;
    ///     var pipeline2 = MyPipelineFactory.Create(settings);
    ///
    ///     // process pipeline item (the results from above)
    ///     Console.WriteLine("Executing Pipeline Item");
    ///     var results2 = pipeline2.Execute(results);
    ///     var decodedPayload = ((MyPipelineItem)results2).Payload;
    ///
    ///     var decodedPayloadHexString = dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(decodedPayload);
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Decoded Payload  = {decodedPayloadHexString}"); Console.WriteLine();
    ///
    ///     // test results
    ///     var isSame = originalPayload.SequenceEqual(decodedPayload);
    ///     Console.WriteLine();
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine($"Is Same = {isSame}");
    ///
    ///     // generate error 
    ///     Console.WriteLine();
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine("Generating Exception");
    ///     Console.WriteLine();
    ///     pipeline.Execute(new MyPipelineItem(null));
    ///
    ///     //
    ///     Console.WriteLine();
    ///     Console.Write("press anykey&gt;");
    ///     Console.ReadKey(true);
    /// }
    ///
    /// // ################################
    /// // Enumerations and Data Holder Classes
    /// // ################################
    ///
    /// public enum EncryptionLevel
    /// {
    ///     None = 0,
    ///     LevelOne,
    ///     LevelTwo
    /// }
    ///
    /// public enum ObfuscationLevel
    /// {
    ///     None = 0,
    ///     Simple,
    ///     Complex
    /// }
    ///
    /// public class ProcessSettings
    /// {
    ///     public bool EncodeData { get; set; }
    ///     public bool Compress { get; set; }
    ///     public EncryptionLevel EncryptionLevel { get; set; }
    ///     public ObfuscationLevel ObfuscationLevel { get; set; }
    ///     public EventHandler&lt;dodSON.Core.PipeAndFilter.ExceptionEventArgs&gt; ExceptionEventHandler { get; set; }
    ///     public EventHandler&lt;dodSON.Core.PipeAndFilter.PreProcessFilterEventArgs&gt; PreProcessFilterEvent { get; set; }
    /// }
    ///
    /// // ################################
    /// // The Pipeline Factory
    /// // ################################
    ///
    /// public static class MyPipelineFactory
    /// {
    ///     public static dodSON.Core.PipeAndFilter.IPipeline Create(ProcessSettings settings)
    ///     {
    ///         if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
    ///         // create pipeline
    ///         var pipeline = new dodSON.Core.PipeAndFilter.Pipeline();
    ///         // connect event handlers
    ///         pipeline.ExceptionEvent += settings.ExceptionEventHandler;
    ///         pipeline.PreProcessFilterEvent += settings.PreProcessFilterEvent;
    ///         // build pipeline
    ///         // NOTE: these must be built in reverse of each other.
    ///         if (settings.EncodeData)
    ///         {
    ///             if (settings.Compress) { pipeline.AddFilter(new CompressFilter()); }
    ///             if (settings.EncryptionLevel == EncryptionLevel.LevelOne) { pipeline.AddFilter(new EncryptLevelOneFilter()); }
    ///             if (settings.EncryptionLevel == EncryptionLevel.LevelTwo) { pipeline.AddFilter(new EncryptLevelTwoFilter()); }
    ///             if (settings.ObfuscationLevel == ObfuscationLevel.Simple) { pipeline.AddFilter(new ObfuscateSimpleFilter()); }
    ///             if (settings.ObfuscationLevel == ObfuscationLevel.Complex) { pipeline.AddFilter(new ObfuscateComplexFilter()); }
    ///         }
    ///         else
    ///         {
    ///             if (settings.ObfuscationLevel == ObfuscationLevel.Complex) { pipeline.AddFilter(new DeObfuscateComplexFilter()); }
    ///             if (settings.ObfuscationLevel == ObfuscationLevel.Simple) { pipeline.AddFilter(new DeObfuscateSimpleFilter()); }
    ///             if (settings.EncryptionLevel == EncryptionLevel.LevelTwo) { pipeline.AddFilter(new DecryptLevelTwoFilter()); }
    ///             if (settings.EncryptionLevel == EncryptionLevel.LevelOne) { pipeline.AddFilter(new DecryptLevelOneFilter()); }
    ///             if (settings.Compress) { pipeline.AddFilter(new DecompressFilter()); }
    ///         }
    ///         // check for empty pipeline
    ///         if (pipeline.FilterCount == 0) { pipeline.AddFilter(new NullFilter()); }
    ///         // return constructed pipeline
    ///         return pipeline;
    ///     }
    /// }
    ///
    /// // ################################
    /// // The Pipeline Item
    /// // ################################
    ///
    /// public class MyPipelineItem
    ///     : dodSON.Core.PipeAndFilter.PipelineItemBase
    /// {
    ///     public MyPipelineItem(byte[] payload) { Payload = payload; }
    ///
    ///     public byte[] Payload { get; set; }
    /// }
    ///
    /// // ################################
    /// // The Filters
    /// // ################################
    ///
    /// public class NullFilter
    /// : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Null"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem) { return pipelineItem; }
    /// }
    ///
    /// public class CompressFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Compress"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         var item = pipelineItem as MyPipelineItem;
    ///         var compressor = new dodSON.Core.Compression.DeflateStreamCompressor();
    ///         item.Payload = compressor.Compress(item.Payload);
    ///         return item;
    ///     }
    /// }
    /// public class DecompressFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Decompress"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         var item = pipelineItem as MyPipelineItem;
    ///         var compressor = new dodSON.Core.Compression.DeflateStreamCompressor();
    ///         item.Payload = compressor.Decompress(item.Payload);
    ///         return item;
    ///     }
    /// }
    ///
    /// public class EncryptLevelOneFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Encrypt Level 1"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5Cng();
    ///         System.Security.SecureString password = new System.Security.SecureString();
    ///         foreach (var ch in "Pa$$W0rd") { password.AppendChar(ch); }
    ///         byte[] salt = System.Text.Encoding.ASCII.GetBytes("PasswordSalt");
    ///         dodSON.Core.Cryptography.ISaltedPassword saltedPassword = new dodSON.Core.Cryptography.SaltedPassword(hashAlgorithm, password, salt);
    ///         Type symmetricAlgorithmType = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///         dodSON.Core.Cryptography.IEncryptorConfiguration encryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
    ///         //
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = dodSON.Core.Cryptography.CryptographyHelper.Encrypt(item.Payload, encryptorConfiguration);
    ///         return item;
    ///     }
    /// }
    /// public class DecryptLevelOneFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Decrypt Level 1"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5Cng();
    ///         System.Security.SecureString password = new System.Security.SecureString();
    ///         foreach (var ch in "Pa$$W0rd") { password.AppendChar(ch); }
    ///         byte[] salt = System.Text.Encoding.ASCII.GetBytes("PasswordSalt");
    ///         dodSON.Core.Cryptography.ISaltedPassword saltedPassword = new dodSON.Core.Cryptography.SaltedPassword(hashAlgorithm, password, salt);
    ///         Type symmetricAlgorithmType = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///         dodSON.Core.Cryptography.IEncryptorConfiguration encryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
    ///         //
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = dodSON.Core.Cryptography.CryptographyHelper.Decrypt(item.Payload, encryptorConfiguration);
    ///         return item;
    ///     }
    /// }
    ///
    /// public class EncryptLevelTwoFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Encrypt Level 2"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5Cng();
    ///         System.Security.SecureString password = new System.Security.SecureString();
    ///         foreach (var ch in "Pa$$W0rdNumber-2") { password.AppendChar(ch); }
    ///         byte[] salt = System.Text.Encoding.ASCII.GetBytes("PasswordSaltNumber-2");
    ///         dodSON.Core.Cryptography.ISaltedPassword saltedPassword = new dodSON.Core.Cryptography.SaltedPassword(hashAlgorithm, password, salt);
    ///         // encrypt with level one
    ///         pipelineItem = (new EncryptLevelOneFilter()).Process(pipelineItem);
    ///         // encrypt with level two
    ///         Type symmetricAlgorithmType = typeof(System.Security.Cryptography.RijndaelManaged);
    ///         dodSON.Core.Cryptography.IEncryptorConfiguration encryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = dodSON.Core.Cryptography.CryptographyHelper.Encrypt(item.Payload, encryptorConfiguration);
    ///         //
    ///         return item;
    ///     }
    /// }
    /// public class DecryptLevelTwoFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Decrypt Level 2"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         System.Security.Cryptography.HashAlgorithm hashAlgorithm = new System.Security.Cryptography.MD5Cng();
    ///         System.Security.SecureString password = new System.Security.SecureString();
    ///         foreach (var ch in "Pa$$W0rdNumber-2") { password.AppendChar(ch); }
    ///         byte[] salt = System.Text.Encoding.ASCII.GetBytes("PasswordSaltNumber-2");
    ///         dodSON.Core.Cryptography.ISaltedPassword saltedPassword = new dodSON.Core.Cryptography.SaltedPassword(hashAlgorithm, password, salt);
    ///         // decrypt with level two
    ///         Type symmetricAlgorithmType = typeof(System.Security.Cryptography.RijndaelManaged);
    ///         dodSON.Core.Cryptography.IEncryptorConfiguration encryptorConfiguration = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = dodSON.Core.Cryptography.CryptographyHelper.Decrypt(item.Payload, encryptorConfiguration);
    ///         // decrypt with level one
    ///         pipelineItem = (new DecryptLevelOneFilter()).Process(pipelineItem);
    ///         //
    ///         return item;
    ///     }
    /// }
    ///
    /// public class ObfuscateSimpleFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Obfuscation: Simple"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = item.Payload.Reverse().ToArray();
    ///         //
    ///         return item;
    ///     }
    /// }
    /// public class DeObfuscateSimpleFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "De-Obfuscation: Simple"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         var item = pipelineItem as MyPipelineItem;
    ///         item.Payload = item.Payload.Reverse().ToArray();
    ///         //
    ///         return item;
    ///     }
    /// }
    ///
    /// public class ObfuscateComplexFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "Obfuscation: Complex"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         // obfuscate with level one
    ///         pipelineItem = (new ObfuscateSimpleFilter()).Process(pipelineItem);
    ///         // obfuscate with level two
    ///         var item = pipelineItem as MyPipelineItem;
    ///         var newByteArray = new byte[item.Payload.Length];
    ///         for (int i = 0; i &lt; item.Payload.Length; i++)
    ///         {
    ///             var b = item.Payload[i];
    ///             var newB = ((b &amp; 0xF0) &gt;&gt; 4) | ((b &amp; 0x0f) &lt;&lt; 4);
    ///             newByteArray[i] = (byte)newB;
    ///         }
    ///         item.Payload = newByteArray;
    ///         //
    ///         return item;
    ///     }
    /// }
    /// public class DeObfuscateComplexFilter
    ///     : dodSON.Core.PipeAndFilter.FilterBase
    /// {
    ///     public override string Name { get { return "De-Obfuscation: Complex"; } }
    ///
    ///     public override dodSON.Core.PipeAndFilter.IPipelineItem Process(dodSON.Core.PipeAndFilter.IPipelineItem pipelineItem)
    ///     {
    ///         // de-obfuscate with level two
    ///         var item = pipelineItem as MyPipelineItem;
    ///         var newByteArray = new byte[item.Payload.Length];
    ///         for (int i = 0; i &lt; item.Payload.Length; i++)
    ///         {
    ///             var b = item.Payload[i];
    ///             var newB = ((b &amp; 0xF0) &gt;&gt; 4) | ((b &amp; 0x0f) &lt;&lt; 4);
    ///             newByteArray[i] = (byte)newB;
    ///         }
    ///         item.Payload = newByteArray;
    ///         // de-obfuscate with level one
    ///         pipelineItem = (new DeObfuscateSimpleFilter()).Process(pipelineItem);
    ///         //
    ///         return item;
    ///     }
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// 
    /// // Original Payload = 664B8BD1B285273833500078D755C1A04328B2994E3D53DBBFF5BF732BC85760
    /// // 
    /// // Creating Pipeline(Encoder)
    /// // Creating Pipeline Item
    /// // Executing Pipeline Item
    /// //         -&gt; Processing Filter: Compress
    /// //         -&gt; Processing Filter: Encrypt Level 2
    /// //         -&gt; Processing Filter: Obfuscation: Complex
    /// // 
    /// // Encoded Payload  = 87291CACE0F9335ECBB938534BEA97C1EEA4FC239D6D2C3FECAC09139D68015477F5E58F09C1A679825635E27CB12ECC
    /// // 
    /// // Creating Pipeline(Decoder)
    /// // Executing Pipeline Item
    /// //         -&gt; Processing Filter: De-Obfuscation: Complex
    /// //         -&gt; Processing Filter: Decrypt Level 2
    /// //         -&gt; Processing Filter: Decompress
    /// // 
    /// // Decoded Payload  = 664B8BD1B285273833500078D755C1A04328B2994E3D53DBBFF5BF732BC85760
    /// // 
    /// // --------------------------------
    /// // 
    /// // Is Same = True
    /// // 
    /// // --------------------------------
    /// // 
    /// // Generating Exception
    /// // 
    /// //         -&gt; Processing Filter: Compress
    /// //         -&gt; Processing Filter: Encrypt Level 2
    /// //         -&gt; Processing Filter: Obfuscation: Complex
    /// // 
    /// // #### EXCEPTION ####
    /// //   Filter   : Obfuscation: Complex
    /// //   Exception: Value cannot be null. Parameter name: source
    /// // 
    /// // press anykey&gt;
    /// </code>
    /// </example>
    public class Pipeline
        : IPipeline
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the Pipeline class.
        /// </summary>
        public Pipeline()
        {
        }
        /// <summary>
        /// Initializes a new instance of the Pipeline class with the given <see cref="IFilter"/>s.
        /// </summary>
        /// <param name="filters">Filters to add to this Pipeline instance.</param>
        public Pipeline(params IFilter[] filters)
            : this()
        {
            if (filters != null)
            {
                _Filters.AddRange(filters);
            }
        }
        #endregion
        #region Private Fields
        private List<IFilter> _Filters = new List<IFilter>();
        #endregion
        #region IPipeline Methods
        /// <summary>
        /// An event raised when an exception occurs while processing a pipeline item.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> ExceptionEvent;
        /// <summary>
        /// Occurs before each filter processes a pipeline item.
        /// </summary>
        public event EventHandler<PreProcessFilterEventArgs> PreProcessFilterEvent;
        /// <summary>
        /// Adds a filter to the end of the filter list.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddFilter(IFilter filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter", "Parameter filter cannot be null.");
            }
            InsertFilter(_Filters.Count, filter);
        }
        /// <summary>
        /// Inserts a filter at the specified index.
        /// </summary>
        /// <param name="filterIndex">The zero-based index at which the filter should be inserted.</param>
        /// <param name="filter">The filter to insert.</param>
        public void InsertFilter(int filterIndex, IFilter filter)
        {
            if ((filterIndex < 0) || (filterIndex > _Filters.Count))
            {
                throw new ArgumentOutOfRangeException(("filterIndex"));
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter", "Parameter filter cannot be null.");
            }
            _Filters.Insert(filterIndex, filter);
        }
        /// <summary>
        /// Inserts a filter before the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to insert this filter before.</param>
        /// <param name="filter">The filter to insert.</param>
        public void InsertFilterBefore(string filterName, IFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException("filterName", "Parameter filterName cannot be null or empty.");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter", "Parameter filter cannot be null.");
            }
            // find index of filter; insert at (index)
            int filterIndex = FindFilterIndexByName(filterName);
            if (filterIndex != -1)
            {
                InsertFilter(filterIndex, filter);
            }
            else
            {
                throw new ArgumentException(string.Format("The named filter ({0}) cannot be found.", filterName), "filterName");
            }
        }
        /// <summary>
        /// Inserts a filter after the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to insert this filter after.</param>
        /// <param name="filter">The filter to insert.</param>
        public void InsertFilterAfter(string filterName, IFilter filter)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException("filterName", "Parameter filterName cannot be null or empty.");
            }
            if (filter == null)
            {
                throw new ArgumentNullException("filter", "Parameter filter cannot be null.");
            }
            // find index of filter; insert at (index + 1)
            int filterIndex = FindFilterIndexByName(filterName);
            if (filterIndex != -1)
            {
                InsertFilter(filterIndex + 1, filter);
            }
            else
            {
                throw new ArgumentException(string.Format("The named filter ({0}) cannot be found.", filterName), "filterName");
            }
        }
        /// <summary>
        /// Removes the filter by ordinal index.
        /// </summary>
        /// <param name="filterIndex">The zero-based index of the filter to remove.</param>
        /// <returns>The filter removed.</returns>
        public IFilter RemoveFilter(int filterIndex)
        {
            if ((filterIndex < 0) || (filterIndex > _Filters.Count))
            {
                throw new ArgumentOutOfRangeException(("filterIndex"));
            }
            IFilter holdFilter = _Filters[filterIndex];
            _Filters.RemoveAt(filterIndex);
            return holdFilter;
        }
        /// <summary>
        /// Removes the filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter to remove.</param>
        /// <returns>The filter removed.</returns>
        public IFilter RemoveFilter(string filterName)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException("filterName", "Parameter filterName cannot be null or empty.");
            }
            int filterIndex = FindFilterIndexByName(filterName);
            if (filterIndex != -1)
            {
                return RemoveFilter(filterIndex);
            }
            else
            {
                throw new ArgumentException(string.Format("The named filter ({0}) cannot be found.", filterName), "filterName");
            }
        }
        /// <summary>
        /// Gets the number of filters in this instance.
        /// </summary>
        public int FilterCount => _Filters.Count;
        /// <summary>
        /// Gets all the names of the filters in this instance.
        /// </summary>
        public IEnumerable<string> FilterNames
        {
            get
            {
                foreach (var item in _Filters)
                {
                    yield return item.Name;
                }
            }
        }
        /// <summary>
        /// Will execute the given pipeline item starting with the first filter.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        public IPipelineItem Execute(IPipelineItem pipelineItem)
        {
            if (pipelineItem == null)
            {
                throw new ArgumentNullException("pipelineItem", "Parameter pipelineItem cannot be null.");
            }
            return Execute(0, pipelineItem);
        }
        /// <summary>
        /// Will execute the given pipeline item starting with the filter at the filter index.
        /// </summary>
        /// <param name="filterIndex">The index of the filter to start process.</param>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        public IPipelineItem Execute(int filterIndex, IPipelineItem pipelineItem)
        {
            if ((filterIndex < 0) || (filterIndex >= _Filters.Count))
            {
                throw new ArgumentOutOfRangeException(("filterIndex"));
            }
            if (pipelineItem == null)
            {
                throw new ArgumentNullException("pipelineItem", "Parameter pipelineItem cannot be null.");
            }
            for (int index = filterIndex; index < _Filters.Count; index++)
            {
                RaisePreProcessFilterEvent(pipelineItem, index);
                if (pipelineItem.ContinueProcessing)
                {
                    try
                    {
                        pipelineItem = _Filters[index].Process(pipelineItem);
                    }
                    catch (Exception ex)
                    {
                        RaiseExceptionEvent(this, new ExceptionEventArgs(pipelineItem, index, _Filters[index].Name, ex));
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return pipelineItem;
        }
        /// <summary>
        /// Will execute the given pipeline item starting with the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to start processing.</param>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        public IPipelineItem Execute(string filterName, IPipelineItem pipelineItem)
        {
            if (string.IsNullOrWhiteSpace(filterName))
            {
                throw new ArgumentNullException("filterName", "Parameter filterName cannot be null or empty.");
            }
            if (pipelineItem == null)
            {
                throw new ArgumentNullException("pipelineItem", "Parameter pipelineItem cannot be null.");
            }
            int filterIndex = FindFilterIndexByName(filterName);
            if (filterIndex != -1)
            {
                return Execute(filterIndex, pipelineItem);
            }
            else
            {
                throw new ArgumentException(string.Format("The named filter ({0}) cannot be found.", filterName), "filterName");
            }
        }
        #endregion
        #region Private Methods
        private void RaisePreProcessFilterEvent(IPipelineItem pipelineItem, int index)
        {
            PreProcessFilterEvent?.Invoke(this, new PreProcessFilterEventArgs(pipelineItem, index, _Filters[index].Name));
        }
        private void RaiseExceptionEvent(object sender, ExceptionEventArgs e)
        {
            ExceptionEvent?.Invoke(this, e);
        }
        private int FindFilterIndexByName(string filterName)
        {
            for (int i = 0; i < _Filters.Count; i++)
            {
                if (_Filters[i].Name.Equals(filterName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }
        #endregion
    }
}
