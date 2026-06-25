using System;
using Core.View;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities.Enemies
{
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(NavMeshAgent))]
	// [RequireComponent(typeof(BehaviorGraphAgent))]
	public class EnemyView : EntityView
	{
		
		protected Animator Animator = null!;
		protected NavMeshAgent NavMeshAgent = null!;
		// protected BehaviorGraphAgent BehaviorGraphAgent = null!;
		
		private void Start()
		{
			Animator = GetComponent<Animator>();
			NavMeshAgent = GetComponent<NavMeshAgent>();
			// BehaviorGraphAgent = GetComponent<BehaviorGraphAgent>();
		}

		private void Update()
		{
			
			float desiredVelocitySqrMagnitude = NavMeshAgent.desiredVelocity.sqrMagnitude;
			if(!NavMeshAgent.isStopped && desiredVelocitySqrMagnitude > 0.01f)
			{
				if (desiredVelocitySqrMagnitude > AnimatorParam.RUNNING_MINIMUM_SPEED)
				{
					Animator.SetBool(AnimatorParam.ISRUNNING_ANIMATOR_PARAM, false);
					Animator.SetBool(AnimatorParam.ISMOVING_ANIMATOR_PARAM, true);
				} else
				{
					Animator.SetBool(AnimatorParam.ISRUNNING_ANIMATOR_PARAM, false);
					Animator.SetBool(AnimatorParam.ISMOVING_ANIMATOR_PARAM, true);
				}
			}
			else
			{
				Animator.SetBool(AnimatorParam.ISRUNNING_ANIMATOR_PARAM, false);
				Animator.SetBool(AnimatorParam.ISMOVING_ANIMATOR_PARAM, false);
			}
		}
	}
}