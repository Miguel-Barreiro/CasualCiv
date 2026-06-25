using Core.Events;
using Core.Model;
using FixedPointy;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	public sealed class AimPlayerEvent : EntityEvent<AimPlayerEvent>
	{
		public Vector2 AimPosition = Vector2.zero;

		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		
		public override void Execute()
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			playerData.AimPosition = AimPosition;
			
			
			
		}
	}
}