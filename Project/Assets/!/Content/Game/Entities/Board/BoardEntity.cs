using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.View;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Entities.Board
{
	public sealed class BoardEntity : Entity, IBoard
	{
		public BoardEntity(BoardView boardView)
		{
			ref BoardData boardData = ref GetComponent<BoardData>();
			boardData.View = boardView;

			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes entityViewAtributes = viewEntitiesContainer.Spawn(boardView.gameObject, ID);
			
		}
	}


	[StructLayout(LayoutKind.Auto)]
	public struct BoardData : IComponentData
	{
		public EntId ID { get; set; }
		public BoardView View;
		public Dictionary<Vector2Int, EntId> Ground;
		public Dictionary<Vector2Int, EntId> Objects;

		public void Init()
		{
			Ground = new Dictionary<Vector2Int, EntId>();
			Objects = new Dictionary<Vector2Int, EntId>();
		}
	}

	public interface IBoard : Component<BoardData> { }
}