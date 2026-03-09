using System.Collections.Generic;
using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using UnityEngine;

namespace Game.Board
{
	public class BoardEntity : Entity, IBoardComponent
	{
	}

	public interface IBoardComponent : Component<BoardComponentData> { }

	[StructLayout(LayoutKind.Auto)]
	public struct BoardComponentData : IComponentData
	{
		public EntId ID { get; set; }
		public Dictionary<Vector2Int, EntId> FloorEntities;
		public Dictionary<Vector2Int, EntId> SurfaceEntities;
		public Dictionary<Vector2Int, EntId> ObjectEntities;
		public Dictionary<Vector2Int, EntId> AirEntities;

		public void Init() { }
	}
}