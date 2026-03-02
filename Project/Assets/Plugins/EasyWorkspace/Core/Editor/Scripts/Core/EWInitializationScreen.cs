using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace EasyWorkspace
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class EWInitializationScreen : VisualElement
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<EWInitializationScreen, UxmlTraits> { }
#endif

        private Button _buttonCreateWorkspace;
        private Button _buttonOpenWorkspace;

        public EWInitializationScreen()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    
            _buttonCreateWorkspace = this.Q<Button>("button-create-workspace");
            _buttonOpenWorkspace = this.Q<Button>("button-open-workspace");

            if (_buttonCreateWorkspace == null) return;

            _buttonCreateWorkspace.clickable.clicked += CreateWorkspace;
    
            if (EWWorkspaceSystem.GetClosedWorkspaces().Length == 0)
                _buttonOpenWorkspace.SetEnabled(false);
    
            _buttonOpenWorkspace.clickable.clicked += Open;
        }

        private void CreateWorkspace()
        {
            EWWorkspaceSystem.CreateWorkspace();
        }

        private void Open()
        {
            GenericMenu menu = new GenericMenu();
    
            foreach (EWWorkspace workspace in EWWorkspaceSystem.GetClosedWorkspaces())
                menu.AddItem(new GUIContent(workspace.name), false, () => EWWorkspaceSystem.AddWorkspace(workspace));

            menu.DropDown(_buttonOpenWorkspace.worldBound);
        }
    }
}