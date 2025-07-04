using BP.TextMotion;
using NUnit.Framework;
using UnityEngine;

namespace BP.TextMotionTests
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
