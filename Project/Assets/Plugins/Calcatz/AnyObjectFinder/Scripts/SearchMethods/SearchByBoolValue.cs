using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public class SearchByBoolValue : BaseSearchBy {

        private SerializedProperty serializedProperty;

        private SerializedProperty boolValueProp;

        private IMGUIContainer imguiContainer;
        private GUIContent valueLabel;


        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;

            boolValueProp = GetBoolArgProperty(serializedProperty, 0);
            valueLabel = new GUIContent("Value:", "Search in all filtered objects' serialized property where it's a boolean with the same value as this.");

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
                EditorGUILayout.PropertyField(boolValueProp, valueLabel, GUILayout.ExpandWidth(true));
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
            bool boolValue = searchArguments.boolArgs.Count > 0 ? searchArguments.boolArgs[0] : false;

            List<ObjectFinder.ResultItem> resultItems = null;

            ComparerMethod<bool> comparer = fieldValue => {
                return fieldValue == boolValue;
            };

            FieldFoundCallback<bool> onFieldFound = (objectReference, fieldValue, fieldPath) => {

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
                TraverseSerializedObject<bool>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.ByReflection)) {
                TraverseFields<bool>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
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
