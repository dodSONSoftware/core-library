using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Installation
{
    /// <summary>
    /// Provides control for install states.
    /// </summary>
    internal class InstallStateTracker
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the InstallStateTracker. All values will be false.
        /// </summary>
        public InstallStateTracker() { }
        /// <summary>
        /// Initializes a new instance of the InstallStateTracker using the specified value.
        /// </summary>
        /// <param name="newState">The initial new state.</param>
        /// <param name="updateState">The initial update state.</param>
        /// <param name="removeState">The initial remove state.</param>
        public InstallStateTracker(bool newState,
                                   bool updateState,
                                   bool removeState)
            : this()
        {
            IsNew = newState;
            IsUpdate = updateState;
            IsRemove = removeState;
        }
        /// <summary>
        /// Initializes a new instance of the InstallStateTracker using the specified <see cref="InstallStateTracker"/>. Copy constructor.
        /// </summary>
        /// <param name="state">The <see cref="InstallStateTracker"/> to copy.</param>
        public InstallStateTracker(InstallStateTracker state) : this(state.IsNew, state.IsUpdate, state.IsRemove) { }
        #endregion
        #region IStateChangeTracking Methods
        /// <summary>
        /// Indicates all flag are true.
        /// </summary>
        public bool IsMarked { get { return (IsNew && IsUpdate && IsRemove); } }
        /// <summary>
        /// Indicates all flag are false.
        /// </summary>
        public bool IsClear { get { return ((!IsNew) && (!IsUpdate) && (!IsRemove)); } }
        /// <summary>
        /// Indicates the package does not exist.
        /// </summary>
        public bool IsNew { get; private set; }
        /// <summary>
        /// Indicates the package can be removed and added.
        /// </summary>
        public bool IsUpdate { get; private set; }
        /// <summary>
        /// Indicates the package can be removed.
        /// </summary>
        public bool IsRemove { get; private set; }
        /// <summary>
        /// Sets IsNew to true.
        /// </summary>
        public void MarkNew() { IsNew = true; }
        /// <summary>
        /// Sets IsUpdate to true.
        /// </summary>
        public void MarkUpdate() { IsUpdate = true; }
        /// <summary>
        /// Sets IsRemove to true.
        /// </summary>
        public void MarkRemove() { IsRemove = true; }
        /// <summary>
        /// Sets all states to true.
        /// </summary>
        public void MarkAll()
        {
            IsNew = true;
            IsUpdate = true;
            IsRemove = true;
        }
        /// <summary>
        /// Sets IsNew to false.
        /// </summary>
        public void ClearNew() { IsNew = false; }
        /// <summary>
        /// Sets IsUpdate to false.
        /// </summary>
        public void ClearUpdate() { IsUpdate = false; }
        /// <summary>
        /// Sets IsRemove to false.
        /// </summary>
        public void ClearRemove() { IsRemove = false; }
        /// <summary>
        /// Sets all states to false.
        /// </summary>
        public void ClearAll()
        {
            IsNew = false;
            IsUpdate = false;
            IsRemove = false;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Returns a string representation of the InstallStateTracker class.
        /// </summary>
        /// <returns>A string representation of the InstallStateTracker class.</returns>
        public override string ToString()=>string.Format($"IsNew={IsNew}; IsUpdate={IsUpdate}; IsRemove={IsRemove}");
        #endregion
    }
}
