namespace BP.TextMotion
{
    public class MotionValidator : ITagValidator
    {
        private readonly TextMotionPro textMotion;
        public MotionValidator(TextMotionPro textMotion)
        {
            this.textMotion = textMotion;
        }

        public bool Validate(string tagName, string attributes)
        {
            if (string.IsNullOrEmpty(tagName))
                return false;

            if (TagFilter.IsBuiltIn(tagName))
                return true;

            if (TagFilter.IsReserved(tagName))
                return false;

            var profile = textMotion.Profile;
            return profile != null &&
                  profile.TryGetTextEffectWithTag(tagName, out var effect) &&
                  effect.ValidateTag(tagName, attributes);
        }
    }
}
