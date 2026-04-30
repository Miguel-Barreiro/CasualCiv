using UnityEngine;

namespace Game.Entities.Enemies
{
	public sealed class EnemyConfig : EntityConfig
	{
		[SerializeField] private EnemyMoveBehaviourType _EnemyMoveBehaviourType;
		public EnemyMoveBehaviourType EnemyMoveBehaviourType => _EnemyMoveBehaviourType;

		

	}
}