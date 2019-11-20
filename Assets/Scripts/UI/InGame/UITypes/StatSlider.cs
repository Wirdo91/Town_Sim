using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CreatureUI;

public class StatSlider : MonoBehaviour
{
	[SerializeField]
	private Slider _slider = default;
	[SerializeField]
	private Text _name = default;

	private DataSet<float> _data;

	public void Init(DataSet<float> data)
	{
		_data = data;

		_name.text = _data.name;

		_slider.colors = new ColorBlock() { normalColor = data.color, disabledColor = data.color }; 
	}

	private void Update()
	{
		_slider.value = _data.getValue();
	}
}
