using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Events;
using Core.Model;
using Core.Model.ModelSystems;
using DebugUtils;
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
		[SerializeField] private ITileSystem.TileType _TileType = ITileSystem.TileType.Ground;

		public Sprite Sprite => _Sprite;
		public BlockType BlockType => _BlockType;
		public ITileSystem.TileType Type => _TileType;

		
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
			
			// Dictionary<Vector3Int, CustomTile> dictionary = _TileType == ITileSystem.TileType.Ground 
			// 													? TileEntitiesGround 
			// 													: TileEntitiesObjects;
			// // Debug.Log($"TILE {number++} { position}");
			// if(dictionary.ContainsKey(position))
			// {
			// 	Debug.Log($"Tile already exists at {position} <{_TileType}, {Sprite.name}> == <{dictionary[position]._TileType}, {dictionary[position]._Sprite.name}>");
			// }
			// else
			// {
			// 	dictionary.Add(position, this);
			// }
			
			return true;
		}


		// public static int number = 0;
		// public static Dictionary<Vector3Int, CustomTile> TileEntitiesObjects = new Dictionary<Vector3Int, CustomTile>();
		// public static Dictionary<Vector3Int, CustomTile> TileEntitiesGround = new Dictionary<Vector3Int, CustomTile>();
	}

	public sealed class TileEntity : Entity, ICollider
	{
		public TileEntity(CustomTile tile)
		{
			ref TileComponentData componentData = ref GetComponent<TileComponentData>();
			componentData.TileType = tile.Type;
			ref ColliderData colliderData = ref GetComponent<ColliderData>();
			colliderData.BlockType = tile.BlockType;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	public struct TileComponentData : IComponentData
	{
		public EntId ID { get; set; }

		public ITileSystem.TileType TileType;

		public void Init()
		{
			TileType = ITileSystem.TileType.Ground;
		}
	}

	public interface ITile : Component<TileComponentData> { }

	public sealed class SetupTileEvent : Event<SetupTileEvent>
	{
		[Inject] private readonly TileSystem TileSystem = null!;
		[Inject] private readonly BoardSystem BoardSystem = null!;
		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;
		// [Inject] private readonly EntitiesContainer EntitiesContainer = null!;

		
		public Vector2Int Position;
		public CustomTile Tile;
		public ITilemap tilemap;
		public GameObject go;
		
		public override void Execute()
		{
			EntId newTile = EntitySpawnSystem.SpawnTile(Tile, go);
			EntId previousEnt = TileSystem.AddTileEntity(newTile, Position, Tile);
			
			// Debug.Log($"Custom tile <{Position}> ");			
			
			if (previousEnt != EntId.Invalid)
			{
				// Log($"Replacing tile ({previousEnt}) -> {Tile.name} ({newTile})");
				EntitiesContainer.DestroyEntity(previousEnt);
			}
			
			if(Tile.BlockType == BlockType.EmptyBlock)
				return;
			
			previousEnt = BoardSystem.AddCollider(newTile, Position);
			
			if (previousEnt != EntId.Invalid)
			{
				// Log($"Replacing collider ({previousEnt}) -> {Tile.name} ({newTile})");
				EntitiesContainer.DestroyEntity(previousEnt);
			}
		}
	}


}