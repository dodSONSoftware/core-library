using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Represents a lock.
    /// </summary>
    public interface ILockableView
        : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value representing the state of the lock. <b>True</b> indicates the lock is engaged; otherwise, <b>false</b> means the lock is unlocked.
        /// </summary>
        bool IsLocked { get; }
    }
}
