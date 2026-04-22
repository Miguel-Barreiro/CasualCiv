using Core.Events;
using Core.Model;
using Core.View;
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
		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;


		public override void Execute()
		{
			// EntityViewAtributes? entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(EntityID);
			// Rigidbody2D rigidbody2D = entityViewAtributes.Get<Rigidbody2D>();
			// rigidbody2D.AddForce(new Vector2(Direction.x.ToFloat(), Direction.y.ToFloat()) * 10f, ForceMode2D.Force);
			ref PositionComponentData positionComponentData = ref PositionComponentContainer.GetComponent(EntityID);
			positionComponentData.MoveDirection = Direction;
		}
	}
}