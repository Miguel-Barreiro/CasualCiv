using Core.Initialization;
using Core.Model.ModelSystems;
using Core.Zenject.Source.Main;
using Menus.MainMenu;

namespace Scenes.MainMenu
{
	public sealed class MainMenuBootstrap : SceneBootstrap
	{
		private MainMenuInstaller _mainMenuInstaller;
		public override SystemsInstallerBase GetLogicInstaller()
		{
			if(_mainMenuInstaller == null)
				_mainMenuInstaller = new MainMenuInstaller(Container);
			
			return _mainMenuInstaller;
		}
	}
	
	public sealed class MainMenuInstaller : SystemsInstallerBase
	{
		public MainMenuInstaller(DiContainer container) : base(container) { }
		
		public override void SetupConfigurations() { }
		
		protected override void InstallSystems()
		{
			
			BindInstance(new MainMenuController());
			
		}
		public override void ResetComponentContainers(DataContainersController dataController)
		{
			
			
		}
	}
}