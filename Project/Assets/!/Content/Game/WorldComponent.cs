using System;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using Game.Board;
using Global;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game
{
    [Flags]
    public enum WorldObjectType : byte
    {
        Floor   = 1 << 0,
        Surface = 1 << 1,
        Object  = 1 << 2,
        Air     = 1 << 3,
    }

    public interface IWorldComponent : Component<WorldEntityComponentData> { }

    [StructLayout(LayoutKind.Auto)]
    public struct WorldEntityComponentData : IComponentData
    {

        public EntId ID { get; set; }
        public Vector2Int TilePosition;
        public WorldObjectType ObjectType;
        public TileBase Tile;

        public void Init()
        {
            TilePosition = BoardSystem.FLOATING_POSITION;
        }
        
        public bool IsFloating() => TilePosition == BoardSystem.FLOATING_POSITION;
    }
    

    /// Base system for any logic that manages world-visible entities.
    /// Subclasses implement OnCreateComponent and call CreateView(id, prefab) to spawn the view.
    public abstract class BaseWorldComponentLogic : OnDestroyComponent<WorldEntityComponentData>,
                                                    UpdateComponents<WorldEntityComponentData>
    {
        [Inject] protected readonly IViewEntitiesContainer ViewEntitiesContainer = null!;
        [Inject] protected readonly BasicCompContainer<WorldEntityComponentData> Container = null!;
        [Inject] private readonly GameConfig _gameConfig = null!;

        public bool Active { get; set; } = true;
        public SystemGroup Group { get; } = CoreSystemGroups.CoreViewEntitySystemGroup;

        protected static bool IsTypeOf(ref WorldEntityComponentData data, WorldObjectType type)
            => (data.ObjectType & type) != 0;

        protected void CreateView(EntId entityId, GameObject prefab)
        {
            ViewEntitiesContainer.Spawn(prefab, entityId);
        } 

        public void OnDestroyComponent(EntId destroyedComponentId)
        {
            ViewEntitiesContainer.Destroy(destroyedComponentId);
        }

        public void UpdateComponents(float deltaTime)
        {
            Vector2 tileSize = _gameConfig.TileSize;
            uint count = Container.Count;
            for (int i = 0; i < count; i++)
            {
                ref WorldEntityComponentData data = ref Container.Components[i];
                EntityViewAtributes? view = ViewEntitiesContainer.GetEntityViewAtributes(data.ID);
                if (view?.GameObject != null)
                    view.GameObject.transform.position = new Vector3(
                        data.TilePosition.x * tileSize.x,
                        data.TilePosition.y * tileSize.y,
                        0f);
            }
        }
    }
}
