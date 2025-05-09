using BP.TextMotion;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionProEditor
{
    [CustomEditor(typeof(MotionProfile))]
    public class TextEffectsProfileEditor : Editor
    {
        [SerializeField] private VisualTreeAsset mainTreeAsset;
        [SerializeField] private VisualTreeAsset effectTreeAsset;

        private MotionProfile profile;

        private void OnEnable()
        {
            profile = (MotionProfile)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            mainTreeAsset.CloneTree(root);

            var effectsList = root.Q("effects-list");
            UpdateEffectsList(effectsList);

            // Setup the add button functionality.
            var addButton = root.Q<Button>("add-button");
            addButton.clicked += () =>
            {
                var menu = new GenericMenu();
                var availableTypes = GetAvailableEffectTypes();
                foreach (var effectTypeName in availableTypes)
                {
                    string menuLabel = Type.GetType(effectTypeName).Name;
                    menu.AddItem(new GUIContent(menuLabel), false, () =>
                    {
                        AddEffect(effectTypeName);
                        UpdateEffectsList(effectsList);
                    });
                }
                menu.ShowAsContext();
            };

            return root;
        }

        private void UpdateEffectsList(VisualElement parent)
        {
            parent.Clear();
            foreach (var effect in profile.GetAllTextEffects())
            {
                if (effect == null) continue;

                var serObject = new SerializedObject(effect);
                var foldedProp = serObject.FindProperty("isFolded");

                var entry = effectTreeAsset.CloneTree();
                var effectLabel = entry.Q<Label>("label");
                effectLabel.text = effect.GetType().Name;

                var content = entry.Q("content");
                content.style.display = foldedProp.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
                content.TrackPropertyValue(foldedProp, (prop) =>
                {
                    content.style.display = prop.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
                });

                DrawEditor(content, effect);

                // Clicking the label toggles folding.
                effectLabel.RegisterCallback<ClickEvent>(evt =>
                {
                    foldedProp.boolValue = !foldedProp.boolValue;
                    serObject.ApplyModifiedProperties();
                });

                // Setup the options button to open a context menu with removal.
                var optionsButton = entry.Q<ToolbarButton>("options-button");
                if (optionsButton != null)
                {
                    optionsButton.clicked += () =>
                    {
                        var menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Remove"), false, () =>
                        {
                            RemoveEffect(effect.GetType());
                            UpdateEffectsList(parent);
                        });
                        menu.ShowAsContext();
                    };
                }

                parent.Add(entry);
            }
        }

        private void DrawEditor(VisualElement root, UnityEngine.Object target)
        {
            var editor = CreateEditor(target);
            if (editor == null) return;

            var editorContentElement = editor.CreateInspectorGUI();

            if (editorContentElement != null)
            {
                root.Add(editorContentElement);
            }
            else
            {
                var imgui = new IMGUIContainer(() =>
                {
                    editor.OnInspectorGUI();
                });
                root.Add(imgui);
            }
            DestroyImmediate(editor);
        }

        private string[] GetAvailableEffectTypes() => TextEffectRegistry.TextEffects
              .Where(x => !profile.HasTextEffect(x.Type))
              .Select(x => x.Type.AssemblyQualifiedName)
              .ToArray();

        private void RemoveEffect(Type type)
        {
            Undo.RecordObject(profile, "Remove Text Effect");
            profile.RemoveTextEffect(type);
            EditorUtility.SetDirty(profile);
            UpdateAssets();
        }

        private void AddEffect(string effectTypeName)
        {
            Type effectType = Type.GetType(effectTypeName);
            if (effectType == null) return;

            Undo.RecordObject(profile, "Added Effect");
            profile.AddTextEffect(effectType);
            EditorUtility.SetDirty(profile);
            UpdateAssets();
        }

        public void UpdateAssets()
        {
            // Path of the main file.
            string path = AssetDatabase.GetAssetPath(profile);

            // Gets all components.
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
