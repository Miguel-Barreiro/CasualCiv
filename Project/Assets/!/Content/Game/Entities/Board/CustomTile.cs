using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Entities.Board
{
	
	[CreateAssetMenu(fileName = "CustomTile", menuName = "CustomTile")]
	public class CustomTile : TileBase
	{
		[SerializeField] private Sprite _Sprite;
		public Sprite Sprite => _Sprite;

		
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			tileData.sprite = _Sprite;
			tileData.transform = Matrix4x4.identity;
			tileData.flags = TileFlags.LockColor;
			tileData.colliderType = Tile.ColliderType.None;	
		}

		public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
		{
			Debug.Log($"new custom tile {position}");
			
			return true;
		}

	}
}