using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GraphDataController : MonoBehaviour
{
	[SerializeField]
	private Toggle _identifierPrefab = default;

	[SerializeField]
	private GraphBase _barGraphPrefab = default;
	[SerializeField]
	private GraphBase _lineGraphPrefab = default;

	[SerializeField]
	private Transform _identifierToggleParent = default;
	[SerializeField]
	private Transform _graphParent = default;

	Dictionary<string, IGraphDataCollection> _graphIdentifiers = new Dictionary<string, IGraphDataCollection>();

	private Dictionary<string, GraphBase> _currentGraphs = new Dictionary<string, GraphBase>();

	private void Start()
	{
		Init();
	}

	private void Init()
	{
		List<IGraphDataCollection> graphDataCollections = FindObjectsOfType<MonoBehaviour>().OfType<IGraphDataCollection>().ToList();
		Debug.Log($"Found graph data {graphDataCollections.Count}");
		foreach (var item in graphDataCollections)
		{
			List<string> newIdentifiers = item.GetIdentifiers(OnDataChanged);

			foreach (var identifier in newIdentifiers)
			{
				if (!_graphIdentifiers.ContainsKey(identifier))
				{
					_graphIdentifiers.Add(identifier, item);
				}
				else
				{
					Debug.LogWarning($"Key {identifier} already used");
				}
			}
		}

		SetupIdentifierToggles();
	}

	private void SetupIdentifierToggles()
	{
		foreach (var item in _graphIdentifiers)
		{
			Toggle newToggle = Instantiate(_identifierPrefab, _identifierToggleParent);

			newToggle.GetComponentInChildren<Text>().text = item.Key;

			newToggle.onValueChanged.AddListener((toggled) =>
			{
				if (toggled)
				{
					if (!_currentGraphs.ContainsKey(item.Key))
					{
						GraphBase newGraph = Instantiate(GetGraphPrefab(item.Value.GetData(item.Key).gType), _graphParent);
						_currentGraphs.Add(item.Key, newGraph);
					}

					StartCoroutine(UpdateNextFrame());

					IEnumerator UpdateNextFrame()
					{
						yield return null;
						OnDataChanged(item.Value, item.Key);
					}
				}
				else
				{
					if (_currentGraphs.ContainsKey(item.Key))
					{
						Destroy(_currentGraphs[item.Key].gameObject);
						_currentGraphs.Remove(item.Key);
					}
				}
			});
		}
	}

	private GraphBase GetGraphPrefab(GraphType gType)
	{
		switch (gType)
		{
			case GraphType.Line:
				return _lineGraphPrefab;
			case GraphType.Bar:
				return _barGraphPrefab;
			default:
				return null;
		}
	}

	private void OnDataChanged(IGraphDataCollection owner, string identifier)
	{
		Debug.Log(identifier + " changed");
		if (_currentGraphs.ContainsKey(identifier))
		{
			_currentGraphs[identifier].SetAllDataPoints(owner.GetData(identifier).data);
		}
	}
}
