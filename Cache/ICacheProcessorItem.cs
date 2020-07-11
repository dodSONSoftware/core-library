using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Defines the properties required to be processed by the <see cref="CacheProcessor"/>.
    /// </summary>
    public interface ICacheProcessorItem
    {
        /// <summary>
        /// The key representing this <see cref="ICacheProcessorItem"/>.
        /// </summary>
        string Key
        {
            get;
        }
        /// <summary>
        /// The <see cref="Action{ICacheProcessorItem}"/> which will be executed when the <see cref="ICacheProcessorItem"/> is purged from the cache.
        /// </summary>
        Action<ICacheProcessorItem> Process
        {
            get;
        }
        /// <summary>
        /// Get whether <see cref="Process"/> has been executed. <b>True</b> indicates that <see cref="Process"/> has been executed; otherwise, <b>false</b> means the <see cref="Process"/> has <u>not</u> been executed.
        /// </summary>
        bool HasProcessExecuted
        {
            get;
        }
        /// <summary>
        /// The date and time this <see cref="ICacheProcessorItem"/> was created in Coordinated Universal Time (UTC).
        /// </summary>
        DateTimeOffset CreatedTimeUtc
        {
            get;
        }
        /// <summary>
        /// The length of time the cache item has been in the cache.
        /// </summary>
        TimeSpan CachedTime
        {
            get;
        }
        /// <summary>
        /// The <see cref="ICacheValidater{ICacheProcessorItem}"/> used to determine if the <see cref="ICacheValidater{ICacheProcessorItem}"/> can be purged.
        /// </summary>
        ICacheValidater<ICacheProcessorItem> Validater
        {
            get;
        }
    }
}
