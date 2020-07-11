using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cryptography
{
    /// <summary>
    /// Common cryptographic methods.
    /// </summary>
    /// <example>
    /// For the following examples, be sure to add the following line to the using group at the top of the .cs file. 
    /// This will ensure you can access the System.Security.SecureString Extensions <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendBytes"/> and
    /// <see cref="dodSON.Core.Cryptography.CryptographyExtensions.AppendChars"/>.
    /// <code>
    /// using dodSON.Core.Cryptography;
    /// </code>
    /// <para>
    /// This example will first demonstrate how to create a <see cref="System.Security.SecureString"/> and a
    /// <see cref="dodSON.Core.Cryptography.SaltedPassword"/>. Then it will create a copy of the 
    /// <see cref="System.Security.SecureString"/>, using the <see cref="dodSON.Core.Cryptography.CryptographyHelper.SecureStringToByteArray"/>
    /// and the <see cref="dodSON.Core.Cryptography.CryptographyHelper.ByteArrayToSecureString"/> methods, 
    /// use the copy to create a second <see cref="dodSON.Core.Cryptography.SaltedPassword"/> then use the 
    /// original <see cref="dodSON.Core.Cryptography.SaltedPassword"/> to encrypt the data and 
    /// the second <see cref="dodSON.Core.Cryptography.SaltedPassword"/> to decrypt it. 
    /// Successful decryption is proof that the original <see cref="System.Security.SecureString"/> was correctly duplicated.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create source text and convert to a byte array
    ///     var source = dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(64, true);
    ///     var sourceData = System.Text.Encoding.Unicode.GetBytes(source);
    ///     
    ///     // create a System.Security.SecureString
    ///     // NOTE:    DO NOT store passwords in strings like below. This password is embedded in the executable, as a string, in the clear!
    ///     //          Be sure the source of password information is secure and transitory.
    ///     var passwordCharArray = "BAD-Pa$$W0rd".ToCharArray();
    ///     var passwordSecureStr1 = new System.Security.SecureString();
    ///     passwordSecureStr1.AppendChars(passwordCharArray, true);
    ///     
    ///     // copy the System.Security.SecureString to new System.Security.SecureString
    ///     var passwordByteArray = dodSON.Core.Cryptography.CryptographyHelper.SecureStringToByteArray(passwordSecureStr1);
    ///     var passwordSecureStr2 = dodSON.Core.Cryptography.CryptographyHelper.ByteArrayToSecureString(passwordByteArray, true);
    ///     
    ///     // create a cryptographically random byte array
    ///     var salt = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(64);
    ///     
    ///     // create two salted passwords
    ///     var saltedPassword1 = new dodSON.Core.Cryptography.SaltedPassword(new System.Security.Cryptography.SHA512CryptoServiceProvider(),
    ///                                                                                passwordSecureStr1,
    ///                                                                                salt);
    ///     var saltedPassword2 = new dodSON.Core.Cryptography.SaltedPassword(new System.Security.Cryptography.SHA512CryptoServiceProvider(),
    ///                                                                                passwordSecureStr2,
    ///                                                                                salt);
    ///                                                                                
    ///     // create two encryptors
    ///     var symmetricAlgorithmType = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///     var encryptorConfiguration1 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword1, symmetricAlgorithmType);
    ///     var encryptor1 = new dodSON.Core.Cryptography.StackableEncryptor(encryptorConfiguration1);
    ///     var encryptorConfiguration2 = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword2, symmetricAlgorithmType);
    ///     var encryptor2 = new dodSON.Core.Cryptography.StackableEncryptor(encryptorConfiguration2);
    ///     
    ///     // encrypting with encryptor1 and decrypting with encryptor2 will demonstrate the successful copy of the System.Security.SecureString
    ///     // encrypt data
    ///     var encryptedData = encryptor1.Encrypt(sourceData);
    ///     
    ///     // decrypt encrypted-data
    ///     var decryptedData = encryptor2.Decrypt(encryptedData);
    ///     
    ///     // compare original data with transformed data
    ///     var isSame = sourceData.SequenceEqual(decryptedData);
    ///     
    ///     // validate the two salted passwords; this also demonstrates the successful copy of the System.Security.SecureString
    ///     var isSame2 = saltedPassword1.IsValid(saltedPassword2.PasswordSaltHash);
    ///     
    ///     // display results
    ///     Console.WriteLine(source);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine(string.Format("Encryption/Decryption Results= {0}", isSame));
    ///     Console.WriteLine(string.Format("SaltedPasswords Test Results= {0}", isSame2));
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("passwordCharArray= " + dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString((from x in passwordCharArray
    ///                                                                                                                         select (byte)x).ToArray()));
    ///     Console.WriteLine("passwordByteArray= " + dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(passwordByteArray));
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("Original Data Size = {0:N0}", sourceData.Length));
    ///     Console.WriteLine(string.Format("Encrypted Data Size= {0:N0}", encryptedData.Length));
    ///     Console.WriteLine(string.Format("Decrypted Data Size= {0:N0}", decryptedData.Length));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// // (only the end of the output stream is displayed...)
    ///
    /// // .
    /// // .
    /// // .
    /// // Contentiones aeterno felis nibh scriptorem adhuc eripuit in mel accumsan dolor a
    /// // t an. Alterum usu vis phaedrum eu dolor vulputate vim vel dicant ad condimentum
    /// // blandit. Quis mea his explicari sea munere ligula quo ut integer placerat deleni
    /// // ti sit. Nibh at ullamcorper has at forensibus antiopam quod placerat massa optio
    /// // n te copiosae. No liberavisse civibus ei principes oblique mel ridens has ne mas
    /// // sa eu ei. Fugit non diam feugiat intellegat per verear electram modo aenean ad a
    /// // d ex. Adhuc oblique eos auctor choro principes consectetuer mei ei eam sit dolor
    /// // um vitae. Est aliquam mei quis ancillae interpretaris aliquam posuere vel vis pr
    /// // o nunc volutpat. Ut ad justo ius platonem nunc tincidunt dicam tacimates torquat
    /// // os exerci delenit aenean. Nulla iriure consectetuer oporteat sed elementum felis
    /// //  accusamus sonet perpetua stet putent ex.
    /// // 
    /// // Sint auctor ea sed constituto qui ad mei. Singulis minimum risus te eum scaevola
    /// //  mediocritatem euismod. Cum eu suscipit sit percipit has quo ex. Verear ferri re
    /// // pudiare lacinia cu repudiandae dictas at. Doming has per sodales mucius iuvaret
    /// // mel mi. Sed in cu erat usu dicunt cu deleniti. Persecuti bonorum vulputate meis
    /// // pro theophrastus scribentur tibique. Vivendo adversarium cu adipisci ut nunc viv
    /// // endum laoreet.
    /// // 
    /// // Nostro pri eam corpora mea iusto accusam facilis. Minim vim eam nec facete eu pa
    /// // trioque salutatus. Malis ea eu his id malorum aliquid impedit. Ac ea non quodsi
    /// // comprehensam te qui equidem. Percipitur dolorum dignissim ut feugiat probatus ex
    /// //  convenire. Solet movet tortor non cum feugiat civibus ex. Has iisque duo nulla
    /// // aenean convenire brute evertitur. Percipitur ridens eos accusamus in dicta ea au
    /// // dire. Vel quo adipiscing nam definitionem nec euripidis proin.
    /// // 
    /// // ------------------------------------
    /// // Encryption/Decryption Results= True
    /// // SaltedPasswords Test Results= True
    /// // 
    /// // ------------------------------------
    /// // passwordCharArray= 000000000000000000000000
    /// // passwordByteArray= 000000000000000000000000
    /// // ------------------------------------
    /// // 
    /// // Original Data Size = 96,318
    /// // Encrypted Data Size= 96,320
    /// // Decrypted Data Size= 96,318
    /// // 
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/><br/>
    /// The following example creates 8 cryptographically random hashes, selects one at random, uses it to create
    /// an encryption configuration which is then used to encrypt and decrypt text data and test the results.
    /// <br/><br/>
    /// Create a console application and add the following code:
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create cryptographically random data
    ///     var source = dodSON.Core.Cryptography.CryptographyHelper.GenerateCryptographicallyRandomArray(1024);
    ///     
    ///     // create 8 hashes from each 128 byte segment of the cryptographically random data
    ///     var lengthOfHashSource = 128;
    ///     var hashProvider = new System.Security.Cryptography.SHA512Managed();
    ///     List&lt;byte[]&gt; hashes = new List&lt;byte[]&gt;();
    ///     using (var ms = new System.IO.MemoryStream(source))
    ///     {
    ///         for (int i = 0; i &lt; source.Length; i += lengthOfHashSource)
    ///         {
    ///             hashes.Add(dodSON.Core.Cryptography.CryptographyHelper.ComputeHash(ms, lengthOfHashSource, hashProvider));
    ///         }
    ///     }
    ///     
    ///     // use an random hash to create a System.Security.SecureString
    ///     var secureStr = new System.Security.SecureString();
    ///     secureStr.AppendBytes(hashes[(new Random()).Next(0, 7)], System.Text.Encoding.Unicode, true);
    ///     
    ///     // create a salted password
    ///     var saltedPassword = new dodSON.Core.Cryptography.SaltedPassword(hashProvider, secureStr, 64);
    ///     
    ///     // create encryptor configuration
    ///     var symmetricAlgorithmType = typeof(System.Security.Cryptography.TripleDESCryptoServiceProvider);
    ///     var encryptionConfig = new dodSON.Core.Cryptography.EncryptorConfiguration(saltedPassword, symmetricAlgorithmType);
    ///     
    ///     // encrypt and decrypt data
    ///     var originalData = System.Text.Encoding.Unicode.GetBytes(
    ///                               dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(7, true));
    ///     var encryptedData = dodSON.Core.Cryptography.CryptographyHelper.Encrypt(originalData, encryptionConfig);
    ///     var decryptedData = dodSON.Core.Cryptography.CryptographyHelper.Decrypt(encryptedData, encryptionConfig);
    ///     var isSame = originalData.SequenceEqual(decryptedData);
    ///     
    ///     // display results
    ///     Console.WriteLine("The hash with all zeros was the randomly selected hash used");
    ///     Console.WriteLine("to create the System.Security.SecureString. Setting the");
    ///     Console.WriteLine("autoClearIncomingByteArray to true on the System.Security.SecureString");
    ///     Console.WriteLine("extension AppendBytes(), clears the source byte array");
    ///     Console.WriteLine("after injecting it into the System.Security.SecureString.");
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine();
    ///     foreach (var item in hashes)
    ///     {
    ///         Console.WriteLine(string.Format("{0}", dodSON.Core.Common.ByteArrayHelper.ConvertByteArrayToHexString(item)));
    ///         Console.WriteLine();
    ///     }
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine(string.Format("Encryption/Decryption Results= {0}", isSame));
    ///     Console.WriteLine(string.Format("Original Data Size = {0:N0}", originalData.Length));
    ///     Console.WriteLine(string.Format("Encrypted Data Size= {0:N0}", encryptedData.Length));
    ///     Console.WriteLine(string.Format("Decrypted Data Size= {0:N0}", decryptedData.Length));
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following: 
    ///
    /// // The hash with all zeros was the randomly selected hash used
    /// // to create the System.Security.SecureString. Setting the
    /// // autoClearIncomingByteArray to true on the System.Security.SecureString
    /// // extension AppendBytes(), clears the source byte array.
    /// // after injecting it into the System.Security.SecureString.
    /// // ------------------------
    /// //
    /// // 6C3529FDB82EAE42AFAA9134F96892FCD6188CE9B84B7334E71D0D5BC28529E23B870C4CEDAF5424
    /// // 75838141B28483900458AC90C1B1D6D43284C9BFD5D3D549
    /// // 
    /// // E89045C2CDF9213D6A5FF1B5D422BEE7149CAB4E297F005B35538A673CB8BFB08D33A3B7E74346C2
    /// // 93FDF7990DD539A86C5CD79479E0424FF88D32C49703F13D
    /// // 
    /// // 00000000000000000000000000000000000000000000000000000000000000000000000000000000
    /// // 000000000000000000000000000000000000000000000000
    /// // 
    /// // 9DB6ECD7A8251062F173EA603E38CAFCD2128353011DDA40870915F5F1FAA85F76A51EC64A0D24F5
    /// // A2BAE6BEFB52A461917EB76FC0F334AFC8BF0707189F8B39
    /// // 
    /// // B7619E65521A501AB1FB72F77A51F76D3AF4C0A19FA75501F021769762619B3AD2A844A91FB8BB64
    /// // 2B328244910747B697FB627CB370E1B2E67ABBB942525CD3
    /// // 
    /// // 956585B872F8B1BA1752463A9205539957C3039C218C0A33BDF9C90D065C3644615E81478350B66E
    /// // AD79AADDC013B641AFAFA9185C0349BAC7FF0C8C11D83A27
    /// // 
    /// // F74080901BD239FC9159DD33816FA210DFF99E957EB85071EA8A0B99DC76CADBA694784597BEBA9C
    /// // 85C885DC6F273FE3B5F80AFC4F5E4BEA1AD85B392AAED44B
    /// // 
    /// // A63BA4E4FEE92F7C19B6C9985B01A2D38C0C09E9CFA15C508C21E7F03366635BC855F862040255BD
    /// // 2B68FFE330C4E354BD49BBFF8CCCF73266495887B1D12DFD
    /// // 
    /// // ------------------------
    /// // Encryption/Decryption Results= True
    /// // Original Data Size = 10,688
    /// // Encrypted Data Size= 10,696
    /// // Decrypted Data Size= 10,688
    /// // ------------------------
    /// // press anykey... 
    /// </code>
    /// </example>
    public static class CryptographyHelper
    {
        #region Public Static Fields
        /// <summary>
        /// RSA Asymmetric Cryptographic Algorithm default key size in bytes. 
        /// Currently equals 2048
        /// </summary>
        public static readonly int DefaultRSAKeyLengthInBits = 2048;
        /// <summary>
        /// The default, safe, value to use for the size of each byte array when using the <see cref="PrepareForTransport(byte[], string)"/> and <see cref="RestoreFromTransport(List{byte[]}, string)"/> functions.
        ///  Currently equals 80
        /// </summary>
        public static readonly int DefaultTransportChuckSize = 80;
        #endregion     
        #region Public Static Methods
        /// <summary>
        /// Generates a Public and a Private key pair used for asymmetrical encryption. (Item1=PublicKey, Item2=PrivateKey)
        /// </summary>
        /// <returns>A <see cref="Tuple{T1, T2}"/> containing the generated Public and Private Keys, respectively.</returns>
        public static Tuple<string, string> GeneratePublicPrivateKeys()
        {
            System.Security.Cryptography.RSACryptoServiceProvider encryptor = new System.Security.Cryptography.RSACryptoServiceProvider(DefaultRSAKeyLengthInBits);
            return Tuple.Create(encryptor.ToXmlString(false), encryptor.ToXmlString(true));
        }
        /// <summary>
        /// Prepares data by encrypting it into smaller chucks.
        /// </summary>
        /// <param name="data">The data to prepare.</param>
        /// <param name="xmlPublicKey">An XML string representation of a Public Key.</param>
        /// <returns>A list of byte arrays encrypted and split into smaller chucks.</returns>
        public static List<byte[]> PrepareForTransport(byte[] data, string xmlPublicKey)
        {
            var list = new List<byte[]>();
            // create encryptor from public key
            System.Security.Cryptography.RSACryptoServiceProvider transportEncryptor = new System.Security.Cryptography.RSACryptoServiceProvider(DefaultRSAKeyLengthInBits);
            transportEncryptor.FromXmlString(xmlPublicKey);
            //
            foreach (var item in Common.ByteArrayHelper.SplitByteArray(data, DefaultTransportChuckSize))
            {
                list.Add(transportEncryptor.Encrypt(item, true));
            }
            return list;
        }
        /// <summary>
        /// Restores data by decrypting and joining all of the parts together.
        /// </summary>
        /// <param name="parts">A list of byte arrays encrypted and split into smaller chucks.</param>
        /// <param name="xmlPrivateKey">An XML string representation of a Private Key.</param>
        /// <returns>A byte array decrypted and reassembled from the <paramref name="parts"/>.</returns>
        public static byte[] RestoreFromTransport(List<byte[]> parts, string xmlPrivateKey)
        {
            var list = new List<byte[]>();
            // create encryptor from private key
            System.Security.Cryptography.RSACryptoServiceProvider transportEncryptor = new System.Security.Cryptography.RSACryptoServiceProvider(DefaultRSAKeyLengthInBits);
            transportEncryptor.FromXmlString(xmlPrivateKey);
            //
            foreach (var item in parts)
            {
                list.Add(transportEncryptor.Decrypt(item, true));
            }
            return Common.ByteArrayHelper.RestoreByteArray(list);
        }
        /// <summary>
        /// Generates a byte array, of <paramref name="length"/> size, filled with cryptographically random values.
        /// </summary>
        /// <param name="length">Sets the length of the byte array returned.</param>
        /// <returns>A byte array, of <paramref name="length"/> size, containing cryptographically random values.</returns>
        public static byte[] GenerateCryptographicallyRandomArray(int length)
        {
            // check for errors
            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException("length", "Parameter length must be greater than zero. (length > 0)");
            }
            // create byte array of specified length
            var array = new byte[length];
            // create random cryptographic service
            var hashProvider = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // fill the byte array
            hashProvider.GetBytes(array);
            // return results
            return array;
        }
        /// <summary>
        /// Will convert a <see cref="System.Security.SecureString"/> instance into a byte array.
        /// </summary>
        /// <param name="source">The System.Security.SecureString instance to convert.</param>
        /// <returns>A byte array containing the values in the <see cref="System.Security.SecureString"/> instance.</returns>
        /// <remarks>
        /// <para>This function will clear memory of any values related to the password, except for the byte array returned.</para>
        /// <para><b>Important:</b> It is incumbent on the developer using this function to clear the returned byte array after using it. 
        /// Otherwise, it will contain the characters of the password (as bytes) in the clear and it will remain in memory until the garbage collector frees the memory and the memory is overwritten. 
        /// Also note, the garbage collector freeing the memory does not clear the memory; the memory will remain as is until it is allocated and written over.</para>
        /// </remarks>
        public static byte[] SecureStringToByteArray(System.Security.SecureString source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            byte[] result = new byte[source.Length];
            char[] workingArray = new char[source.Length];
            IntPtr sourcePointer = IntPtr.Zero;
            try
            {
                // creating an Unicode block of memory containing the contents of the System.Security.SecureString
                sourcePointer = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(source);
                // copy Unicode block of memory into the local Unicode char array
                System.Runtime.InteropServices.Marshal.Copy(sourcePointer, workingArray, 0, source.Length);
                // convert the local Unicode char array into a byte array
                for (int i = 0; i < workingArray.Length; i++)
                {
                    result[i] = (byte)workingArray[i];
                }
            }
            finally
            {
                // clear the contents of the Unicode block of memory and release its resources
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocAnsi(sourcePointer);
                // clear the contents of the working array
                Array.Clear(workingArray, 0, workingArray.Length);
            }
            // return results
            return result;
        }
        /// <summary>
        /// Will convert a byte array into a <see cref="System.Security.SecureString"/>.
        /// </summary>
        /// <param name="source">A byte array to convert into a <see cref="System.Security.SecureString"/>.</param>
        /// <param name="autoClearIncomingByteArray">If <b>true</b>, will automatically clear the contents of the incoming byte array; <b>false</b>, will leave the contents of the incoming byte array as is.</param>
        /// <returns>A <see cref="System.Security.SecureString"/> converted from the incoming byte array.</returns>
        /// <remarks>
        /// <para><b>Important:</b> It is incumbent on the developer using this function to clear the incoming byte array after using it. 
        /// Otherwise, it will contain the characters of the password (as bytes) in the clear and it will remain in memory until the garbage collector frees the memory and the memory is overwritten. 
        /// Also note, the garbage collector freeing the memory does not clear the memory; the memory will remain as is until it is allocated and written over.</para>
        /// </remarks>
        public static System.Security.SecureString ByteArrayToSecureString(byte[] source,
                                                                           bool autoClearIncomingByteArray)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            // create result System.Security.SecureString
            var result = new System.Security.SecureString();
            // process byte array
            foreach (var byt in source)
            {
                result.AppendChar((char)byt);
            }
            // clear incoming byte array?
            if (autoClearIncomingByteArray)
            {
                Array.Clear(source, 0, source.Length);
            }
            // return results
            return result;
        }
        /// <summary>
        /// Will generate a hash for the given byte array using the given <see cref="System.Security.Cryptography.HashAlgorithm"/>.
        /// </summary>
        /// <param name="source">The byte array to create the hash from.</param>
        /// <param name="hashProvider">The <see cref="System.Security.Cryptography.HashAlgorithm"/> used to compute the hash.</param>
        /// <returns>A byte array containing the hash.</returns>
        public static byte[] ComputeHash(byte[] source,
                                         System.Security.Cryptography.HashAlgorithm hashProvider)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (hashProvider == null)
            {
                throw new ArgumentNullException("hashProvider");
            }
            return hashProvider.ComputeHash(source);
        }
        /// <summary>
        /// Will generate a hash for the given string using the given <see cref="System.Security.Cryptography.HashAlgorithm"/>.
        /// </summary>
        /// <param name="source">The string to create the hash from.</param>
        /// <param name="hashProvider">The <see cref="System.Security.Cryptography.HashAlgorithm"/> used to compute the hash.</param>
        /// <returns>A byte array containing the hash.</returns>
        public static byte[] ComputeHash(string source,
                                         System.Security.Cryptography.HashAlgorithm hashProvider)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (hashProvider == null)
            {
                throw new ArgumentNullException("hashProvider");
            }
            return ComputeHash(System.Text.Encoding.Unicode.GetBytes(source), hashProvider);
        }
        /// <summary>
        /// Will read the <paramref name="length"/> number of bytes from the given stream, then convert that byte array into a hash.
        /// </summary>
        /// <param name="source">The stream to read bytes from.</param>
        /// <param name="length">The number of bytes to read from the stream.</param>
        /// <param name="hashProvider">The <see cref="System.Security.Cryptography.HashAlgorithm"/> used to compute the hash.</param>
        /// <returns>A byte array containing the hash.</returns>
        public static byte[] ComputeHash(System.IO.Stream source,
                                         int length,
                                         System.Security.Cryptography.HashAlgorithm hashProvider)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (!source.CanRead)
            {
                throw new ArgumentException("The parameter source represents an unreadable stream.", "source");
            }
            if (length < 0)
            {
                throw new ArgumentException("The parameter length cannot be less than zero. (0 >= length)", "length");
            }
            if (hashProvider == null)
            {
                throw new ArgumentNullException("hashProvider");
            }
            var buffer = new byte[length];
            source.Read(buffer, 0, length);
            return ComputeHash(buffer, hashProvider);
        }
        /// <summary>
        /// Will read the all the bytes from the given stream, then convert that byte array into a hash.
        /// </summary>
        /// <param name="source">The stream to read bytes from.</param>
        /// <param name="hashProvider">The <see cref="System.Security.Cryptography.HashAlgorithm"/> used to compute the hash.</param>
        /// <returns>A byte array containing the hash.</returns>
        public static byte[] ComputeHash(System.IO.Stream source,
                                         System.Security.Cryptography.HashAlgorithm hashProvider) => ComputeHash(source, (int)source.Length, hashProvider);
        /// <summary>
        /// Will encrypt the <paramref name="source"/> byte array using the given <paramref name="encryptorConfiguration"/>.
        /// </summary>
        /// <param name="source">The byte array to encrypt.</param>
        /// <param name="encryptorConfiguration">The encryption configuration used to encrypt the <paramref name="source"/> byte array.</param>
        /// <returns>The <paramref name="source"/> byte array encrypted using the given <paramref name="encryptorConfiguration"/>.</returns>
        public static byte[] Encrypt(byte[] source,
                                     IEncryptorConfiguration encryptorConfiguration)
        {
            // TODO: Fix This. Problem Id= 3333E15BFE8746CEAA1D5664BFFD0E18
            //      What's happening here is that I'm using the IEncryptorConfiguration.PasswordSaltHash as the PASSWORD for the PasswordDeriveBytes type; is that right?
            //      According to Microsoft documentation, it is using SHA1 by default.
            // Must Rethink This!

            if ((source == null) || (source.Length == 0))
            {
                return source;
            }
            if (encryptorConfiguration == null)
            {
                return source;
            }
            System.Security.Cryptography.PasswordDeriveBytes passwordDerivedBytes = null;
            using (var ec = encryptorConfiguration.SymmetricAlgorithm)
            {
                passwordDerivedBytes = new System.Security.Cryptography.PasswordDeriveBytes(encryptorConfiguration.SaltedPassword.PasswordSaltHash,
                                                                                            encryptorConfiguration.SaltedPassword.Salt);
                ec.Key = passwordDerivedBytes.GetBytes(ec.KeySize / 8);
                ec.IV = passwordDerivedBytes.GetBytes(ec.BlockSize / 8);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new System.Security.Cryptography.CryptoStream(ms,
                                                                                  ec.CreateEncryptor(),
                                                                                  System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(source, 0, source.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }
        /// <summary>
        /// Will decrypt the <paramref name="source"/> byte array using the given <paramref name="encryptorConfiguration"/>.
        /// </summary>
        /// <param name="source">The byte array to decrypt.</param>
        /// <param name="encryptorConfiguration">The encryption configuration used to decrypt the <paramref name="source"/> byte array.</param>
        /// <returns>The <paramref name="source"/> byte array decrypted using the given <paramref name="encryptorConfiguration"/>.</returns>
        public static byte[] Decrypt(byte[] source,
                                     IEncryptorConfiguration encryptorConfiguration)
        {
            // TODO: Fix This. Problem Id= 3333E15BFE8746CEAA1D5664BFFD0E18
            //      What's happening here is that I'm using the IEncryptorConfiguration.PasswordSaltHash as the PASSWORD for the PasswordDeriveBytes type; that's not right!
            //      According to Microsoft documentation, it is using SHA1 by default.
            // Must Rethink This!

            if ((source == null) || (source.Length == 0))
            {
                return source;
            }
            if (encryptorConfiguration == null)
            {
                return source;
            }
            System.Security.Cryptography.PasswordDeriveBytes passwordDerivedBytes = null;
            using (var ec = encryptorConfiguration.SymmetricAlgorithm)
            {
                passwordDerivedBytes = new System.Security.Cryptography.PasswordDeriveBytes(encryptorConfiguration.SaltedPassword.PasswordSaltHash,
                                                                                            encryptorConfiguration.SaltedPassword.Salt);
                ec.Key = passwordDerivedBytes.GetBytes(ec.KeySize / 8);
                ec.IV = passwordDerivedBytes.GetBytes(ec.BlockSize / 8);
                using (var ms = new System.IO.MemoryStream())
                {
                    using (var cs = new System.Security.Cryptography.CryptoStream(ms,
                                                                                  ec.CreateDecryptor(),
                                                                                  System.Security.Cryptography.CryptoStreamMode.Write))
                    {
                        cs.Write(source, 0, source.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }
        #endregion
    }
}
