using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Initialization;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using Scenes.Play;
using UnityEngine;
using Zenject;

namespace Game.Entities.Board
{
	
	public interface IBoardSystem
	{
		
		public enum TileType
		{
			Ground, Object
		}

		/// <returns>the previous entity in that position or EntId.invalid if it was nothing there</returns>
		public EntId AddEntity(EntId entity, Vector2Int position, CustomTile tile);
		public EntId GetEntityAt(Vector2Int position,  TileType tileType);
	}
	
	
	
	public sealed class BoardSystem : IStartSystem, IBoardSystem
	{
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly ObjectBuilder ObjectBuilder = null!;

		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;

		[Inject] private readonly BasicCompContainer<BoardData> BoardContainer = null!;
		

		// public void AddObjectEntity(EntId entity, Vector2Int position, CustomTile tile)
		// {
		// 	ref BoardData boardData = ref BoardContainer.Components[0];
		// 	boardData.View.ObjectsTilemap.SetTile((Vector3Int) position, tile);
		// 	boardData.Objects[position] = entity;
		// }
		//
		// public void AddGroundEntity(EntId entity, Vector2Int position, CustomTile tile)
		// {
		// 	ref BoardData boardData = ref BoardContainer.Components[0];
		// 	boardData.View.GroundTilemap.SetTile((Vector3Int) position, tile);
		// 	boardData.Ground[position] = entity;
		// }
		//
		// public EntId GetGroundEntityAt(Vector2Int position)
		// {
		// 	ref BoardData boardData = ref BoardContainer.Components[0];
		// 	if (boardData.Ground.TryGetValue(position, out EntId result))
		// 		return result;
		//
		// 	return EntId.Invalid;
		// }
		//
		// public EntId GetObjectEntityAt(Vector2Int position)
		// {
		// 	ref BoardData boardData = ref BoardContainer.Components[0];
		// 	if (boardData.Objects.TryGetValue(position, out EntId result))
		// 		return result;
		//
		// 	return EntId.Invalid;
		// }
		//
		
		public void StartSystem()
		{
			BoardEntity boardEntity = new BoardEntity(GameplayViewConfig.GridPrefab);
		}


		public EntId AddEntity(EntId entity, Vector2Int position, CustomTile tile)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			Dictionary<Vector2Int, EntId> layer = tile.Type switch
			{
				IBoardSystem.TileType.Ground => boardData.Ground,
				IBoardSystem.TileType.Object => boardData.Objects,
				_ => throw new ArgumentOutOfRangeException()
			};

			bool hadPrevious = layer.TryGetValue(position, out EntId previousEntity);
			layer[position] = entity;

			if (hadPrevious)
				return previousEntity;

			return EntId.Invalid;
		}
		
		public EntId GetEntityAt(Vector2Int position, IBoardSystem.TileType tileType)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			Dictionary<Vector2Int, EntId> layer = tileType switch
			{
				IBoardSystem.TileType.Ground => boardData.Ground,
				IBoardSystem.TileType.Object => boardData.Objects,
				_ => throw new ArgumentOutOfRangeException()
			};
			
			if (layer.TryGetValue(position, out EntId result))
				return result;

			return EntId.Invalid;
		}
	}


	[StructLayout(LayoutKind.Auto)]
	public struct BoardPieceData : IComponentData
	{
		public EntId ID { get; set; }
		

		public void Init() { }
	}

	public interface IBoardPiece : Component<BoardPieceData> { }
}