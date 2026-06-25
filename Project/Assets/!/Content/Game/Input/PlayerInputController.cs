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

		public void OnAim(InputAction.CallbackContext aimValue)
		{
			AimPlayerEvent aimPlayerEvent = EntityEventQueue.Execute<AimPlayerEvent>(EntityID);
			Vector2 readValue = aimValue.ReadValue<Vector2>();
			aimPlayerEvent.AimPosition = readValue;
		}

		// public void OnSprint(InputAction.CallbackContext sprintValue)
		// {
		// 	bool pressingSprinting = sprintValue.ReadValueAsButton();
		// 	if (PlayerContainer.GetComponent(EntityID).isSprinting)
		// 	{
		// 		if (!pressingSprinting)
		// 		{
		// 			SprintEndPlayerEvent sprintEndPlayerEvent = EntityEventQueue.Execute<SprintEndPlayerEvent>(EntityID);
		// 		}
		// 	} else
		// 	{
		// 		if (!pressingSprinting)
		// 		{
		// 			SprintStartPlayerEvent sprintStartPlayerEvent = EntityEventQueue.Execute<SprintStartPlayerEvent>(EntityID);
		// 		}
		// 	}
		//
		// }

		public void OnJump(InputAction.CallbackContext jumpValue)
		{
			Debug.Log($"on jump pressed");
			
			DeploySpawnerEvent newEvent = EntityEventQueue.Execute<DeploySpawnerEvent>(EntityID);
			newEvent.SpawnPosition = BoardSystem.GetBoardPosition(transform.position);
		}

		public void OnDash(InputAction.CallbackContext dashValue)
		{
			
		}
		
		public void OnAttack(InputAction.CallbackContext attackValue)
		{
		}
		

	}
}