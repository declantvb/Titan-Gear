using UnityEngine;

/* MOTOR CLASS
 * Takes care of generating appropriate torque and power for the given rpm.
 * Does not control RPM - RPM controls it.
 */

[System.Serializable]
public class Motor
{
	// RPM
	[Tooltip("Maximum RPM motor is allowed to have.")]
	public float maxRpm = 4200;

	//[HideInInspector]
	public float currentRpm;

	[Tooltip("Minimum RPM motor is allowed to have.")]
	public float minRpm = 0;

	//[HideInInspector]
	public float load; //0...1

	[Tooltip("Array of different power values in kW at steps of 1000rpm. Torque curve is generated from this. Change step in getPowerAtRpm function for smaller or bigger steps.")]
	public float[] powerArray = new float[7] { 0, 30, 60, 80, 95, 105, 100 }; //represents power(kW) at steps of 1k rpm

	public void Update(WheeledVehicle vehicle)
	{
		//motor can only directly communicate with transmission
		currentRpm = Mathf.Abs(vehicle.transmission.toMotorRpm);

		// rpm limiter
		currentRpm = Mathf.Clamp(currentRpm, minRpm, maxRpm);

		// Update torque based on motor rpm and power
		vehicle.transmission.fromMotorTorque = getPowerAtRpm(powerArray, currentRpm) * 9549 / (currentRpm + 1); // torque(Nm) = 9549*Power(kW)/Speed(RPM)

		// Rev control between shifts
		if (vehicle.transmission.isShifting)
		{
			currentRpm = minRpm;
		}

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

		if (currentRpm > maxRpm)
		{
			currentRpm = maxRpm;
		}
	}

	// Interpolate between values based on powerArray as y and rpm as x axis
	private float getPowerAtRpm(float[] powerArray, float rpm)
	{
		int selector;
		int step = 1000;
		float x_axis;

		selector = (int)(Mathf.Floor(rpm / step));
		x_axis = currentRpm % step;
		return powerArray[selector] + ((x_axis / step) * powerArray[selector + 1]);
	}
}