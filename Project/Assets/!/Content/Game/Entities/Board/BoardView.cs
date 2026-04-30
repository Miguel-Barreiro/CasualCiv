using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Entities.Board
{
	public sealed class BoardView : MonoBehaviour
	{
		[Space(10)]
		[SerializeField] private Tilemap groundTilemap;
		[SerializeField] private Tilemap surfaceTilemap;
		[SerializeField] private Tilemap objectsTilemap;
		[SerializeField] private Tilemap airTilemap;
		
		public Tilemap GroundTilemap => groundTilemap;
		public Tilemap SurfaceTilemap => surfaceTilemap;
		public Tilemap ObjectsTilemap => objectsTilemap;
		public Tilemap AirTilemap => airTilemap;

		
	}
}