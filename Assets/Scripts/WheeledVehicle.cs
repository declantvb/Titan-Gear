using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class WheeledVehicle : MonoBehaviour
{
	[Header("Ground Movement")]
	[SerializeField]
	float DriveForce = 100f;
	[SerializeField]
	float MaxGroundSpeed = 40f;
	[SerializeField]
	float MaxSteeringAngle = 35f;
	[SerializeField]
	AnimationCurve SpeedVsSteeringFactor;          //Defines how much steering ability is decreased as speed increases
	[SerializeField]
	AnimationCurve SpeedVsDownforce;
	[SerializeField]
	bool UseDownforce = true;
	[SerializeField]
	Transform CenterOfMass;

	public Rigidbody rb;

	[SerializeField]
	List<VehicleWheel> wheels;

	// Use this for initialization
	void Start()
	{
		rb = GetComponent<Rigidbody>();
		wheels = GetComponentsInChildren<VehicleWheel>().ToList();
	}

	// Update is called once per frame
	void Update()
	{
		float throttle = Input.GetAxis("Vertical");
		float steering = Input.GetAxis("Horizontal");

		//Update rigidbody centerofmass position
		rb.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

		//var handbraking = _state.Inputs.Handbrake > 0;
		//Wheel_BL.Handbraking = handbraking;
		//Wheel_BR.Handbraking = handbraking;
		//Wheel_FL.Handbraking = handbraking;
		//Wheel_FR.Handbraking = handbraking;

		//Add drive force   
		if (rb.velocity.magnitude < MaxGroundSpeed)
		{
			var drivableWheels = wheels.Where(x => x.Drivable);
			var driveForce = throttle * DriveForce * Time.deltaTime / drivableWheels.Count();
			foreach (var wheel in drivableWheels)
			{
				wheel.DriveWheel(driveForce);
			}
		}

		//Apply steering
		DoSteering(steering);

		//Debug.Log(rb.velocity.magnitude);

		//Apply downforce
		//if (UseDownforce)
		//{
		//	rb.AddForce(0, -SpeedVsDownforce.Evaluate(rb.velocity.magnitude), 0);
		//}

		//Count wheels on ground
		//int wheelsOnGround = GetCountOfWheelsOnGround();

		//Apply jump        
		//jumpCooldownElapsed += Time.deltaTime;
		//if (_state.Inputs.Jump > 0 && _previousState.Inputs.Jump <= 0)
		//{
		//	// if at least 3/4 wheels are touching ground, allow player to press jump button and apply jump force
		//	if (wheelsOnGround >= 3 && jumpCooldownElapsed >= JumpCooldownTime)
		//	{
		//		rb.AddForce(JumpForce * transform.up, ForceMode.Force);
		//		jumpCooldownElapsed = 0;
		//	}

		//	//If player is not on the ground (i.e, they have jumped once already, or are falling), engage flight mode when they press jump button
		//	else if (wheelsOnGround == 0)
		//	{
		//		FlightModeEngaged = true;
		//	}
		//}

		//Check if player has landed since last frame and do necessary things 
		//if (wheelsOnGround >= 4)
		//{
		//	//End flight mode when car is fully landed
		//	FlightModeEngaged = false;
		//}

		////Apply air movement if player has pressed space once to jump, and then another to engage flight mode
		//if (FlightModeEngaged)
		//{
		//	rb.angularDrag = AngDrag_FlightMode;

		//	if (_state.Inputs.Handbrake > 0)
		//	{
		//		rb.AddRelativeTorque(0, 0, -RollTorque * _state.Inputs.Horizontal, ForceMode.Force);
		//	}
		//	else
		//	{
		//		rb.AddRelativeTorque(0, YawTorque * _state.Inputs.Horizontal, 0, ForceMode.Force);
		//	}
		//	rb.AddRelativeTorque(-PitchTorque * _state.Inputs.Pitch, 0, 0, ForceMode.Force);
		//}
		//else
		//{
		//	rb.angularDrag = AngDrag_Grounded;
		//}
	}

	private void DoSteering(float steering)
	{
		var steerableWheels = wheels.Where(x => x.Steerable);

		VehicleWheel innerWheel = null;
		if (steering > 0)
		{
			innerWheel = steerableWheels.MaxBy(wheel => wheel.transform.localPosition.x);
		}
		else if (steering < 0)
		{
			innerWheel = steerableWheels.MinBy(wheel => wheel.transform.localPosition.x);
		}
		else
		{
			//reset
			foreach (var wheel in steerableWheels)
			{
				wheel.transform.localEulerAngles = Vector3.zero;
			}
			return;
		}

		// All this crazy math is to implement Ackermann steering to make sure both steering wheels are pointing along their respective turning circles
		float rearSteeringFactor = CenterOfMass.InverseTransformPoint(innerWheel.transform.position).z > 0 ? 1 : -1;

		innerWheel.transform.localEulerAngles = new Vector3(0, steering * MaxSteeringAngle * SpeedVsSteeringFactor.Evaluate(rb.velocity.magnitude) * rearSteeringFactor, 0);
		
		var nonSteerableWheels = wheels.Where(x => !x.Steerable);
		Vector3 fixedWheelCentre;
		if (nonSteerableWheels.Any())
		{
			fixedWheelCentre = nonSteerableWheels.Aggregate(Vector3.zero, (acc, val) => acc + val.transform.position) / nonSteerableWheels.Count(); 
		}
		else
		{
			fixedWheelCentre = wheels.Aggregate(Vector3.zero, (acc, val) => acc + val.transform.position) / wheels.Count();
		}
		Vector3 intersection;

		var steeringNegator = steering > 0 ? 1 : -1;
		var linePoint1 = innerWheel.transform.position;
		var linePoint2 = fixedWheelCentre;

		var lineVec1 = innerWheel.transform.right * steeringNegator;
		var lineVec2 = transform.right * steeringNegator;
		var lineVec3 = linePoint2 - linePoint1;

		Debug.DrawLine(linePoint1, linePoint1 + lineVec1);
		Debug.DrawLine(linePoint2, linePoint2 + lineVec2);

		var crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
		var crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

		var planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

		//is coplanar, and not parrallel
		if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
		{
			var s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
			intersection = linePoint1 + (lineVec1 * s);
		}
		else
		{
			//reset
			foreach (var wheel in steerableWheels)
			{
				wheel.transform.localEulerAngles = Vector3.zero;
			}
			return;
		}

		Debug.DrawLine(intersection, intersection + Vector3.up);

		foreach (var wheel in steerableWheels.Except(new List<VehicleWheel> { innerWheel }))
		{
			float wheelRearSteeringFactor = CenterOfMass.InverseTransformPoint(wheel.transform.position).z > 0 ? 1 : -1;
			var angle = Vector3.Angle(transform.right * steeringNegator, intersection - wheel.transform.position) * steeringNegator * wheelRearSteeringFactor;
			wheel.transform.localEulerAngles = new Vector3(0, angle, 0);
		}
	}
}
