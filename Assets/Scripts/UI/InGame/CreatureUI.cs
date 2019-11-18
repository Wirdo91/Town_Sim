using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CreatureUIData))]
public class CreatureUI : MonoBehaviour
{
	[SerializeField]
	private VerticalLayoutGroup _targetTransform = default;
	[SerializeField]
	private StatSlider _sliderPrefab = default;
	[SerializeField]
	private StatText _labelPrefab = default;

	private CreatureUIData _dataSupplier;

	private void Awake()
	{
		_dataSupplier = GetComponentInParent<CreatureUIData>();
	}

	private void Start()
	{
		foreach (var dataSet in _dataSupplier.GetData())
		{
			switch (dataSet.type)
			{
				case UIType.Slider:
					StatSlider newSlider = Instantiate(_sliderPrefab, _targetTransform.transform);
					newSlider.Init((DataSet<float>)dataSet);
					break;
				case UIType.Text:
					StatText newText = Instantiate(_labelPrefab, _targetTransform.transform);
					newText.Init((DataSet<string>)dataSet);
					break;
				default:
					Debug.LogError("Shit's broken");
					break;
			}
		}
	}

	public enum UIType
	{
		Slider,
		Text
	}

	public class DataSet
	{
		public UIType type = UIType.Slider;
		public string name = "";
		public Color color = Color.black;
	}
	public class DataSet<T> : DataSet
	{
		public Func<T> getValue;
	}

	public interface CreatureUIData
	{
		List<DataSet> GetData();
	}
}
