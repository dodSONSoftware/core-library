using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.PipeAndFilter
{
    /// <summary>
    /// Defines functionality for adding, inserting and removing filters and executing pipeline items.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// An event raised when an exception occurs while processing a pipeline item.
        /// </summary>
        event EventHandler<ExceptionEventArgs> ExceptionEvent;
        /// <summary>
        /// Occurs before each filter processes a pipeline item.
        /// </summary>
        event EventHandler<PreProcessFilterEventArgs> PreProcessFilterEvent;
        /// <summary>
        /// Adds a filter to the end of the filter list.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        void AddFilter(IFilter filter);
        /// <summary>
        /// Inserts a filter at the specified index.
        /// </summary>
        /// <param name="filterIndex">The zero-based index at which the filter should be inserted.</param>
        /// <param name="filter">The filter to insert.</param>
        void InsertFilter(int filterIndex, IFilter filter);
        /// <summary>
        /// Inserts a filter before the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to insert this filter before.</param>
        /// <param name="filter">The filter to insert.</param>
        void InsertFilterBefore(string filterName, IFilter filter);
        /// <summary>
        /// Inserts a filter after the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to insert this filter after.</param>
        /// <param name="filter">The filter to insert.</param>
        void InsertFilterAfter(string filterName, IFilter filter);
        /// <summary>
        /// Removes the filter by ordinal index.
        /// </summary>
        /// <param name="filterIndex">The zero-based index of the filter to remove.</param>
        /// <returns>The filter removed.</returns>
        IFilter RemoveFilter(int filterIndex);
        /// <summary>
        /// Removes the filter by name.
        /// </summary>
        /// <param name="filterName">The name of the filter to remove.</param>
        /// <returns>The filter removed.</returns>
        IFilter RemoveFilter(string filterName);
        /// <summary>
        /// Gets the number of filters in this instance.
        /// </summary>
        int FilterCount { get; }
        /// <summary>
        /// Gets all the names of the filters in this instance.
        /// </summary>
        IEnumerable<string> FilterNames { get; }
        /// <summary>
        /// Will execute the given pipeline item starting with the first filter.
        /// </summary>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        IPipelineItem Execute(IPipelineItem pipelineItem);
        /// <summary>
        /// Will execute the given pipeline item starting with the filter at the filter index.
        /// </summary>
        /// <param name="filterIndex">The index of the filter to start process.</param>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        IPipelineItem Execute(int filterIndex, IPipelineItem pipelineItem);
        /// <summary>
        /// Will execute the given pipeline item starting with the named filter.
        /// </summary>
        /// <param name="filterName">The name of the filter to start processing.</param>
        /// <param name="pipelineItem">The pipeline item to process.</param>
        /// <returns>The processed pipeline item.</returns>
        IPipelineItem Execute(string filterName, IPipelineItem pipelineItem);
    }
}
