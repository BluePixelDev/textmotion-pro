namespace BP.TextMotionPro
{
    /// <summary>
    /// Data container for character data.
    /// </summary>
    public class CharacterData
    {
        public int index;
        public bool isVisible;
        public float visibleStartTime;
        public float hiddenStartTime;

        public CharacterData(int index)
        {
            this.index = index;
            isVisible = false;
            visibleStartTime = 0f;
            hiddenStartTime = 0f;
        }
    }
}
