using Core.Events;
using Core.Model;
using Core.View;
using FixedPointy;
using Game.Entities.Board;
using UnityEngine;
using Zenject;

namespace Game.Entities
{
	public sealed class MovePlayerEvent : EntityEvent<MovePlayerEvent>
	{
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		[Inject] private readonly BoardSystem BoardSystem = null!;

		
		public Vector2 Direction = Vector2.zero;
		// public Fix MoveValue = Fix.Zero;

		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;


		public override void Execute()
		{
			EntityViewAtributes? entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(EntityID);
			if(entityViewAtributes == null || entityViewAtributes.GameObject == null)
				return;

			Vector2 delta = Direction * 0.01f;
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);

			playerData.Position += delta;

			Vector3 newWorldPosition = BoardSystem.GetCellWorldPosition(playerData.Position);
			entityViewAtributes.GameObject.transform.position = newWorldPosition;
			
			// new Vector3(Direction.x, Direction.y, 0) );

			// Rigidbody2D rigidbody2D = entityViewAtributes.Get<Rigidbody2D>();
			// rigidbody2D.AddForce(new Vector2(Direction.x.ToFloat(), Direction.y.ToFloat()) * 10f, ForceMode2D.Force);
		}
	}
}