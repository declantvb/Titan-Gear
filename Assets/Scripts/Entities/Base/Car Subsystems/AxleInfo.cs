using UnityEngine;

/* AXLE CLASS
 * Each instance contains two WheelInfo instances.
 * Distributes torque between front and rear.
 */

[System.Serializable]
public class AxleInfo
{
	public WheelInfo leftWheel;
	public WheelInfo rightWheel;

	[Tooltip("Does this axle recieve torque?")]
	public bool motor = true;

	[Tooltip("Does this axle steer?")]
	public bool steering = false;

	[HideInInspector]
	public float torque;

	// Axle slip
	[HideInInspector]
	public float axleRpm; //Abs()

	[HideInInspector]
	public bool axleSlip;

	internal Vector3 centre;
}