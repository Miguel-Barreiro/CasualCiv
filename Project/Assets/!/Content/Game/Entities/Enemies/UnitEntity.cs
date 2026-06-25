using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using Game.Entities.Board;
using Global;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Vector3 = UnityEngine.Vector3;

namespace Game.Entities.Enemies
{
	public sealed class UnitEntity : Entity,
										IUnit,
										IDestructible, 
										IMovingBehaviour, 
										IWalkEntity,
										ICollider,
										IHierarchyEntity
	{
		public UnitEntity(UnitConfig config, Vector3 position, bool isEnemy)
		{
			
			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes? entityViewAtributes = viewEntitiesContainer.Spawn(config.Prefab, ID);
			if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
			{
				Debug.LogError($"Could not instantiate prefab {config.Prefab.name} for enemy entity ");
				return;
			}
			entityViewAtributes.GameObject.transform.position = position;

			ref ColliderData colliderData = ref GetComponent<ColliderData>();
			colliderData.BlockType = Board.BlockType.FullBlock;

			ref UnitData unitData = ref GetComponent<UnitData>();
			unitData.IsEnemy = isEnemy;
			
			NavMeshAgent navMeshAgent = entityViewAtributes.Get<NavMeshAgent>();
			ref MovingBehaviourData movingBehaviourData = ref GetComponent<MovingBehaviourData>();
			movingBehaviourData.NavMeshAgent = navMeshAgent;
			movingBehaviourData.isLeftSide = position.x < 0;

			if(!navMeshAgent.Warp(position))
				Debug.LogError($"could not warp the enemy to the position {position}");
		}
	}


	public sealed class EnemiesSystem : UpdateComponents<UnitData>
	{
		[Inject] private readonly BasicCompContainer<ColliderData> ColliderContainer = null!;
		[Inject] private readonly BasicCompContainer<UnitData> EnemyContainer = null!;
		[Inject] private readonly DebugConfig DebugConfig = null!;
		[Inject] private readonly BoardSystem BoardSystem = null!;



		public void UpdateComponents(float deltaTime)
		{
			uint topIndex = EnemyContainer.TopEmptyIndex;
			Vector3 target = Vector3.zero;
			for (int i = 0; i < topIndex; i++)
			{
				ref UnitData componentData = ref EnemyContainer.Components[i];
				// ref ColliderData colliderData = ref ColliderContainer.GetComponent(componentData.ID);
				// if (colliderData.Position != ColliderData.InvalidPosition)
				// 	continue;
				//
				//
				// List<Vector2Int> allPossibleAttackPositions;
				// allPossibleAttackPositions = componentData.isLeftSide ? 
				// 								DebugConfig.SpawnEntitiesDebug.AttackPositionsLeft 
				// 								: DebugConfig.SpawnEntitiesDebug.AttackPositionsRight;
				//
				// Vector2Int targetPosition = getFinalAttackPosition(allPossibleAttackPositions);
				// if(targetPosition == ColliderData.InvalidPosition)
				// 	continue;
				//
				// BoardSystem.AddCollider(componentData.ID, targetPosition);
				//
				// target = BoardSystem.GetWorldPosition(targetPosition);
				// componentData.Agent.SetDestination(target);
				// Vector2Int getFinalAttackPosition(List<Vector2Int> possiblePositions)
				// {
				// 	foreach (Vector2Int possiblePosition in possiblePositions)
				// 		if(BoardSystem.GetColliderAt(possiblePosition, out _ ) != BlockType.FullBlock)
				// 			return possiblePosition;
				// 	
				// 	return ColliderData.InvalidPosition;
				// }
			}

		}
		
		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}



	[StructLayout(LayoutKind.Auto)]
	public struct UnitData : IComponentData
	{
		public EntId ID { get; set; }

		public bool IsEnemy;
		
		public void Init() { }
	}
	

	public interface IUnit : IMovingBehaviour,
							IDestructible,
							Component<UnitData> { }
}