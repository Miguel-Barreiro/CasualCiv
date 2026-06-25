using System;
using System.Collections.Generic;
using Core.Initialization;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using Scenes.Play;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game.Entities.Board
{
	
	public interface ITileSystem
	{
		public enum TileType
		{
			Ground, Object
		}

		/// <returns>the previous entity in that position or EntId.invalid if it was nothing there</returns>
		public EntId AddTileEntity(EntId entity, Vector2Int position, CustomTile tile);
		public EntId GetTileEntityAt(Vector2Int position, TileType tileType);
	}

	public sealed class TileSystem : ITileSystem, IInitSystem, 
									OnDestroyComponent<TileComponentData>
	{
		
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly ObjectBuilder ObjectBuilder = null!;
		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;
		[Inject] private readonly BasicCompContainer<BoardData> BoardContainer = null!;
		[Inject] private readonly BasicCompContainer<TileComponentData> TileComponentContainer = null!;

		
		public void Initialize()
		{
			if(BoardContainer.Components[0].ID != EntId.Invalid)
				return;
			BoardEntity boardEntity = new BoardEntity(GameplayViewConfig.GridPrefab);
		}
		
		
		public EntId AddTileEntity(EntId entity, Vector2Int position, CustomTile tile)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			Dictionary<Vector2Int, EntId> layer = tile.Type switch
			{
				ITileSystem.TileType.Ground => boardData.Ground,
				ITileSystem.TileType.Object => boardData.Objects,
				_ => throw new ArgumentOutOfRangeException()
			};
			
			EntityViewAtributes entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(boardData.ID);
			if(entityViewAtributes == null || entityViewAtributes.GameObject == null)
				throw new Exception("board view not found");
			
			BoardView boardView = entityViewAtributes.Get<BoardView>();

			Tilemap tilemap = tile.Type switch
			{
				ITileSystem.TileType.Ground => boardView.GroundTilemap,	
				ITileSystem.TileType.Object => boardView.ObjectsTilemap,
				_ => throw new ArgumentOutOfRangeException()
			};
			
			bool hadPrevious = layer.TryGetValue(position, out EntId previousEntity);

			layer[position] = entity;
			tilemap.SetTile((Vector3Int) position, tile);

			if (hadPrevious)
				return previousEntity;
			
			return EntId.Invalid;
		}

		public EntId GetTileEntityAt(Vector2Int position, ITileSystem.TileType tileType)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			Dictionary<Vector2Int, EntId> layer = tileType switch
			{
				ITileSystem.TileType.Ground => boardData.Ground,
				ITileSystem.TileType.Object => boardData.Objects,
				_ => throw new ArgumentOutOfRangeException()
			};
			
			if (layer.TryGetValue(position, out EntId result))
				return result;

			return EntId.Invalid;
		}

		public void OnDestroyComponent(EntId destroyedComponentId)
		{
			ref TileComponentData tileComponentData = ref TileComponentContainer.GetComponent(destroyedComponentId);
			ref BoardData boardData = ref BoardContainer.Components[0];
			
			Dictionary<Vector2Int, EntId> layer = GetLayerByType(ref boardData, tileComponentData.TileType);
			foreach (KeyValuePair<Vector2Int, EntId> ground in layer)
			{
				if (ground.Value == destroyedComponentId)
				{
					layer.Remove(ground.Key);
					return;
				}
			}
		}

		private Dictionary<Vector2Int, EntId> GetLayerByType(ref BoardData boardData, ITileSystem.TileType tileType)
		{
			return tileType switch
			{
				ITileSystem.TileType.Ground => boardData.Ground,
				ITileSystem.TileType.Object => boardData.Objects
			};
		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}
}