using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Preprocesses text for <see cref="MotionPreprocessor"/> by removing tags and parsing tag ranges.
    /// Caches results to avoid redundant processing.
    /// </summary>
    internal class MotionPreprocessor : ITextPreprocessor
    {
        /// <summary>
        /// Gets the input text that was last processed.
        /// </summary>
        public string LastInputText { get; private set; }

        /// <summary>
        /// Gets the processed text with tags removed.
        /// </summary>
        public string ProcessedText { get; private set; }

        // Instance of the tag parser used for processing.
        private readonly MotionParser tagParser;
        // Cached collection of tag ranges from the parsed text.
        private IReadOnlyCollection<TagRange> cachedRanges;
        // Cached tag range for quick lookup when retrieving tag effects.
        private TagRange cachedRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionPreprocessor"/> class.
        /// </summary>
        /// <param name="validator">An instance of <see cref="ITagValidator"/> used for validating tags.</param>
        public MotionPreprocessor(ITagValidator validator)
        {
            tagParser = new MotionParser(validator);
        }

        /// <summary>
        /// Preprocesses the input text by removing valid tags and parsing tag ranges.
        /// The result is cached to avoid redundant processing.
        /// </summary>
        /// <param name="inputText">The text to preprocess.</param>
        /// <returns>The processed text with tags removed.</returns>
        public string PreprocessText(string inputText)
        {
            if (string.IsNullOrEmpty(inputText))
            {
                cachedRange = null;
                cachedRanges = null;
                return string.Empty;
            }

            // Reprocess only if the input has changed or cache is empty.
            if (inputText != LastInputText || cachedRanges == null || cachedRange == null)
            {
                ProcessedText = tagParser.RemoveTags(inputText);
                cachedRanges = tagParser.Parse(inputText);
                LastInputText = inputText;
            }

            return ProcessedText;
        }

        /// <summary>
        /// Retrieves the tags affecting the specified index in the processed text.
        /// </summary>
        /// <param name="index">The index within the processed text.</param>
        /// <returns>
        /// A read-only collection of <see cref="TagData"/> affecting the specified index,
        /// or <c>null</c> if no range applies.
        /// </returns>
        public IReadOnlyCollection<TagData> GetTagEffectsAtIndex(int index)
        {
            UpdateCacheIfNeeded(index);
            if (cachedRange == null)
                return null;

            return cachedRange.Tags;
        }

        /// <summary>
        /// Updates the cached tag range if the specified index falls outside the current cached range.
        /// </summary>
        /// <param name="index">The character index to check.</param>
        private void UpdateCacheIfNeeded(int index)
        {
            if (cachedRanges == null)
                return;

            // If the current cached range covers the index, no need to update.
            if (cachedRange != null && index >= cachedRange.StartIndex && index <= cachedRange.EndIndex)
                return;

            // Otherwise, search for the appropriate range.
            cachedRange = cachedRanges.FirstOrDefault(range => range.StartIndex <= index && index <= range.EndIndex);
        }

        /// <summary>
        /// Clears all cached data so that the next call to preprocess text will reprocess it.
        /// </summary>
        public void ClearCache()
        {
            LastInputText = string.Empty;
            ProcessedText = string.Empty;
            cachedRange = null;
            cachedRanges = null;
        }
    }
}
