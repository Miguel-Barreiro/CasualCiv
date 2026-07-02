using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using DebugUtils;
using FixedPointy;
using Game.Input;
using Global;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	public sealed class PlayerEntity : Entity, 
										IPlayer
	{
		public PlayerEntity(PlayerInputController playerInputController, EntityConfig config)
		{
			ref PlayerData playerData = ref GetComponent<PlayerData>();
			playerData.PlayerInputController = playerInputController;
			playerInputController.SetEntity(ID);

			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes? entityViewAtributes = viewEntitiesContainer.Spawn(config.Prefab, ID);
			if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
				return;

			entityViewAtributes.GameObject.transform.position = Vector3.zero;
		}
		
	}

	[StructLayout(LayoutKind.Auto)]
	public struct PlayerData : IComponentData
	{
		public bool isSprinting;
		public Vector2 AimPosition;
		public PlayerInputController PlayerInputController;
		public Vector2 Position;

		public EntId ID { get; set; }


		public void Init()
		{
			AimPosition = Vector2.zero;
			Position = Vector2.zero;
		}
	}

	public interface IPlayer : Component<PlayerData> { }



	[UpdateComponentPropertiesAttribute(Priority = SystemPriority.Early)]
	public sealed class PlayerSystem : UpdateComponents<PlayerData>
	{
		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		[Inject] private readonly MovementConfig MovementConfig = null!;
		[Inject] private readonly StatsSystem StatsSystem = null!;
		[Inject] private readonly GameStatsContainer GameStatsContainer = null!;


		[Inject] private readonly DebugEnt DebugEnt = null!;
		
		public void UpdateComponents(float deltaTime)
		{
			uint topIndex = PlayerContainer.TopEmptyIndex;
			for (int i = 0; i < topIndex; i++)
			{
				ref PlayerData componentData = ref PlayerContainer.Components[i];
				EntId entityId = componentData.ID;

				EntityViewAtributes entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(entityId);

				if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
				{
					DebugEnt.LogError($"no gameobject set", entityId);
					continue;
				}

				// Rigidbody2D rigidbody2D = entityViewAtributes.Get<Rigidbody2D>();
				// Vector2 moveDirection = componentData.MoveDirection;
				//
				// Fix magnitude = moveDirection.magnitude;
				// if (magnitude < 0.1f)
				// 	rigidbody2D.mass = MovementConfig.PlayerRestMass;
				// else
				// 	rigidbody2D.mass = MovementConfig.PlayerMovingMass;

				if (componentData.isSprinting)
					StatsSystem.SetBaseValue(entityId, GameStatsContainer.Speed, MovementConfig.SprintSpeed);
				else
					StatsSystem.SetBaseValue(entityId, GameStatsContainer.Speed, MovementConfig.WalkSpeed);			}

		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}

}