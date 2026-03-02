using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    public class FolderSelectionWindow : EditorWindow {

        [SerializeField] private AnyObjectFinderWindow m_parentWindow;

        private FolderTreeView treeView;
#if UNITY_6000_2_OR_NEWER
        private TreeViewState<int> treeViewState;
#else
        private TreeViewState treeViewState;
#endif

        public static FolderSelectionWindow CreateNewWindow() {
            var window = CreateInstance<FolderSelectionWindow>();
            window.titleContent.text = "Folders to Search";
            return window;
        }

        private void OnEnable() {
            AssemblyReloadEvents.beforeAssemblyReload += Close;
#if UNITY_6000_2_OR_NEWER
            treeViewState = treeViewState ?? new TreeViewState<int>();
#else
            treeViewState = treeViewState ?? new TreeViewState();
#endif
            treeView = new FolderTreeView(treeViewState);
            treeView.Reload();
        }

        public AnyObjectFinderWindow parentWindow {
            get => m_parentWindow;
            set {
                m_parentWindow = value;
                treeView = new FolderTreeView(treeViewState, m_parentWindow.GetSelectedFolders());
                treeView.Reload();
            }
        }

        private void OnGUI() {
            GUILayout.BeginArea(new Rect(10, 10, position.width - 20, position.height - 20));

            if (treeView != null) {
                if (GUILayout.Button("Select All in Assets")) {
                    treeView.SetSelectedFolders(treeView.GetAssetsFolders());
                }
                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Select All")) {
                        treeView.SetSelectedFolders(treeView.GetAllFolders());
                    }
                    if (GUILayout.Button("Select None")) {
                        treeView.SetSelectedFolders(new List<string>());
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(4f);
                var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
                treeView.OnGUI(rect);


                GUILayout.Space(4f);
                if (GUILayout.Button("Apply")) {
                    var selectedFolders = treeView.GetSelectedFolders();
                    if (parentWindow != null) {
                        parentWindow.SetSelectedFolders(selectedFolders);
                    }
                    Close();
                }
            }

            GUILayout.EndArea();
        }
    }

}