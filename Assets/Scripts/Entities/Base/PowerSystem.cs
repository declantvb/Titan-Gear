using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerSystem : MonoBehaviour
{
	public FuelTank FuelTank;

	public Powerplant Powerplant;

	public BatteryBank BatteryBank;
	
	public GameObject FuelSlider;
	public GameObject BatterySlider;

	public float draw;
	public float charge;
	public float powerUsed;

	private void Start()
	{
		FuelTank.Fuel = FuelData.Methane;
	}

	private void Update()
	{
		//debug
		draw = 0;
		charge = 0;
		powerUsed = 0;

		var consumers = GetComponentsInChildren<IPowerConsumer>();

		var consumerDict = consumers.ToDictionary(c => c, c => c.GetPowerDemand());

		var demand = consumerDict.Values.Sum();

		var maxPowerGeneration = Powerplant.MaxOutput(FuelTank.Fuel);
		var possibleSupply = maxPowerGeneration + BatteryBank.MaxDraw;

		var satisfaction = 1f;
		if (possibleSupply < demand)
		{
			satisfaction = possibleSupply / demand;
		}

		powerUsed = 0f;
		foreach (var consumer in consumers)
		{
			consumer.SupplyPower(satisfaction);
			powerUsed += satisfaction * consumerDict[consumer];
		}

		if (powerUsed < maxPowerGeneration)
		{
			var surplus = maxPowerGeneration - powerUsed;
			charge = Mathf.Min(surplus, BatteryBank.MaxCharge);
			BatteryBank.Charge(charge);
			powerUsed += charge;
		}
		else if (powerUsed < possibleSupply)
		{
			draw = powerUsed - maxPowerGeneration;
			BatteryBank.Draw(draw);
		}
		else
		{
			//what
		}

		FuelTank.Consume(Powerplant.FuelUsageFromPower(FuelTank.Fuel, powerUsed));

		if (FuelSlider != null)
		{
			FuelSlider.GetComponent<Slider>().value = FuelTank.Stored / FuelTank.Capacity;
		}

		if (BatterySlider != null)
		{
			BatterySlider.GetComponent<Slider>().value = BatteryBank.Stored / BatteryBank.Capacity;
		}
	}
}