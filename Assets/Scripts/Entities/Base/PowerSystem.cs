using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PowerSystem : MonoBehaviour
{
	private const float priorityPowerDemand = 2f;
	private const float priorityEffect = 1.5f;
	private const float depriorityPowerDemand = 0.3f;
	private const float depriorityEffect = 0.75f;

	public FuelTank FuelTank;

	public Powerplant Powerplant;

	public BatteryBank BatteryBank;

	public ConsumerType? Prioritise;
	public ConsumerType? Deprioritise;

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

		//supply / demand
		var maxPowerGeneration = Powerplant.MaxOutput(FuelTank.Fuel);
		var maxDraw = BatteryBank.Stored > 0 ? BatteryBank.MaxDraw : 0;

		var supply = maxPowerGeneration + maxDraw;
		var fullSupply = supply;

		var consumers = GetComponentsInChildren<IPowerConsumer>();
		var consumerDict = consumers.ToDictionary(c => c, c => c.GetPowerDemand());

		var demand = consumerDict.Values.Sum();

		//priority power
		var prioritisedSatisfaction = 1f;
		if (Prioritise != null)
		{
			var prioritisedDemand = consumers.Where(c => c.GetConsumerType() == Prioritise).Sum(c => consumerDict[c]);
			var realPrioritisedDemand = prioritisedDemand * priorityPowerDemand;
			var prioritisedSupply = Mathf.Min(supply, realPrioritisedDemand);

			supply -= prioritisedSupply;
			demand -= prioritisedDemand;

			if (realPrioritisedDemand > 0)
			{
				prioritisedSatisfaction = prioritisedSupply / realPrioritisedDemand * priorityEffect;
			}
		}

		//depriority power
		var deprioritisedSatisfaction = 1f;
		if (Deprioritise != null)
		{
			var deprioritisedDemand = consumers.Where(c => c.GetConsumerType() == Deprioritise).Sum(c => consumerDict[c]);
			var realDeprioritisedDemand = deprioritisedDemand * depriorityPowerDemand;
			var deprioritisedSupply = Mathf.Min(supply, realDeprioritisedDemand);

			supply -= deprioritisedSupply;
			demand -= deprioritisedDemand;

			if (realDeprioritisedDemand > 0)
			{
				deprioritisedSatisfaction = deprioritisedSupply / realDeprioritisedDemand * depriorityEffect;
			}
		}

		//regular power
		var satisfaction = 1f;
		if (supply < demand)
		{
			satisfaction = supply / demand;
		}

		//use power
		powerUsed = 0f;
		foreach (var consumer in consumers)
		{
			if (Prioritise != null && consumer.GetConsumerType() == Prioritise)
			{
				consumer.SupplyPower(prioritisedSatisfaction);
				powerUsed += prioritisedSatisfaction * consumerDict[consumer];
			}
			else if (Deprioritise != null && consumer.GetConsumerType() == Deprioritise)
			{
				consumer.SupplyPower(deprioritisedSatisfaction);
				powerUsed += deprioritisedSatisfaction * consumerDict[consumer];
			}
			else
			{
				consumer.SupplyPower(satisfaction);
				powerUsed += satisfaction * consumerDict[consumer];
			}
		}

		//draw/charge batteries
		if (powerUsed < maxPowerGeneration)
		{
			var surplus = maxPowerGeneration - powerUsed;
			charge = Mathf.Min(surplus, BatteryBank.MaxCharge);
			BatteryBank.Charge(charge);
			powerUsed += charge;
		}
		else if (powerUsed < fullSupply)
		{
			draw = powerUsed - maxPowerGeneration;
			BatteryBank.Draw(draw);
		}
		else
		{
			//what
			Debug.LogError("Used more power than supplied! " + powerUsed + "/" + fullSupply);
		}

		//use fuel
		FuelTank.Consume(Powerplant.FuelUsageFromPower(FuelTank.Fuel, powerUsed));
	}

	public void OnGUI()
	{
		//debug
		if (GUI.Button(new Rect(10, 100, 70, 20), ConsumerType.Defense.ToString()))
		{
			Prioritise = ConsumerType.Defense;
		}
		if (GUI.Button(new Rect(10, 130, 70, 20), ConsumerType.Offense.ToString()))
		{
			Prioritise = ConsumerType.Offense;
		}
		if (GUI.Button(new Rect(10, 160, 70, 20), ConsumerType.Mobility.ToString()))
		{
			Prioritise = ConsumerType.Mobility;
		}
	}
}