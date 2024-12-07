using System;

namespace BP.TMPA
{
    /// <summary>
    /// Provides a thread-safe, reusable context for text rendering and animation operations.
    /// </summary>
    public class TextRenderContext
    {
        [ThreadStatic]
        private static TextRenderContext _threadLocalInstance;

        /// <summary>
        /// Gets the current thread-local rendering context instance.
        /// </summary>
        public static TextRenderContext Current
        {
            get
            {
                _threadLocalInstance ??= new TextRenderContext();
                return _threadLocalInstance;
            }
        }

        /// <summary>
        /// Gets the tag data associated with the current rendering context.
        /// </summary>
        public TextTagData TagData { get; private set; }

        /// <summary>
        /// Gets the duration since the character became visible.
        /// </summary>
        public float VisibleDuration { get; private set; }

        /// <summary>
        /// Gets the index of the current character being processed.
        /// </summary>
        public int CharacterIndex { get; private set; }

        /// <summary>
        /// Gets the total elapsed time for text animation.
        /// </summary>
        public float AnimationTime { get; private set; }

        /// <summary>
        /// Gets the TextMeshProAnimated component associated with this context.
        /// </summary>
        public TextMeshProAnimated Component { get; private set; }

        /// <summary>
        /// Resets the context with new rendering parameters.
        /// </summary>
        /// <param name="tagData">The tag data for the current rendering pass.</param>
        /// <param name="visibilityDuration">Duration since character visibility.</param>
        /// <param name="characterIndex">Index of the current character.</param>
        /// <param name="animationTime">Total animation time.</param>
        /// <param name="animatedComponent">The TextMeshProAnimated component.</param>
        public void Reset(TextTagData tagData = null, float visibleDuration = 0, int characterIndex = -1, float animationTime = 0, TextMeshProAnimated Component = null)
        {
            TagData = tagData;
            VisibleDuration = visibleDuration;
            CharacterIndex = characterIndex;
            AnimationTime = animationTime;
            this.Component = Component;
        }
    }
}