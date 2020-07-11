using System;

namespace dodSON.Core.Threading
{
    /// <summary>
    /// Provides notification that the current threading operation should be canceled.
    /// </summary>
    /// <remarks>
    /// <para>The <see cref="CancelThread"/> property indicates whether cancellation has been requested for this token.</para>
    /// <para>If this property is <b>True</b>, it only guarantees that cancellation has been requested. It is up to the derived type's <see cref="ThreadBase.OnExecute(ThreadCancelToken)"/> function to monitor and act when this property changes and terminate its process.</para>
    /// </remarks>
    [Serializable()]
    public class ThreadCancelToken
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the ThreadCancelToken class.
        /// </summary>
        public ThreadCancelToken() { }
        #endregion
        #region Public Methods
        /// <summary>
        /// Gets or sets whether cancellation has been requested for this token.
        /// </summary>
        /// <value><b>True</b> if cancellation has been requested for this token; otherwise <b>False</b>.</value>
        public bool CancelThread { get; set; } = false;
        #endregion
    }
}
