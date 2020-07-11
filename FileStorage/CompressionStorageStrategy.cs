using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Determines how a file will be added to a compressed file store.
    /// </summary>
    /// <example>
    /// The following example will create a new Ionic Zip file store, instruct the compressed file store to 
    /// store files with the extension ".dat" without compression, add a few files, save it and then extract a few files.
    /// <br/><br/>
    /// <note type="note">
    /// The following example requires a reference to the <b>dodSON.Core.FileStorage.IonicZip.dll</b> assembly.
    /// </note>
    /// Create a console application and add the following code:
    /// <br/><br/>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** 
    ///     // **** IMPORTANT ****
    ///     // ****
    ///     // **** the files source directory should contain a few files, with 
    ///     // both .txt and .dat extensions, sub directories will be included
    ///     var filesSourceDirectory = @"D:\Dev\Sandbox\Example-7";
    ///     // **** the source .zip file should not exist
    ///     var sourceZipFilename = @"D:\Dev\Sandbox\Example-7.zip";
    ///     // **** the extraction directory should point to an empty directory
    ///     // **** Do not put the extraction directory in the source directory
    ///     var extractionDirectory = @"D:\Dev\Sandbox\Example-7 Extract";
    ///    
    ///     // random
    ///     var rnd = new Random();
    ///    
    ///     // create a file store
    ///     Console.WriteLine("---- Creating the File Store");
    ///     Console.WriteLine();
    ///     // this will cause the MSdotNETFileSystem to create a file in the root directory, which contains 
    ///     // all of the, known, original filenames for the files stored in the file store backend
    ///     // That being said, I should note that other implementations may store the 
    ///     // original filenames in a completely different manner
    ///     var saveOriginalFilenames = true;
    ///     // this will cause the compressed file store to store files with the extension ".dat" without compression
    ///     // periods are optional "dat" &amp;&amp; ".dat" are the same
    ///     var extensionsToStore = new string[] { "dat" };
    ///     var fileStore = new dodSON.Core.FileStorage.IonicZip.FileStore(
    ///                                     sourceZipFilename,
    ///                                     extractionDirectory,
    ///                                     saveOriginalFilenames,
    ///                                     extensionsToStore);
    ///    
    ///     // display file store information
    ///     Console.WriteLine("{0}", fileStore.BackendStorageZipFilename);
    ///     Console.WriteLine("{0} v{1}", fileStore.CompressionEngineName, fileStore.CompressionEngineVersion);
    ///     Console.WriteLine("Uncompressed Size = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.UncompressedSize), fileStore.UncompressedSize);
    ///     Console.WriteLine("Compressed Size   = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.CompressedSize), fileStore.CompressedSize);
    ///     Console.WriteLine("Compression Ratio = {0:N5} ({1:N1}%)", fileStore.CompressionRatio, (((1.0 - fileStore.CompressionRatio)) * 100.0));
    ///     Console.WriteLine();
    ///     Console.WriteLine("{0}", fileStore.ExtractionRootDirectory);
    ///     Console.WriteLine("Count= {0}", fileStore.Count);
    ///     Console.WriteLine("CountAvailableRootFiles= {0}", fileStore.CountAvailableRootFiles);
    ///     Console.WriteLine("CountDeleted= {0}", fileStore.CountDeleted);
    ///     Console.WriteLine("CountExtracted= {0}", fileStore.CountExtracted);
    ///     Console.WriteLine("CountExtractedUpdatable= {0}", fileStore.CountExtractedUpdatable);
    ///     Console.WriteLine("CountOriginal= {0}", fileStore.CountOriginal);
    ///    
    ///     // add files to the file store
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- Adding Files to the File Store");
    ///     Console.WriteLine();
    ///     foreach (var filename in System.IO.Directory.GetFiles(filesSourceDirectory, "*", System.IO.SearchOption.AllDirectories))
    ///     {
    ///         var fileInfo = new System.IO.FileInfo(filename);
    ///         var rootFilename = filename.Substring(filesSourceDirectory.Length + 1);
    ///         var rootFileLastModifiedTimeUtc = fileInfo.LastWriteTimeUtc;
    ///         var originalFilename = filename;
    ///         var storeItem = fileStore.CreateNewFileStoreItem(rootFilename, rootFileLastModifiedTimeUtc, originalFilename);
    ///         Console.WriteLine(string.Format("Adding File--&gt; {0}", rootFilename));
    ///         fileStore.Add(storeItem);
    ///     }
    ///    
    ///     // save file store
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- Saving the File Store");
    ///     Console.WriteLine();
    ///     fileStore.Save(true);
    ///    
    ///     // display file store information
    ///     Console.WriteLine("{0}", fileStore.BackendStorageZipFilename);
    ///     Console.WriteLine("{0} v{1}", fileStore.CompressionEngineName, fileStore.CompressionEngineVersion);
    ///     Console.WriteLine("Uncompressed Size = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.UncompressedSize), fileStore.UncompressedSize);
    ///     Console.WriteLine("Compressed Size   = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.CompressedSize), fileStore.CompressedSize);
    ///     Console.WriteLine("Compression Ratio = {0:N5} ({1:N1}%)", fileStore.CompressionRatio, (((1.0 - fileStore.CompressionRatio)) * 100.0));
    ///     Console.WriteLine();
    ///     Console.WriteLine("{0}", fileStore.ExtractionRootDirectory);
    ///     Console.WriteLine("Count= {0}", fileStore.Count);
    ///     Console.WriteLine("CountAvailableRootFiles= {0}", fileStore.CountAvailableRootFiles);
    ///     Console.WriteLine("CountDeleted= {0}", fileStore.CountDeleted);
    ///     Console.WriteLine("CountExtracted= {0}", fileStore.CountExtracted);
    ///     Console.WriteLine("CountExtractedUpdatable= {0}", fileStore.CountExtractedUpdatable);
    ///     Console.WriteLine("CountOriginal= {0}", fileStore.CountOriginal);
    ///    
    ///     // display file information
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- Files Loaded into the File Store");
    ///     Console.WriteLine();
    ///     foreach (dodSON.Core.FileStorage.ICompressedFileStoreItem fileItem in fileStore.GetAll())
    ///     {
    ///         Console.WriteLine(string.Format("{0,8}/{1,-8} {2,5:N1}% {3,9} {4}",
    ///                                         dodSON.Core.Common.ByteCountHelper.ToString(fileItem.UncompressedSize),
    ///                                         dodSON.Core.Common.ByteCountHelper.ToString(fileItem.CompressedSize),
    ///                                         (((1.0 - fileItem.CompressionRatio)) * 100.0),
    ///                                         fileItem.CompressionStrategy,
    ///                                         fileItem.RootFilename));
    ///     }
    ///    
    ///     // select/extract a few files as random
    ///     Console.WriteLine();
    ///     Console.WriteLine("---- Extract a few files from the File Store");
    ///     Console.WriteLine();
    ///     var numberOfItemsToAttemptToExtract = 4;
    ///     var list = fileStore.Find((dodSON.Core.FileStorage.IFileStoreItem item) =&gt;
    ///     {
    ///         if ((numberOfItemsToAttemptToExtract &gt; 0) &amp;&amp;
    ///             (rnd.NextDouble() &lt; 0.333))
    ///         {
    ///             --numberOfItemsToAttemptToExtract;
    ///             return true;
    ///         }
    ///         return false;
    ///     });
    ///    
    ///     // extract the selected files
    ///     var extractedFiles = fileStore.Extract(list, true);
    ///    
    ///     // display extracted files
    ///     Console.WriteLine("The following files should have been extracted:");
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///     foreach (var filename in extractedFiles)
    ///     {
    ///         Console.WriteLine(filename);
    ///     }
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///    
    ///     // display file store information
    ///     Console.WriteLine("{0}", fileStore.BackendStorageZipFilename);
    ///     Console.WriteLine("{0} v{1}", fileStore.CompressionEngineName, fileStore.CompressionEngineVersion);
    ///     Console.WriteLine("Uncompressed Size = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.UncompressedSize), fileStore.UncompressedSize);
    ///     Console.WriteLine("Compressed Size   = {0} ({1:N0} bytes)", dodSON.Core.Common.ByteCountHelper.ToString(fileStore.CompressedSize), fileStore.CompressedSize);
    ///     Console.WriteLine("Compression Ratio = {0:N5} ({1:N1}%)", fileStore.CompressionRatio, (((1.0 - fileStore.CompressionRatio)) * 100.0));
    ///     Console.WriteLine();
    ///     Console.WriteLine("{0}", fileStore.ExtractionRootDirectory);
    ///     Console.WriteLine("Count= {0}", fileStore.Count);
    ///     Console.WriteLine("CountAvailableRootFiles= {0}", fileStore.CountAvailableRootFiles);
    ///     Console.WriteLine("CountDeleted= {0}", fileStore.CountDeleted);
    ///     Console.WriteLine("CountExtracted= {0}", fileStore.CountExtracted);
    ///     Console.WriteLine("CountExtractedUpdatable= {0}", fileStore.CountExtractedUpdatable);
    ///     Console.WriteLine("CountOriginal= {0}", fileStore.CountOriginal);
    ///    
    ///     // 
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // ---- Creating the File Store
    /// // 
    /// // D:\Dev\Sandbox\Example-7.zip
    /// // Ionic.Zip v1.9.1.8
    /// // Uncompressed Size = 0 bytes (0 bytes)
    /// // Compressed Size   = 0 bytes (0 bytes)
    /// // Compression Ratio = 0.00000 (100.0%)
    /// // 
    /// // D:\Dev\Sandbox\Example-7 Extract
    /// // Count= 0
    /// // CountAvailableRootFiles= 0
    /// // CountDeleted= 0
    /// // CountExtracted= 0
    /// // CountExtractedUpdatable= 0
    /// // CountOriginal= 0
    /// // 
    /// // ---- Adding Files to the File Store
    /// // 
    /// // Adding File--> Bucket_1\B1.dat
    /// // Adding File--> Bucket_1\B1.txt
    /// // Adding File--> Bucket_1\Slot_1\B1_S1.dat
    /// // Adding File--> Bucket_1\Slot_1\B1_S1.txt
    /// // Adding File--> Bucket_2\B2.dat
    /// // Adding File--> Bucket_2\B2.txt
    /// // Adding File--> Bucket_3\B3.dat
    /// // Adding File--> Bucket_3\B3.txt
    /// // Adding File--> Bucket_3\Slot_1\B3_S1.dat
    /// // Adding File--> Bucket_3\Slot_1\B3_S1.txt
    /// // Adding File--> Bucket_3\Slot_1\Slot_1\B3_S1_S1.dat
    /// // Adding File--> Bucket_3\Slot_1\Slot_1\B3_S1_S1.txt
    /// // Adding File--> Bucket_3\Slot_2\B3_S2.dat
    /// // Adding File--> Bucket_3\Slot_2\B3_S2.txt
    /// // Adding File--> Bucket_3\Slot_3\B3_S3.dat
    /// // Adding File--> Bucket_3\Slot_3\B3_S3.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_1\B3_S3_S1.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_1\B3_S3_S1.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\B3_S3_S2.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\B3_S3_S2.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_1\B3_S3_S2_S1.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_1\B3_S3_S2_S1.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_2\B3_S3_S2_S2.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_2\B3_S3_S2_S2.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_2\Slot_1\B3_S3_S2_S2_S1.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_2\Slot_2\Slot_1\B3_S3_S2_S2_S1.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_3\B3_S3_S3.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_3\B3_S3_S3.txt
    /// // Adding File--> Bucket_3\Slot_3\Slot_3\Slot_1\B3_S3_S3_S1.dat
    /// // Adding File--> Bucket_3\Slot_3\Slot_3\Slot_1\B3_S3_S3_S1.txt
    /// // Adding File--> Bucket_3\Slot_4\B3_S4.dat
    /// // Adding File--> Bucket_3\Slot_4\B3_S4.txt
    /// // Adding File--> Bucket_3\Slot_5\B3_S5.dat
    /// // Adding File--> Bucket_3\Slot_5\B3_S5.txt
    /// // Adding File--> Bucket_4\B4.dat
    /// // Adding File--> Bucket_4\B4.txt
    /// // Adding File--> Bucket_5\B5.dat
    /// // Adding File--> Bucket_5\B5.txt
    /// // Adding File--> Bucket_6\B6.dat
    /// // Adding File--> Bucket_6\B6.txt
    /// // Adding File--> Bucket_7\B7.dat
    /// // Adding File--> Bucket_7\B7.txt
    /// // Adding File--> Bucket_7\Slot_1\B7_S1.dat
    /// // Adding File--> Bucket_7\Slot_1\B7_S1.txt
    /// // 
    /// // ---- Saving the File Store
    /// // 
    /// // D:\Dev\Sandbox\Example-7.zip
    /// // Ionic.Zip v1.9.1.8
    /// // Uncompressed Size = 11.5 MB (12,104,126 bytes)
    /// // Compressed Size   = 5.3 MB (5,578,990 bytes)
    /// // Compression Ratio = 0.46092 (53.9%)
    /// // 
    /// // D:\Dev\Sandbox\Example-7 Extract
    /// // Count= 44
    /// // CountAvailableRootFiles= 44
    /// // CountDeleted= 0
    /// // CountExtracted= 0
    /// // CountExtractedUpdatable= 0
    /// // CountOriginal= 44
    /// // 
    /// // ---- Files Loaded into the File Store
    /// // 
    /// //    87 KB/87 KB      0.0%     Store Bucket_1\B1.dat
    /// //   537 KB/173 KB    67.8%  Compress Bucket_1\B1.txt
    /// //   129 KB/129 KB     0.0%     Store Bucket_1\Slot_1\B1_S1.dat
    /// //   750 KB/241 KB    67.9%  Compress Bucket_1\Slot_1\B1_S1.txt
    /// //   109 KB/109 KB     0.0%     Store Bucket_2\B2.dat
    /// //   343 KB/111 KB    67.6%  Compress Bucket_2\B2.txt
    /// //    59 KB/59 KB      0.0%     Store Bucket_3\B3.dat
    /// //   322 KB/105 KB    67.5%  Compress Bucket_3\B3.txt
    /// //    46 KB/46 KB      0.0%     Store Bucket_3\Slot_1\B3_S1.dat
    /// //   727 KB/234 KB    67.8%  Compress Bucket_3\Slot_1\B3_S1.txt
    /// //    49 KB/49 KB      0.0%     Store Bucket_3\Slot_1\Slot_1\B3_S1_S1.dat
    /// //   494 KB/159 KB    67.8%  Compress Bucket_3\Slot_1\Slot_1\B3_S1_S1.txt
    /// //   142 KB/142 KB     0.0%     Store Bucket_3\Slot_2\B3_S2.dat
    /// //   510 KB/165 KB    67.7%  Compress Bucket_3\Slot_2\B3_S2.txt
    /// //    86 KB/86 KB      0.0%     Store Bucket_3\Slot_3\B3_S3.dat
    /// //   355 KB/115 KB    67.6%  Compress Bucket_3\Slot_3\B3_S3.txt
    /// //   110 KB/110 KB     0.0%     Store Bucket_3\Slot_3\Slot_1\B3_S3_S1.dat
    /// //   266 KB/87 KB     67.5%  Compress Bucket_3\Slot_3\Slot_1\B3_S3_S1.txt
    /// //   102 KB/102 KB     0.0%     Store Bucket_3\Slot_3\Slot_2\B3_S3_S2.dat
    /// //   438 KB/142 KB    67.7%  Compress Bucket_3\Slot_3\Slot_2\B3_S3_S2.txt
    /// //    74 KB/74 KB      0.0%     Store Bucket_3\Slot_3\Slot_2\Slot_1\B3_S3_S2_S1.dat
    /// //   499 KB/161 KB    67.7%  Compress Bucket_3\Slot_3\Slot_2\Slot_1\B3_S3_S2_S1.txt
    /// //   156 KB/156 KB     0.0%     Store Bucket_3\Slot_3\Slot_2\Slot_2\B3_S3_S2_S2.dat
    /// //   186 KB/61 KB     67.2%  Compress Bucket_3\Slot_3\Slot_2\Slot_2\B3_S3_S2_S2.txt
    /// //   129 KB/129 KB     0.0%     Store Bucket_3\Slot_3\Slot_2\Slot_2\Slot_1\B3_S3_S2_S2_S1.dat
    /// //   165 KB/54 KB     67.1%  Compress Bucket_3\Slot_3\Slot_2\Slot_2\Slot_1\B3_S3_S2_S2_S1.txt
    /// //   104 KB/104 KB     0.0%     Store Bucket_3\Slot_3\Slot_3\B3_S3_S3.dat
    /// //   661 KB/213 KB    67.8%  Compress Bucket_3\Slot_3\Slot_3\B3_S3_S3.txt
    /// //    85 KB/85 KB      0.0%     Store Bucket_3\Slot_3\Slot_3\Slot_1\B3_S3_S3_S1.dat
    /// //   587 KB/190 KB    67.7%  Compress Bucket_3\Slot_3\Slot_3\Slot_1\B3_S3_S3_S1.txt
    /// //   163 KB/163 KB     0.0%     Store Bucket_3\Slot_4\B3_S4.dat
    /// //   161 KB/53 KB     67.0%  Compress Bucket_3\Slot_4\B3_S4.txt
    /// //    44 KB/44 KB      0.0%     Store Bucket_3\Slot_5\B3_S5.dat
    /// //   397 KB/128 KB    67.7%  Compress Bucket_3\Slot_5\B3_S5.txt
    /// //   132 KB/132 KB     0.0%     Store Bucket_4\B4.dat
    /// //   187 KB/61 KB     67.2%  Compress Bucket_4\B4.txt
    /// //   168 KB/168 KB     0.0%     Store Bucket_5\B5.dat
    /// //   615 KB/198 KB    67.8%  Compress Bucket_5\B5.txt
    /// //   166 KB/166 KB     0.0%     Store Bucket_6\B6.dat
    /// //   356 KB/115 KB    67.6%  Compress Bucket_6\B6.txt
    /// //   164 KB/164 KB     0.0%     Store Bucket_7\B7.dat
    /// //   358 KB/116 KB    67.6%  Compress Bucket_7\B7.txt
    /// //    97 KB/97 KB      0.0%     Store Bucket_7\Slot_1\B7_S1.dat
    /// //   503 KB/162 KB    67.7%  Compress Bucket_7\Slot_1\B7_S1.txt
    /// // 
    /// // ---- Extract a few files from the File Store
    /// // 
    /// // The following files should have been extracted:
    /// // ------------------------------------
    /// // 
    /// // D:\Dev\Sandbox\Example-7 Extract\Bucket_2\B2.dat
    /// // D:\Dev\Sandbox\Example-7 Extract\Bucket_3\Slot_1\B3_S1.dat
    /// // D:\Dev\Sandbox\Example-7 Extract\Bucket_3\Slot_1\Slot_1\B3_S1_S1.dat
    /// // D:\Dev\Sandbox\Example-7 Extract\Bucket_3\Slot_3\B3_S3.dat
    /// // 
    /// // ------------------------------------
    /// // 
    /// // D:\Dev\Sandbox\Example-7.zip
    /// // Ionic.Zip v1.9.1.8
    /// // Uncompressed Size = 11.5 MB (12,104,126 bytes)
    /// // Compressed Size   = 5.3 MB (5,578,990 bytes)
    /// // Compression Ratio = 0.46092 (53.9%)
    /// // 
    /// // D:\Dev\Sandbox\Example-7 Extract
    /// // Count= 44
    /// // CountAvailableRootFiles= 44
    /// // CountDeleted= 0
    /// // CountExtracted= 4
    /// // CountExtractedUpdatable= 0
    /// // CountOriginal= 44
    /// // 
    /// // ------------------------------------
    /// // press anykey...
    /// </code>
    /// </example>
    public enum CompressionStorageStrategy
    {
        /// <summary>
        /// The file will be compressed.
        /// </summary>
        Compress = 0,
        /// <summary>
        /// The file will not be compressed.
        /// </summary>
        Store
    }
}
