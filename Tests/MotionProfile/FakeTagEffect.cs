using BP.TextMotion;

namespace BP.TextMotionTests
{
    public class FakeTagEffect : TagComponent
    {
        public override string Key => "mockEffect";
        public override void Apply(MotionContext context) { }
        public override bool IsActive() => true;
        public override bool ValidateTag(string tag, string attributes) => true;
    }
}
