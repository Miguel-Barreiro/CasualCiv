using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public class SearchByIntValue : BaseSearchBy {

        public enum IntComparer {
            Equal = 0,
            LessThan = 1,
            LessThanEqual = 2,
            GreaterThan = 3,
            GreaterThanEqual = 4,
            NotEqual = 5
        }

        private SerializedProperty serializedProperty;

        private SerializedProperty intValueProp;
        private SerializedProperty comparerEnumProp;

        private IMGUIContainer imguiContainer;
        private GUIContent valueLabel;


        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;

            intValueProp = GetIntArgProperty(serializedProperty, 2);
            comparerEnumProp = GetIntArgProperty(serializedProperty, 1);
            valueLabel = new GUIContent("Value:", "Search in all filtered objects' serialized property where it's an integer with the matched condition.");

            imguiContainer = new IMGUIContainer(OnGUI);
            imguiContainer.AOF_Grow();
            imguiContainer.AOF_Margin(2, 2, 2, 2);
            customAreaRoot.Add(imguiContainer);
        }

        private void OnGUI() {
            serializedProperty.serializedObject.Update();
            bool changed = false;

            GUILayout.BeginHorizontal();
            {
                var prevLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 40;
                EditorGUI.BeginChangeCheck();
                comparerEnumProp.intValue = (int)(IntComparer)EditorGUILayout.EnumPopup(valueLabel, (IntComparer)comparerEnumProp.intValue, GUILayout.MinWidth(160), GUILayout.MaxWidth(160));
                EditorGUILayout.PropertyField(intValueProp, GUIContent.none, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck()) {
                    changed = true;
                }
                EditorGUIUtility.labelWidth = prevLabelWidth;
            }
            GUILayout.EndHorizontal();

            if (changed) {
                serializedProperty.serializedObject.ApplyModifiedProperties();
                imguiContainer.MarkDirtyRepaint();
            }
            var e = Event.current;
            if (e.type == EventType.ValidateCommand) {
                if (e.commandName == "UndoRedoPerformed") {
                    imguiContainer.MarkDirtyRepaint();
                }
            }
        }

        public override List<ObjectFinder.ResultItem> FilterObject(ObjectFinder.TraversalMode traversalMode, string fieldNameFilter, UnityEngine.Object rootObject, UnityEngine.Object objectToSearch, SerializableSearchArguments searchArguments) {
            float intValue = searchArguments.intArgs.Count >= 2 ? searchArguments.intArgs[2] : 0f;
            IntComparer intComparer = searchArguments.intArgs.Count >= 1 ? (IntComparer)searchArguments.intArgs[1] : IntComparer.Equal;

            List<ObjectFinder.ResultItem> resultItems = null;

            ComparerMethod<int> comparer;
            switch (intComparer) {
                case IntComparer.Equal:
                    comparer = fieldValue => fieldValue == intValue;
                    break;
                case IntComparer.LessThan:
                    comparer = fieldValue => fieldValue < intValue;
                    break;
                case IntComparer.LessThanEqual:
                    comparer = fieldValue => fieldValue <= intValue;
                    break;
                case IntComparer.GreaterThan:
                    comparer = fieldValue => fieldValue > intValue;
                    break;
                case IntComparer.GreaterThanEqual:
                    comparer = fieldValue => fieldValue >= intValue;
                    break;
                default:
                    comparer = fieldValue => fieldValue != intValue;
                    break;
            }

            if (objectToSearch is GameObject gameObjectToSearch) {
                if (comparer.Invoke(gameObjectToSearch.layer)) {
                    string fieldPath = GetSceneObjectPath(gameObjectToSearch.transform, gameObjectToSearch.scene.IsValid()) + ": (GameObject's Layer)";

                    if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                    resultItems.Add(new ObjectFinder.ResultItem() {
                        targetObject = gameObjectToSearch,
                        info = rootObject == gameObjectToSearch ? fieldPath : rootObject.name + " - " + fieldPath
                    });
                }
            }

            FieldFoundCallback<int> onFieldFound = (objectReference, fieldValue, fieldPath) => {

                if (rootObject != objectToSearch) {
                    if (rootObject is SceneAsset && objectToSearch is Component component) {
                        fieldPath = GetSceneObjectPath(component) + ": " + fieldPath;
                    }
                }

                if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                resultItems.Add(new ObjectFinder.ResultItem() {
                    targetObject = objectReference,
                    info = rootObject == objectReference ? fieldPath : rootObject.name + " - " + fieldPath
                });
            };

            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.BySerializedObject)) {
                TraverseSerializedObject<int>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.ByReflection)) {
                TraverseFields<int>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            return resultItems;
        }

        public override void OnCreateResultItemGUI(VisualElement parentElement) {
            base.OnCreateResultItemGUI(parentElement);
            parentElement.Add(CreateSelectableTextElement());
        }

        public override void OnBindResultItemGUI(VisualElement parentElement, int index, ObjectFinder.ResultItem resultItem) {
            base.OnBindResultItemGUI(parentElement, index, resultItem);
            if (parentElement.ElementAt(2) is TextField textField) {
                textField.value = resultItem.info;
            }
        }

    }

}
