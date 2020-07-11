
namespace dodSON.Core.Common
{
    /// <summary>
    /// Represents commonly tracked states.
    /// </summary>
    public interface IStateChangeView
        : System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Represents the state of change.
        /// </summary>
        bool IsDirty { get; }
        /// <summary>
        /// Represents the state of age.
        /// </summary>
        bool IsNew { get; }
        /// <summary>
        /// Represents the state of existence.
        /// </summary>
        bool IsDeleted { get; }
    }
}
