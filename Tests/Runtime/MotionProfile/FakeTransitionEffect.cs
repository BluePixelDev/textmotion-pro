using BP.TextMotionPro;

namespace BP.TextMotionPro.Tests
{
    public class FakeTransitionEffect : TransitionComponent
    {
        public override string Key => "testTransition";
        public override void Apply(MotionContext context) { }
    }
}
