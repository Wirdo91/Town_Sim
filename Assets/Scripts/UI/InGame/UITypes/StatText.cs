using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CreatureUI;

public class StatText : MonoBehaviour
{
	[SerializeField]
	private Text _value = default;
	[SerializeField]
	private Text _name = default;

	private DataSet<string> _data;

	public void Init(DataSet<string> data)
	{
		_data = data;

		_name.text = _data.name;
		_value.color = _data.color;
	}

	private void Update()
	{
		_value.text = _data.getValue();
	}
}
