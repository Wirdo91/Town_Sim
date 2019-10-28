namespace Extensions
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using Random = UnityEngine.Random;

	public static class MathExtensions
	{
		public static float Normalized(this float value, float min, float max)
		{
			return (value - min) / (max - min);
		}
		public static float NormalizedBetween(this float value, float min, float max, float normalizeMin, float normalizeMax)
		{
			return (normalizeMax - normalizeMin) * (value - min) / (max - min) + normalizeMin;
		}

		public static float Round(this float value, int decimals = 0)
		{
			return Mathf.Round(value * Mathf.Pow(10, decimals)) / Mathf.Pow(10, decimals);
		}
	}
}