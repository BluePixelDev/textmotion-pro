namespace BP.TextMotion
{
    public class TagEffectContext
    {
        public TokenData TagData { get; private set; }
        public float AnimationTime { get; private set; }
        public CharacterData CharacterData { get; private set; }
        public TextMotionPro TextMotion { get; private set; }
        public int CharacterIndex { get; private set; }

        public void Reset(TextMotionPro component, TokenData tagData, CharacterData characterData, int characterIndex, float animationTime = 0)
        {
            TagData = tagData;
            CharacterData = characterData;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;
            TextMotion = component;
        }

        public void Clear()
        {
            TagData = null;
            CharacterData = null;
            TextMotion = null;
            CharacterIndex = -1;
            AnimationTime = 0;
        }
    }
}
