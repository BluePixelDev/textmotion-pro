using System;

namespace BP.TextMotion
{
    /// <summary>
    /// Base class for creating custom text animation and rendering effects for TextMesh Pro.
    /// Provides a standardized interface for defining and applying text character effects.
    /// </summary>
    [Serializable]
    public abstract class TagEffect : MotionComponent
    {
        /// <summary>
        /// Determines whether the text motion handler is currently active.
        /// </summary>
        /// <returns>
        /// True if the handler is active and should apply its effect; otherwise, false.
        /// </returns>
        public abstract bool IsActive();

        /// <summary>
        /// Validates the attributes provided for the text effect.
        /// </summary>
        /// <param name="tag">The effect tag.</param>
        /// <param name="attributes">The attributes to validate.</param>
        /// <returns>True if attributes are valid, otherwise false.</returns>
        public abstract bool ValidateTag(string tag, string attributes);

        /// <summary>
        /// Applies the effect or transition for the current rendering frame.
        /// </summary>
        /// <param name="context">
        /// A <see cref="TagEffectContext"/> object containing rendering state and character-specific data.
        /// </param>
        public abstract void Apply(TagEffectContext context);

    }
}
