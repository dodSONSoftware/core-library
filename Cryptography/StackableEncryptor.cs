using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: Fix This. Problem Id= 3333E15BFE8746CEAA1D5664BFFD0E18
//      I'm using the salted password wrong in the dodSON.Core.Cryptography.CryptographyHelper code.
//      See dodSON.Core.Cryptography.CryptographyHelper where I'm only using the PASSWORD byte array as the PASSWORD; that's not right
//
//      It's probable that I should just create a ( byte[] EVIDENCE ) property instead.
//      Must Rethink This!

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Implements the <see cref="IEncryptor"/> interface using any number of <see cref="IEncryptorConfiguration"/>s to encrypt and decrypt byte arrays.
    /// </summary>
    /// <example>
    /// The following example will demonstrate how to stack multiple encryptions.
    /// <para>
    /// Create a console application.<br/><br/>
    /// Be sure to add the following line to the using group at the top of the .cs file.
    /// This will ensure you can access the System.Security.SecureString Extensions <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendBytes"/> and
    /// <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendChars"/>.
    /// </para>
    /// <code>
    /// using dodSON.Core.Cryptography;
    /// </code>
    /// <para>
    /// Then, add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create source data
    ///     var sourceDataStr = dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(14, true);
    ///     var sourceData = System.Text.Encoding.Unicode.GetBytes(sourceDataStr);
    ///     
    ///     // create 3 System.Security.SecureStrings
    ///     // NOTE:    DO NOT store passwords in strings like below. These password are embedded in the executable, as strings, in the clear!
    ///     //          Be sure the source of password information is secure and transitory.
    ///     var passwordCharArray1 = "{359FDD30-3DEE-4DBF-A508-D21BED3A2BE2}".ToCharArray();
    ///     var secureStr1 = new System.Security.SecureString();
    ///     secureStr1.AppendChars(passwordCharArray1, true);
    ///     
    ///     var passwordCharArray2 = "{29854AD6-FD76-4607-8B9B-4D357C713D52}".ToCharArray();
    ///     var secureStr2 = new System.Security.SecureString();
    ///     secureStr2.AppendChars(passwordCharArray2, true);
    ///     
    ///     var passwordCharArray3 = "{570333AE-E256-479C-946B-035788FDE8DC}".ToCharArray();
    ///     var secureStr3 = new System.Security.SecureString();
    ///     secureStr3.AppendChars(passwordCharArray3, true);
    ///     
    ///     // create 3 salted passwords
    ///     var hashProvider1 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    ///     var saltedPassword1 = new dodSON.Core.Cryptography.SaltedPassword(hashProvider1, secureStr1, 64);
    ///     
    ///     var hashProvider2 = new System.Security.Cryptography.SHA512CryptoServiceProvider();
    ///     var saltedPassword2 = new dodSON.Core.Cryptography.SaltedPassword(hashProvider2, secureStr2, 64);
    ///     
    ///     var hashProvider3 = new System.Security.Cryptography.RIPEMD160Managed();
    ///     var saltedPassword3 = new dodSON.Core.Cryptography.SaltedPassword(hashProvider3, secureStr3, 64);
    ///     
    ///     // create 3 encryptor configurations
    ///     var symmetricAlgorithmType1 = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///     var encryptionConfig1 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword1, symmetricAlgorithmType1);
    ///     var symmetricAlgorithmType2 = typeof(System.Security.Cryptography.AesCryptoServiceProvider);
    ///     var encryptionConfig2 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword2, symmetricAlgorithmType2);
    ///     var symmetricAlgorithmType3 = typeof(System.Security.Cryptography.RijndaelManaged);
    ///     var encryptionConfig3 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword3, symmetricAlgorithmType3);
    ///     
    ///     // create a stackable encryptor with all 3 encryptor configurations
    ///     var configList = new List&lt;dodSON.Core.Cryptography.EncryptorConfiguration&gt;()
    ///                             {
    ///                                 encryptionConfig1, 
    ///                                 encryptionConfig2,
    ///                                 encryptionConfig3
    ///                             };
    ///     var encryptor = new dodSON.Core.Cryptography.StackableEncryptor(configList);
    ///     
    ///     // encrypt data
    ///     var encryptedData = encryptor.Encrypt(sourceData);
    ///     
    ///     // decrypt encrypted-data
    ///     var decryptedData = encryptor.Decrypt(encryptedData);
    ///     
    ///     // compare original data with transformed data
    ///     var isSame = sourceData.SequenceEqual(decryptedData);
    ///     
    ///     // display results
    ///     Console.WriteLine(sourceDataStr);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("Encryption/Decryption Results= {0}", isSame));
    ///     Console.WriteLine(string.Format("Original Data Size = {0:N0}", sourceData.Length));
    ///     Console.WriteLine(string.Format("Encrypted Data Size= {0:N0}", encryptedData.Length));
    ///     Console.WriteLine(string.Format("Decrypted Data Size= {0:N0}", decryptedData.Length));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///  
    /// // This code produces output similar to the following:
    /// // (this is only the end of the output stream)
    /// 
    /// // .
    /// // .
    /// // .
    /// // Modo feugiat fabellas no at vivendum persequeris epicurei cu elaboraret. Quaesti
    /// // o semper nam his pri cu periculis referrentur ullum vituperatoribus. Ut vix eu d
    /// // ignissim pharetra vidit repudiandae fermentum nibh te. Corrumpit no nusquam eum
    /// // montes tollit cum quaerendum facilisis mei. Quaestio sem labores placerat amet s
    /// // ed eos has ius vidit. Per magna per eum mea eum quando ut viderer definitionem.
    /// // Pri dictas donec accusam sonet est eu tincidunt impedit ullamcorper. Cras id ame
    /// // t nam accusata eam vel periculis non odio. Erat vestibulum facete elitr ponderum
    /// //  in quaerendum ei aliquando fierent. Regione eligendi pri vivendum tortor mel qu
    /// // i proin invidunt liber. Quo nostro in urbanitas mea sit usu mazim no platonem. V
    /// // arius erant voluptua regione soleat appellantur nec eam at eripuit. Ei honestati
    /// // s ad pro nonumes assentior labores tractatos suscipiantur accusamus. Aliquam ad
    /// // dignissim at accusam velit sed alia te quaerendum.
    /// // 
    /// // Vis hendrerit elementum posse tractatos voluptaria accumsan adhuc. Pretium in pu
    /// // rus fugit viderer at sit ludus. Arcu deterruisset ut usu sed ei eripuit eos. Ine
    /// // rmis ea option commodo vidit solum inciderint expetenda. Imperdiet tincidunt ut
    /// // officiis est id et ex. Pri eget alii homero laoreet phasellus eos has. Ac his pr
    /// // o consequuntur oblique donec tation delicatissimi. Dolore petentium mea ut legen
    /// // dos laboramus nec sit. Viderer modus ei duo lucilius et tale solum. Consequat ea
    /// // m vehicula suscipiantur cu persius ea harum.
    /// // 
    /// // Accommodare dictumst an suas te delenit at morbi eu. Placerat ornatus te nec qui
    /// //  ipsum ex no cu. Etiam qui at definitiones a duo meis vis justo. Eros falli scri
    /// // pta ad legendos te his deserunt ut. Bibendum mel intellegam deleniti quo ut vis
    /// // sed an. Pede eripuit eos noster maecenas comprehensam prodesset vestibulum stet.
    /// //  Porta appareat wisi denique his has timeam dicant consetetur. Odio usu liber pr
    /// // odesset orci tollit mauris senserit possim. His quo pro ponderum vel utroque tor
    /// // tor eros qui. Deleniti ut elementum laboramus tota nusquam at eos et. Nisl atomo
    /// // rum commodo democritum errem nonummy usu pro dicit. Adversarium duo libero ponde
    /// // rum corpora an ut integre sed. Sed instructior pro detracto eros sensibus eam so
    /// // ciis fuisset.
    /// // 
    /// // ------------------------------------
    /// // 
    /// // Encryption/Decryption Results= True
    /// // Original Data Size = 21,974
    /// // Encrypted Data Size= 22,000
    /// // Decrypted Data Size= 21,974
    /// // 
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/><br/>
    /// This example will demonstrate how to stack compression and encryption. Then it will demonstrate how data should be compressed before it is encrypted; 
    /// in other words, compressing encrypted data has, historically, resulted in more data returned than entered.
    /// <br/><br/>
    /// <b>Note:</b> Raw data, such as strings and serialized objects (byte arrays), should be compressed before being encrypted. Typically, encrypted data gets bigger when compressed.
    /// <para>
    /// Create a console application.<br/><br/>
    /// Be sure to add the following line to the using group at the top of the .cs file.
    /// This will ensure you can access the System.Security.SecureString Extensions <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendBytes"/> and
    /// <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendChars"/>.
    /// </para>
    /// <code>
    /// using dodSON.Core.Cryptography;
    /// </code>
    /// <para>
    /// Then, add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     var numberOfParagraphs = 64;
    ///    
    ///     // create 2 System.Security.SecureStrings
    ///     // NOTE:    DO NOT store passwords in strings like below. These password are embedded in the executable, as strings, in the clear!
    ///     //          Be sure the source of password information is secure and transitory.
    ///     var passwordCharArray1 = dodSON.Core.Common.LoremIpsumGenerator.GenerateSentence(7, "").Replace(" ", "").ToCharArray();
    ///     var secureStr1 = new System.Security.SecureString();
    ///     secureStr1.AppendChars(passwordCharArray1, true);
    ///    
    ///     var passwordCharArray2 = dodSON.Core.Common.LoremIpsumGenerator.GenerateSentence(7, "").Replace(" ", "").ToCharArray();
    ///     var secureStr2 = new System.Security.SecureString();
    ///     secureStr2.AppendChars(passwordCharArray2, true);
    ///    
    ///     // create 2 salted passwords
    ///     var hashProvider1 = new System.Security.Cryptography.MD5CryptoServiceProvider();
    ///     var saltedPassword1 = new dodSON.Core.Cryptography.SaltedPassword(hashProvider1, secureStr1, 64);
    ///    
    ///     var hashProvider2 = new System.Security.Cryptography.SHA512CryptoServiceProvider();
    ///     var saltedPassword2 = new dodSON.Core.Cryptography.SaltedPassword(hashProvider2, secureStr2, 64);
    ///    
    ///     // create 2 encryptor configurations
    ///     var symmetricAlgorithmType1 = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///     var encryptionConfig1 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword1, symmetricAlgorithmType1);
    ///    
    ///     var symmetricAlgorithmType2 = typeof(System.Security.Cryptography.AesCryptoServiceProvider);
    ///     var encryptionConfig2 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword2, symmetricAlgorithmType2);
    ///    
    ///     // create a stackable encryptor
    ///     var encryptor = new dodSON.Core.Cryptography.StackableEncryptor(
    ///                             new dodSON.Core.Cryptography.IEncryptorConfiguration[]
    ///                             {
    ///                                 encryptionConfig1,
    ///                                 encryptionConfig2
    ///                             });
    ///    
    ///     // create a compressor
    ///     var compressor = new dodSON.Core.Compression.DeflateStreamCompressor();
    ///    
    ///     // create source data
    ///     var sourceStr = dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(numberOfParagraphs, true);
    ///     var sourceData = System.Text.Encoding.Unicode.GetBytes(sourceStr);
    ///    
    ///     // compress and encrypt data
    ///     var compressedData = compressor.Compress(sourceData);
    ///     var encryptedData = encryptor.Encrypt(compressedData);
    ///    
    ///     // decrypt and decompress data
    ///     var decryptedData = encryptor.Decrypt(encryptedData);
    ///     var decompressedData = compressor.Decompress(decryptedData);
    ///    
    ///     // compare original data with transformed data
    ///     var isSame = sourceData.SequenceEqual(decompressedData);
    ///    
    ///     // In general, raw data, such as strings and serialized objects, should be compressed before being encrypted.
    ///     // **** This is the wrong way; I feel confident enough to say that when you run this on your system in the future,
    ///     //      the encryptedThenCompressedData.Length value below WILL be larger than the sourceData.Length!
    ///     var encryptedThenCompressedData = compressor.Compress(encryptor.Encrypt(sourceData));
    ///    
    ///    
    ///     // display results
    ///     Console.WriteLine(sourceStr);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("CORRECT WAY RESULTS:");
    ///     Console.WriteLine(string.Format("Comparison Results    = {0}", isSame));
    ///     Console.WriteLine(string.Format("Original Data Size    = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(sourceData.Length), sourceData.Length));
    ///     Console.WriteLine(string.Format("Compressed Data Size  = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(compressedData.Length), compressedData.Length));
    ///     Console.WriteLine(string.Format("Encrypted Data Size   = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(encryptedData.Length), encryptedData.Length));
    ///     Console.WriteLine(string.Format("Decrypted Data Size   = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(decryptedData.Length), decryptedData.Length));
    ///     Console.WriteLine(string.Format("Decompressed Data Size= {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(decompressedData.Length), decompressedData.Length));
    ///     Console.WriteLine(string.Format("Compression Ratio     = {0:N2}", ((double)compressedData.Length / (double)decompressedData.Length)));
    ///     Console.WriteLine(string.Format("Compression Percentage= {0:N0}%", (1.0 - ((double)compressedData.Length / (double)decompressedData.Length)) * 100.0));
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("WRONG WAY RESULTS:");
    ///     Console.WriteLine(string.Format("Original Data Size              = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(sourceData.Length), sourceData.Length));
    ///     Console.WriteLine(string.Format("Encrypted/Compressed Data Size  = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(encryptedThenCompressedData.Length), encryptedThenCompressedData.Length));
    ///     Console.WriteLine(string.Format("        {0} by {1:N0} bytes!", ((sourceData.Length > encryptedThenCompressedData.Length) ? "Smaller" : "Bigger"), Math.Abs(sourceData.Length - encryptedThenCompressedData.Length)));
    ///     Console.WriteLine(string.Format("Compression Ratio               = {0}", ((double)encryptedThenCompressedData.Length / (double)sourceData.Length)));
    ///     Console.WriteLine(string.Format("Compression Percentage          = {0}%", (1.0 - ((double)encryptedThenCompressedData.Length / (double)sourceData.Length)) * 100.0));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    ///    
    /// // This code produces output similar to the following:
    /// // (this is only the end of the output stream)
    /// 
    /// // .
    /// // .
    /// // .
    /// // Ocurreret mea id aperiam consequuntur officiis ad ocurreret quaeque pri. Ut haru
    /// // m ut sea senserit fuisset sit ut sit no. Sit ne diam tincidunt referrentur no me
    /// // l cu idque ut. Summo nonumy sed oblique eripuit velit electram quo liber dapibus
    /// // . Ut vix patrioque in periculis at salutatus eruditi nobis te. Sententiae usu iu
    /// // s amet posse verterem tollit tibique cu quo. Latine ad pro pri sea mei eloquenti
    /// // am integer illum magna. Ei diceret eum no enim commodo mel quo virtute ut. Lobor
    /// // tis id tortor eget invenire sit augue deleniti usu iriure. Ut sed ex facete demo
    /// // critum no aeque a usu ex. Ad fabulas invenire malorum morbi sententiae vix an in
    /// //  an. Aut sit elitr sit fermentum vel philosophia et numquam feugiat. At mea te i
    /// // n vix docendi ei et quo urna. Fuisset vidit has pertinacia vix elit rutrum modo
    /// // vix ne.
    /// // 
    /// // Iriure in prima ne sensibus delicata eu orci aliquip. Quaeque theophrastus alien
    /// // um graeci vix mei aliquid exerci forensibus. Eum mea diam eam nunc voluptatum et
    /// //  ea exerci. Virtute has voluptaria parturient abhorreant ponderum alii aperiam n
    /// // am. Urbanitas sumo oblique singulis erat ridens vix posidonium detracto. Eu nec
    /// // ne vix eu tibique rationibus has fermentum. Id pri quo movet tincidunt id discer
    /// // e massa usu. Ad nisl audiam suscipit accusata aliquid case aeque summo. Sed tamq
    /// // uam munere posse ex intellegam et elit alia. Usu sed scelerisque primis mauris d
    /// // ebet vis vel nam. At falli et nulla pri aenean eripuit tale condimentum. Patrioq
    /// // ue at orci ne mentitum malorum id munere noster. Amet mandamus audiam cu fabulas
    /// //  dolorum eu inceptos ponderum. Patrioque mel cotidieque lobortis ius sit nec dic
    /// // at quis.
    /// // 
    /// // ------------------------------------
    /// // CORRECT WAY RESULTS:
    /// // Comparison Results    = True
    /// // Original Data Size    = 216 KB (220,678 bytes)
    /// // Compressed Data Size  = 43 KB (43,892 bytes)
    /// // Encrypted Data Size   = 43 KB (43,904 bytes)
    /// // Decrypted Data Size   = 43 KB (43,892 bytes)
    /// // Decompressed Data Size= 216 KB (220,678 bytes)
    /// // Compression Ratio     = 0.20
    /// // Compression Percentage= 80%
    /// // 
    /// // ------------------------------------
    /// // WRONG WAY RESULTS:
    /// // Original Data Size              = 216 KB (220,678 bytes)
    /// // Encrypted/Compressed Data Size  = 216 KB (220,758 bytes)
    /// //         Bigger by 80 bytes!
    /// // Compression Ratio               = 1.00036251914554
    /// // Compression Percentage          = -0.0362519145542306%
    /// // 
    /// // press anykey...
    /// </code>
    /// </example>
    [Serializable]
    public class StackableEncryptor
        : IEncryptor
    {
        #region Ctor
        private StackableEncryptor()
        {
        }
        /// <summary>
        /// Initializes a new instance of the StackableEncryptor, with a single <see cref="IEncryptorConfiguration"/>.
        /// </summary>
        /// <param name="encryptorConfiguration">The <see cref="IEncryptorConfiguration"/> to implement during encryption and decryption.</param>
        public StackableEncryptor(IEncryptorConfiguration encryptorConfiguration)
            : this()
        {
            if (encryptorConfiguration == null)
            {
                throw new ArgumentNullException("encryptorConfiguration");
            }
            _EncryptorConfigurations = new List<IEncryptorConfiguration> { encryptorConfiguration };
        }
        /// <summary>
        /// Initializes a new instance of the StackableEncryptor, with any number of <see cref="IEncryptorConfiguration"/>s.
        /// </summary>
        /// <param name="encryptorConfigurations">A list of <see cref="IEncryptorConfiguration"/>s to implement during encryption and decryption.</param>
        public StackableEncryptor(IEnumerable<IEncryptorConfiguration> encryptorConfigurations)
            : this()
        {
            if (encryptorConfigurations == null)
            {
                throw new ArgumentNullException("encryptorConfigurations");
            }
            _EncryptorConfigurations = new List<IEncryptorConfiguration>(encryptorConfigurations);
        }
        /// <summary>
        /// Instantiates an new instance with the data from the <paramref name="configuration"/>.
        /// </summary>
        /// <param name="configuration">The configuration to use to populate the new instance.</param>
        public StackableEncryptor(Configuration.IConfigurationGroup configuration)
            : this()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            if (configuration.Key != "Encryptor")
            {
                throw new ArgumentException($"Wrong configuration. Configuration Key must equal \"Encryptor\". Configuration Key={configuration.Key}", nameof(configuration));
            }
            // EncryptorConfigurations 
            if (!configuration.ContainsKey("EncryptorConfigurations"))
            {
                throw new ArgumentException($"Configuration missing subgroup. Configuration must have subgroup: \"EncryptorConfigurations\".", nameof(configuration));
            }
            _EncryptorConfigurations = new List<IEncryptorConfiguration>();
            foreach (var item in configuration["EncryptorConfigurations"])
            {
                // TODO: change this configuration item name to HashAlgorithmType to be consistent
                // HashAlgorithm
                var hashAlgorithmType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "HashAlgorithm", typeof(Type)).Value);
                // Salt
                var saltValue = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "Salt", typeof(string)).Value;
                byte[] salt = Convert.FromBase64String(saltValue);
                // PasswordSaltHash
                var passwordSaltHashValue = (string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "PasswordSaltHash", typeof(string)).Value;
                byte[] passwordSaltHash = Convert.FromBase64String(passwordSaltHashValue);
                // SymmetricAlgorithmType
                Type symmetricAlgorithmType = Type.GetType((string)Core.Configuration.ConfigurationHelper.FindConfigurationItem(configuration, "SymmetricAlgorithmType", typeof(Type)).Value);
                ISaltedPassword saltedPassword = new SaltedPassword(hashAlgorithmType, passwordSaltHash, salt);
                var dude = new EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
                _EncryptorConfigurations.Add(dude);
            }
        }
        #endregion
        #region Private Fields
        List<IEncryptorConfiguration> _EncryptorConfigurations = null;
        #endregion
        #region IEncryptor Methods
        /// <summary>
        /// Encrypts and returns the <paramref name="source"/> byte array , using the any number of <see cref="System.Security.Cryptography.SymmetricAlgorithm"/>s, with accompanying passwords and salts.
        /// </summary>
        /// <param name="source">The byte array to encrypt.</param>
        /// <returns>The encrypted <paramref name="source"/> byte array.</returns>
        public byte[] Encrypt(byte[] source)
        {
            foreach (var config in LiveEncryptorConfigurations(true))
            {
                source = dodSON.Core.Cryptography.CryptographyHelper.Encrypt(source, config);
            }
            return source;
        }
        /// <summary>
        /// Decrypts and returns the <paramref name="source"/> byte array , using the any number of <see cref="System.Security.Cryptography.SymmetricAlgorithm"/>s, with accompanying passwords and salts.
        /// </summary>
        /// <param name="source">The byte array to decrypt.</param>
        /// <returns>The decrypted <paramref name="source"/> byte array.</returns>
        public byte[] Decrypt(byte[] source)
        {
            foreach (var config in LiveEncryptorConfigurations(false))
            {
                source = dodSON.Core.Cryptography.CryptographyHelper.Decrypt(source, config);
            }
            return source;
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
                var result = new Configuration.ConfigurationGroup("Encryptor");
                result.Items.Add("Type", this.GetType(), typeof(Type));
                // 
                result.Add("EncryptorConfigurations");
                var encryptorCounter = 0;
                foreach (var item in _EncryptorConfigurations)
                {
                    var subGroupName = "Encryptor" + (++encryptorCounter).ToString("00");
                    result["EncryptorConfigurations"].Add(subGroupName);

                    // TODO: change this configuration item name to HashAlgorithmType to be consistent
                    result["EncryptorConfigurations"][subGroupName].Items.Add("HashAlgorithm", item.SaltedPassword.HashAlgorithmType, item.SaltedPassword.HashAlgorithmType.GetType());
                    result["EncryptorConfigurations"][subGroupName].Items.Add("Salt", item.SaltedPassword.Salt, item.SaltedPassword.Salt.GetType());
                    result["EncryptorConfigurations"][subGroupName].Items.Add("PasswordSaltHash", item.SaltedPassword.PasswordSaltHash, item.SaltedPassword.PasswordSaltHash.GetType());
                    result["EncryptorConfigurations"][subGroupName].Items.Add("SymmetricAlgorithmType", item.SymmetricAlgorithmType, item.SymmetricAlgorithmType.GetType());
                }
                return result;
            }
        }
        #endregion
        #region Private Methods
        private IEnumerable<IEncryptorConfiguration> LiveEncryptorConfigurations(bool forward)
        {
            // return all, non-null, IEncryptorConfigurations either in forward or reverse order
            if (forward)
            {
                for (int i = 0; i < _EncryptorConfigurations.Count; i++)
                {
                    if (_EncryptorConfigurations[i] != null)
                    {
                        yield return _EncryptorConfigurations[i];
                    }
                }
            }
            else
            {
                for (int i = _EncryptorConfigurations.Count - 1; i >= 0; i--)
                {
                    if (_EncryptorConfigurations[i] != null)
                    {
                        yield return _EncryptorConfigurations[i];
                    }
                }
            }
        }
        #endregion
    }
}
