using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;

namespace Game.Entities.Enemies
{
	public sealed class EnemyEntity :   Entity, 
										IEnemy,
										IHierarchyEntity
	{
		public EnemyEntity(EnemyConfig config) { }
	}


	public sealed class EnemiesSystem
	{
		
	}



	[StructLayout(LayoutKind.Auto)]
	public struct EnemyData : IComponentData
	{
		public EntId ID { get; set; }

		public void Init() { }
	}

	public enum EnemyMoveBehaviourType
	{
		TargetCloseFirst = 1,
	}
	

	public interface IEnemy : Component<EnemyData> { }
}