using System.Runtime.InteropServices;
using Core.Events;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Zenject.Source.Internal;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game.Entities.Board
{
	
	[CreateAssetMenu(fileName = "CustomTile", menuName = "CustomTile")]
	public class CustomTile : TileBase
	{

		[SerializeField] private Sprite _Sprite;
		[SerializeField] private BlockType _BlockType;
		[SerializeField] private IBoardSystem.TileType _TileType = IBoardSystem.TileType.Ground;

		public Sprite Sprite => _Sprite;
		public BlockType BlockType => _BlockType;
		public IBoardSystem.TileType Type => _TileType;

		
		public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
		{
			tileData.sprite = _Sprite;
			tileData.transform = Matrix4x4.identity;
			tileData.flags = TileFlags.LockColor;
			tileData.colliderType = Tile.ColliderType.None;	
		}

		public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
		{

#if UNITY_EDITOR
			if (!Application.isPlaying)
			{
				return true;
			}
#endif			

			SetupTileEvent setupTileEvent = EventQueue.Trigger<SetupTileEvent>();
			setupTileEvent.tilemap = tilemap;
			setupTileEvent.go = go;
			setupTileEvent.Position = new Vector2Int(position.x, position.y);
			setupTileEvent.Tile = this;

			return true;
		}

	}

	public sealed class TileEntity : Entity
	{
		
	}

	[StructLayout(LayoutKind.Auto)]
	public struct TileComponentData : IComponentData
	{
		public EntId ID { get; set; }

		public BlockType BlockType;
		public IBoardSystem.TileType TileType;

		public void Init()
		{
			BlockType = BlockType.FullBlock;
			TileType = IBoardSystem.TileType.Ground;
		}
	}

	public interface ITile : Component<TileComponentData> { }

	public sealed class SetupTileEvent : Event<SetupTileEvent>
	{
		[Inject] private readonly BoardSystem BoardSystem = null!;
		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;
		// [Inject] private readonly EntitiesContainer EntitiesContainer = null!;

		
		public Vector2Int Position;
		public CustomTile Tile;
		public ITilemap tilemap;
		public GameObject go;
		
		public override void Execute()
		{
			EntId newTile = EntitySpawnSystem.SpawnTile(Tile, Position, go);
			EntId previousEnt = BoardSystem.AddEntity(newTile, Position, Tile);

			if (previousEnt != EntId.Invalid)
			{
				Log($"Replacing tile ({previousEnt}) -> {Tile.name} ({newTile})");
				EntitiesContainer.DestroyEntity(previousEnt);
			}
			// Debug.Log($"new custom tile {Position} {Tile.Sprite.name} {go}");			
		}
	}


	public enum BlockType
	{
		FullBlock, 
		EmptyBlock,
	}
}