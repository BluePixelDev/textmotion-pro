using BP.TextMotionPro;

namespace BP.TextMotionProTests
{
    public class TagParserValidator : ITagValidator
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
