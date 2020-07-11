
namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides control over commonly tracked states.
    /// </summary>
    public interface IStateChangeTracking
        : IStateChangeView
    {
        /// <summary>
        /// Sets IsDirty to true.
        /// </summary>
        void MarkDirty();
        /// <summary>
        /// Sets IsNew to true.
        /// </summary>
        void MarkNew();
        /// <summary>
        /// Sets IsDeleted to true.
        /// </summary>
        void MarkDeleted();
        /// <summary>
        /// Sets all states to true.
        /// </summary>
        void MarkAll();
        /// <summary>
        /// Sets IsDirty to false.
        /// </summary>
        void ClearDirty();
        /// <summary>
        /// Sets IsNew to false.
        /// </summary>
        void ClearNew();
        /// <summary>
        /// Sets IsDeleted to false.
        /// </summary>
        void ClearDeleted();
        /// <summary>
        /// Sets all states to false.
        /// </summary>
        void ClearAll();
    }
}
