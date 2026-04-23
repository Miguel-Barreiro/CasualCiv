using System;
using UnityEngine;

namespace Game.Entities.Spawnpoints
{
	[RequireComponent(typeof(SpriteRenderer))]
	public sealed class SpawnpointView : MonoBehaviour
	{
		private SpriteRenderer _spriteRenderer;
		[SerializeField] private Gradient ColorGlow;
		[SerializeField] private AnimationCurve GlowAnimationCurve;
		
		private void Awake()
		{
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			_spriteRenderer.color = ColorGlow.Evaluate(GlowAnimationCurve.Evaluate(Mathf.Abs(Mathf.Sin(Time.time))));
		}
	}
}