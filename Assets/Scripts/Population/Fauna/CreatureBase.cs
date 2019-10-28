using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CreatureBase : MonoBehaviour
{
	//Hunger (Foodtype? (herb(tree, bush)(size maybe)/carni(risk/reward)(estimated damage for food amount(maybe base on maxEnergy))/omnivore)
	//Thirst (Need to add water object)

	//Values
	//Sex
	private float _currentHunger = 0; //++
	private float _currentThirst = 0; //++
	//private float _currentEnergy = 100; //-- (0 == dead) //Should maybe change to be a multiplier for hunger/thirst
	private float _matingDrive = 0; //++
	//What about sleep?
	//Age? Maybe lifetime affected by movespeed (Rabbits (fast, short lifespan), elephant (slow, long lifespan))
	// - Maybe have a maxEnergy instead of age, and have that decline over time

	//Limits
	//private float _hungerThreshold = 80f; //When energy drops
	//private float _thirstThreshold = 80f; //When energy drops

	//Stats
	//Should be something like, it cost more energy to move often
	/// <summary>
	/// How often it can move and how fast it gets hungry/thirsty
	/// </summary>
	private float _energyUsage = 1f;

	//Mating result/Mating cost
	// - Should cost energy to mate. The more energy spent, the more kids

	//Pregnant time
	// - while pregnant (Slower, more hungry) Quick (underdeveloped(Long grow up time)) Slow (Well developed)

	//Sight Range

	//?Damage, defensive for herbivores, offensive for carnivores, both for omnivores
	// - Health or just energy loss

	/// <summary>
	/// How often the creature can make a move
	/// </summary>
	private float _moveDelay = 2f;

	/// <summary>
	/// How far to find stuff
	/// </summary>
	private float _sensoryDistance = 5f;

	private float _moveTimer = 0;

	private Action _currentAction = default;

	private Transform _currentTarget = default;
	private int _currentPathIndex = 0;
	private List<Vector3> _currentPath = default;
	
	private void Update()
	{
		_moveTimer += Time.deltaTime * _energyUsage;

		UpdateValues();
		UpdateAction();

		DoAction();
	}

	private void UpdateValues()
	{
		_currentHunger += Time.deltaTime * _energyUsage * 10;
		_currentThirst += Time.deltaTime * _energyUsage * 10;
		//Maybe not multiply energyUsage
		_matingDrive += Time.deltaTime * _energyUsage;
	}

	private void UpdateAction()
	{
		//Should also check for enemies
		if (_currentHunger >= _currentThirst && _currentHunger >= _matingDrive)
		{
			//Get correct type food
			//If target not already food
			if (_currentTarget?.GetComponent<Bush>() == null)
			{
				//Find food
				List<Collider> objectsSeen = Physics.OverlapSphere(transform.position, _sensoryDistance).Where(obj => obj.GetComponent<Bush>() != null).ToList();

				if (objectsSeen.Count > 0)
				{
					_currentTarget = objectsSeen.FindNearest(transform.position).transform;

					_currentPath = AStar.FindPath(GraphHelper.CurrentGraph, transform.position, _currentTarget.position);
					_currentPathIndex = 1;

					if (transform.position.Neighbors().Contains(_currentTarget.position))
					{
						_currentAction = InteractWithTarget;
					}
					else
					{
						_currentAction = MoveTowardsTarget;
					}
				}
				else
				{
					_currentTarget = null;
				}
			}
		}
		else if (_currentThirst >= _matingDrive)
		{
			//If target not already water
			//Find water
		}
		else
		{
			//Find mate
		}

		if (_currentTarget == null)
		{
			_currentAction = Wander;
		}
		else
		{
			if (transform.position.Neighbors().Contains(_currentTarget.position))
			{
				_currentAction = InteractWithTarget;
			}
			else
			{
				_currentAction = MoveTowardsTarget;
			}
		}

		void Wander()
		{
			Debug.Log("Wander");
			transform.position = transform.position.Neighbors().GetRandomElement();
			//Move should check map size
		}
		void MoveTowardsTarget()
		{
			if (_currentPathIndex < _currentPath.Count)
			{
				Debug.Log("Move towards target");
				transform.position = _currentPath[_currentPathIndex++];
			}
			else
			{
				Debug.Log("Path is empty");
				Wander();
			}
		}
		void InteractWithTarget()
		{
			if (_currentTarget.GetComponent<Bush>() != null)
			{
				float amountEaten = _currentTarget.GetComponent<Bush>().Eat(_currentHunger);
				_currentHunger -= amountEaten;
				Debug.Log($"Nom Nom ({amountEaten})");
			}

			_currentTarget = null;
		}
	}

	private void DoAction()
	{
		if (_moveTimer >= _moveDelay)
		{
			_moveTimer -= _moveDelay;
			//DoAction
			//-Move
			//-Eat
			//-Drink
			//-Mate

			_currentAction();
		}
	}
}
