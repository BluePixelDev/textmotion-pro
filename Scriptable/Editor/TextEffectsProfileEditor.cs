using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BP.TMPA
{
    [CustomEditor(typeof(TextEffectsProfile))]
    public class TextEffectsProfileEditor : Editor
    {
        private TextEffectsProfile profile;

        private void OnEnable()
        {
            profile = (TextEffectsProfile)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUI.BeginChangeCheck();

            var effects = profile.textEffects;
            for (int i = 0; i < effects.Count; i++)
            {
                var effect = effects[i];
                if (effect == null) continue;

                EditorGUI.indentLevel++;

                // Create a custom editor for each component type
                var effectEditor = CreateEditor(effect);
                if (effectEditor != null)
                {
                    effectEditor.OnInspectorGUI();
                    DestroyImmediate(effectEditor);
                }

                // Add remove button for the component
                if (GUILayout.Button("Remove Component"))
                {
                    RemoveEffect(effect.GetType());
                    break;
                }

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            var effectTypes = GetAvailableEffectTypes();
            int selectedEffectIndex = EditorGUILayout.Popup("Add Effect", -1, effectTypes);

            if (selectedEffectIndex >= 0)
            {
                Type selectedEffectType = Type.GetType(effectTypes[selectedEffectIndex]);
                if (selectedEffectType != null)
                {
                    AddEffect(selectedEffectType);
                }
            }
        }

        private string[] GetAvailableEffectTypes() => TextEffectRegistry.TextEffects.Where(x => !profile.HasTextEffect(x.Type)).Select(x => x.Type.FullName).ToArray();


        private void RemoveEffect(Type type)
        {
            Undo.RecordObject(profile, "Remove Text Effect");
            profile.RemoveTextEffect(type);
            EditorUtility.SetDirty(profile);
            UpdateAssets();
        }

        private void AddEffect(Type effectType)
        {
            Undo.RecordObject(profile, "Added Effect");
            profile.AddTextEffect(effectType);
            EditorUtility.SetDirty(profile);
            UpdateAssets();
        }

        public void UpdateAssets()
        {
            //Path of the main file
            string path = AssetDatabase.GetAssetPath(profile);

            //Gets all components
            var effects = profile.GetAllTextEffects();
            var currentSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            var componentsToRemove = new List<UnityEngine.Object>(currentSubAssets);

            foreach (var component in effects)
            {
                if (!AssetDatabase.IsSubAsset(component) && !AssetDatabase.Contains(component))
                {
                    AssetDatabase.AddObjectToAsset(component, path);
                }
                componentsToRemove.Remove(component);
            }

            foreach (var componentToRemove in componentsToRemove)
            {
                AssetDatabase.RemoveObjectFromAsset(componentToRemove);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
