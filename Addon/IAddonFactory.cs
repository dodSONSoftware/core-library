using System;

namespace dodSON.Core.Addon
{
    /// <summary>
    /// Defines controls for loading, unloading and accessing <see cref="IAddon"/>s.
    /// </summary>
    public interface IAddonFactory
    {
        /// <summary>
        /// Returns whether the <see cref="IAddon"/> is loaded. Returns <b>true</b> if the <see cref="IAddon"/> is loaded; otherwise, <b>false</b>.
        /// </summary>
        bool IsLoaded { get; }
        /// <summary>
        /// Loads and returns the <see cref="IAddon"/>.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>.</returns>
        IAddon Load();
        /// <summary>
        /// Unloads the <see cref="IAddon"/>.
        /// </summary>
        void Unload();
        /// <summary>
        /// Returns the loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.
        /// </summary>
        /// <returns>The loaded <see cref="IAddon"/>; or null, <b>Nothing</b> in VB.NET, if the <see cref="IAddon"/> is not loaded.</returns>
        IAddon Addon { get; }
    }
}
