using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Extensions;
using NaughtyAttributes;

public class UnitStat
{
	public const float BASEMINVALUE = 0;
	public const float BASEMAXVALUE = 100;
	
	public UnitStat (float initialValue)
	{
		value = initialValue;
	}

	private float _value = BASEMINVALUE;
	public float value
	{
		get { return _value; }
		set
		{
			_value = Mathf.Clamp(value, BASEMINVALUE, BASEMAXVALUE);
		}
	}

	public static implicit operator UnitStat(float v)
	{
		return new UnitStat(v);
	}

	public static implicit operator float(UnitStat v)
	{
		return v.value;
	}
}

[RequireComponent(typeof(Movement))]
public class Unit : MonoBehaviour
{
	public enum Sex
	{
		MALE,
		FEMALE
	}

	private Sex _sex = Sex.MALE;
	private UnitStat _hunger = UnitStat.BASEMINVALUE;//Start lowering health at 100, decrease with eating
	private UnitStat _health = UnitStat.BASEMAXVALUE; //Dead at 0, regain with sleep, eating, healing
	private UnitStat _energy = UnitStat.BASEMAXVALUE; //Exhausted at 0, regain with sleep or relaxing

	[SerializeField]
	private WorldGenerator _world;

	private float _energyUsageModifier = 1f;
	
	private Movement _movement;

	private void Awake()
	{
		_movement = GetComponent<Movement>();
	}

	public void Init(WorldGenerator world)
	{
		_sex = ((Sex[])Enum.GetValues(typeof(Sex))).ToList().GetRandomElement();
		_movement.Init(world.graph);
		_movement.SetState(Movement.State.WANDER);
	}

    // Update is called once per frame
    void Update()
	{
		//Hunger
		_hunger += Time.deltaTime; //Speed should be dependent on activity (Same with energy)
		
		if (_hunger >= UnitStat.BASEMAXVALUE)
		{
			_health -= Time.deltaTime;
		}

		//Health
		if(_health <= UnitStat.BASEMINVALUE)
		{
			Die();
		}

		//Energy
		_energy -= Time.deltaTime * _energyUsageModifier;
		_movement.movementSpeedModifer = Mathf.Min(Mathf.Max(_energy / UnitStat.BASEMAXVALUE, 0.2f), 1f);

	}

	void Die()
	{
		_movement.SetState(Movement.State.NONE);
	}
}
