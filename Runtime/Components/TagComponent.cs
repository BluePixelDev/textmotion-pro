using System;

namespace BP.TextMotionPro
{
    /// <summary>
    /// Base class for creating custom text animation and rendering effects for TextMesh Pro.
    /// Provides a standardized interface for defining and applying text character effects.
    /// </summary>
    [Serializable]
    public abstract class TagComponent : MotionComponent
    {
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
        /// A <see cref="MotionContext"/> object containing rendering state and character-specific data.
        /// </param>
        public abstract void Apply(MotionContext context);

    }
}
