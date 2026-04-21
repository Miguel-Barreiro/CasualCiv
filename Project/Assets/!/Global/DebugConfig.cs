using Core.Model.Data;
using Game.Entities;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Global
{
	public sealed class DebugConfig : DataConfig
	{
		[SerializeField] private TileBase testTile;
		public TileBase TestTile => testTile;

		[SerializeField] private EntityConfig testEntityConfig;
		public EntityConfig TestEntityConfig => testEntityConfig;
		
		
		[SerializeField] private EntityConfig testPlayerEntityConfig;
		public EntityConfig TestPlayerEntityConfig => testPlayerEntityConfig;
		
		
	}
}