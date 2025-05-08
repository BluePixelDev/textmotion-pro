namespace BP.TextMotion
{
    /// <summary>
    /// Provides a thread-safe, reusable context for text rendering and animation operations.
    /// </summary>
    public class MotionRenderContext
    {
        /// <summary>
        /// Gets the tag data associated with the current rendering context.
        /// </summary>
        public TokenData TagData { get; private set; }

        /// <summary>
        /// Total elapsed time of the text animation.
        /// </summary>
        public float AnimationTime { get; private set; }

        /// <summary>
        /// Data of the currently processed character.
        /// </summary>
        public CharacterData CharacterData { get; private set; }

        /// <summary>
        /// Gets the <see cref="TextMotionPro"/> component associated with this context.
        /// </summary>
        public TextMotionPro TextMotion { get; private set; }

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
        public void Reset(TextMotionPro component, TokenData tagData, CharacterData characterData, int characterIndex, float animationTime = 0)
        {
            TagData = tagData;
            CharacterData = characterData;
            CharacterIndex = characterIndex; // Store the current character’s index
            AnimationTime = animationTime;
            TextMotion = component;
        }

        /// <summary>
        /// Clears the context for reuse.
        /// </summary>
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
