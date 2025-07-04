using BP.TextMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace BP.TextMotionEditor
{
    public readonly struct ComponentDescriptor
    {
        public readonly Type Type;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly string Category;
        public readonly TextMotionRole Role;

        public ComponentDescriptor(Type type, string displayName, TextMotionRole role, string description, string category = null)
        {
            Type = type;
            DisplayName = displayName;
            Description = description;
            Role = role;
            Category = category;
        }
    }

    [InitializeOnLoad]
    internal static class TextMotionProPlugin
    {
        private static readonly List<ComponentDescriptor> Components = new();
        static TextMotionProPlugin() => Init();

        public static void Init()
        {
            Components.Clear();
            DiscoverComponents<TagComponent, TextMotionAttribute>();
        }

        // Discovery process
        private static void DiscoverComponents<TBase, TAttr>() where TBase : MotionComponent where TAttr : Attribute
        {
            foreach (var type in GetTypesWithAttribute<TAttr>())
            {
                if (!typeof(TBase).IsAssignableFrom(type)) continue;
                var attr = type.GetCustomAttribute<TAttr>();
                if (attr == null) continue;

                string name = (string)typeof(TAttr).GetProperty("DisplayName")?.GetValue(attr) ?? type.Name;
                string desc = (string)typeof(TAttr).GetProperty("Description")?.GetValue(attr);
                string category = (string)typeof(TAttr).GetProperty("Category")?.GetValue(attr);
                TextMotionRole role = (TextMotionRole)typeof(TAttr).GetProperty("Role").GetValue(attr);
                Components.Add(new ComponentDescriptor(type, name, role, desc, category));
            }
        }
        private static IEnumerable<Type> GetTypesWithAttribute<T>() where T : Attribute
        {
            return AppDomain.CurrentDomain.GetAssemblies()
               .Where(a => !a.IsDynamic && !a.FullName.StartsWith("Unity"))
               .SelectMany(a =>
               {
                   try { return a.GetTypes(); }
                   catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
               })
               .Where(t => t.GetCustomAttribute<T>() != null);
        }

        // Utilities
        public static bool TryGetComponentOfType(Type type, out ComponentDescriptor result)
        {
            foreach (var desc in Components)
            {
                if (desc.Type == type)
                {
                    result = desc;
                    return true;
                }
            }

            result = default;
            return false;
        }
        public static ComponentDescriptor[] GetComponents() => Components.ToArray();
    }
}