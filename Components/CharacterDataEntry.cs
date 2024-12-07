namespace BP.TMPA
{
    internal class CharacterDataEntry
    {
        public bool IsVisible { get; private set; }
        public float FirstVisibleTime { get; private set; }

        // Constructor for invisible state
        public CharacterDataEntry()
        {
            IsVisible = false;
            FirstVisibleTime = 0f;
        }

        internal void Update(bool visible, float currentTime)
        {
            IsVisible = visible;
            FirstVisibleTime = currentTime;
        }
    }
}
