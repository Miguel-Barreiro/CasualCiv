using Game.Entities;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace Editor.Entities
{
    [CustomEditor(typeof(EntityConfig))]
    public sealed class EntityConfigEditor : OdinEditor
    {
        // OdinEditor renders all Odin attributes ([EnumToggleButtons], [PreviewField], [Title])
        // declared on EntityConfig's fields automatically. No override needed.
    }
}
