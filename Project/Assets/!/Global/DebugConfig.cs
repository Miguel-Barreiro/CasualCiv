using System;
using System.Collections.Generic;
using Core.Model.Data;
using Game.Entities;
using Game.Entities.Enemies;
using Game.Entities.Spawners;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Global
{
	public sealed class DebugConfig : DataConfig
	{
		[SerializeField] private TileBase testTile;
		public TileBase TestTile => testTile;

		[SerializeField] private EntityConfig testEntityConfig;
		public EntityConfig TestEntityConfig => testEntityConfig;
		
		[SerializeField] private UnitConfig testEnemyConfig;
		public UnitConfig TestUnitConfig => testEnemyConfig;
		
		[SerializeField] private UnitConfig testSpeedyEnemyConfig;
		public UnitConfig TestSpeedyUnitConfig => testSpeedyEnemyConfig;
		
		
		[SerializeField] private EntityConfig testPlayerEntityConfig;
		public EntityConfig TestPlayerEntityConfig => testPlayerEntityConfig;


		[SerializeField] private SpawnEntitiesDebugConfig _SpawnEntitiesDebug;
		public SpawnEntitiesDebugConfig SpawnEntitiesDebug => _SpawnEntitiesDebug;

		
		[SerializeField] private SpawnerConfig _TestSpawnerConfig;
		public SpawnerConfig TestSpawnerConfig => _TestSpawnerConfig;

		

		[Serializable]
		public sealed class SpawnEntitiesDebugConfig
		{
			[SerializeField] public List<Vector2Int> SpawnPositionsLeft = new List<Vector2Int>();
			[SerializeField] public List<Vector2Int> SpawnPositionsRight = new List<Vector2Int>();
			
			[SerializeField] public List<Vector2Int> AttackPositionsLeft = new List<Vector2Int>();
			[SerializeField] public List<Vector2Int> AttackPositionsRight = new List<Vector2Int>();
			
		}

	}
}