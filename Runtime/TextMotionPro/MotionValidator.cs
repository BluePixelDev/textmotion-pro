namespace BP.TextMotionPro
{
    public class MotionValidator : ITagValidator
    {
        private readonly TextMotionPro TextMotionPro;
        public MotionValidator(TextMotionPro TextMotionPro)
        {
            this.TextMotionPro = TextMotionPro;
        }

        public bool Validate(string key, string attributes)
        {
            if (string.IsNullOrEmpty(key))
                return false;

            if (TagFilter.IsBuiltIn(key))
                return true;

            if (TagFilter.IsReserved(key))
                return false;

            var profile = TextMotionPro.Profile;
            return profile != null &&
                  profile.TagComponents.TryGetByKey(key, out var effect) &&
                  effect.ValidateTag(key, attributes);
        }
    }
}
