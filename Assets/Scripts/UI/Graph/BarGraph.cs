using Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarGraph : GraphBase
{
	[SerializeField]
	private LineRenderer _lineRendererPrefab = default;
	
	private List<LineRenderer> _lineRendererInstances = new List<LineRenderer>();

	public override void Clear()
	{
		base.Clear();
		foreach(LineRenderer line in _lineRendererInstances)
		{
			if (line != null)
			{
				DestroyImmediate(line.gameObject);
			}
		}
		_lineRendererInstances.Clear();
	}

	protected override void RefreshView()
	{
		int maxIndex = 0;
		float maxValue = 0;
		foreach (var dataPoint in _graphData)
		{
			if (dataPoint.Key > maxIndex)
			{
				maxIndex = (int)dataPoint.Key;
			}
			if (dataPoint.Value > maxValue)
			{
				maxValue = dataPoint.Value;
			}
		}

		float lineThickness = _graphArea.rect.width / (maxIndex + 1);
		Debug.Log($"Area {_graphArea.rect.width} mIndex {maxIndex} Line {lineThickness}");

		while (_lineRendererInstances.Count <= maxIndex)
		{
			LineRenderer newBar = Instantiate(_lineRendererPrefab, _graphArea);
			_lineRendererInstances.Add(newBar);
		}

		foreach (LineRenderer line in _lineRendererInstances)
		{
			line.gameObject.SetActive(false);
		}
		
		foreach (var dataPoint in _graphData)
		{
			LineRenderer line = _lineRendererInstances[(int)dataPoint.Key];

			line.gameObject.SetActive(true);

			float x = (lineThickness / 2) + (dataPoint.Key.Normalized(0, maxIndex) * lineThickness);

			line.startWidth = lineThickness / lineRendererWidth;
			line.endWidth = lineThickness / lineRendererWidth;

			line.SetPosition(0, Vector3.right * x);
			line.SetPosition(1, new Vector3(x, dataPoint.Value.Normalized(0, maxValue) * _graphArea.rect.height));
		}
	}

	public override void AddOrSetNewDataPoint(float barIndex, float value)
	{
		int index = (int)barIndex;
		if (!_graphData.ContainsKey(index))
		{
			_graphData.Add(index, 0);
		}
		_graphData[index] = value;
		
		Refresh();
	}
}
