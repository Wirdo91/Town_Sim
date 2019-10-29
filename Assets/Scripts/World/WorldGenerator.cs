using Extensions;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;
using System.Diagnostics;

public class WorldGenerator : MonoBehaviour
{
	[SerializeField]
	private int _width = 10;
	[SerializeField]
	private int _height = 10;
	[SerializeField]
	private float _zoomLevel = 1f;
	[SerializeField]
	private Transform _plantParent = default;
	public Transform plantParent => _plantParent;
	[SerializeField]
	private Transform _creatureParent = default;
	public Transform creatureParent => _creatureParent;
	[SerializeField]
	private Transform _waterParent = default;

	[SerializeField]
	private Transform _worldCellPrefab = default;
	[SerializeField]
	private Water _waterPrefab = default;
	[SerializeField]
	private Zone _waterZone = default;

	[SerializeField]
	private float _zoneEdgeRange = 0.1f;
	[SerializeField, ReorderableList]
	private List<ZoneWeight> _zoneWeights = new List<ZoneWeight>();

	[SerializeField]
	private bool _generatePerlinNoiseMaps = false;

	[SerializeField]
	private PerlinNoiseSetting _primarySetting = default;
	[SerializeField]
	private List<PerlinNoiseSetting> _additionalSettings = new List<PerlinNoiseSetting>();

	[SerializeField]
	private bool _showGrid = false;
	
	private Graph _graph = default;
	public Graph graph => _graph;

	float[,] _speedModifierMap;
	public float[,] speedModifierMap => _speedModifierMap;

	private Zone[,] _zoneMap = default;
	public Zone[,] ZoneMap => _zoneMap;

	private Dictionary<Zone, List<Vector2>> _mapZones = new Dictionary<Zone, List<Vector2>>();
	public Dictionary<Zone, List<Vector2>> MapZones => _mapZones;

	private Texture2D _currentCellTexture = default;

	private bool _worldCreated = false;
	float min = float.MaxValue, max = float.MinValue;

	private void Start()
	{
		GetComponent<PlantGenerator>().Init(this);
		GetComponent<CreatureGenerator>().Init(this);

		GenerateWorld();
	}

	[Button("Start Units")]
	private void StartUnits()
	{
		if (!_worldCreated)
		{
			GenerateWorld();
			_worldCreated = true;
		}

		foreach (Unit unit in GameObject.FindObjectsOfType<Unit>())
		{
			unit.Init(this);
		}
	}

	[Button("Generate World")]
	private void GenerateWorld()
	{
		KillWorld();
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();

		InitWorldGenerator();

		GenerateMinMax();

		CalculateZoneRanges();

		GenerateMaps();

		GenerateGraph();

		foreach (Vector2 waterPos in MapZones[_waterZone])
		{
			foreach (Vector3 neighbor in waterPos.Convert().Neighbors())
			{
				if (((neighbor.x > 0 && neighbor.z > 0) && (neighbor.x < _zoneMap.GetLength(0) && neighbor.z < _zoneMap.GetLength(1))) && _zoneMap[(int) neighbor.x, (int) neighbor.z] != _waterZone)
				{
					Water water = Instantiate(_waterPrefab, _waterParent);
					water.transform.localPosition = neighbor;

					continue;
				}
			}
		}

		foreach (ZoneWeight zone in _zoneWeights)
		{
			if (_mapZones.ContainsKey(zone.zone))
			{
				GetComponent<PlantGenerator>().Generate(zone.zone.availablePlants, _mapZones[zone.zone]);
			}
		}

		stopwatch.Stop();
	}
	
	private void InitWorldGenerator()
	{
		Transform newCell = Instantiate(_worldCellPrefab, transform.position, Quaternion.Euler(0, 180, 0), transform);
		newCell.localPosition = new Vector3(_width / 2, 0, _height / 2);
		newCell.localScale = new Vector3(_width * 0.1f, 1, _height * 0.1f);
		_currentCellTexture = new Texture2D(_width, _height);
		_currentCellTexture.filterMode = FilterMode.Point; //Pixelated

		newCell.GetComponent<Renderer>().sharedMaterial.mainTexture = _currentCellTexture;

		_primarySetting.SetOffset();
		if (_generatePerlinNoiseMaps)
		{
			_primarySetting.GenerateVisualMap(_width, _height);
		}
		foreach (PerlinNoiseSetting setting in _additionalSettings)
		{
			setting.SetOffset();
			if (_generatePerlinNoiseMaps)
			{
				setting.GenerateVisualMap(_width, _height);
			}
		}
	}

	private void GenerateMinMax()
	{
		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				float xPos = ((float)x - (_width / 2)) / _width;
				float yPos = ((float)y - (_height / 2)) / _height;

				xPos /= _zoomLevel;
				yPos /= _zoomLevel;

				float sample = _primarySetting.GetSample(xPos, yPos);

				foreach (PerlinNoiseSetting setting in _additionalSettings)
				{
					sample += setting.GetSample(xPos, yPos);
				}

				min = Mathf.Min(min, sample);
				max = Mathf.Max(max, sample);
			}
		}
	}

	private void CalculateZoneRanges()
	{
		float tValue = 0;

		foreach (ZoneWeight setting in _zoneWeights)
		{
			tValue += setting.weight;
		}

		tValue += (_zoneWeights.Count - 1) * _zoneEdgeRange;

		_zoneEdgeRange = _zoneEdgeRange / tValue;

		for (int i = 0; i < _zoneWeights.Count; i++)
		{
			ZoneWeight setting = _zoneWeights[i];

			setting.normalizedWeight = setting.weight / tValue;

			setting.minRange = i <= 0 ? min : _zoneWeights[i - 1].maxRange + (_zoneEdgeRange * (max - min));
			setting.maxRange = i < _zoneWeights.Count - 1 ? setting.minRange + (setting.normalizedWeight * (max - min)) : max;

			_zoneWeights[i] = setting;
		}
	}

	private void GenerateMaps()
	{
		Color[] texColor = new Color[_width * _height];
		_speedModifierMap = new float[_width, _height];
		_zoneMap = new Zone[_width, _height];

		for (int x = 0; x < _width; x++)
		{
			for (int y = 0; y < _height; y++)
			{
				float xPos = ((float)x - (_width / 2)) / _width;
				float yPos = ((float)y - (_height / 2)) / _height;

				xPos /= _zoomLevel;
				yPos /= _zoomLevel;

				float sample = _primarySetting.GetSample(xPos, yPos);

				foreach (PerlinNoiseSetting setting in _additionalSettings)
				{
					sample += setting.GetSample(xPos, yPos);
				}

				bool colorSet = false;
				Color nodeColor = Color.magenta;
				float speedModifer = 0.2f;
				bool speedSet = false;

				for (int z = 0; z < _zoneWeights.Count; z++)
				{
					if (sample >= _zoneWeights[z].minRange && sample <= _zoneWeights[z].maxRange)
					{
						nodeColor = Color.Lerp(_zoneWeights[z].zone.minColor, _zoneWeights[z].zone.maxColor, sample.Normalized(_zoneWeights[z].minRange, _zoneWeights[z].maxRange));
						colorSet = true;

						float speedAt0 = z > 0 ? _zoneWeights[z - 1].zone.speedModifier : 0.2f;

						speedModifer = _zoneWeights[z].zone.speedModifier;
						speedSet = true;

						_zoneMap[x, y] = _zoneWeights[z].zone;
						if (!_mapZones.ContainsKey(_zoneWeights[z].zone))
						{
							_mapZones.Add(_zoneWeights[z].zone, new List<Vector2>());
						}
						_mapZones[_zoneWeights[z].zone].Add(new Vector2(x, y));
						break;
					}
					else if (z > 0 && sample >= _zoneWeights[z - 1].maxRange && sample <= _zoneWeights[z].minRange)
					{
						nodeColor = Color.Lerp(_zoneWeights[z - 1].zone.maxColor, _zoneWeights[z].zone.minColor, sample.Normalized(_zoneWeights[z - 1].maxRange, _zoneWeights[z].minRange).Round(1));
						colorSet = true;

						float speedAt0 = z > 0 ? _zoneWeights[z - 1].zone.speedModifier : 0.2f;

						speedModifer = Mathf.Lerp(speedAt0, _zoneWeights[z].zone.speedModifier, sample.Normalized(_zoneWeights[z - 1].maxRange, _zoneWeights[z].minRange).Round(1));
						speedSet = true;
						break;
					}
				}
				if (!speedSet)
				{
					Debug.Log($"Speed not set in zones");
				}
				if (!colorSet)
				{
					Debug.Log($"Color not set in zones");
				}

				texColor[(int)y * _width + (int)x] = nodeColor;
				_speedModifierMap[x, y] = speedModifer;
			}
		}

		_currentCellTexture.SetPixels(texColor);
		_currentCellTexture.Apply();
	}
	
	[Serializable]
	private class PerlinNoiseSetting
	{
		public bool randomOffset = false;
		public Vector2 texPerlinOffset = Vector2.zero;
		public float frequency = 0.5f;

		public float amplitude = 1f;

		[ShowNonSerializedField]
		private Texture2D _rawPerlin = default;

		public void GenerateVisualMap(int width, int height)
		{
			_rawPerlin = new Texture2D(width, height);
			Color[] perlinColor = new Color[width * height];

			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					if (x == width / 2 && y == height / 2)
					{
						Debug.Log($"Test grab at ({x}, {y}) = {GetSample(x, y) / amplitude}");
					}
					perlinColor[(int)y * width + (int)x] = Color.Lerp(Color.white, Color.black, GetSample(x, y) / amplitude);
				}
			}
			_rawPerlin.SetPixels(perlinColor);
			_rawPerlin.Apply();
		}

		public void SetOffset()
		{
			if (randomOffset)
			{
				texPerlinOffset = new Vector2(Random.Range(0f, 100f), Random.Range(0f, 100f));
			}
		}

		public float GetSample(float x, float y)
		{
			return Mathf.PerlinNoise(texPerlinOffset.x + x * frequency, texPerlinOffset.y + y * frequency) * amplitude;
		}
	}

	[Serializable]
	public class ZoneWeight
	{
		public Zone zone;

		public float weight = 1;

		[NonSerialized]
		public float normalizedWeight = 0;
		[NonSerialized]
		public float minRange = -1, maxRange = 1;
	}

	[Button("Kill Children")]
	private void KillWorld()
	{
		transform.KillChildren();
	}

	[Button("Generate Graph")]
	private void GenerateGraph()
	{
		//obstruction grid should reflect Water, Plants and Creatures
		_graph = GraphHelper.GenerateGraph(new bool[_width, _height], _speedModifierMap, Vector3.zero, Vector2.one, true);
	}

	[Button("Destroy Graph")]
	private void DestroyGraph()
	{
		_graph = null;
	}

	private void OnDrawGizmosSelected()
	{
		if (_graph == null || !_showGrid)
		{
			return;
		}

		foreach(Node node in _graph.GraphGrid)
		{
			foreach(Node connection in node.ConnectingNodes.Keys)
			{
				Gizmos.color = Color.Lerp(Color.green, Color.red, 1 - (1 / connection.traversalCost));
				Gizmos.DrawLine(node._graphPos, connection._graphPos);
			}
		}
	}
}
