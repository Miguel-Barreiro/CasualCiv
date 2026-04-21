using Core.Events;
using Game.Entities;
using Zenject;

namespace Game.Input
{
	public sealed class OnNewPlayerJoinsEvent : Event<OnNewPlayerJoinsEvent>
	{
		[Inject] private readonly EntitySpawnSystem EntitySpawnSystem = null!;

		
		public PlayerInputController PlayerInputController;

		
		public override void Execute()
		{
			EntitySpawnSystem.SpawnPlayer(PlayerInputController);


		}
	}
}