using Core.Model;
using Game.Input;
using Global;
using Zenject;

namespace Game.Entities
{
    public sealed class EntitySpawnSystem
    {
        [Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
        [Inject] private readonly DebugConfig DebugConfig = null!;


        public EntId SpawnPlayer(PlayerInputController playerInputController)
        {
            PlayerEntity newPlayer = new PlayerEntity(playerInputController, DebugConfig.TestPlayerEntityConfig);

            return newPlayer.ID;
        }

        public EntId SpawnEntity(EntityConfig config)
        {

            // ref BoardEntityComponentData boardEntity = ref WorldContainer.GetComponent(entity.ID);
            // boardEntity.ObjectType   = config.ObjectType;
            // boardEntity.Tile         = config.Tile;
            
            return EntId.Invalid;
        }

    }
}
