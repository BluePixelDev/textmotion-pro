using System.Collections.Generic;

namespace BP.TextMotion
{

    /// <summary>
    /// Represents the result of parsing motion text, including tag ranges, actions, and the cleaned text.
    /// </summary>
    public sealed class ParseResult
    {
        private readonly List<TagRange> ranges = new();
        private readonly List<TokenData> actions = new();

        /// <summary>
        /// Gets the list of tag ranges detected in the input text.
        /// </summary>
        public IReadOnlyList<TagRange> Ranges => ranges.AsReadOnly();

        /// <summary>
        /// Gets the list of actions detected in the input text.
        /// </summary>
        public IReadOnlyList<TokenData> Actions => actions.AsReadOnly();

        /// <summary>
        /// Gets the input text with all tags and actions removed.
        /// </summary>
        public string CleanText { get; private set; }

        public ParseResult(ICollection<TagRange> ranges, ICollection<TokenData> actions, string cleanText)
        {
            this.ranges.AddRange(ranges);
            this.actions.AddRange(actions);
            CleanText = cleanText;
        }
    }
}
