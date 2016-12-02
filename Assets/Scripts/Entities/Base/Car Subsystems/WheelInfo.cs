using UnityEngine;

/* WHEEL CLASS
 * Each wheel has one instance, passes final params to colliders and transfoms.
 * Distributes torque between left and right.
 */

[System.Serializable]
public class WheelInfo
{
	[Tooltip("Wheel collider corresponding to the wheel. One of the parents has to have Rigidbody for it to work!")]
	public WheelCollider collider;

	[Tooltip("Visual wheel transform corresponding to the wheel")]
	public Transform visual;

	[HideInInspector]
	public float torque;

	[HideInInspector]
	public float brakeTorque;

	[HideInInspector]
	public float steering;

	[HideInInspector]
	public bool slip;
}