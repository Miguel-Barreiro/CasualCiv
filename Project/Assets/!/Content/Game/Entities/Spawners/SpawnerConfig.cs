using Game.Entities.Enemies;
using UnityEngine;

namespace Game.Entities.Spawners
{
	public sealed class SpawnerConfig : EntityConfig
	{

		[SerializeField] private UnitConfig _UnitConfig;
		public UnitConfig UnitConfig => _UnitConfig;
		
		
	}
}