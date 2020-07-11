using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Defines functionality for the processing of pipeline items within a pipeline.
    /// </summary>
    public interface IPipelineItem
    {
        /// <summary>
        /// Gets and sets a value indicating whether the pipeline should continue to process this pipeline item.
        /// </summary>
        bool ContinueProcessing { get; set; }
    }
}
