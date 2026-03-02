using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    [System.Serializable]
    public class ObjectFinder {

        [System.Flags]
        public enum SearchTarget {
            Assets = 1,
            Scenes = 2
        }

        [System.Flags]
        public enum TraversalMode {
            BySerializedObject = 1,
            ByReflection = 2
        }

        public SearchTarget searchTarget = SearchTarget.Assets | SearchTarget.Scenes;
        public string typeNameToSearch = "";
        public List<string> selectedFolders = new List<string>();
        public TraversalMode traversalMode = TraversalMode.BySerializedObject;
        public bool filterByFieldName = false;
        public string fieldNameFilter = "";
        public SerializableSearchArguments searchArguments = new SerializableSearchArguments();

        public static List<System.Type> GetAvailableSearchMethods() {
            var result = TypeCache.GetTypesDerivedFrom<BaseSearchBy>();
            return result.ToList();
        }

        public static List<System.Type> GetAvailableObjectTypes() {
            var result = new List<System.Type>();
            //result.AddRange(TypeCache.GetTypesDerivedFrom<Component>());
            //result.AddRange(TypeCache.GetTypesDerivedFrom<ScriptableObject>());
            //result.AddRange(TypeCache.GetTypesDerivedFrom<GameObject>());
            result.AddRange(TypeCache.GetTypesDerivedFrom<UnityEngine.Object>());
            result = result.OrderBy(type => type.Name).ToList();
            //result.Insert(0, null);
            return result;
        }

        public List<ResultItem> Search(BaseSearchBy searchMethod, System.Type typeToSearch) {
            List<ResultItem> result = new List<ResultItem>();
            EditorUtility.DisplayProgressBar("Searching Object", "Getting assets...", 0f);

            var validFolders = selectedFolders.Where(folder => {
                return folder[folder.Length - 1] != '~' && 
                        !folder.Contains("~\\") && 
                        !folder.Contains("\\.") &&
                        !folder.Contains("~/") &&
                        !folder.Contains("/.");
            }).ToArray();

            bool targetAssets = searchTarget.HasFlag(SearchTarget.Assets);
            bool targetScenes = searchTarget.HasFlag(SearchTarget.Scenes) &&
                                (typeToSearch == null || typeof(Component).IsAssignableFrom(typeToSearch));

            string[] guids;
            if (typeToSearch == null)
                guids = AssetDatabase.FindAssets("", validFolders);
            else
                guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeToSearch.Name), validFolders);

            for (int i = 0; i < guids.Length; i++) {

                string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
                UnityEngine.Object asset;
                try {
                    asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                }
                catch {
                    asset = null;
                }
                if (asset == null) continue;

                if (targetScenes && asset is SceneAsset sceneAsset) {
                    if (EditorUtility.DisplayCancelableProgressBar("Searching Objects", '[' + (i + 1).ToString() + " / " + guids.Length + "] Checking scene: " + asset.name, (float)(i + 1) / (float)guids.Length)) {
                        break;
                    }

                    bool isTempScene = false;
                    var scene = EditorSceneManager.GetSceneByPath(assetPath);
                    if (!scene.IsValid()) {
                        isTempScene = true;
                        scene = EditorSceneManager.OpenScene(assetPath, OpenSceneMode.Additive);
                    }
                    if (scene.IsValid()) {
                        GameObject[] rootGameObjects = scene.GetRootGameObjects();
                        var componentType = typeToSearch == null? typeof(Component) : typeToSearch;
                        System.Action<UnityEngine.Object> objectHandler = unityObject => {
                            FilterObject(searchMethod, result, sceneAsset, unityObject);
                        };
                        foreach (GameObject rootGameObject in rootGameObjects) {
                            TraverseTransform(rootGameObject.transform, componentType, objectHandler);
                        }

                        if (isTempScene) {
                            EditorSceneManager.CloseScene(scene, true);
                        }
                    }
                    else {
                        Debug.LogError("Failed to load the scene: " + assetPath);
                    }
                }
                else if (targetAssets) {
                    if (EditorUtility.DisplayCancelableProgressBar("Searching Objects", '[' + (i + 1).ToString() + " / " + guids.Length + "] Checking asset: " + asset.name, (float)(i + 1) / (float)guids.Length)) {
                        break;
                    }
                    if (asset is GameObject prefabAsset) {
                        var componentType = typeToSearch == null ? typeof(Component) : typeToSearch;
                        System.Action<UnityEngine.Object> objectHandler = unityObject => {
                            FilterObject(searchMethod, result, prefabAsset, unityObject);
                        };
                        TraverseTransform(prefabAsset.transform, componentType, objectHandler);
                    }
                    else {
                        FilterObject(searchMethod, result, asset, asset);
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            return result;
        }

        private void FilterObject(BaseSearchBy searchMethod, List<ResultItem> result, UnityEngine.Object rootObject, UnityEngine.Object objectToSearch) {
            try {
                var resultItems = searchMethod.FilterObject(traversalMode, filterByFieldName? fieldNameFilter : null, rootObject, objectToSearch, searchArguments);
                if (resultItems != null) {
                    foreach (var resultItem in resultItems) {
                        if (resultItem == null) continue;
                        if (resultItem.targetObject == null) resultItem.targetObject = rootObject;
                    }
                    result.AddRange(resultItems);
                }
            }
            catch (System.Exception e) {
                Debug.LogError(e.Message);
            }
        }

        private static void TraverseTransform(Transform transform, System.Type typeToSearch, System.Action<UnityEngine.Object> objectHandler) {
            var components = transform.GetComponents(typeToSearch);

            objectHandler.Invoke(transform.gameObject);
            foreach (var component in components) {
                objectHandler.Invoke(component);
            }

            for (int i=0; i<transform.childCount; i++) {
                TraverseTransform(transform.GetChild(i), typeToSearch, objectHandler);
            }
        }

        [System.Serializable]
        public class ResultItem {
            public int index;
            public UnityEngine.Object targetObject;
            public string info;
        }

    }

}