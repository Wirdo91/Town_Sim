using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class PlantGenerator : MonoBehaviour, IGraphDataCollection
{
	[SerializeField]
	private float _initialPlants = 5;

	private Dictionary<Vector3, PlantBase> _currentPlants = new Dictionary<Vector3, PlantBase>();

	private WorldGenerator _world;

	public int plantCount = 0;

	private Dictionary<float, float> _treeCount = new Dictionary<float, float>();
	private Dictionary<float, float> _bushCount = new Dictionary<float, float>();

	public void Init(WorldGenerator world)
	{
		_world = world;
	}

	public void Generate(List<PlantBase> plants, List<Vector2> availableArea)
	{
		if (plants.Count > 0)
		{
			for (int i = 0; i < _initialPlants; i++)
			{
				Vector2 pos = availableArea.GetRandomElement();
				Vector3 realPos = new Vector3(pos.x, 0, pos.y);
				PlantPlant(realPos, plants.GetRandomElement());
			}
		}
	}

	private void PlantPlant(Vector3 position, PlantBase plantPrefab)
	{
		if (_currentPlants.ContainsKey(position) ||
			position.x < 0 || position.z < 0 ||
			position.x >= _world.ZoneMap.GetLength(0) || position.z >= _world.ZoneMap.GetLength(1) ||
			_world.ZoneMap[(int)position.x, (int)position.z] == null ||
			!_world.ZoneMap[(int)position.x, (int)position.z].availablePlants.Exists((p) => plantPrefab.GetType() == p.GetType()))
		{
			return;
		}

		PlantBase newPlant = Instantiate(plantPrefab, _world.plantParent);
		newPlant.transform.localPosition = position;
		newPlant.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
		//newPlant.gameObject.hideFlags = HideFlags.HideInHierarchy;
		plantCount++;

		_bushCount.SetOrAdd((float)TimeControl.instance.elapsedTime.TotalDays, _currentPlants.Values.Count((plant) => plant is Bush));
		_treeCount.SetOrAdd((float)TimeControl.instance.elapsedTime.TotalDays, _currentPlants.Values.Count((plant) => plant is Tree));
		_onDataChanged?.Invoke(this, "PlantType");
		_onDataChanged?.Invoke(this, "TreeCount");
		_onDataChanged?.Invoke(this, "BushCount");

		newPlant.Init(5);
		newPlant.onPlantReproduce += (plant) => Replant(plant.position, plant);
		newPlant.onPlantDeath += (plant) =>
		{
			if (_currentPlants.ContainsKey(plant.position))
			{
				_currentPlants.Remove(plant.position);
				Destroy(plant.gameObject);
			}
		};

		_currentPlants.Add(position, newPlant);
	}

	private void Replant(Vector3 origin, PlantBase plant)
	{
		PlantPlant(origin.Neighbors(plant.replantRange).GetRandomElement(), plant);
	}

	public List<string> GetIdentifiers(Action<IGraphDataCollection, string> onDataChanged)
	{
		_onDataChanged = onDataChanged;
		return new List<string> {
			"PlantType",
			"TreeCount",
			"BushCount"
		};
	}

	Action<IGraphDataCollection, string> _onDataChanged = default;

	public (GraphType gType, Dictionary<float, float> data) GetData(string identifier)
	{
		switch(identifier)
		{
			case "PlantType":
				return (GraphType.Bar, new Dictionary<float, float>
				{
					{0, _currentPlants.Values.Count((plant) => plant is Tree) },
					{1, _currentPlants.Values.Count((plant) => plant is Bush) },
				});
			case "TreeCount":
				return (GraphType.Line, _treeCount);
			case "BushCount":
				return (GraphType.Line, _bushCount);
		}
		return (GraphType.Bar, null);
	}
}
