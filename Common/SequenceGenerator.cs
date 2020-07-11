using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides a sequence generator based on the provided sequence and item weight function and implements the <see cref="IEnumerable{T}"/> and <see cref="IEnumerator{T}"/> interfaces.
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> in the sequence.</typeparam>
    /// <example>
    /// The following code example will create the following sequence { A:1, B:1, C:5, D:1, E:1, F:1 } and 2 sequence generators; one which will allow repeat characters and the other which will not.
    /// <br/>
    /// It will generate 1,000,000 weighted-random values from each generator and record the statistics.
    /// Once the data is gathered, the statistics will be output.
    /// <br/>
    /// This demonstrates using the sequence generators to get a proper set a weighted-random values and the affect of the constructor's <b>allowRepeats</b> parameter has when set to <b>false</b>.
    /// <br/><br/>
    /// It should be noted that for a truly randomly-weighted sequence, the constructor's <b>allowRepeats</b> parameter must be <b>True</b>.
    /// <para>
    /// <br/>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main()
    /// {
    ///     // create sequence
    ///     var sequence = new Dictionary&lt;string, int&gt;()
    ///     {
    ///         { "A", 1 },
    ///         { "B", 1 },
    ///         { "C", 5 },
    ///         { "D", 1 },
    ///         { "E", 1 },
    ///         { "F", 1 }
    ///     };
    /// 
    ///     // create generators
    ///     var generatorAllow = new dodSON.Core.Common.SequenceGenerator&lt;string&gt;(sequence.Keys, (x) =&gt; { return sequence[x]; }, true);
    ///     var generatorNotAllow = new dodSON.Core.Common.SequenceGenerator&lt;string&gt;(sequence.Keys, (x) =&gt; { return sequence[x]; }, false);
    /// 
    ///     // initialize
    ///     var valuesStatisticsAllow = new Dictionary&lt;string, int&gt;();
    ///     var valuesStatisticsNotAllow = new Dictionary&lt;string, int&gt;();
    ///     var total = 1_000_000;
    /// 
    ///     // produce values
    ///     for (int i = 0; i &lt; total; i++)
    ///     {
    ///         // get next value from generators
    ///         generatorAllow.MoveNext();
    ///         generatorNotAllow.MoveNext();
    /// 
    ///         // update allowed stats
    ///         if (valuesStatisticsAllow.ContainsKey(generatorAllow.Current)) { ++valuesStatisticsAllow[generatorAllow.Current]; }
    ///         else { valuesStatisticsAllow.Add(generatorAllow.Current, 1); }
    /// 
    ///         // update not allowed stats
    ///         if (valuesStatisticsNotAllow.ContainsKey(generatorNotAllow.Current)) { ++valuesStatisticsNotAllow[generatorNotAllow.Current]; }
    ///         else { valuesStatisticsNotAllow.Add(generatorNotAllow.Current, 1); }
    /// 
    ///         // update UI
    ///         Console.WriteLine($"{i + 1:N0}\tAllowed={generatorAllow.Current}, Not Allowed={generatorNotAllow.Current}");
    ///     }
    /// 
    ///     // update UI: Allowed
    ///     Console.WriteLine($"--------------------------------{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}{Environment.NewLine}======== Results for DUPLICATES ALLOWED{Environment.NewLine}--------------------------------");
    ///     foreach (var item in from x in valuesStatisticsAllow
    ///                          orderby x.Key ascending
    ///                          select x)
    ///     {
    ///         var valueStr = $"{item.Value:N0}".PadLeft(8);
    ///         var percentageStr = $"{((double)item.Value / total) * 100.0:N1}%".PadLeft(8);
    ///         Console.WriteLine($"{item.Key}  {valueStr}{percentageStr}");
    ///     }
    /// 
    ///     // update UI: Not Allowed
    ///     Console.WriteLine($"--------------------------------{Environment.NewLine}{Environment.NewLine}======== Results for DUPLICATES NOT ALLOWED{Environment.NewLine}--------------------------------");
    ///     foreach (var item in from x in valuesStatisticsNotAllow
    ///                          orderby x.Key ascending
    ///                          select x)
    ///     {
    ///         var valueStr = $"{item.Value:N0}".PadLeft(8);
    ///         var percentageStr = $"{((double)item.Value / total) * 100.0:N1}%".PadLeft(8);
    ///         Console.WriteLine($"{item.Key}  {valueStr}{percentageStr}");
    ///     }
    /// 
    ///     // 
    ///     Console.WriteLine($"--------------------------------{Environment.NewLine}");
    ///     Console.WriteLine("press any key&gt;");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// // (this is only the end of the output stream)
    /// 
    /// // .
    /// // .
    /// // .
    /// // 999,980 Allowed=C, Not Allowed=B
    /// // 999,981 Allowed=C, Not Allowed=D
    /// // 999,982 Allowed=A, Not Allowed=C
    /// // 999,983 Allowed=C, Not Allowed=A
    /// // 999,984 Allowed=C, Not Allowed=C
    /// // 999,985 Allowed=B, Not Allowed=E
    /// // 999,986 Allowed=A, Not Allowed=D
    /// // 999,987 Allowed=B, Not Allowed=C
    /// // 999,988 Allowed=E, Not Allowed=E
    /// // 999,989 Allowed=C, Not Allowed=C
    /// // 999,990 Allowed=C, Not Allowed=A
    /// // 999,991 Allowed=D, Not Allowed=C
    /// // 999,992 Allowed=F, Not Allowed=B
    /// // 999,993 Allowed=A, Not Allowed=C
    /// // 999,994 Allowed=F, Not Allowed=B
    /// // 999,995 Allowed=B, Not Allowed=C
    /// // 999,996 Allowed=F, Not Allowed=F
    /// // 999,997 Allowed=A, Not Allowed=D
    /// // 999,998 Allowed=C, Not Allowed=B
    /// // 999,999 Allowed=C, Not Allowed=E
    /// // 1,000,000       Allowed=B, Not Allowed=C
    /// // --------------------------------
    /// // 
    /// // 
    /// // 
    /// // ======== Results for DUPLICATES ALLOWED
    /// // --------------------------------
    /// // A   100,005   10.0%
    /// // B    99,612   10.0%
    /// // C   500,501   50.1%
    /// // D    99,681   10.0%
    /// // E    99,914   10.0%
    /// // F   100,287   10.0%
    /// // --------------------------------
    /// // 
    /// // ======== Results for DUPLICATES NOT ALLOWED
    /// // --------------------------------
    /// // A   128,627   12.9%
    /// // B   128,402   12.8%
    /// // C   357,149   35.7%
    /// // D   129,114   12.9%
    /// // E   128,458   12.8%
    /// // F   128,250   12.8%
    /// // --------------------------------
    /// // 
    /// // press any key&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// To generate proper mathematical sets of randomly-weighted values from the <b>sequence</b> and the <b>itemWeightFunction</b>, the constructor's <b>allowRepeats</b> parameter must be set to <b>True</b>.
    /// Setting the <b>allowRepeats</b> to <b>False</b> will force the system to select non-repeating values; 
    /// by doing so, you have interfered with the probability mathematics and the <b>sequence</b> and the <b>itemWeightFunction</b> will no longer generate proper sets of randomly-weighted values.
    /// <br/><br/>
    /// For example, if you have the following sequence { A:1, B:1, C:5, D:1, E:1, F:1 }, you would expect to receive 5 times more C's than the other letter and all of the other letters should be about equal. 
    /// However, if you set the constructor's <b>allowRepeats</b> parameter to <b>False</b>, then the mathematical probability of getting 5 times more C's will no longer be true since you cannot have 2 C's next to each other. 
    /// <br/><br/>
    /// When the constructor's <b>allowRepeats</b> parameter is set to <b>False</b> the system will force the selection of non-repeating values, thereby, interfering with the probability mathematics.
    /// </remarks>
    public class SequenceGenerator<T>
        : IEnumerable<T>, IEnumerator<T>
    {
        #region Ctor
        private SequenceGenerator() { }
        /// <summary>
        /// Instantiates a new instance of the <see cref="SequenceGenerator{T}"/> class.
        /// </summary>
        /// <param name="sequence">The sequence of items to randomly choose from.</param>
        /// <param name="itemWeightFunction">A function that should return the weight of the given item.</param>
        public SequenceGenerator(IEnumerable<T> sequence,
                                 Func<T, int> itemWeightFunction)
            : this(sequence, itemWeightFunction, true) { }
        /// <summary>
        /// Instantiates a new instance of the <see cref="SequenceGenerator{T}"/> class.
        /// </summary>
        /// <param name="sequence">The sequence of items to randomly choose from.</param>
        /// <param name="itemWeightFunction">A function that should return the weight of the given item.</param>
        /// <param name="allowRepeats">
        /// Determines if an item in the list can be selected more than once in a row. <b>True</b> allows repeated values; <b>False</b> will not.
        /// <br/>
        /// It should be noted that for a truly randomly-weighted sequence, the <paramref name="allowRepeats"/> parameter must be <b>True</b>.
        /// </param>
        /// <remarks>
        /// To generate proper mathematical sets of randomly-weighted values from the <b>sequence</b> and the <b>itemWeightFunction</b>, the constructor's <b>allowRepeats</b> parameter must be set to <b>True</b>.
        /// Setting the <b>allowRepeats</b> to <b>False</b> will force the system to select non-repeating values; 
        /// by doing so, you have interfered with the probability mathematics and the <b>sequence</b> and the <b>itemWeightFunction</b> will no longer generate proper sets of randomly-weighted values.
        /// <br/><br/>
        /// For example, if you have the following sequence { A:1, B:1, C:5, D:1, E:1, F:1 }, you would expect to receive 5 times more C's than the other letter and all of the other letters should be about equal. 
        /// However, if you set the constructor's <b>allowRepeats</b> parameter to <b>False</b>, then the mathematical probability of getting 5 times more C's will no longer be true since you cannot have 2 C's next to each other. 
        /// <br/><br/>
        /// When the constructor's <b>allowRepeats</b> parameter is set to <b>False</b> the system will force the selection of non-repeating values, thereby, interfering with the probability mathematics.
        /// </remarks>
        public SequenceGenerator(IEnumerable<T> sequence,
                                 Func<T, int> itemWeightFunction,
                                 bool allowRepeats)
            : this()
        {
            Sequence = sequence ?? throw new ArgumentNullException(nameof(sequence));
            if (sequence.Count() < 2) { throw new ArgumentException("Must be at least 2 elements in the [sequence] parameter.", nameof(sequence)); }
            _ItemWeightFunction = itemWeightFunction ?? throw new ArgumentNullException(nameof(itemWeightFunction));
            AllowRepeats = allowRepeats;
        }
        #endregion
        #region Private Fields
        private readonly Func<T, int> _ItemWeightFunction;
        #endregion
        #region Public Properties
        /// <summary>
        /// The sequence of items to randomly choose from.
        /// </summary>
        public IEnumerable<T> Sequence { get; private set; }
        /// <summary>
        /// Determines if an item in the list can be selected more than once in a row. <b>True</b> allows repeated values; <b>false</b> will not.
        /// </summary>
        /// <remarks>
        /// To generate proper mathematical sets of randomly-weighted values from the <b>sequence</b> and the <b>itemWeightFunction</b>, the constructor's <b>allowRepeats</b> parameter must be set to <b>True</b>.
        /// Setting the <b>allowRepeats</b> to <b>False</b> will force the system to select non-repeating values; 
        /// by doing so, you have interfered with the probability mathematics and the <b>sequence</b> and the <b>itemWeightFunction</b> will no longer generate proper sets of randomly-weighted values.
        /// <br/><br/>
        /// For example, if you have the following sequence { A:1, B:1, C:5, D:1, E:1, F:1 }, you would expect to receive 5 times more C's than the other letter and all of the other letters should be about equal. 
        /// However, if you set the constructor's <b>allowRepeats</b> parameter to <b>False</b>, then the mathematical probability of getting 5 times more C's will no longer be true since you cannot have 2 C's next to each other. 
        /// <br/><br/>
        /// When the constructor's <b>allowRepeats</b> parameter is set to <b>False</b> the system will force the selection of non-repeating values, thereby, interfering with the probability mathematics.
        /// </remarks>
        public bool AllowRepeats { get; private set; }
        #endregion
        #region IEnumerable<T> Methods
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        public T Current { get; private set; } = default;
        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        object IEnumerator.Current => Current;
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }
        /// <summary>
        /// Advances the enumerator to the next element of the collection and gets the next randomly-weighted value.
        /// </summary>
        /// <returns>The next randomly-weighted value.</returns>
        public bool MoveNext()
        {
            T candidate = default;
            do
            {
                candidate = SequenceHelper.RandomItemFromSequenceByWeight(Sequence, _ItemWeightFunction);
            } while (!AllowRepeats && (candidate.Equals(Current)));
            Current = candidate;
            return true;
        }
        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public void Reset() { Current = default; }
        #endregion
        #region IEnumerable<T> Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() { return this; }
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() { return this; }
        #endregion
    }
}
