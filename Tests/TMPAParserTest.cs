using BP.TextMotionPro;
using BP.TextMotionPro.Tests;
using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace BP.TextMotionProTests
{
    public class TagParserTests
    {
        private TagParserValidator validator;

        [SetUp]
        public void Setup()
        {
            validator = new();
        }

        [Test]
        public void SingleTag_BasicParsing()
        {
            string input = "<b>Test</b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            Debug.Assert(ranges.Count == 1, "There should be 1 range");
            foreach (var range in ranges)
            {
                Debug.Assert(range.Tags.Count == 1, "Each range should have 1 tag");
            }
        }

        [Test]
        public void MultipleTags_Sequential()
        {
            string input = "<b>Test</b><i>Test</i>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            Debug.Assert(ranges.Count == 2, "Range count should be 2");
            foreach (var range in ranges)
            {
                Debug.Assert(range.Tags.Count == 1, "Each range should have 1 tag");
            }
        }

        [Test]
        public void Tags_SimpleNesting()
        {
            string input = "<b>Test <i>Test</i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            Debug.Assert(ranges.Count == 2, "Range count should be 2");

            TagRangeCheck[] expectedRanges = new TagRangeCheck[]
            {
                new(expectedTags: "b"),
                new(expectedTags: new string[] { "i", "b" })
            };
            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void SingleTag_RangeIndices()
        {
            string input = "<b>Test</b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            Debug.Assert(ranges.Count == 1, "There should be 1 range");
            var firstRange = ranges.FirstOrDefault();
            Debug.Assert(firstRange != null, "First range should not be null");
            Debug.Assert(firstRange.StartIndex == 0, "Start index should be 0");
            Debug.Assert(firstRange.EndIndex == 3, "End index should be 3");
        }

        [Test]
        public void NestedTags_MultipleRanges()
        {
            string input = "<b>Testing<i>is my passion</i>Joe Mama</b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            TagRangeCheck[] expectedRanges = new TagRangeCheck[]
            {
                new(0, 6, "b"),
                new(7, 19, "i", "b"),
                new(20, 27, "b")
            };

            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void InvalidTags_NoRanges()
        {
            string input = "<invalid>Testing<invalid>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            Debug.Assert(ranges.Count == 0, "Range count should be 0 for invalid tags");
        }

        [Test]
        public void ComplexNesting_WithAttributes()
        {
            string input = "<b><b=5><i></b><i>There is a text in-between</i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            foreach (var range in ranges)
            {
                Debug.Log(range);
            }

            var expectedRanges = new TagRangeCheck[]
            {
                new(0, 25, "i", "b"),
            };

            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void NestedTags_WithAttributes()
        {
            string input = "<b=bold><i=italic>Nested with attributes</i><i=2>Second italic</i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            var expectedRanges = new TagRangeCheck[]
            {
                new(0, 21, "i=italic", "b=bold"),
                new(22, 34, "i=2", "b=bold"),
            };
            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void OverlappingTags_WithAttributes()
        {
            string input = "<b=1><i=2><b=3>Multiple attributes</b></i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            var expectedRanges = new TagRangeCheck[]
            {
                new(0, 18, "b=3", "i=2")
            };
            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void ComplexTags_MultipleAttributes()
        {
            string input = "<b=style1><i=em1>First</i><i=em2><b=style2>Nested</b></i><i=em3>Last</i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);

            var expectedRanges = new TagRangeCheck[]
            {
                new(0, 4, "i=em1", "b=style1"),
                new(5, 10, "b=style2", "i=em2"),
                new(11, 14, "i=em3", "b=style1"),
            };
            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void EmptyTags_WithAttributes()
        {
            string input = "<b=1><i=2></i><i=3></i><i=4>Text</i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);
            var expectedRanges = new TagRangeCheck[]
            {
                new(0, 3, "i=4", "b=1"),
            };
            ParserTestUtility.TestTagRanges(ranges, expectedRanges);
        }

        [Test]
        public void EmptyTags_NoContent()
        {
            string input = "<b><i></i></b>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);
            Debug.Assert(ranges.Count == 0, "There shouldn't be any ranges.");
        }

        [Test]
        public void TMPTags_MixedWithCustomTags_ParseOnlyCustom()
        {
            string input = "<b>Test</b><color=yellow>Ignore</color>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);
            Debug.Assert(ranges.Count == 1, "Only the custom <b> tag should produce a range.");
            var range = ranges.First();
            Debug.Assert(range.Tags.Any(t => t.Name.ToLower().Contains("b")), "Range should contain the <b> tag.");
        }

        [Test]
        public void MixedTMPAndCustomTags_Nested_ShouldParseCustomOnly()
        {
            string input = "<color=red><b>Test</b></color>";
            var parser = new MotionParser(validator);
            var ranges = parser.Parse(input);
            Debug.Assert(ranges.Count == 1, "Only the custom <b> tag should produce a range.");
            var range = ranges.First();
            Debug.Assert(range.Tags.Any(t => t.Name.ToLower().Contains("b")), "Range should contain the <b> tag.");
        }
    }
}                   