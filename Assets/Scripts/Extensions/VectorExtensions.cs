using System.Collections.Generic;
using UnityEngine;

namespace Extensions
{
	public static class VectorExtensions
	{
		public static List<Vector3> Neighbors(this Vector3 origin, float distance = 1, float interval = 1)
		{
			List<Vector3> result = new List<Vector3>();

			for (float x = 0; x <= distance; x += interval)
			{
				for (float z = 0; z <= distance; z += interval)
				{
					if (!(x == 0 && z == 0))
					{
						result.Add(origin + new Vector3(x, 0, z));
						if (z > 0)
						{
							result.Add(origin + new Vector3(x, 0, -z));
						}
					}
				}
			}
			for (float x = interval; x <= distance; x += interval)
			{
				for (float z = 0; z <= distance; z += interval)
				{
					if (!(x == 0 && z == 0))
					{
						result.Add(origin + new Vector3(-x, 0, z));
						if (z > 0)
						{
							result.Add(origin + new Vector3(-x, 0, -z));
						}
					}
				}
			}

			return result;
		}

		public static Vector3 Rotate(this Vector3 vector)
		{
			return new Vector3(vector.x, vector.z, vector.y);
		}
		public static Vector3 Convert(this Vector2 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}
	}
}
