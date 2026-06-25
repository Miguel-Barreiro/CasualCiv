namespace BehaviourTrees
{
	public interface IBehaviourNode
	{
		public BehaviourResult Execute( EntityBehaviourMemory memory );
		public void Reset();
	}
	
	
	public enum BehaviourResult
	{
		Success,
		Running,
		Failure,
	}
}