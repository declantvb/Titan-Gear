using System.Linq;
using UnityEngine;

public class Missile : MonoBehaviour
{
	public float TimeToLive = 30f;
	private Rigidbody _rigidbody;
	private Vector3 previousPos;
	private bool destroyed;
	private LayerMask layerMask;

	public GameObject ExplosionPrefab;
	public float DamageModifier = 50f;
	public float BlastRadius = 2f;
	public float ThrustForce;

	public float safetyTime = 0.2f;

	public Transform Target;
	public float alignmentSpeed = 0.025f;
	public float alignmentDamping = 0.2f;

	private readonly VectorPid angularVelocityController = new VectorPid(33.7766f, 0, 0.2553191f);
	private readonly VectorPid headingController = new VectorPid(9.244681f, 0, 0.06382979f);

	public void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		previousPos = transform.position;

		layerMask = LayerMaskExtensions.Create(LayerMask.NameToLayer("Bullets")).Inverse();
	}

	public void FixedUpdate()
	{
		if (destroyed) return;

		TimeToLive -= Time.fixedDeltaTime;
		if (TimeToLive <= 0)
		{
			Destroy(gameObject);
		}

		RaycastHit rayHit;
		if (Physics.Linecast(previousPos, transform.position, out rayHit, layerMask))
		{
			Hit(rayHit.point, rayHit.collider);
			Destroy(gameObject);
			destroyed = true;
			return;
		}

		safetyTime -= Time.fixedDeltaTime;
		if (safetyTime <= 0)
		{
			if (Target != null)
			{
				AlignToTarget();
			}

			var drag = Vector3.Cross(Vector3.Cross(_rigidbody.velocity, transform.forward), transform.forward) * 10;
			_rigidbody.AddForce(drag);

			//thrust
			_rigidbody.AddForce(transform.forward * ThrustForce, ForceMode.Force);
		}

		previousPos = transform.position;
	}

	private void AlignToTarget()
	{
		//from http://answers.unity3d.com/questions/199055/addtorque-to-rotate-rigidbody-to-look-at-a-point.html

		var angularVelocityError = _rigidbody.angularVelocity * -1;
		var angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
		_rigidbody.AddTorque(angularVelocityCorrection);

		var targetRb = Target.GetComponent<Rigidbody>();
		var targetVelocity = targetRb.velocity;
		var targetPosition = Target.position + Target.TransformVector(targetRb.centerOfMass);

		var timeToTarget = (targetPosition - transform.position).magnitude * 0.01f;

		var targetPositionAtHit = targetPosition + targetVelocity * timeToTarget;

		var currentHeading = transform.forward;
		var desiredHeading = targetPositionAtHit - transform.position;

		var headingError = Vector3.Cross(currentHeading, desiredHeading);
		var headingCorrection = headingController.Update(headingError, Time.deltaTime);

		_rigidbody.AddTorque(headingCorrection.normalized * Mathf.Min(headingCorrection.magnitude, 5000f * Time.deltaTime));
	}

	public void Hit(Vector3 point, Collider other)
	{
		var explosion = (GameObject)Instantiate(ExplosionPrefab, point, Quaternion.identity);
		var hits = Physics.OverlapSphere(point, BlastRadius);
		var enemyHits = hits.Select(x => x.GetComponentInParent<Health>()).Distinct();

		var directHit = other.GetComponentInParent<Health>();
		if (directHit)
		{
			directHit.YaGotShot(DamageModifier);
		}

		foreach (var enemy in enemyHits)
		{
			if (enemy != null && enemy != directHit)
			{
				var dist = (point - enemy.transform.position).magnitude;
				var effect = 1 - (dist / BlastRadius);
				enemy.YaGotShot(Mathf.Max(0, effect) * DamageModifier);
			}
		}

		Destroy(explosion, 5f);
	}
}

public class VectorPid
{
	public float pFactor, iFactor, dFactor;

	private Vector3 integral;
	private Vector3 lastError;

	public VectorPid(float pFactor, float iFactor, float dFactor)
	{
		this.pFactor = pFactor;
		this.iFactor = iFactor;
		this.dFactor = dFactor;
	}

	public Vector3 Update(Vector3 currentError, float timeFrame)
	{
		integral += currentError * timeFrame;
		var deriv = (currentError - lastError) / timeFrame;
		lastError = currentError;
		return currentError * pFactor
			+ integral * iFactor
			+ deriv * dFactor;
	}
}