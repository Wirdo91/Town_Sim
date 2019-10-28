using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GraphType
{
	Line,
	Bar
}

[RequireComponent(typeof(RectTransform))]
public abstract class GraphBase : MonoBehaviour
{
	[SerializeField] private GraphBackground _background = default;
	[SerializeField]
	protected RectTransform _graphArea = default;

	protected Dictionary<float, float> _graphData = new Dictionary<float, float>();

	[SerializeField]
	protected float minTimeStamp = 0;
	[SerializeField]
	protected float maxTimeStamp = 100f;
	[SerializeField]
	protected float minValue = 0;
	[SerializeField]
	protected float maxValue = 100;

	protected float valueStep = 10f;
	protected float timeStep = 10f;

	protected const float lineRendererWidth = 110;

	private RectTransform _rectTransform;
	protected RectTransform rectTransform => _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());

	protected void UpdateBackground()
	{

	}

#if UNITY_EDITOR
	[SerializeField]
	private List<Vector2> _testData = default;

	[Button("Test Data")]
	public void TestData()
	{
		Clear();

		foreach (var item in _testData)
		{
			_graphData.Add(item.x, item.y);
		}

		Refresh();
	}
#endif

	[Button("Clear")]
	public virtual void Clear()
	{
		_graphData.Clear();
	}

	[Button("Refresh Graph")]
	public void Refresh()
	{
		_background?.Refresh();
		RefreshView();
	}

	public abstract void AddOrSetNewDataPoint(float val1, float val2);

	public virtual void SetAllDataPoints(Dictionary<float, float> data)
	{
		_graphData = data;

		Refresh();
	}

	protected abstract void RefreshView();
}
