using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour 
{
	[SerializeField]
	Vector3 _moveSpeedScale = Vector2.one;

	[SerializeField]
	private Vector3 _upperLimit, _lowerLimit;

	Vector3 _lastMousePosition;

	private void Start()
	{
		_lastMousePosition = Input.mousePosition;

		//SetCameraPosition(new Vector3((BuildController.instance._availableWidth / 2) - 0.5f, 0, -5));
	}

	public void SetLimit(Vector3 lowerLimit, Vector3 upperLimit)
	{
		_lowerLimit = lowerLimit;
		_upperLimit = upperLimit;
	}

	public void SetCameraPosition (Vector3 newPos)
	{
		transform.position = new Vector3(
		Mathf.Clamp(newPos.x, _lowerLimit.x, _upperLimit.x), 
		Mathf.Clamp(newPos.y, _lowerLimit.y, _upperLimit.y), 
		Mathf.Clamp(newPos.z, _lowerLimit.z, _upperLimit.z));
	}

	void Update () 
	{
		Vector3 _currentMousePosition = Input.mousePosition;

		if (Input.GetMouseButton(2))
		{
			Vector3 difference = new Vector3(_currentMousePosition.x - _lastMousePosition.x, 0, _currentMousePosition.y - _lastMousePosition.y);
			difference.Scale(_moveSpeedScale);
			SetCameraPosition(transform.position + difference);
		}

		float scroll = Input.GetAxis("Mouse ScrollWheel") * _moveSpeedScale.y;
		SetCameraPosition(transform.position + new Vector3(0, scroll, 0));

		_lastMousePosition = Input.mousePosition;
	}
}
