using System;

namespace dodSON.Core.Packaging
{
    /// <summary>
    /// Defines a single, connected package.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The package, which should be some form a single file, file container, like a zip file, has been extracted from the package file store, to a temporary location, and automatically connected to.
    /// In order to save and return this temporarily extracted zip file back to the package store it was extracted from, the <see cref="Close"/> method must be called.
    /// <br/><br/>
    /// This type implements the <see cref="IDisposable"/> interface which ensures that the <see cref="Close"/> method is called. 
    /// Best practice would be to use a <b>using(...)</b> statement; otherwise, you must call <see cref="Close"/> explicitly.
    /// </para>
    /// </remarks>
    public interface IConnectedPackage
          : IDisposable
    {
        /// <summary>
        /// The <see cref="FileStorage.IFileStore"/> containing all the files for this package.
        /// </summary>
        /// <remarks>
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to; this property is that <see cref="FileStorage.ICompressedFileStore"/>. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        FileStorage.IFileStore FileStore { get; }
        /// <summary>
        /// The package configuration representing this package.
        /// </summary>
        IPackageConfiguration PackageConfiguration { get; }
        /// <summary>
        /// <para>Indicates whether this package is opened or closed.</para>
        /// <para><b>True</b> indicates the package is open, meaning the file store is connected to a zip file extracted from the package store; otherwise <b>false</b>, the package is closed and the file store has been saved and the extracted zip file deleted.</para>
        /// </summary>
        /// <remarks>    
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        bool IsOpen { get; }
        /// <summary>
        /// Will save the package file store, clean up residual files and close the package.
        /// </summary>
        /// <remarks>
        /// <para>The package, which should be some form of zip file, has been extracted from the package file store and automatically connected to. Which means it must be disconnected and deleted when finished.
        /// Therefore, it is imperative that the <see cref="Close"/> method is called so it can save the package file store and disconnect from it; allowing the <see cref="PackageProvider"/> to delete the extracted zip file, and any other residual files.</para>
        /// <para>This type implements <see cref="IDisposable"/>, best practice would be to use a <b>using(...)</b> statement. Otherwise, you must call <see cref="Close"/>.</para>
        /// </remarks>
        void Close();
    }
}
