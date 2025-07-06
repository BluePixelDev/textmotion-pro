using UnityEngine;

namespace BP.TextMotionPro
{
    public abstract class MotionComponent : ScriptableObject
    {
        [SerializeField, HideInInspector] private bool isFolded = false;
        [SerializeField, HideInInspector] protected bool isActive = false;

        public abstract string Key { get; }
        public virtual bool IsActive() => isActive;
        public virtual void ResetContext(TextMotionPro renderer) { }
        public virtual void Release() { }
    }
}
