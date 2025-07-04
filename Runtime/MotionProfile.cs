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
        [SerializeField] private TransitionComponent fallbackInTransition;
        [SerializeField] private TransitionComponent fallbackOutTransition;

        [SerializeField] private MotionComponentRegistry<TagComponent> tagComponents = new();
        [SerializeField] private MotionComponentRegistry<TransitionComponent> transitionComponents = new();

        public MotionComponentRegistry<TagComponent> TagComponents => tagComponents;
        public MotionComponentRegistry<TransitionComponent> TransitionComponents => transitionComponents;

        /// <summary> Default fallback transition effect for 'in' transitions. </summary>
        public TransitionComponent FallbackInTransition => fallbackInTransition;

        /// <summary> Default fallback transition effect for 'out' transitions. </summary>
        public TransitionComponent FallbackOutTransition => fallbackOutTransition;

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

        // Utilities
        public MotionComponent[] GetAllComponents()
        {
            var combined = new List<MotionComponent>();
            combined.AddRange(tagComponents);
            combined.AddRange(transitionComponents);
            return combined.ToArray();
        }
    }
}
