using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.Utils;
using Core.View;
using DebugUtils;
using FixedPointy;
using Zenject;

namespace Game.Entities
{

	[StructLayout(LayoutKind.Auto)]
	public struct PositionComponentData : IComponentData
	{
		public FixVec2 Position;
		public FixVec2 MoveDirection;
		
		public EntId ID { get; set; }

		public void Init()
		{
			MoveDirection = FixVec2.Zero;
			Position = FixVec2.Zero;
		}
	}

	public interface IPositionComponent : Component<PositionComponentData> { }


	public sealed class PositionEntitiesSystem : UpdateComponents<PositionComponentData>
	{
		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;
		[Inject] private readonly BasicCompContainer<PositionComponentData> PositionComponentContainer = null!;
		[Inject] private readonly StatsSystem StatsSystem = null!;
		[Inject] private readonly GameStatsContainer GameStatsContainer = null!;
		[Inject] private readonly DebugEnt DebugEnt = null!;
		
		public void UpdateComponents(float deltaTime)
		{
			uint topIndex = PositionComponentContainer.TopEmptyIndex;
			for (int i = 0; i < topIndex; i++)
			{
				ref PositionComponentData componentData = ref PositionComponentContainer.Components[i];
				EntId entityId = componentData.ID;
				EntityViewAtributes entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(entityId);

				if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
				{
					DebugEnt.LogError($"no gameobject set", entityId);
					continue;
				}
				
				Fix speed = StatsSystem.GetStatValue(entityId, GameStatsContainer.Speed);
				componentData.Position += componentData.MoveDirection * speed * deltaTime ;
				entityViewAtributes.GameObject.transform.position = componentData.Position.ToVector3();
			}

		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.InputGroup;
	}
}