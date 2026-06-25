namespace DebugUtils
{

	
	public class DebugAssert
	{
#if UNITY_EDITOR
		public static void Assert<T>(T system, bool condition, string message = "")
		{
			if (!condition)
			{
				UnityEngine.Debug.LogError($"{system.GetType().Name}: Assert failed! {message}");
				UnityEngine.Debug.Break();
			}
		}
#else
		public static void Assert(bool condition, string message = ""){}
		
#endif	
		
	}
}