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
		
		
		
		[SerializeField] private StatConfig _cooldown;
		public StatConfig Cooldown => _cooldown;

		
		[SerializeField] private StatConfig _LifeTimer;
		public StatConfig LifeTimer => _LifeTimer;


		[SerializeField] private StatConfig _Health;
		public StatConfig Health => _Health;

	}
}