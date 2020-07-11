using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Allows control of a lock.
    /// </summary>
    public interface ILockable
        : ILockableView
    {
        /// <summary>
        /// Will set <see cref="ILockableView.IsLocked"/> to <b>true</b>; engaging the lock.
        /// </summary>
        void Lock();
        /// <summary>
        /// Will set <see cref="ILockableView.IsLocked"/> to <b>false</b>; disengaging the lock.
        /// </summary>
        void Unlock();
        /// <summary>
        /// Will set <see cref="ILockableView.IsLocked"/> to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to set the <see cref="ILockableView.IsLocked"/> to.</param>
        void SetLock(bool value);
    }
}
