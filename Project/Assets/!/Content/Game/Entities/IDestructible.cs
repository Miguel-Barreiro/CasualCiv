using System.ComponentModel;
using System.Runtime.InteropServices;
using Core.Events;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using FixedPointy;
using Zenject;

namespace Game.Entities
{
	[StructLayout(LayoutKind.Auto)]
	public struct DestructableData : IComponentData
	{
		public EntId ID { get; set; }
		
		public void Init() { }
	}

	public interface IDestructible : Component<DestructableData> { }
	
	
	
	[UpdateComponentProperties(Priority = SystemPriority.Late)]
	public sealed class DestructibleSystem : UpdateComponents<DestructableData>
	{
		[Inject] private readonly BasicCompContainer<DestructableData> DestructableContainer = null!;
		[Inject] private readonly StatsSystem StatsSystem = null!;
		[Inject] private readonly GameStatsContainer GameStatsContainer = null!;

		
		public void UpdateComponents(float deltaTime)
		{
			uint topIndex = DestructableContainer.TopEmptyIndex;
			for (int i = 0; i < topIndex; i++)
			{
				ref DestructableData componentData = ref DestructableContainer.Components[i];
				
				Fix currentHealth = StatsSystem.GetStatDepletedValue(componentData.ID, GameStatsContainer.Health);
				if(currentHealth <= 0)
					EntitiesContainer.DestroyEntity(componentData.ID);
			}		

		}
		
		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}
}