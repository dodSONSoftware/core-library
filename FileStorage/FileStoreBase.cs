using System;
using System.Collections.Generic;
using System.Linq;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// The abstract base class for creating and managing disconnected file stores. A disconnected store will give the consumer
    /// access to add, remove, update and extract files and get information about the state of the file relative to the original file, 
    /// the back-end storage file and the extracted file.
    /// </summary>
    /// <remarks>
    /// Inheritors of this base class can create file storage types that can store files in any conceivable fashion.
    /// A derived store could use various cloud-based techniques, database storage or even design a 
    /// system where files had to be constructed from any number of fixed strings of hexadecimal character pairs stored in text files, or anything else you can imagine.   :-)
    /// </remarks>
    /// <example>
    /// <para>The following example will open an <see cref="dodSON.Core.FileStorage.MSdotNETFileSystem.FileStore"/> and extracts a few files.</para>
    /// <para>Create a console application and add the following code:</para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // **** 
    ///     // **** IMPORTANT ****
    ///     // ****
    ///     // **** the source directory should contain a few files, sub directories will be included.
    ///     var sourceDirectory = @"D:\Dev\Sandbox\Temp-7";
    ///     // **** the extraction directory should point to an empty directory. 
    ///     // **** Do not put the extraction directory in the source directory.
    ///     var extractionDirectory = @"D:\Dev\Sandbox\Temp-7 Extract";
    ///    
    ///     // random
    ///     var rnd = new Random();
    ///    
    ///     // this will cause the MSdotNETFileSystem to create a file in the root directory, which contains 
    ///     // all of the, known, original filenames for the files stored in the file store back-end
    ///     // That being said, I should note that other implementations may store the 
    ///     // original filenames in a completely different manner
    ///     var saveOriginalFilenames = true;
    ///     // create a file store
    ///     var fileStore = new dodSON.Core.FileStorage.OpenDirectoryStore.MSdotNETFileSystem(
    ///                                     sourceDirectory,
    ///                                     extractionDirectory,
    ///                                     saveOriginalFilenames);
    ///    
    ///     // display file store information);
    ///     Console.WriteLine("{0}", fileStore.ExtractionRootDirectory);
    ///     Console.WriteLine("Count= {0}", fileStore.Count);
    ///     Console.WriteLine("CountAvailableRootFiles= {0}", fileStore.CountAvailableRootFiles);
    ///     Console.WriteLine("CountDeleted= {0}", fileStore.CountDeleted);
    ///     Console.WriteLine("CountExtracted= {0}", fileStore.CountExtracted);
    ///     Console.WriteLine("CountExtractedUpdatable= {0}", fileStore.CountExtractedUpdatable);
    ///     Console.WriteLine("CountOriginal= {0}", fileStore.CountOriginal);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///    
    ///     // select a few files as random
    ///     var numberOfItemsToAttemptToExtract = 4;
    ///     var list = fileStore.Find((dodSON.Core.FileStorage.IFileStoreItem item) =&gt;
    ///                          {
    ///                              if ((numberOfItemsToAttemptToExtract &gt; 0) &amp;&amp;
    ///                                  (rnd.NextDouble() &lt; 0.333))
    ///                              {
    ///                                  --numberOfItemsToAttemptToExtract;
    ///                                  return true;
    ///                              }
    ///                              return false;
    ///                          });
    ///    
    ///     // extract the selected files
    ///     var extractedFiles = fileStore.Extract(list, true);
    ///    
    ///     // display extracted files
    ///     Console.WriteLine("The following files should have been extracted:");
    ///     Console.WriteLine("------------------------------------");
    ///     foreach (var filename in extractedFiles)
    ///     {
    ///         Console.WriteLine(filename);
    ///     }
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine();
    ///    
    ///     // display file store information
    ///     Console.WriteLine("{0}", fileStore.ExtractionRootDirectory);
    ///     Console.WriteLine("Count= {0}", fileStore.Count);
    ///     Console.WriteLine("CountAvailableRootFiles= {0}", fileStore.CountAvailableRootFiles);
    ///     Console.WriteLine("CountDeleted= {0}", fileStore.CountDeleted);
    ///     Console.WriteLine("CountExtracted= {0}", fileStore.CountExtracted);
    ///     Console.WriteLine("CountExtractedUpdatable= {0}", fileStore.CountExtractedUpdatable);
    ///     Console.WriteLine("CountOriginal= {0}", fileStore.CountOriginal);
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    ///
    /// // D:\Dev\Sandbox\Temp-7 Extract
    /// // Count= 55
    /// // CountAvailableRootFiles= 55
    /// // CountDeleted= 0
    /// // CountExtracted= 0
    /// // CountExtractedUpdatable= 0
    /// // CountOriginal= 55
    /// // 
    /// // ------------------------------------
    /// // 
    /// // The following files should have been extracted:
    /// // ------------------------------------
    /// // D:\Dev\Sandbox\Temp-7 Extract\(Example)\Bucket_1\Document B1.txt
    /// // D:\Dev\Sandbox\Temp-7 Extract\(Example)\Bucket_4\TestFile_B4.xml
    /// // D:\Dev\Sandbox\Temp-7 Extract\(Example)\Bucket_1\Slot_1\Document B1_S1.txt
    /// // D:\Dev\Sandbox\Temp-7 Extract\(Example)\Bucket_6\Document B6.txt
    /// // 
    /// // ------------------------------------
    /// // 
    /// // D:\Dev\Sandbox\Temp-7 Extract
    /// // Count= 55
    /// // CountAvailableRootFiles= 55
    /// // CountDeleted= 0
    /// // CountExtracted= 4
    /// // CountExtractedUpdatable= 0
    /// // CountOriginal= 55
    /// //
    /// // ------------------------------------
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/><br/>
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
    ///     // all of the, known, original filenames for the files stored in the file store back-end
    ///     // That being said, I should note that other implementations may store the 
    ///     // original filenames in a completely different manner
    ///     var saveOriginalFilenames = true;
    ///     // this will cause the compressed file store to store files with the extension ".dat" without compression
    ///     // periods are optional "dat" and ".dat" are the same
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
    public abstract class FileStoreBase
        : IFileStore
    {
        #region Constants
        // for backwards compatibility, do not change this value.
        /// <summary>
        /// The filename for the file containing the original names of the files in the file store.
        /// </summary>
        public static readonly string _OriginalFilenames_Filename = "AAA1966B51F095A794E276C7163FAB537RED49.filestore.dat";
        #endregion
        #region System.Collections.Specialized.INotifyCollectionChanged Methods
        /// <summary>
        /// Occurs when an item is added, replaced, removed or the entire list is cleared.
        /// </summary>
        public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;
        /// <summary>
        /// Will raise the collection changed event.
        /// </summary>
        /// <param name="action">The action that is being performed.</param>
        /// <param name="newOrChangedItem">The new, changed or deleted item.</param>
        /// <param name="oldItem">If an item is being replaced, this is the original item.</param>
        protected void RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction action,
                                                   IFileStoreItem newOrChangedItem,
                                                   IFileStoreItem oldItem)
        {
            var handler = CollectionChanged;
            if (handler != null)
            {
                if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action));
                }
                else if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, newOrChangedItem, oldItem));
                }
                else if (action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, oldItem));
                }
                else
                {
                    handler(this, new System.Collections.Specialized.NotifyCollectionChangedEventArgs(action, newOrChangedItem));
                }
            }
        }
        #endregion
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected FileStoreBase()
        {
        }
        /// <summary>
        ///  A protected constructor.
        /// </summary>
        /// <param name="extractionDirectory">The default extraction root path.</param>
        /// <param name="saveOriginalFilenames">Determines whether the original filenames should be persisted.</param>
        protected FileStoreBase(string extractionDirectory,
                                bool saveOriginalFilenames)
            : this()
        {
            if (string.IsNullOrWhiteSpace(extractionDirectory))
            {
                throw new ArgumentNullException("extractionDirectory");
            }
            if (!System.IO.Directory.Exists(extractionDirectory))
            {
                FileStorageHelper.CreateDirectory(extractionDirectory);
            }
            _ExtractionDirectory = extractionDirectory;
            SaveOriginalFilenames = saveOriginalFilenames;
        }
        #endregion
        #region Private Fields
        private string _ExtractionDirectory = "";
        private Dictionary<string, IFileStoreItem> _Items = new Dictionary<string, IFileStoreItem>();
        private List<IFileStoreItem> _DeletedItems = new List<IFileStoreItem>();
        #endregion
        #region IFileStore Methods
        /// <summary>
        /// Gets or sets whether the original filenames should be persisted, or not.
        /// </summary>
        public bool SaveOriginalFilenames { get; set; } = false;
        /// <summary>
        /// Gets and sets the extraction root path.
        /// </summary>
        public string ExtractionRootDirectory
        {
            get
            {
                return _ExtractionDirectory;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("value");
                }
                if (!System.IO.Directory.Exists(value))
                {
                    throw new System.IO.DirectoryNotFoundException();
                }
                _ExtractionDirectory = value;
            }
        }
        /// <summary>
        ///  Gets the number of <see cref="IFileStoreItem"/> contained in this file store.
        /// </summary>
        public int Count => _Items.Count;
        /// <summary>
        /// Gets the number of deleted <see cref="IFileStoreItem"/> contained in this file store.
        /// </summary>
        public int CountDeleted => _DeletedItems.Count;
        /// <summary>
        /// Gets the number of <see cref="IFileStoreItem"/> whose original files are accessible.
        /// </summary>
        public int CountOriginal
        {
            get
            {
                int result = 0;
                ForEach((item) => { if (item.IsOriginalFileAvailable) { ++result; } });
                return result;
            }
        }
        /// <summary>
        /// Gets the number of <see cref="IFileStoreItem"/> which have been extracted.
        /// </summary>
        public int CountExtracted
        {
            get
            {
                int result = 0;
                ForEach((x) => { if (x.IsExtractedFileAvailable) { ++result; } });
                return result;
            }
        }
        /// <summary>
        ///  Gets the number of <see cref="IFileStoreItem"/> which have been extracted and are updatable.
        /// </summary>
        public int CountExtractedUpdatable
        {
            get
            {
                int result = 0;
                ForEach((x) => { if (x.IsExtractedFileUpdatable) { ++result; } });
                return result;
            }
        }
        /// <summary>
        ///  Gets the number of <see cref="IFileStoreItem"/> which have root file available in the backend store. This includes deleted files, because these files still have root files available in the backend store.
        /// </summary>
        public int CountAvailableRootFiles
        {
            get
            {
                int result = 0;
                ForEach((x) => { if (x.IsRootFileAvailable) { ++result; } });
                return result;
            }
        }
        /// <summary>
        ///  Determines if an <see cref="IFileStoreItem"/> is contained in this file store.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>Returns true if the <see cref="IFileStoreItem"/> is contained in the file store, otherwise false.</returns>
        public bool Contains(string rootFilename)
        {
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            return _Items.ContainsKey(rootFilename);
        }
        /// <summary>
        ///  Determines if an <see cref="IFileStoreItem"/> is to be deleted from the file store.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>Returns true if the <see cref="IFileStoreItem"/> is to be deleted from the file store, otherwise false.</returns>
        public bool ContainsDeleted(string rootFilename)
        {
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            return (_DeletedItems.Find((x) => { return (rootFilename.Equals(x.RootFilename, StringComparison.InvariantCultureIgnoreCase)); }) != null);
        }
        /// <summary>
        /// The size, in bytes, of all files in the store.
        /// </summary>
        public long SizeInBytes
        {
            get
            {
                long result = 0;
                ForEach(item => result += item.FileSize);
                return result;
            }
        }
        /// <summary>
        /// A view the current states for this file store.
        /// </summary>
        public Common.IStateChangeView StateChangeViewer
        {
            get
            {
                bool foundDirty = false;
                bool foundNew = false;
                foreach (var item in _Items)
                {
                    if (item.Value.StateChangeViewer.IsDirty)
                    {
                        foundDirty = true;
                    }
                    if (item.Value.StateChangeViewer.IsNew)
                    {
                        foundNew = true;
                    }
                    // short-circuit 
                    if (foundDirty && foundNew)
                    {
                        break;
                    }
                }
                return new dodSON.Core.Common.StateChangeTracking(foundDirty, foundNew, (_DeletedItems.Count > 0));
            }
        }
        /// <summary>
        /// Creates the proper <see cref="IFileStoreItem"/>, populates it with the given information and adds it to the store.
        /// </summary>
        /// <param name="rootFilename">The name of the file.</param>
        /// <param name="originalFilename">The full filename to the original file.</param>
        /// <param name="rootFileLastModifiedTimeUtc">Last time the file was modified, in UTC time.</param>
        /// <param name="sizeInBytes">The size, in bytes, of the file.</param>
        /// <returns>The added <see cref="IFileStoreItem"/>.</returns>
        public IFileStoreItem Add(string rootFilename, string originalFilename, DateTime rootFileLastModifiedTimeUtc, long sizeInBytes) => AddItem(CreateNewFileStoreItem(rootFilename, originalFilename, rootFileLastModifiedTimeUtc, sizeInBytes));
        /// <summary>
        /// Deletes an <see cref="IFileStoreItem"/> from this file store. If the item is new it will be deleted, otherwise it will be moved to the deleted list.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>The <see cref="IFileStoreItem"/> deleted.</returns>
        public IFileStoreItem Delete(string rootFilename)
        {
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            return DeleteByID(rootFilename, false);
        }
        /// <summary>
        /// Will undelete an item if that item is in the deleted list; otherwise an exception will be thrown.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>The undeleted <see cref="IFileStoreItem"/>.</returns>
        public IFileStoreItem Undelete(string rootFilename)
        {
            // rules...
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            if (!ContainsDeleted(rootFilename))
            {
                throw new ArgumentException(string.Format("Item with the provided rootFilename not found in deleted list. rootFilename={0}", rootFilename), "rootFilename");
            }
            if (Contains(rootFilename) && !Get(rootFilename).StateChangeViewer.IsNew)
            {
                throw new ArgumentException("A non-new item already exists in the list. This is incongruent with established rules and should never happen.");
            }
            //if (OnUndelete(rootFilename))
            //{
            // undelete...
            IFileStoreItem returnValue = null;
            // remove current item 
            if (Contains(rootFilename))
            {
                returnValue = Delete(rootFilename);
            }
            // remove item from deleted list
            var item = GetDeleted(rootFilename);
            _DeletedItems.Remove(item);
            // set the item's parent to this
            (item as IFileStoreItemAdvanced).SetParent(this);
            // clear deleted flag
            ((dodSON.Core.Common.IStateChangeTracking)item.StateChangeViewer).ClearDeleted();
            // add item to list
            AddItem(item);
            // return null or replaced item item
            return returnValue;
            //}
            //return null;
        }
        /// <summary>
        /// Will clear all <see cref="IFileStoreItem"/>; moving non-new items to the deleted list as necessary.
        /// </summary>
        public void Clear()
        {
            //if (OnClear())
            //{
            // delete all non-new item
            var deleteditems = new List<IFileStoreItem>();
            foreach (IFileStoreItem item in _Items.Values)
            {
                if (!item.StateChangeViewer.IsNew)
                {
                    deleteditems.Add(item);
                }
            }
            foreach (IFileStoreItem item in deleteditems)
            {
                DeleteByID(item.RootFilename, true);
            }
            // clear remaining (new) items
            _Items.Clear();
            RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null, null);
            //}
        }
        /// <summary>
        /// When overridden, should reload the file store.
        /// </summary>
        public void Refresh() => Refresh_Refresh();
        /// <summary>
        /// Get the <see cref="IFileStoreItem"/>.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>The <see cref="IFileStoreItem"/> requested; or an exception if not found.</returns>
        public IFileStoreItem Get(string rootFilename)
        {
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            if (!Contains(rootFilename))
            {
                throw new FileStoreException(rootFilename, string.Format("Item with RootFilename not found. RootFilename={0}", rootFilename));
            }
            return _Items[rootFilename];
        }
        /// <summary>
        /// Get the <see cref="IFileStoreItem"/> from the deleted list.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>The <see cref="IFileStoreItem"/> requested; or an exception if not found in the deleted list.</returns>
        public IFileStoreItem GetDeleted(string rootFilename)
        {
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            if (_DeletedItems.Count > 0)
            {
                var foundItem = _DeletedItems.Find((x) => { return (rootFilename.Equals(x.RootFilename, StringComparison.InvariantCultureIgnoreCase)); });
                if (foundItem != null)
                {
                    return foundItem;
                }
            }
            throw new FileStoreException(rootFilename, string.Format("Deleted item with RootFilename not found. RootFilename={0}", rootFilename));
        }
        /// <summary>
        /// Gets all <see cref="IFileStoreItem"/>.
        /// </summary>
        /// <returns>A IEnumerable&lt;IFileStoreItem&gt; that can be used to iterate through all <see cref="IFileStoreItem"/>s.</returns>
        public IEnumerable<IFileStoreItem> GetAll() => _Items.Values as IEnumerable<IFileStoreItem>;
        /// <summary>
        /// Gets all deleted <see cref="IFileStoreItem"/>.
        /// </summary>
        /// <returns>A IEnumerable&lt;IFileStoreItem&gt; that can be used to iterate through all deleted <see cref="IFileStoreItem"/>s.</returns>
        public IEnumerable<IFileStoreItem> GetAllDeleted() => _DeletedItems as IEnumerable<IFileStoreItem>;
        /// <summary>
        /// Gets all <see cref="IFileStoreItem"/>s based on the Predicate&lt;IFileStoreItem&gt; provided.
        /// </summary>
        /// <param name="predicate">Represents the method that defines a set of criteria and determines whether the specified <see cref="IFileStoreItem"/> meets those criteria.</param>
        /// <returns>A IEnumerable&lt;IFileStoreItem&gt; that can be used to iterate through the selected <see cref="IFileStoreItem"/>s.</returns>
        public IEnumerable<IFileStoreItem> Find(Predicate<IFileStoreItem> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            foreach (IFileStoreItem item in _Items.Values)
            {
                if (predicate(item))
                {
                    yield return item;
                }
            }
        }
        /// <summary>
        /// Performs the specified action on each <see cref="IFileStoreItem"/>.
        /// </summary>
        /// <param name="action">The Action&lt;IFileStoreItem&gt; delegate to perform on each <see cref="IFileStoreItem"/>.</param>
        public void ForEach(Action<IFileStoreItem> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            foreach (IFileStoreItem item in _Items.Values)
            {
                action(item);
            }
        }
        /// <summary>
        /// Should perform all the operations necessary to add, update, and remove items from the back-end storage based on the specific type of file store. The specifics of this is up to the developer of the concrete class.
        /// </summary>
        /// <param name="autoUpdateNonDirtyRootFiles">If <b>true</b> then files which are update-able, but not dirty, will be updated. This is the case when a file has been replaced with a newer file; its not dirty, but it should be updated.</param>
        public void Save(bool autoUpdateNonDirtyRootFiles)
        {
            object save_state = null;
            try
            {
                // initialize save
                Save_Startup(out save_state);
                // delete all (deletable) files
                foreach (var item in this.GetAllDeleted())
                {
                    Save_RemoveFileFromSource(item, save_state);
                }
                // clear deleted list
                ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).ClearDeleted();
                // process all files            
                this.ForEach((x) =>
                {
                    if (x.IsOriginalFileAvailable)
                    {
                        // process by state
                        if (x.StateChangeViewer.IsNew)
                        {
                            Save_AddFileToSource(x, save_state);
                        }
                        else if (x.StateChangeViewer.IsDirty)
                        {
                            Save_UpdateFileInSource(x, save_state);
                        }
                        else if (autoUpdateNonDirtyRootFiles && x.IsRootFileUpdatable)
                        {
                            Save_UpdateFileInSource(x, save_state);
                        }
                        // set root file last modified time to the original file's last modified time
                        if (x.OriginalFileLastModifiedTimeUtc != x.RootFileLastModifiedTimeUtc)
                        {
                            ((dodSON.Core.FileStorage.IFileStoreItemAdvanced)x).SetRootFileLastModifiedTimeUtc(x.OriginalFileLastModifiedTimeUtc);
                        }
                        // clear state
                        ((dodSON.Core.FileStorage.IFileStoreItemAdvanced)x).StateChangeTracker.ClearAll();
                    }
                });
                // original filenames
                if (SaveOriginalFilenames)
                {
                    Save_SaveOriginalFilenames(save_state);
                }
            }
            finally
            {
                // shutdown save
                Save_Shutdown(ref save_state);
            }
        }
        /// <summary>
        /// Extracts all <see cref="IFileStoreItem"/>s to the defined extraction root directory.
        /// </summary>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of filenames for all the files that have been extracted.</returns>/// <returns></returns>
        public IEnumerable<string> ExtractAll(bool explicitExtraction) => ExtractAll(ExtractionRootDirectory, explicitExtraction);
        /// <summary>
        /// Extracts all <see cref="IFileStoreItem"/>s to the provided extraction root directory.
        /// </summary>
        /// <param name="destRootPath">The root path where the <see cref="IFileStoreItem"/>s are to be extracted.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of filenames for all the files that have been extracted.</returns>/// <returns></returns>
        public IEnumerable<string> ExtractAll(string destRootPath, bool explicitExtraction) => Extract(destRootPath, this.GetAll(), explicitExtraction);
        /// <summary>
        /// Extracts the specified <see cref="IFileStoreItem"/> to the default extraction root directory.
        /// </summary>
        /// <param name="item">The <see cref="IFileStoreItem"/> to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The filename for the file that was extracted.</returns>
        public string Extract(IFileStoreItem item, bool explicitExtraction) => Extract(ExtractionRootDirectory, new IFileStoreItem[] { item }, explicitExtraction).ElementAtOrDefault(0);
        /// <summary>
        /// Extracts the specified <see cref="IFileStoreItem"/> to the provided extraction root directory.
        /// </summary>
        /// <param name="destRootPath"></param>
        /// <param name="item">The <see cref="IFileStoreItem"/> to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>The filename for the file that was extracted.</returns>
        public string Extract(string destRootPath, IFileStoreItem item, bool explicitExtraction) => Extract(destRootPath, new IFileStoreItem[] { item }, explicitExtraction).ElementAtOrDefault(0);
        /// <summary>
        /// Extracts the specified <see cref="IFileStoreItem"/>s to the default extraction root directory.
        /// </summary>
        /// <param name="items">The <see cref="IFileStoreItem"/>s to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of filenames for all the files that have been extracted.</returns>/// <returns></returns>
        public IEnumerable<string> Extract(IEnumerable<IFileStoreItem> items, bool explicitExtraction) => Extract(ExtractionRootDirectory, items, explicitExtraction);
        /// <summary>
        /// Extracts the specified <see cref="IFileStoreItem"/>s to the provided extraction root directory.
        /// </summary>
        /// <param name="destRootPath">The root path where the <see cref="IFileStoreItem"/>s are to be extracted.</param>
        /// <param name="items">The <see cref="IFileStoreItem"/>s to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of filenames for all the files that have been extracted.</returns>
        public IEnumerable<string> Extract(string destRootPath, IEnumerable<IFileStoreItem> items, bool explicitExtraction)
        {
            // checks...
            if (string.IsNullOrWhiteSpace(destRootPath))
            {
                throw new ArgumentNullException("destRootPath");
            }
            if (!System.IO.Directory.Exists(destRootPath))
            {
                throw new System.IO.DirectoryNotFoundException(destRootPath);
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            // temporarily change _DestinationRootPath to destRootpath
            var originalExtractionDirectory = _ExtractionDirectory;
            _ExtractionDirectory = destRootPath;
            // extracted files list
            var extractedFiles = new List<string>();
            object state = null;
            try
            {
                // startup extraction
                Extract_StartUp(destRootPath, out state);
                // extract selected files
                foreach (var item in items)
                {
                    if (explicitExtraction || !item.IsExtractedFileAvailable || item.IsExtractedFileUpdatable)
                    {
                        extractedFiles.Add(Extract_ExtractFile(item, state));
                    }
                }
            }
            finally
            {
                // shutdown extraction
                Extract_Shutdown(ref state);
            }
            // restore original _ExtractionDirectory
            _ExtractionDirectory = originalExtractionDirectory;
            // return all extracted file name
            return extractedFiles;
        }
        /// <summary>
        /// Will extract the <paramref name="items"/> from the back-end directories to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to extract the files to.</param>
        /// <param name="items">The files to extract.</param>
        /// <param name="explicitExtraction">Determines how to extract a file <b>True</b> will extract the file, period; otherwise <b>false</b> will use rules to determine if the file needs to be extracted.</param>
        /// <returns>A list of the extracted files.</returns>
        public IEnumerable<string> ExtractToPath(string destRootPath, IEnumerable<IFileStoreItem> items, bool explicitExtraction)
        {
            // checks...
            if (string.IsNullOrWhiteSpace(destRootPath))
            {
                throw new ArgumentNullException("destRootPath");
            }
            if (!System.IO.Directory.Exists(destRootPath))
            {
                throw new System.IO.DirectoryNotFoundException(destRootPath);
            }
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            // temporarily change _DestinationRootPath to destRootpath
            var originalExtractionDirectory = _ExtractionDirectory;
            _ExtractionDirectory = destRootPath;
            // extracted files list
            var extractedFiles = new List<string>();
            object state = null;
            try
            {
                // startup extraction
                Extract_StartUp(destRootPath, out state);
                // extract selected files
                foreach (var item in items)
                {
                    if (explicitExtraction || !item.IsExtractedFileAvailable || item.IsExtractedFileUpdatable)
                    {
                        extractedFiles.Add(Extract_ExtractFileToPath(destRootPath, item, state));
                    }
                }
            }
            finally
            {
                // shutdown extraction
                Extract_Shutdown(ref state);
            }
            // restore original _ExtractionDirectory
            _ExtractionDirectory = originalExtractionDirectory;
            // return all extracted file name
            return extractedFiles;
        }
        /// <summary>
        /// Determines if the root file for the specific <see cref="IFileStoreItem"/>, identified by <paramref name="rootFilename"/> key, is available in the backend store. This includes deleted files, because these files still have root files available in the backend store.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <returns>Returns true if the <see cref="IFileStoreItem"/> is available, otherwise false.</returns>
        public bool IsRootFileAvailable(string rootFilename)
        {
            // check deleted items list
            var deletedItem = _DeletedItems.FirstOrDefault(x => { return x.RootFilename.Equals(rootFilename, StringComparison.InvariantCultureIgnoreCase); });
            if (deletedItem != null)
            {
                return true;
            }
            // check items list
            return ((_Items.ContainsKey(rootFilename)) && (!_Items[rootFilename].StateChangeViewer.IsNew));
        }
        #endregion
        #region Explicit IFileStoreControl<T> Methods
        void IFileStoreAdvanced.ResetLists()
        {
            _Items = new Dictionary<string, IFileStoreItem>();
            ((dodSON.Core.FileStorage.IFileStoreAdvanced)this).ClearDeleted();
            RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Reset, null, null);
        }
        void IFileStoreAdvanced.ClearDeleted()
        {
            //if (OnClearDeleted())
            //{
            _DeletedItems = new List<IFileStoreItem>();
            //}
        }
        IDictionary<string, string> IFileStoreAdvanced.GetOriginalFilenames()
        {
            var dictionary = new Dictionary<string, string>();
            ForEach((x) => { dictionary.Add(x.RootFilename, x.OriginalFilename); });
            return dictionary;
        }
        void IFileStoreAdvanced.SetOriginalFilenames(IDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            foreach (var item in dictionary)
            {
                if (Contains(item.Key))
                {
                    Get(item.Key).SetOriginalFilename(item.Value);
                }
            }
        }
        void IFileStoreAdvanced.ClearOriginalFilenames() => ForEach(item => { item.SetOriginalFilename(""); });
        #endregion
        #region Private Methods 
        private IFileStoreItem AddItem(IFileStoreItem item)
        {
            // rules...
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            if (item.StateChangeViewer.IsDeleted)
            {
                throw new FileStoreException(item, "Item is marked deleted. Cannot add item marked deleted. Try the Undelete() method.");
            }
            if (ContainsDeleted(item.RootFilename))
            {
                if (!item.StateChangeViewer.IsNew)
                {
                    throw new FileStoreException(item, "Item has already been deleted and item is not marked new. Can only add new item when item has already been deleted.");
                }
                // If the rules work correctly, this test will never be true.
                if (Contains(item.RootFilename) && !Get(item.RootFilename).StateChangeViewer.IsNew)
                {
                    throw new FileStoreException(item, "There is a deleted item and a non-new current item; this is incongruent with established rules and should never happen.");
                }
            }
            // add item...
            if (Contains(item.RootFilename))
            {
                // ######## DELETE/ADD ITEM...
                // delete item
                var currentItem = DeleteByID(item.RootFilename, true);
                // add new item
                _Items.Add(item.RootFilename, item);
                // set the item's parent to this
                (item as IFileStoreItemAdvanced).SetParent(this);
                // mark dirty
                ((dodSON.Core.Common.IStateChangeTracking)item.StateChangeViewer).MarkDirty();
                // raise collection changed event
                RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Replace, item, currentItem);
                // return current item
                return currentItem;
            }
            else
            {
                // ######## ADD NEW ITEM
                // add new item
                _Items.Add(item.RootFilename, item);
                // set the item's parent to this
                (item as IFileStoreItemAdvanced).SetParent(this);
                // mark dirty
                ((dodSON.Core.Common.IStateChangeTracking)item.StateChangeViewer).MarkDirty();
                // raise collection changed event
                RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Add, item, null);
                // return new item
                return item;
            }
        }
        private IFileStoreItem DeleteByID(string rootFilename, bool suppressCollectionChangedEvent)
        {
            // rules...
            if (string.IsNullOrWhiteSpace(rootFilename))
            {
                throw new ArgumentOutOfRangeException("rootFilename", "Parameter rootFilename cannot be null, empty or whitespace.");
            }
            //// call OnDelete()...
            //T deletedItem = OnDelete(rootFilename, suppressCollectionChangedEvent);
            //if (deletedItem != null)
            //{
            //    return deletedItem;
            //}
            // delete item...
            if (_Items.ContainsKey(rootFilename))
            {
                // get current item
                IFileStoreItem currentItem = _Items[rootFilename];
                // rules...
                //if (currentItem.StateChangeTracker.IsDeleted) { throw new FileStoreException<T>(currentItem, "Item is marked deleted. Cannot delete an item marked deleted."); }
                if (!currentItem.StateChangeViewer.IsNew && ContainsDeleted(currentItem.RootFilename))
                {
                    throw new FileStoreException(currentItem, "Item has already been deleted and is not marked new. Can only delete non-new item when item has not already been deleted.");
                }
                // delete item...
                // remove current item from ITEMS
                _Items.Remove(rootFilename);
                // if new, clear parent store; otherwise add to deleted list (rule...)
                if (currentItem.StateChangeViewer.IsNew)
                {
                    // clear the item's parent 
                    (currentItem as IFileStoreItemAdvanced).SetParent(null);
                }
                else
                {
                    // add current item to DELETEDITEMS
                    _DeletedItems.Add(currentItem);
                    ((dodSON.Core.Common.IStateChangeTracking)currentItem.StateChangeViewer).MarkDeleted();
                }
                // raise collection changed event
                if (!suppressCollectionChangedEvent)
                {
                    RaiseCollectionChangedEvent(System.Collections.Specialized.NotifyCollectionChangedAction.Remove, null, currentItem);
                }
                // return current item
                return currentItem;
            }
            throw new Exception(string.Format("Item not found. Item RootFilename={0};", rootFilename));
        }
        #endregion
        #region Public Abstract Methods
        /// <summary>
        /// Creates an item, specific to this type of file store, based on the given meta-data.
        /// </summary>
        /// <param name="rootFilename">The name of the <see cref="IFileStoreItem"/> relative to the file store.</param>
        /// <param name="rootFileLastModifiedTimeUtc">A <see cref="DateTime"/> representing the last modified time of the <see cref="IFileStoreItem"/> to be created, in Universal Coordinated Time.</param>
        /// <param name="sizeInBytes">The size, in bytes, of the file.</param>
        /// <param name="originalFilename">The full pathname of the file to add.</param>
        /// <returns>An <see cref="IFileStoreItem"/> created using the original filename and given meta-data.</returns>
        public abstract IFileStoreItem CreateNewFileStoreItem(string rootFilename, string originalFilename, DateTime rootFileLastModifiedTimeUtc, long sizeInBytes);
        /// <summary>
        /// Determines if this file store can be restored to its original state. The specifics of this is up to the developer of the concrete class.
        /// </summary>
        public abstract bool SupportRollback
        {
            get;
        }
        /// <summary>
        /// If this file store supports rollback, this function should restore the file stored to its original state. The specifics of this is up to the developer of the concrete class.
        /// </summary>
        public abstract void Rollback();
        /// <summary>
        /// Will populate an <see cref="Core.Configuration.IConfigurationGroup"/> containing data needed to serialize the target object. 
        /// </summary>
        public abstract Configuration.IConfigurationGroup Configuration
        {
            get;
        }
        #endregion
        #region Protected Abstract Methods
        /// <summary>
        /// When overridden, should reload the file store.
        /// </summary>
        protected abstract void Refresh_Refresh();
        /// <summary>
        /// When overridden, should initialize the underlying storage system to prepare to add, update and remove files.
        /// </summary>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_Startup(out object state);
        /// <summary>
        /// When overridden, should add the <paramref name="item"/> to the underlying storage system.
        /// </summary>
        /// <param name="item">The store item to add.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_AddFileToSource(IFileStoreItem item, object state);
        /// <summary>
        /// When overridden, should update an existing file in the underlying storage system with the <paramref name="item"/>.
        /// </summary>
        /// <param name="item">The store item to update from.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_UpdateFileInSource(IFileStoreItem item, object state);
        /// <summary>
        /// When overridden, should remove an existing file, referenced by the <paramref name="item"/>, from the underlying storage system. 
        /// </summary>
        /// <param name="item">The store item to remove.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_RemoveFileFromSource(IFileStoreItem item, object state);
        /// <summary>
        /// When overridden, should persist the list of original filenames to the underlying storage system.
        /// </summary>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_SaveOriginalFilenames(object state);
        /// <summary>
        /// When overridden, should finalize the adding, updating and removing of files from the underlying storage system.
        /// </summary>
        /// <param name="state">An object, defined by the user, that will be passed to all Save_ methods.</param>
        protected abstract void Save_Shutdown(ref object state);
        /// <summary>
        /// When overridden, should initialize the underlying storage system to prepare to extract files.
        /// </summary>
        /// <param name="destRootPath">The root directory where extraction will be performed.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Extract_ methods.</param>
        protected abstract void Extract_StartUp(string destRootPath, out object state);
        /// <summary>
        /// When overridden, should extract the <paramref name="item"/> from the underlying storage system to the destination root path given in the method <see cref="Extract_StartUp(string, out object)"/>.
        /// </summary>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Extract_ methods.</param>
        /// <returns>The filename of the extract file.</returns>
        protected abstract string Extract_ExtractFile(IFileStoreItem item, object state);
        /// <summary>
        /// When overridden, should extract the <paramref name="item"/> from the underlying storage system to the <paramref name="destRootPath"/>.
        /// </summary>
        /// <param name="destRootPath">The directory to export the files to.</param>
        /// <param name="item">The store item to extract.</param>
        /// <param name="state">An object, defined by the user, that will be passed to all Extract_ methods.</param>
        /// <returns>The filename of the extract file.</returns>
        protected abstract string Extract_ExtractFileToPath(string destRootPath, IFileStoreItem item, object state);
        /// <summary>
        /// When overridden, should finalize the extraction of files from the underlying storage system. 
        /// </summary>
        /// <param name="state">An object, defined by the user, that will be passed to all Extract_ methods.</param>
        protected abstract void Extract_Shutdown(ref object state);
        #endregion
    }
}
