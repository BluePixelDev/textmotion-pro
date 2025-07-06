using UnityEditor;

namespace BP.TextMotionPro.Editor
{
    [CustomEditor(typeof(MotionComponent), editorForChildClasses: true, isFallback = true)]
    internal class MotionComponentEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Iterate through all visible properties
            SerializedProperty prop = serializedObject.GetIterator();
            bool isFirst = true;

            while (prop.NextVisible(isFirst))
            {
                // Skip the script field
                if (prop.name == "m_Script")
                {
                    isFirst = false;
                    continue;
                }

                // Draw the property
                EditorGUILayout.PropertyField(prop, true);
                isFirst = false;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
