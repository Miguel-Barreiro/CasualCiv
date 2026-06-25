using System;
using Core.Model;
using Core.Utils;
using Core.View;
using Global;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	public sealed class PlayerView : EntityView
	{
		[SerializeField] private Transform _AimTarget;
		public Transform AimTarget => _AimTarget;

		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		[Inject] private readonly InputConfig InputConfig = null!;

		
		private void Update()
		{
			if(EntityID == EntId.Invalid)
				return;
			
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			Vector3 delta = playerData.AimPosition;
			float clampedValue = Mathf.Clamp(delta.magnitude * InputConfig.TargetMoveSpeed, InputConfig.TargetMaxDistance,  InputConfig.TargetMinDistance);
			
			AimTarget.transform.localPosition = delta.normalized * clampedValue;
			
		}
	}
}