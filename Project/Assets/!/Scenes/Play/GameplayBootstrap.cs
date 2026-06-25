using System;
using Core.Initialization;
using Core.Model.ModelSystems;
using Core.Systems;
using Core.Zenject.Source.Main;
using DebugUtils;
using Game;
using Game.Entities;
using Game.Entities.Board;
using Game.Entities.Enemies;
using Game.Entities.Spawners;
using Game.Entities.Spawnpoints;
using Game.Input;
using Game.UI;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Scenes.Play
{

	[Serializable]
	public sealed class GameplayViewConfig
	{

		[SerializeField] private GameUIConfig gameUIConfig;
		public GameUIConfig GameUIConfig => gameUIConfig;

		
		[Space(10)]
		[SerializeField] private Camera _mainCamera;
		[SerializeField] private CinemachineTargetGroup _CinemachineTargetGroup;
		[SerializeField] private CinemachineCamera _cinemachineCamera;
		[SerializeField] private PlayerInputManager _PlayerInputManagerPrefab;

		[SerializeField] private SpawnpointView _PlayerSpawnPoint;
		
		[SerializeField] private BoardView _GridPrefab;
		
		public SpawnpointView PlayerSpawnPoint => _PlayerSpawnPoint;
		public CinemachineCamera CinemachineCamera => _cinemachineCamera;
		public Camera MainCamera => _mainCamera;
		public CinemachineTargetGroup CinemachineTargetGroup => _CinemachineTargetGroup;
		public PlayerInputManager PlayerInputManagerPrefab => _PlayerInputManagerPrefab;
		
		public BoardView GridPrefab => _GridPrefab;
	}

	public sealed class GameplayBootstrap : SceneBootstrap
	{
		[SerializeField] private GameplayViewConfig GameplayViewConfig;
		
		private GameplayInstaller _installer;

		public override SystemsInstallerBase GetLogicInstaller()
		{
			if(_installer == null)
				_installer = new GameplayInstaller(Container, GameplayViewConfig);

			return _installer;
		}
	}
	
	public sealed class GameplayInstaller : SystemsInstallerBase
	{
		private readonly GameplayViewConfig GameplayViewConfig;

		public GameplayInstaller(DiContainer container, GameplayViewConfig gameplayViewConfig) : base(container)
		{
			GameplayViewConfig = gameplayViewConfig;
		}

		public override void SetupConfigurations()
		{
			BindInstance(GameplayViewConfig);
			BindInstance(GameplayViewConfig.GameUIConfig);
			BindInstance(new DefaultInputActions());
		}
		
#if DEBUG
		protected override void AddDebugOptions() { }
#endif    


		protected override void InstallSystems()
		{
			
			GameEntity gameEntity = new GameEntity();
			BindInstance(gameEntity);
			
			BindInstance(new EntitySpawnSystem());
			BindInstance(new WalkEntitySystem());
			
			BindInstance(new DebugGameplaySystem());
			
			GameUIMessenger gameUIMessenger = new GameUIMessenger();
			RegisterUIScreenDefinition(GameplayViewConfig.GameUIConfig.MainGameUI, gameUIMessenger);
			BindInstance(gameUIMessenger);
			
			BindInstance(new GameUISystem());
			BindInstance(new GameInputSystem());

			BindInstance(new PlayerSystem());
			BindInstance(new BoardSystem());
			BindInstance(new TileSystem());
			BindInstance(new EnemiesSystem());
			BindInstance(new MovingBehaviourSystem());
			BindInstance(new SpawnerSystem());
			BindInstance(new DestructibleSystem());

//			BindInstance(new EntityMovingLogicSystem());
			
			BindInstance(GameplayViewConfig.MainCamera);
			BindInstance(GameplayViewConfig.CinemachineCamera);
			BindInstance(GameplayViewConfig.CinemachineTargetGroup);
			CinemachineBasicMultiChannelPerlin cinemachineNoise = GameplayViewConfig.CinemachineCamera.gameObject.GetComponent<CinemachineBasicMultiChannelPerlin>();
			BindInstance(cinemachineNoise);
			
			InstantiatePrefabAndBind<PlayerInputManager>(GameplayViewConfig.PlayerInputManagerPrefab);
			
		}
		
		private const int MAX_TILE_NUMBER = 1000;
		private const int MAX_CREATURES_NUMBER = 400;

		public override void ResetComponentContainers(DataContainersController dataController)
		{
			
			dataController.ResizeComponentsContainer<DestructableData>(MAX_CREATURES_NUMBER);
			
			dataController.ResizeComponentsContainer<HierarchyData>(MAX_TILE_NUMBER + MAX_CREATURES_NUMBER);
			dataController.ResizeComponentsContainer<ColliderData>(MAX_TILE_NUMBER + MAX_CREATURES_NUMBER);
			
			//TILES
			dataController.ResizeComponentsContainer<TileComponentData>(MAX_TILE_NUMBER);
			
			
			//creatures
			
			dataController.ResizeComponentsContainer<UnitData>(MAX_CREATURES_NUMBER);
			dataController.ResizeComponentsContainer<MovingBehaviourData>(MAX_CREATURES_NUMBER);
			dataController.ResizeComponentsContainer<BoardData>(MAX_CREATURES_NUMBER);
			dataController.ResizeComponentsContainer<WalkEntityData>(MAX_CREATURES_NUMBER);
			
		}
	}
}