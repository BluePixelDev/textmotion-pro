using BP.TextMotionPro;
using NUnit.Framework;
using System;

namespace BP.TextMotionPro.Tests
{
    public class WrongComponentType
    {

    }

    public class UtilityTests
    {
        [Test]
        public void CreateComponent_GenericType_ReturnsNonNullComponent()
        {
            var newComponent = MotionUtility.CreateComponent<FakeMotionComponent>();
            Assert.NotNull(newComponent, "No component was created");
        }

        [Test]
        public void CreateComponent_ByType_ReturnsNonNullComponent()
        {
            var newComponent = MotionUtility.CreateComponent(typeof(FakeMotionComponent));
            Assert.NotNull(newComponent, "No component was created");
        }

        [Test]
        public void CreateComponent_InvalidType_ThrowsInvalidOperationException()
        {
            Assert.Throws(typeof(InvalidOperationException), () =>
            {
                MotionUtility.CreateComponent(typeof(WrongComponentType));
            });
        }

        // CopyResizeArray

        [Test]
        public void CopyResizeArray_EmptyDestinationSameSize_CopiesCorrectly()
        {
            int[] srcArray = new[] { 1, 2, 3 };
            int[] dstArray = new int[3];

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            CollectionAssert.AreEqual(srcArray, dstArray, "Destination array should match source array.");
        }

        [Test]
        public void CopyResizeArray_EmptyDestinationSmallerThanSource_ResizesAndCopies()
        {
            int[] srcArray = new[] { 1, 2, 3, 4 };
            int[] dstArray = new int[3]; // smaller

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            CollectionAssert.AreEqual(srcArray, dstArray, "Destination array should match source array after resizing.");
        }

        [Test]
        public void CopyResizeArray_EmptyDestinationLargerThanSource_OverwritesAndKeepsExtraElements()
        {
            int[] srcArray = new[] { 7, 8 };
            int[] dstArray = new int[5] { 99, 99, 99, 99, 99 };

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            Assert.AreEqual(5, dstArray.Length, "Destination should keep its original length if larger.");
            CollectionAssert.AreEqual(new[] { 7, 8, 99, 99, 99 }, dstArray, "First elements should be copied; rest unchanged.");
        }

        [Test]
        public void CopyResizeArray_NullDestinationArray_CreatesAndCopies()
        {
            int[] srcArray = new[] { 10, 20, 30 };
            int[] dstArray = null;

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            CollectionAssert.AreEqual(srcArray, dstArray, "Should create a new array matching the source.");
        }

        [Test]
        public void CopyResizeArray_EmptySource_DestinationRemainsEmpty()
        {
            int[] srcArray = new int[0];
            int[] dstArray = null;

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            Assert.NotNull(dstArray, "Destination should not be null.");
            Assert.AreEqual(0, dstArray.Length, "Destination should be empty.");
        }

        [Test]
        public void CopyResizeArray_SourceAndDestinationSameReference_DoesNotChange()
        {
            int[] srcArray = new[] { 1, 2, 3 };
            int[] dstArray = srcArray;

            MotionUtility.CopyResizeArray(srcArray, ref dstArray);

            Assert.AreSame(srcArray, dstArray, "If destination is same as source, reference should remain.");
            CollectionAssert.AreEqual(srcArray, dstArray, "Contents should remain the same.");
        }
    }
}
