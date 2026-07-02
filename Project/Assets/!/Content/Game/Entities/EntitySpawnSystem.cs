#nullable enable
using System.Collections.Generic;
using Core.Model;
using Core.Model.Data;
using Core.Model.Stats;
using Core.View;
using Core.VSEngine;
using FixedPointy;
using Game.Entities.Board;
using Game.Entities.Enemies;
using Game.Entities.Spawners;
using Game.Input;
using Global;
using Scenes.Play;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
    public sealed class EntitySpawnSystem
    {
        [Inject] private readonly BasicCompContainer<UnitData> EnemyContainer = null!;
        [Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
        [Inject] private readonly DebugConfig DebugConfig = null!;

        [Inject] private readonly GameStatsContainer GameStatsContainer = null!;
        [Inject] private readonly StatsSystem StatsSystem = null!;
        [Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;

        [Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;
        [Inject] private readonly VSAbilitySystem VSAbilitySystem = null!;

        [Inject] private readonly BoardSystem BoardSystem = null!;





        public EntId SpawnPlayer(PlayerInputController playerInputController)
        {
            EntityConfig playerEntityConfig = DebugConfig.TestPlayerEntityConfig;
            PlayerEntity newPlayer = new PlayerEntity(playerInputController, playerEntityConfig);
            
            foreach ( StatOverride statOverride in playerEntityConfig.StatOverrides)
                StatsSystem.SetBaseValue(newPlayer.ID, statOverride.StatConfig, statOverride.Value);

            AddAbilities(newPlayer.ID, playerEntityConfig.Abilities);
            
            // EntityViewAtributes? entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(newPlayer.ID);
            // if (entityViewAtributes != null && entityViewAtributes.GameObject != null)
            // {
            //     GameplayViewConfig.CinemachineTargetGroup.RemoveMember(GameplayViewConfig.PlayerSpawnPoint.transform);
            //     GameplayViewConfig.CinemachineTargetGroup.AddMember(entityViewAtributes.GameObject.transform, 1, 
            //                                                         playerEntityConfig.PlayerCameraClearanceRadius);
            // }

            
            return newPlayer.ID;
        }
        
        public EntId SpawnUnitSpawner(SpawnerConfig  config, Vector2Int position, bool isEnemy)
        {
            Vector3 worldPosition = BoardSystem.GetWorldPosition(position);
            SpawnerEntity spawnerEntity = new SpawnerEntity(config, position, worldPosition, isEnemy);
            
            SetupGenericEntity(config, spawnerEntity.ID);
            
            BoardSystem.AddCollider(spawnerEntity.ID, position);
            
            return spawnerEntity.ID;
        }

        public EntId SpawnUnit(UnitConfig config, Vector2Int position, bool isEnemy )
        {
            Vector3 worldPosition = BoardSystem.GetWorldPosition(position);
            
            UnitEntity newUnit = new UnitEntity(config, worldPosition, isEnemy);
            
            SetupGenericEntity(config, newUnit.ID);
            
            return newUnit.ID;
        }

        private void SetupGenericEntity(EntityConfig config, EntId newEntityId)
        {
            foreach ( StatOverride statOverride in config.StatOverrides)
                StatsSystem.SetBaseValue(newEntityId, statOverride.StatConfig, statOverride.Value);

            AddAbilities(newEntityId, config.Abilities);
        }

        private void AddAbilities(EntId parent, VSAbilityDataConfig[] configAbilities)
        {
            foreach (VSAbilityDataConfig vsAbilityDataConfig in configAbilities)
            {
                VSAbility newAbility = new VSAbility(parent, vsAbilityDataConfig);
                
                foreach (StatOverride statOverride in vsAbilityDataConfig.StatsOverride)
                {
                    switch (statOverride.OverrideType)
                    {
                        case OverrideType.Additive:
                            Fix parentValue = StatsSystem.GetStatValue(parent, statOverride.StatConfig);
                            StatsSystem.SetBaseValue(newAbility.ID, statOverride.StatConfig, parentValue);
                            StatsSystem.AddModifier(newAbility.ID, newAbility.ID, statOverride.StatConfig, 
                                                    statOverride.Value, StatModifierType.Additive);
                            break;
                        case OverrideType.Override:
                            StatsSystem.SetBaseValue(newAbility.ID, statOverride.StatConfig, statOverride.Value);
                            break;
                        case OverrideType.Multiplicative:
                            parentValue = StatsSystem.GetStatValue(parent, statOverride.StatConfig);
                            StatsSystem.SetBaseValue(newAbility.ID, statOverride.StatConfig, parentValue);
                            StatsSystem.AddModifier(newAbility.ID, newAbility.ID, statOverride.StatConfig, 
                                                    statOverride.Value, StatModifierType.Multiplicative);
                            break;
                    } 
                }
            }
        }

        // public EntId SpawnEntity(EntityConfig config)
        // {
        //
        //     // ref BoardEntityComponentData boardEntity = ref WorldContainer.GetComponent(entity.ID);
        //     // boardEntity.ObjectType   = config.ObjectType;
        //     // boardEntity.Tile         = config.Tile;
        //     
        //     return EntId.Invalid;
        // }

        public EntId SpawnTile(CustomTile tile, GameObject go)
        {
            TileEntity tileEntity = new TileEntity(tile);
            return tileEntity.ID;
        }
    }
}
