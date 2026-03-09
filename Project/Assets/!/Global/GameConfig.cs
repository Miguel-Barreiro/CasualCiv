using Core.Model.Data;
using UnityEngine;

namespace Global
{
	public sealed class GameConfig : DataConfig
	{
		[SerializeField] private Vector2 _tileSize = Vector2.one;
		public Vector2 TileSize => _tileSize;
	}
}