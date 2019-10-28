using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Movement : MonoBehaviour
{
	public enum State
	{
		NONE,
		WANDER,
		TARGET,
	}

	[SerializeField]
	private float _movementSpeed = 5f;
	private float _distanceToNewTarget = 0.5f;
	private Vector3 _wanderDistance = Vector3.one * 5f;

	public float movementSpeedModifer = 1;

	private State _currentState = State.NONE;
	private Vector3 _currentDirection = Vector3.forward;

	private Vector3 _currentTargetPosition = Vector3.zero;

	private List<Vector3> _currentPathToTarget = new List<Vector3>();
	private int _currentPathTargetIndex = 0;

	private Graph _worldGraph;

	private Rigidbody _rigidBody;

	private void Awake()
	{
		_rigidBody = GetComponent<Rigidbody>();
	}

	public void Init(Graph worldGraph)
	{
		_worldGraph = worldGraph;
		_currentTargetPosition = _rigidBody.position;
	}

	private void Update()
	{
		if (_currentState != State.NONE)
		{
			//FIXME Change speed by environment
			UpdateMoveDirection();

			_rigidBody.MovePosition(_rigidBody.position + (_currentDirection * _movementSpeed * _worldGraph.GetNode(transform.position).traverselSpeed * Time.deltaTime));

			//FIXME Block by walls
		}
	}

	private void UpdateMoveDirection()
	{
		if (Vector3.Distance(_rigidBody.position, _currentTargetPosition) <= _distanceToNewTarget)
		{
			UpdatePathList();
		}

		if (Vector3.Distance(_rigidBody.position, _currentPathToTarget[_currentPathTargetIndex]) <= _distanceToNewTarget)
		{
			_currentPathTargetIndex++;
		}

		switch (_currentState)
		{
			case State.WANDER:
				_currentDirection = (_currentPathToTarget[_currentPathTargetIndex] - transform.position).normalized;
				break;
			case State.TARGET:
				break;
		}
	}

	private void UpdatePathList()
	{
		switch (_currentState)
		{
			case State.WANDER:
				_currentTargetPosition = GetNewWanderTarget();
				_currentPathToTarget = AStar.FindPath(_worldGraph, transform.position, _currentTargetPosition);
				Debug.Log($"New path contains {_currentPathToTarget.Count} nodes");
				_currentPathTargetIndex = 0;
				break;
		}
	}

	private Vector3 GetNewWanderTarget()
	{
		Vector3 minPos = _worldGraph.minimumPosition;
		Vector3 maxPos = _worldGraph.maximumPosition;

		Vector3 randomWanderTarget = new Vector3(
			Mathf.Clamp(Random.Range(_rigidBody.transform.position.x - _wanderDistance.x, _rigidBody.transform.position.x + _wanderDistance.x), _worldGraph.minimumPosition.x, _worldGraph.maximumPosition.x),
			0,
			Mathf.Clamp(Random.Range(_rigidBody.transform.position.z - _wanderDistance.z, _rigidBody.transform.position.z + _wanderDistance.z), _worldGraph.minimumPosition.z, _worldGraph.maximumPosition.z));

		return randomWanderTarget;
	}

	public void SetState(State newState)
	{
		_currentState = newState;
	}
}
