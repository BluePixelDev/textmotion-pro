using UnityEditor;

namespace BP.TMPA.Internal
{
    [CustomEditor(typeof(TextEffectBase), true)]
    internal class TextEffectEditor : Editor
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
