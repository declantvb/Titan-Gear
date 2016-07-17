using UnityEngine;
using System.Collections;

public class VehicleWheel : MonoBehaviour
{
	[SerializeField]
	WheeledVehicle ParentVehicle;

	[SerializeField]
	public bool Drivable = true;
	[SerializeField]
	public bool Steerable = true;
	[SerializeField]
	float Radius = 1f;
	[SerializeField]
	bool Grounded;
	[SerializeField]
	float GroundedThreshold;
	[SerializeField]
	AnimationCurve LatForceVsLatFricFactor;

	RaycastHit lastHit;
	Vector3 forceLateral;

	// Use this for initialization
	void Start()
	{
		ParentVehicle = GetComponentsInParent<WheeledVehicle>()[0]; //there will only be one
	}

	// Update is called once per frame
	void Update()
	{
		Grounded = false;

		//Raycast down to see if wheel is touching something
		if (Physics.Raycast(transform.position, -transform.up, out lastHit, Radius + GroundedThreshold))
		{
			Grounded = true;
		}

		//Do lateral friction
		if (Grounded)
		{
			float latFrictionFactor = LatForceVsLatFricFactor.Evaluate(forceLateral.magnitude);
			forceLateral = Vector3.Dot(ParentVehicle.rb.GetPointVelocity(transform.position), transform.right) * transform.right * latFrictionFactor;

			//if (Handbraking)
			//{
			//	latFrictionFactor = FrictionLateral_Drift;
			//}

			ParentVehicle.rb.AddForceAtPosition(-forceLateral * 0.25f, transform.position, ForceMode.VelocityChange);          //Change 0.25f when changing weight transfer
		}
	}

	void OnDrawGizmos()
	{
		if (Grounded)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, lastHit.point);
		}
		else
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, transform.position + (-transform.up * (Radius + GroundedThreshold)));
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawLine(transform.position, transform.position - forceLateral);
	}

	/// <summary>
	/// Used to drive wheel. Positive force is forward rotation, negative is reverse.
	/// </summary>
	/// <param name="force"></param>
	public void DriveWheel(float force)
	{
		if (Grounded)
		{
			ParentVehicle.rb.AddForceAtPosition(transform.forward * force, transform.position, ForceMode.Force);
		}
	}
}
