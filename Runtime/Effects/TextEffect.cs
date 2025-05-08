using System;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// Base class for creating custom text animation and rendering effects for TextMesh Pro.
    /// Provides a standardized interface for defining and applying text character effects.
    /// </summary>
    [Serializable]
    public abstract class TextEffect : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool isFolded = false;

        /// <summary>
        /// Gets the unique identifier tag for this text effect.
        /// </summary>
        public abstract string EffectTag { get; }

        /// <summary>
        /// Validates the attributes provided for the text effect.
        /// </summary>
        /// <param name="tag">The effect tag.</param>
        /// <param name="attributes">The attributes to validate.</param>
        /// <returns>True if attributes are valid, otherwise false.</returns>
        public virtual bool ValidateTag(string tag, string attributes) => false;

        /// <summary>
        /// Checks whether this text effect is active or not.
        /// </summary>
        /// <returns>True if this text effect is active.</returns>
        public virtual bool IsActive() => true;

        /// <summary>
        /// Resets any component-level cached data or internal state for this text effect.
        /// </summary>
        /// <param name="renderer">
        /// The <see cref="TextMotionPro"/> instance associated with the text component.
        /// Override this method in derived classes to clear any cached per-component state (such as initial positions,
        /// alpha values, or other animation parameters) so that the effect can be recalculated correctly when the text is updated.
        /// </param>
        public virtual void ResetContext(TextMotionPro renderer) { }

        /// <summary>
        /// Applies the text effect to the current character.
        /// </summary>
        public virtual void ApplyEffect(MotionRenderContext context) { }

        /// <summary>
        /// Releases resources used by the text effect.
        /// </summary>
        public virtual void Release() { }
    }
}