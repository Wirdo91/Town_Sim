using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
	[SerializeField]
	private Transform _hourArm = default;
	[SerializeField]
	private Transform _minuteArm = default;

	void Update()
    {
		float hour = (((int)TimeControl.instance.timeOfDay) / 12f);
		_hourArm.localEulerAngles = Vector3.back * hour * 360;

		float minute = (TimeControl.instance.timeOfDay - ((int)TimeControl.instance.timeOfDay));
		_minuteArm.localEulerAngles = Vector3.back * minute * 360;
    }
}
