using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BP.TextMotion
{
    /// <summary>
    /// A generic collection manager for MotionComponents (e.g., TagEffects, TextTransitions).
    /// Provides efficient lookup, caching, and lifecycle management.
    /// </summary>
    [Serializable]
    public class MotionCollection<T> where T : MotionComponent
    {
        [SerializeField] private List<T> components = new();
        private readonly Dictionary<string, T> cache = new();

        /// <summary>
        /// Called when the collection is modified.
        /// </summary>
        public event Action Changed;

        /// <summary>
        /// Cleans up the list and resets the cache. Should be called from OnEnable.
        /// </summary>
        public void Initialize()
        {
            cache.Clear();
            components.RemoveAll(x => x == null);
        }

        public T[] GetAll() => components.ToArray();
        public bool Has(Type type) => components.Any(x => x.GetType() == type);
        public bool HasKey(string key)
        {
            if (cache.TryGetValue(key, out var cached)) return cached != null;
            return components.Any(x => x.Key == key);
        }
        public T GetByKey(string key)
        {
            if (cache.TryGetValue(key, out var cached)) return cached;

            var found = components.FirstOrDefault(x => x.Key == key);
            if (found != null) cache[key] = found;
            return found;
        }

        public bool TryGetByKey(string key, out T component)
        {
            component = GetByKey(key);
            return component != null;
        }
        public T Add(Type type)
        {
            if (Has(type))
                throw new InvalidOperationException($"Component of type {type.Name} already exists.");

            var instance = ScriptableObject.CreateInstance(type) as T;
            instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            instance.name = type.Name;

            components.Add(instance);
            Changed?.Invoke();
            return instance;
        }

        public void Remove(Type type)
        {
            var toRemove = components.Where(x => x.GetType() == type).ToList();

            foreach (var comp in toRemove)
            {
                if (comp == null) continue;
                components.Remove(comp);
                if (!string.IsNullOrEmpty(comp.Key))
                    cache.Remove(comp.Key);
            }

            if (toRemove.Count > 0)
                Changed?.Invoke();
        }
        public void ResetContext(TextMotionPro renderer)
        {
            foreach (var comp in components)
                comp?.ResetContext(renderer);
        }
    }
}
