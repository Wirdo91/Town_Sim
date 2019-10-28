using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlantBase : MonoBehaviour
{
	[SerializeField]
	private Transform _model = default;
	[SerializeField]
	private float _replantChance = 0.3f;
	[SerializeField]
	private float _replantDelay = 1f;
	[SerializeField]
	private int _replantRange = 1;
	[SerializeField]
	private float _growthSpeed = 1f;

	public Vector3 position => transform.position;
	public int replantRange => _replantRange;

	public Action<PlantBase> onPlantDeath = default;
	public Action<PlantBase> onPlantReproduce = default;

	private float _growth = 0f;
	private float _replantTimer = 0f;

	private bool _alive => _growth > 0;

	public void Init(float initialGrowth)
	{
		_growth = initialGrowth;
		_model.localScale = Vector3.one * (_growth / 100f);
	}

	void Update()
    {
		if (!_alive)
		{
			return;
		}

		_model.localScale = Vector3.one * (_growth / 100f);

		if (_growth <= 0)
		{
			PlantDeath();
			return;
		}

		if (TimeControl.instance.isDayTime)
		{
			_growth += _growthSpeed * Time.deltaTime;
			_growth = Mathf.Clamp(_growth, 0, 100);
		}

		if (_growth >= 100)
		{
			if (_replantTimer > _replantDelay)
			{
				_replantTimer -= _replantDelay;

				if (Random.Range(0f, 1f) <= _replantChance)
				{
					onPlantReproduce?.Invoke(this);
				}
			}

			_replantTimer += Time.deltaTime;
		}
		else
		{
			_replantTimer = 0;
		}
	}

	private void PlantDeath()
	{
		Destroy(gameObject);
		
		onPlantDeath?.Invoke(this);
	}

	public float Eat(float amount)
	{
		float amountEaten = 0;
		if (amount > _growth)
		{
			amountEaten = _growth;
			_growth = 0;
		}
		else
		{
			amountEaten = amount;
			_growth -= amount;
		}
		return amountEaten;
	}
}
