using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace EasyWorkspace
{
#if UNITY_6000_0_OR_NEWER
    [UxmlElement]
#endif
    public partial class EWResizableElement : ResizableElement
    {
#if !UNITY_6000_0_OR_NEWER
        public new class UxmlFactory : UxmlFactory<EWResizableElement, UxmlTraits> { }
#endif
    }
}