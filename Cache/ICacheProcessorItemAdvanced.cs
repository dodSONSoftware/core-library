using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Cache
{
    /// <summary>
    /// Defines advanced properties required to be processed by the <see cref="CacheProcessor"/>.
    /// </summary>
    public interface ICacheProcessorItemAdvanced
    {
        /// <summary>
        /// Set whether <see cref="ICacheProcessorItem.Process"/> has been executed. 
        /// <b>True</b> indicates that <see cref="ICacheProcessorItem.Process"/> has been executed; otherwise, <b>false</b> indicates the <see cref="ICacheProcessorItem.Process"/> has <u>not</u> been executed.
        /// </summary>
        bool HasProcessExecuted
        {
            set;
        }
        /// <summary>
        /// Sets <see cref="ICacheValidater{ICacheProcessorItem}"/> used to determine if the <see cref="ICacheValidater{ICacheProcessorItem}"/> can be purged.
        /// </summary>
        ICacheValidater<ICacheProcessorItem> Validater
        {
            set;
        }
    }
}
