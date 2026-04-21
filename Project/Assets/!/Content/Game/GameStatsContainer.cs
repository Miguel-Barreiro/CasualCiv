using Core.Model.Data;
using Core.Model.Stats;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
	public sealed class GameStatsContainer : DataTable
	{
		[SerializeField] private StatConfig _speed;
		public StatConfig Speed => _speed;
		

	}
}