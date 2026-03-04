using Core.Model.Data;
using Core.View.UI;
using UnityEngine;

namespace Global
{
	public sealed class MenusConfig : DataConfig
	{
		[SerializeField] private UIScreenDefinition mainMenuUI = null!;
		public UIScreenDefinition MainMenuUI => mainMenuUI;

	}
}