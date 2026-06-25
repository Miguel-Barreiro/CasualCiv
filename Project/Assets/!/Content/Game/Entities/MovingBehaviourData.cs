using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.Utils;
using Game.Entities.Board;
using Global;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Entities
{

	public interface IMovingBehaviourSystem
	{
		public void SetCurrentState(EntId entId, MovingBehaviourState state);
		public MovingBehaviourState GetCurrentState(EntId entId);
	}

	public enum MovingBehaviourState
	{
		None = 0,
		Move = 1,
		Attack = 2,
	}

	[StructLayout(LayoutKind.Auto)]
	[ComponentData( ContainerType = typeof(BehaviourContainer)) ]
	public struct MovingBehaviourData : IComponentData
	{
		public const int NUMBER_STATES = 3; 

		public EntId ID { get; set; }
		
		
		public NavMeshAgent NavMeshAgent;
		public bool isLeftSide;
		

		public void Init() { }
	}

	public interface IMovingBehaviour : ICollider, Component<MovingBehaviourData> { }


	public sealed class BehaviourContainer : MultiArrayComponentContainer<MovingBehaviourData>
	{
		public BehaviourContainer(uint maxNumber) : base(maxNumber) { }
		protected override int ComponentArrayCount { get; } = MovingBehaviourData.NUMBER_STATES;
	}
	

	public sealed class MovingBehaviourSystem : IMovingBehaviourSystem, 
										OnCreateComponent<MovingBehaviourData>, 
										UpdateComponents<MovingBehaviourData>
	{
		[Inject] private readonly BehaviourContainer BehaviourContainer = null!;
		[Inject] private readonly BasicCompContainer<ColliderData> ColliderContainer = null!;
		[Inject] private readonly DebugConfig DebugConfig = null!;
		[Inject] private readonly BoardSystem BoardSystem = null!;
		[Inject] private readonly StatsSystem StatsSystem = null!;
		[Inject] private readonly GameStatsContainer GameStatsContainer = null!;
		
		public static readonly Vector2Int InvalidPosition = new Vector2Int(-1000, -1000);
		
		
		public void SetCurrentState(EntId entId, MovingBehaviourState state)
		{
			BehaviourContainer.SwitchToArray(entId, (uint) state.GetHashCode());
		}
		
		public MovingBehaviourState GetCurrentState(EntId entId)
		{
			return MovingBehaviourState.None;
		}

		public void OnCreateComponent(EntId newComponentId)
		{
			// ref BehaviourData data = ref GetComponent(newComponentId);
			BehaviourContainer.SwitchToArray(newComponentId, (uint) MovingBehaviourState.None.GetHashCode());
		}

		public static readonly Dictionary<EntId, uint> NewStatesUtil = new Dictionary<EntId, uint>();
		public void UpdateComponents(float deltaTime)
		{
			MovingBehaviourState newState = MovingBehaviourState.None;
			NewStatesUtil.Clear();
			
			
			//NONE
			ref PushBackArray<MovingBehaviourData> dataArray = ref BehaviourContainer.ComponentArrays[MovingBehaviourState.None.GetHashCode()];
			uint topIndex = dataArray.count;
			for (int i = 0; i < topIndex; i++)
			{
				ref MovingBehaviourData movingBehaviourData = ref dataArray.Items[i];
				newState = UpdateDefaultState(ref movingBehaviourData);
				NewStatesUtil.Add(movingBehaviourData.ID, (uint) newState.GetHashCode());
			}
			
			//MOVING
			dataArray = ref BehaviourContainer.ComponentArrays[MovingBehaviourState.Move.GetHashCode()];
			topIndex = dataArray.count;
			for (int i = 0; i < topIndex; i++)
			{
				ref MovingBehaviourData movingBehaviourData = ref dataArray.Items[i];
				newState = UpdateMoveState(ref movingBehaviourData);
				NewStatesUtil.Add(movingBehaviourData.ID, (uint) newState.GetHashCode());
			}
			
			
			//ATTACK
			dataArray = ref BehaviourContainer.ComponentArrays[MovingBehaviourState.Attack.GetHashCode()];
			topIndex = dataArray.count;
			for (int i = 0; i < topIndex; i++)
			{
				ref MovingBehaviourData movingBehaviourData = ref dataArray.Items[i];
				newState = UpdateAttackState(ref movingBehaviourData);
				NewStatesUtil.Add(movingBehaviourData.ID, (uint) newState.GetHashCode());
			}

			foreach ( (EntId entity, uint newIndexState) in NewStatesUtil)
				BehaviourContainer.SwitchToArray(entity, newIndexState);
			

		}

		private MovingBehaviourState UpdateAttackState(ref MovingBehaviourData movingBehaviourData)
		{
			// if the enemy is in attack range, attack, else move
			return MovingBehaviourState.None;
		}

		private MovingBehaviourState UpdateMoveState(ref MovingBehaviourData movingBehaviourData)
		{
			movingBehaviourData.NavMeshAgent.speed = (float)StatsSystem.GetStatValue(movingBehaviourData.ID, GameStatsContainer.Speed);

			// behaviourData.NavMeshAgent.destination
			return MovingBehaviourState.Move;
		}

		private MovingBehaviourState UpdateDefaultState(ref MovingBehaviourData movingBehaviourData)
		{
		
			ref ColliderData colliderData = ref ColliderContainer.GetComponent(movingBehaviourData.ID);
			// if (colliderData.Position != ColliderData.InvalidPosition)
			// 	return;
			
			
			List<Vector2Int> allPossibleAttackPositions;
			allPossibleAttackPositions = movingBehaviourData.isLeftSide ? 
											DebugConfig.SpawnEntitiesDebug.AttackPositionsLeft 
											: DebugConfig.SpawnEntitiesDebug.AttackPositionsRight;
			
			Vector2Int targetPosition = getFinalAttackPosition(allPossibleAttackPositions, movingBehaviourData.ID);
			if (targetPosition == InvalidPosition)
			{
				targetPosition = movingBehaviourData.isLeftSide ?
									DebugConfig.SpawnEntitiesDebug.AttackPositionsLeft[0] 
									: DebugConfig.SpawnEntitiesDebug.AttackPositionsRight[0];
			} else
			{
				BoardSystem.AddCollider(movingBehaviourData.ID, targetPosition);
			}

			Vector3 target = BoardSystem.GetWorldPosition(targetPosition);
			movingBehaviourData.NavMeshAgent.SetDestination(target);
		
			return MovingBehaviourState.Move;
			
			Vector2Int getFinalAttackPosition(List<Vector2Int> possiblePositions, EntId behaviourDataID)
			{
				foreach (Vector2Int possiblePosition in possiblePositions)
				{
					BlockType blockType = BoardSystem.GetColliderAt(possiblePosition, out EntId entityId );
					if(blockType != BlockType.FullBlock)
						return possiblePosition;
					if(entityId == behaviourDataID)
						return possiblePosition;
				}

				return InvalidPosition;
			}
		}


		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}
	
}