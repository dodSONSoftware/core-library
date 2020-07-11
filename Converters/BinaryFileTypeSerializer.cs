using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Converters
{
    /// <summary>
    /// Provides methods to load and save any serializable <see cref="Type"/>, to, and from, a file using binary serialization.
    /// </summary>
    [Serializable]
    public class BinaryFileTypeSerializer
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the BinaryFileTypeSerializer.
        /// </summary>
        public BinaryFileTypeSerializer() { }
        #endregion
        #region Public Methods
        /// <summary>
        /// Writes the <paramref name="source"/> instance, of type <typeparamref name="T"/>, as binary data to the file with the given <paramref name="filename"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the instance.</typeparam>
        /// <param name="source">The instance to save.</param>
        /// <param name="filename">The name of the file to save the binary data to.</param>
        /// <param name="overWrite">Determines whether to overwrite the file or append to it. <b>True</b> will overwrite an existing file, or create a new one; <b>false</b> will append an existing file, or create a new one.</param>
        public void Save<T>(T source, string filename, bool overWrite)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException(nameof(filename)); }
            // save T to file
            using (var sw = new System.IO.StreamWriter(filename, !overWrite))
            {
                sw.Write(Convert.ToBase64String((new TypeSerializer<T>()).ToByteArray(source)));
                sw.Flush();
            }
        }
        /// <summary>
        /// Writes the <paramref name="source"/> instance, of type <typeparamref name="T"/>, as binary data to the file with the given <paramref name="rootFilename"/> in the specified <paramref name="fileStore"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> of the instance.</typeparam>
        /// <param name="source">The instance to save.</param>
        /// <param name="fileStore">The <see cref="FileStorage.IFileStore"/> to save the file into.</param>
        /// <param name="rootFilename">The name of the file to save the binary data to.</param>
        public void Save<T>(T source, FileStorage.IFileStore fileStore, string rootFilename)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(rootFilename)) { throw new ArgumentNullException(nameof(rootFilename)); }
            if (fileStore == null) { throw new ArgumentNullException(nameof(fileStore)); }
            // save T to file
            var tempFilename = System.IO.Path.GetTempFileName();
            try
            {
                // save file to temp file
                Save(source, tempFilename, true);
                // add temp file to file store
                var fileInfo = new System.IO.FileInfo(tempFilename);
                fileStore.Add(rootFilename, tempFilename, fileInfo.LastWriteTimeUtc, fileInfo.Length);
                fileStore.Save(false);
            }
            finally
            {
                // delete temp file
                if (System.IO.File.Exists(tempFilename)) { System.IO.File.Delete(tempFilename); }
            }
        }
        /// <summary>
        /// Reads the contents of the file, specified by the <paramref name="filename"/>, and converts it from binary data into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the contents of the <paramref name="filename"/> into.</typeparam>
        /// <param name="filename">The name of the file to convert.</param>
        /// <returns>The contents of the file, specified by the <paramref name="filename"/>, converted from binary data into an instance of type <typeparamref name="T"/>.</returns>
        public T Load<T>(string filename)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(filename)) { throw new ArgumentNullException("filename", "Parameter filename cannot be null or whitespace."); }
            if (!System.IO.File.Exists(filename)) { throw new System.IO.FileNotFoundException(string.Format("File not found. Filename={0}", filename), filename); }
            // load T from file
            using (var sr = new System.IO.StreamReader(filename))
            {
                return (new dodSON.Core.Converters.TypeSerializer<T>()).FromByteArray(Convert.FromBase64String(sr.ReadToEnd()));
            }
        }
        /// <summary>
        /// Reads the contents of the file, specified by the <paramref name="rootFilename"/>, from the <paramref name="fileStore"/> and converts it from binary data into an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to convert the contents of the <paramref name="rootFilename"/> into.</typeparam>
        /// <param name="rootFilename">The name of the file to convert.</param>
        /// <param name="fileStore">The <see cref="FileStorage.IFileStore"/> to read the file from.</param>
        /// <returns>The contents of the file, specified by the <paramref name="rootFilename"/>, from the <paramref name="fileStore"/> and converted from binary data into an instance of type <typeparamref name="T"/>.</returns>
        public T Load<T>(string rootFilename, FileStorage.IFileStore fileStore)
        {
            // check for errors
            if (string.IsNullOrWhiteSpace(rootFilename)) { throw new ArgumentNullException(nameof(rootFilename)); }
            if (fileStore == null) { throw new ArgumentNullException(nameof(fileStore)); }
            if (!fileStore.Contains(rootFilename)) { throw new System.IO.FileNotFoundException(string.Format("File not found. Filename={0}", rootFilename), rootFilename); }
            //
            var results = default(T);
            var fileStoreItem = fileStore.Get(rootFilename);
            var extractedFile = "";
            if (fileStoreItem != null)
            {
                try
                {
                    extractedFile = fileStore.Extract(System.IO.Path.GetTempPath(), fileStoreItem, true);
                    results = Load<T>(extractedFile);
                }
                finally
                {
                    // delete temp file
                    if (System.IO.File.Exists(extractedFile)) { System.IO.File.Delete(extractedFile); }
                }
            }
            return results;
        }
        #endregion
    }
}
