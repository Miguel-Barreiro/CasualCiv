
using Core.Editor;
using Core.Model;
using Core.Model.ModelSystems;
using Game;
using Game.Board;
using Game.Entities;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Tests
{
	
	public class BoardTests : UnitTest
	{
		[SerializeField] private EntityConfig testAirEntityConfig;
		public EntityConfig TestAirEntityConfig => testAirEntityConfig;

		[SerializeField] private EntityConfig testSurfaceEntityConfig;
		public EntityConfig TestSurfaceEntityConfig => testSurfaceEntityConfig;


		[SerializeField] private EntityConfig testFloorEntityConfig;
		public EntityConfig TestFloorEntityConfig => testFloorEntityConfig;

		
		[SerializeField] private EntityConfig testSurfaceGroundEntityConfig;
		public EntityConfig TestSurfaceGroundEntityConfig => testSurfaceGroundEntityConfig;
		
		
		[Inject] private readonly BoardSystem BoardSystem = null!;
		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;


		[Inject] private readonly BasicCompContainer<WorldEntityComponentData> WorldContainer;
		[Inject] private readonly BasicCompContainer<BoardComponentData> BoardContainer = null!;

		[Inject] private readonly BasicCompContainer<GameComponentData> GameContainer;

		private EntId _gameEntityID;
		
		protected override void InstallTestSystems(IUnitTestInstaller installer)
		{
			GameEntity gameEntity = new GameEntity();
			_gameEntityID = gameEntity.ID;
			
			installer.AddTestSystem(gameEntity);
			installer.AddTestSystem(new BoardSystem());
			installer.AddTestSystem(new EntitySpawnSystem());
		}

		protected override void ResetComponentContainers(DataContainersController dataController) { }


		[SetUp]
		public void Setup()
		{
			ExecuteFrame(0.016f);
			BoardSystem.Reset();
			ExecuteFrame(0.016f);
		}

		[Test]
		public void BoardSystem_Start_AllLayerDictionariesAreEmpty()
		{
			// ExecuteFrame(0.016f);
			
			ref GameComponentData gameComponentData = ref GameContainer.GetComponent(_gameEntityID);
			ref BoardComponentData board = ref BoardContainer.GetComponent(gameComponentData.BoardEntityID);

			Assert.IsNotNull(board.FloorEntities);
			Assert.IsNotNull(board.SurfaceEntities);
			Assert.IsNotNull(board.ObjectEntities);
			Assert.IsNotNull(board.AirEntities);
			Assert.AreEqual(0, board.FloorEntities.Count);
			Assert.AreEqual(0, board.SurfaceEntities.Count);
			Assert.AreEqual(0, board.ObjectEntities.Count);
			Assert.AreEqual(0, board.AirEntities.Count);
		}

		[Test]
		public void BoardSystem_AddEntity_CanBeRetrievedOnSameLayer()
		{
			// ExecuteFrame(0.016f);
			var pos = new Vector2Int(2, 3);
			var id = EntitySpawnSystem.SpawnEntity(testSurfaceEntityConfig);
			BoardSystem.AddEntity(id, pos);
			
			Assert.AreEqual(id,            BoardSystem.GetEntity(pos, WorldObjectType.Surface));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Floor));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Object));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Air));

			// BoardSystem.RemoveEntity(pos, testSurfaceEntityConfig.ObjectType);
			// EntitiesContainer.DestroyEntity(id);
		}

		[Test]
		public void BoardSystem_RemoveEntity_ReturnsInvalidAfterRemoval()
		{
			// ExecuteFrame(0.016f);
			var pos = new Vector2Int(0, 0);
			
			var id = EntitySpawnSystem.SpawnEntity(testFloorEntityConfig);
			BoardSystem.AddEntity(id, pos);
			BoardSystem.RemoveEntity(pos, testFloorEntityConfig.ObjectType);

			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Floor));

			// EntitiesContainer.DestroyEntity(id);
		}

		[Test]
		public void BoardSystem_SupportsMovingEntities()
		{
			// ExecuteFrame(0.016f);
			var pos = new Vector2Int(-5, -3);
			var position2 = new Vector2Int(0, 0);
			
			var id = EntitySpawnSystem.SpawnEntity(testAirEntityConfig);
			
			BoardSystem.AddEntity(id, pos);
			BoardSystem.AddEntity(id, position2);
			
			Assert.AreNotEqual(id, BoardSystem.GetEntity(pos, WorldObjectType.Air));
			Assert.AreEqual(id, BoardSystem.GetEntity(position2, WorldObjectType.Air));
		}
		
		[Test]
		public void BoardSystem_SupportsNegativeCoordinates()
		{
			// ExecuteFrame(0.016f);
			var pos = new Vector2Int(-5, -3);
			
			var id = EntitySpawnSystem.SpawnEntity(testAirEntityConfig);
			
			BoardSystem.AddEntity(id, pos);
			
			Assert.AreEqual(id, BoardSystem.GetEntity(pos, WorldObjectType.Air));
		}

	}
}