namespace Extensions
{
	using System.Collections.Generic;
	using UnityEngine;
	using Random = UnityEngine.Random;

	public static class CollectionExtensions
	{
		public static T GetRandomElement<T>(this List<T> collection)
		{
			if (collection != null && collection.Count > 0)
			{
				return collection[Random.Range(0, collection.Count)];
			}

			return default;
		}

		public static void ShuffleCollection<T>(this List<T> collection)
		{
			if (collection != null && collection.Count > 1)
			{
				for (int i = 0; i < collection.Count; i++)
				{
					T temp = collection[i];
					int randomIndex = Random.Range(i, collection.Count);
					collection[i] = collection[randomIndex];
					collection[randomIndex] = temp;
				}
			}
		}

		public static void SetOrAdd<T1,T2>(this Dictionary<T1,T2> dictionary, T1 key, T2 value)
		{
			if (dictionary.ContainsKey(key))
			{
				dictionary[key] = value;
			}
			else
			{
				dictionary.Add(key, value);
			}
		}

		public static T FindNearest<T> (this List<T> objects, Vector3 origin) where T : Component
		{
			T bestTarget = null;
			float closestDistanceSqr = Mathf.Infinity;
			foreach (T potentialTarget in objects)
			{
				Vector3 directionToTarget = potentialTarget.transform.position - origin;
				float dSqrToTarget = directionToTarget.sqrMagnitude;
				if (dSqrToTarget < closestDistanceSqr)
				{
					closestDistanceSqr = dSqrToTarget;
					bestTarget = potentialTarget;
				}
			}

			return bestTarget;
		}
	}
}