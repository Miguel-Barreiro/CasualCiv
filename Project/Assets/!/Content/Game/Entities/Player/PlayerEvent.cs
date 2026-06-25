using Core.Events;
using Core.Model;
using Global;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	
	
	public sealed class SprintStartPlayerEvent : EntityEvent<SprintStartPlayerEvent>
	{
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		public override void Execute()
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			playerData.isSprinting = true;
			
		}
	}
	
	
	public sealed class SprintEndPlayerEvent : EntityEvent<SprintEndPlayerEvent>
	{
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		
		public override void Execute()
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			playerData.isSprinting = false;
		}
	}

	public sealed class DeploySpawnerEvent : EntityEvent<DeploySpawnerEvent>
	{
		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;
		[Inject] private readonly DebugConfig DebugConfig = null!;
		
		public Vector2Int SpawnPosition = Vector2Int.zero;
		
		public override void Execute()
		{
			EntitySpawnSystem.SpawnUnitSpawner(DebugConfig.TestSpawnerConfig, SpawnPosition, false);
		}
	}
	
}