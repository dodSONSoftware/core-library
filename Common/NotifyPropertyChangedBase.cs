using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// An abstract base class which contains the plumbing for the <see cref="INotifyPropertyChanged"/> event.
    /// </summary>
    public abstract class NotifyPropertyChangedBase
        : INotifyPropertyChanged
    {
        #region Ctor
        /// <summary>
        /// Instantiates a new instance of the <see cref="NotifyPropertyChangedBase"/>.
        /// </summary>
        protected NotifyPropertyChangedBase() { }
        #endregion
        #region INotifyPropertyChanged Methods
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Sends notification that a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        /// <summary>
        /// Updates the <paramref name="storage"/> with the <paramref name="value"/> and calls the <see cref="INotifyPropertyChanged.PropertyChanged"/> event. Returns <b>true</b> if the property has changed; otherwise, <b>false</b>.
        /// </summary>
        /// <typeparam name="T">The type of the property being set.</typeparam>
        /// <param name="storage">A reference to the property being set.</param>
        /// <param name="value">The value to set the property to.</param>
        /// <param name="propertyName">The name of the property being set.</param>
        /// <returns>Whether the property has changed. <b>True</b> indicates the property has changed; otherwise, <b>false</b> indicates the property has not changed.</returns>
        protected bool SetProperty<T>(ref T storage,
                                      T value,
                                      [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                // object not changed
                return false;
            }
            // update object
            storage = value;
            // notify that there was a change
            OnPropertyChanged(propertyName);
            // object changed
            return true;
        }
        #endregion
    }
}
