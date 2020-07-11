using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides methods to work with sequences.
    /// </summary>
    /// <example>
    /// The following code example will populate a dictionary with a few words and their associated weights; it will exercise the method RandomItemFromSequenceByWeight, demonstrating that Gamma should be chosen 10x more often than the other words.
    /// <br/><br/>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     var sequence = new Dictionary&lt;string, int&gt;();
    ///     sequence.Add("Alpha", 1);
    ///     sequence.Add("Beta", 1);
    ///     sequence.Add("Gamma", 10);
    ///     sequence.Add("Delta", 1);
    ///     sequence.Add("Epsilon", 1);
    ///     var totalWords = 1000;
    ///     var foundSequence = new Dictionary&lt;string, int&gt;();
    ///     for (int i = 0; i &lt; totalWords; i++)
    ///     {
    ///         var randomWord = dodSON.Core.Common.SequenceHelper.RandomItemFromSequenceByWeight&lt;string&gt;(
    ///                                         sequence.Keys,
    ///                                         (str) =&gt; { return sequence[str]; });
    ///         if (foundSequence.ContainsKey(randomWord))
    ///         {
    ///             ++foundSequence[randomWord];
    ///         }
    ///         else
    ///         {
    ///             foundSequence.Add(randomWord, 1);
    ///         }
    ///         Console.WriteLine(randomWord);
    ///     }
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine();
    ///     foreach (var item in foundSequence)
    ///     {
    ///         Console.WriteLine(string.Format("{0:N0} {1} {2:N0}%", 
    ///                                             item.Value, 
    ///                                             item.Key, 
    ///                                             ((double)item.Value / (double)totalWords) * 100.0));
    ///     }
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// // (this is only the end of the output stream)
    ///
    /// // .
    /// // .
    /// // .
    /// // Gamma
    /// // Gamma
    /// // Delta
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Beta
    /// // Gamma
    /// // Gamma
    /// // Delta
    /// // Gamma
    /// // Alpha
    /// // Delta
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Epsilon
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Gamma
    /// // Alpha
    /// // Gamma
    /// // Alpha
    /// // Alpha
    /// // Beta
    /// // Gamma
    /// // Gamma
    /// // ------------------------
    /// // 
    /// // 733 Gamma 73%
    /// // 67 Delta 7%
    /// // 62 Beta 6%
    /// // 66 Alpha 7%
    /// // 72 Epsilon 7%
    /// // 
    /// // ------------------------
    /// // press anykey...
    /// </code>
    /// <br/>
    /// <font size="4"><b>Example</b></font>
    /// <br/><br/>
    /// The following example will simulate tossing a pair of 6-sided dice.
    /// <br/><br/>
    /// <para>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create all the pip values and their statistical frequency for the available values of a random toss of two 6-sided dice.
    ///     var dicePipsAndWeights = new Dictionary&lt;int, int&gt;();
    ///     dicePipsAndWeights.Add(2, 1);
    ///     dicePipsAndWeights.Add(3, 2);
    ///     dicePipsAndWeights.Add(4, 3);
    ///     dicePipsAndWeights.Add(5, 4);
    ///     dicePipsAndWeights.Add(6, 5);
    ///     dicePipsAndWeights.Add(7, 6);
    ///     dicePipsAndWeights.Add(8, 5);
    ///     dicePipsAndWeights.Add(9, 4);
    ///     dicePipsAndWeights.Add(10, 3);
    ///     dicePipsAndWeights.Add(11, 2);
    ///     dicePipsAndWeights.Add(12, 1);
    ///     
    ///     // the number of tosses to simulate and a dictionary to hold the results
    ///     var totalDiceTosses = 10240;
    ///     var foundSequence = new Dictionary&lt;int, int&gt;();
    ///     
    ///     // simulate dice
    ///     for (int i = 0; i &lt; totalDiceTosses; i++)
    ///     {
    ///         // get, weighted, random dice toss
    ///         var randomDiceToss = dodSON.Core.Common.SequenceHelper.RandomItemFromSequenceByWeight&lt;int&gt;(
    ///                                             dicePipsAndWeights.Keys,
    ///                                             (str) =&gt; { return dicePipsAndWeights[str]; });
    ///     
    ///         // process dice toss
    ///         if (foundSequence.ContainsKey(randomDiceToss))
    ///         {
    ///             // increment existing dice toss' count
    ///             ++foundSequence[randomDiceToss];
    ///         }
    ///         else
    ///         {
    ///             // create new dice toss
    ///             foundSequence.Add(randomDiceToss, 1);
    ///         }
    ///     
    ///         // display random dice toss
    ///         Console.WriteLine(randomDiceToss);
    ///     }
    ///     
    ///     // display results
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine(string.Format("Total Dice Tosses= {0:N0}", totalDiceTosses));
    ///     Console.WriteLine();
    ///     foreach (var item in from x in foundSequence
    ///                          orderby x.Key
    ///                          select x)
    ///     {
    ///         Console.WriteLine(string.Format("Dice Toss= {0,2} {1,3:N0}% {2,6:N0}", 
    ///                                                 item.Key,
    ///                                                 (((double)item.Value / (double)totalDiceTosses) * 100.0),
    ///                                                 item.Value));
    ///     }
    ///     Console.WriteLine();
    ///     Console.WriteLine("------------------------");
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// }
    /// 
    /// // This code produces output similar to the following:
    /// // (this is only the end of the output stream)
    ///
    /// // .
    /// // .
    /// // .
    /// // 7
    /// // 7
    /// // 9
    /// // 12
    /// // 2
    /// // 10
    /// // 9
    /// // 7
    /// // 10
    /// // 8
    /// // 7
    /// // 10
    /// // 5
    /// // 4
    /// // 8
    /// // 2
    /// // ------------------------
    /// // Total Dice Tosses= 10,240
    /// // 
    /// // Dice Toss=  2   3%    284
    /// // Dice Toss=  3   6%    638
    /// // Dice Toss=  4   8%    854
    /// // Dice Toss=  5  11%  1,145
    /// // Dice Toss=  6  14%  1,402
    /// // Dice Toss=  7  16%  1,627
    /// // Dice Toss=  8  14%  1,417
    /// // Dice Toss=  9  12%  1,179
    /// // Dice Toss= 10   9%    871
    /// // Dice Toss= 11   6%    578
    /// // Dice Toss= 12   2%    245
    /// // 
    /// // ------------------------
    /// // press anykey...
    /// </code>
    /// <br/>
    ///  <b>Other Run Results:</b>
    /// <code>
    /// // ------------------------
    /// // Total Dice Tosses= 36,099,228
    /// // 
    /// // Dice Toss=  2   3%  1,001,702
    /// // Dice Toss=  3   6%  2,003,858
    /// // Dice Toss=  4   8%  3,009,127
    /// // Dice Toss=  5  11%  4,010,274
    /// // Dice Toss=  6  14%  5,015,436
    /// // Dice Toss=  7  17%  6,016,244
    /// // Dice Toss=  8  14%  5,015,718
    /// // Dice Toss=  9  11%  4,012,217
    /// // Dice Toss= 10   8%  3,007,232
    /// // Dice Toss= 11   6%  2,005,696
    /// // Dice Toss= 12   3%  1,001,725
    /// // ------------------------
    /// 
    /// // ------------------------
    /// // Total Dice Tosses= 372,515,490
    /// // 
    /// // Dice Toss=  2   3%  10,346,665
    /// // Dice Toss=  3   6%  20,693,610
    /// // Dice Toss=  4   8%  31,040,889
    /// // Dice Toss=  5  11%  41,395,143
    /// // Dice Toss=  6  14%  51,741,920
    /// // Dice Toss=  7  17%  62,083,067
    /// // Dice Toss=  8  14%  51,728,974
    /// // Dice Toss=  9  11%  41,391,255
    /// // Dice Toss= 10   8%  31,048,556
    /// // Dice Toss= 11   6%  20,698,815
    /// // Dice Toss= 12   3%  10,346,597
    /// // ------------------------
    /// 
    /// // ------------------------
    /// // Total Dice Tosses= 1,335,001,967
    /// // 
    /// // Dice Toss=  2   3%   37,083,229
    /// // Dice Toss=  3   6%   74,182,080
    /// // Dice Toss=  4   8%  111,249,458
    /// // Dice Toss=  5  11%  148,325,836
    /// // Dice Toss=  6  14%  185,417,764
    /// // Dice Toss=  7  17%  222,493,457
    /// // Dice Toss=  8  14%  185,395,437
    /// // Dice Toss=  9  11%  148,336,228
    /// // Dice Toss= 10   8%  111,254,831
    /// // Dice Toss= 11   6%   74,170,892
    /// // Dice Toss= 12   3%   37,092,756
    /// // ------------------------
    /// </code>
    /// </example>
    public static class SequenceHelper
    {
        #region Private Fields
        private static Random _RndNumberGenerator = null;
        #endregion
        #region Public Methods
        /// <summary>
        /// Will return a random item from a sequence taking the weight of the item into account. The greater the weight, the more likely it will be returned.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="sequence">The sequence of items to randomly choose from.</param>
        /// <param name="itemWeightFunction">A function that should return the weight of the given item.</param>
        /// <returns>An item chosen at random, from the <paramref name="sequence"/>, taking it's weight, from the <paramref name="itemWeightFunction"/>, into account.</returns>
        public static T RandomItemFromSequenceByWeight<T>(IEnumerable<T> sequence, Func<T, int> itemWeightFunction)
        {
            // lazy-load random number generator, once
            if (_RndNumberGenerator == null) { _RndNumberGenerator = new Random(); }
            // this stores sum of weights of all elements before current
            int totalWeight = 0;
            T selected = default;
            foreach (var item in sequence)
            {
                // current item weight
                int weight = itemWeightFunction(item);
                int rnd = _RndNumberGenerator.Next(totalWeight + weight);
                // probability this is the weight / (totalWeight + weight)
                if (rnd >= totalWeight)
                {
                    // it is the probability of discarding last selected item and selecting current one instead
                    selected = item;
                }
                // increase weight sum
                totalWeight += weight;
            }
            //
            return selected;
        }
        #endregion
    }
}
