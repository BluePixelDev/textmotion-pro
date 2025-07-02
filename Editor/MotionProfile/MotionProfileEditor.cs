using BP.TextMotion;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionProEditor
{
    [CustomEditor(typeof(MotionProfile))]
    public class MotionProfileEditor : Editor
    {
        [SerializeField] private VisualTreeAsset mainTreeAsset;
        private MotionProfile profile;

        private void OnEnable()
        {
            profile = (MotionProfile)target;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            mainTreeAsset.CloneTree(root);
            return root;
        }

        public void UpdateAssets()
        {
            // Path of the main file.
            string path = AssetDatabase.GetAssetPath(profile);

            // Gets all components.
            var components = profile.GetAllComponents();
            var currentSubAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            var componentsToRemove = new List<UnityEngine.Object>(currentSubAssets);

            foreach (var component in components)
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
