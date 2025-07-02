using UnityEngine;

namespace BP.TextMotion
{
    public abstract class MotionComponent : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool isFolded = false;

        /// <summary>
        /// A unique string identifier used to reference this component (e.g. "wave", "cut").
        /// </summary>
        public abstract string Key { get; }

        /// <summary>
        /// Resets any internal or cached data for this handler associated with the specified renderer.
        /// </summary>
        /// <param name="renderer">
        /// The <see cref="TextMotionPro"/> instance tied to the text component.
        /// This method should clear per-instance state, such as animation progress, cached positions, or timing values.
        /// </param>
        public virtual void ResetContext(TextMotionPro renderer) { }

        /// <summary>
        /// Releases any resources or state used by this handler when it is no longer needed.
        /// This may include clearing memory, unsubscribing from events, or stopping animations.
        /// </summary>
        public virtual void Release() { }
    }
}
