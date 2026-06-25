using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.View;
using UnityEngine;

namespace Game.Entities.Board
{
	public sealed class BoardEntity : Entity, IBoard
	{
		public BoardEntity(BoardView boardView)
		{
			ref BoardData boardData = ref GetComponent<BoardData>();

			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes entityViewAtributes = viewEntitiesContainer.Spawn(boardView.gameObject, ID);
			
		}
	}


	[StructLayout(LayoutKind.Auto)]
	public struct BoardData : IComponentData
	{
		public EntId ID { get; set; }
		public Dictionary<Vector2Int, EntId> Ground;
		public Dictionary<Vector2Int, EntId> Objects;

		public Dictionary<Vector2Int, EntId> Coliders;
		
		public void Init()
		{
			Ground = new Dictionary<Vector2Int, EntId>();
			Objects = new Dictionary<Vector2Int, EntId>();
			Coliders = new Dictionary<Vector2Int, EntId>();
		}
	}
	
	public interface IBoard : Component<BoardData> { }


	[StructLayout(LayoutKind.Auto)]
	public struct ColliderData : IComponentData
	{
		public EntId ID { get; set; }

		public BlockType BlockType;
		// public Vector2Int Position;

		public void Init()
		{
			BlockType = BlockType.EmptyBlock;
			// Position = InvalidPosition;
		}
	}

	public interface ICollider : Component<ColliderData> { }
}