using UnityEngine;
using System;

[Serializable]
public class FuelTank
{
	[Tooltip("Type of fuel stored")]
	public FuelData Fuel;

	[Tooltip("Current units of fuel stored")]
	public float Stored;

	[Tooltip("Maximum units of fuel stored")]
	public float Capacity;

	public void Consume(float amount)
	{
		Stored -= amount;
	}
}

public class FuelData
{
	public string Name { get; private set; }

	public FuelType FuelType { get; private set; }

	//Power produced per unit fuel in kWh
	public float EnergyDensity { get; private set; }

	private FuelData(string name, FuelType type, float energyDensity)
	{
		Name = name;
		FuelType = type;
		EnergyDensity = energyDensity;
	}

	//static members 
	public static FuelData Methane = new FuelData("Methane", FuelType.Hydrocarbon, 1f);
	public static FuelData Acetylene = new FuelData("Acetylene", FuelType.Hydrocarbon, 2f);

	public static FuelData Plutonium = new FuelData("Plutonium", FuelType.Fisson, 5f);
	public static FuelData Uranium = new FuelData("Uranium", FuelType.Fisson, 7f);
	public static FuelData Thorium = new FuelData("Thorium", FuelType.Fisson, 8f);

	public static FuelData DeuteriumTritium = new FuelData("Deuterium-Tritium", FuelType.Fusion, 4f);
	public static FuelData Deuterium = new FuelData("Deuterium", FuelType.Fusion, 5f);
}

public enum FuelType
{
	Hydrocarbon,
	Fisson,
	Fusion,
}