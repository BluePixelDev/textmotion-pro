using System;
using System.Collections;
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
    public class MotionCollection<T> : IEnumerable<T>, ICollection<T> where T : MotionComponent
    {
        [SerializeField] private List<T> components = new();
        private readonly Dictionary<string, T> cache = new();
        public int Count => components.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// Called when the collection is modified.
        /// </summary>
        public event Action Changed;

        public T this[string key]
        {
            get => GetByKey(key);
        }

        /// <summary>
        /// Cleans up the list and resets the cache. Should be called from OnEnable.
        /// </summary>
        public void Initialize()
        {
            cache.Clear();
            components.RemoveAll(x => x == null);
        }

        /// <summary>
        /// Gets all components as an array.
        /// </summary>
        public T[] GetAll() => components.ToArray();

        /// <summary>
        /// Checks if the collection contains a component of the specified type.
        /// </summary>
        public bool Has(Type type) => components.Any(x => x.GetType() == type);

        /// <summary>
        /// Checks if a component with the given key exists.
        /// </summary>
        public bool HasKey(string key)
        {
            if (cache.TryGetValue(key, out var cached))
                return cached != null;

            return components.Any(x => x.Key == key);
        }

        /// <summary>
        /// Gets a component by key, caching the result.
        /// </summary>
        public T GetByKey(string key)
        {
            if (cache.TryGetValue(key, out var cached))
                return cached;

            var found = components.FirstOrDefault(x => x.Key == key);
            if (found != null)
                cache[key] = found;

            return found;
        }

        /// <summary>
        /// Tries to get a component by key.
        /// </summary>
        public bool TryGetByKey(string key, out T component)
        {
            component = GetByKey(key);
            return component != null;
        }

        /// <summary>
        /// Adds a new component of the specified type.
        /// </summary>
        public T Add(Type type)
        {
            if (Has(type))
                throw new InvalidOperationException($"Component of type {type.Name} already exists.");

            var instance = ScriptableObject.CreateInstance(type) as T ?? throw new InvalidOperationException($"Failed to create instance of type {type.Name}.");
            instance.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            instance.name = type.Name;

            components.Add(instance);
            Changed?.Invoke();
            return instance;
        }

        /// <summary>
        /// Removes all components of the specified type.
        /// </summary>
        public void Remove(Type type)
        {
            var toRemove = components.Where(x => x.GetType() == type).ToList();
            foreach (var comp in toRemove)
            {
                if (comp != null)
                {
                    components.Remove(comp);
                    if (!string.IsNullOrEmpty(comp.Key))
                        cache.Remove(comp.Key);
                }
            }

            if (toRemove.Count > 0)
                Changed?.Invoke();
        }

        /// <summary>
        /// Returns an enumerator over the components.
        /// </summary>
        public IEnumerator<T> GetEnumerator() => components.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(T item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }
    }
}
