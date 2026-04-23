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

		[SerializeField] 
		public MovementConfig MovementConfig;

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

	[Serializable]
	public sealed class MovementConfig
	{
		[SerializeField, Range(1, 500)] 
		private float _playerRestMass = 500;
		[SerializeField, Range(1, 100)] 
		private float _playerMovingMass = 5;
		
		[SerializeField, Range(5, 40)]
		private float _walkSpeed = 15f;

		[SerializeField, Range(10, 40)]
		private float _sprintSpeed = 30f;
		
		
		public float PlayerMovingMass => _playerMovingMass;
		public float PlayerRestMass => _playerRestMass;
		public float WalkSpeed => _walkSpeed;
		public float SprintSpeed => _sprintSpeed;
	}

}