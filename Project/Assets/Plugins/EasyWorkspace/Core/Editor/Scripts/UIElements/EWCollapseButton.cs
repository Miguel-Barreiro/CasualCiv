using UnityEngine.Events;
using UnityEngine.UIElements;

namespace EasyWorkspace
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class EWCollapseButton : VisualElement
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<EWCollapseButton, UxmlTraits> { }
#endif

        private bool _isCollapsed;

        public bool IsCollapsed
        {
            get => _isCollapsed;
            set
            {
                _isCollapsed = value;
                UpdateState();
        
                ValueChanged?.Invoke(_isCollapsed);
            }
        }

        public UnityAction<bool> ValueChanged;

        public EWCollapseButton()
        {
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            styleSheets.Add(EWContainer.Instance.CollapseButton);
        }

        private void OnMouseDown(MouseDownEvent evt)
        {
            IsCollapsed = !IsCollapsed;
        }

        private void UpdateState()
        {
            if (_isCollapsed)
                AddToClassList("collapsed");
            else
                RemoveFromClassList("collapsed");
        }
    }
}