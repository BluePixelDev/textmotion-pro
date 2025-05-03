using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace BP.TextMotionPro
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public static class TextEffectRegistry
    {
        public static List<TextEffectDescriptor> TextEffects { get; } = new();
        static TextEffectRegistry() => Init();

        [RuntimeInitializeOnLoadMethod]
        public static void Init()
        {
            ShowAvailableEffects();
        }

        public static void ShowAvailableEffects()
        {
            var effectTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.GetCustomAttribute<TextEffectAttribute>() != null)
                .ToList();

            foreach (var effectType in effectTypes)
            {
                var attribute = effectType.GetCustomAttribute<TextEffectAttribute>();
                TextEffects.Add(new TextEffectDescriptor(effectType, attribute.Name, attribute.Description));
            }
        }
    }
}