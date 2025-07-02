namespace BP.TextMotion
{
    public class MotionValidator : ITagValidator
    {
        private readonly TextMotionPro textMotion;
        public MotionValidator(TextMotionPro textMotion)
        {
            this.textMotion = textMotion;
        }

        public bool Validate(string key, string attributes)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (TagFilter.IsBuiltIn(key))
                return true;

            if (TagFilter.IsReserved(key))
                return false;

            var profile = textMotion.Profile;
            return profile != null &&
                  profile.TagComponents.TryGetByKey(key, out var effect) &&
                  effect.ValidateTag(key, attributes);
        }
    }
}
