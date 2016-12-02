using UnityEngine;
using System;

[Serializable]
public class BatteryBank
{
	[Tooltip("Type of battery used")]
	public BatteryData Battery;

	[Tooltip("Number of batteries in the bank")]
	public int BatteryCount;

	[Tooltip("Maximum draw in W")]
	public float PowerDensity;

	[Tooltip("Current units of power stored")]
	public float Stored;

	[Tooltip("Maximum units of power stored")]
	public float Capacity;

	public void ChangeBatteryType(BatteryData newType)
	{
		Battery = newType;
		PowerDensity = Battery.PowerDensity * BatteryCount;
		Capacity = Battery.EnergyDensity * BatteryCount;
	}
}

public class BatteryData
{
	public string Name { get; private set; }

	//Power stored per battery in Wh
	public float EnergyDensity { get; private set; }

	//Maximum draw in W
	public float PowerDensity { get; private set; }

	private BatteryData(string name, float energyDensity, float powerDensity)
	{
		Name = name;
		EnergyDensity = energyDensity;
		PowerDensity = powerDensity;
	}

	//static members 
	public static BatteryData LithiumIon = new BatteryData("Lithium-Ion", 4f, 1f);
	public static BatteryData SolidState = new BatteryData("Solid State Lithium-Ion", 4f, 2f);
	public static BatteryData Supercapacitor = new BatteryData("Supercapacitor", 1f, 5f);
	public static BatteryData LithiumSulfur = new BatteryData("Lithium-Sulfur", 3f, 4f);
	public static BatteryData SurfaceMediatedCell = new BatteryData("Surface Mediated Cell", 3f, 5f);
}