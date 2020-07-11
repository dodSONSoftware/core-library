using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Common
{
	
	// TODO: add AcceptablePunctuationList and CapitalizedWordsList
	
    /// <summary>
    /// Represents statistical information used to generate randomized, yet reasonably structured, textual content.
    /// </summary>
    [Serializable]
    public class LoremIpsumConfiguration
    {
        #region Ctor
        private LoremIpsumConfiguration() { }
        /// <summary>
        /// Initializes a new LoremIpsumConfiguration class with the specified arguments.
        /// </summary>
        /// <param name="firstSentence">The first sentence element.</param>
        /// <param name="sentencePatterns">A dictionary representing statistical sentence patterns.</param>
        /// <param name="paragraphPatterns">A dictionary representing statistical paragraph patterns.</param>
        /// <param name="punctuationPatterns">A dictionary representing statistical punctuation patterns.</param>
        /// <param name="words">A dictionary containing all words and their relative counts.</param>
        public LoremIpsumConfiguration(string firstSentence,
                                       Dictionary<int, int> sentencePatterns,
                                       Dictionary<int, int> paragraphPatterns,
                                       Dictionary<char, int> punctuationPatterns,
                                       Dictionary<string, int> words)
            : this()
        {
            if (string.IsNullOrWhiteSpace(firstSentence)) { throw new ArgumentNullException(nameof(firstSentence)); }
            FirstSentence = firstSentence;
            SentencePatterns = sentencePatterns ?? throw new ArgumentNullException(nameof(sentencePatterns));
            ParagraphPatterns = paragraphPatterns ?? throw new ArgumentNullException(nameof(paragraphPatterns));
            PunctuationPatterns = punctuationPatterns ?? throw new ArgumentNullException(nameof(punctuationPatterns));
            Words = words ?? throw new ArgumentNullException(nameof(words));
        }
        #endregion
        #region Public Methods
        /// <summary>
        /// Returns the first sentence.
        /// </summary>
        public string FirstSentence { get; }
        /// <summary>
        /// Returns the dictionary representing statistical sentence patterns.
        /// </summary>
        public Dictionary<int, int> SentencePatterns { get; }
        /// <summary>
        /// Returns the dictionary representing statistical paragraph patterns.
        /// </summary>
        public Dictionary<int, int> ParagraphPatterns { get; }
        /// <summary>
        /// Returns the dictionary representing statistical punctuation patterns.
        /// </summary>
        public Dictionary<char, int> PunctuationPatterns { get; }
        /// <summary>
        /// Returns the dictionary containing all words and their relative counts.
        /// </summary>
        public Dictionary<string, int> Words { get; }
        #endregion
    }
}
