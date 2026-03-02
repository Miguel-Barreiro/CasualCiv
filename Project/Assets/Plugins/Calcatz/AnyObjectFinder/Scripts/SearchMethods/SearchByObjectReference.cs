using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public class SearchByObjectReference : BaseSearchBy {

        private SerializedProperty serializedProperty;

        private SerializedProperty objectToCompareProp;

        private IMGUIContainer imguiContainer;
        private GUIContent valueLabel;


        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;

            objectToCompareProp = GetUnityObjectArgProperty(serializedProperty, 0);
            valueLabel = new GUIContent("Object:", "Search in all filtered objects' serialized property or prefab instance where it's a unity object with the same reference with the assigned object.");

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
                EditorGUIUtility.labelWidth = 44;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(objectToCompareProp, valueLabel, GUILayout.ExpandWidth(true));
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
            UnityEngine.Object objectToCompare = searchArguments.unityObjectArgs.Count > 0 ? searchArguments.unityObjectArgs[0] : null;

            List<ObjectFinder.ResultItem> resultItems = null;

            if (objectToCompare is GameObject && objectToSearch is GameObject gameObjectToSearch) {
                if (gameObjectToSearch.scene.IsValid()) {
                    if (PrefabUtility.GetPrefabInstanceStatus(gameObjectToSearch) == PrefabInstanceStatus.Connected) {
                        if (PrefabUtility.GetCorrespondingObjectFromSource(gameObjectToSearch) == objectToCompare) {
                            var fieldPath = GetSceneObjectPath(gameObjectToSearch.transform) + ": (Prefab Instance)";

                            if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                            resultItems.Add(new ObjectFinder.ResultItem() {
                                targetObject = gameObjectToSearch,
                                info = rootObject == gameObjectToSearch ? fieldPath : rootObject.name + " - " + fieldPath
                            });
                        }
                    }
                }
                else { // In nested prefab
                    if (objectToSearch != rootObject && objectToSearch == gameObjectToSearch) {
                        var fieldPath = GetSceneObjectPath(gameObjectToSearch.transform, false) + ": (Nested Prefab Reference)";

                        if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                        resultItems.Add(new ObjectFinder.ResultItem() {
                            targetObject = gameObjectToSearch,
                            info = rootObject == gameObjectToSearch ? fieldPath : rootObject.name + " - " + fieldPath
                        });
                    }
                }
            }

            ComparerMethod<UnityEngine.Object> comparer = fieldValue => {
                return fieldValue == objectToCompare;
            };

            FieldFoundCallback<UnityEngine.Object> onFieldFound = (objectReference, fieldValue, fieldPath) => {

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
                TraverseSerializedObject<UnityEngine.Object>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.ByReflection)) {
                TraverseFields<UnityEngine.Object>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
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
