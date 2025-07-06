using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionPro.Editor
{
    [CustomEditor(typeof(TextMotionPro)), CanEditMultipleObjects]
    public class TextMotionProEditor : UnityEditor.Editor
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
