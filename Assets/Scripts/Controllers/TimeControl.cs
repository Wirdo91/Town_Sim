using System;
using UnityEngine;

public class TimeControl : MonoBehaviour
{
	public static TimeControl instance => _instance;
	private static TimeControl _instance;

	[SerializeField]
	private bool _pauseTime = false;

	[SerializeField]
	private float _dayDuration = 20f;
	[SerializeField]
	private float _timeOfDay = 12;
	[SerializeField]
	private Transform _sunAnchor = default;

	private float _maxTimeOfDay = 24;

	private TimeSpan _elapsedTime = default;
	public string ElapsedTimeString = "";

	public float timeOfDay => _timeOfDay;
	public bool isDayTime => _timeOfDay > 6 && _timeOfDay < 18;
	public bool isNightTime => !isDayTime;
	public float maxTimeOfDay => _maxTimeOfDay;
	public TimeSpan elapsedTime => _elapsedTime;

	private void Awake()
	{
		_instance = this;
		_elapsedTime = new TimeSpan(
			(int)_timeOfDay,
			(int)(60 * (_timeOfDay - ((int)_timeOfDay))),
			0);

		if (_sunAnchor == null)
		{
			_sunAnchor = transform;
		}
	}

	void Update()
	{
		if (!_pauseTime)
		{
			UpdateTimeOfDay();
		}

		UpdateSunPosition();
		UpdateTimeScale();
	}

	void UpdateTimeOfDay()
	{
		float hourDifference = _maxTimeOfDay * (Time.deltaTime / _dayDuration);
		_timeOfDay += hourDifference;

		if (_timeOfDay > _maxTimeOfDay)
		{
			_timeOfDay -= _maxTimeOfDay;
		}

		_elapsedTime = _elapsedTime.Add(new TimeSpan(
			(int)hourDifference, 
			(int)(60 * (hourDifference - ((int)hourDifference))), 
			0));

		ElapsedTimeString = $"D:{_elapsedTime.Days} H:{_elapsedTime.Hours} M:{_elapsedTime.Minutes}";
	}

	void UpdateSunPosition()
	{
		_sunAnchor.localEulerAngles = Vector3.forward * 360 * (_timeOfDay / _maxTimeOfDay);
	}

	void UpdateTimeScale()
	{
		float modifier = 1;
		if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl))
		{
			modifier = 10;
		}
		else if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftShift))
		{
			modifier = 100;
		}
		if (Input.GetKeyUp(KeyCode.KeypadPlus))
		{
			Time.timeScale += 0.1f * modifier;

			Debug.Log($"New timescale {Time.timeScale}");
		}
		else if (Input.GetKeyUp(KeyCode.KeypadMinus))
		{
			Time.timeScale -= 0.1f * modifier;

			Debug.Log($"New timescale {Time.timeScale}");
		}
		else if (Input.GetKeyUp(KeyCode.Keypad0))
		{
			Time.timeScale = 0;

			Debug.Log($"New timescale {Time.timeScale}");
		}
		else if (Input.GetKeyUp(KeyCode.Keypad1))
		{
			Time.timeScale = 1;

			Debug.Log($"New timescale {Time.timeScale}");
		}
	}
}
