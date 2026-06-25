using System;
using System.Collections.Generic;
using Core.Initialization;
using Core.Model;
using Game.Entities.Board;
using Game.Entities.Enemies;
using Global;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Zenject;
using Action = Unity.Behavior.Action;


namespace Game.Entities.Behaviours
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(name: "MoveToClosestAttackPosition", 
                        story: "[Agent] moves to closes attack position", 
                        category: "Action", 
                        id: "7c12f4ecbb6a7997e985168dfad7238c")]
    public partial class MoveToClosestAttackPositionAction : Action
    {
        [SerializeReference] public BlackboardVariable<GameObject>  Agent;

        [Inject] private readonly DebugConfig DebugConfig = null!;
        [Inject] private readonly BasicCompContainer<UnitData> EnemyContainer = null!;
        [Inject] private readonly BoardSystem BoardSystem = null!;

        

        EnemyView enemyView;
        NavMeshAgent agent;
        Vector2Int? targetPosition = null;
        
        protected override Status OnStart()
        {
            
            // GameObject gameObject = Agent.Value;
            //
            // enemyView = gameObject?.GetComponent<EnemyView>();
            // agent = gameObject?.GetComponent<NavMeshAgent>();
            // ObjectBuilder.GetInstance().Inject(this);
            //
            // ref EnemyData enemyData = ref EnemyContainer.GetComponent(enemyView.EntityID);
            //
            // List<Vector2Int> attackPositions = enemyData.isLeftSide ? 
            //                                        DebugConfig.SpawnEntitiesDebug.AttackPositionsLeft 
            //                                        : DebugConfig.SpawnEntitiesDebug.AttackPositionsRight;
            //
            // if (!GetAttackPosition(out Vector2Int newTargetPosition)) return Status.Failure;
            //
            // targetPosition = newTargetPosition;
            // BoardSystem.AddCollider( enemyView.EntityID, newTargetPosition);
            // agent.SetDestination(BoardSystem.GetWorldPosition(targetPosition.Value));
            //
            // return Status.Running;
            //
            // bool GetAttackPosition(out Vector2Int position)
            // {
            //     foreach (Vector2Int possiblePosition in attackPositions)
            //     {
            //         if (BoardSystem.GetColliderAt(possiblePosition, out EntId _) == BlockType.EmptyBlock)
            //         {
            //             position = possiblePosition;
            //             return true;
            //         }
            //     }
            //
            //     position = attackPositions[0];
            //     return true;
            // }
            return Status.Failure;
        }

        protected override Status OnUpdate()
        {
            if(targetPosition == null )
                return Status.Success;
            
            if(agent.pathPending)
                return Status.Running;
            
            if(!agent.hasPath)
                return Status.Success;

            Vector3 delta = agent.destination - agent.transform.position;

            if(delta.sqrMagnitude < agent.stoppingDistance * agent.stoppingDistance)
                return Status.Success;
            
            return Status.Running;
        }

        protected override void OnEnd()
        {
            targetPosition = null;
        }

        protected override void ResetStatus()
        {
            targetPosition = null;
            base.ResetStatus();
        }
    }
}

