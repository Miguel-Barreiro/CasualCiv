using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using XNode;

namespace BehaviourTrees
{
	public sealed class BehaviourTreeGraph : NodeGraph
	{
		
	}

	public sealed class BehavourTreeSystem
	{
		
		
		
		
	}


	[StructLayout(LayoutKind.Auto)]
	public struct EntityBehaviourMemory : IComponentData
	{
		public EntId ID { get; set; }
		

		public void Init() { }
	}

	public interface IMemory : Component<EntityBehaviourMemory> { }

}