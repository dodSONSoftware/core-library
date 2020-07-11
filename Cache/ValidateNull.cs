using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents a settable cache item validater.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    [Serializable]
    public class ValidateNull<T>
        : dodSON.Core.Cache.ICacheValidater<T>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the ValidateNull class.
        /// </summary>
        private ValidateNull()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ValidateNull class.
        /// </summary>
        /// <param name="response">A <see cref="bool"/> which represents the validity for this validater.</param>
        public ValidateNull(bool response)
            : this()
        {
            Response = response;
        }
        #endregion
        #region Private Fields
        private bool _IsMarkedInvalid = false;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets, or sets, a <see cref="bool"/> which represents the validity for this validater.
        /// </summary>
        /// <remarks>IsValid() will always return this value.</remarks>
        public bool Response { get; set; } = false;
        #endregion
        #region dodSON.Core.Cache.ICacheValidater<> Methods
        /// <summary>
        /// Gets a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.
        /// </summary>
        /// <param name="cacheItem">The ICacheItem whose validity is being tested.</param>
        /// <returns>Returns a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.</returns>
        public bool IsValid(ICacheItem<T> cacheItem) => (!_IsMarkedInvalid) && Response;
        /// <summary>
        /// Really nothing to do here...
        /// </summary>
        public void Reset() => _IsMarkedInvalid = false;
        /// <summary>
        /// Sets the <see cref="ICacheItem{T}"/> invalid; this will cause it to be purged at the very next purge cycle.
        /// </summary>
        public void MarkInvalid() => _IsMarkedInvalid = true;
        #endregion
    }
}
