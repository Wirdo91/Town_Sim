namespace Extensions
{
	using UnityEngine;

	public static class TransformExtensions
	{
		#region Set Positions
		//Set Local pos
		public static void SetLocalPosX(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localPosition;
			tmpVector.x = value;
			transform.localPosition = tmpVector;
		}
		public static void SetLocalPosY(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localPosition;
			tmpVector.y = value;
			transform.localPosition = tmpVector;
		}
		public static void SetLocalPosZ(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localPosition;
			tmpVector.z = value;
			transform.localPosition = tmpVector;
		}

		//Set Global pos
		public static void SetPosX(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.position;
			tmpVector.x = value;
			transform.position = tmpVector;
		}
		public static void SetPosY(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.position;
			tmpVector.y = value;
			transform.position = tmpVector;
		}
		public static void SetPosZ(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.position;
			tmpVector.z = value;
			transform.position = tmpVector;
		}

		#endregion

		#region Set Rotation

		//Set Local euler
		public static void SetLocalEulerX(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localEulerAngles;
			tmpVector.x = value;
			transform.localEulerAngles = tmpVector;
		}
		public static void SetLocalEulerY(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localEulerAngles;
			tmpVector.y = value;
			transform.localEulerAngles = tmpVector;
		}
		public static void SetLocalEulerZ(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localEulerAngles;
			tmpVector.z = value;
			transform.localEulerAngles = tmpVector;
		}

		//Set Global Euler
		public static void SetEulerX(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.eulerAngles;
			tmpVector.x = value;
			transform.eulerAngles = tmpVector;
		}
		public static void SetEulerY(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.eulerAngles;
			tmpVector.y = value;
			transform.eulerAngles = tmpVector;
		}
		public static void SetEulerZ(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.eulerAngles;
			tmpVector.z = value;
			transform.eulerAngles = tmpVector;
		}

		#endregion

		#region Set Scale

		//Set Local scale
		public static void SetLocalScaleX(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localScale;
			tmpVector.x = value;
			transform.localScale = tmpVector;
		}
		public static void SetLocalScaleY(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localScale;
			tmpVector.y = value;
			transform.localScale = tmpVector;
		}
		public static void SetLocalScaleZ(this Transform transform, float value)
		{
			Vector3 tmpVector = transform.localScale;
			tmpVector.z = value;
			transform.localScale = tmpVector;
		}

		#endregion

		public static void KillChildren(this Transform transform)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				transform.GetChild(i).gameObject.SetActive(false);
#if UNITY_EDITOR
				Object.DestroyImmediate(transform.GetChild(i).gameObject);
#else
				Object.Destroy(transform.GetChild(i).gameObject);
#endif
			}
		}
	}
}