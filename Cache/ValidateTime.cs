using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Represents a time-based cache item validater.
    /// </summary>
    /// <typeparam name="T">The type of the payload carried by the cache item.</typeparam>
    [Serializable]
    public class ValidateTime<T>
        : dodSON.Core.Cache.ICacheValidater<T>
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the ValidateTime class.
        /// </summary>
        private ValidateTime()
        {
        }
        /// <summary>
        /// Initializes a new instance of the ValidateTime class.
        /// </summary>
        /// <param name="expirationDateTime">A <see cref="DateTime"/> which representing the expiration date for this validater.</param>
        public ValidateTime(DateTime expirationDateTime)
            : this()
        {
            ExpirationDateTime = expirationDateTime;
            _ExpirationDateDistance = ExpirationDateTime - DateTime.Now;
        }
        #endregion
        #region Private Fields
        private TimeSpan _ExpirationDateDistance = TimeSpan.Zero;
        private bool _MarkedInvalid = false;
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets, or sets, a <see cref="DateTime"/> which representing the expiration date for this validater.
        /// </summary>
        /// <remarks>IsValid() will be <b>true</b> as long as <see cref="DateTime.Now"/> is less than this <see cref="DateTime"/>.</remarks>
        public DateTime ExpirationDateTime { get; set; } = DateTime.MinValue;
        #endregion
        #region ICacheValidater<> Methods
        /// <summary>
        /// Gets a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.
        /// </summary>
        /// <param name="cacheItem">The ICacheItem whose validity is being tested.</param>
        /// <returns>Returns a value indicating whether the given cache item is valid or not. <b>True</b> indicates it is valid; otherwise <b>false</b>.</returns>
        public bool IsValid(ICacheItem<T> cacheItem) => (!_MarkedInvalid) && (DateTime.Now < ExpirationDateTime);
        /// <summary>
        /// Will move the <see cref="ExpirationDateTime"/> forward by the calculated time span given in the constructor.
        /// </summary>
        public void Reset() => Reset(DateTime.Now.Add(_ExpirationDateDistance));
        /// <summary>
        /// Will move the <see cref="ExpirationDateTime"/> forward by the calculated time span given in the constructor.
        /// </summary>
        /// <param name="expirationDateTime">The <see cref="DateTime"/> which representing the expiration date for this validater.</param>
        public void Reset(DateTime expirationDateTime)
        {
            _MarkedInvalid = false;
            ExpirationDateTime = expirationDateTime;
        }
        /// <summary>
        /// Sets the <see cref="ICacheItem{T}"/> invalid; this will cause it to be purged at the very next purge cycle.
        /// </summary>
        public void MarkInvalid() => _MarkedInvalid = true;
        #endregion
    }
}
