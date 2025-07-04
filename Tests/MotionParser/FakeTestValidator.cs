using BP.TextMotion;

namespace BP.TextMotionTests
{
    public class FakeTestValidator : ITagValidator
    {
        public bool Validate(string tagName, string attributes)
        {
            return tagName switch
            {
                "b" or "i" => true,
                _ => false,
            };
        }
    }
}
