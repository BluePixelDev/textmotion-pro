using NUnit.Framework;
using TMPro;

namespace BP.TextMotionPro.Tests
{
    public class MotionRenderFlagsTests
    {
        [SetUp]
        public void Setup()
        {
            var popMethod = typeof(MotionRenderFlags)
            .GetMethod("Pop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            popMethod.Invoke(null, null);
        }

        [Test]
        public void Initially_HasFlags_ShouldBeFalse()
        {
            Assert.IsFalse(MotionRenderFlags.HasFlags, "Expected HasFlags to be false initially.");
        }

        [Test]
        public void Add_SingleFlag_HasFlagsShouldBeTrue()
        {
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
            Assert.IsTrue(MotionRenderFlags.HasFlags, "Expected HasFlags to be true after adding a flag.");
        }

        [Test]
        public void Pop_ShouldReturnCurrentFlagsAndReset()
        {
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32);

            var popMethod = typeof(MotionRenderFlags)
                .GetMethod("Pop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (TMP_VertexDataUpdateFlags)popMethod.Invoke(null, null);

            Assert.AreEqual(TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32, result,
                "Expected Pop to return the correct flags before reset.");
            Assert.IsFalse(MotionRenderFlags.HasFlags, "Expected HasFlags to be false after Pop.");
        }

        [Test]
        public void Reset_ShouldClearAllFlags()
        {
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Uv2);

            var resetMethod = typeof(MotionRenderFlags)
                .GetMethod("Reset", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            resetMethod.Invoke(null, null);

            Assert.IsFalse(MotionRenderFlags.HasFlags, "Expected HasFlags to be false after Reset.");
        }

        [Test]
        public void Add_MultipleFlags_ShouldCombineCorrectly()
        {
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Vertices);
            MotionRenderFlags.Add(TMP_VertexDataUpdateFlags.Colors32);

            var popMethod = typeof(MotionRenderFlags)
                .GetMethod("Pop", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            var result = (TMP_VertexDataUpdateFlags)popMethod.Invoke(null, null);

            var expected = TMP_VertexDataUpdateFlags.Vertices | TMP_VertexDataUpdateFlags.Colors32;
            Assert.AreEqual(expected, result, "Expected combined flags to match after adding multiple flags.");
        }
    }
}
