using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Defines functionality for the execution of filters.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// The name of the filter.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Takes a pipeline item, processes it (possibly transforming it), and then returns it.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        IPipelineItem Process(IPipelineItem pipelineItem);
    }
}
