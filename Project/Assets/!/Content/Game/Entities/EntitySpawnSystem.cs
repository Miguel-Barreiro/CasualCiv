#nullable enable
using Core.Model;
using Core.Model.Stats;
using Core.View;
using Game.Entities.Board;
using Game.Input;
using Global;
using Scenes.Play;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
    public sealed class EntitySpawnSystem
    {
        [Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
        [Inject] private readonly DebugConfig DebugConfig = null!;

        [Inject] private readonly StatsSystem StatsSystem = null!;
        [Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;

        [Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;



        public EntId SpawnPlayer(PlayerInputController playerInputController)
        {
            EntityConfig playerEntityConfig = DebugConfig.TestPlayerEntityConfig;
            PlayerEntity newPlayer = new PlayerEntity(playerInputController, playerEntityConfig);

            
            foreach ( StatOverride statOverride in playerEntityConfig.StatOverrides)
                StatsSystem.SetBaseValue(newPlayer.ID, statOverride.StatConfig, statOverride.Value);

            
            // EntityViewAtributes? entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(newPlayer.ID);
            // if (entityViewAtributes != null && entityViewAtributes.GameObject != null)
            // {
            //     GameplayViewConfig.CinemachineTargetGroup.RemoveMember(GameplayViewConfig.PlayerSpawnPoint.transform);
            //     GameplayViewConfig.CinemachineTargetGroup.AddMember(entityViewAtributes.GameObject.transform, 1, 
            //                                                         playerEntityConfig.PlayerCameraClearanceRadius);
            // }

            
            return newPlayer.ID;
        }

        public EntId SpawnEntity(EntityConfig config)
        {

            // ref BoardEntityComponentData boardEntity = ref WorldContainer.GetComponent(entity.ID);
            // boardEntity.ObjectType   = config.ObjectType;
            // boardEntity.Tile         = config.Tile;
            
            return EntId.Invalid;
        }

        public EntId SpawnTile(CustomTile tile, Vector2Int Position, GameObject go)
        {
            TileEntity tileEntity = new TileEntity();
            return tileEntity.ID;
        }
    }
}
