using System.Collections.Generic;
using Core.Model;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

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

		[Header("Debug")]
		[SerializeField] private bool _debugColliders;


		[Inject] private readonly BoardSystem BoardSystem = null!;

		
		private void OnDrawGizmos()
		{
			if (!_debugColliders || groundTilemap == null) return;

			Tilemap[] tilemaps = { groundTilemap, surfaceTilemap, objectsTilemap, airTilemap };
			HashSet<Vector2Int> visited = new();

			foreach (Tilemap tilemap in tilemaps)
			{
				if (tilemap == null) continue;
				BoundsInt bounds = tilemap.cellBounds;

				for (int x = bounds.xMin; x < bounds.xMax; x++)
				for (int y = bounds.yMin; y < bounds.yMax; y++)
				{
					Vector3Int cellPos = new(x, y, 0);
					if (!tilemap.HasTile(cellPos)) continue;

					Vector2Int pos = new(x, y);
					if (!visited.Add(pos)) continue;

					BlockType blockType = BoardSystem.GetColliderAt(pos, out EntId colliderEntity);

					Gizmos.color = blockType == BlockType.FullBlock
						? new Color(1f, 0f, 0f, 0.5f)
						: new Color(0f, 1f, 0f, 0.2f);

					Vector3 worldPos = tilemap.CellToWorld(cellPos) + (Vector3)tilemap.tileAnchor;
					Gizmos.DrawCube(worldPos, new Vector3(0.8f, 0.8f, 0f));
				}
			}
		}
	}
}
