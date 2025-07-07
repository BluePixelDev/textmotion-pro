using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BP.TextMotionPro.Editor
{
    [CustomPropertyDrawer(typeof(MotionComponentCollection<>), true)]
    public class MotionComponentRegistryEditor : PropertyDrawer
    {
        [SerializeField] private readonly VisualTreeAsset listContainerAsset;
        [SerializeField] private readonly VisualTreeAsset elementContainerAsset;

        private VisualElement listRoot;
        private SerializedProperty listProperty;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var root = listContainerAsset.CloneTree();
            listRoot = root.Q<VisualElement>("elements-list");
            listProperty = property.FindPropertyRelative("components");

            RefreshList();
            return root;
        }

        private void RefreshList()
        {
            listRoot.Clear();
            for (int i = 0; i < listProperty.arraySize; i++)
            {
                listRoot.Add(CreateComponentElement(i));
            }
        }

        private VisualElement CreateComponentElement(int index)
        {
            var elementProp = listProperty.GetArrayElementAtIndex(index);
            var motionComponent = elementProp.objectReferenceValue as MotionComponent;
            if (motionComponent == null) return new Label("Missing component");

            var container = elementContainerAsset.CloneTree();
            var header = container.Q<VisualElement>("header");
            var body = container.Q<VisualElement>("body");
            var optionsButton = container.Q<ToolbarButton>("options-button");

            SetupHeader(header, motionComponent, body);
            SetupOptionsMenu(optionsButton, index);

            return container;
        }

        private void SetupHeader(VisualElement header, MotionComponent component, VisualElement body)
        {
            var type = component.GetType();
            var attr = type.GetCustomAttribute<ComponentDescriptorAttribute>();
            header.Q<Label>("label").text = attr?.DisplayName ?? type.Name;

            var foldoutClickable = header.Q("foldout-box");
            var activeToggle = header.Q<Toggle>("active-toggle");

            var so = new SerializedObject(component);
            var isFoldedProp = so.FindProperty("isFolded");
            var isActiveProp = so.FindProperty("isActive");

            activeToggle.BindProperty(isActiveProp);

            body.Add(CreateEditorElement(so));
            UpdateBodyVisibility(body, isFoldedProp.boolValue);

            foldoutClickable.RegisterCallback<ClickEvent>(_ =>
            {
                isFoldedProp.boolValue = !isFoldedProp.boolValue;
                so.ApplyModifiedProperties();
                UpdateBodyVisibility(body, isFoldedProp.boolValue);
            });

            foldoutClickable.TrackPropertyValue(isFoldedProp, prop =>
            {
                UpdateBodyVisibility(body, prop.boolValue);
            });
        }

        private void UpdateBodyVisibility(VisualElement body, bool isFolded)
        {
            body.style.display = isFolded ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void SetupOptionsMenu(ToolbarButton button, int index)
        {
            button.clicked += () =>
            {
                var menu = new GenericMenu();

                // Remove option
                menu.AddItem(new GUIContent("Remove"), false, () =>
                {
                    RemoveComponentAt(index);
                });
                menu.DropDown(button.worldBound);
            };
        }

        private void RemoveComponentAt(int index)
        {
            var targetObj = listProperty.serializedObject.targetObject;
            Undo.RecordObject(targetObj, "Remove Motion Component");

            listProperty.DeleteArrayElementAtIndex(index);
            listProperty.serializedObject.ApplyModifiedProperties();

            RefreshList();
        }

        private VisualElement CreateEditorElement(SerializedObject serializedObject)
        {
            var editor = UnityEditor.Editor.CreateEditor(serializedObject.targetObject);
            if (editor == null)
                return new Label("No custom editor available");

            var inspectorGUI = editor.CreateInspectorGUI();
            if (inspectorGUI != null)
                return inspectorGUI;

            return new IMGUIContainer(() =>
            {
                serializedObject.Update();
                editor.OnInspectorGUI();
                serializedObject.ApplyModifiedProperties();
            });
        }
    }
}
