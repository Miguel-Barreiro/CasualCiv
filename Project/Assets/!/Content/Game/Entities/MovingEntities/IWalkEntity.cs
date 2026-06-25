using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.Systems;
using Game.Entities.Board;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Entities
{
	[StructLayout(LayoutKind.Auto)]
	// [ComponentData( ContainerType = typeof(WalkDataContainer)) ]
	public struct WalkEntityData : IComponentData
	{
		public EntId ID { get; set; }
		
		public bool IsFriendly;
		public NavMeshAgent Agent;

		public void Init()
		{
		}

		private const int MAX_PATH_POINTS = 8;

		public const uint NO_MOVING = 0;
		public const uint MOVING = 1;
		public const uint PLANNING = 2;
	}
	
	public sealed class WalkEntitySystem
	{
		
	}


	

	public interface IWalkEntity : Component<WalkEntityData> { }
}