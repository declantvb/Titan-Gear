using UnityEngine;

public class BipedVehicle : MonoBehaviour
{
	[SerializeField]
	private float DriveForce = 100f;

	[SerializeField]
	private float TorqueFactor;

	[SerializeField]
	private float FrictionFactor;

	[SerializeField]
	private float MaxGroundSpeed = 40f;

	[SerializeField]
	private Transform CenterOfMass;

	[SerializeField]
	private float FootMoveDistance;

	[SerializeField]
	private float StrideHeight;

	[SerializeField]
	private float WalkSpeedFactor;

	public Rigidbody rb;

	[SerializeField]
	private Transform leftTarget;

	[SerializeField]
	private Transform rightTarget;

	[SerializeField]
	private GameObject ikTargetPrefab;

	private Transform leftIkTarget;
	private Transform rightIkTarget;
	private Vector3 leftPrevIKTarget;
	private Vector3 rightPrevIKTarget;

	// Use this for initialization
	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		leftTarget = transform.FindChild("left target");
		rightTarget = transform.FindChild("right target");

		leftIkTarget = ((GameObject)Instantiate(ikTargetPrefab, leftTarget.transform.position, Quaternion.identity)).transform;
		rightIkTarget = ((GameObject)Instantiate(ikTargetPrefab, rightTarget.transform.position, Quaternion.identity)).transform;

		var leftSolver = leftTarget.GetComponent<SimpleIKSolver>();
		leftSolver.Target = leftIkTarget;
		leftSolver.IsActive = true;
		var rightSolver = rightTarget.GetComponent<SimpleIKSolver>();
		rightSolver.Target = rightIkTarget;
		rightSolver.IsActive = true;
	}

	// Update is called once per frame
	private void Update()
	{
		float throttle = Input.GetAxis("Vertical");
		float steering = Input.GetAxis("Horizontal");

		//Update rigidbody centerofmass position
		rb.centerOfMass = transform.InverseTransformPoint(CenterOfMass.position);

		//Add drive force
		if (rb.velocity.magnitude < MaxGroundSpeed)
		{
			rb.AddForce(transform.forward * DriveForce * throttle, ForceMode.Force);
		}

		rb.AddTorque(0, steering * TorqueFactor, 0, ForceMode.Force);

		Debug.DrawLine(transform.position, transform.position + rb.velocity);
		var forceLateral = Vector3.Dot(rb.velocity, transform.right) * -transform.right * FrictionFactor;
		Debug.DrawLine(transform.position + transform.up, transform.position + transform.up + forceLateral);
		rb.AddForce(forceLateral, ForceMode.Force);

		//do feet duty cycle
		var leftHeight = Mathf.Max(0, Mathf.Sin(Time.time * WalkSpeedFactor));
		if (leftHeight > 0 && (throttle > 0 || Vector3.Distance(leftIkTarget.transform.position, leftTarget.transform.position) > 0.1f))
		{
			if (leftPrevIKTarget == Vector3.zero)
			{
				leftPrevIKTarget = leftIkTarget.transform.position;
			}
			var leftCurrentOffset = leftPrevIKTarget - leftTarget.transform.position;
			Debug.DrawLine(leftTarget.transform.position, leftTarget.transform.position + leftCurrentOffset);
			var leftOffset = Vector3.Lerp(leftCurrentOffset, transform.forward * throttle, (Time.time % (Mathf.PI * 2 / WalkSpeedFactor)) / Mathf.PI * WalkSpeedFactor);
			leftIkTarget.transform.position = leftTarget.transform.position + transform.up * leftHeight * StrideHeight + leftOffset;
		}
		else
		{
			leftPrevIKTarget = Vector3.zero;
		}

		var rightHeight = Mathf.Max(0, -Mathf.Sin(Time.time * WalkSpeedFactor));
		if (rightHeight > 0 && (throttle > 0 || Vector3.Distance(rightIkTarget.transform.position, rightTarget.transform.position) > 0.1f))
		{
			if (rightPrevIKTarget == Vector3.zero)
			{
				rightPrevIKTarget = rightIkTarget.transform.position;
			}
			var rightCurrentOffset = rightPrevIKTarget - rightTarget.transform.position;
			Debug.DrawLine(rightTarget.transform.position, rightTarget.transform.position + rightCurrentOffset);
			var rightOffset = Vector3.Lerp(rightCurrentOffset, transform.forward * throttle, ((Time.time + (Mathf.PI / 4f)) % (Mathf.PI * 2 / WalkSpeedFactor)) / Mathf.PI * WalkSpeedFactor);
			rightIkTarget.transform.position = rightTarget.transform.position + transform.up * rightHeight * StrideHeight + rightOffset;
		}
		else
		{
			rightPrevIKTarget = Vector3.zero;
		}
	}
}