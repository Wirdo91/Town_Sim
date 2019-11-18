using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class CreatureBase : MonoBehaviour, CreatureUI.CreatureUIData
{
	[SerializeField]
	private Transform _model = default;

	//Hunger (Foodtype? (herb(tree, bush)(size maybe)/carni(risk/reward)(estimated damage for food amount(maybe base on maxEnergy))/omnivore)
	//Thirst (Need to add water object)

	//Values
	//Sex
	private float _currentHunger = 0; //++
	private float _currentThirst = 0; //++
									  //private float _currentEnergy = 100; //-- (0 == dead) //Should maybe change to be a multiplier for hunger/thirst
	private float _matingDrive = 0; //++

	private float _primaryLimit = 50;
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
	private float _sensoryDistance = 50f;

	private float _moveTimer = 0;

	private AvailableAction _currentAction = AvailableAction.None;

	private Transform _currentTarget = default;
	private int _currentPathIndex = 0;
	private List<Vector3> _currentPath = default;

	private enum AvailableAction
	{
		None,
		Move,
		Interact
	}

	private void Update()
	{
		_moveTimer += Time.deltaTime * _energyUsage;

		UpdateValues();

		UpdateTarget();

		DoAction();
	}

	private void UpdateValues()
	{
		_currentHunger += Time.deltaTime * _energyUsage;
		_currentHunger = Mathf.Clamp(_currentHunger, -0, 100f);
		_currentThirst += Time.deltaTime * _energyUsage;
		_currentThirst = Mathf.Clamp(_currentThirst, -0, 100f);

		//FIXME Add threshold for food and drink, when they should be highest priority
		//FIXME Add UI for creatures to show hunger,thirst,mate and current action
		//Same as https://www.youtube.com/watch?v=r_It_X7v-1E

		if (_currentHunger >= 100)
		{
			Debug.Log("Died of Hunger");
			Destroy(gameObject);
		}
		else if (_currentThirst >= 100)
		{
			Debug.Log("Died of Thirst");
			Destroy(gameObject);
		}

		//Maybe not multiply energyUsage
		_matingDrive += Time.deltaTime * _energyUsage;
		_matingDrive = Mathf.Clamp(_matingDrive, 0, 100f);
	}

	void UpdateTarget()
	{
		//Should also check for enemies
		if (_currentHunger > Mathf.Max(new[] { _currentThirst, _matingDrive }) || _currentHunger > _primaryLimit)
		{
			FindFood();
		}
		else if (_currentThirst > Mathf.Max(new[] { _currentHunger, _matingDrive }) || _currentThirst > _primaryLimit)
		{
			FindWater();
		}
		else if (_matingDrive > Mathf.Max(new[] { _currentHunger, _currentThirst }))
		{
			//Find mate
			FindMate();
		}
		else
		{
			_currentTarget = null;
		}

		if (_currentTarget == null)
		{
			_currentAction = AvailableAction.None;
		}
		else if (transform.position.Neighbors().Contains(_currentTarget.position))
		{
			_currentAction = AvailableAction.Interact;
		}
		else
		{
			_currentAction = AvailableAction.Move;
		}

		void FindFood()
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
				}
				else
				{
					_currentTarget = null;
				}
			}
		}
		void FindWater()
		{
			//If target not already water
			//Find waterif (_currentTarget?.GetComponent<Bush>() == null)
			if (_currentTarget?.GetComponent<Water>() == null)
			{
				//Find water
				List<Collider> objectsSeen = Physics.OverlapSphere(transform.position, _sensoryDistance).Where(obj => obj.GetComponent<Water>() != null).ToList();

				if (objectsSeen.Count > 0)
				{
					_currentTarget = objectsSeen.FindNearest(transform.position).transform;

					_currentPath = AStar.FindPath(GraphHelper.CurrentGraph, transform.position, _currentTarget.position);
					_currentPathIndex = 1;
				}
				else
				{
					_currentTarget = null;
				}
			}
		}
		void FindMate()
		{

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

			switch (_currentAction)
			{
				case AvailableAction.None:
				case AvailableAction.Move:
					Move();
					break;
				case AvailableAction.Interact:
					InteractWithTarget();
					break;
			}
		}
	}

	void Move()
	{
		Vector3 prevPos = transform.position;

		switch (_currentAction)
		{
			case AvailableAction.None:
				transform.position = Wander();
				break;
			case AvailableAction.Move:
				transform.position = MoveTowardsTarget();
				break;
		}

		//Move should check map size
		Vector3 newDirection = transform.position - prevPos;
		newDirection.Normalize();

		_model.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
		
		Vector3 Wander()
		{
			return transform.position.Neighbors().Where((neighbor) => GraphHelper.CurrentGraph.IsInsideGraph(neighbor) && GraphHelper.CurrentGraph.GetNode(neighbor).walkable).ToList().GetRandomElement();
		}
		Vector3 MoveTowardsTarget()
		{
			if (_currentPathIndex < _currentPath.Count)
			{
				return transform.position = _currentPath[_currentPathIndex++];
			}
			else
			{
				return Wander();
			}
		}
	}

	void InteractWithTarget()
	{
		if (_currentTarget.GetComponent<Bush>() != null)
		{
			float amountEaten = _currentTarget.GetComponent<Bush>().Eat(_currentHunger);
			_currentHunger -= amountEaten;
		}
		else if (_currentTarget.GetComponent<Water>() != null)
		{
			float amountDrunk = _currentTarget.GetComponent<Water>().Drink(_currentThirst);
			_currentThirst -= amountDrunk;
		}

		_currentTarget = null;
	}

	public virtual List<CreatureUI.DataSet> GetData()
	{
		return new List<CreatureUI.DataSet>
		{
			new CreatureUI.DataSet<float>(){type = CreatureUI.UIType.Slider, color = Color.blue, name = "Thirst", getValue = ()=> {return _currentThirst / 100; } },
			new CreatureUI.DataSet<float>(){type = CreatureUI.UIType.Slider, color = Color.green, name = "Hunger", getValue = ()=> {return _currentHunger / 100; } },
			new CreatureUI.DataSet<float>(){type = CreatureUI.UIType.Slider, color = Color.cyan, name = "Mate", getValue = ()=> {return _matingDrive / 100; } },
		};
	}
}
