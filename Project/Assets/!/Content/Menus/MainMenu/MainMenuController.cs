using Core.Initialization;
using Core.Systems;
using Core.View.UI;
using Global;
using Zenject;

namespace Menus.MainMenu
{
	public sealed class MainMenuController : IStartSystem
	{
		[Inject] private readonly MenusConfig MenusConfig = null!;
		[Inject] private readonly UIRoot UIRoot = null!;
		[Inject] private readonly MainMenuMessenger MainMenuMessenger = null!;
		[Inject] private readonly ScenesController ScenesController = null!;


		public void StartSystem()
		{
			UIRoot.Show(MenusConfig.MainMenuUI);
			
			MainMenuMessenger.OnPlayButtonClicked += OnPlayButtonClicked;
		}

		private void OnPlayButtonClicked()
		{
			ScenesController.SwitchScene(SceneNames.PLAY_SCENE);
		}
	}
}