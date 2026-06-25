using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.View;
using Game.Entities.Board;
using Game.Entities.Enemies;
using UnityEngine;

namespace Game.Entities.Spawners
{
	public sealed class SpawnerEntity : Entity,
										ICollider, 
										IDestructible, 
										ISpawner
	{
		public SpawnerEntity(SpawnerConfig config, Vector2Int position, Vector3 worldPosition, bool isEnemy)
		{
			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes? entityViewAtributes = viewEntitiesContainer.Spawn(config.Prefab, ID);
			if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
			{
				Debug.LogError($"Could not instantiate prefab {config.Prefab.name} for enemy entity ");
				return;
			}
			entityViewAtributes.GameObject.transform.position = worldPosition;

			ref ColliderData colliderData = ref GetComponent<ColliderData>();
			colliderData.BlockType = Board.BlockType.FullBlock;

			ref SpawnerData spawnerData = ref GetComponent<SpawnerData>();
			spawnerData.Config = config;
			spawnerData.SpawnPosition = position;
			spawnerData.IsEnemy = isEnemy;
		}
	}


	[StructLayout(LayoutKind.Auto)]
	public struct SpawnerData : IComponentData
	{
		public EntId ID { get; set; }
		
		public SpawnerConfig Config;
		public Vector2Int SpawnPosition;
		public bool IsEnemy;

		public void Init() { }
	}

	public interface ISpawner : Component<SpawnerData> { }
}