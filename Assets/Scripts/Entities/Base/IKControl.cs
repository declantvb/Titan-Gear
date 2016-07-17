using UnityEngine;
using System.Collections;

public class IKControl : MonoBehaviour
{
	public Transform forearm;
	public Transform hand;
	public Transform target;
 
	public float transition = 1.0f;
	public float elbowAngle;

	private Transform armIK;
	private Transform armRotation;

	private float upperArmLength ;
	private float forearmLength ;
	private float armLength ;

	public void Start()
	{
		var armIKGameObject = new GameObject("Arm IK");
		armIK = armIKGameObject.transform;
		armIK.parent = transform;
		var armRotationGameObject = new GameObject("Arm Rotation");
		armRotation = armRotationGameObject.transform;
		armRotation.parent = armIK;
		upperArmLength = Vector3.Distance(transform.position, forearm.position);
		forearmLength = Vector3.Distance(forearm.position, hand.position);
		armLength = upperArmLength + forearmLength;
	}

	public void LateUpdate()
	{
		//Store rotation before IK.
		var storeUpperArmRotation = transform.rotation;
		var storeForearmRotation = forearm.rotation;

		//Upper Arm looks target.
		armIK.position = transform.position;
		armIK.LookAt(forearm);
		armRotation.position = transform.position;
		armRotation.rotation = transform.rotation;
		armIK.LookAt(target);
		transform.rotation = armRotation.rotation;

		//Upper Arm IK angle.
		var targetDistance = Vector3.Distance(transform.position, target.position);
		targetDistance = Mathf.Min(targetDistance, armLength - 0.00001f);
		var adjacent = ((upperArmLength * upperArmLength) - (forearmLength * forearmLength) + (targetDistance * targetDistance)) / (2 * targetDistance);
		var angle = Mathf.Acos(adjacent / upperArmLength) * Mathf.Rad2Deg;
		transform.RotateAround(transform.position, transform.forward, -angle);

		//Forearm looks target.
		armIK.position = forearm.position;
		armIK.LookAt(hand);
		armRotation.position = forearm.position;
		armRotation.rotation = forearm.rotation;
		armIK.LookAt(target);
		forearm.rotation = armRotation.rotation;

		//Elbow angle.
		transform.RotateAround(transform.position, target.position - transform.position, elbowAngle);

		//Transition IK rotations with animation rotation.
		transition = Mathf.Clamp01(transition);
		transform.rotation = Quaternion.Slerp(storeUpperArmRotation, transform.rotation, transition);
		forearm.rotation = Quaternion.Slerp(storeForearmRotation, forearm.rotation, transition);
	}
}
