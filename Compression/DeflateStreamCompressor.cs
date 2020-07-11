using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Compression
{
    /// <summary>
    /// Implements the <see cref="ICompressor"/> interface, using the <see cref="System.IO.Compression.DeflateStream"/> algorithm to compress and decompress the byte arrays.
    /// </summary>
    /// <example>
    /// The following code example will create data, compresses it, decompresses it and compare the results.
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create data
    ///     var source = dodSON.Core.Common.LoremIpsumGenerator.GenerateChapter(100, true);
    ///     var data = System.Text.Encoding.Unicode.GetBytes(source);
    ///     
    ///     // create compressor
    ///     var compressor = new dodSON.Core.Compression.DeflateStreamCompressor();
    ///     
    ///     // compress data
    ///     var compressedData = compressor.Compress(data);
    ///     
    ///     // decompress compressed-data
    ///     var uncompressedData = compressor.Decompress(compressedData);
    ///     
    ///     // compare original data with transformed data
    ///     var isSame = data.SequenceEqual(uncompressedData);
    ///     
    ///     // display results
    ///     Console.WriteLine(source);
    ///     Console.WriteLine();
    ///     Console.WriteLine("--------------------------------");
    ///     Console.WriteLine();
    ///     Console.WriteLine(string.Format("Compression/Decompression Results= {0}", isSame));
    ///     Console.WriteLine(string.Format("Original Data Size= {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(data.Length), data.Length));
    ///     Console.WriteLine(string.Format("Compressed Data Size= {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(compressedData.Length), compressedData.Length));
    ///     Console.WriteLine(string.Format("Decompressed Data Size= {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(uncompressedData.Length), uncompressedData.Length));
    ///     Console.WriteLine(string.Format("Compression Ratio= {0:N2}", ((double)compressedData.Length / (double)uncompressedData.Length)));
    ///     Console.WriteLine(string.Format("Compression Percentage= {0:N0}%", (1.0 - ((double)compressedData.Length / (double)uncompressedData.Length)) * 100.0));
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
    /// // Primis repudiandae eu interdum interpretaris ea oporteat suscipit dissentiet pel
    /// // lentesque. Viderer cotidieque tractatos menandri reprimique lorem postea delectu
    /// // s at mi. Repudiandae ex saepe usu an expetendis mel nonumes mei an. Omnis tation
    /// //  ei praesent te expetendis delectus pro mel quo. Dissentiet sententiae no qualis
    /// // que wisi vel aeque gloriatur possit mazim. Nec tamquam repudiare has ea amet imp
    /// // edit eam debitis dictas. Expetenda vivendo nec pede torquent legere posse nibh e
    /// // xplicari et.
    /// // 
    /// // Mea risus sensibus mei has ad ridens integre. Percipitur repudiandae ornare luci
    /// // lius lorem ei detraxit nam. Civibus putant fierent at eloquentiam iudicabit per
    /// // minimum. Ea disputationi donec at in mundi leo mea. Mei wisi salutandi odio summ
    /// // o molestie in nulla. Omnis ferri maecenas erroribus vis agam libris mei. Adversa
    /// // rium ea lucilius imperdiet solum duo eum risus. Te eu zril sit quaerendum movet
    /// // omnesque vitae. Ex eum an simul meis albucius dissentiet ipsum. Vivendum an has
    /// // facer placerat proin putant maecenas. Facilisi duo verterem case in et quaestio
    /// // tation.
    /// // 
    /// // Liberavisse quam quo quam nobis placerat his delicata appellantur. Aperiri natum
    /// //  illum commune ex ut ex an quam. Exerci senserit augue his tibique eum tortor ve
    /// // stibulum vide. Virtute te verear eros quis vestibulum albucius auctor eos. Nonum
    /// // es deleniti inimicus constituto at definitionem vocibus appareat justo. Ferri ea
    /// // m libero his phaedrum non postulant voluptua phaedrum. Eu purto explicari molest
    /// // ie non mea ullamcorper constituam inceptos. Accumsan iusto oportere est etiam cu
    /// // m liberavisse vim pri. An consul pro delectus no adhuc at eu cu. Mel vis consul
    /// // at gloriatur quo donec no pharetra.
    /// // 
    /// // --------------------------------
    /// // 
    /// // Compression/Decompression Results= True
    /// // Original Data Size= 156 KB (159,492 bytes)
    /// // Compressed Data Size= 31 KB (31,888 bytes)
    /// // Decompressed Data Size= 156 KB (159,492 bytes)
    /// // Compression Ratio= 0.20
    /// // Compression Percentage= 80%
    /// // 
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/><br/>
    /// This example will demonstrate how to stack compression and encryption. Then it will demonstrate how data should be compressed before it is encrypted; 
    /// in otherwords, compressing encrypted data has, historically, resulted in more data returned than entered.
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
    public class DeflateStreamCompressor
        : ICompressor
    {
        #region Private Readonly Constants
        /// <summary>
        /// The size of the internal buffer used by the decompression algorithm.
        /// </summary>
        private static readonly int _DecompressionReadBufferByteSize_ = 1024;
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the DeflateStreamCompressor.
        /// </summary>
        public DeflateStreamCompressor() { }
        #endregion
        /// <summary>
        /// Compresses and returns the <paramref name="source"/> byte array using the <see cref="System.IO.Compression.DeflateStream"/> algorithm.
        /// </summary>
        /// <param name="source">The byte array to compress.</param>
        /// <returns>The compressed <paramref name="source"/> byte array.</returns>
        public byte[] Compress(byte[] source)
        {
            if (source == null) { return source; }
            byte[] result = null;
            using (var ms = new System.IO.MemoryStream())
            {
                using (var ds = new System.IO.Compression.DeflateStream(ms,
                                                                        System.IO.Compression.CompressionMode.Compress,
                                                                        true))
                {
                    ds.Write(source, 0, source.Length);
                    ds.Flush();
                    ds.Close();
                }
                result = ms.ToArray();
                ms.Close();
            }
            return result;
        }
        /// <summary>
        /// Decompresses and returns the <paramref name="source"/> byte array using the <see cref="System.IO.Compression.DeflateStream"/> algorithm.
        /// </summary>
        /// <param name="source">The byte array to decompress.</param>
        /// <returns>The decompressed <paramref name="source"/> byte array.</returns>
        public byte[] Decompress(byte[] source)
        {
            if (source == null) { return source; }
            byte[] result = null;
            using (var ms = new System.IO.MemoryStream(source))
            {
                using (var ds = new System.IO.Compression.DeflateStream(ms,
                                                                        System.IO.Compression.CompressionMode.Decompress,
                                                                        true))
                {
                    var byteArrayPieces = new List<byte[]>();
                    var count = 0;
                    var length = 0;
                    var readBuffer = new byte[_DecompressionReadBufferByteSize_];
                    while ((count = ds.Read(readBuffer, 0, _DecompressionReadBufferByteSize_)) > 0)
                    {
                        if (count == _DecompressionReadBufferByteSize_)
                        {
                            byteArrayPieces.Add(readBuffer);
                            readBuffer = new byte[_DecompressionReadBufferByteSize_];
                        }
                        else
                        {
                            byte[] tempBuffer = new byte[count];
                            Array.Copy(readBuffer, 0, tempBuffer, 0, count);
                            byteArrayPieces.Add(tempBuffer);
                        }
                        length += count;
                    }
                    result = new byte[length];
                    var resultIndex = 0;
                    foreach (var byteArrayPiece in byteArrayPieces)
                    {
                        Array.Copy(byteArrayPiece, 0, result, resultIndex, byteArrayPiece.Length);
                        resultIndex += byteArrayPiece.Length;
                    }
                }
            }
            return result;
        }
    }
}
