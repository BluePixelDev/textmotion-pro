namespace BP.TextMotionPro
{
    public enum TransitionPhase
    {
        In = 0,
        Out = 1,
    }

    public class MotionContext
    {
        public TextMotionPro TextMotionPro { get; private set; }
        public CharacterData CharacterData { get; private set; }
        public int CharacterIndex { get; private set; }
        public float AnimationTime { get; private set; }

        public TokenData TagData { get; private set; }
        public TransitionPhase? Phase { get; private set; }
        public string Parameters { get; private set; }

        public void ResetForTag(TextMotionPro textMotionPro, TokenData tagData, CharacterData characterData, int characterIndex, float animationTime = 0)
        {
            TextMotionPro = textMotionPro;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;

            TagData = tagData;
            Phase = null;
            Parameters = null;
        }

        public void ResetForTransition(TextMotionPro textMotionPro, CharacterData characterData, int characterIndex, float animationTime, TransitionPhase phase, string parameters)
        {
            TextMotionPro = textMotionPro;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;

            TagData = null;
            Phase = phase;
            Parameters = parameters;
        }

        public void Clear()
        {
            TextMotionPro = null;
            CharacterData = null;
            CharacterIndex = -1;
            AnimationTime = 0;
            TagData = null;
            Phase = null;
            Parameters = null;
        }
        public override string ToString()
        {
            return $"MotionContext: " +
                   $"TextMotionPro={TextMotionPro?.ToString() ?? "null"}, " +
                   $"CharacterData={CharacterData?.ToString() ?? "null"}, " +
                   $"CharacterIndex={CharacterIndex}, " +
                   $"AnimationTime={AnimationTime}, " +
                   $"TagData={TagData?.ToString() ?? "null"}, " +
                   $"Phase={(Phase.HasValue ? Phase.Value.ToString() : "null")}, " +
                   $"Parameters={Parameters ?? "null"}";
        }
    }
}
