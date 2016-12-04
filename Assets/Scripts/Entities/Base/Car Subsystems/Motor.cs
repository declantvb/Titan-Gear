using UnityEngine;

/* MOTOR CLASS
 * Takes care of generating appropriate torque and power for the given rpm.
 * Does not control RPM - RPM controls it.
 */

[System.Serializable]
public class Motor
{
	public float maxRpm = 4200;
	
	public float currentRpm;
	
	public float load; //0...1

	public float breakRPM = 6000f;

	public float maxPower = 100f;

	public float CurrentPowerLevel;

	public void Update(WheeledVehicle vehicle)
	{
		//motor can only directly communicate with transmission
		currentRpm = Mathf.Abs(vehicle.transmission.toMotorRpm);

		// Update torque based on motor rpm and power
		vehicle.transmission.fromMotorTorque = getPowerAtRpm(currentRpm) * 9549 / (currentRpm + 1); // torque(Nm) = 9549*Power(kW)/Speed(RPM)

		// Calculate load based on input and speed feedback
		float speedDiff = 0.2f - (vehicle.speed - vehicle.previousSpeed) * (1 - (currentRpm / maxRpm));
		load = Mathf.Clamp(speedDiff * 5, 0.0f, 1.0f) * vehicle.GetThrottle();
		if (load < 0) load = 0;

		if (vehicle.throttleDirection != 0)
		{
			if (speedDiff > 0)
			{
				//regulates perception of load. Divide speed whith larger number to get longer "clutch"
				currentRpm += load * (2000 / ((vehicle.speed * vehicle.speed) + 2));
			}
		}

		//if (currentRpm > maxRpm)
		//{
		//	currentRpm = maxRpm;
		//}
	}

	public float GetCurrentPower()
	{
		return getPowerAtRpm(currentRpm);
	}

	// Interpolate between values based on powerArray as y and rpm as x axis
	private float getPowerAtRpm(float rpm)
	{
		var modifiedBreakRPM = breakRPM / CurrentPowerLevel;
		var modifiedMaxPower = maxPower * CurrentPowerLevel;

		if (rpm < modifiedBreakRPM)
		{
			return modifiedMaxPower * rpm / modifiedBreakRPM;
		}

		return modifiedMaxPower;
	}
}