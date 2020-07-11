using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Provides the base functionality for users to create a custom filter.
    /// </summary>
    public abstract class FilterBase
        : IFilter
    {
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the FilterBase class.
        /// </summary>
        protected FilterBase() { }
        #endregion
        #region IFilterBase<IPipelineItem> Methods
        /// <summary>
        /// The name of the filter.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Takes a pipeline item, processes it (possibly transforming it), and then returns it.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        public abstract IPipelineItem Process(IPipelineItem pipelineItem);
        #endregion
    }
}
