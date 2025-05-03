using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BP.TextMotionPro.Tests
{
    public class TagRangeCheck
    {
        public int startIndex = -1;
        public int endIndex = -1;
        public string[] expectedTags = null;

        public TagRangeCheck(int startIndex = -1, int endIndex = -1, params string[] expectedTags)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.expectedTags = expectedTags;
        }
    }

    public static class ParserTestUtility
    {
        public static void TestTagRanges(IReadOnlyCollection<TagRange> ranges, TagRangeCheck[] expectedRanges)
        {
            // Check if the range count matches the expected count
            Debug.Assert(ranges.Count == expectedRanges.Length, $"Expected {expectedRanges.Length} ranges, but got {ranges.Count}");
            for (int i = 0; i < ranges.Count; i++)
            {
                var range = ranges.ElementAt(i);
                var expectedRange = expectedRanges[i];

                if (expectedRange.startIndex != -1)
                {
                    Debug.Assert(
                    range.StartIndex == expectedRange.startIndex,
                    $"Start index mismatch at range {i}: Expected {expectedRange.startIndex}, but got {range.StartIndex}");
                }

                // Check the start and end indices
                if (expectedRange.endIndex != -1)
                {
                    Debug.Assert(
                    range.EndIndex == expectedRange.endIndex,
                    $"End index mismatch at range {i}: Expected {expectedRange.endIndex}, but got {range.EndIndex}");
                }

                if (expectedRange.expectedTags != null)
                {
                    // Checks if tags in range match the expected tags
                    var tags = range.Tags;
                    Debug.Assert(
                        tags.Count == expectedRange.expectedTags.Length,
                        $"Tag count mismatch at range {i}: Expected {expectedRange.expectedTags.Length} tags, but found {tags.Count}");


                    foreach (var expectedTag in expectedRange.expectedTags)
                    {
                        var checkTag = SanitizeTag(expectedTag);
                        bool containsTag = tags.Any(x => SanitizeTag(x.RawTag) == checkTag);
                        Debug.Assert(containsTag, $"Tag, {checkTag}, not found in range {i}");
                    }
                }
            }
        }

        private static string SanitizeTag(string tag)
        {
            return tag.Replace("<", string.Empty).Replace(">", string.Empty).ToLower();
        }
    }
}
