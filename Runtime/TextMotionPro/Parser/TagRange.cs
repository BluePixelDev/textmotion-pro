using System.Collections.Generic;
using System.Linq;

namespace BP.TextMotion
{
    public sealed class TagRange
    {
        public int StartIndex { get; }
        public int EndIndex { get; }
        public IReadOnlyList<TokenData> Tags { get; }

        public TagRange(int startIndex, int endIndex, IEnumerable<TokenData> tags)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
            Tags = tags.ToList().AsReadOnly();
        }
    }
}
