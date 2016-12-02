using UnityEngine;

/* TRANSMISSION CLASS
 * Manages rpm for motor, changes gears and passes torque from motor to diffs.
 */

[System.Serializable]
public class Transmission
{
	// rpm
	[HideInInspector]
	public float toMotorRpm; // controlls motor

	[Tooltip("At this RPM motor wants to shift up a gear. This number dynamically increases with load.")]
	public int shiftUpRpm = 2700;

	[Tooltip("At this RPM motor wants to shift down a gear.")]
	public int shiftDownRpm = 1400;

	// gears
	[Tooltip("Gear ratios for each gear. First element is reverse and should be < 0, second element is N, element 3 is 1st gear and so on.")]
	public float[] gearRatios = new float[3] { -6, 0, 7 };

	[Tooltip("All other gear rations are multiplied by it. Try 1-10. Higher the number slower vehicle but more torque.")]
	public float finalGearRatio = 3.7f; //Simulates both final gear and differential ratio, = final gear ratio * diff. ratio

	[HideInInspector]
	public int gear;

	// shift duration
	[Tooltip("Time(s) it takes driver to shift. No torque is delivered to wheels while shifting. Try 0.3-0.8.")]
	public float shiftDuration = 0.2f;

	[HideInInspector]
	private float shiftTimer;

	[HideInInspector]
	public bool isShifting;

	// shift disable after each shift
	[Tooltip("Time(s) from start of shift until next shift is allowed. Can prevent gear hunting. Try 0.3-1.")]
	public float shiftDisableDuration = 0.5f;

	[HideInInspector]
	public float disableTimer;

	[HideInInspector]
	public bool isDisabled;

	// torque
	[HideInInspector]
	public float fromMotorTorque;

	[HideInInspector]
	public float toWheelTorque; // to wheels

	public void Update(WheeledVehicle vehicle)
	{
		float rpmSum = 0;
		foreach (AxleInfo axleInfo in vehicle.axleInfos)
		{
			if (axleInfo.motor)
			{
				rpmSum += axleInfo.axleRpm;
			}
		}

		toMotorRpm = ((rpmSum / vehicle.motorAxleCount)) * gearRatios[gear] * finalGearRatio;

		// Get torque to wheels, if allowed
		if ((toMotorRpm > vehicle.motor.maxRpm && gear == gearRatios.Length)
		   || isShifting
		   || (Mathf.Abs(toMotorRpm) > vehicle.motor.maxRpm && gear == 0))
		{
			toWheelTorque = 0;
		}
		else
		{
			toWheelTorque = fromMotorTorque * finalGearRatio * gearRatios[gear];
		}

		#region ShiftingLogic

		if (isShifting)
		{ // shift delay
			shiftTimer += Time.deltaTime;
			isDisabled = true;
			if (shiftTimer > shiftDuration)
			{
				isShifting = false;
			}
		}
		else
		{
			shiftTimer = 0;
		}

		if (isDisabled)
		{ // disable shifting in too close intervals
			disableTimer += Time.deltaTime;
			if (disableTimer > shiftDisableDuration)
			{
				isDisabled = false;
			}
		}
		else
		{
			disableTimer = 0;
		}

		// if accelerating in reverse without user input, e.g. downhill
		if (vehicle.throttleDirection == 0 && vehicle.velocity < 0)
		{
			gear = 0;
			isShifting = true;
		}

		// in gear
		if (gear >= 2)
		{
			if (vehicle.throttleDirection < 0 && vehicle.velocity < 0.4f)
			{
				gear = 0;
				isShifting = true;
			}
		}
		// neutral
		else if (gear == 1)
		{
			if (vehicle.throttleDirection > 0)
			{
				isShifting = ShiftUp();
			}
			else if (vehicle.throttleDirection < 0)
			{
				isShifting = ShiftDown();
			}
		}
		// reverse
		else if (gear == 0)
		{
			if (vehicle.throttleDirection > 0)
			{
				isShifting = ShiftUp();
			}
		}

		// shift to neutral
		if (gear != 1 && toMotorRpm < 400 && vehicle.throttleDirection == 0)
		{
			gear = 1;
			isShifting = true;
		}

		// shift from neutral to reverse
		if (gear == 1 && vehicle.throttleDirection < 0 && vehicle.velocity < 1)
		{
			gear = 0;
			isShifting = true;
		}

		#endregion ShiftingLogic
	}

	public bool ShiftUp()
	{
		if (!isShifting && !isDisabled && gear < gearRatios.Length - 1)
		{
			gear++;
			return true;
		}
		return false;
	}

	public bool ShiftDown()
	{
		if (!isShifting && !isDisabled && gear > 2)
		{
			gear--;
			return true;
		}
		return false;
	}
}