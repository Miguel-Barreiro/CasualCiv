using Core.Events;
using Game;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Events
{
	public sealed class OnTileChangedEvent : Event<OnTileChangedEvent>
	{
		public Vector2Int Position;
		public WorldObjectType Layer;
		public TileBase Tile;

		public override void Execute() { }
	}
}