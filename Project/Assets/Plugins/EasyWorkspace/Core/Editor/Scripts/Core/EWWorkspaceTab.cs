using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyWorkspace
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class EWWorkspaceTab : ToolbarButton
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<EWWorkspaceTab, UxmlTraits> { }
#endif

        private EWWorkspace _workspace;
        private EWWorkspaceView _workspaceView;

        private bool _initialized;
        private bool _active;
        private bool _needToRename;

        private Button _buttonClose;
        private Label _labelTitle;
        private TextField _textFieldRenaming;

        public EWWorkspace Workspace => _workspace;

        public EWWorkspaceTab()
        {
            RegisterCallback<GeometryChangedEvent>(InitializeLayout);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
        }

        private void InitializeLayout(GeometryChangedEvent evt)
        {
            if (_workspace == null)
                return;

            UnregisterCallback<GeometryChangedEvent>(InitializeLayout);

            _initialized = true;

            _buttonClose = this.Q<Button>("Close");
            _labelTitle = this.Q<Label>("Label");
            _textFieldRenaming = this.Q<TextField>("Renaming");

            _textFieldRenaming.RegisterValueChangedCallback(Rename);
            _textFieldRenaming.RegisterCallback<FocusOutEvent>(EndRenaming);

            UpdateTab();
            EndRenaming();

            if (_needToRename)
            {
                _needToRename = false;
                StartRenaming();
            }

            clickable.clicked += Open;
            _buttonClose.clickable.clicked += Close;
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            if (evt.button == 1)
            {
                evt.StopImmediatePropagation();
            
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Rename"), false, StartRenaming);
                menu.AddItem(new GUIContent("Close"), false, Close);
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Shift Left"), false, () => EWWorkspaceSystem.ShiftWorkspace(_workspace, false));
                menu.AddItem(new GUIContent("Shift Right"), false, () => EWWorkspaceSystem.ShiftWorkspace(_workspace, true));
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete"), false, Delete);

                menu.DropDown(worldBound);
            }
        }

        private void UpdateTab()
        {
            _labelTitle.text = _workspace.name;

            if (_active)
                AddToClassList("workspace-tab-active");
            else
                RemoveFromClassList("workspace-tab-active");
        }

        public void Initialize(EWWorkspaceView workspaceView, EWWorkspace workspace, bool active)
        {
            _workspaceView = workspaceView;
            _workspace = workspace;
            _active = active;

            if (_initialized)
                UpdateTab();
        }

        private void Open()
        {
            EWWorkspaceSystem.OpenWorkspace(_workspace);
        }

        private void Close()
        {
            EWWorkspaceSystem.RemoveWorkspace(_workspace);
        }

        private void Delete()
        {
            EWWorkspaceSystem.DeleteWorkspace(_workspace);
        }

        public void StartRenaming()
        {
            if (!_initialized)
            {
                _needToRename = true;
                return;
            }

            _labelTitle.style.display = DisplayStyle.None;
            _buttonClose.style.display = DisplayStyle.None;
            _textFieldRenaming.style.display = DisplayStyle.Flex;
            _textFieldRenaming.value = _workspace.name;
            _textFieldRenaming.Focus();

            _workspaceView.Window.FocusWindow();
        }

        private void EndRenaming()
        {
            _labelTitle.style.display = DisplayStyle.Flex;
            _buttonClose.style.display = DisplayStyle.Flex;
            _textFieldRenaming.style.display = DisplayStyle.None;
        }

        private void Rename(string newName)
        {
            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(_workspace), newName);
            UpdateTab();
            EndRenaming();
        }

        private void Rename(ChangeEvent<string> evt)
        {
            if (string.IsNullOrEmpty(evt.previousValue))
                return;

            Rename(evt.newValue);
        }

        private void EndRenaming(FocusOutEvent evt)
        {
            Rename(_textFieldRenaming.value);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {

        }
    }
}