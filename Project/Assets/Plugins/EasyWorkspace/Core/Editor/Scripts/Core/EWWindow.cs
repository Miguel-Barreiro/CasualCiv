using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyWorkspace
{
    public class EWWindow : EditorWindow
    {
        private static List<EWWindow> _windows = new();

        private bool _initialized;
        private bool _needToUpdate;

        public static EWWorkspaceView WorkspaceView;

        public static bool TrackingProjectChanges { get; private set; }

        private void Initialize()
        {
            if (!EWContainer.IsInitialized)
                return;

            if (_initialized)
                return;

            _initialized = true;

            EWWorkspaceSystem.Initialize();

            Undo.undoRedoPerformed += OnUndoRedo;
            SetTrackingProjectChanges(true);
        }

        private void OnEnable()
        {
            _windows.Add(this);

            Initialize();
        }

        private void OnDisable()
        {
            _windows.Remove(this);

            _initialized = false;

            Undo.undoRedoPerformed -= OnUndoRedo;
            SetTrackingProjectChanges(false);
        }

        public static void SetTrackingProjectChanges(bool value)
        {
            if (TrackingProjectChanges == value)
                return;

            TrackingProjectChanges = value;

            if (value)
                EditorApplication.projectChanged += OnProjectChanged;
            else
                EditorApplication.projectChanged -= OnProjectChanged;
        }

        [MenuItem("Window/Easy Workspace")]
        public static void OpenWindow()
        {
            EWWindow window = GetWindow<EWWindow>();
            window.titleContent = new GUIContent("Easy Workspace");
            window.minSize = new Vector2(400f, 300f);
        }

        private void Update()
        {
            WorkspaceView?.Update();
        }

        private void OnGUI()
        {
            WorkspaceView?.OnGUI();

            if (_needToUpdate)
            {
                _needToUpdate = false;
                UpdateWorkspaceView();
            }
            else if (Event.current.type == EventType.KeyDown)
            {
                KeyDown(Event.current.keyCode);
            }
        }

        private void OnUndoRedo()
        {
            _needToUpdate = true;
            AssetDatabase.SaveAssets();
        }

        private void ShowInitializationScreen()
        {
            if (EWWorkspaceSystem.GetClosedWorkspaces().Length == 0)
            {
                if (EWBackup.NeedRestore() && EWBackup.HasBackup())
                    EWBackup.TryRestore();
                else
                    EWWorkspaceSystem.CreateWorkspace();
                return;
            }

            EWContainer.Instance.InitializationScreenUXML.CloneTree(rootVisualElement);
        }

        private void ShowWorkspaceScreen(EWWorkspace workspace)
        {
            EWContainer.Instance.WorkspaceUXML.CloneTree(rootVisualElement);

            WorkspaceView = rootVisualElement.Q<EWWorkspaceView>();
            WorkspaceView.SetWindow(this);
            WorkspaceView.UpdateWorkspace(workspace);
        }

        private void ShowSettings()
        {
            EWContainer.Instance.SettingsUXML.CloneTree(rootVisualElement);
        }

        private void CreateGUI()
        {
            Initialize();

            UpdateWorkspaceView();
        }

        private static void OnProjectChanged()
        {
            if (!EWContainer.IsInitialized)
                return;

            EWWorkspaceSystem.UpdateWorkspaces();
            UpdateWorkspaceView();
        }

        public static void UpdateWorkspaceView()
        {
            foreach (EWWindow window in _windows)
            {
                window.rootVisualElement.Clear();
                int count = window.rootVisualElement.styleSheets.count;
                for (int i = 1; i < count; i++)
                    window.rootVisualElement.styleSheets.Remove(window.rootVisualElement.styleSheets[1]);

                if (EWSettings.SettingsIsOpen)
                {
                    window.ShowSettings();
                    return;
                }

                EWWorkspace workspace = EWWorkspaceSystem.GetCurrentWorkspace();
                if (workspace == null)
                    window.ShowInitializationScreen();
                else
                    window.ShowWorkspaceScreen(workspace);
            }
        }

        public static bool KeyDown(KeyCode keyCode)
        {
            if (keyCode == KeyCode.R)
            {
                UpdateWorkspaceView();
                return true;
            }

            if (keyCode == KeyCode.F)
            {
                WorkspaceView?.Graph?.Frame();
                return true;
            }

            if (keyCode >= KeyCode.Alpha1 && keyCode <= KeyCode.Alpha9 && !Event.current.control && !Event.current.alt && !Event.current.shift && !Event.current.command)
            {
                EWWorkspace[] workspaces = EWWorkspaceSystem.GetAddedWorkspaces();
                int index = keyCode - KeyCode.Alpha1;

                if (index < workspaces.Length)
                    EWWorkspaceSystem.OpenWorkspace(workspaces[index]);
            }

            return false;
        }

        public void FocusWindow()
        {
            Focus();
        }
    }
}