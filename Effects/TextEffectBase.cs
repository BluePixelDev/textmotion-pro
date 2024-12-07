using System;
using TMPro;
using UnityEngine;

namespace BP.TMPA
{
    /// <summary>
    /// Base class for creating custom text animation and rendering effects for TextMesh Pro.
    /// Provides a standardized interface for defining and applying text character effects.
    /// </summary>
    [Serializable]
    public abstract class TextEffectBase : ScriptableObject
    {
        /// <summary>
        /// Gets the current text rendering context for the effect.
        /// </summary>
        public TextRenderContext Context => TextRenderContext.Current;
        /// <summary>
        /// Gets the current TextMeshPro text component being processed.
        /// </summary>
        public TMP_Text Text => Context.Component.TextComponent;
        /// <summary>
        /// Gets the current character index being processed.
        /// </summary>
        public int CharacterIndex => Context.CharacterIndex;
        /// <summary>
        /// Gets the character information for the current character being processed.
        /// </summary>
        public TMP_CharacterInfo CharacterInfo => Text.textInfo.characterInfo[CharacterIndex];

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
        public virtual bool ValidateTagAttributes(string tag, string attributes) => false;

        /// <summary>
        /// Checks whether this text effect is active or not.
        /// </summary>
        /// <returns>True if this text effect is active.</returns>
        public virtual bool IsActive() => true;

        /// <summary>
        /// Applies the text effect to the current character.
        /// </summary>
        public virtual void ApplyEffect() { }

        /// <summary>
        /// Releases resources used by the text effect.
        /// </summary>
        public virtual void Release() { }
    }
}