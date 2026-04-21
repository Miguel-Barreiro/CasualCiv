using Core.Events;
using Core.Model;
using FixedPointy;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	public sealed class MovePlayerEvent : EntityEvent<MovePlayerEvent>
	{
		public FixVec2 Direction = FixVec2.Zero;
		// public Fix MoveValue = Fix.Zero;

		[Inject] private readonly BasicCompContainer<PositionComponentData> PositionComponentContainer = null!;

		public override void Execute()
		{
			ref PositionComponentData positionComponentData = ref PositionComponentContainer.GetComponent(EntityID);
			positionComponentData.MoveDirection = Direction;
		}
	}
}