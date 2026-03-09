using Core.Model;
using Core.Model.ModelSystems;

namespace Game
{
	public sealed class GameEntity : Entity, 
									IGameComponent
	{ }
	
	public interface IGameComponent : Component<GameComponentData> { }
	public struct GameComponentData : IComponentData
	{
		public EntId ID { get; set; }
		public EntId BoardEntityID;
		
		public void Init() { }
	}
}