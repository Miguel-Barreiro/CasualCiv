#if UNITY_EDITOR

using Calcatz.AnyObjectFinder;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.Example {

    /// <summary>
    /// A custom search-by example where it searches fields with odd integer value. 
    /// </summary>
    public class MyCustomSearch : BaseSearchBy {

        // The filter logic
        public override List<ObjectFinder.ResultItem> FilterObject(ObjectFinder.TraversalMode traversalMode,
                                                                   string fieldNameFilter,
                                                                   Object rootObject,
                                                                   Object objectToSearch,
                                                                   SerializableSearchArguments searchArguments) {
            var resultItems = new List<ObjectFinder.ResultItem>();

            ComparerMethod<int> comparer = fieldValue => {
                return fieldValue % 2 == 1;
            };

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

        // GUI area if additional parameter fields are needed.
        public override void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {
            base.CreateGUI(customAreaRoot, serializedProperty);
            customAreaRoot.Add(new Label("If additional parameters are needed, use this area."));
        }

        // GUI template for each search result item.
        public override void OnCreateResultItemGUI(VisualElement parentElement) {
            base.OnCreateResultItemGUI(parentElement);
            parentElement.Add(CreateSelectableTextElement());
        }

        // Fill the contents of the previously created result item GUI with the actual result item data.
        public override void OnBindResultItemGUI(VisualElement parentElement, int index, ObjectFinder.ResultItem resultItem) {
            base.OnBindResultItemGUI(parentElement, index, resultItem);
            if (parentElement.ElementAt(2) is TextField textField) {
                textField.value = resultItem.info;
            }
        }

    }

}

#endif


