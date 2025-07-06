using BP.TextMotionPro;
using NUnit.Framework;
using UnityEngine;

namespace BP.TextMotionPro.Tests
{
    public class MotionProfileTests
    {
        private MotionProfile profile;

        [SetUp]
        public void Setup()
        {
            profile = ScriptableObject.CreateInstance<MotionProfile>();
        }
    }
}
