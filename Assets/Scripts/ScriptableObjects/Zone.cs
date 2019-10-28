using System.Collections.Generic;
using UnityEngine;

public class Zone : ScriptableObject
{
	public Color minColor = Color.white;
	public Color maxColor = Color.black;

	public List<PlantBase> availablePlants;

	public float speedModifier = 1f;
}
