using BP.TextMotion;

namespace BP.TextMotionTests
{
    public class ParserTestValidator : ITagValidator
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
