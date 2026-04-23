using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.Utils;
using Core.View;
using DebugUtils;
using FixedPointy;
using Global;
using UnityEngine;
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
		[Inject] private readonly MovementConfig MovementConfig = null!;

		
		public void UpdateComponents(float deltaTime)
		{
			Vector2 speedV2 = new Vector2(1, 0);
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
				FixVec2 direction = componentData.MoveDirection.Normalize();
				Fix speed = StatsSystem.GetStatValue(entityId, GameStatsContainer.Speed);
				// componentData.Position += componentData.MoveDirection * speed * deltaTime;
				Rigidbody2D rigidbody2D = entityViewAtributes.Get<Rigidbody2D>();

				speedV2.x = (float) (direction.X * speed * 10);
				speedV2.y = (float) (direction.Y * speed* 10);
				
				rigidbody2D.AddForce(speedV2, ForceMode2D.Force);

				Fix magnitude = direction.GetMagnitude();
				if (magnitude < 0.1f)
					rigidbody2D.mass = MovementConfig.PlayerRestMass;
				else
					rigidbody2D.mass = MovementConfig.PlayerMovingMass;


				// rigidbody2D.linearVelocity = (componentData.MoveDirection * speed).ToVector2();
				// entityViewAtributes.GameObject.transform.position = componentData.Position.ToVector3();
			}

		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.InputGroup;
	}
}