using Core.Events;
using Events;
using Scenes.Play;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game.Board
{
	public class BoardViewSystem : IEventListener<OnTileChangedEvent>
	{
		
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;

		public void OnEvent(OnTileChangedEvent onEvent)
		{
		
			WorldObjectType topLayer = WorldObjectType.Floor;
			foreach (WorldObjectType layer in BoardSystem.ALL_LAYERS)
			{
				if ((onEvent.Layer & layer) == 0) continue;
				
				GetTilemap(layer)?.SetTile((Vector3Int) onEvent.Position, null);
				topLayer = layer;
			}
			
			// GetTilemap(layer)?.SetTile((Vector3Int) onEvent.Position, onEvent.Tile);
			GetTilemap(topLayer)?.SetTile((Vector3Int) onEvent.Position, onEvent.Tile);
		}
		
		private Tilemap GetTilemap(WorldObjectType layer) =>
			layer switch
			{
				WorldObjectType.Floor   => GameplayViewConfig.GroundTilemap,
				WorldObjectType.Surface => GameplayViewConfig.SurfaceTilemap,
				WorldObjectType.Object  => GameplayViewConfig.ObjectsTilemap,
				WorldObjectType.Air     => GameplayViewConfig.AirTilemap,
				_                       => null
			};
	}
}