namespace BP.TextMotion
{
    public enum TransitionPhase
    {
        In = 0,
        Out = 1,
    }

    public class MotionContext
    {
        public TextMotionPro TextMotion { get; private set; }
        public CharacterData CharacterData { get; private set; }
        public int CharacterIndex { get; private set; }
        public float AnimationTime { get; private set; }

        public TokenData TagData { get; private set; }
        public TransitionPhase? Phase { get; private set; }
        public string Parameters { get; private set; }

        public void ResetForTag(TextMotionPro textMotion, TokenData tagData, CharacterData characterData, int characterIndex, float animationTime = 0)
        {
            TextMotion = textMotion;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;

            TagData = tagData;
            Phase = null;
            Parameters = null;
        }

        public void ResetForTransition(TextMotionPro textMotion, CharacterData characterData, int characterIndex, float animationTime, TransitionPhase phase, string parameters)
        {
            TextMotion = textMotion;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;

            TagData = null;
            Phase = phase;
            Parameters = parameters;
        }

        public void Clear()
        {
            TextMotion = null;
            CharacterData = null;
            CharacterIndex = -1;
            AnimationTime = 0;
            TagData = null;
            Phase = null;
            Parameters = null;
        }
    }
}
