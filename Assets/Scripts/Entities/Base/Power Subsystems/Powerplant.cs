using UnityEngine;
using System;

[Serializable]
public class Powerplant
{
	[Tooltip("Fuel usage per second in units")]
	public float FuelUsage;

	[Tooltip("Percentage of fuel energy converted to power, 0..1")]
	public float Efficiency;
}