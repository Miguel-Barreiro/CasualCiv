using Core.Events;
using Core.Initialization;
using Core.Model;
using FixedPointy;
using Game.Entities;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Input
{
	[RequireComponent(typeof(PlayerInput))]
	public sealed class PlayerInputController : MonoBehaviour
	{
		[Inject] private readonly EventQueue EventQueue = null!;

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
			movePlayerEvent.Direction = new FixVec2(readValue.x, readValue.y);
			transform.Translate(new Vector3(readValue.x, readValue.y, 0));
		}

		public void OnAim(InputAction.CallbackContext aimValue)
		{
			
		}



	}
}