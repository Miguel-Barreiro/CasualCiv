using Core.Systems;

namespace Game
{
	public static class GameSystemGroups
	{
		public static SystemGroup InputGroup = new SystemGroup("Input");
		public static SystemGroup UIGroup = new SystemGroup("UI");
		public static SystemGroup GameLogicGroup = new SystemGroup("GameLogic");
		
	}
}