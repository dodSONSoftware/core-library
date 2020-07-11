using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Provides data when an exception occurs while processing pipeline items.
    /// </summary>
    public class ExceptionEventArgs
        : EventArgs
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the ExceptionEventArgs class.
        /// </summary>
        private ExceptionEventArgs() { }
        /// <summary>
        /// Initializes a new instance of the ExceptionEventArgs class.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item being processed when the exception occurred.</param>
        /// <param name="filterIndex">The index of the filter that was processing the <paramref name="pipelineItem"/>.</param>
        /// <param name="filterName">The name of the filter that was processing the <paramref name="pipelineItem"/>.</param>
        /// <param name="ex">The exception.</param>
        public ExceptionEventArgs(IPipelineItem pipelineItem,
                                  int filterIndex,
                                  string filterName,
                                  Exception ex)
            : this()
        {
            PipelineItem = pipelineItem ?? throw new ArgumentNullException(nameof(pipelineItem));
            FilterIndex = filterIndex;
            FilterName = filterName;
            Exception = ex;
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// The pipeline item being process when the exception occurred.
        /// </summary>
        public IPipelineItem PipelineItem { get; } = default(IPipelineItem);
        /// <summary>
        /// The index of the filter that was processing the <see cref="PipelineItem"/>.
        /// </summary>
        public int FilterIndex { get; } = -1;
        /// <summary>
        /// The name of the filter that was processing the <see cref="PipelineItem"/>.
        /// </summary>
        public string FilterName { get; } = "";
        /// <summary>
        /// The exception.
        /// </summary>
        public Exception Exception { get; } = null;
        #endregion
    }
}
