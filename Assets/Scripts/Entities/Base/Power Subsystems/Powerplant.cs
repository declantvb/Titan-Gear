using UnityEngine;
using System;

[Serializable]
public class Powerplant
{
	[Tooltip("Maximum fuel usage per second in units")]
	public float FuelUsage;

	[Tooltip("Percentage of fuel energy converted to power, 0..1")]
	public float Efficiency;

	//Max power production given fuel, in kW
	public float MaxOutput(FuelData fuel)
	{
		// kWh/unit * unit/s * s/h
		var watts = fuel.EnergyDensity * FuelUsage * 3600;
		return watts / 1000;
	}

	public float FuelUsageFromPower(FuelData fuel, float power)
	{
		var partHoursPerFrame = Time.deltaTime / 3600;
		return power / fuel.EnergyDensity / Efficiency * partHoursPerFrame;
	}
}