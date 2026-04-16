using Core.Events;
using UnityEngine;

namespace Events
{
	public sealed class OnTileClickedEvent : Event<OnTileClickedEvent>
	{
		public Vector2Int TilePosition;

		public override void Execute()
		{
			Debug.Log($"touched {TilePosition} tile");
		}
	}
}
