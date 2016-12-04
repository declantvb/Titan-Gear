using UnityEngine;
using System;

[Serializable]
public class Capacitor : MonoBehaviour, IPowerConsumer
{
	public ConsumerType ConsumerType;

	public float Capacity;

	public float Stored;

	public float MaxChargeOrDraw = 150f;

	public Capacitor(float capacity, float maxChargeOrDraw)
	{
		Capacity = capacity;
		MaxChargeOrDraw = maxChargeOrDraw;
		Stored = 0;
	}

	public float GetPowerDemand()
	{
		return Stored < Capacity ? MaxChargeOrDraw : 0;
	}

	public void SupplyPower(float satisfaction)
	{
		Stored += MaxChargeOrDraw * TimeExtensions.deltaTimeHours * satisfaction;
		if (Stored > Capacity)
		{
			Stored = Capacity;
		}
	}

	public ConsumerType GetConsumerType()
	{
		return ConsumerType;
	}
}