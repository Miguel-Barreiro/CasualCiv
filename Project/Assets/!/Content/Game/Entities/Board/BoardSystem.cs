using System;
using System.Collections.Generic;
using Core.Initialization;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.View;
using DebugUtils;
using Scenes.Play;
using UnityEngine;
using Zenject;

namespace Game.Entities.Board
{
	
	public interface IBoardSystem
	{
		/// <returns>the previous entity in that position or EntId.invalid if it was nothing there</returns>
		public EntId AddCollider(EntId entity, Vector2Int position);
		public BlockType GetColliderAt(Vector2Int position, out EntId colliderEntity);

		public Vector3 GetWorldPosition(Vector2Int position);

		public Vector2Int GetBoardPosition(Vector3 worldPosition);
	}
	
	public enum BlockType
	{
		FullBlock, 
		EmptyBlock,
	}
	
	public sealed class BoardSystem : IInitSystem, IBoardSystem, 
									OnDestroyComponent<ColliderData>
	{
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly ObjectBuilder ObjectBuilder = null!;
		[Inject] private readonly ViewEntitiesContainer ViewEntitiesContainer = null!;
		[Inject] private readonly BasicCompContainer<BoardData> BoardContainer = null!;
		[Inject] private readonly BasicCompContainer<ColliderData> ColliderContainer = null!;
		
		public void Initialize()
		{
			if(BoardContainer.Components[0].ID != EntId.Invalid)
				return;
			BoardEntity boardEntity = new BoardEntity(GameplayViewConfig.GridPrefab);
		}

		public void OnDestroyComponent(EntId destroyedComponentId)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			
			foreach (KeyValuePair<Vector2Int, EntId> colliderPositionPair in boardData.Coliders)
			{
				if (colliderPositionPair.Value == destroyedComponentId)
				{
					boardData.Coliders.Remove(colliderPositionPair.Key);
					return;
				}
			}
		}



		public EntId AddCollider(EntId entity, Vector2Int position)
		{
			ref ColliderData colliderData = ref ColliderContainer.GetComponent(entity);
			ref BoardData boardData = ref BoardContainer.Components[0];	
			
			DebugAssert.Assert(this, colliderData.ID != EntId.Invalid, "AddCollider called with non collider entity");
			
			EntId previousEntity;
			
			if (!boardData.Coliders.ContainsKey(position))
				previousEntity = EntId.Invalid;
			else
				previousEntity = boardData.Coliders[position];

			boardData.Coliders[position] = entity;
			// colliderData.Position = position;
			
			return previousEntity;
		}
		
		public BlockType GetColliderAt(Vector2Int position, out EntId colliderEntity)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			if (!boardData.Coliders.ContainsKey(position))
			{
				colliderEntity = EntId.Invalid;
				return BlockType.EmptyBlock;
			}

			EntId boardDataColiderId = boardData.Coliders[position];
			ref ColliderData colliderData = ref ColliderContainer.GetComponent(boardDataColiderId);
			colliderEntity = colliderData.ID;
			
			return colliderData.BlockType;

		}

		public Vector3 GetWorldPosition(Vector2Int position)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			EntityViewAtributes entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(boardData.ID);
			if(entityViewAtributes == null || entityViewAtributes.GameObject == null)
				throw new Exception("board view not found");
			
			BoardView boardView = entityViewAtributes.Get<BoardView>();
			return boardView.GroundTilemap.CellToWorld((Vector3Int) position) + (Vector3) boardView.GroundTilemap.tileAnchor;
		}

		public Vector2Int GetBoardPosition(Vector3 worldPosition)
		{
			ref BoardData boardData = ref BoardContainer.Components[0];
			EntityViewAtributes entityViewAtributes = ViewEntitiesContainer.GetEntityViewAtributes(boardData.ID);
			if(entityViewAtributes == null || entityViewAtributes.GameObject == null)
				throw new Exception("board view not found");
			
			BoardView boardView = entityViewAtributes.Get<BoardView>();
			Vector3Int cellPosition = boardView.GroundTilemap.WorldToCell(worldPosition);
			return (Vector2Int) cellPosition;
		}


		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.GameLogicGroup;
	}
	
}