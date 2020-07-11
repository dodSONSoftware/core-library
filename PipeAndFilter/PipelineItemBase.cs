using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Provides base functionality for a pipeline item.
    /// </summary>
    public abstract class PipelineItemBase
        : IPipelineItem
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the PipelineItemBase class
        /// </summary>
        protected PipelineItemBase()
        {
        }
        #endregion
        #region IPipelineItem Methods
        /// <summary>
        /// Gets and sets a value indicating whether the pipeline should continue to process this pipeline item.
        /// </summary>
        public bool ContinueProcessing { get; set; } = true;
        #endregion
    }
}
