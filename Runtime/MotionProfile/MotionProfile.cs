using System;
using System.Collections.Generic;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// Manages collections of tag effects and text transitions for TextMeshPro animations.
    /// Provides methods for adding, removing, and querying text effects and transitions.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMotionProfile", menuName = "TextMotionPro/MotionProfile", order = 210)]
    public class MotionProfile : ScriptableObject
    {
        [Header("Fallback Transitions (by name)")]
        [SerializeField] private TransitionEffect fallbackInTransition;
        [SerializeField] private TransitionEffect fallbackOutTransition;

        [Header("Components")]
        [SerializeField] private MotionCollection<TagEffect> tagComponents = new();
        [SerializeField] private MotionCollection<TransitionEffect> transitionComponents = new();

        /// <summary> Default fallback transition effect for 'in' transitions. </summary>
        public TransitionEffect FallbackInTransition => fallbackInTransition;

        /// <summary> Default fallback transition effect for 'out' transitions. </summary>
        public TransitionEffect FallbackOutTransition => fallbackOutTransition;

        /// <summary> Invoked whenever any component collection changes. </summary>
        public event Action ComponentsChanged;

        private void OnEnable()
        {
            tagComponents.Changed += OnComponentsChanged;
            transitionComponents.Changed += OnComponentsChanged;
        }

        private void OnDestroy()
        {
            tagComponents.Changed -= OnComponentsChanged;
            transitionComponents.Changed -= OnComponentsChanged;
        }

        private void OnValidate()
        {
            tagComponents.Changed -= OnComponentsChanged;
            tagComponents.Changed += OnComponentsChanged;

            transitionComponents.Changed -= OnComponentsChanged;
            transitionComponents.Changed += OnComponentsChanged;
        }

        private void OnComponentsChanged() => ComponentsChanged?.Invoke();

        /// <summary>
        /// Resets context for all components in both collections.
        /// </summary>
        public void ResetContext(TextMotionPro motionPro)
        {
            foreach (var component in tagComponents)
                component.ResetContext(motionPro);

            foreach (var component in transitionComponents)
                component.ResetContext(motionPro);
        }

        //==== TEXT EFFECTS ====//
        public IReadOnlyList<TagEffect> GetAllTagEffects() => tagComponents.GetAll();
        public void AddTagEffect(TagEffect effect) => tagComponents.Add(effect);
        public void RemoveTagEffect(TagEffect effect) => tagComponents.Remove(effect);
        public bool TryGetTagEffect(string name, out TagEffect effect) =>
            tagComponents.TryGetByKey(name, out effect);

        //==== TRANSITION EFFECTS ====//
        public IReadOnlyList<TransitionEffect> GetAllTransitonEffects() => transitionComponents.GetAll();
        public void AddTransitionEffect(TransitionEffect effect) => transitionComponents.Add(effect);
        public void RemoveTransitionEffect(TransitionEffect effect) => transitionComponents.Remove(effect);
        public bool TryGetTransitionEffect(string name, out TransitionEffect effect) =>
            transitionComponents.TryGetByKey(name, out effect);

        //==== UTILITIES ====//

        /// <summary>
        /// Returns all components (both tag effects and transitions) as a single array.
        /// </summary>
        public MotionComponent[] GetAllComponents()
        {
            var combined = new List<MotionComponent>();
            combined.AddRange(tagComponents);
            combined.AddRange(transitionComponents);
            return combined.ToArray();
        }
    }
}
