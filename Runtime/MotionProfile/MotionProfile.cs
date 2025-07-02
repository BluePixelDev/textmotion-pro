using System;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// Manages a collection of text effects for TextMeshPro animations.
    /// Provides methods for adding, removing, and querying text effects.
    /// </summary>
    [CreateAssetMenu(fileName = "NewMotionProfile", menuName = "TextMotionPro/MotionProfile", order = 210)]
    public class MotionProfile : ScriptableObject
    {
        [SerializeField] private MotionCollection<TagEffect> tagComponents = new();
        public MotionCollection<TagEffect> TagComponents => tagComponents;
        public event Action Changed;

        private void OnEnable() => tagComponents.Changed += OnTagComponentsChanged;
        private void OnDestroy() => tagComponents.Changed -= OnTagComponentsChanged;

        private void OnValidate()
        {
            tagComponents.Changed -= OnTagComponentsChanged;
            tagComponents.Changed += OnTagComponentsChanged;
        }

        private void OnTagComponentsChanged() => Changed?.Invoke();

        public void ResetContext(TextMotionPro motionPro) => tagComponents.ResetContext(motionPro);
        public MotionComponent[] GetAllMotionComponents() => tagComponents.GetAll();
    }
}