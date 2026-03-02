using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace Calcatz.AnyObjectFinder {

    public class SearchByStringValue : BaseSearchBy {

        private SerializedProperty serializedProperty;

        private SerializedProperty searchTermProp;
        private SerializedProperty caseSensitiveProp;

        private IMGUIContainer imguiContainer;
        private GUIContent valueLabel;


        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            this.serializedProperty = serializedProperty;

            searchTermProp = GetStringArgProperty(serializedProperty, 0);
            caseSensitiveProp = GetBoolArgProperty(serializedProperty, 0);
            valueLabel = new GUIContent("Value:", "Search in all filtered objects' serialized property where it's a string with the matching text input.");

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
                EditorGUILayout.PropertyField(searchTermProp, valueLabel, GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck()) {
                    changed = true;
                }
                GUILayout.Space(3);

                EditorGUIUtility.labelWidth = 90;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(caseSensitiveProp, new GUIContent("case-sensitive", "If not, then this will also treat pascal case as space."), GUILayout.MaxWidth(110));
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
            string searchTerm = searchArguments.stringArgs.Count > 0? searchArguments.stringArgs[0] : "";
            //if (string.IsNullOrEmpty(searchTerm)) return null;

            List<ObjectFinder.ResultItem> resultItems = null;
            bool caseSensitive = searchArguments.boolArgs.Count > 0? searchArguments.boolArgs[0] : false;

            string[] searchSplits;
            if (caseSensitive) searchSplits = null;
            else searchSplits = ObjectNames.NicifyVariableName(searchTerm).ToLower().Split(' ');

            if (traversalMode == ObjectFinder.TraversalMode.ByReflection) {
                if (rootObject != objectToSearch && objectToSearch is GameObject gameObjectToSearch) {
                    if (StringUtility.CompareString(gameObjectToSearch.name, searchTerm, caseSensitive, searchSplits)) {
                        string fieldPath = GetSceneObjectPath(gameObjectToSearch.transform, gameObjectToSearch.scene.IsValid()) + ": (GameObject's Name)";

                        if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                        resultItems.Add(new ObjectFinder.ResultItem() {
                            targetObject = gameObjectToSearch,
                            info = rootObject == gameObjectToSearch ? fieldPath : rootObject.name + " - " + fieldPath
                        });
                    }
                    if (StringUtility.CompareString(gameObjectToSearch.tag, searchTerm, caseSensitive, searchSplits)) {
                        string fieldPath = GetSceneObjectPath(gameObjectToSearch.transform, gameObjectToSearch.scene.IsValid()) + ": (GameObject's Tag)";

                        if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                        resultItems.Add(new ObjectFinder.ResultItem() {
                            targetObject = gameObjectToSearch,
                            info = rootObject == gameObjectToSearch ? fieldPath : rootObject.name + " - " + fieldPath
                        });
                    }
                }
                //else if (!(objectToSearch is Component)) {
                //    if (StringUtility.CompareString(objectToSearch.name, searchTerm, caseSensitive, searchSplits)) {
                //        if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                //        resultItems.Add(new ObjectFinder.ResultItem() {
                //            targetObject = rootObject,
                //            info = rootObject.name + ": (Object's Name)"
                //        });
                //    }
                //}
            }

            ComparerMethod<string> comparer = fieldValue => {
                return StringUtility.CompareString(fieldValue, searchTerm, caseSensitive, searchSplits);
            };

            FieldFoundCallback<string> onFieldFound = (objectReference, fieldValue, fieldPath) => {

                if (rootObject != objectToSearch) {
                    if (rootObject is SceneAsset && objectToSearch is Component component) {
                        fieldPath = GetSceneObjectPath(component) + ": " + fieldPath;
                    }
                }

                if (resultItems == null) resultItems = new List<ObjectFinder.ResultItem>();
                resultItems.Add(new ObjectFinder.ResultItem() {
                    targetObject = objectReference,
                    info = rootObject == objectReference? fieldPath : rootObject.name + " - " + fieldPath
                });
            };

            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.BySerializedObject)) {
                TraverseSerializedObject<string>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
            }
            if (traversalMode.HasFlag(ObjectFinder.TraversalMode.ByReflection)) {
                TraverseFields<string>(fieldNameFilter, comparer, onFieldFound, objectToSearch);
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
