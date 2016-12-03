using UnityEngine;
using System;

[Serializable]
public class Capacitor
{
	public float Capacity { get; private set; }

	public float Demand { get; private set; }

	public float Stored { get; private set; }

	public Capacitor(float capacity)
	{
		Capacity = capacity;
		Demand = 0;
		Stored = 0;
	}
}