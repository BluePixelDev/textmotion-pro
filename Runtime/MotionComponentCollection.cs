using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BP.TextMotionPro
{
    /// <summary>
    /// A generic collection manager for MotionComponents (e.g., TagEffects, TextTransitions).
    /// Provides efficient lookup, caching, and lifecycle management.
    /// </summary>
    [Serializable]
    public class MotionComponentCollection<T> : IEnumerable<T> where T : MotionComponent
    {
        [SerializeField] private List<T> components = new();
        private readonly Dictionary<string, T> cacheByKey = new();

        public int Count => components.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// Called when the collection is modified.
        /// </summary>
        public event Action Changed;

        public T[] GetAll() => components.ToArray();
        public bool Has(Type type) => components.Any(x => x.GetType() == type);
        public bool HasKey(string key)
        {
            if (cacheByKey.TryGetValue(key, out var cached))
                return cached != null;

            return components.Any(x => x.Key == key);
        }

        public TComponent Add<TComponent>() where TComponent : MotionComponent => (TComponent)Add(typeof(TComponent));
        public MotionComponent Add(Type type)
        {
            if (Has(type))
                throw new InvalidOperationException("Component already exists in the volume");

            var component = (T)ScriptableObject.CreateInstance(type);

#if UNITY_EDITOR
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            component.name = type.Name;
#endif
            if (string.IsNullOrEmpty(component.Key))
                throw new InvalidOperationException("Component key cannot be null");

            components.Add(component);
            cacheByKey.Add(component.Key, component);
            Changed?.Invoke();
            return component;
        }
        public bool Remove<TComponent>() where TComponent : MotionComponent => Remove(typeof(TComponent));
        public bool Remove(Type type)
        {
            foreach (var component in components)
            {
                if (component == null) continue;
                if (component.GetType().Equals(type))
                {
                    components.Remove(component);
                    Changed?.Invoke();
                    return true;
                }
            }

            return false;
        }
        public TComponent Get<TComponent>() where TComponent : T
        {
            foreach (var component in components)
            {
                if (component is TComponent typedComponent)
                {
                    return typedComponent;
                }
            }
            return default;
        }
        public bool TryGetByKey(string key, out T component)
        {
            if (HasKey(key))
            {
                if (cacheByKey.TryGetValue(key, out component)) return true;
                foreach (var comp in components)
                {
                    if (comp.Key == key)
                    {
                        component = comp;
                        cacheByKey.Add(key, component);
                        return true;
                    }
                }
            }

            component = default;
            return false;
        }

        public IEnumerator<T> GetEnumerator() => components.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
