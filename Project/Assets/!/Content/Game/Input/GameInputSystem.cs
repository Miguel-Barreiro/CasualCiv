using Core.Events;
using Core.Systems;
using Scenes.Play;
using UnityEngine;
using Zenject;

namespace Game.Input
{
	public sealed class GameInputSystem : IUpdateSystem
	{
		[Inject] private readonly EventQueue EventQueue = null!;
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly Camera MainCamera = null!;

		public void UpdateSystem(float deltaTime)
		{
		
			
		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.InputGroup;
	}
}
