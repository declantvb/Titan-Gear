using UnityEngine;

public class VehicleWheel : MonoBehaviour
{
	[SerializeField]
	private WheeledVehicle ParentVehicle;

	[SerializeField]
	public bool Drivable = true;

	[SerializeField]
	public bool Steerable = true;

	[SerializeField]
	private float Radius = 1f;

	[SerializeField]
	private bool Grounded;

	[SerializeField]
	private float GroundedThreshold;

	[SerializeField]
	private AnimationCurve LatForceVsLatFricFactor;

	private RaycastHit lastHit;
	private Vector3 forceLateral;
	private int totalWheelCount;

	// Use this for initialization
	private void Start()
	{
		ParentVehicle = GetComponentsInParent<WheeledVehicle>()[0]; //there will only be one
		totalWheelCount = ParentVehicle.GetComponentsInChildren<VehicleWheel>().Length;
	}

	// Update is called once per frame
	private void Update()
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

			ParentVehicle.rb.AddForceAtPosition(-forceLateral / totalWheelCount, transform.position, ForceMode.VelocityChange);          //Change 0.25f when changing weight transfer
		}
	}

	private void OnDrawGizmos()
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