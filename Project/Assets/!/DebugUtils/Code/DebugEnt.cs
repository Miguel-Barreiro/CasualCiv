using Core.Model;
using UnityEngine;
using Zenject;

namespace DebugUtils
{
	public sealed class DebugEnt
	{
		[Inject] private readonly EntitiesContainer EntitiesContainer = null!;
		
		public void Log(string message, EntId entId)
		{
			Entity entity = EntitiesContainer.GetEntity(entId);
			Debug.Log($"[{entity} ({entId})] {message}");
		}
		public void LogError(string message, EntId entId)
		{
			Entity entity = EntitiesContainer.GetEntity(entId);
			Debug.LogError($"[{entity} ({entId})] {message}");
		}
	}
}