using System.Collections.Generic;
using UnityEngine;
using Extensions;

public class LineGraph : GraphBase
{
	[SerializeField]
	private LineRenderer _lineRenderer = default;
	[SerializeField]
	private float _lineThickness = 0.5f;

	protected override void RefreshView()
	{
		_lineRenderer.startWidth = _lineThickness / lineRendererWidth;
		_lineRenderer.endWidth = _lineThickness / lineRendererWidth;

		List<Vector3> points = new List<Vector3>();
		foreach (var item in _graphData)
		{
			if (item.Key < minTimeStamp || item.Key > maxTimeStamp)
			{
				continue;
			}

			Vector3 newPoint = Vector3.zero;

			newPoint.x = item.Key.Normalized(minTimeStamp, maxTimeStamp) * _graphArea.rect.width;
			newPoint.y = item.Value.Normalized(minValue, maxValue) * _graphArea.rect.height;

			points.Add(newPoint);
		}
		_lineRenderer.positionCount = points.Count;
		_lineRenderer.SetPositions(points.ToArray());
	}

	public override void AddOrSetNewDataPoint(float timeStamp, float value)
	{
		_graphData.Add(timeStamp, value);

		Refresh();
	}
}
