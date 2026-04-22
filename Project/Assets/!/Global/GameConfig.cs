using System;
using Core.Model.Data;
using UnityEngine;

namespace Global
{
	public sealed class GameConfig : DataConfig
	{
		[SerializeField] private Vector2 _tileSize = Vector2.one;
		public Vector2 TileSize => _tileSize;
		
		[SerializeField]
		public InputConfig InputConfig;
		
	}

	[Serializable]
	public sealed class InputConfig
	{
		[SerializeField, Range(1, 10)] 
		private float _targetMaxDistance;
		[SerializeField, Range(0, 4)] 
		private float _targetMinDistance;
		
		[SerializeField, Range(0.1f, 4)] 
		private float _targetMoveSpeed;
		public float TargetMinDistance => _targetMinDistance;
		public float TargetMaxDistance => _targetMaxDistance;

		public float TargetMoveSpeed => _targetMoveSpeed;
	}

}