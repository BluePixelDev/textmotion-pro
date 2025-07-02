using BP.TextMotion;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionEditor
{

    [CustomPropertyDrawer(typeof(MotionCollection<>), true)]
    public class MotionCollectionEditor : PropertyDrawer
    {
        [SerializeField] private VisualTreeAsset listContainerAsset;
        [SerializeField] private VisualTreeAsset elementContainerAsset;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = listContainerAsset.CloneTree();
            var listRoot = root.Q<VisualElement>("elements-list");
            var addButton = root.Q<Button>("add-element-button");
            var listProp = property.FindPropertyRelative("components");

            void RefreshList()
            {
                listRoot.Clear();
                for (int i = 0; i < listProp.arraySize; i++)
                {
                    var elementProp = listProp.GetArrayElementAtIndex(i);
                    var objRef = elementProp.objectReferenceValue as MotionComponent;
                    if (objRef == null) continue;

                    var elemContainer = elementContainerAsset.CloneTree();
                    var header = elemContainer.Q<VisualElement>("header");
                    var body = elemContainer.Q<VisualElement>("body");
                    var optionsButton = elemContainer.Q<ToolbarButton>("options-button");

                    var type = objRef.GetType();
                    var attr = type.GetCustomAttribute<TextMotionAttribute>();
                    var displayName = attr != null ? attr.DisplayName : type.Name;
                    header.Q<Label>("label").text = displayName;

                    var so = new SerializedObject(objRef);
                    body.Add(DrawEditor(so));

                    var isFoldedProp = so.FindProperty("isFolded");
                    void UpdateBody()
                    {
                        body.style.display = isFoldedProp.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                    }
                    UpdateBody();
                    header.TrackPropertyValue(isFoldedProp, (prop) => UpdateBody());
                    header.RegisterCallback<ClickEvent>(evt =>
                    {
                        isFoldedProp.boolValue = !isFoldedProp.boolValue;
                        so.ApplyModifiedProperties();
                    });


                    // Remove button
                    optionsButton.clicked += () =>
                    {
                        var menu = new GenericDropdownMenu();
                        menu.AddItem("Remove", false, () =>
                        {
                            var listTarget = property.serializedObject.targetObject as ScriptableObject;
                            Undo.RecordObject(listTarget, "Remove Component");
                            listProp.DeleteArrayElementAtIndex(i);
                            property.serializedObject.ApplyModifiedProperties();
                            RefreshList();
                        });
                        menu.DropDown(optionsButton.worldBound, optionsButton, false);
                    };

                    listRoot.Add(elemContainer);
                }
            }

            RefreshList();
            return root;
        }

        private VisualElement DrawEditor(SerializedObject serializedObject)
        {
            var targetObject = serializedObject.targetObject;
            var editor = Editor.CreateEditor(targetObject);
            if (!editor)
            {
                return new Label("No editor implemented");
            }

            var visualElement = editor.CreateInspectorGUI();
            if (visualElement != null)
            {
                return visualElement;
            }

            var container = new IMGUIContainer(() =>
            {
                serializedObject.Update();
                editor.OnInspectorGUI();
                serializedObject.ApplyModifiedProperties();
            });

            Object.DestroyImmediate(editor);
            return container;
        }
    }
}
