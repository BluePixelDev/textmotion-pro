using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// Manages a collection of text effects for TextMeshPro animations.
    /// Provides methods for adding, removing, and querying text effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMotionProfile", menuName = "TextMotionPro/MotionProfile")]
    public class MotionProfile : ScriptableObject
    {
        [SerializeField] private List<TextEffect> textEffects = new();
        private readonly Dictionary<string, TextEffect> cache = new();

        /// <summary>
        /// Invoked whenever the text effects list changes (added, removed, etc).
        /// </summary>
        public event Action TextEffectsChanged;

        /// <summary>
        /// Invoked whenever the text effects list changes (added, removed, etc).
        /// </summary>
        private void OnEnable()
        {
            cache.Clear();
            textEffects.RemoveAll(x => x == null);
        }

        /// <summary>
        /// Adds a new text effect of the specified type to the profile.
        /// </summary>
        /// <typeparam name="T">The type of text effect to add.</typeparam>
        /// <returns>The newly created text effect.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the effect type already exists.</exception>
        public T AddTextEffect<T>() where T : TextEffect
        {
            Type effectType = typeof(T);
            return (T)AddTextEffect(effectType);
        }

        /// <summary>
        /// Adds a new text effect of the specified type to the profile.
        /// </summary>
        /// <param name="type">The type of text effect to add.</param>
        /// <returns>The newly created text effect.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the effect type already exists.</exception>
        public TextEffect AddTextEffect(Type type)
        {
            // Prevent duplicate effects
            if (HasTextEffect(type))
                throw new InvalidOperationException($"Effect of type {type.Name} is already instantiated.");

            // Create and configure the new effect
            TextEffect component = (TextEffect)CreateInstance(type);
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            component.name = type.Name;

            // Add to effects list and mark as dirty
            textEffects.Add(component);
            TextEffectsChanged?.Invoke();
            return component;
        }

        /// <summary>
        /// Removes all text effects of a specific type from the profile.
        /// </summary>
        /// <typeparam name="T">The type of text effect to remove.</typeparam>
        public void RemoveTextEffect<T>() where T : TextEffect => RemoveTextEffect(typeof(T));

        /// <summary>
        /// Removes all text effects of a specific type from the profile.
        /// </summary>
        /// <param name="type">The type of text effect to remove.</param>
        public void RemoveTextEffect(Type type)
        {
            var textEffectsToRemove = textEffects.Where(x => x.GetType().Equals(type)).ToList();
            foreach (var effect in textEffectsToRemove)
            {
                textEffects.Remove(effect);
                if (cache.ContainsKey(effect.Tag))
                {
                    cache.Remove(effect.Tag);
                }
            }
            if (textEffectsToRemove.Count > 0)
                TextEffectsChanged?.Invoke();
        }

        /// <summary>
        /// Checks if a specific type of text effect exists in the profile.
        /// </summary>
        /// <typeparam name="T">The type of text effect to check.</typeparam>
        /// <returns>True if the text effect exists, otherwise false.</returns>
        public bool HasTextEffect<T>() where T : TextEffect => HasTextEffect(typeof(T));

        /// <summary>
        /// Checks if a specific type of text effect exists in the profile.
        /// </summary>
        /// <param name="type">The type of text effect to check.</param>
        /// <returns>True if the text effect exists, otherwise false.</returns>
        public bool HasTextEffect(Type type) => textEffects.Any(x => x.GetType().Equals(type));

        // <summary>
        /// Checks if a text effect with the specified tag exists in the profile.
        /// </summary>
        /// <param name="tag">The tag to search for.</param>
        /// <returns>True if a text effect with the tag exists, otherwise false.</returns>
        public bool HasTextEffectWithTag(string tag)
        {
            // Check cache first for performance
            if (cache.TryGetValue(tag, out var cachedEffect))
                return cachedEffect != null;

            return textEffects.Any(x => x.Tag == tag);
        }

        public bool TryGetTextEffectWithTag(string tag, out TextEffect textEffect)
        {
            var effect = GetTextEffectWithTag(tag);
            if (effect != null)
            {
                textEffect = effect;
                return true;
            }

            textEffect = null;
            return false;
        }

        /// <summary>
        /// Retrieves a text effect with the specified tag.
        /// </summary>
        /// <param name="tag">The tag of the text effect to retrieve.</param>
        /// <returns>The text effect with the specified tag, or null if not found.</returns>
        public TextEffect GetTextEffectWithTag(string tag)
        {
            // Looks up the tag in cache
            if (cache.TryGetValue(tag, out var cached))
                return cached;

            // Find effect and cache it
            var effect = textEffects.FirstOrDefault(x => x.Tag == tag);
            if (effect != null)
                cache[tag] = effect;

            return effect;
        }

        /// <summary>
        /// Resets the context for all text effects in this profile.
        /// </summary>
        /// <param name="renderer">
        /// The <see cref="TextMotionPro"/> instance associated with the text component.
        /// This method is typically called when the text component or its animation context is refreshed,
        /// ensuring that all cached states and per-component variables for each effect are reinitialized.
        /// </param>
        public void ResetContext(TextMotionPro renderer)
        {
            foreach (var effect in textEffects)
            {
                effect.ResetContext(renderer);
            }
        }

        /// <summary>
        /// Retrieves all text effects in the profile.
        /// </summary>
        /// <returns>An array of all text effects.</returns>
        public TextEffect[] GetAllTextEffects() => textEffects.ToArray();
    }
}