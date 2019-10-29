using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CreatureGenerator : MonoBehaviour
{
	[SerializeField]
	private List<CreatureSpawn> _creatures = default;

	[SerializeField]
	private float _spawnDelay = 5f;

	private List<CreatureSpawn> _creaturesToSpawn = default;

	private WorldGenerator _world;

	public void Init(WorldGenerator world)
	{
		_world = world;
	}

	private void Start()
	{
		_creaturesToSpawn = _creatures;
		_creaturesToSpawn = _creaturesToSpawn.OrderByDescending((cs) => cs.timeForSpawn).ToList();
	}

	private void Update()
	{
		if (_world == null)
		{
			return;
		}

		if (_spawnDelay < 0 && _creaturesToSpawn.Count > 0)
		{
			for (int i = _creaturesToSpawn.Count - 1; i >= 0; i--)
			{
				if (TimeControl.instance.elapsedTime > _creaturesToSpawn[i].timeForSpawn)
				{
					for (int c = 0; c < _creaturesToSpawn[i].amountToSpawn; c++)
					{
						CreatureBase newCreature = Instantiate(_creaturesToSpawn[i].creaturePrefab, _world.creatureParent);

						newCreature.transform.localPosition = _world.MapZones[_creaturesToSpawn[i].zoneToSpawnIn].GetRandomElement().Convert();
					}

					_creaturesToSpawn.RemoveAt(i);
				}
				else
				{
					break;
				}
			}
		}
		else
		{
			_spawnDelay -= Time.deltaTime;
		}
	}

	[Serializable]
	public class CreatureSpawn
	{
		public CreatureBase creaturePrefab;
		public TimeSpan timeForSpawn;
		public int amountToSpawn;
		public Zone zoneToSpawnIn;
	}
}
