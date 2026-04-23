using Core.Events;
using Core.Model;
using Zenject;

namespace Game.Entities
{
	public sealed class SprintStartPlayerEvent : EntityEvent<SprintStartPlayerEvent>
	{
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		public override void Execute()
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			playerData.isSprinting = true;
			
		}
	}
	
	
	public sealed class SprintEndPlayerEvent : EntityEvent<SprintEndPlayerEvent>
	{
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		
		public override void Execute()
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			playerData.isSprinting = false;
		}
	}

}