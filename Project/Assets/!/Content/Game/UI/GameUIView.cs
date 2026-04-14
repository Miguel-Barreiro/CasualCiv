using Core.View.UI;

namespace Game.UI
{
	public sealed class GameUIView : UIView<GameUIMessenger>
	{
		protected override void OnUnregister(GameUIMessenger uiMessenger)
		{ }
		protected override void OnRegister(GameUIMessenger uiMessenger)
		{ }
	}
}