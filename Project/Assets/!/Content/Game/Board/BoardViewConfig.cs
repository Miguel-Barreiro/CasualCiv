using Core.Model.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Board
{
	
	public sealed class BoardViewConfig : DataTable
	{
		[SerializeField] private TileBase water;
		public TileBase Water => water;
		
		[SerializeField] private TileBase bushes;
		public TileBase Bushes => bushes;

		[SerializeField] private TileBase tree;
		public TileBase Tree => tree;
		
	}
}