using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* AXLE CLASS
 * Each instance contains two WheelInfo instances.
 * Distributes torque between front and rear.
 */
[System.Serializable]
public class TrailerAxleInfo {
	public WheelInfo leftWheel;
	public WheelInfo rightWheel;
}

/* WHEEL CLASS
 * Each wheel has one instance, passes final params to colliders and transfoms. 
 * Distributes torque between left and right.
 */
[System.Serializable]
public class TrailerWheelInfo {
	[Tooltip("Wheel collider corresponding to the wheel. One of the parents has to have Rigidbody for it to work!")]
	public WheelCollider collider;
	[Tooltip("Visual wheel transform corresponding to the wheel")]
	public Transform visual;

	[HideInInspector]
	public float brakeTorque;
}

public class TrailerController : MonoBehaviour {

	[Tooltip("List of axles and wheels a vehicle has. Up to 10 axles, 2 wheels each.")]
	public List<TrailerAxleInfo> trailerAxleInfos; 

	public float trailerBrakeTorque = 100000;
	float axleNo;

	Joint joint;
	GameObject tractor;

	// Use this for initialization
	void Start () {
		joint = GetComponent<Joint>();
		tractor = joint.connectedBody.gameObject;
		axleNo = (trailerAxleInfos.Count*2);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		UpdateWheelVisuals(trailerAxleInfos);
	}

	public void UpdateWheelVisuals(List<TrailerAxleInfo> trailerAxleInfos){
		foreach(TrailerAxleInfo axleInfo in trailerAxleInfos){
			UpdateWheel(axleInfo.leftWheel);
			UpdateWheel(axleInfo.rightWheel);
		}
	}
	
	public void UpdateWheel(WheelInfo wheel){
		Vector3 position;
		Quaternion rotation;
		wheel.collider.GetWorldPose(out position, out rotation);
		wheel.visual.transform.position = position;
		wheel.visual.rotation = rotation;

		//if tractor is braking, brake
		if(tractor.GetComponent<WheeledVehicle>().brake){
			wheel.collider.brakeTorque = trailerBrakeTorque / axleNo;
		} else {
			wheel.collider.brakeTorque = 0;
			wheel.collider.motorTorque = 5;
		}
	}
}
