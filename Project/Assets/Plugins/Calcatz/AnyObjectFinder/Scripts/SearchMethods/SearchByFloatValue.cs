using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public class SearchByFloatValue : BaseSearchBy {

        public enum FloatComparer {
            NearlyEqual = 0,
            LessThan = 1,
            LessThanEqual = 2,
            GreaterThan = 3,
            GreaterThanEqual = 4,
            NotEqual = 5
        }

        private SerializedProperty serializedProperty;

        private SerializedProperty floatValueProp;
        private SerializedProperty comparerEnumProp;

        private IMGUIContainer imguiContainer;
        private GUIContent valueLabel;


        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;

            floatValueProp = GetFloatArgProperty(serializedProperty, 0);
            comparerEnumProp = GetIntArgProperty(serializedProperty, 0);
            valueLabel = new GUIContent("Value:", "Search in all filtered objects' serialized property where it's a float with the matched condition.");

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
                comparerEnumProp.intValue = (int)(FloatComparer)EditorGUILayout.EnumPopup(valueLabel, (FloatComparer)comparerEnumProp.intValue, GUILayout.MinWidth(160), GUILayout.MaxWidth(160));
                EditorGUILayout.PropertyField(floatValueProp, GUIContent.none, GUILayout.ExpandWidth(true));
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
            float floatValue = searchArguments.floatArgs.Count > 0 ? searchArguments.floatArgs[0] : 0f;
            FloatComparer floatComparer = searchArguments.intArgs.Count > 0 ? (FloatComparer)searchArguments.intArgs[0] : FloatComparer.NearlyEqual;

            List<ObjectFinder.ResultItem> resultItems = null;

            ComparerMethod<float> comparer;
            switch (floatComparer) {
                case FloatComparer.NearlyEqual:
                    comparer = fieldValue => {
                        float min = floatValue - float.Epsilon;
                        float max = floatValue + float.Epsilon;
                        return fieldValue >= min && fieldValue <= max;
                    };
                    break;
                case FloatComparer.LessThan:
                    comparer = fieldValue => fieldValue < floatValue;
                    break;
                case FloatComparer.LessThanEqual:
                    comparer = fieldValue => fieldValue <= floatValue;
                    break;
                case FloatComparer.GreaterThan:
                    comparer = fieldValue => fieldValue > floatValue;
                    break;
                case FloatComparer.GreaterThanEqual:
                    comparer = fieldValue => fieldValue >= floatValue;
                    break;
                default:
                    comparer = fieldValue => {
                        float min = floatValue - float.Epsilon;
                        float max = floatValue + float.Epsilon;
                        return fieldValue < min || fieldValue > max;
                    };
                    break;
            }

            FieldFoundCallback<float> onFieldFound = (objectReference, fieldValue, fieldPath) => {

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
                TraverseSerializedObject<float>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.ByReflection)) {
                TraverseFields<float>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
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
