using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Provides data for the preprocessing filter event.
    /// </summary>
    public class PreProcessFilterEventArgs
        : EventArgs
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the PreProcessFilterEventArgs class.
        /// </summary>
        private PreProcessFilterEventArgs() { }
        /// <summary>
        /// Initializes a new instance of the PreProcessFilterEventArgs class.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item about to be processed.</param>
        /// <param name="filterIndex">The ordinal index of the filter about to be executed.</param>
        /// <param name="filterName">The name of the filter about to be executed.</param>
        public PreProcessFilterEventArgs(IPipelineItem pipelineItem,
                                         int filterIndex,
                                         string filterName)
            : this()
        {
            if (string.IsNullOrWhiteSpace(filterName)) { throw new ArgumentNullException(nameof(filterName)); }
            PipelineItem = pipelineItem ?? throw new ArgumentNullException(nameof(pipelineItem));
            FilterIndex = filterIndex;
            FilterName = filterName;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The pipeline item about to be processed.
        /// </summary>
        public IPipelineItem PipelineItem { get; } = default(IPipelineItem);
        /// <summary>
        /// The ordinal index of the filter about to be executed.
        /// </summary>
        public int FilterIndex { get; } = 0;
        /// <summary>
        /// The name of the filter about to be executed.
        /// </summary>
        public string FilterName { get; } = "";
        #endregion
    }
}
