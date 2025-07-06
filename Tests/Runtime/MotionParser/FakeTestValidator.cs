using BP.TextMotionPro;

namespace BP.TextMotionPro.Tests
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
