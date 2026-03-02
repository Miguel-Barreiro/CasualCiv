using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    [System.Serializable]
    public class SerializableSearchArguments {
        public List<string> stringArgs = new List<string>();
        public List<int> intArgs = new List<int>();
        public List<float> floatArgs = new List<float>();
        public List<bool> boolArgs = new List<bool>();
        public List<UnityEngine.Object> unityObjectArgs = new List<Object>();
    }

    public class BaseSearchBy {

        public delegate void FieldFoundCallback<T>(UnityEngine.Object rootObject, T fieldValue, string fieldPath);
        public delegate bool ComparerMethod<T>(T fieldValue);

        public virtual void CreateGUI(VisualElement customAreaRoot, SerializedProperty serializedProperty) {

        }

        public virtual List<ObjectFinder.ResultItem> FilterObject(ObjectFinder.TraversalMode traversalMode, string fieldNameFilter, UnityEngine.Object rootObject, UnityEngine.Object objectToSearch, SerializableSearchArguments searchArguments) {
            return null;
        }

        /// <summary>
        /// GUI template for each search result item.
        /// </summary>
        /// <param name="parentElement"></param>
        public virtual void OnCreateResultItemGUI(VisualElement parentElement) {
            var indexLabel = new Label();
            indexLabel.style.fontSize = 10;
            indexLabel.AOF_FixedWidth(36).AOF_Margin(2, 0, 2, 2);
            parentElement.Add(indexLabel);
            parentElement.Add(new ObjectField() {
                style = {
                    width = new StyleLength(new Length(40, LengthUnit.Percent)),
                    marginTop = new StyleLength(new Length(2, LengthUnit.Pixel))
                }
            });
        }

        /// <summary>
        /// Fill the contents of the previously created result item GUI with the actual result item data.
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="index"></param>
        /// <param name="resultItem"></param>
        public virtual void OnBindResultItemGUI(VisualElement parentElement, int index, ObjectFinder.ResultItem resultItem) {
            if (parentElement.ElementAt(0) is Label label) {
                label.text = (index + 1).ToString();
            }
            if (parentElement.ElementAt(1) is ObjectField objectField) {
                var obj = resultItem.targetObject is UnityEngine.Object ? resultItem.targetObject : null;
                if (obj == null) {
#if UNITY_2022_1_OR_NEWER
                    objectField.value = obj;
#else
                    objectField.value = null;
#endif
                }
                else {
                    objectField.objectType = obj.GetType();
                    objectField.value = obj;
                }
                objectField.SetEnabled(false);
            }
        }

        protected SerializedProperty GetStringArgsProperty(SerializedProperty serializedProperty) => serializedProperty.FindPropertyRelative("stringArgs");
        protected SerializedProperty GetIntArgsProperty(SerializedProperty serializedProperty) => serializedProperty.FindPropertyRelative("intArgs");
        protected SerializedProperty GetFloatArgsProperty(SerializedProperty serializedProperty) => serializedProperty.FindPropertyRelative("floatArgs");
        protected SerializedProperty GetBoolArgsProperty(SerializedProperty serializedProperty) => serializedProperty.FindPropertyRelative("boolArgs");
        protected SerializedProperty GetUnityObjectArgsProperty(SerializedProperty serializedProperty) => serializedProperty.FindPropertyRelative("unityObjectArgs");

        protected SerializedProperty GetStringArgProperty(SerializedProperty serializedProperty, int index) {
            var argsProp = GetStringArgsProperty(serializedProperty);
            if (argsProp.arraySize <= index) {
                argsProp.arraySize = index + 1;
                argsProp.serializedObject.ApplyModifiedProperties();
            }
            return argsProp.GetArrayElementAtIndex(index);
        }

        protected SerializedProperty GetIntArgProperty(SerializedProperty serializedProperty, int index) {
            var argsProp = GetIntArgsProperty(serializedProperty);
            if (argsProp.arraySize <= index) {
                argsProp.arraySize = index + 1;
                argsProp.serializedObject.ApplyModifiedProperties();
            }
            return argsProp.GetArrayElementAtIndex(index);
        }

        protected SerializedProperty GetFloatArgProperty(SerializedProperty serializedProperty, int index) {
            var argsProp = GetFloatArgsProperty(serializedProperty);
            if (argsProp.arraySize <= index) {
                argsProp.arraySize = index + 1;
                argsProp.serializedObject.ApplyModifiedProperties();
            }
            return argsProp.GetArrayElementAtIndex(index);
        }

        protected SerializedProperty GetBoolArgProperty(SerializedProperty serializedProperty, int index) {
            var argsProp = GetBoolArgsProperty(serializedProperty);
            if (argsProp.arraySize <= index) {
                argsProp.arraySize = index + 1;
                argsProp.serializedObject.ApplyModifiedProperties();
            }
            return argsProp.GetArrayElementAtIndex(index);
        }

        protected SerializedProperty GetUnityObjectArgProperty(SerializedProperty serializedProperty, int index) {
            var argsProp = GetUnityObjectArgsProperty(serializedProperty);
            if (argsProp.arraySize <= index) {
                argsProp.arraySize = index + 1;
                argsProp.serializedObject.ApplyModifiedProperties();
            }
            return argsProp.GetArrayElementAtIndex(index);
        }

        protected static TextField CreateSelectableTextElement() {
            var textField = new TextField() {
                style = {
                    flexGrow = 1f,
                    //marginTop = new StyleLength(new Length(1, LengthUnit.Pixel)),
                    backgroundColor = Color.clear
                },
                isReadOnly = true
            };
            textField.AOF_BorderWidth(0);

            var textInput = textField.Q("unity-text-input");
            if (textInput != null) {
                textInput.AOF_BorderWidth(0)
                    .AOF_BackgroundColor(Color.clear);
            }

            return textField;
        }

        protected string GetSceneObjectPath(Component component, bool includeRoot = true) {
            string path = component.name;
            var parent = component.transform.parent;
            int siblingIndex = component.transform.GetSiblingIndex();
            path = "(" + siblingIndex + ")" + path;
            if (parent != null) {
                if (includeRoot) {
                    return GetSceneObjectPath(parent) + "/" + path;
                }
                else {
                    return path;
                }
            }
            return path;
        }
        protected void TraverseSerializedObject<T>(string fieldNameFilter, ComparerMethod<T> comparer, FieldFoundCallback<T> onFieldFound, UnityEngine.Object objectToTraverse) {
            HashSet<UnityEngine.Object> traversedObjects = new HashSet<Object>();
            TraverseSerializedObject(fieldNameFilter, comparer, onFieldFound, objectToTraverse, objectToTraverse, traversedObjects);
        }

        protected void TraverseSerializedObject<T>(string fieldNameFilter, ComparerMethod<T> comparer, FieldFoundCallback<T> onFieldFound, UnityEngine.Object rootObject, UnityEngine.Object objectToTraverse, HashSet<UnityEngine.Object> traversedObjects = null) {
            if (objectToTraverse == null) return;
            var so = new SerializedObject(objectToTraverse);
            var sp = so.GetIterator();

            string[] searchSplits;
            if (fieldNameFilter == null) {
                searchSplits = null;
            }
            else {
                searchSplits = ObjectNames.NicifyVariableName(fieldNameFilter).ToLower().Split(' ', '.');
            }
            
            traversedObjects.Add(objectToTraverse);

            while (sp.NextVisible(true)) {
                bool checkProperty = true;
                if (fieldNameFilter != null) {
                    if (!StringUtility.CompareString(sp.propertyPath, fieldNameFilter, false, searchSplits, true)) {
                        checkProperty = false;
                    }
                }

                if (checkProperty) {
                    if (sp.propertyType == SerializedPropertyType.String && typeof(T) == typeof(string)) {
                        T propVal = (T)(object)sp.stringValue;
                        if (comparer.Invoke(propVal)) {
                            onFieldFound.Invoke(objectToTraverse, propVal, sp.propertyPath);
                        }
                    }
                    else if (sp.propertyType == SerializedPropertyType.Integer && (typeof(T) == typeof(int) || typeof(T) == typeof(short) || typeof(T) == typeof(long) || typeof(T) == typeof(uint) || typeof(T) == typeof(ushort) || typeof(T) == typeof(ulong))) {
                        T propVal = (T)(object)sp.intValue;
                        if (comparer.Invoke(propVal)) {
                            onFieldFound.Invoke(objectToTraverse, propVal, sp.propertyPath);
                        }
                    }
                    else if (sp.propertyType == SerializedPropertyType.Float && (typeof(T) == typeof(float) || typeof(T) == typeof(double))) {
                        T propVal = (T)(object)sp.floatValue;
                        if (comparer.Invoke(propVal)) {
                            onFieldFound.Invoke(objectToTraverse, propVal, sp.propertyPath);
                        }
                    }
                    else if (sp.propertyType == SerializedPropertyType.Boolean && typeof(T) == typeof(bool)) {
                        T propVal = (T)(object)sp.boolValue;
                        if (comparer.Invoke(propVal)) {
                            onFieldFound.Invoke(objectToTraverse, propVal, sp.propertyPath);
                        }
                    }
                    else if (sp.propertyType == SerializedPropertyType.Enum && typeof(T) == typeof(System.Enum)) {
                        var obj = ReflectionUtility.GetEnumValue(sp);
                        if (obj != null) {
                            var enumObj = (T)obj;
                            if (comparer.Invoke(enumObj)) {
                                onFieldFound.Invoke(objectToTraverse, enumObj, sp.propertyPath);
                            }
                        }
                    }
                    else if (sp.propertyType == SerializedPropertyType.ObjectReference && typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) {
                        T propVal = (T)(object)sp.objectReferenceValue;
                        if (comparer.Invoke(propVal)) {
                            onFieldFound.Invoke(objectToTraverse, propVal, sp.propertyPath);
                        }
                    }
                }

                if (sp.propertyType == SerializedPropertyType.ObjectReference && sp.objectReferenceValue != null) {
                    if (!traversedObjects.Contains(sp.objectReferenceValue)) {
                        TraverseSerializedObject<T>(fieldNameFilter, comparer, onFieldFound, rootObject, sp.objectReferenceValue, traversedObjects);
                    }
                }
            }

        }

        protected void TraverseFields<T>(string fieldNameFilter, ComparerMethod<T> comparer, FieldFoundCallback<T> onFieldFound, UnityEngine.Object objectToTraverse) {
            TraverseFields<T>(fieldNameFilter, comparer, onFieldFound, objectToTraverse, new List<object>() { objectToTraverse }, "");
        }

        protected void TraverseFields<T>(string fieldNameFilter, ComparerMethod<T> comparer, FieldFoundCallback<T> onFieldFound, UnityEngine.Object rootObject, List<object> visitedObjects, string currentPath) {
            // currentObjs helps tracking checked objects to prevent cyclic traversal
            object currentObj = visitedObjects[visitedObjects.Count - 1];
            if (currentObj == null) return;

            string[] searchSplits;
            if (fieldNameFilter == null) {
                searchSplits = null;
            }
            else {
                searchSplits = ObjectNames.NicifyVariableName(fieldNameFilter).ToLower().Split(' ', '.');
            }

            if (currentObj is Transform transformObject) {
                if (typeof(T) == typeof(float)) {

                    System.Action<string, object> checkTransformField = (transformFieldName, transformFieldValue) => {
                        if (fieldNameFilter != null) {
                            if (StringUtility.CompareString(currentPath + transformFieldName, fieldNameFilter, false, searchSplits, true)) {
                                if (comparer.Invoke((T)transformFieldValue)) {
                                    onFieldFound.Invoke(rootObject, (T)transformFieldValue, currentPath + transformFieldName);
                                }
                            }
                        }
                    };

                    checkTransformField("localPosition.x", transformObject.localPosition.x);
                    checkTransformField("localPosition.y", transformObject.localPosition.y);
                    checkTransformField("localPosition.z", transformObject.localPosition.z);

                    var localRotation = transformObject.localRotation.eulerAngles;
                    checkTransformField("localRotation.x", localRotation.x);
                    checkTransformField("localRotation.y", localRotation.y);
                    checkTransformField("localRotation.z", localRotation.z);

                    checkTransformField("localScale.x", transformObject.localScale.x);
                    checkTransformField("localScale.y", transformObject.localScale.y);
                    checkTransformField("localScale.z", transformObject.localScale.z);

                }
                return;
            }

            FieldInfo[] fields = ReflectionUtility.GetFieldInfosIncludingBaseClasses(currentObj.GetType(), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields) {
                if (!field.IsPublic && 
                    field.GetCustomAttribute<SerializeField>() == null
#if ODIN_INSPECTOR
                    && field.GetCustomAttribute<Sirenix.Serialization.OdinSerializeAttribute>() == null
#endif
                    ) continue;

                if (fieldNameFilter != null) {
                    if (!StringUtility.CompareString(currentPath + field.Name, fieldNameFilter, false, searchSplits, true)) {
                        TryTraverseFieldChildren(fieldNameFilter, comparer, onFieldFound, rootObject, visitedObjects, currentPath, currentObj, field, searchSplits);
                        continue;
                    }
                }

                if (field.FieldType == typeof(T)) {
                    T fieldObj = (T)field.GetValue(currentObj);
                    if (comparer.Invoke(fieldObj)) {
                        onFieldFound.Invoke(rootObject, fieldObj, currentPath + field.Name);
                    }
                }
                else if (field.FieldType == typeof(T[])) {
                    T[] array = (T[])field.GetValue(currentObj);
                    if (array != null) {
                        for (int i = 0; i < array.Length; i++) {
                            if (comparer.Invoke(array[i])) {
                                onFieldFound.Invoke(rootObject, array[i], currentPath + field.Name + '[' + i + ']');
                                //break;
                            }
                        }
                    }
                }
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>)) {
                    System.Type listType = field.FieldType.GetGenericArguments()[0];
                    if (listType == typeof(T)) {
                        IList<T> list = (IList<T>)field.GetValue(currentObj);
                        if (list != null) {
                            for (int i = 0; i < list.Count; i++) {
                                if (comparer.Invoke(list[i])) {
                                    onFieldFound.Invoke(rootObject, list[i], currentPath + field.Name + '[' + i + ']');
                                    //break;
                                }
                            }
                        }
                    }
                }
#if ODIN_INSPECTOR
                else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(Dictionary<,>)) {
                    System.Type[] dictionaryTypes = field.FieldType.GetGenericArguments();
                    if (dictionaryTypes[0] == typeof(T) || dictionaryTypes[1] == typeof(T)) {
                        IDictionary dictionary = (IDictionary)field.GetValue(currentObj);
                        if (dictionary != null) {
                            foreach (DictionaryEntry entry in dictionary) {
                                if (entry.Key is T keyObj) {
                                    if (comparer.Invoke(keyObj)) {
                                        onFieldFound.Invoke(rootObject, keyObj, currentPath + field.Name + '[' + entry.Key + "] (Key)");
                                        //break;
                                    }
                                }
                                if (entry.Value is T valueObj) {
                                    if (comparer.Invoke(valueObj)) {
                                        onFieldFound.Invoke(rootObject, valueObj, currentPath + field.Name + '[' + entry.Key + "] (Value)");
                                        //break;
                                    }
                                }
                            }
                        }
                    }
                }
#endif
                else if (field.FieldType.IsEnum) {
                    if (typeof(T) == typeof(System.Enum)) {
                        object fieldObj = field.GetValue(currentObj);
                        T enumVal = (T)fieldObj;
                        if (comparer.Invoke(enumVal)) {
                            onFieldFound.Invoke(rootObject, enumVal, currentPath + field.Name);
                        }
                    }
                }
                else {
                    TryTraverseFieldChildren(fieldNameFilter, comparer, onFieldFound, rootObject, visitedObjects, currentPath, currentObj, field, searchSplits);
                }
            }
        }

        private void TryTraverseFieldChildren<T>(string                 fieldNameFilter,
                                                ComparerMethod<T>       comparer, 
                                                FieldFoundCallback<T>   onFieldFound, 
                                                Object                  rootObject,
                                                List<object>            visitedObjects, 
                                                string                  currentPath, 
                                                object                  currentObj, 
                                                FieldInfo               field,
                                                string[]                searchSplits) {
            if (!field.FieldType.IsPrimitive && !typeof(System.Delegate).IsAssignableFrom(field.FieldType)) {
                object fieldValue = field.GetValue(currentObj);
                if (!visitedObjects.Contains(fieldValue)) {
                    visitedObjects.Add(fieldValue);
                    if (fieldValue is UnityEngine.Object objectReference) {
                        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T))) {
                            var objToCompare = (T)(object)objectReference;
                            if (comparer.Invoke(objToCompare)) {
                                if (fieldNameFilter == null) {
                                    onFieldFound.Invoke(rootObject, objToCompare, currentPath + field.Name);
                                }
                                else {
                                    if (StringUtility.CompareString(currentPath + field.Name, fieldNameFilter, false, searchSplits, true)) {
                                        onFieldFound.Invoke(rootObject, objToCompare, currentPath + field.Name);
                                    }
                                }
                            }
                        }
                        TraverseFields<T>(fieldNameFilter, comparer, onFieldFound, objectReference, visitedObjects, currentPath + field.Name + '.');
                    }
                    else {
                        TraverseFields<T>(fieldNameFilter, comparer, onFieldFound, rootObject, visitedObjects, currentPath + field.Name + '.');
                    }
                }
            }
        }

    }

}