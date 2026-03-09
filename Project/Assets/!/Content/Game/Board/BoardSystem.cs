using System;
using System.Collections.Generic;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Global;
using Scenes.Play;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace Game.Board
{
	public sealed class BoardSystem : IInitSystem
	{

		[Inject] private readonly GameEntity GameEntity;
		[Inject] private readonly BasicCompContainer<GameComponentData> GameContainer;
		[Inject] private readonly BasicCompContainer<BoardComponentData> BoardContainer;


		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;


		public void Initialize()
		{
			BoardEntity boardEntity = new BoardEntity();

			ref GameComponentData game = ref GameContainer.GetComponent(GameEntity.ID);
			game.BoardEntityID = boardEntity.ID;

			ref BoardComponentData board = ref BoardContainer.GetComponent(boardEntity.ID);
			board.FloorEntities   = new Dictionary<Vector2Int, EntId>();
			board.SurfaceEntities = new Dictionary<Vector2Int, EntId>();
			board.ObjectEntities  = new Dictionary<Vector2Int, EntId>();
			board.AirEntities     = new Dictionary<Vector2Int, EntId>();


			Vector3Int position = Vector3Int.zero;
			TileBase testTile = GameplayViewConfig.BoardViewConfig.TestTile;
			GameplayViewConfig.ObjectsTilemap.SetTile(position, testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(0, 0), testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(0, 1), testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 0), testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 1), testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 2), testTile);
			GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(2, 1), testTile);
		}

		public void AddEntity(Vector2Int position, WorldObjectType layer, EntId entityId)
		{
			ref BoardComponentData board = ref GetBoard();
			GetDict(ref board, layer)[position] = entityId;
		}

		public EntId GetEntity(Vector2Int position, WorldObjectType layer)
		{
			ref BoardComponentData board = ref GetBoard();
			return GetDict(ref board, layer).TryGetValue(position, out EntId id) ? id : EntId.Invalid;
		}

		public void RemoveEntity(Vector2Int position, WorldObjectType layer)
		{
			ref BoardComponentData board = ref GetBoard();
			GetDict(ref board, layer).Remove(position);
		}

		private ref BoardComponentData GetBoard()
		{
			ref GameComponentData game = ref GameContainer.GetComponent(GameEntity.ID);
			return ref BoardContainer.GetComponent(game.BoardEntityID);
		}

		public void Reset()
		{
			ref BoardComponentData boardComponentData = ref GetBoard();

			foreach (var airEntityID in boardComponentData.AirEntities.Values)
				EntitiesContainer.DestroyEntity(airEntityID);

			foreach (var surfaceEntityID in boardComponentData.SurfaceEntities.Values)
				EntitiesContainer.DestroyEntity(surfaceEntityID);

			foreach (var objectEntityID in boardComponentData.ObjectEntities.Values)
				EntitiesContainer.DestroyEntity(objectEntityID);

			foreach (var floorEntityID in boardComponentData.FloorEntities.Values)
				EntitiesContainer.DestroyEntity(floorEntityID);

			boardComponentData.AirEntities.Clear();
			boardComponentData.SurfaceEntities.Clear();
			boardComponentData.ObjectEntities.Clear();
			boardComponentData.FloorEntities.Clear();
		}

		private static Dictionary<Vector2Int, EntId> GetDict(ref BoardComponentData board, WorldObjectType layer)
			=> layer switch
			{
				WorldObjectType.Floor   => board.FloorEntities,
				WorldObjectType.Surface => board.SurfaceEntities,
				WorldObjectType.Object  => board.ObjectEntities,
				WorldObjectType.Air     => board.AirEntities,
				_ => throw new ArgumentOutOfRangeException(nameof(layer))
			};

	}
}
