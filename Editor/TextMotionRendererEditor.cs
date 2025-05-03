using BP.TextMotionPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionEditor
{
    [CustomEditor(typeof(TextMotionRenderer)), CanEditMultipleObjects]
    public class TextMotionRendererEditor : Editor
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
