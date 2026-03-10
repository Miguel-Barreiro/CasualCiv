using Core.Model;
using Game.Board;
using Zenject;

namespace Game.Entities
{
    public sealed class EntitySpawnSystem
    {
        [Inject] private readonly BasicCompContainer<WorldEntityComponentData> WorldContainer;
        [Inject] private readonly BoardSystem BoardSystem;
        
        public EntId SpawnEntity(EntityConfig config)
        {
            WorldEntity entity = new WorldEntity();

            ref WorldEntityComponentData worldEntity = ref WorldContainer.GetComponent(entity.ID);
            worldEntity.ObjectType   = config.ObjectType;
            worldEntity.Tile         = config.Tile;
            
            return entity.ID;
        }

    }
}
