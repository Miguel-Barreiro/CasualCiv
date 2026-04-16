using Core.Events;
using Core.Systems;
using Events;
using Scenes.Play;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Input
{
	public sealed class TileInputSystem : IUpdateSystem
	{
		[Inject] private readonly EventQueue EventQueue = null!;
		[Inject] private readonly GameplayViewConfig GameplayViewConfig = null!;
		[Inject] private readonly Camera MainCamera = null!;


		public void UpdateSystem(float deltaTime)
		{
			Vector2? screenPos = GetScreenPosition();
			if (screenPos == null) return;

			Vector3 worldPos = MainCamera.ScreenToWorldPoint(new Vector3(screenPos.Value.x, screenPos.Value.y,
																		-MainCamera.transform.position.z));
			Vector3Int cellPos = GameplayViewConfig.GroundTilemap.WorldToCell(worldPos);

			OnTileClickedEvent evt = EventQueue.Execute<OnTileClickedEvent>();
			evt.TilePosition = new Vector2Int(cellPos.x, cellPos.y);
		}

		private static Vector2? GetScreenPosition()
		{
			Mouse mouse = Mouse.current;
			if (mouse != null && mouse.leftButton.wasPressedThisFrame)
				return mouse.position.ReadValue();

			Touchscreen touchscreen = Touchscreen.current;
			if (touchscreen != null && touchscreen.primaryTouch.press.wasPressedThisFrame)
				return touchscreen.primaryTouch.position.ReadValue();

			return null;
		}

		public bool Active { get; set; } = true;
		public SystemGroup Group { get; } = GameSystemGroups.InputGroup;

	}
}
