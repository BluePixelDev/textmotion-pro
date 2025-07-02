namespace BP.TextMotion
{
    public enum TransitionPhase
    {
        In,
        Out
    }

    public class TransitionContext
    {
        public CharacterData CharacterData { get; private set; }
        public TextMotionPro TextMotion { get; private set; }
        public float AnimationTime { get; private set; }
        public int CharacterIndex { get; private set; }
        public TransitionPhase Phase { get; private set; }
        public string Parameters { get; private set; }

        public void Reset(TextMotionPro textMotion, CharacterData characterData, int characterIndex, float animationTime, TransitionPhase phase, string parameters)
        {
            TextMotion = textMotion;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;
            Phase = phase;
            Parameters = parameters;
        }

        public void Clear()
        {
            TextMotion = null;
            CharacterData = null;
            CharacterIndex = -1;
            AnimationTime = 0;
            Phase = default;
            Parameters = null;
        }
    }
}
