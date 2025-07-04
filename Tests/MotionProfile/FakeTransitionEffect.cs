using BP.TextMotion;

namespace BP.TextMotionTests
{
    public class FakeTransitionEffect : TransitionComponent
    {
        public override string Key => "testTransition";
        public override void Apply(MotionContext context) { }
    }
}
