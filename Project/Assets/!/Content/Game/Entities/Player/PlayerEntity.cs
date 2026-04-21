using System.Runtime.InteropServices;
using Core.Model;
using Core.Model.ModelSystems;
using Core.View;
using Game.Input;
using UnityEngine;

namespace Game.Entities
{
	public sealed class PlayerEntity : Entity, IPlayer
	{
		public PlayerEntity(PlayerInputController playerInputController, EntityConfig config)
		{
			ref PlayerData playerData = ref GetComponent<PlayerData>();
			playerData.PlayerInputController = playerInputController;
			playerInputController.SetEntity(ID);

			ViewEntitiesContainer viewEntitiesContainer = GetSystem<ViewEntitiesContainer>();
			EntityViewAtributes? entityViewAtributes = viewEntitiesContainer.Spawn(config.Prefab, ID);
			if (entityViewAtributes == null || entityViewAtributes.GameObject == null)
				return;

			entityViewAtributes.GameObject.transform.position = Vector3.zero;
		}
		
	}

	[StructLayout(LayoutKind.Auto)]
	public struct PlayerData : IComponentData
	{
		public PlayerInputController PlayerInputController;

		public EntId ID { get; set; }
		

		public void Init() { }
	}

	public interface IPlayer : IPositionComponent, 
								Component<PlayerData> { }
	
	
}