using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEditor;

[assembly: InternalsVisibleTo("BP.TextMotionPro.Editor.Tests")]

namespace BP.TextMotionPro.Editor
{
    public enum ComponentRole
    {
        Tag = 0,
        Transition = 1,
    }
    public readonly struct ComponentDescriptor
    {
        public readonly Type Type;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly string Category;
        public readonly ComponentRole Role;

        public ComponentDescriptor(Type type, string displayName, ComponentRole role, string description, string category = null)
        {
            Type = type;
            DisplayName = displayName;
            Description = description;
            Role = role;
            Category = category;
        }

        public override string ToString()
        {
            return
                $"Type {Type}" +
                $"Display Name {DisplayName}" +
                $"Description {Description}" +
                $"Category {Category}" +
                $"Role {Role}";
        }
    }

    [InitializeOnLoad]
    internal static class MotionComponentCatalog
    {
        private static readonly List<ComponentDescriptor> Components = new();
        static MotionComponentCatalog() => Init();

        public static void Init()
        {
            Components.Clear();
            CollectComponents();
        }

        private static void CollectComponents()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                // Skip dynamic assemblies and optionally Unity editor-only assemblies
                if (assembly.IsDynamic || string.IsNullOrEmpty(assembly.FullName) || assembly.FullName.StartsWith("Unity"))
                    continue;

                Type[] types;
                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    types = e.Types.Where(t => t != null).ToArray();
                }

                foreach (var type in types)
                {
                    if (!type.IsSubclassOf(typeof(MotionComponent)))
                        continue;

                    var attr = type.GetCustomAttribute<ComponentDescriptorAttribute>();
                    if (attr == null)
                        continue;

                    var isTagComponent = type.IsSubclassOf(typeof(TagComponent));
                    var isTransitionComponent = type.IsSubclassOf(typeof(TransitionComponent));

                    if (!isTagComponent && !isTransitionComponent)
                        continue;

                    var role = isTagComponent ? ComponentRole.Tag : ComponentRole.Transition;
                    var desc = new ComponentDescriptor(
                        type,
                        attr.DisplayName,
                        role,
                        attr.Description,
                        attr.Category
                    );

                    Components.Add(desc);
                }
            }
        }

        // Utilities
        public static bool TryGetComponentOfType<T>(out ComponentDescriptor result) where T : MotionComponent => TryGetComponentOfType(typeof(T), out result);
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