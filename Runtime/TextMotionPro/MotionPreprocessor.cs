using System.Collections.Generic;
using System.Linq;
using TMPro;

namespace BP.TextMotion
{
    /// <summary>
    /// Preprocesses text for <see cref="MotionPreprocessor"/> by removing tags and parsing tag ranges.
    /// Caches results to avoid redundant processing.
    /// </summary>
    public class MotionPreprocessor : ITextPreprocessor
    {
        private string lastInput = null;
        private readonly IParser parser;
        private ParseResult result;
        private TagRange cachedRange;
        private TokenData cachedAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionPreprocessor"/> class.
        /// </summary>
        /// <param name="validator">An instance of <see cref="ITagValidator"/> used for validating tags.</param>
        public MotionPreprocessor(IParser parser)
        {
            this.parser = parser;
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
                cachedAction = null;
                result = null;
                return string.Empty;
            }

            // Reprocess only if the input has changed or cache is empty.
            if (inputText != lastInput || result == null)
            {
                result = parser.Parse(inputText);
                lastInput = inputText;
            }

            return result.CleanText;
        }

        //==== TAGS ====//
        /// <summary>
        /// Retrieves the tags affecting the specified index in the processed text.
        /// </summary>
        /// <param name="index">The index within the processed text.</param>
        /// <returns>
        /// A read-only collection of <see cref="TokenData"/> affecting the specified index,
        /// or <c>null</c> if no range applies.
        /// </returns>
        public IReadOnlyList<TokenData> GetTagsAt(int index)
        {
            UpdateCachedRangeIfNeeded(index);
            if (cachedRange == null)
                return null;

            return cachedRange.Tags;
        }
        private void UpdateCachedRangeIfNeeded(int index)
        {
            if (result.Ranges == null)
                return;

            // If the current cached range covers the index, no need to update.
            if (cachedRange != null && index >= cachedRange.StartIndex && index <= cachedRange.EndIndex)
                return;

            // Otherwise, search for the appropriate range.
            cachedRange = result.Ranges.FirstOrDefault(range => range.StartIndex <= index && index <= range.EndIndex);
        }

        //==== ACTIONS ====//
        public TokenData GetActionAt(int index)
        {
            UpdateActionCacheIfNeeded(index);
            return cachedAction;
        }
        private void UpdateActionCacheIfNeeded(int index)
        {
            if (cachedAction != null && cachedAction.Position == index)
                return;

            cachedAction = result.Actions?.FirstOrDefault(action => action.Position == index);
        }

        public void ClearCache()
        {
            cachedRange = null;
            cachedAction = null;
            result = null;
            lastInput = null;
        }
    }
}
