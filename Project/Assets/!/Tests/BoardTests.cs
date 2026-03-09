using Core.Editor;
using Core.Model;
using Core.Model.ModelSystems;
using Game;
using Game.Board;
using NUnit.Framework;
using UnityEngine;
using Zenject;

namespace Tests
{
	public class BoardTests : UnitTest
	{
		[Inject] private readonly BoardSystem BoardSystem = null!;

		[Inject] private readonly BasicCompContainer<BoardComponentData> BoardContainer = null!;

		protected override void InstallTestSystems(IUnitTestInstaller installer)
		{
			installer.AddTestSystem(new GameEntity());
			installer.AddTestSystem(new BoardSystem());
		}

		protected override void ResetComponentContainers(DataContainersController dataController) { }


		[SetUp]
		public void Setup()
		{
			BoardSystem.Reset();
		}

		[Test]
		public void BoardSystem_Start_AllLayerDictionariesAreEmpty()
		{
			ExecuteFrame(0.016f);

			ref BoardComponentData board = ref BoardContainer.Components[0];

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
			ExecuteFrame(0.016f);
			var pos = new Vector2Int(2, 3);
			var id = new EntId(10);

			BoardSystem.AddEntity(pos, WorldObjectType.Surface, id);

			Assert.AreEqual(id,            BoardSystem.GetEntity(pos, WorldObjectType.Surface));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Floor));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Object));
			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Air));
		}

		[Test]
		public void BoardSystem_RemoveEntity_ReturnsInvalidAfterRemoval()
		{
			ExecuteFrame(0.016f);
			var pos = new Vector2Int(0, 0);
			var id = new EntId(5);

			BoardSystem.AddEntity(pos, WorldObjectType.Floor, id);
			BoardSystem.RemoveEntity(pos, WorldObjectType.Floor);

			Assert.AreEqual(EntId.Invalid, BoardSystem.GetEntity(pos, WorldObjectType.Floor));
		}

		[Test]
		public void BoardSystem_SupportsNegativeCoordinates()
		{
			ExecuteFrame(0.016f);
			var pos = new Vector2Int(-5, -3);
			var id = new EntId(99);

			BoardSystem.AddEntity(pos, WorldObjectType.Air, id);

			Assert.AreEqual(id, BoardSystem.GetEntity(pos, WorldObjectType.Air));
		}
	}
}
