using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// adapted from Off-road Vehicle Physics Kit https://www.assetstore.unity3d.com/en/#!/content/39946
public class WheeledVehicle : MonoBehaviour
{
	// Objects
	[Tooltip("List of axles and wheels a vehicle has. Up to 10 axles, 2 wheels each.")]
	public List<AxleInfo> axleInfos;

	public Motor motor;
	public Transmission transmission;

	// Brakes
	[Tooltip("Maximum system braking torque to the wheels when braking is applied. Divided between wheels.")]
	public float maxBrakeTorque = 30000;

	// Debugs
	[HideInInspector]
	public float totalTorque; //sum of all wheel torques, can be used as control variable

	// Speed
	[HideInInspector]
	public float velocity;

	/* velocity is in m/s, multiply by 3.6 for kmh
	 * don't modify directly beacause all physics in this script is calculated in m/s
	 */

	[HideInInspector]
	public float speed; // abs(velocity)

	[HideInInspector]
	public float previousSpeed; // load sensing

	[HideInInspector]
	public int direction { get { return velocity >= 0 ? 1 : -1; } }

	// Steer
	[Tooltip("Reduces maxSteeringAngle at higher speeds for more stable vehicle. Try 3-20.")]
	public float speedSensitiveSteering = 7; //larger = more influence on steering at speed

	[Tooltip("Maximum angle in degrees a wheel can turn.")]
	public float maxSteeringAngle = 42; // maximum wheel steer angle

	[HideInInspector]
	public float maxAvailableTorque;

	[HideInInspector]
	public bool wheelSlip; //if any of the weels have slipped, dont upshift

	[HideInInspector]
	public int motorAxleCount;

	// defaults
	public Func<float> GetSteering = () => 0;
	public Func<float> GetThrottle = () => 0;
	public Func<float> GetBrake = () => 0;

	[HideInInspector]
	public int throttleDirection { get { return GetThrottle() >= 0 ? 1 : -1; } }

	[HideInInspector]
	public bool brake { get { return GetBrake() > 0 || (Mathf.Abs(GetThrottle()) > 0.2f && throttleDirection != direction && speed > 0.2f); } }

	private Rigidbody rb;

	private void Start()
	{
		rb = GetComponentInParent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		motorAxleCount = countPoweredAxles(); //remove this line if you won't change number of powered axles during gameplay

		// Update different classes
		transmission.Update(this);
		motor.Update(this);

		previousSpeed = speed; //for load calculation
		velocity = transform.InverseTransformDirection(rb.velocity).z; // -...+
		speed = Mathf.Abs(velocity); // 0...+

		//reset variables
		totalTorque = 0;
		wheelSlip = false;

		if (transmission.gear != 0)
		{ // reverse gear neds reverse inputs
			maxAvailableTorque = transmission.toWheelTorque * GetThrottle();
		}
		else
		{
			maxAvailableTorque = transmission.toWheelTorque * -GetThrottle();
		}

		// get axle speed for final speed calculation
		foreach (AxleInfo axleInfo in axleInfos)
		{
			axleInfo.axleRpm = (Mathf.Abs(axleInfo.leftWheel.collider.rpm) + Mathf.Abs(axleInfo.rightWheel.collider.rpm)) / 2;
		}

		float[] torqueArr = CalculatePerAxleTorqueDistribution();

		CalculateSteering(GetSteering());

		var index = 0;
		foreach (AxleInfo axleInfo in axleInfos)
		{
			if (axleInfo.motor) axleInfo.torque = torqueArr[index];
			index++;

			CalculateTorqueDistribution(axleInfo);
			CalculateSlips(axleInfo);
			CalculateBrakeTorques(axleInfo);
		}

		UpdateColliders();

		//Update rotation and position of wheels
		UpdateWheelVisuals(axleInfos);
	}

	private void CalculateSteering(float steering)
	{
		foreach (var axle in axleInfos)
		{
			axle.centre = (axle.leftWheel.collider.transform.position + axle.rightWheel.collider.transform.position) / 2f;
		}

		var steerableAxles = axleInfos.Where(x => x.steering);

		Vector3 fixedWheelCentre;
		var nonSteerableAxles = axleInfos.Where(x => !x.steering);
		if (nonSteerableAxles.Any())
		{
			fixedWheelCentre = nonSteerableAxles.Aggregate(Vector3.zero, (acc, val) => acc + val.centre) / nonSteerableAxles.Count();
		}
		else
		{
			//fixedWheelCentre = axleInfos.Aggregate(Vector3.zero, (acc, val) => acc + val.leftWheel.collider.transform.position + val.rightWheel.collider.transform.position) / (axleInfos.Count()*2);
			fixedWheelCentre = transform.position;
		}

		var furthestAxle = steerableAxles.MaxBy(a => Vector3.Distance(a.centre, fixedWheelCentre));

		WheelInfo innerWheel = null;
		if (steering > 0)
		{
			innerWheel = furthestAxle.rightWheel;
		}
		else if (steering < 0)
		{
			innerWheel = furthestAxle.leftWheel;
		}
		else
		{
			//reset
			foreach (var axle in steerableAxles)
			{
				axle.leftWheel.steering = 0;
				axle.rightWheel.steering = 0;
			}
			return;
		}

		// All this crazy math is to implement Ackermann steering to make sure both steering wheels are pointing along their respective turning circles

		float rearSteeringFactor = Vector3.Dot(transform.forward, furthestAxle.centre - transform.position) > 0 ? 1 : -1;

		//float steering = (maxSteeringAngle / (((speed / 100) * speedSensitiveSteering) + 1)) * inputs.xAxis;
		var speedSteeringFactor = (1 / (speed / 100 + 1));

		innerWheel.steering = steering * maxSteeringAngle * /*speedSteeringFactor **/ rearSteeringFactor;

		Vector3 intersection;

		var steeringNegator = steering > 0 ? 1 : -1;
		var linePoint1 = innerWheel.collider.transform.position;
		var linePoint2 = fixedWheelCentre;

		var lineVec1 = Quaternion.AngleAxis(innerWheel.steering, transform.up) * transform.right * steeringNegator;
		var lineVec2 = transform.right * steeringNegator;
		var lineVec3 = linePoint2 - linePoint1;

		Debug.DrawLine(linePoint1, linePoint1 + lineVec1);
		Debug.DrawLine(linePoint2, linePoint2 + lineVec2);

		var crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		var crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

		var planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		//is coplanar, and not parrallel
		var planar = Mathf.Abs(planarFactor);
		var parallel = crossVec1and2.sqrMagnitude;
		// planar was comparing to 0.0001f, but was breaking due to suspension misaligning the wheels
		if (planar < 0.1f && parallel > 0.0001f)
		{
			var s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
		}
		else
		{
			//reset
			foreach (var axle in steerableAxles)
			{
				axle.leftWheel.steering = 0;
				axle.rightWheel.steering = 0;
			}
			return;
		}

		Debug.DrawLine(intersection, intersection + Vector3.up);

		foreach (var axle in steerableAxles)
		{
			float wheelRearSteeringFactor = Vector3.Dot(transform.forward, axle.centre - transform.position) > 0 ? 1 : -1;
			axle.leftWheel.steering = Vector3.Angle(transform.right * steeringNegator, intersection - axle.leftWheel.collider.transform.position) * steeringNegator * wheelRearSteeringFactor;
			axle.rightWheel.steering = Vector3.Angle(transform.right * steeringNegator, intersection - axle.rightWheel.collider.transform.position) * steeringNegator * wheelRearSteeringFactor;
		}
	}

	private void UpdateColliders()
	{
		// UPDATE COLLIDERS WITH CALCULATED VALUES
		foreach (AxleInfo axleInfo in axleInfos)
		{
			// apply steering to colliders
			if (axleInfo.steering)
			{
				axleInfo.leftWheel.collider.steerAngle = axleInfo.leftWheel.steering;
				axleInfo.rightWheel.collider.steerAngle = axleInfo.rightWheel.steering;
			}

			// if axle has motor enabled apply torque values to wheels
			if (axleInfo.motor)
			{
				axleInfo.leftWheel.collider.motorTorque = axleInfo.leftWheel.torque;
				axleInfo.rightWheel.collider.motorTorque = axleInfo.rightWheel.torque;
			}
			else
			{
				axleInfo.torque = 0;
				axleInfo.leftWheel.collider.motorTorque = 0;
				axleInfo.rightWheel.collider.motorTorque = 0;
			}

			//apply brake torques
			axleInfo.leftWheel.collider.brakeTorque = axleInfo.leftWheel.brakeTorque;
			axleInfo.rightWheel.collider.brakeTorque = axleInfo.rightWheel.brakeTorque;
		}
	}

	private float[] CalculatePerAxleTorqueDistribution()
	{
		/* TORQUE DISTRIBUTION BETWEEN AXLES
		* maxAvailableTorque is spread onto axles depending on rotation speed
		*/

		//get rpm and motor value for each axle
		float[] rpms = new float[axleInfos.Count];
		bool[] motorEnabled = new bool[axleInfos.Count]; //enable torque distribution if axle.motor = true
		for (int i = 0; i < axleInfos.Count; i++)
		{
			rpms[i] = axleInfos[i].axleRpm;
			motorEnabled[i] = axleInfos[i].motor;
		}

		//put rpm values into torque splitter, result is torque array
		float[] torqueArr = TorqueSplit(motorEnabled, rpms, maxAvailableTorque);
		return torqueArr;
	}

	private void CalculateTorqueDistribution(AxleInfo axleInfo)
	{
		float[] wheelTorqueArr = TorqueSplit(
			new bool[] { true, true },
			new float[] { axleInfo.rightWheel.collider.rpm, axleInfo.leftWheel.collider.rpm },
			axleInfo.torque);

		if (axleInfo.rightWheel.collider.isGrounded)
			axleInfo.rightWheel.torque = wheelTorqueArr[0];
		else
			axleInfo.rightWheel.torque = 0;

		if (axleInfo.leftWheel.collider.isGrounded)
			axleInfo.leftWheel.torque = wheelTorqueArr[1];
		else
			axleInfo.leftWheel.torque = 0;

		totalTorque += axleInfo.torque;
	}

	private void CalculateSlips(AxleInfo axleInfo)
	{
		DetectWheelSlip(axleInfo.leftWheel, speed);
		DetectWheelSlip(axleInfo.rightWheel, speed);

		// set wheel slip if any exists to disable shifting
		wheelSlip = wheelSlip || axleInfo.leftWheel.slip || axleInfo.rightWheel.slip;
	}

	private void CalculateBrakeTorques(AxleInfo axleInfo)
	{
		if (brake)
		{
			if (Mathf.Abs(axleInfo.leftWheel.collider.rpm) < (speed * 2))
			{
				axleInfo.leftWheel.brakeTorque = 0;
			}
			else
			{
				axleInfo.leftWheel.brakeTorque = maxBrakeTorque / 4;
			}

			if (Mathf.Abs(axleInfo.rightWheel.collider.rpm) < (speed * 2))
			{
				axleInfo.rightWheel.brakeTorque = 0;
			}
			else
			{
				axleInfo.rightWheel.brakeTorque = maxBrakeTorque / 4;
			}
		}
		else
		{
			// release brakes when no brake applyed
			axleInfo.leftWheel.brakeTorque = 0;
			axleInfo.rightWheel.brakeTorque = 0;
		}
	}

	public void UpdateWheelVisuals(List<AxleInfo> axleInfos)
	{
		foreach (AxleInfo axleInfo in axleInfos)
		{
			UpdateWheel(axleInfo.leftWheel);
			UpdateWheel(axleInfo.rightWheel);
		}
	}

	public void UpdateWheel(WheelInfo wheel)
	{
		Vector3 position;
		Quaternion rotation;
		wheel.collider.GetWorldPose(out position, out rotation);
		wheel.visual.transform.position = position;
		wheel.visual.rotation = rotation;
	}

	public float[] TorqueSplit(bool[] enabled, float[] rpms, float availableTorque)
	{
		float[] torqueArr = new float[rpms.Length];
		bool lowRpmAlert = false;
		int enabledCount = 0;

		for (int i = 0; i < rpms.Length; i++)
		{
			rpms[i] = Mathf.Abs(rpms[i]);
			if (enabled[i])
			{
				lowRpmAlert = rpms[i] < 1 ? true : lowRpmAlert;
				enabledCount++;
			}
		}

		// if one axle dont calculate the rest
		if (enabledCount == 1)
		{
			for (int i = 0; i < torqueArr.Length; i++)
			{
				if (enabled[i]) torqueArr[i] = availableTorque;
			}
			// if multiple axes are enabled
		}
		else
		{
			// if low rpm dont calculate torque, spread it evenly
			if (lowRpmAlert)
			{
				for (int i = 0; i < torqueArr.Length; i++)
				{
					if (enabled[i]) torqueArr[i] = availableTorque / enabledCount;
				}
			}
			else
			{
				// if normal rpm spread torque by rpm
				float rpmSum = 0;
				for (int i = 0; i < rpms.Length; i++)
				{
					if (enabled[i]) rpmSum += rpms[i];
				}
				float[] torqueDemand = new float[rpms.Length];
				float torqueDemandSum = 0;
				for (int i = 0; i < rpms.Length; i++)
				{
					if (enabled[i])
					{
						torqueDemand[i] = (1 - (rpms[i] / rpmSum));
						torqueDemandSum += torqueDemand[i];
					}
				}
				for (int i = 0; i < rpms.Length; i++)
				{
					if (enabled[i])
					{
						torqueArr[i] = (torqueDemand[i] / torqueDemandSum) * availableTorque;
					}
				}
			}
		}

		return torqueArr;
	}

	public void DetectWheelSlip(WheelInfo wheel, float speed)
	{
		/* SLIP CALCULATION BASED ON WHEEL SPEED
		speed (m/s) = wheel.r * RPM * 0.10472 */
		if (Mathf.Abs(wheel.collider.radius * wheel.collider.rpm * 0.10472f) > (speed * 1.5f) + 3)
		{
			// wheel is faster than vehicle, slip occured
			wheel.slip = true;
			wheel.torque = 0; // acts out as mechanical loss when no power applied, spins down the wheel
		}

		wheel.slip = false;
	}

	public int countPoweredAxles()
	{
		int motorAxleCount = 0;
		foreach (AxleInfo axleInfo in axleInfos)
		{
			axleInfo.leftWheel.collider.ConfigureVehicleSubsteps(5, 20, 20); //fix for wheel physics glitch, rpm goes crazy
			axleInfo.rightWheel.collider.ConfigureVehicleSubsteps(5, 20, 20);
			if (axleInfo.motor)
			{
				motorAxleCount++;
			}
		}
		return motorAxleCount;
	}
}