using Core.Model.Data;
using Core.Model.Stats;
using UnityEngine;

namespace Game
{
	public sealed class GameStatsContainer : DataTable
	{
		[SerializeField] private StatConfig day_stat;
		public StatConfig Day => day_stat;

		
		
	}
}