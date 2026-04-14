using Core.Systems;
using Core.View.UI;
using Zenject;

namespace Game.UI
{
	public sealed class GameUISystem : IStartSystem
	{
		[Inject] private readonly GameUIConfig GameUIConfig = null!;
		[Inject] private readonly UIRoot UIRoot = null!;
		[Inject] private readonly GameUIMessenger GameUIMessenger = null!;

		
		public void StartSystem()
		{
			UIRoot.Show(GameUIConfig.MainGameUI);
		}
	}
}