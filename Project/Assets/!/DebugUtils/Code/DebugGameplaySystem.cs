using Core.Model;
using Core.Systems;
using Game.Entities;
using Game.Entities.Enemies;
using Global;
using Scenes.Play;
using UnityEngine;
using UnityEngine.Tilemaps;
using Zenject;

namespace DebugUtils
{
	public sealed class DebugGameplaySystem : IStartSystem
	{
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly DebugConfig DebugConfig = null!;

		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;



		public void StartSystem()
		{
			TestFillTiles();
			TestSpawnEntities();
		}
		
		private void TestFillTiles()
		{
			UnitConfig debugConfigTestUnitConfig = DebugConfig.TestUnitConfig;
			
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsLeft.ForEach(SpawnEnemy);
			
			// DebugConfig.SpawnEntitiesDebug.SpawnPositionsRight.ForEach(SpawnEnemy);


			void SpawnEnemy(Vector2Int tilePosition)
			{
				debugConfigTestUnitConfig = debugConfigTestUnitConfig == DebugConfig.TestUnitConfig? 
												DebugConfig.TestSpeedyUnitConfig : 
												DebugConfig.TestUnitConfig;
				EntitySpawnSystem.SpawnUnit(debugConfigTestUnitConfig, tilePosition, true);

			}

			// EntitySpawnSystem.SpawnEnemy(DebugConfig.TestEntityConfig);
			
			
			// TileBase
			// GameplayViewConfig.ObjectsTilemap.GetTile()
			
			// Vector3Int position = Vector3Int.zero;
			// TileBase testTile = DebugConfig.TestTile;
			// GameplayViewConfig.ObjectsTilemap.SetTile(position, testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(0, 0), testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(0, 1), testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 0), testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 1), testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(1, 2), testTile);
			// GameplayViewConfig.ObjectsTilemap.SetTile(position + new Vector3Int(2, 1), testTile);
		}

		private void TestSpawnEntities()
		{
			// Vector2Int position = new Vector2Int(-2, 0);
			// EntId newEntity = EntitySpawnSystem.SpawnEntity(DebugConfig.TestEntityConfig);
			// BoardSystem.AddEntity(newEntity, position);
		}

	}
}