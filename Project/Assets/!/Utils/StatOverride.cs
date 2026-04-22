using System;
using Core.Model.Stats;
using UnityEngine;

namespace Utils
{
	[Serializable]
	public struct StatOverride
	{
		public StatConfig Stat;

		[Range(0, 300)]
		public float OverrideValue;
	}
}