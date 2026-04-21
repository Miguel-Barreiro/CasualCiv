using System;
using Core.Initialization;
using Core.Model.ModelSystems;
using Core.Zenject.Source.Main;
using DebugUtils;
using Game;
using Game.Entities;
using Game.Input;
using Game.UI;
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
		[SerializeField] private Tilemap groundTilemap;
		public Tilemap GroundTilemap => groundTilemap;

		[SerializeField] private Tilemap surfaceTilemap;
		public Tilemap SurfaceTilemap => surfaceTilemap;
		
		[SerializeField] private Tilemap objectsTilemap;
		public Tilemap ObjectsTilemap => objectsTilemap;
		
		[SerializeField] private Tilemap airTilemap;
		public Tilemap AirTilemap => airTilemap;
		
		[Space(10)]
		[SerializeField] private Camera _mainCamera;
		public Camera MainCamera => _mainCamera;
		
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
			BindInstance(new PositionEntitiesSystem());
			
			BindInstance(new DebugGameplaySystem());
			
			GameUIMessenger gameUIMessenger = new GameUIMessenger();
			RegisterUIScreenDefinition(GameplayViewConfig.GameUIConfig.MainGameUI, gameUIMessenger);
			BindInstance(gameUIMessenger);
			
			BindInstance(new GameUISystem());
			BindInstance(new GameInputSystem());

			BindInstance(GameplayViewConfig.MainCamera);
		}

		public override void ResetComponentContainers(DataContainersController dataController)
		{
			
		}
	}
}