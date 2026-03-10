using Core.Model.Data;
using Game;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Entities
{
    [CreateAssetMenu(fileName = "New EntityConfig", menuName = "Game/Entities/EntityConfig")]
    public sealed class EntityConfig : DataConfig
    {
        [Title("World Settings")]
        [SerializeField, EnumToggleButtons]
        private WorldObjectType objectType = WorldObjectType.Object;
        public WorldObjectType ObjectType => objectType;

        [Title("Visual")]
        [SerializeField, PreviewField(80, ObjectFieldAlignment.Left)]
        private TileBase tile;
        public TileBase Tile => tile;

    }
}
