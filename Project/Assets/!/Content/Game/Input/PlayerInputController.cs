using Core.Events;
using Core.Initialization;
using Core.Model;
using FixedPointy;
using Game.Entities;
using Game.Entities.Board;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Input
{
	[RequireComponent(typeof(PlayerInput))]
	public sealed class PlayerInputController : MonoBehaviour
	{
		[Inject] private readonly EventQueue EventQueue = null!;
		[Inject] private readonly BasicCompContainer<PlayerData> PlayerContainer = null!;
		[Inject] private readonly BoardSystem BoardSystem = null!;


		public EntId EntityID { get; private set; } = EntId.Invalid;

		public void SetEntity(EntId entId)
		{
			EntityID = entId;
		}

		private void Awake()
		{
			ObjectBuilder.GetInstance().Inject(this);
			
			OnNewPlayerJoinsEvent onNewPlayerJoinsEvent = EventQueue.Execute<OnNewPlayerJoinsEvent>();
			onNewPlayerJoinsEvent.PlayerInputController = this;
		}

		public void OnMove(InputAction.CallbackContext moveValue)
		{
			MovePlayerEvent movePlayerEvent = EntityEventQueue.Execute<MovePlayerEvent>(EntityID);
			Vector2 readValue = moveValue.ReadValue<Vector2>();
			movePlayerEvent.Direction = readValue;
			// transform.Translate(new Vector3(readValue.x, readValue.y, 0));
		}
		
		
		public void OnMainAction(InputAction.CallbackContext attackValue)
		{
			ref PlayerData playerData = ref PlayerContainer.GetComponent(EntityID);
			
			DeploySpawnerEvent deploySpawnerEvent = EntityEventQueue.Execute<DeploySpawnerEvent>(EntityID);
			deploySpawnerEvent.SpawnPosition = BoardSystem.GetBoardPosition(playerData.Position);
			
		}

		public void OnCancel(InputAction.CallbackContext attackValue)
		{
			
		}


	}
}