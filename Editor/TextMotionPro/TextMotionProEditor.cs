using BP.TextMotion;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionEditor
{
    [CustomEditor(typeof(TextMotionPro)), CanEditMultipleObjects]
    public class TextMotionProEditor : Editor
    {
        [SerializeField] private VisualTreeAsset treeAsset;
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            treeAsset.CloneTree(root);
            return root;
        }
    }
}
