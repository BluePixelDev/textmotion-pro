using BP.TextMotion;
using NUnit.Framework;
using System.Linq;

namespace BP.TextMotionTests
{
    public class ParserTests
    {
        private ITagValidator _validator;
        private MotionParser _parser;

        [SetUp]
        public void Setup()
        {
            _validator = new ParserTestValidator();
            _parser = new MotionParser(_validator);
        }

        [Test]
        public void SingleTag_ProducesOneRange_AndCorrectCleanText()
        {
            var result = _parser.Parse("<b>Test</b>");

            Assert.AreEqual(1, result.Ranges.Count);
            var range = result.Ranges.Single();
            Assert.AreEqual(0, range.StartIndex);
            Assert.AreEqual(3, range.EndIndex);
            Assert.AreEqual("Test", result.CleanText);
            Assert.AreEqual(1, range.Tags.Count);
            Assert.AreEqual("b", range.Tags[0].Name);
        }

        [Test]
        public void NestedTags_ProducesThreeRanges_WithProperTags()
        {
            var result = _parser.Parse("<b>Hi <i>there</i>!</b>");
            Assert.AreEqual(3, result.Ranges.Count);

            var r1 = result.Ranges[0];
            Assert.AreEqual(0, r1.StartIndex);
            Assert.AreEqual(2, r1.EndIndex);
            CollectionAssert.AreEqual(new[] { "b" }, r1.Tags.Select(t => t.Name).ToArray());

            var r2 = result.Ranges[1];
            Assert.AreEqual(3, r2.StartIndex);
            Assert.AreEqual(7, r2.EndIndex);
            CollectionAssert.AreEqual(new[] { "b", "i" }, r2.Tags.Select(t => t.Name).ToArray());

            var r3 = result.Ranges[2];
            Assert.AreEqual(8, r3.StartIndex);
            Assert.AreEqual(8, r3.EndIndex);
            CollectionAssert.AreEqual(new[] { "b" }, r3.Tags.Select(t => t.Name).ToArray());
        }

        [Test]
        public void ActionToken_IsRecorded_AtAdjustedPosition()
        {
            var result = _parser.Parse("Hello{wave=2}World");

            Assert.AreEqual("HelloWorld", result.CleanText);
            Assert.AreEqual(1, result.Actions.Count);
            var action = result.Actions.Single();
            Assert.AreEqual("wave", action.Name);
            Assert.AreEqual("2", action.Value);
            Assert.AreEqual(5, action.Position);
        }

        [Test]
        public void OutOfOrderClosing_RemovesCorrectTag()
        {
            string input = "<i><b>Text</i>More</b>";
            var result = _parser.Parse(input);

            var first = result.Ranges[0];
            UnityEngine.Debug.Log(result.Ranges.Count);
            CollectionAssert.AreEqual(new[] { "i", "b" }, first.Tags.Select(t => t.Name).ToArray());

            var last = result.Ranges.Last();
            CollectionAssert.AreEqual(new[] { "b" }, last.Tags.Select(t => t.Name).ToArray());
            Assert.AreEqual("More", result.CleanText[last.StartIndex..]);
        }

        [Test]
        public void NoValidTags_ReturnsEmptyRanges_AndOriginalCleanText()
        {
            var result = _parser.Parse("Plain text with{action=1} and <x>invalid</x>");

            Assert.IsEmpty(result.Ranges);
            Assert.AreEqual("Plain text with and <x>invalid</x>", result.CleanText);
        }
    }
}