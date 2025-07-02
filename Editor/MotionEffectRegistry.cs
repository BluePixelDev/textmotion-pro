using BP.TextMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace BP.TextMotionEditor
{
    public readonly struct MotionComponentDescriptor
    {
        public readonly Type Type;
        public readonly string DisplayName;
        public readonly string Description;
        public readonly string Category;
        public readonly TextMotionRole Role;

        public MotionComponentDescriptor(Type type, string displayName, TextMotionRole role, string description, string category = null)
        {
            Type = type;
            DisplayName = displayName;
            Description = description;
            Role = role;
            Category = category;
        }
    }

    [InitializeOnLoad]
    internal static class MotionEffectRegistry
    {
        public static readonly List<MotionComponentDescriptor> Components = new();
        static MotionEffectRegistry() => Init();

        public static void Init()
        {
            Components.Clear();
            DiscoverComponents<TagEffect, TextMotionAttribute>();
        }

        private static void DiscoverComponents<TBase, TAttr>() where TBase : MotionComponent where TAttr : Attribute
        {
            foreach (var type in GetTypesWithAttribute<TAttr>())
            {
                if (!typeof(TBase).IsAssignableFrom(type)) continue;
                var attr = type.GetCustomAttribute<TAttr>();
                if (attr == null) continue;

                string name = (string)typeof(TAttr).GetProperty("DisplayName")?.GetValue(attr) ?? type.Name;
                string desc = (string)typeof(TAttr).GetProperty("Description")?.GetValue(attr);
                string category = (string)typeof(TAttr).GetProperty("Category").GetValue(attr);
                TextMotionRole role = (TextMotionRole)typeof(TAttr).GetProperty("Role").GetValue(attr);
                Components.Add(new MotionComponentDescriptor(type, name, role, desc, category));
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
    }
}