using Core.Events;
using UnityEngine;

namespace Game.Entities
{
	public sealed class AimPlayerEvent : EntityEvent<AimPlayerEvent>
	{
		public Vector2 AimDirection;

		public override void Execute()
		{
			

		}
	}
}