using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Calcatz.AnyObjectFinder {

#if UNITY_6000_2_OR_NEWER
    public class FolderTreeView : TreeView<int> {
#else
    public class FolderTreeView : TreeView {
#endif

        private HashSet<string> selectedFolders = new HashSet<string>();

#if UNITY_6000_2_OR_NEWER
        public FolderTreeView(TreeViewState<int> state) : base(state) {
#else
        public FolderTreeView(TreeViewState state) : base(state) {
#endif
            Reload();
        }

#if UNITY_6000_2_OR_NEWER
        public FolderTreeView(TreeViewState<int> state, List<string> selectedFolders) : base(state) {
#else
        public FolderTreeView(TreeViewState state, List<string> selectedFolders) : base(state) {
#endif
            this.selectedFolders.Clear();
            foreach (var selectedFolder in selectedFolders) {
                this.selectedFolders.Add(selectedFolder);
            }
            Reload();
        }

        public void SetSelectedFolders(List<string> selectedFolders) {
            this.selectedFolders.Clear();
            foreach (var selectedFolder in selectedFolders) {
                this.selectedFolders.Add(selectedFolder);
            }
        }

        public List<string> GetSelectedFolders() {
            var result = new List<string>();
            foreach (var selectedFolder in selectedFolders) {
                result.Add(selectedFolder.Replace('\\', '/'));
            }
            return result;
        }

        public List<string> GetAllFolders() {
            var result = new List<string>();
            GetFoldersRecursive(result, rootItem.children[0]);
            GetFoldersRecursive(result, rootItem.children[1]);
            return result;
        }

        public List<string> GetAssetsFolders() {
            var result = new List<string>();
            GetFoldersRecursive(result, rootItem.children[0]);
            return result;
        }

#if UNITY_6000_2_OR_NEWER
        private void GetFoldersRecursive(List<string> result, TreeViewItem<int> item) {
#else
        private void GetFoldersRecursive(List<string> result, TreeViewItem item) {
#endif
            result.Add(GetFullPath(item));
            if (item.hasChildren) {
                foreach (var child in item.children) {
                    GetFoldersRecursive(result, child);
                }
            }
        }

#if UNITY_6000_2_OR_NEWER
        protected override TreeViewItem<int> BuildRoot() {
            var root = new TreeViewItem<int> { id = 0, depth = -1, displayName = "Root" };

            // Add Assets and Packages folders
            var assetFolder = new TreeViewItem<int> { id = 1, depth = 0, displayName = "Assets" };
            var packagesFolder = new TreeViewItem<int> { id = 2, depth = 0, displayName = "Packages" };
#else
        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

            // Add Assets and Packages folders
            var assetFolder = new TreeViewItem { id = 1, depth = 0, displayName = "Assets" };
            var packagesFolder = new TreeViewItem { id = 2, depth = 0, displayName = "Packages" };
#endif

            root.AddChild(assetFolder);
            root.AddChild(packagesFolder);

            AddChildren(assetFolder, "Assets");
            AddPackagesChildren(packagesFolder);

            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

#if UNITY_6000_2_OR_NEWER
        private void AddChildren(TreeViewItem<int> parent, string path) {
#else
        private void AddChildren(TreeViewItem parent, string path) {
#endif
            if (!Directory.Exists(path))
                return;

            var directories = Directory.GetDirectories(path);
            AddChildrenFromDirectories(parent, directories);
        }

#if UNITY_6000_2_OR_NEWER
        private void AddPackagesChildren(TreeViewItem<int> parent) {
#else
        private void AddPackagesChildren(TreeViewItem parent) {
#endif
#if UNITY_2021_1_OR_NEWER
            var packages = UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages();
#else
        var packages = typeof(UnityEditor.PackageManager.PackageInfo)
            .GetMethod("GetAll", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
            .Invoke(null, new object[0]) as UnityEditor.PackageManager.PackageInfo[];
#endif
            AddChildrenFromDirectories(parent, packages.Select(_packageInfo => _packageInfo.assetPath));
        }

#if UNITY_6000_2_OR_NEWER
        private void AddChildrenFromDirectories(TreeViewItem<int> parent, IEnumerable<string> directories) {
#else
        private void AddChildrenFromDirectories(TreeViewItem parent, IEnumerable<string> directories) {
#endif
            foreach (var directory in directories) {
                var directoryName = Path.GetFileName(directory);
#if UNITY_6000_2_OR_NEWER
                var child = new TreeViewItem<int> { id = directory.GetHashCode(), depth = parent.depth + 1, displayName = directoryName };
#else
                var child = new TreeViewItem { id = directory.GetHashCode(), depth = parent.depth + 1, displayName = directoryName };
#endif
                parent.AddChild(child);
                AddChildren(child, directory);
            }
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = args.item;
            var indent = GetContentIndent(item);

            Rect toggleRect = args.rowRect;
            toggleRect.x += indent;
            toggleRect.width = 18;

            bool isSelected = selectedFolders.Contains(GetFullPath(item));

            if (isSelected) {
                EnsureChildrenSelected(item);
            }

            EditorGUI.BeginChangeCheck();
            var guiEnabled = GUI.enabled;
            GUI.enabled = guiEnabled && (item.parent == null || item.parent == rootItem || !selectedFolders.Contains(GetFullPath(item.parent)));
            bool isNowSelected = EditorGUI.Toggle(toggleRect, isSelected);
            GUI.enabled = guiEnabled;
            if (EditorGUI.EndChangeCheck()) {
                if (isNowSelected)
                    SelectItem(item);
                else
                    DeselectItem(item);
            }

            Rect labelRect = args.rowRect;
            labelRect.x += indent + 20;
            labelRect.width -= indent + 20;

            EditorGUI.LabelField(labelRect, item.displayName);
        }

#if UNITY_6000_2_OR_NEWER
        private void SelectItem(TreeViewItem<int> item) {
#else
        private void SelectItem(TreeViewItem item) {
#endif
            string fullPath = GetFullPath(item);
            if (!selectedFolders.Contains(fullPath)) {
                selectedFolders.Add(fullPath);
                if (item.hasChildren) {
                    foreach (var child in item.children) {
                        SelectItem(child);
                    }
                }
            }
        }

#if UNITY_6000_2_OR_NEWER
        private void EnsureChildrenSelected(TreeViewItem<int> item) {
#else
        private void EnsureChildrenSelected(TreeViewItem item) {
#endif
            if (item.hasChildren) {
                string fullFirstChildPath = GetFullPath(item.children[0]);
                if (!selectedFolders.Contains(fullFirstChildPath)) {
                    foreach (var child in item.children) {
                        SelectItem(child);
                    }
                }
            }
        }

#if UNITY_6000_2_OR_NEWER
        private void DeselectItem(TreeViewItem<int> item, bool recurse = true) {
#else
        private void DeselectItem(TreeViewItem item, bool recurse = true) {
#endif
            string fullPath = GetFullPath(item);
            if (selectedFolders.Contains(fullPath)) {
                selectedFolders.Remove(fullPath);
            }
        }

#if UNITY_6000_2_OR_NEWER
        private string GetFullPath(TreeViewItem<int> item) {
#else
        private string GetFullPath(TreeViewItem item) {
#endif
            if (item == null)
                return "";

            string fullPath = item.displayName;
            while (item.parent != null) {
                item = item.parent;
                if (item == rootItem) break;
                fullPath = Path.Combine(item.displayName, fullPath);
            }
            return fullPath.Replace('\\', '/');
        }

    }

}