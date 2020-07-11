using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Provides an abstract base class with the all properties required to be processed by the <see cref="CacheProcessor"/> already implemented.
    /// </summary>
    public abstract class CacheProcessorItemBase
        : ICacheProcessorItem,
          ICacheProcessorItemAdvanced
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected CacheProcessorItemBase() { }
        /// <summary>
        /// Instantiates a <see cref="CacheProcessorItemBase"/> with the given <paramref name="key"/>, <paramref name="validater"/> and <paramref name="process"/>.
        /// </summary>
        /// <param name="key">The key representing this <see cref="ICacheProcessorItem"/>.</param>
        /// <param name="validater">The <see cref="ICacheValidater{ICacheProcessorItem}"/> used to determine if the <see cref="ICacheValidater{ICacheProcessorItem}"/> can be purged.</param>
        /// <param name="process">The <see cref="Action{ICacheProcessorItem}"/> which will be executed when the <see cref="ICacheProcessorItem"/> is purged from the cache.</param>
        public CacheProcessorItemBase(string key,
                                      ICacheValidater<ICacheProcessorItem> validater,
                                      Action<ICacheProcessorItem> process)
            : this()
        {
            if (string.IsNullOrWhiteSpace(key)) { throw new ArgumentNullException(nameof(key)); }
            Key = key;
            Validater = validater ?? throw new ArgumentNullException(nameof(validater));
            Process = process ?? throw new ArgumentNullException(nameof(process));
        }
        #endregion
        #region ICacheProcessorItem Methods
        /// <summary>
        /// The key representing this <see cref="ICacheProcessorItem"/>.
        /// </summary>
        public string Key { get; }
        /// <summary>
        /// The <see cref="Action{ICacheProcessorItem}"/> which will be executed when the <see cref="ICacheProcessorItem"/> is purged from the cache.
        /// </summary>
        public Action<ICacheProcessorItem> Process { get; }
        /// <summary>
        /// Get whether <see cref="Process"/> has been executed. <b>True</b> indicates that <see cref="Process"/> has been executed; otherwise, <b>false</b> indicates the <see cref="Process"/> has <u>not</u> been executed.
        /// </summary>
        public bool HasProcessExecuted { get; private set; } = false;
        /// <summary>
        /// The date and time this <see cref="ICacheProcessorItem"/> was created in Coordinated Universal Time (UTC).
        /// </summary>
        public DateTimeOffset CreatedTimeUtc { get; } = DateTimeOffset.UtcNow;
        /// <summary>
        /// The length of time the cache item has been in the cache.
        /// </summary>
        public TimeSpan CachedTime => DateTimeOffset.UtcNow - CreatedTimeUtc;
        /// <summary>
        /// The <see cref="ICacheValidater{ICacheProcessorItem}"/> used to determine if the <see cref="ICacheValidater{ICacheProcessorItem}"/> can be purged.
        /// </summary>
        public ICacheValidater<ICacheProcessorItem> Validater { get; private set; }
        #endregion
        #region ICacheProcessorItemAdvanced Methods
        bool ICacheProcessorItemAdvanced.HasProcessExecuted
        {
            set { HasProcessExecuted = value; }
        }
        /// <summary>
        /// Sets <see cref="ICacheValidater{ICacheProcessorItem}"/> used to determine if the <see cref="ICacheValidater{ICacheProcessorItem}"/> can be purged.
        /// </summary>
        ICacheValidater<ICacheProcessorItem> ICacheProcessorItemAdvanced.Validater
        {
            set { Validater = value ?? throw new ArgumentNullException(nameof(value)); }
        }
        #endregion
    }
}
