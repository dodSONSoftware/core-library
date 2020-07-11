using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.FileStorage
{
    /// <summary>
    /// Core functionality used by the types in the dodSON.Core.FileStorage namespace; presented here for your amusement.
    /// </summary>
    public static class FileStorageHelper
    {
        #region Public Static Methods
        /// <summary>
        /// Attempts to delete the specified directory.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        /// <param name="retries">The number of times to retry deleting the directory before abandoning the attempt.</param>
        /// <returns>Whether the directory was successfully deleted or not.</returns>
        public static bool DeleteDirectory(string path, int retries = 3)
        {
            if (!System.IO.Directory.Exists(path))
            {
                // directory not found; return TRUE indicating successful delete
                return true;
            }
            // attempt to delete the directory 3 times
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    if (System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.Delete(path, true);
                        return true;
                    }
                }
                catch
                {
                    Threading.ThreadingHelper.Sleep(50);
                }
            }
            return false;
        }
        /// <summary>
        /// Attempts to delete the specified file.
        /// </summary>
        /// <param name="filename">The file to delete.</param>
        /// <param name="retries">The number of times to retry deleting the file before abandoning the attempt.</param>
        /// <returns>Whether the file was successfully deleted or not.</returns>
        public static bool DeleteFile(string filename, int retries = 3)
        {
            if (!System.IO.File.Exists(filename))
            {
                // file not found; return TRUE indicating successful delete
                return true;
            }
            // attempt to delete the file 3 times
            for (int i = 0; i < retries; i++)
            {
                try
                {
                    if (System.IO.File.Exists(filename))
                    {
                        System.IO.File.Delete(filename);
                        return true;
                    }
                }
                catch
                {
                    Threading.ThreadingHelper.Sleep(50);
                }
            }
            //
            return false;
        }
        /// <summary>
        /// Will remove the <paramref name="sourcePath"/> from the <paramref name="fullFilename"/> and return the name of the file relative to the <paramref name="sourcePath"/>.
        /// </summary>
        /// <param name="sourcePath">The source path of the file.</param>
        /// <param name="fullFilename">The full path and filename of the file.</param>
        /// <returns>The name of the file relative to the <paramref name="sourcePath"/>.</returns>
        public static string GetRootFilename(string sourcePath, string fullFilename)
        {
            var result = NormalizeName(sourcePath, fullFilename);
            if (string.IsNullOrWhiteSpace(result))
            {
                return fullFilename;
            }
            return result;
        }

        // --------

        /// <summary>
        /// Will convert an <see cref="IFileStore"/> into a list of hierarchical data suitably for binding to TreeViews.
        /// </summary>
        /// <param name="fileStore">The <see cref="IFileStore"/> to convert.</param>
        /// <param name="rootDirectoryName">The display name for the root directory. The root directory's RootPath is an empty string, this value will be used in place of that empty string.</param>
        /// <param name="folderSeparator">The string to use to separate folders.</param>
        /// <returns>A list of hierarchical data suitably for binding to TreeViews.</returns>
        public static List<IHierarchicalFolder> ConvertToHierarchicalList(IFileStore fileStore,
                                                                         string rootDirectoryName = "ROOT",
                                                                         string folderSeparator = "\\")
        {
            if (fileStore == null) { throw new ArgumentNullException(nameof(fileStore)); }
            var isCompressed = fileStore is CompressedFileStoreBase;
            var folders = new List<IHierarchicalFolder>() { new HierarchicalFolder(null, rootDirectoryName, "", isCompressed) };
            foreach (var item in fileStore.GetAll()) { AddFile(item); }
            return folders;

            // ######## INTERNAL FUNCTIONS

            void AddFile(IFileStoreItem item_)
            {
                IHierarchicalFolder folder = null;

                // get the folder
                if (item_.Path == "") { folder = folders[0]; }
                else { folder = GetFolderRecursive(folders[0], item_.Path, ""); }

                // add file to folder
                folder.Files.Add(item_);
            }

            IHierarchicalFolder GetFolderRecursive(IHierarchicalFolder current_, string myPath_, string rootPath_)
            {
                // get path names
                var localDirName = "";
                var remainingPath = "";
                if (myPath_.Contains(folderSeparator))
                {
                    localDirName = myPath_.Substring(0, myPath_.IndexOf(folderSeparator));
                    remainingPath = myPath_.Substring(myPath_.IndexOf(folderSeparator) + 1);
                    rootPath_ = rootPath_ + folderSeparator + localDirName;
                }
                else
                {
                    localDirName = myPath_;
                    rootPath_ = rootPath_ + folderSeparator + localDirName;
                }
                // check for local directory
                IHierarchicalFolder foundFolder = null;
                foreach (var item in current_.Folders)
                {
                    if (item.Name.Equals(localDirName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundFolder = item;
                        break;
                    }
                }
                // create local directory, if required
                if (foundFolder == null)
                {
                    var rp_ = rootPath_;
                    if (rp_.StartsWith("\\")) { rp_ = rp_.Substring(1); }
                    foundFolder = new HierarchicalFolder(current_, localDirName, rp_, isCompressed);
                    current_.Folders.Add(foundFolder);
                }
                // check if done
                if (!string.IsNullOrWhiteSpace(remainingPath))
                {
                    // go again
                    foundFolder = GetFolderRecursive(foundFolder, remainingPath, rootPath_);
                }
                // done
                return foundFolder;
            }
        }


        // --------

        /// <summary>
        /// Will compare all files in all directories in the <paramref name="sourcePath"/> to all files in all directories in the <paramref name="destPath"/> and 
        /// will generate a report which will record whether files are ok, new or need to be removed or updated.
        /// </summary>
        /// <param name="sourcePath">The source directory to use for the comparison.</param>
        /// <param name="destPath">The destination directory to use for the comparison.</param>
        /// <param name="cancelToken">Provides a means to cancel the process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the comparison process.</param>
        /// <returns>A list containing a comparison report for each file.</returns>
        public static IEnumerable<ICompareResult> Compare(string sourcePath, string destPath, System.Threading.CancellationToken cancelToken, Action<double> feedback)
        {
            return GenerateReport(sourcePath, destPath, GatherInformation(sourcePath, cancelToken), GatherInformation(destPath, cancelToken), cancelToken, feedback);
        }
        /// <summary>
        /// Will compare all files in all directories in the <paramref name="sourcePath"/> to all files in all files in the file store <paramref name="destStore"/> and 
        /// will generate a report which will record whether files are ok, new or need to be removed or updated.
        /// </summary>
        /// <param name="sourcePath">The source directory to use for the comparison.</param>
        /// <param name="destStore">The <see cref="IFileStore"/> to use for the comparison.</param>
        /// <param name="cancelToken">Provides a means to cancel the process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the comparison process.</param>
        /// <returns>A list containing a comparison report for each file.</returns>
        public static IEnumerable<ICompareResult> Compare(string sourcePath, IFileStore destStore, System.Threading.CancellationToken cancelToken, Action<double> feedback)
        {
            return GenerateReport(sourcePath, "", GatherInformation(sourcePath, cancelToken), GatherInformation(destStore, cancelToken), cancelToken, feedback);
        }
        /// <summary>
        /// Will compare all files in the file store <paramref name="sourceStore"/> to all files in all directories in the <paramref name="destPath"/> and 
        /// will generate a report which will record whether files are ok, new or need to be removed or updated.
        /// </summary>
        /// <param name="sourceStore">The <see cref="IFileStore"/> to use for the comparison.</param>
        /// <param name="destPath">The destination directory to use for the comparison.</param>
        /// <param name="cancelToken">Provides a means to cancel the process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the comparison process.</param>
        /// <returns>A list containing a comparison report for each file.</returns>
        public static IEnumerable<ICompareResult> Compare(IFileStore sourceStore, string destPath, System.Threading.CancellationToken cancelToken, Action<double> feedback)
        {
            return GenerateReport("", destPath, GatherInformation(sourceStore, cancelToken), GatherInformation(destPath, cancelToken), cancelToken, feedback);
        }
        /// <summary>
        /// Will compare all files in the file store <paramref name="sourceStore"/> to all files in all files in the file store <paramref name="destStore"/> and 
        /// will generate a report which will record whether files are ok, new or need to be removed or updated.
        /// </summary>
        /// <param name="sourceStore">The <see cref="IFileStore"/> to use for the comparison.</param>
        /// <param name="destStore">The <see cref="IFileStore"/> to use for the comparison.</param>
        /// <param name="cancelToken">Provides a means to cancel the process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the comparison process.</param>
        /// <returns>A list containing a comparison report for each file.</returns>
        public static IEnumerable<ICompareResult> Compare(IFileStore sourceStore, IFileStore destStore, System.Threading.CancellationToken cancelToken, Action<double> feedback)
        {
            return GenerateReport("", "", GatherInformation(sourceStore, cancelToken), GatherInformation(destStore, cancelToken), cancelToken, feedback);
        }

        // ----------

        // TODO: add controls to the feedback Action, i.e. skipAction: maybe change it to a ( Func<bool, ICompareAction, double> )
        //       add type of action being performed
        //       add PreFeedback and PostFeedback events

        /// <summary>
        /// Will process the report generated by the Compare methods by adding, removing and updating files to the <paramref name="destPath"/> directory.
        /// </summary>
        /// <param name="report">The report, generated by one of the Compare methods, to process.</param>
        /// <param name="sourcePath">The source directory to process.</param>
        /// <param name="destPath">The destination directory to process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the mirroring process.</param>
        public static void MirrorSourceToDestination(
            IEnumerable<ICompareResult> report,
            string sourcePath,
            string destPath,
            Action<ICompareResult, double> feedback)
        {
            // **** initialize
            var total = (from x in report
                         where (x.Action == CompareAction.New || x.Action == CompareAction.Update || x.Action == CompareAction.Remove)
                         select x).Count();
            var count = 0;
            // **** delete REMOVE files from MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                       x.Action == CompareAction.Remove
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (System.IO.File.Exists(item.DestinationFullPath))
                {
                    var fInfo = new System.IO.FileInfo(item.DestinationFullPath);
                    if (fInfo.IsReadOnly)
                    {
                        System.IO.File.SetAttributes(item.DestinationFullPath, System.IO.FileAttributes.Normal);
                    }
                    System.IO.File.Delete(item.DestinationFullPath);
                }
            }
            // **** copy NEW and UPDATE files from SOURCE to MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                       (x.Action == CompareAction.New || x.Action == CompareAction.Update)
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                var sourceFile = item.SourceFullPath;
                var destfile = System.IO.Path.Combine(destPath, item.SourceRootFilename);
                CreateDirectory(System.IO.Path.GetDirectoryName(destfile));
                if (System.IO.File.Exists(sourceFile))
                {
                    System.IO.File.Copy(sourceFile, destfile, true);
                }
            }
            // **** process NEW/REMOVE folders
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.Directory &&
                                       (x.Action == CompareAction.New || x.Action == CompareAction.Remove)
                                 orderby (x.SourceFullPath + x.DestinationFullPath) descending
                                 select x)
            {
                if (item.Action == CompareAction.New)
                {
                    // NEW
                    if (!System.IO.Directory.Exists(item.DestinationFullPath))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(item.DestinationFullPath);
                        }
                        catch { }
                    }
                }
                else
                {
                    // REMOVE
                    if (System.IO.Directory.Exists(item.DestinationFullPath))
                    {
                        if (System.IO.Directory.GetFiles(item.DestinationFullPath).Length == 0)
                        {
                            DeleteDirectory(item.DestinationFullPath);
                        }
                        else
                        {
                            throw new FileStoreException((IFileStoreItem)null, string.Format("Directory not empty; cannot delete directory. Directory={0}", item.DestinationFullPath));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Will process the report generated by the Compare methods by adding, removing and updating files to the <paramref name="destStore"/> file store.
        /// </summary>
        /// <param name="report">The report, generated by one of the Compare methods, to process.</param>
        /// <param name="sourcePath">The source directory to process.</param>
        /// <param name="destStore">The file store to process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the mirroring process.</param>
        public static void MirrorSourceToDestination(
            IEnumerable<ICompareResult> report,
            string sourcePath,
            IFileStore destStore,
            Action<ICompareResult, double> feedback)
        {
            // **** initialize
            var total = (from x in report
                         where x.ItemType == CompareType.File &&
                                (x.Action == CompareAction.New || x.Action == CompareAction.Update || x.Action == CompareAction.Remove)
                         select x).Count();
            var count = 0;
            // **** save the original 'original filenames'
            var originalFilenames = ((IFileStoreAdvanced)destStore).GetOriginalFilenames();
            // **** delete REMOVE files from MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File && x.Action == CompareAction.Remove
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (destStore.Contains(item.DestinationRootFilename))
                {
                    destStore.Delete(item.DestinationRootFilename);
                }
            }
            // **** add NEW and UPDATE files to SOURCE
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                        (x.Action == CompareAction.New || x.Action == CompareAction.Update)
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                destStore.Add(item.SourceRootFilename, item.SourceFullPath, item.SourceLastModifiedTimeUtc, item.SourceFileSizeInBytes);
            }
            // **** save store (finalize mirror process)
            feedback?.Invoke(null, 1);
            destStore.Save(false);
            // **** clear/restore the 'original filenames'
            ((IFileStoreAdvanced)destStore).ClearOriginalFilenames();
            ((IFileStoreAdvanced)destStore).SetOriginalFilenames(originalFilenames);
        }
        /// <summary>
        /// Will process the report generated by the Compare methods by adding, removing and updating files to the <paramref name="destPath"/> directory.
        /// </summary>
        /// <param name="report">The report, generated by one of the Compare methods, to process.</param>
        /// <param name="sourceStore">The file store to process.</param>
        /// <param name="destPath">The destination directory to process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the mirroring process.</param>
        public static void MirrorSourceToDestination(
            IEnumerable<ICompareResult> report,
            IFileStore sourceStore,
            string destPath,
            Action<ICompareResult, double> feedback)
        {
            // **** initialize
            var total = (from x in report
                         where x.ItemType == CompareType.File &&
                                (x.Action == CompareAction.New || x.Action == CompareAction.Update || x.Action == CompareAction.Remove)
                         select x).Count();
            var count = 0;
            // **** delete REMOVE files from MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File && x.Action == CompareAction.Remove
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (System.IO.File.Exists(item.DestinationFullPath))
                {
                    var fInfo = new System.IO.FileInfo(item.DestinationFullPath);
                    if (fInfo.IsReadOnly)
                    {
                        System.IO.File.SetAttributes(item.DestinationFullPath, System.IO.FileAttributes.Normal);
                    }
                    System.IO.File.Delete(item.DestinationFullPath);
                }
            }
            // **** add/update NEW and UPDATE files into MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                        (x.Action == CompareAction.New || x.Action == CompareAction.Update)
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (sourceStore.Contains(item.SourceRootFilename))
                {
                    CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(destPath, item.SourceRootFilename)));
                    sourceStore.Extract(destPath, new IFileStoreItem[] { sourceStore.Get(item.SourceRootFilename) }, true);
                }
            }
            // **** process NEW/REMOVE folders
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.Directory &&
                                       (x.Action == CompareAction.New || x.Action == CompareAction.Remove)
                                 orderby (x.SourceFullPath + x.DestinationFullPath) descending
                                 select x)
            {
                if (item.Action == CompareAction.New)
                {
                    // NEW
                    if (!System.IO.Directory.Exists(item.SourceFullPath))
                    {
                        try
                        {
                            System.IO.Directory.CreateDirectory(item.SourceFullPath);
                        }
                        catch { }
                    }
                }
                else
                {
                    // REMOVE
                    if (System.IO.Directory.Exists(item.DestinationFullPath))
                    {
                        if (System.IO.Directory.GetFiles(item.DestinationFullPath).Length == 0)
                        {
                            DeleteDirectory(item.DestinationFullPath);
                        }
                        else
                        {
                            throw new FileStoreException((IFileStoreItem)null, string.Format("Directory not empty; cannot delete directory. Directory={0}", item.DestinationFullPath));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Will process the report generated by the Compare methods by adding, removing and updating files to the <paramref name="destStore"/> file store.
        /// </summary>
        /// <param name="report">The report, generated by one of the Compare methods, to process.</param>
        /// <param name="sourceStore">The file store to process.</param>
        /// <param name="destStore">The file store to process.</param>
        /// <param name="feedback">Provides a means to gather feedback from the mirroring process.</param>
        public static void MirrorSourceToDestination(
            IEnumerable<ICompareResult> report,
            IFileStore sourceStore,
            IFileStore destStore,
            Action<ICompareResult, double> feedback)
        {
            // **** initialize
            var total = (from x in report
                         where x.ItemType == CompareType.File &&
                               (x.Action == CompareAction.New || x.Action == CompareAction.Update || x.Action == CompareAction.Remove)
                         select x).Count();
            var count = 0;
            // create temporary extraction folder
            var tempPath = CreateTemporaryDirectory();
            // temporarily change source extraction path
            var originalExtractionPath = sourceStore.ExtractionRootDirectory;
            sourceStore.ExtractionRootDirectory = tempPath;
            // save the original 'original filenames'
            var originalFilenames = ((IFileStoreAdvanced)destStore).GetOriginalFilenames();
            // **** extract NEW and UPDATE files from SOURCE to temporary extraction folder
            var list = new List<IFileStoreItem>();
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                       (x.Action == CompareAction.New || x.Action == CompareAction.Update)
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (sourceStore.Contains(item.SourceRootFilename))
                {
                    list.Add(sourceStore.Get(item.SourceRootFilename));
                }
            }
            sourceStore.Extract(list, true);
            // **** delete REMOVE files from MIRROR
            foreach (var item in from x in report
                                 where x.ItemType == CompareType.File &&
                                       x.Action == CompareAction.Remove
                                 select x)
            {
                if (feedback != null)
                {
                    feedback(item, (double)((double)count / (double)total));
                    ++count;
                }
                if (destStore.Contains(item.DestinationRootFilename))
                {
                    destStore.Delete(item.DestinationRootFilename);
                }
            }
            // **** add/update NEW and UPDATE files, from temporary extraction folder, into MIRROR
            foreach (var item in list)
            {
                destStore.Add(item.RootFilename, item.ExtractedFilename, item.RootFileLastModifiedTimeUtc, item.FileSize);
            }
            // **** save store (finalize mirror process)
            destStore.Save(false);
            // **** clear/restore the 'original filenames'
            ((IFileStoreAdvanced)destStore).ClearOriginalFilenames();
            ((IFileStoreAdvanced)destStore).SetOriginalFilenames(originalFilenames);
            // **** restore original source extraction path
            sourceStore.ExtractionRootDirectory = originalExtractionPath;
            // **** delete temporary extraction folder
            DeleteDirectory(tempPath);
        }
        #endregion
        #region Internal Static Methods
        internal static string CreateTemporaryDirectory()
        {
            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), 'd' + new char[] { 'o', 'd', 'S' }.ToString() + "ON" + Guid.NewGuid().ToString("N"));
            CreateDirectory(tempPath);
            return tempPath;
        }
        internal static void CreateDirectory(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
        #endregion
        #region Private Static Methods
        private static IEnumerable<ICompareResult> GenerateReport(string sourcePath,
                                                                  string destinationPath,
                                                                  Dictionary<string, DataHolder> sourceInfo,
                                                                  Dictionary<string, DataHolder> destinationInfo,
                                                                  System.Threading.CancellationToken cancelToken,
                                                                  Action<double> feedback)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            feedback?.Invoke(-1.0);
            var total = sourceInfo.Count((x) => { return x.Value.ItemType == CompareType.File || x.Value.ItemType == CompareType.Directory; }) +
                        destinationInfo.Count((x) => { return x.Value.ItemType == CompareType.File || x.Value.ItemType == CompareType.Directory; });
            var count = 0;
            var AllResults = new List<ICompareResult>();
            // ######## PROCESS FILES
            // ******** process source files <Ok, New, Old, Update>
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            foreach (var sourceFileData in from item in sourceInfo
                                           where item.Value.ItemType == CompareType.File
                                           orderby item.Value.Name
                                           select item)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                // create compare result
                var compResult = new CompareResult()
                {
                    ItemType = CompareType.File,
                    SourceRootPath = sourcePath,
                    SourceRootFilename = sourceFileData.Value.Name,
                    SourceFileSizeInBytes = sourceFileData.Value.SizeInBytes,
                    SourceLastModifiedTimeUtc = sourceFileData.Value.LastModifiedTimeUtc,
                    DestinationRootPath = destinationPath,
                    DestinationRootFilename = sourceFileData.Value.Name
                };
                // get destination fileInfo
                var destFileInfo = FindRelativeFile(sourceFileData.Value.Name, destinationInfo);
                if (destFileInfo == null)
                {
                    // NEW
                    compResult.Action = CompareAction.New;
                    compResult.DestinationLastModifiedTimeUtc = DateTime.MinValue;
                    compResult.SourceFileSizeInBytes = 0;
                    compResult.DestinationRootFilename = "";
                    compResult.DestinationRootPath = "";
                }
                else
                {
                    if (IsTimeSpanLesserThan(sourcePath, destinationPath, destFileInfo.LastModifiedTimeUtc, sourceFileData.Value.LastModifiedTimeUtc, TimeSpan.FromSeconds(1)))
                    {
                        // UPDATE
                        compResult.Action = CompareAction.Update;
                        compResult.DestinationLastModifiedTimeUtc = destFileInfo.LastModifiedTimeUtc;
                    }
                    else if (IsTimeSpanGreaterThan(sourcePath, destinationPath, destFileInfo.LastModifiedTimeUtc, sourceFileData.Value.LastModifiedTimeUtc, TimeSpan.FromSeconds(1)))
                    {
                        // OLD
                        compResult.Action = CompareAction.Old;
                        compResult.DestinationLastModifiedTimeUtc = destFileInfo.LastModifiedTimeUtc;
                    }
                    else
                    {
                        // OK
                        compResult.Action = CompareAction.Ok;
                        compResult.DestinationLastModifiedTimeUtc = destFileInfo.LastModifiedTimeUtc;
                    }
                }
                // 
                AllResults.Add(compResult);
                // 
                feedback?.Invoke((double)(double)count++ / (double)total);
            }
            // ******** process destination files <Remove>
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            foreach (var fileInfo in from item in destinationInfo
                                     where item.Value.ItemType == CompareType.File
                                     orderby item.Value.Name
                                     select item)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                // get source fileInfo
                var sourceFileInfo = FindRelativeFile(fileInfo.Value.Name, sourceInfo);
                if (sourceFileInfo == null)
                {
                    // REMOVE
                    AllResults.Add(new CompareResult()
                    {
                        Action = CompareAction.Remove,
                        ItemType = CompareType.File,
                        DestinationRootPath = destinationPath,
                        DestinationRootFilename = fileInfo.Value.Name,
                        DestinationLastModifiedTimeUtc = fileInfo.Value.LastModifiedTimeUtc
                    });
                }
                // 
                feedback?.Invoke((double)(double)count++ / (double)total);
            }
            // ######## PROCESS DIRECTORIES
            // ******** process source directories <Ok, New>
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            foreach (var sourceFileData in from item in sourceInfo
                                           where item.Value.ItemType == CompareType.Directory
                                           orderby item.Value.Name
                                           select item)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                var destInfo = FindRelativeFile(sourceFileData.Value.Name, destinationInfo);
                if (destInfo == null)
                {
                    // NEW
                    AllResults.Add(new CompareResult()
                    {
                        ItemType = CompareType.Directory,
                        Action = CompareAction.New,
                        SourceRootPath = sourcePath,
                        SourceRootFilename = sourceFileData.Value.Name,
                        DestinationRootPath = destinationPath,
                        DestinationRootFilename = sourceFileData.Value.Name
                    });
                }
                else
                {
                    // OK
                    AllResults.Add(new CompareResult()
                    {
                        ItemType = CompareType.Directory,
                        Action = CompareAction.Ok,
                        SourceRootPath = sourcePath,
                        SourceRootFilename = sourceFileData.Value.Name,
                        DestinationRootPath = destinationPath,
                        DestinationRootFilename = sourceFileData.Value.Name
                    });
                }
                // 
                feedback?.Invoke((double)(double)count++ / (double)total);
            }
            // ******** process dest directories <Remove>
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            foreach (var destFileData in from item in destinationInfo
                                         where item.Value.ItemType == CompareType.Directory
                                         orderby item.Value.Name
                                         select item)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                var sInfo = FindRelativeFile(destFileData.Value.Name, sourceInfo);
                if (sInfo == null)
                {
                    // REMOVE
                    AllResults.Add(new CompareResult()
                    {
                        ItemType = CompareType.Directory,
                        Action = CompareAction.Remove,
                        DestinationRootPath = destinationPath,
                        DestinationRootFilename = destFileData.Value.Name
                    });
                }
                // 
                feedback?.Invoke((double)(double)count++ / (double)total);
            }
            // ******** return results
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            AllResults.Sort(CompareItemsForFinalSorting);
            return AllResults;
        }
        private static bool IsTimeSpanLesserThan(string sourcePath, string destinationPath, DateTime dateTime, DateTime dateTime_2, TimeSpan timeSpan)
        {
            if ((sourcePath == "") && (destinationPath == ""))
            {
                return dateTime < dateTime_2;
            }
            return (dateTime + timeSpan) < dateTime_2;
        }
        private static bool IsTimeSpanGreaterThan(string sourcePath, string destinationPath, DateTime dateTime, DateTime dateTime_2, TimeSpan timeSpan)
        {
            if ((sourcePath == "") && (destinationPath == ""))
            {
                return dateTime > dateTime_2;
            }
            return dateTime > (dateTime_2 + timeSpan);
        }
        private static int CompareItemsForFinalSorting(ICompareResult x, ICompareResult y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                return ((int)x.Action == (int)y.Action) ? 0 : (((int)x.Action < (int)y.Action) ? -1 : 1);

                //// this is less efficient, however, this technique will work with concepts beyond enums...
                //// notice this is built backwards, next time reverse the values(-1,1) and build the switch/if..else statements in order.
                //switch (x.Action)
                //{
                //    case CompareAction.Remove:
                //        if (y.Action == CompareAction.Remove) { return 0; }
                //        return 1;
                //    case CompareAction.Update:
                //        if (y.Action == CompareAction.Update) { return 0; }
                //        if (y.Action == CompareAction.Remove) { return -1; }
                //        return 1;
                //    case CompareAction.Old:
                //        if (y.Action == CompareAction.Old) { return 0; }
                //        if (y.Action == CompareAction.Remove) { return -1; }
                //        if (y.Action == CompareAction.Update) { return -1; }
                //        return 1;
                //    case CompareAction.New:
                //        if (y.Action == CompareAction.New) { return 0; }
                //        if (y.Action == CompareAction.Remove) { return -1; }
                //        if (y.Action == CompareAction.Update) { return -1; }
                //        if (y.Action == CompareAction.Old) { return -1; }
                //        return 1;
                //    case CompareAction.Ok:
                //        if (y.Action == CompareAction.Ok) { return 0; }
                //        if (y.Action == CompareAction.Remove) { return -1; }
                //        if (y.Action == CompareAction.Update) { return -1; }
                //        if (y.Action == CompareAction.Old) { return -1; }
                //        if (y.Action == CompareAction.New) { return -1; }
                //        return 1;
                //    case CompareAction.Unknown:
                //        if (y.Action == CompareAction.Unknown) { return 0; }
                //        if (y.Action == CompareAction.Remove) { return -1; }
                //        if (y.Action == CompareAction.Update) { return -1; }
                //        if (y.Action == CompareAction.Old) { return -1; }
                //        if (y.Action == CompareAction.New) { return -1; }
                //        if (y.Action == CompareAction.Ok) { return -1; }
                //        return 1; // technically, this line should be unreachable.
                //    default:
                //        return 1; // technically, this line should be unreachable.
                //}
            }
        }
        private static DataHolder FindRelativeFile(string candidateFilename, Dictionary<string, DataHolder> fileInfo)
        {
            if (fileInfo.ContainsKey(candidateFilename))
            {
                return fileInfo[candidateFilename];
            }
            return null;
        }
        private static string NormalizeName(string rootPath, string name)
        {
            if (rootPath.Equals(name, StringComparison.InvariantCultureIgnoreCase))
            {
                return "";
            }
            var result = name.Substring(rootPath.Length);
            return ((result.StartsWith("\\")) ? result.Substring(1) : result);
        }
        private static DateTime NormalizeDateTime(DateTime dt)
        {
            var newDate = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Kind);
            //if (dt.Millisecond >= 500) { newDate.AddSeconds(1); }
            return newDate;
        }
        private static Dictionary<string, DataHolder> GatherInformation(string rootPath, System.Threading.CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            // **** test for non-existent root directory
            if (!System.IO.Directory.Exists(rootPath))
            {
                var result = new Dictionary<string, DataHolder>();
                // add root directory
                result.Add(NormalizeName(rootPath, rootPath),
                           new DataHolder()
                           {
                               ItemType = CompareType.Directory,
                               Name = NormalizeName(rootPath, rootPath)
                           });
                return result;
            }
            var gatheredInfo = new Dictionary<string, DataHolder>();
            // **** test for root directory access

            // TODO: study this: it does not process files in hidden directories
            //if ((new System.IO.DirectoryInfo(rootPath).Attributes & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden)
            //{
            // **** process root directory
            // add all files
            foreach (var filename in System.IO.Directory.GetFiles(rootPath, "*", System.IO.SearchOption.AllDirectories))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                // test for false file
                if (System.IO.Path.GetFileNameWithoutExtension(filename) != "")
                {
                    // test for access
                    var dirInfo = new System.IO.DirectoryInfo(System.IO.Path.GetDirectoryName(filename));
                    // TODO: study this: do not process files in hidden directories
                    //if ((dirInfo.Attributes & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden)
                    //{
                    var rootFileInfo = new System.IO.FileInfo(filename);
                    gatheredInfo.Add(NormalizeName(rootPath, filename),
                                     new DataHolder()
                                     {
                                         ItemType = CompareType.File,
                                         Name = NormalizeName(rootPath, filename),
                                         LastModifiedTimeUtc = NormalizeDateTime(rootFileInfo.LastWriteTimeUtc),
                                         SizeInBytes = rootFileInfo.Length
                                     });
                    //}
                }
            }
            // add root directory
            gatheredInfo.Add(NormalizeName(rootPath, rootPath),
                             new DataHolder()
                             {
                                 ItemType = CompareType.Directory,
                                 Name = NormalizeName(rootPath, rootPath)
                             });
            // add all sub-directories
            foreach (var directoryname in System.IO.Directory.GetDirectories(rootPath, "*", System.IO.SearchOption.AllDirectories))
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                // test for access
                var dirInfo = new System.IO.DirectoryInfo(directoryname);

                // TODO: study this: it does not process hidden directories
                //if ((dirInfo.Attributes & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden)
                //{
                gatheredInfo.Add(NormalizeName(rootPath, directoryname),
                                 new DataHolder()
                                 {
                                     ItemType = CompareType.Directory,
                                     Name = NormalizeName(rootPath, directoryname)
                                 });
                //}
            }
            //}
            // **** return gathered results
            return gatheredInfo;
        }
        private static Dictionary<string, DataHolder> GatherInformation(IFileStore fileStore, System.Threading.CancellationToken cancelToken)
        {
            if (cancelToken.IsCancellationRequested)
            {
                return null;
            }
            var gatheredInfo = new Dictionary<string, DataHolder>();
            var uniquePaths = new HashSet<string>();
            // add all files & process unique directories
            foreach (var storeItem in fileStore.GetAll())
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                gatheredInfo.Add(storeItem.RootFilename,
                                 new DataHolder()
                                 {
                                     ItemType = CompareType.File,
                                     Name = storeItem.RootFilename,
                                     LastModifiedTimeUtc = NormalizeDateTime(storeItem.RootFileLastModifiedTimeUtc),
                                     SizeInBytes = storeItem.FileSize
                                 });
                var pathName = System.IO.Path.GetDirectoryName(storeItem.RootFilename);
                if (!uniquePaths.Contains(pathName))
                {
                    uniquePaths.Add(pathName);
                }
            }
            // add all sub-directories (this, may, include the root path "")
            foreach (var directoryName in uniquePaths)
            {
                if (cancelToken.IsCancellationRequested)
                {
                    return null;
                }
                gatheredInfo.Add(NormalizeName("", directoryName),
                                 new DataHolder()
                                 {
                                     ItemType = CompareType.Directory,
                                     Name = NormalizeName("", directoryName)
                                 });
            }
            // return gathered results
            return gatheredInfo;
        }
        #endregion
        #region Private Class: DataHolder
        [Serializable]
        private class DataHolder
        {
            public CompareType ItemType { get; set; }
            public string Name { get; set; }
            public DateTime LastModifiedTimeUtc { get; set; }
            public long SizeInBytes { get; set; }
        }
        #endregion
    }
}
