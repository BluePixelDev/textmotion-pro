namespace BP.TextMotionPro
{
    /// <summary>
    /// Provides a thread-safe, reusable context for text rendering and animation operations.
    /// </summary>
    public class MotionRenderContext
    {
        /// <summary>
        /// Gets the tag data associated with the current rendering context.
        /// </summary>
        public TagData TagData { get; private set; }

        /// <summary>
        /// Total elapsed time of the text animation.
        /// </summary>
        public float AnimationTime { get; private set; }

        /// <summary>
        /// Data of the currently processed character.
        /// </summary>
        public CharacterData CharacterData { get; private set; }

        /// <summary>
        /// Gets the TextMotionRenderer component associated with this context.
        /// </summary>
        public TextMotionRenderer Renderer { get; private set; }

        /// <summary>
        /// The index of the character in the entire character array.
        /// </summary>
        public int CharacterIndex { get; private set; }

        /// <summary>
        /// Resets the context with new rendering parameters.
        /// </summary>
        /// <param name="component">The TextMeshProAnimated component.</param>
        /// <param name="tagData">The tag data for the current rendering pass.</param>
        /// <param name="characterData">Data associated with each character.</param>
        /// <param name="characterIndex">Index of the character in the full character array.</param>
        /// <param name="animationTime">Total animation time.</param>
        public void Reset(TextMotionRenderer component, TagData tagData, CharacterData characterData, int characterIndex, float animationTime = 0)
        {
            TagData = tagData;
            CharacterData = characterData;
            CharacterIndex = characterIndex; // Store the current character’s index
            AnimationTime = animationTime;
            Renderer = component;
        }

        /// <summary>
        /// Clears the context for reuse.
        /// </summary>
        public void Clear()
        {
            TagData = null;
            CharacterData = null;
            Renderer = null;
            CharacterIndex = -1; // Reset index
            AnimationTime = 0;
        }
    }
}
