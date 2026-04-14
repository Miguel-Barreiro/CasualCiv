using Core.Model.Data;
using Core.View.UI;
using UnityEngine;

namespace Game.UI
{
	public sealed class GameUIConfig : DataConfig
	{
		[SerializeField] private UIScreenDefinition _mainGameUI = null!;
		public UIScreenDefinition MainGameUI => _mainGameUI;
		
	}
}