using System;
using System.Collections.Generic;
using Core.Events;
using Core.Model;
using Core.Systems;
using Events;
using Game.Entities;
using UnityEngine;
using Zenject;

namespace Game.Board
{
	public sealed class BoardSystem : IInitSystem
	{
		
		[Inject] private readonly GameEntity GameEntity;
		[Inject] private readonly BasicCompContainer<GameComponentData> GameContainer;
		[Inject] private readonly BasicCompContainer<BoardComponentData> BoardContainer;
		[Inject] private readonly BasicCompContainer<WorldEntityComponentData> WorldContainer;

		[Inject] private readonly EventQueue EventQueue = null!;

		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;

		
		public static readonly Vector2Int FLOATING_POSITION = new Vector2Int(int.MaxValue, int.MaxValue);


		public static readonly WorldObjectType[] ALL_LAYERS =
		{
			WorldObjectType.Floor,
			WorldObjectType.Surface,
			WorldObjectType.Object,
			WorldObjectType.Air,
		};

		public void Initialize()
		{
			BoardEntity boardEntity = new BoardEntity();

			ref GameComponentData game = ref GameContainer.GetComponent(GameEntity.ID);
			game.BoardEntityID = boardEntity.ID;
		}
		
		public void AddEntity(EntId entityID, Vector2Int position)
		{
			ref WorldEntityComponentData worldEntity = ref WorldContainer.GetComponent(entityID);
			if(worldEntity.ID == EntId.Invalid) return;
			
			EntId entId = GetEntity(worldEntity.TilePosition, worldEntity.ObjectType);
			if (entId == entityID) 
				RemoveEntity(worldEntity.TilePosition, worldEntity.ObjectType);
			
			EntId entityId = worldEntity.ID;
			ref BoardComponentData board = ref GetBoard();

			foreach (WorldObjectType layer in ALL_LAYERS)
			{
				if ((worldEntity.ObjectType & layer) == 0) continue;
				GetDict(ref board, layer)[position] = entityId;
			}

			worldEntity.TilePosition = position;

			OnTileChangedEvent onTileChangedEvent = EventQueue.Execute<OnTileChangedEvent>();
			onTileChangedEvent.Position = position;
			onTileChangedEvent.Layer = worldEntity.ObjectType;
			onTileChangedEvent.Tile = worldEntity.Tile;
		}
		

		/// Returns the entity in the first matching layer.
		public EntId GetEntity(Vector2Int position, WorldObjectType layers)
		{
			ref BoardComponentData board = ref GetBoard();
			foreach (WorldObjectType layer in ALL_LAYERS)
			{
				if ((layers & layer) == 0) continue;
				if (GetDict(ref board, layer).TryGetValue(position, out EntId id))
					return id;
			}
			return EntId.Invalid;
		}

		public void RemoveEntity(Vector2Int position, WorldObjectType layers)
		{
			ref BoardComponentData board = ref GetBoard();
			foreach (WorldObjectType layer in ALL_LAYERS)
			{
				if ((layers & layer) != 0)
				{
					GetDict(ref board, layer).Remove(position);

					OnTileChangedEvent onTileChangedEvent = EventQueue.Execute<OnTileChangedEvent>();
					onTileChangedEvent.Position = position;
					onTileChangedEvent.Layer = layer;
					onTileChangedEvent.Tile = null;
				}

			}
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
		
		

		private static Dictionary<Vector2Int, EntId> GetDict(ref BoardComponentData board, WorldObjectType layer) =>
			layer switch
			{
				WorldObjectType.Floor   => board.FloorEntities,
				WorldObjectType.Surface => board.SurfaceEntities,
				WorldObjectType.Object  => board.ObjectEntities,
				WorldObjectType.Air     => board.AirEntities,
				_ => throw new ArgumentOutOfRangeException(nameof(layer))
			};

	}
}
