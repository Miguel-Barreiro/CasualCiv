using Core.Events;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Model.Stats;
using Core.Systems;
using FixedPointy;
using Zenject;

namespace Game.Entities.Spawners
{
	public sealed class SpawnerSystem : UpdateComponents<SpawnerData>
	{
		[Inject] private readonly BasicCompContainer<SpawnerData> SpawnerContainer = null!;
		[Inject] private readonly StatsSystem StatsSystem = null!;
		[Inject] private readonly GameStatsContainer GameStatsContainer = null!;

		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;


		
		public void UpdateComponents(float deltaTime)
		{
			uint topIndex = SpawnerContainer.TopEmptyIndex;
			StatConfig cooldownStat = GameStatsContainer.Cooldown;
			for (int i = 0; i < topIndex; i++)
			{
				ref SpawnerData componentData = ref SpawnerContainer.Components[i];
				EntId entId = componentData.ID;
				
				Fix cooldown = StatsSystem.GetStatDepletedValue(entId, cooldownStat);
				cooldown -= (Fix) deltaTime;
				
				if (cooldown <= 0)
				{
					cooldown = StatsSystem.GetStatValue(entId, cooldownStat) + cooldown;
					EntityEventQueue.Execute<OnEntityActivate>(entId);
					EntitySpawnSystem.SpawnUnit(componentData.Config.UnitConfig, componentData.SpawnPosition, componentData.IsEnemy);
				}
				
				StatsSystem.SetDepletedValue(entId, cooldownStat, cooldown);
			}


		}
		
		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}
}